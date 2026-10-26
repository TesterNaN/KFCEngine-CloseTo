using System.Collections.Generic;

namespace KidFansChannel.GalCore.Records
{
	public class TextRecord : Record
	{
		private List<int> _speakers = new List<int>();

		private int _speakerNameLength;

		private string _voice = string.Empty;

		public override byte Code
		{
			get
			{
				return 1;
			}
		}

		public List<int> Speakers
		{
			get
			{
				return _speakers;
			}
		}

		public int SpeakerNameLength
		{
			get
			{
				return _speakerNameLength;
			}
			set
			{
				_speakerNameLength = value;
			}
		}

		public string Voice
		{
			get
			{
				return _voice;
			}
			set
			{
				_voice = value;
				HasVoice = true;
			}
		}

		~TextRecord()
		{
			_speakers.Clear();
		}

		protected override void SaveInfo(ref List<byte> result)
		{
			base.SaveInfo(ref result);
			result.Add((byte)Line);
			result.Add((byte)_speakers.Count);
			foreach (int speaker in _speakers)
			{
				result.Add((byte)speaker);
			}
			result.Add((byte)_speakerNameLength);
			Record.StringToBytes(_voice, ref result);
		}

		protected override void LoadInfo(byte[] info)
		{
			base.LoadInfo(info);
			Line = info[2];
			int num = info[3];
			_speakers.Clear();
			for (int i = 0; i < num; i++)
			{
				_speakers.Add(info[i + 4]);
			}
			_speakerNameLength = info[num + 4];
			_voice = Record.BytesToString(info, num + 5);
			if (_voice != string.Empty)
			{
				HasVoice = true;
			}
		}
	}
}
