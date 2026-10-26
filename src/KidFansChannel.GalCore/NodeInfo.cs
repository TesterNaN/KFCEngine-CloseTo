namespace KidFansChannel.GalCore
{
	public class NodeInfo
	{
		private string _file;

		private string _chapter;

		private int _scene;

		private int _click;

		public string File
		{
			get
			{
				return _file;
			}
			set
			{
				_file = value;
			}
		}

		public string Chapter
		{
			get
			{
				return _chapter;
			}
			set
			{
				_chapter = value;
			}
		}

		public int Scene
		{
			get
			{
				return _scene;
			}
			set
			{
				_scene = value;
			}
		}

		public int Click
		{
			get
			{
				return _click;
			}
			set
			{
				_click = value;
			}
		}
	}
}
