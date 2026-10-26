using System;
using System.IO;
using System.Threading;
using KidFansChannel.SoundCore.Ogg;

namespace KidFansChannel.SoundCore
{
	public class SoundPlayer : IDisposable
	{
		protected SoundCore soundCore;

		protected OggPlayer player;

		private int _volumeLayers;

		private float[] _volume;

		private float _position;

		private float _length;

		private bool closeAfter;

		private bool _prepared;

		private bool _disposed;

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
				float clampedPosition = player.Position;
				if (clampedPosition < _position)
				{
					clampedPosition = _position;
				}
				else if (clampedPosition > _length)
				{
					clampedPosition = _length;
				}
				_position = clampedPosition;
				return clampedPosition;
			}
		}

		public int VolumeLayers
		{
			get
			{
				return _volumeLayers;
			}
		}

		public bool Prepared
		{
			get
			{
				return _prepared;
			}
		}

		public bool Playing
		{
			get
			{
				if (player != null)
				{
					return player.Playing;
				}
				return false;
			}
		}

		public bool Disposed
		{
			get
			{
				return _disposed;
			}
		}

		internal SoundPlayer(SoundCore core, int volumeLayers)
		{
			soundCore = core;
			_volumeLayers = volumeLayers;
			_volume = new float[_volumeLayers];
			for (int i = 0; i < _volumeLayers; i++)
			{
				_volume[i] = 1f;
			}
		}

		public void Open(string filename, bool instant)
		{
			if (_prepared)
			{
				throw new InvalidOperationException("Already opened.");
			}
			if (_disposed)
			{
				throw new InvalidOperationException();
			}
			if (string.IsNullOrEmpty(filename))
			{
				throw new ArgumentNullException("filename");
			}
			if (!File.Exists(filename))
			{
				throw new ArgumentException("filename");
			}
			if (instant)
			{
				OpenThreadProc(filename);
			}
			else
			{
				new Thread(OpenThreadProc).Start(filename);
			}
		}

		public void Open(string filename)
		{
			Open(filename, false);
		}

		public void Open(Stream stream, bool instant)
		{
			if (_prepared)
			{
				throw new InvalidOperationException("Already opened.");
			}
			if (_disposed)
			{
				throw new InvalidOperationException();
			}
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (!stream.CanRead)
			{
				throw new ArgumentException("stream");
			}
			if (instant)
			{
				OpenThreadProc(stream);
			}
			else
			{
				new Thread(OpenThreadProc).Start(stream);
			}
		}

		public void Open(Stream stream)
		{
			Open(stream, false);
		}

		public void OpenThreadProc(object arg)
		{
			if (arg is string)
			{
				string fileName = (string)arg;
				player = new OggPlayer(fileName);
			}
			else
			{
				if (!(arg is Stream))
				{
					throw new ArgumentException("arg");
				}
				Stream stream = (Stream)arg;
				player = new OggPlayer(stream);
				stream.Close();
				stream.Dispose();
			}
			player.Stopped += Player_Stopped;
			float clampedPosition = 1f;
			for (int i = 0; i < _volumeLayers; i++)
			{
				clampedPosition *= _volume[i];
			}
			player.Volume = clampedPosition;
			_length = player.Length;
			_position = 0f;
			_prepared = true;
		}

		public void Close()
		{
			if (!_prepared)
			{
				throw new InvalidOperationException();
			}
			Dispose();
		}

		public void Play(float position, bool loop, float repeatPosition, bool closeAfterStopped)
		{
			if (!_prepared || _disposed)
			{
				throw new InvalidOperationException();
			}
			if (loop)
			{
				player.Play(position, repeatPosition);
			}
			else
			{
				player.Play(position, float.NaN);
			}
			closeAfter = closeAfterStopped;
			_position = position;
		}

		public void Play()
		{
			Play(0f, false, 0f, true);
		}

		public void Play(bool loop)
		{
			Play(0f, loop, loop ? 0f : float.NaN, true);
		}

		public void Play(float position, bool loop)
		{
			Play(position, loop, loop ? 0f : float.NaN, true);
		}

		public void Play(float position, float repeatPosition)
		{
			Play(position, true, repeatPosition, true);
		}

		private void Player_Stopped(object sender, OggPlayEventArgs e)
		{
			if (closeAfter)
			{
				Close();
				return;
			}
			_length = 0f;
			_position = 0f;
		}

		public void Stop()
		{
			if (player != null)
			{
				player.Stop();
			}
		}

		public void Stop(bool close)
		{
			if (player != null)
			{
				player.Stop();
				if (close)
				{
					player.Close();
				}
			}
		}

		public float GetVolume(int layer)
		{
			return _volume[layer];
		}

		public void SetVolume(int layer, float value)
		{
			_volume[layer] = value;
			float clampedPosition = 1f;
			for (int i = 0; i < _volumeLayers; i++)
			{
				clampedPosition *= _volume[i];
			}
			if (player != null)
			{
				player.Volume = clampedPosition;
			}
		}

		~SoundPlayer()
		{
			Dispose();
		}

		public void Dispose()
		{
			lock (this)
			{
				if (!_disposed)
				{
					if (player != null)
					{
						player.Close();
						player = null;
					}
					_length = 0f;
					_position = 0f;
					_disposed = true;
					if (soundCore.PlayerList.Contains(this))
					{
						soundCore.PlayerList.Remove(this);
					}
					GC.SuppressFinalize(this);
				}
			}
		}
	}
}
