using System.Collections.Generic;
using Microsoft.DirectX.Direct3D;

namespace KidFansChannel.GameCore
{
	internal class EnumAdapterInformation
	{
		public uint AdapterOrdinal;

		public AdapterDetails AdapterInformation;

		public List<DisplayMode> displayModeList = new List<DisplayMode>();

		public List<EnumDeviceInformation> deviceInfoList = new List<EnumDeviceInformation>();

		public string UniqueDescription;
	}
}
