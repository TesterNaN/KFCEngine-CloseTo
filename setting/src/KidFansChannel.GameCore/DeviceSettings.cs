using System;
using Microsoft.DirectX.Direct3D;

namespace KidFansChannel.GameCore
{
	internal class DeviceSettings : ICloneable
	{
		public uint AdapterOrdinal;

		public DeviceType DeviceType;

		public Format AdapterFormat;

		public CreateFlags BehaviorFlags;

		public PresentParameters PresentParameters;

		public DeviceSettings Clone()
		{
			DeviceSettings deviceSettings = new DeviceSettings();
			deviceSettings.AdapterOrdinal = AdapterOrdinal;
			deviceSettings.BehaviorFlags = BehaviorFlags;
			deviceSettings.DeviceType = DeviceType;
			deviceSettings.PresentParameters = (PresentParameters)PresentParameters.Clone();
			return deviceSettings;
		}

		object ICloneable.Clone()
		{
			throw new NotSupportedException("Use the strongly typed overload instead.");
		}
	}
}
