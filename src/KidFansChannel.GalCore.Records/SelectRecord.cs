using System.Collections.Generic;

namespace KidFansChannel.GalCore.Records
{
	public class SelectRecord : Record
	{
		private bool _selected;

		private string _title;

		private string _case;

		public override byte Code
		{
			get
			{
				return 4;
			}
		}

		public bool Selected
		{
			get
			{
				return _selected;
			}
			set
			{
				_selected = value;
			}
		}

		public string Title
		{
			get
			{
				return _title;
			}
			set
			{
				_title = value;
			}
		}

		public string Case
		{
			get
			{
				return _case;
			}
			set
			{
				_case = value;
			}
		}

		protected override void SaveInfo(ref List<byte> result)
		{
			base.SaveInfo(ref result);
			result.Add((byte)(_selected ? 1u : 0u));
			Record.StringToBytes(_title, ref result);
			result.Add(0);
			Record.StringToBytes(_case, ref result);
		}

		protected override void LoadInfo(byte[] info)
		{
			base.LoadInfo(info);
			_selected = info[2] > 0;
			int index = 3;
			_title = Record.BytesToString(info, ref index);
			index++;
			_case = Record.BytesToString(info, ref index);
		}
	}
}
