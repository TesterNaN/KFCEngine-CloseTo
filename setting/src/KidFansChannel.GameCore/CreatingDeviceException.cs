using System;

namespace KidFansChannel.GameCore
{
	public class CreatingDeviceException : ApplicationException
	{
		public CreatingDeviceException()
			: base("Failed creating the Direct3D device.")
		{
		}

		public CreatingDeviceException(Exception inner)
			: base("Failed creating the Direct3D device.", inner)
		{
		}
	}
}
