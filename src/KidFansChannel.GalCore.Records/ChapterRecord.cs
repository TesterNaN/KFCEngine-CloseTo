using System.Collections.Generic;

namespace KidFansChannel.GalCore.Records
{
	public class ChapterRecord : Record
	{
		private string _title;

		public override byte Code
		{
			get
			{
				return 3;
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

		protected override void SaveInfo(ref List<byte> result)
		{
			base.SaveInfo(ref result);
			Record.StringToBytes(_title, ref result);
		}

		protected override void LoadInfo(byte[] info)
		{
			base.LoadInfo(info);
			_title = Record.BytesToString(info, 2);
		}
	}
}
