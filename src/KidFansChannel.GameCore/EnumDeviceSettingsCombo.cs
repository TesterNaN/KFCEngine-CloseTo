using System.Collections.Generic;
using Microsoft.DirectX.Direct3D;

namespace KidFansChannel.GameCore
{
	internal class EnumDeviceSettingsCombo
	{
		public uint AdapterOrdinal;

		public DeviceType DeviceType;

		public Format AdapterFormat;

		public Format BackBufferFormat;

		public bool IsWindowed;

		public List<DepthFormat> depthStencilFormatList = new List<DepthFormat>();

		public List<MultiSampleType> multiSampleTypeList = new List<MultiSampleType>();

		public List<int> multiSampleQualityList = new List<int>();

		public List<PresentInterval> presentIntervalList = new List<PresentInterval>();

		public List<EnumDepthStencilMultisampleConflict> depthStencilConflictList = new List<EnumDepthStencilMultisampleConflict>();

		public EnumAdapterInformation adapterInformation;

		public EnumDeviceInformation deviceInformation;
	}
}
