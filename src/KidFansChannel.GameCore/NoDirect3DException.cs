using System;

namespace KidFansChannel.GameCore
{
	public class NoDirect3DException : ApplicationException
	{
		public NoDirect3DException()
			: base("Could not initialize Direct3D. You may want to check that the latest version of DirectX is correctly installed on your system.")
		{
		}

		public NoDirect3DException(Exception inner)
			: base("Could not initialize Direct3D. You may want to check that the latest version of DirectX is correctly installed on your system.", inner)
		{
		}
	}
}
