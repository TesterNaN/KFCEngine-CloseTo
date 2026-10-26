using System;

namespace KidFansChannel.GalCore
{
	public class LanguageEventArgs : EventArgs
	{
		private string _name;

		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}

		public LanguageEventArgs(string name)
		{
			_name = name;
		}
	}
}
