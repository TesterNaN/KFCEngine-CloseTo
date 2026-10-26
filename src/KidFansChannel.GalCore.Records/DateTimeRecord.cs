using System.Collections.Generic;

namespace KidFansChannel.GalCore.Records
{
	public class DateTimeRecord : Record
	{
		private string _value;

		public override byte Code
		{
			get
			{
				return 5;
			}
		}

		public string Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
			}
		}

		protected override void SaveInfo(ref List<byte> result)
		{
			base.SaveInfo(ref result);
			Record.StringToBytes(Value, ref result);
		}

		protected override void LoadInfo(byte[] info)
		{
			base.LoadInfo(info);
			Value = Record.BytesToString(info, 2);
		}
	}
}
