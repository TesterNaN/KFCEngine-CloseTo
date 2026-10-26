using System;

namespace KidFansChannel.GameCore
{
	public class MediaNotFoundException : ApplicationException
	{
		public MediaNotFoundException()
			: base("Could not find required media. Ensure that the DirectX SDK is correctly installed.")
		{
		}

		public MediaNotFoundException(Exception inner)
			: base("Could not find required media. Ensure that the DirectX SDK is correctly installed.", inner)
		{
		}
	}
}
