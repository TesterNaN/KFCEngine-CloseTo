using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Microsoft.DirectX.DirectSound;

namespace KidFansChannel.SoundCore.Ogg
{
	public class OggPlayer : IDisposable
	{
		private const int WaitTimeout = 5000;

		private const int bitsPerSample = 16;

		private static Device directSoundDevice;

		private static bool _globalFocus;

		private static bool initialized;

		private bool stopNow;

		private EventWaitHandle ewh = new EventWaitHandle(false, EventResetMode.ManualReset);

		private Thread playbackThread;

		private SecondaryBuffer SecBuf;

		private int bufferVolume;

		private float _length;

		private float _position;

		private float _repeatPosition = float.NaN;

		private volatile IntPtr vorbisFile = IntPtr.Zero;

		private int bufferPosition;

		private int bufferRate = 1;

		private float SecBufHoldThisManySeconds = 1.2f;

		private bool _playing;

		private object thislock = new object();

		public static bool GlobalFocus
		{
			get
			{
				return _globalFocus;
			}
			set
			{
				_globalFocus = value;
			}
		}

		public float Volume
		{
			get
			{
				return (float)Math.Pow(10.0, (float)bufferVolume / 2000f);
			}
			set
			{
				int channels;
				if (value == 0f || (channels = (int)(2000.0 * Math.Log10(value))) < -10000)
				{
					channels = -10000;
				}
				bufferVolume = channels;
				if (SecBuf != null)
				{
					SecBuf.Volume = bufferVolume;
				}
			}
		}

		public float Length
		{
			get
			{
				return _length;
			}
		}

		public float Position
		{
			get
			{
				float channels = _position;
				if (SecBuf != null)
				{
					int playPosition = SecBuf.PlayPosition;
					channels = ((playPosition < bufferPosition) ? (channels - (float)(bufferPosition - playPosition) / (float)bufferRate) : (channels - (SecBufHoldThisManySeconds - (float)(playPosition - bufferPosition) / (float)bufferRate)));
				}
				return channels;
			}
			protected set
			{
				_position = value;
			}
		}

		public float RepeatPosition
		{
			get
			{
				return _repeatPosition;
			}
			protected set
			{
				_repeatPosition = value;
			}
		}

		public bool Playing
		{
			get
			{
				return _playing;
			}
		}

		public event EventHandler<OggPlayEventArgs> Stopped;

		static OggPlayer()
		{
			initialized = false;
		}

		public static void Initial(Control owner, bool wantGlobalFocus, int deviceIndex)
		{
			if (initialized)
			{
				throw new InvalidOperationException("Already initialized.");
			}
			DevicesCollection devicesCollection = new DevicesCollection();
			if (devicesCollection.Count > deviceIndex)
			{
				if (null == (directSoundDevice = new Device(devicesCollection[deviceIndex].DriverGuid)))
				{
					throw new Exception("Unable to create Microsoft.DirectX.DirectSound Device object.");
				}
			}
			else if (null == (directSoundDevice = new Device(DSoundHelper.DefaultPlaybackDevice)))
			{
				throw new Exception("Unable to create Microsoft.DirectX.DirectSound Device object.");
			}
			directSoundDevice.SetCooperativeLevel(owner, (owner == null) ? CooperativeLevel.Normal : CooperativeLevel.Priority);
			_globalFocus = wantGlobalFocus;
			initialized = true;
		}

		public static void Final()
		{
			Thread.Sleep(20);
			if (directSoundDevice != null)
			{
				if (!directSoundDevice.Disposed)
				{
					directSoundDevice.Dispose();
				}
				directSoundDevice = null;
			}
		}

		public OggPlayer(string fileName)
		{
			Open(fileName);
		}

		public OggPlayer(Stream stream)
		{
			byte[] decodeBuffer = new byte[stream.Length];
			using (BinaryReader binaryReader = new BinaryReader(stream))
			{
				binaryReader.Read(decodeBuffer, 0, (int)stream.Length);
			}
			Open(decodeBuffer);
			decodeBuffer = null;
		}

		protected unsafe void Open(string fileName)
		{
			if (vorbisFile != IntPtr.Zero)
			{
				return;
			}
			lock (thislock)
			{
				void* ptr = null;
				int channels = 0;
				try
				{
					if (NativeMethods.OpenFile(fileName, &ptr) != 0)
					{
						return;
					}
					_length = (float)NativeMethods.LengthTime(ptr);
				}
				catch (Exception)
				{
					throw;
				}
				vorbisFile = (IntPtr)ptr;
			}
		}

		protected unsafe void Open(byte[] oggData)
		{
			if (vorbisFile != IntPtr.Zero)
			{
				return;
			}
			lock (thislock)
			{
				void* ptr = null;
				int channels = 0;
				try
				{
					if (NativeMethods.OpenStream(oggData, oggData.Length, &ptr) != 0)
					{
						return;
					}
					_length = (float)NativeMethods.LengthTime(ptr);
				}
				catch (Exception)
				{
					throw;
				}
				vorbisFile = (IntPtr)ptr;
			}
		}

		public unsafe void Close()
		{
			lock (thislock)
			{
				if (!stopNow)
				{
					stopNow = true;
					ewh.Set();
					ewh.Reset();
				}
				playbackThread = null;
				if (vorbisFile == IntPtr.Zero)
				{
					return;
				}
				int channels = 0;
				void* vf_ptr = (void*)vorbisFile;
				vorbisFile = IntPtr.Zero;
				try
				{
					if (NativeMethods.Final(vf_ptr) != 0)
					{
						return;
					}
				}
				catch
				{
				}
				if (SecBuf != null)
				{
					SecBuf.Dispose();
					SecBuf = null;
				}
			}
			GC.Collect();
		}

		public void Play(float position, float repeatPosition)
		{
			if (vorbisFile == IntPtr.Zero)
			{
				return;
			}
			if (_playing)
			{
				Stop();
				while (_playing)
				{
					Thread.Sleep(20);
				}
			}
			playbackThread = new Thread(PlayOggDecodeThreadProc);
			if (playbackThread != null)
			{
				_position = position;
				_repeatPosition = repeatPosition;
				stopNow = false;
				playbackThread.Priority = ThreadPriority.Highest;
				playbackThread.Start();
			}
		}

		public void Play()
		{
			Play(0f, float.NaN);
		}

		public void Stop()
		{
			stopNow = true;
		}

		public void Dispose()
		{
			Close();
			GC.SuppressFinalize(this);
		}

		~OggPlayer()
		{
			Dispose();
		}

		public unsafe void PlayOggDecodeThreadProc()
		{
			_playing = true;
			if (vorbisFile == IntPtr.Zero)
			{
				_playing = false;
				return;
			}
			SecBuf = null;
			BufferDescription bufferDescription = null;
			Notify notify = null;
			MemoryStream memoryStream = null;
			AutoResetEvent autoResetEvent = new AutoResetEvent(false);
			AutoResetEvent autoResetEvent2 = new AutoResetEvent(false);
			AutoResetEvent autoResetEvent3 = new AutoResetEvent(false);
			try
			{
				int channels = 0;
				int samplingRate = 0;
				int lastChannels = 0;
				int lastSamplingRate = 0;
				int bytesPerSecond = 0;
				int blockAlign = 0;
				int written = 0;
				int bufferSize = 0;
				int writePos = 0;
				int playPos = 0;
				WaitHandle[] waitHandles = new WaitHandle[4] { autoResetEvent, autoResetEvent2, autoResetEvent3, ewh };
				bool keepDecoding = true;
				bool playbackDone = false;
				bool firstFill = true;
				byte[] decodeBuffer = new byte[4096];
				memoryStream = new MemoryStream();
				WaveFormat format = new WaveFormat();
				lock (thislock)
				{
					if (!(vorbisFile != IntPtr.Zero))
					{
						return;
					}
					NativeMethods.SeekTime((void*)vorbisFile, _position);
				}
				while (true)
				{
					if (stopNow)
					{
						if (null != SecBuf)
						{
							SecBuf.Stop();
						}
						break;
					}
					int decodeResult;
					fixed (byte* buf_out = &decodeBuffer[0])
					{
						int sampleCount = 0;
						int channelMask = 0;
						lock (thislock)
						{
							if (!(vorbisFile != IntPtr.Zero))
							{
								return;
							}
							decodeResult = NativeMethods.DecodePacket((void*)vorbisFile, buf_out, decodeBuffer.Length, 16, &channels, &samplingRate, &sampleCount, &channelMask);
							_position = (float)NativeMethods.TellTime((void*)vorbisFile);
						}
					}
					if (decodeResult == 0)
					{
						if (float.IsNaN(_repeatPosition))
						{
							if (!firstFill)
							{
								if (playPos <= writePos)
								{
									memoryStream.SetLength(bufferSize - writePos);
									memoryStream.Position = 0L;
									SecBuf.Write(writePos, memoryStream, bufferSize - writePos, LockFlag.None);
									memoryStream.SetLength(playPos);
									memoryStream.Position = 0L;
									SecBuf.Write(0, memoryStream, playPos, LockFlag.None);
								}
								else
								{
									memoryStream.SetLength(playPos - writePos);
									memoryStream.Position = 0L;
									SecBuf.Write(writePos, memoryStream, playPos - writePos, LockFlag.None);
								}
							}
							playbackDone = true;
						}
						else
						{
							fixed (byte* buf_out2 = &decodeBuffer[0])
							{
								int unused14 = 0;
								int unused15 = 0;
								lock (thislock)
								{
									if (!(vorbisFile != IntPtr.Zero))
									{
										return;
									}
									NativeMethods.SeekTime((void*)vorbisFile, _repeatPosition);
									decodeResult = NativeMethods.DecodePacket((void*)vorbisFile, buf_out2, decodeBuffer.Length, 16, &channels, &samplingRate, &unused14, &unused15);
									_position = (float)NativeMethods.TellTime((void*)vorbisFile);
								}
							}
						}
					}
					if (keepDecoding && playbackDone)
					{
						if (Stopped != null)
						{
							OggPlayEventArgs e = new OggPlayEventArgs(false, "The Ogg Vorbis file or memory stream has no usable data.", false);
							Stopped(this, e);
						}
						_playing = false;
						return;
					}
					if (!keepDecoding && (channels != lastChannels || samplingRate != lastSamplingRate))
					{
						if (null != SecBuf)
						{
							SecBuf.Stop();
						}
						if (Stopped != null)
						{
							OggPlayEventArgs e2 = new OggPlayEventArgs(false, "The Ogg Vorbis file has a format change and DirectSound can't handle this.", false);
							Stopped(this, e2);
						}
						_playing = false;
						return;
					}
					if (keepDecoding)
					{
						blockAlign = channels * 2;
						bytesPerSecond = samplingRate * blockAlign;
						int unused16 = (int)((float)samplingRate * SecBufHoldThisManySeconds);
						SecBufHoldThisManySeconds = (float)unused16 / (float)samplingRate;
						bufferRate = bytesPerSecond;
						format.AverageBytesPerSecond = bytesPerSecond;
						format.BitsPerSample = 16;
						format.BlockAlign = (short)blockAlign;
						format.Channels = (short)channels;
						format.SamplesPerSecond = samplingRate;
						format.FormatTag = WaveFormatTag.Pcm;
						bufferDescription = new BufferDescription();
						bufferDescription.Format = format;
						bufferSize = (bufferDescription.BufferBytes = unused16 * blockAlign);
						bufferDescription.CanGetCurrentPosition = true;
						bufferDescription.ControlPositionNotify = true;
						bufferDescription.ControlVolume = true;
						bufferDescription.ControlPan = true;
						bufferDescription.GlobalFocus = _globalFocus;
						try
						{
							if (SecBuf == null)
							{
								SecBuf = new SecondaryBuffer(bufferDescription, directSoundDevice);
							}
							SecBuf.Volume = bufferVolume;
							SecBuf.Pan = 0;
						}
						catch (ControlUnavailableException)
						{
							if (SecBuf != null)
							{
								if (!SecBuf.Disposed)
								{
									SecBuf.Dispose();
								}
								SecBuf = null;
							}
						}
						if (SecBuf == null)
						{
							bufferDescription.ControlVolume = false;
							bufferDescription.ControlPan = false;
							SecBuf = new SecondaryBuffer(bufferDescription, directSoundDevice);
						}
						BufferPositionNotify[] tempBuffer = new BufferPositionNotify[3];
						tempBuffer[0].Offset = 0;
						tempBuffer[0].EventNotifyHandle = autoResetEvent.SafeWaitHandle.DangerousGetHandle();
						tempBuffer[1].Offset = unused16 / 3 * blockAlign;
						tempBuffer[1].EventNotifyHandle = autoResetEvent2.SafeWaitHandle.DangerousGetHandle();
						tempBuffer[2].Offset = unused16 * 2 / 3 * blockAlign;
						tempBuffer[2].EventNotifyHandle = autoResetEvent3.SafeWaitHandle.DangerousGetHandle();
						notify = new Notify(SecBuf);
						notify.SetNotificationPositions(tempBuffer);
						lastChannels = channels;
						lastSamplingRate = samplingRate;
						keepDecoding = false;
					}
					else if (playbackDone)
					{
						if (firstFill)
						{
							SecBuf.Play(0, BufferPlayFlags.Looping);
						}
						int wrapCount = 0;
						int playSnapshot = playPos;
						while (!stopNow)
						{
							int playPosition = SecBuf.PlayPosition;
							if (playPosition < playSnapshot)
							{
								wrapCount++;
								memoryStream.SetLength(bufferSize - playSnapshot);
								memoryStream.Position = 0L;
								SecBuf.Write(playSnapshot, memoryStream, bufferSize - playSnapshot, LockFlag.None);
								memoryStream.SetLength(playPosition);
								memoryStream.Position = 0L;
								SecBuf.Write(0, memoryStream, playPosition, LockFlag.None);
							}
							else if (playPosition > playSnapshot)
							{
								memoryStream.SetLength(playPosition - playSnapshot);
								memoryStream.Position = 0L;
								SecBuf.Write(playSnapshot, memoryStream, playPosition - playSnapshot, LockFlag.None);
							}
							if (playPos <= writePos)
							{
								if (playPosition >= writePos || wrapCount > 0)
								{
									break;
								}
							}
							else if ((playPosition < playPos && playPosition >= writePos) || wrapCount > 1)
							{
								break;
							}
							Thread.Sleep(20);
							playSnapshot = playPosition;
						}
						if (null != SecBuf)
						{
							SecBuf.Stop();
						}
						break;
					}
					memoryStream.SetLength(0L);
					memoryStream.Write(decodeBuffer, 0, decodeResult);
					memoryStream.Position = 0L;
					written = 0;
					if (firstFill)
					{
						int chunkLength = (int)Math.Min(memoryStream.Length, bufferSize - writePos);
						if (chunkLength > 0)
						{
							SecBuf.Write(writePos, memoryStream, chunkLength, LockFlag.None);
							writePos += chunkLength;
							written += chunkLength;
						}
						if (bufferSize != writePos)
						{
							continue;
						}
						firstFill = false;
						writePos = 0;
						SecBuf.Play(0, BufferPlayFlags.Looping);
						Thread.Sleep(0);
					}
					while (written < memoryStream.Length && !stopNow)
					{
						int spaceLeft = 0;
						int playPosition2 = SecBuf.PlayPosition;
						int writePosition = SecBuf.WritePosition;
						if (writePos < playPosition2 && (writePosition >= playPosition2 || writePosition < writePos))
						{
							spaceLeft = playPosition2 - writePos;
						}
						else if (writePos > writePosition && writePosition >= playPosition2)
						{
							spaceLeft = bufferSize - writePos + playPosition2;
						}
						if (spaceLeft > memoryStream.Length / 2)
						{
							spaceLeft = (int)Math.Min(spaceLeft, memoryStream.Length - written);
							memoryStream.Position = written;
							SecBuf.Write(writePos, memoryStream, spaceLeft, LockFlag.None);
							writePos = (writePos + spaceLeft) % bufferSize;
							playPos = playPosition2;
							written += spaceLeft;
						}
						else
						{
							WaitHandle.WaitAny(waitHandles, new TimeSpan(0, 0, 5), true);
						}
					}
					bufferPosition = writePos;
				}
				if (Stopped != null)
				{
					OggPlayEventArgs e3 = new OggPlayEventArgs(stopNow);
					Stopped(this, e3);
				}
				_playing = false;
			}
			catch (NoDriverException ex2)
			{
				if (Stopped != null)
				{
					OggPlayEventArgs e4 = new OggPlayEventArgs(false, ex2.Message, false);
					Stopped(this, e4);
				}
				_playing = false;
			}
			catch (ControlUnavailableException ex3)
			{
				if (Stopped != null)
				{
					OggPlayEventArgs e5 = new OggPlayEventArgs(false, ex3.Message, false);
					Stopped(this, e5);
				}
				_playing = false;
			}
			catch (ArgumentException ex4)
			{
				if (Stopped != null)
				{
					OggPlayEventArgs e6 = new OggPlayEventArgs(false, ex4.Message, false);
					Stopped(this, e6);
				}
				_playing = false;
			}
			catch (Exception ex5)
			{
				if (Stopped != null)
				{
					OggPlayEventArgs e7 = new OggPlayEventArgs(false, ex5.Message, false);
					Stopped(this, e7);
				}
				_playing = false;
			}
			finally
			{
				if (autoResetEvent != null)
				{
					if (autoResetEvent.SafeWaitHandle != null)
					{
						autoResetEvent.SafeWaitHandle.Close();
					}
					autoResetEvent.Close();
					autoResetEvent = null;
				}
				if (autoResetEvent2 != null)
				{
					if (autoResetEvent2.SafeWaitHandle != null)
					{
						autoResetEvent2.SafeWaitHandle.Close();
					}
					autoResetEvent2.Close();
					autoResetEvent2 = null;
				}
				if (autoResetEvent3 != null)
				{
					if (autoResetEvent3.SafeWaitHandle != null)
					{
						autoResetEvent3.SafeWaitHandle.Close();
					}
					autoResetEvent3.Close();
					autoResetEvent3 = null;
				}
				if (null != SecBuf)
				{
					if (!SecBuf.Disposed)
					{
						SecBuf.Dispose();
					}
					SecBuf = null;
				}
				if (memoryStream != null)
				{
					memoryStream.Close();
					memoryStream = null;
				}
				if (bufferDescription != null)
				{
					bufferDescription.Dispose();
					bufferDescription = null;
				}
				if (null != notify)
				{
					if (!notify.Disposed)
					{
						notify.Dispose();
					}
					notify = null;
				}
			}
		}
	}
}
