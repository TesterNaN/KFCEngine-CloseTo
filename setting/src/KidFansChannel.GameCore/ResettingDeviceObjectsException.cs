using System;

namespace KidFansChannel.GameCore
{
	public class ResettingDeviceObjectsException : ApplicationException
	{
		public ResettingDeviceObjectsException()
			: base("Failed resetting Direct3D device objects.")
		{
		}

		public ResettingDeviceObjectsException(Exception inner)
			: base("Failed resetting Direct3D device objects.", inner)
		{
		}
	}
}
