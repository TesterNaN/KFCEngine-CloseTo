using Microsoft.DirectX.Direct3D;

namespace KidFansChannel.GameCore
{
	internal class ManagedUtility
	{
		private ManagedUtility()
		{
		}

		public static uint GetColorChannelBits(Format format)
		{
			switch (format)
			{
			case Format.R8G8B8:
			case Format.A8R8G8B8:
			case Format.X8R8G8B8:
				return 8u;
			case Format.R5G6B5:
			case Format.X1R5G5B5:
			case Format.A1R5G5B5:
				return 5u;
			case Format.A4R4G4B4:
			case Format.X4R4G4B4:
				return 4u;
			case Format.R3G3B2:
			case Format.A8R3G3B2:
				return 2u;
			case Format.A2B10G10R10:
			case Format.A2R10G10B10:
				return 10u;
			default:
				return 0u;
			}
		}

		public static uint GetAlphaChannelBits(Format format)
		{
			switch (format)
			{
			case Format.R8G8B8:
			case Format.X8R8G8B8:
			case Format.R5G6B5:
			case Format.X1R5G5B5:
			case Format.R3G3B2:
			case Format.X4R4G4B4:
				return 0u;
			case Format.A8R8G8B8:
			case Format.A8R3G3B2:
				return 8u;
			case Format.A1R5G5B5:
				return 1u;
			case Format.A4R4G4B4:
				return 4u;
			case Format.A2B10G10R10:
			case Format.A2R10G10B10:
				return 2u;
			default:
				return 0u;
			}
		}

		public static uint GetDepthBits(DepthFormat format)
		{
			switch (format)
			{
			case DepthFormat.D16Lockable:
			case DepthFormat.D16:
				return 16u;
			case DepthFormat.D15S1:
				return 15u;
			case DepthFormat.D24S8:
			case DepthFormat.D24X8:
			case DepthFormat.D24X4S4:
			case DepthFormat.D24SingleS8:
				return 24u;
			case DepthFormat.D32:
			case DepthFormat.D32SingleLockable:
				return 32u;
			default:
				return 0u;
			}
		}

		public static uint GetStencilBits(DepthFormat format)
		{
			switch (format)
			{
			case DepthFormat.D16Lockable:
			case DepthFormat.D32:
			case DepthFormat.D24X8:
			case DepthFormat.D16:
			case DepthFormat.D32SingleLockable:
				return 0u;
			case DepthFormat.D15S1:
				return 1u;
			case DepthFormat.D24X4S4:
				return 4u;
			case DepthFormat.D24S8:
			case DepthFormat.D24SingleS8:
				return 8u;
			default:
				return 0u;
			}
		}
	}
}
