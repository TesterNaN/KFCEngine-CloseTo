using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using KidFansChannel.SoundCore;

namespace KidFansChannel.GalCore
{
	public class GameSound : KidFansChannel.SoundCore.SoundCore
	{
		private GalCore galCore;

		private SoundPlayer bgmPlayer;

		private float _bgmVolumeSpeed = 2f;

		private float _volumeGlobalBgm = 1f;

		private float bgmVolumeTo = float.NaN;

		private bool bgmVolumeUp;

		private float bgmVolumeTempTo = float.NaN;

		private bool bgmVolumeTempUp;

		private float _volumeBgm;

		private float _volumeTempBgm;

		private SoundPlayer voicePlayer;

		private float _volumeGlobalVoice = 1f;

		private Dictionary<int, SoundPlayer> soundPlayers = new Dictionary<int, SoundPlayer>();

		private float _soundVolumeSpeed = 1f;

		private Dictionary<int, float> volumeSounds = new Dictionary<int, float>();

		private Dictionary<int, float> soundVolumeTos = new Dictionary<int, float>();

		private Dictionary<int, bool> soundVolumeUps = new Dictionary<int, bool>();

		private float _volumeGlobalSound;

		private SoundPlayer systemPlayer;

		private float _volumeGlobalSystem;

		public float BgmVolumeSpeed
		{
			get
			{
				return _bgmVolumeSpeed;
			}
			set
			{
				_bgmVolumeSpeed = value;
			}
		}

		public bool PreparedBgm
		{
			get
			{
				if (bgmPlayer != null)
				{
					return bgmPlayer.Prepared;
				}
				return false;
			}
		}

		public float VolumeGlobalBgm
		{
			get
			{
				return _volumeGlobalBgm;
			}
			set
			{
				_volumeGlobalBgm = value;
				if (bgmPlayer != null)
				{
					bgmPlayer.SetVolume(0, _volumeGlobalBgm);
				}
			}
		}

		public float VolumeBgm
		{
			get
			{
				if (bgmPlayer != null)
				{
					return bgmPlayer.GetVolume(1);
				}
				return _volumeBgm;
			}
			set
			{
				_volumeBgm = value;
				bgmVolumeTo = float.NaN;
				if (bgmPlayer != null)
				{
					bgmPlayer.SetVolume(1, _volumeBgm);
				}
			}
		}

		public float VolumeTempBgm
		{
			get
			{
				if (bgmPlayer != null)
				{
					return bgmPlayer.GetVolume(2);
				}
				return _volumeTempBgm;
			}
			set
			{
				_volumeTempBgm = value;
				bgmVolumeTempTo = float.NaN;
				if (bgmPlayer != null)
				{
					bgmPlayer.SetVolume(2, _volumeTempBgm);
				}
			}
		}

		public bool IsPlayingBgm
		{
			get
			{
				if (bgmPlayer != null)
				{
					return bgmPlayer.Playing;
				}
				return false;
			}
		}

		public bool PreparedVoice
		{
			get
			{
				if (voicePlayer != null)
				{
					return voicePlayer.Prepared;
				}
				return false;
			}
		}

		public float LengthVoice
		{
			get
			{
				if (voicePlayer != null)
				{
					return voicePlayer.Length;
				}
				return 0f;
			}
		}

		public float VolumeGlobalVoice
		{
			get
			{
				return _volumeGlobalVoice;
			}
			set
			{
				_volumeGlobalVoice = value;
				if (voicePlayer != null)
				{
					voicePlayer.SetVolume(0, _volumeGlobalVoice);
				}
			}
		}

		public bool IsPlayingVoice
		{
			get
			{
				if (voicePlayer != null)
				{
					return voicePlayer.Playing;
				}
				return false;
			}
		}

		public float VolumeGlobalSound
		{
			get
			{
				return _volumeGlobalSound;
			}
			set
			{
				_volumeGlobalSound = value;
				foreach (SoundPlayer value2 in soundPlayers.Values)
				{
					value2.SetVolume(0, _volumeGlobalSound);
				}
			}
		}

		public bool PreparedSystem
		{
			get
			{
				if (systemPlayer != null)
				{
					return systemPlayer.Prepared;
				}
				return false;
			}
		}

		public float VolumeGlobalSystem
		{
			get
			{
				return _volumeGlobalSystem;
			}
			set
			{
				_volumeGlobalSystem = value;
				if (systemPlayer != null)
				{
					systemPlayer.SetVolume(0, _volumeGlobalSystem);
				}
			}
		}

		public GameSound(Form owner, int deviceIndex)
			: base(owner, deviceIndex)
		{
		}

		public void SetCore(GalCore gc)
		{
			galCore = gc;
			galCore.GameOptions.OptionsUpdate += OptionsUpdate;
			OptionsUpdate(galCore.GameOptions, EventArgs.Empty);
		}

		private void OptionsUpdate(object sender, EventArgs e)
		{
			VolumeGlobalBgm = galCore.GameOptions.BgmVolume;
			VolumeGlobalSound = galCore.GameOptions.SeVolume;
			VolumeGlobalSystem = galCore.GameOptions.SystemVolume;
			VolumeGlobalVoice = galCore.GameOptions.VoiceVolume;
		}

		public void Update(double appTime, float elaspedTime)
		{
			UpdateBgm(elaspedTime);
			UpdateVoice(elaspedTime);
			UpdateSound(elaspedTime);
			UpdateSystem(elaspedTime);
		}

		public void OpenBgm(string filename, bool instant)
		{
			if (bgmPlayer != null)
			{
				bgmPlayer.Dispose();
			}
			bgmPlayer = CreatePlayer(3);
			bgmPlayer.Open(filename, instant);
		}

		public void OpenBgm(string filename)
		{
			OpenBgm(filename, false);
		}

		public void OpenBgm(Stream stream, bool instant)
		{
			if (bgmPlayer != null)
			{
				bgmPlayer.Dispose();
			}
			bgmPlayer = CreatePlayer(3);
			bgmPlayer.Open(stream, instant);
		}

		public void OpenBgm(Stream stream)
		{
			OpenBgm(stream, false);
		}

		public void PlayBgm()
		{
			PlayBgm(0f);
		}

		public void PlayBgm(float position)
		{
			PlayBgm(position, 0f);
		}

		public void PlayBgm(float position, float repeatPosition)
		{
			if (bgmPlayer != null && !bgmPlayer.Disposed)
			{
				bgmPlayer.SetVolume(0, _volumeGlobalBgm);
				bgmPlayer.SetVolume(1, _volumeBgm);
				_volumeTempBgm = 1f;
				bgmPlayer.SetVolume(2, _volumeTempBgm);
				bgmPlayer.Play(position, true, repeatPosition, true);
			}
		}

		public void StopBgm()
		{
			if (bgmPlayer != null && !bgmPlayer.Disposed)
			{
				bgmPlayer.Stop();
			}
		}

		public void ChangeVolumeBgm(float value)
		{
			_volumeBgm = value;
			if (bgmPlayer == null)
			{
				return;
			}
			if (_bgmVolumeSpeed == 0f)
			{
				bgmPlayer.SetVolume(1, value);
				return;
			}
			bgmVolumeTo = value;
			if (bgmVolumeTo > bgmPlayer.GetVolume(1))
			{
				bgmVolumeUp = true;
			}
			else
			{
				bgmVolumeUp = false;
			}
		}

		public void ChangeVolumeTempBgm(float value)
		{
			_volumeBgm = value;
			if (bgmPlayer == null)
			{
				return;
			}
			if (_bgmVolumeSpeed == 0f)
			{
				bgmPlayer.SetVolume(2, value);
				return;
			}
			bgmVolumeTempTo = value;
			if (bgmVolumeTempTo > bgmPlayer.GetVolume(2))
			{
				bgmVolumeTempUp = true;
			}
			else
			{
				bgmVolumeTempUp = false;
			}
		}

		protected void UpdateBgm(float elaspedTime)
		{
			if (bgmPlayer != null && bgmPlayer.Disposed)
			{
				bgmPlayer = null;
			}
			if (bgmPlayer == null)
			{
				return;
			}
			float volume;
			if (!float.IsNaN(bgmVolumeTo))
			{
				volume = bgmPlayer.GetVolume(1);
				if (bgmVolumeUp)
				{
					volume += _bgmVolumeSpeed * elaspedTime;
					if (volume >= bgmVolumeTo)
					{
						volume = bgmVolumeTo;
						bgmVolumeTo = float.NaN;
					}
				}
				else
				{
					volume -= _bgmVolumeSpeed * elaspedTime;
					if (volume <= bgmVolumeTo)
					{
						volume = bgmVolumeTo;
						bgmVolumeTo = float.NaN;
					}
				}
				bgmPlayer.SetVolume(1, volume);
			}
			if (float.IsNaN(bgmVolumeTempTo))
			{
				return;
			}
			volume = bgmPlayer.GetVolume(2);
			if (bgmVolumeTempUp)
			{
				volume += _bgmVolumeSpeed * elaspedTime;
				if (volume >= bgmVolumeTempTo)
				{
					volume = bgmVolumeTempTo;
					bgmVolumeTempTo = float.NaN;
				}
			}
			else
			{
				volume -= _bgmVolumeSpeed * elaspedTime;
				if (volume <= bgmVolumeTempTo)
				{
					volume = bgmVolumeTempTo;
					bgmVolumeTempTo = float.NaN;
				}
			}
			bgmPlayer.SetVolume(2, volume);
		}

		public void OpenVoice(string filename, bool instant)
		{
			if (voicePlayer != null)
			{
				voicePlayer.Dispose();
			}
			voicePlayer = CreatePlayer(1);
			voicePlayer.Open(filename, instant);
		}

		public void OpenVoice(string filename)
		{
			OpenVoice(filename, false);
		}

		public void OpenVoice(Stream stream, bool instant)
		{
			if (voicePlayer != null)
			{
				voicePlayer.Dispose();
			}
			voicePlayer = CreatePlayer(1);
			voicePlayer.Open(stream, instant);
		}

		public void OpenVoice(Stream stream)
		{
			OpenVoice(stream, false);
		}

		public void PlayVoice()
		{
			if (PreparedVoice)
			{
				voicePlayer.SetVolume(0, _volumeGlobalVoice);
				voicePlayer.Play();
			}
		}

		public void StopVoice()
		{
			if (PreparedVoice && !voicePlayer.Disposed)
			{
				voicePlayer.Stop();
			}
		}

		protected void UpdateVoice(float elaspedTime)
		{
			if (voicePlayer != null && voicePlayer.Disposed)
			{
				voicePlayer = null;
			}
		}

		public void OpenSound(string filename, int index, bool instant)
		{
			if (soundPlayers.ContainsKey(index))
			{
				soundPlayers[index].Dispose();
				soundPlayers[index] = CreatePlayer(2);
				volumeSounds[index] = 1f;
				soundVolumeTos[index] = float.NaN;
				soundVolumeUps[index] = false;
			}
			else
			{
				soundPlayers.Add(index, CreatePlayer(2));
				volumeSounds.Add(index, 1f);
				soundVolumeTos.Add(index, float.NaN);
				soundVolumeUps.Add(index, false);
			}
			soundPlayers[index].Open(filename, instant);
		}

		public void OpenSound(string filename, int index)
		{
			OpenSound(filename, index, false);
		}

		public void OpenSound(Stream stream, int index, bool instant)
		{
			if (soundPlayers.ContainsKey(index))
			{
				soundPlayers[index].Dispose();
				soundPlayers[index] = CreatePlayer(2);
				volumeSounds[index] = 1f;
				soundVolumeTos[index] = float.NaN;
				soundVolumeUps[index] = false;
			}
			else
			{
				soundPlayers.Add(index, CreatePlayer(2));
				volumeSounds.Add(index, 1f);
				soundVolumeTos.Add(index, float.NaN);
				soundVolumeUps.Add(index, false);
			}
			soundPlayers[index].Open(stream, instant);
		}

		public void OpenSound(Stream stream, int index)
		{
			OpenSound(stream, index, false);
		}

		public bool PreparedSound(int index)
		{
			if (soundPlayers.ContainsKey(index))
			{
				return soundPlayers[index].Prepared;
			}
			return false;
		}

		public void PlaySound(bool loop, bool closeAfterStopped, float repeatPosition, int index)
		{
			if (soundPlayers.ContainsKey(index) && !soundPlayers[index].Disposed)
			{
				soundPlayers[index].SetVolume(0, _volumeGlobalSound);
				soundPlayers[index].Play(0f, loop, loop ? repeatPosition : 0f, closeAfterStopped);
			}
		}

		public void PlaySound(bool loop, bool closeAfterStopped, int index)
		{
			PlaySound(loop, closeAfterStopped, 0f, index);
		}

		public void PlaySound(bool loop, float repeatPosition, int index)
		{
			PlaySound(loop, true, repeatPosition, index);
		}

		public void PlaySound(bool loop, int index)
		{
			PlaySound(loop, true, index);
		}

		public void StopSound(bool close, int index)
		{
			if (soundPlayers.ContainsKey(index) && !soundPlayers[index].Disposed)
			{
				soundPlayers[index].Stop(close);
			}
		}

		public void StopSound(int index)
		{
			StopSound(true, index);
		}

		public void CloseSound(int index)
		{
			if (soundPlayers.ContainsKey(index) && !soundPlayers[index].Disposed)
			{
				soundPlayers[index].Close();
			}
		}

		public float GetVolumeSound(int index)
		{
			if (!soundPlayers.ContainsKey(index))
			{
				return 0f;
			}
			return soundPlayers[index].GetVolume(1);
		}

		public void SetVolumeSound(float value, int index)
		{
			if (soundPlayers.ContainsKey(index))
			{
				volumeSounds[index] = value;
				soundPlayers[index].SetVolume(1, value);
			}
		}

		public void ChangeVolumeSound(float value, int index)
		{
			if (!soundPlayers.ContainsKey(index))
			{
				return;
			}
			volumeSounds[index] = value;
			if (_soundVolumeSpeed == 0f)
			{
				soundPlayers[index].SetVolume(1, value);
				return;
			}
			soundVolumeTos[index] = value;
			if (soundVolumeTos[index] > soundPlayers[index].GetVolume(1))
			{
				soundVolumeUps[index] = true;
			}
			else
			{
				soundVolumeUps[index] = false;
			}
		}

		public bool IsPlayingSound(int index)
		{
			if (soundPlayers.ContainsKey(index))
			{
				return soundPlayers[index].Playing;
			}
			return false;
		}

		protected void UpdateSound(float elaspedTime)
		{
			List<int> list = new List<int>();
			foreach (KeyValuePair<int, SoundPlayer> soundPlayer in soundPlayers)
			{
				if (soundPlayer.Value.Disposed)
				{
					list.Add(soundPlayer.Key);
				}
			}
			foreach (int item in list)
			{
				soundPlayers.Remove(item);
				volumeSounds.Remove(item);
				soundVolumeTos.Remove(item);
				soundVolumeUps.Remove(item);
			}
			foreach (KeyValuePair<int, SoundPlayer> soundPlayer2 in soundPlayers)
			{
				if (float.IsNaN(soundVolumeTos[soundPlayer2.Key]))
				{
					continue;
				}
				float volume = soundPlayer2.Value.GetVolume(1);
				if (soundVolumeUps[soundPlayer2.Key])
				{
					volume += _soundVolumeSpeed * elaspedTime;
					if (volume >= soundVolumeTos[soundPlayer2.Key])
					{
						volume = soundVolumeTos[soundPlayer2.Key];
						soundVolumeTos[soundPlayer2.Key] = float.NaN;
					}
				}
				else
				{
					volume -= _soundVolumeSpeed * elaspedTime;
					if (volume <= soundVolumeTos[soundPlayer2.Key])
					{
						volume = soundVolumeTos[soundPlayer2.Key];
						soundVolumeTos[soundPlayer2.Key] = float.NaN;
					}
				}
				soundPlayer2.Value.SetVolume(1, volume);
			}
			list.Clear();
		}

		public void OpenSystem(string filename, bool instant)
		{
			if (systemPlayer != null)
			{
				systemPlayer.Dispose();
			}
			systemPlayer = CreatePlayer(1);
			systemPlayer.Open(filename, instant);
		}

		public void OpenSystem(string filename)
		{
			OpenSystem(filename, false);
		}

		public void OpenSystem(Stream stream, bool instant)
		{
			if (systemPlayer != null)
			{
				systemPlayer.Dispose();
			}
			systemPlayer = CreatePlayer(1);
			systemPlayer.Open(stream, instant);
		}

		public void OpenSystem(Stream stream)
		{
			OpenSystem(stream, false);
		}

		public void PlaySystem()
		{
			if (PreparedSystem)
			{
				systemPlayer.SetVolume(0, _volumeGlobalSystem);
				systemPlayer.Play(0f, false, float.NaN, true);
			}
		}

		protected void UpdateSystem(float elaspedTime)
		{
			if (systemPlayer != null && systemPlayer.Disposed)
			{
				systemPlayer = null;
			}
		}
	}
}
