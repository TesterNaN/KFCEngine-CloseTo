using System;

namespace KidFansChannel.GameCore
{
	public class ResettingDeviceException : ApplicationException
	{
		public ResettingDeviceException()
			: base("Failed resetting the Direct3D device.")
		{
		}

		public ResettingDeviceException(Exception inner)
			: base("Failed resetting the Direct3D device.", inner)
		{
		}
	}
}
