namespace KidFansChannel.GalCore
{
	public static class GameManager
	{
		private static GalCore _galCore;

		public static GalCore GalCore
		{
			get
			{
				return _galCore;
			}
			set
			{
				_galCore = value;
			}
		}

		public static GameData Data
		{
			get
			{
				return _galCore.GameData;
			}
		}

		public static GameSound Sound
		{
			get
			{
				return _galCore.GameSound;
			}
		}

		public static GameGraphics Graphics
		{
			get
			{
				return _galCore.GameGraphics;
			}
		}

		public static GameState GameState
		{
			get
			{
				return _galCore.GameState;
			}
		}

		public static GameScript GameScript
		{
			get
			{
				return _galCore.GameScript;
			}
		}

		public static GameInstanceScript CurrentScript
		{
			get
			{
				return _galCore.GameScript.CurrentGameScript;
			}
		}
	}
}
