using System;
using Microsoft.DirectX.Direct3D;

namespace KidFansChannel.GameCore
{
	public class DeviceEventArgs : EventArgs
	{
		public Device Device;

		public SurfaceDescription BackBufferDescription;

		public DeviceEventArgs(Device d, SurfaceDescription desc)
		{
			Device = d;
			BackBufferDescription = desc;
		}
	}
}
