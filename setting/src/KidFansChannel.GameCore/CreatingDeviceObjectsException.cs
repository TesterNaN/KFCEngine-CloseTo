using System;

namespace KidFansChannel.GameCore
{
	public class CreatingDeviceObjectsException : ApplicationException
	{
		public CreatingDeviceObjectsException()
			: base("Failed creating Direct3D device objects.")
		{
		}

		public CreatingDeviceObjectsException(Exception inner)
			: base("Failed creating Direct3D device objects.", inner)
		{
		}
	}
}
