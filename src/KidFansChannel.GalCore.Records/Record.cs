using System;
using System.Collections.Generic;
using System.Text;

namespace KidFansChannel.GalCore.Records
{
	public abstract class Record
	{
		private int _length = 1;

		private string _text = string.Empty;

		private int _line = 1;

		private bool _hasVoice;

		protected static Dictionary<int, Type> RecordType = new Dictionary<int, Type>();

		public int Length
		{
			get
			{
				return _length;
			}
		}

		public abstract byte Code { get; }

		public string Text
		{
			get
			{
				return _text;
			}
			set
			{
				_text = value;
			}
		}

		public int Line
		{
			get
			{
				return _line;
			}
			set
			{
				_line = value;
			}
		}

		public bool HasVoice
		{
			get
			{
				return _hasVoice;
			}
			set
			{
				_hasVoice = value;
			}
		}

		public void Save(out byte[] info, out string text)
		{
			List<byte> result = new List<byte>();
			result.Add(0);
			result.Add(Code);
			SaveInfo(ref result);
			_length = result.Count - 1;
			result[0] = (byte)_length;
			info = result.ToArray();
			text = SaveText().Replace('\n', '\t');
		}

		protected virtual void SaveInfo(ref List<byte> result)
		{
		}

		protected virtual string SaveText()
		{
			return _text;
		}

		public void Load(byte[] info, string text)
		{
			if (info[0] != info.Length - 1)
			{
				throw new ArgumentException();
			}
			if (info[1] != Code)
			{
				throw new ArgumentException();
			}
			LoadInfo(info);
			LoadText(text.Replace('\t', '\n'));
		}

		protected virtual void LoadInfo(byte[] info)
		{
		}

		protected virtual void LoadText(string text)
		{
			_text = text;
		}

		protected static void StringToBytes(string value, ref List<byte> result)
		{
			char[] array = value.ToCharArray();
			for (int i = 0; i < array.Length; i++)
			{
				result.Add((byte)(array[i] & 0xFF));
				result.Add((byte)((int)array[i] >> 8));
			}
		}

		protected static string BytesToString(byte[] value, int index)
		{
			return BytesToString(value, ref index);
		}

		protected static string BytesToString(byte[] value, ref int index)
		{
			List<char> list = new List<char>();
			while (index < value.Length - 1 && value[index] != 0)
			{
				list.Add((char)(value[index] | (value[index + 1] << 8)));
				index += 2;
			}
			return new StringBuilder().Append(list.ToArray()).ToString();
		}

		public static void AddRecordType(Type recordType)
		{
			if (!recordType.IsSubclassOf(typeof(Record)))
			{
				throw new ArgumentException();
			}
			RecordType.Add(((Record)recordType.Assembly.CreateInstance(recordType.FullName)).Code, recordType);
		}

		public static void ClearRecordType()
		{
			RecordType.Clear();
		}

		public static Record Load(byte[] infos, ref int index, string text)
		{
			byte[] array = new byte[infos[index] + 1];
			Array.Copy(infos, index, array, 0, infos[index] + 1);
			index += array[0] + 1;
			if (array[0] == 0)
			{
				return null;
			}
			byte key = array[1];
			Record record = (Record)RecordType[key].Assembly.CreateInstance(RecordType[key].FullName);
			record.Load(array, text);
			return record;
		}
	}
}
