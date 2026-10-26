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
				int num;
				if (value == 0f || (num = (int)(2000.0 * Math.Log10(value))) < -10000)
				{
					num = -10000;
				}
				bufferVolume = num;
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
				float num = _position;
				if (SecBuf != null)
				{
					int playPosition = SecBuf.PlayPosition;
					num = ((playPosition < bufferPosition) ? (num - (float)(bufferPosition - playPosition) / (float)bufferRate) : (num - (SecBufHoldThisManySeconds - (float)(playPosition - bufferPosition) / (float)bufferRate)));
				}
				return num;
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
			byte[] array = new byte[stream.Length];
			using (BinaryReader binaryReader = new BinaryReader(stream))
			{
				binaryReader.Read(array, 0, (int)stream.Length);
			}
			Open(array);
			array = null;
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
				int num = 0;
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
				int num = 0;
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
				int num = 0;
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
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				int num4 = 0;
				int num5 = 0;
				int num6 = 0;
				int num7 = 0;
				int num8 = 0;
				int num9 = 0;
				int num10 = 0;
				WaitHandle[] waitHandles = new WaitHandle[4] { autoResetEvent, autoResetEvent2, autoResetEvent3, ewh };
				bool flag = true;
				bool flag2 = false;
				bool flag3 = true;
				byte[] array = new byte[4096];
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
					int num13;
					fixed (byte* buf_out = &array[0])
					{
						int num11 = 0;
						int num12 = 0;
						lock (thislock)
						{
							if (!(vorbisFile != IntPtr.Zero))
							{
								return;
							}
							num13 = NativeMethods.DecodePacket((void*)vorbisFile, buf_out, array.Length, 16, &num, &num2, &num11, &num12);
							_position = (float)NativeMethods.TellTime((void*)vorbisFile);
						}
					}
					if (num13 == 0)
					{
						if (float.IsNaN(_repeatPosition))
						{
							if (!flag3)
							{
								if (num10 <= num9)
								{
									memoryStream.SetLength(num8 - num9);
									memoryStream.Position = 0L;
									SecBuf.Write(num9, memoryStream, num8 - num9, LockFlag.None);
									memoryStream.SetLength(num10);
									memoryStream.Position = 0L;
									SecBuf.Write(0, memoryStream, num10, LockFlag.None);
								}
								else
								{
									memoryStream.SetLength(num10 - num9);
									memoryStream.Position = 0L;
									SecBuf.Write(num9, memoryStream, num10 - num9, LockFlag.None);
								}
							}
							flag2 = true;
						}
						else
						{
							fixed (byte* buf_out2 = &array[0])
							{
								int num14 = 0;
								int num15 = 0;
								lock (thislock)
								{
									if (!(vorbisFile != IntPtr.Zero))
									{
										return;
									}
									NativeMethods.SeekTime((void*)vorbisFile, _repeatPosition);
									num13 = NativeMethods.DecodePacket((void*)vorbisFile, buf_out2, array.Length, 16, &num, &num2, &num14, &num15);
									_position = (float)NativeMethods.TellTime((void*)vorbisFile);
								}
							}
						}
					}
					if (flag && flag2)
					{
						if (Stopped != null)
						{
							OggPlayEventArgs e = new OggPlayEventArgs(false, "The Ogg Vorbis file or memory stream has no usable data.", false);
							Stopped(this, e);
						}
						_playing = false;
						return;
					}
					if (!flag && (num != num3 || num2 != num4))
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
					if (flag)
					{
						num6 = num * 2;
						num5 = num2 * num6;
						int num16 = (int)((float)num2 * SecBufHoldThisManySeconds);
						SecBufHoldThisManySeconds = (float)num16 / (float)num2;
						bufferRate = num5;
						format.AverageBytesPerSecond = num5;
						format.BitsPerSample = 16;
						format.BlockAlign = (short)num6;
						format.Channels = (short)num;
						format.SamplesPerSecond = num2;
						format.FormatTag = WaveFormatTag.Pcm;
						bufferDescription = new BufferDescription();
						bufferDescription.Format = format;
						num8 = (bufferDescription.BufferBytes = num16 * num6);
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
						BufferPositionNotify[] array2 = new BufferPositionNotify[3];
						array2[0].Offset = 0;
						array2[0].EventNotifyHandle = autoResetEvent.SafeWaitHandle.DangerousGetHandle();
						array2[1].Offset = num16 / 3 * num6;
						array2[1].EventNotifyHandle = autoResetEvent2.SafeWaitHandle.DangerousGetHandle();
						array2[2].Offset = num16 * 2 / 3 * num6;
						array2[2].EventNotifyHandle = autoResetEvent3.SafeWaitHandle.DangerousGetHandle();
						notify = new Notify(SecBuf);
						notify.SetNotificationPositions(array2);
						num3 = num;
						num4 = num2;
						flag = false;
					}
					else if (flag2)
					{
						if (flag3)
						{
							SecBuf.Play(0, BufferPlayFlags.Looping);
						}
						int num18 = 0;
						int num19 = num10;
						while (!stopNow)
						{
							int playPosition = SecBuf.PlayPosition;
							if (playPosition < num19)
							{
								num18++;
								memoryStream.SetLength(num8 - num19);
								memoryStream.Position = 0L;
								SecBuf.Write(num19, memoryStream, num8 - num19, LockFlag.None);
								memoryStream.SetLength(playPosition);
								memoryStream.Position = 0L;
								SecBuf.Write(0, memoryStream, playPosition, LockFlag.None);
							}
							else if (playPosition > num19)
							{
								memoryStream.SetLength(playPosition - num19);
								memoryStream.Position = 0L;
								SecBuf.Write(num19, memoryStream, playPosition - num19, LockFlag.None);
							}
							if (num10 <= num9)
							{
								if (playPosition >= num9 || num18 > 0)
								{
									break;
								}
							}
							else if ((playPosition < num10 && playPosition >= num9) || num18 > 1)
							{
								break;
							}
							Thread.Sleep(20);
							num19 = playPosition;
						}
						if (null != SecBuf)
						{
							SecBuf.Stop();
						}
						break;
					}
					memoryStream.SetLength(0L);
					memoryStream.Write(array, 0, num13);
					memoryStream.Position = 0L;
					num7 = 0;
					if (flag3)
					{
						int num20 = (int)Math.Min(memoryStream.Length, num8 - num9);
						if (num20 > 0)
						{
							SecBuf.Write(num9, memoryStream, num20, LockFlag.None);
							num9 += num20;
							num7 += num20;
						}
						if (num8 != num9)
						{
							continue;
						}
						flag3 = false;
						num9 = 0;
						SecBuf.Play(0, BufferPlayFlags.Looping);
						Thread.Sleep(0);
					}
					while (num7 < memoryStream.Length && !stopNow)
					{
						int num21 = 0;
						int playPosition2 = SecBuf.PlayPosition;
						int writePosition = SecBuf.WritePosition;
						if (num9 < playPosition2 && (writePosition >= playPosition2 || writePosition < num9))
						{
							num21 = playPosition2 - num9;
						}
						else if (num9 > writePosition && writePosition >= playPosition2)
						{
							num21 = num8 - num9 + playPosition2;
						}
						if (num21 > memoryStream.Length / 2)
						{
							num21 = (int)Math.Min(num21, memoryStream.Length - num7);
							memoryStream.Position = num7;
							SecBuf.Write(num9, memoryStream, num21, LockFlag.None);
							num9 = (num9 + num21) % num8;
							num10 = playPosition2;
							num7 += num21;
						}
						else
						{
							WaitHandle.WaitAny(waitHandles, new TimeSpan(0, 0, 5), true);
						}
					}
					bufferPosition = num9;
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
