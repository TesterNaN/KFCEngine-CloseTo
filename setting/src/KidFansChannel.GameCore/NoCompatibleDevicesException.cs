using System;

namespace KidFansChannel.GameCore
{
	public class NoCompatibleDevicesException : ApplicationException
	{
		public NoCompatibleDevicesException()
			: base("Could not find any compatible Direct3D devices.")
		{
		}

		public NoCompatibleDevicesException(Exception inner)
			: base("Could not find any compatible Direct3D devices.", inner)
		{
		}
	}
}
