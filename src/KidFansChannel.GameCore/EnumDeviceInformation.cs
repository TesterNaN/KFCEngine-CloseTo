using System.Collections.Generic;
using Microsoft.DirectX.Direct3D;

namespace KidFansChannel.GameCore
{
	internal class EnumDeviceInformation
	{
		public uint AdapterOrdinal;

		public DeviceType DeviceType;

		public Caps Caps;

		public List<EnumDeviceSettingsCombo> deviceSettingsList = new List<EnumDeviceSettingsCombo>();
	}
}
