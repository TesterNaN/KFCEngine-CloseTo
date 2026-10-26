namespace KidFansChannel.GalCore
{
	public struct GamePadMap
	{
		public int OK;

		public int Cancel;

		public int SkipRead;

		public int SkipAll;

		public int Auto;

		public int Quick;

		public int Hide;

		public int Message;

		public int PageUp;

		public int PageDown;

		public GamePadMap(bool defaultValue)
		{
			if (defaultValue)
			{
				OK = 0;
				Cancel = 9;
				SkipRead = 4;
				SkipAll = 5;
				Auto = 3;
				Quick = 8;
				Hide = 1;
				Message = 2;
				PageUp = 6;
				PageDown = 7;
			}
			else
			{
				OK = -1;
				Cancel = -1;
				SkipRead = -1;
				SkipAll = -1;
				Auto = -1;
				Quick = -1;
				Hide = -1;
				Message = -1;
				PageUp = -1;
				PageDown = -1;
			}
		}
	}
}
