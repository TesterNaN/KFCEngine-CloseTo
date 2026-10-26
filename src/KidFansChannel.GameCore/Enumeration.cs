using System;
using System.Collections.Generic;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace KidFansChannel.GameCore
{
	internal sealed class Enumeration
	{
		private static bool isPostPixelShaderBlendingRequired;

		private static bool isSoftwareVertexProcessing;

		private static bool isHardwareVertexProcessing;

		private static bool isPureHardwareVertexProcessing;

		private static bool isMixedVertexProcessing;

		private static uint minimumWidth;

		private static uint maximumWidth;

		private static uint minimumHeight;

		private static uint maximumHeight;

		private static uint minimumRefresh;

		private static uint maximumRefresh;

		private static uint multisampleQualityMax;

		private static List<DepthFormat> depthStencilPossibleList;

		private static List<MultiSampleType> multiSampleTypeList;

		private static List<PresentInterval> presentIntervalList;

		private static List<EnumAdapterInformation> adapterInformationList;

		private static readonly Format[] allowedFormats;

		private static readonly Format[] backbufferFormatsArray;

		private static readonly DeviceType[] deviceTypeArray;

		public static uint MultisampleQualityMax
		{
			get
			{
				return multisampleQualityMax;
			}
			set
			{
				if (value > 65535)
				{
					multisampleQualityMax = 65535u;
				}
				else
				{
					multisampleQualityMax = value;
				}
			}
		}

		public static bool IsPostPixelShaderBlendingRequred
		{
			get
			{
				return isPostPixelShaderBlendingRequired;
			}
			set
			{
				isPostPixelShaderBlendingRequired = value;
			}
		}

		public static bool IsSoftwareVertexProcessingPossible
		{
			get
			{
				return isSoftwareVertexProcessing;
			}
			set
			{
				isSoftwareVertexProcessing = value;
			}
		}

		public static bool IsHardwareVertexProcessingPossible
		{
			get
			{
				return isHardwareVertexProcessing;
			}
			set
			{
				isHardwareVertexProcessing = value;
			}
		}

		public static bool IsPureHardwareVertexProcessingPossible
		{
			get
			{
				return isPureHardwareVertexProcessing;
			}
			set
			{
				isPureHardwareVertexProcessing = value;
			}
		}

		public static bool IsMixedVertexProcessingPossible
		{
			get
			{
				return isMixedVertexProcessing;
			}
			set
			{
				isMixedVertexProcessing = value;
			}
		}

		public static List<DepthFormat> PossibleDepthStencilFormatList
		{
			get
			{
				return depthStencilPossibleList;
			}
		}

		public static List<MultiSampleType> PossibleMultisampleTypeList
		{
			get
			{
				return multiSampleTypeList;
			}
		}

		public static List<PresentInterval> PossiblePresentIntervalsList
		{
			get
			{
				return presentIntervalList;
			}
		}

		public static List<EnumAdapterInformation> AdapterInformationList
		{
			get
			{
				return adapterInformationList;
			}
		}

		private Enumeration()
		{
		}

		static Enumeration()
		{
			isPostPixelShaderBlendingRequired = true;
			isSoftwareVertexProcessing = true;
			isHardwareVertexProcessing = true;
			isPureHardwareVertexProcessing = true;
			isMixedVertexProcessing = false;
			minimumWidth = 0u;
			maximumWidth = uint.MaxValue;
			minimumHeight = 0u;
			maximumHeight = uint.MaxValue;
			minimumRefresh = 0u;
			maximumRefresh = uint.MaxValue;
			multisampleQualityMax = 65535u;
			depthStencilPossibleList = new List<DepthFormat>();
			multiSampleTypeList = new List<MultiSampleType>();
			presentIntervalList = new List<PresentInterval>();
			adapterInformationList = new List<EnumAdapterInformation>();
			allowedFormats = new Format[4]
			{
				Format.X8R8G8B8,
				Format.X1R5G5B5,
				Format.R5G6B5,
				Format.A2R10G10B10
			};
			backbufferFormatsArray = new Format[6]
			{
				Format.A8R8G8B8,
				Format.X8R8G8B8,
				Format.A1R5G5B5,
				Format.X1R5G5B5,
				Format.R5G6B5,
				Format.A2R10G10B10
			};
			deviceTypeArray = new DeviceType[3]
			{
				DeviceType.Hardware,
				DeviceType.Software,
				DeviceType.Reference
			};
			ResetPossibleDepthStencilFormats();
			ResetPossibleMultisampleTypeList();
			ResetPossiblePresentIntervalList();
		}

		public static void Enumerate()
		{
			DisplayModeSorter comparer = new DisplayModeSorter();
			try
			{
				adapterInformationList.Clear();
				List<Format> supportedFormats = new List<Format>();
				foreach (AdapterInformation adapter in Manager.Adapters)
				{
					EnumAdapterInformation enumAdapterInformation = new EnumAdapterInformation();
					enumAdapterInformation.AdapterOrdinal = (uint)adapter.Adapter;
					enumAdapterInformation.AdapterInformation = adapter.Information;
					supportedFormats.Clear();
					for (int i = 0; i < allowedFormats.Length; i++)
					{
						foreach (DisplayMode item in adapter.SupportedDisplayModes[allowedFormats[i]])
						{
							if (item.Width >= minimumWidth && item.Height >= minimumHeight && item.Width <= maximumWidth && item.Height <= maximumHeight && item.RefreshRate >= minimumRefresh && item.RefreshRate <= maximumRefresh)
							{
								enumAdapterInformation.displayModeList.Add(item);
								if (!supportedFormats.Contains(item.Format))
								{
									supportedFormats.Add(item.Format);
								}
							}
						}
					}
					DisplayMode currentDisplayMode = adapter.CurrentDisplayMode;
					if (!supportedFormats.Contains(currentDisplayMode.Format))
					{
						supportedFormats.Add(currentDisplayMode.Format);
					}
					enumAdapterInformation.displayModeList.Sort(comparer);
					EnumerateDevices(enumAdapterInformation, supportedFormats);
					if (enumAdapterInformation.deviceInfoList.Count > 0)
					{
						adapterInformationList.Add(enumAdapterInformation);
					}
				}
				bool supportsFormat = true;
				for (int j = 0; j < adapterInformationList.Count; j++)
				{
					for (int k = j + 1; k < adapterInformationList.Count; k++)
					{
						EnumAdapterInformation enumAdapterInformation2 = adapterInformationList[j];
						EnumAdapterInformation enumAdapterInformation3 = adapterInformationList[k];
						if (string.Compare(enumAdapterInformation2.AdapterInformation.Description, enumAdapterInformation3.AdapterInformation.Description, true) == 0)
						{
							supportsFormat = false;
							break;
						}
					}
					if (!supportsFormat)
					{
						break;
					}
				}
				for (int l = 0; l < adapterInformationList.Count; l++)
				{
					EnumAdapterInformation enumAdapterInformation4 = adapterInformationList[l];
					enumAdapterInformation4.UniqueDescription = enumAdapterInformation4.AdapterInformation.Description;
					if (!supportsFormat)
					{
						enumAdapterInformation4.UniqueDescription += string.Format(" (#{0})", enumAdapterInformation4.AdapterOrdinal);
					}
				}
			}
			catch (TypeLoadException)
			{
				throw new NoDirect3DException();
			}
		}

		private static void EnumerateDevices(EnumAdapterInformation adapterInfo, List<Format> adapterFormatList)
		{
			DirectXException.IgnoreExceptions();
			for (uint typeIndex = 0u; typeIndex < deviceTypeArray.Length; typeIndex++)
			{
				EnumDeviceInformation enumDeviceInformation = new EnumDeviceInformation();
				enumDeviceInformation.DeviceType = deviceTypeArray[typeIndex];
				enumDeviceInformation.Caps = Manager.GetDeviceCaps((int)adapterInfo.AdapterOrdinal, enumDeviceInformation.DeviceType);
				EnumerateDeviceCombos(adapterInfo, enumDeviceInformation, adapterFormatList);
				if (enumDeviceInformation.deviceSettingsList.Count > 0)
				{
					adapterInfo.deviceInfoList.Add(enumDeviceInformation);
				}
			}
			DirectXException.EnableExceptions();
		}

		private static void EnumerateDeviceCombos(EnumAdapterInformation adapterInfo, EnumDeviceInformation deviceInfo, List<Format> adapterFormatList)
		{
			foreach (Format adapterFormat in adapterFormatList)
			{
				for (int i = 0; i < backbufferFormatsArray.Length; i++)
				{
					for (int j = 0; j < 2; j++)
					{
						bool supportsFormat = j == 1;
						if ((!supportsFormat && adapterInfo.displayModeList.Count == 0) || !Manager.CheckDeviceType((int)adapterInfo.AdapterOrdinal, deviceInfo.DeviceType, adapterFormat, backbufferFormatsArray[i], supportsFormat) || (isPostPixelShaderBlendingRequired && !Manager.CheckDeviceFormat((int)adapterInfo.AdapterOrdinal, deviceInfo.DeviceType, adapterFormat, Usage.QueryPostPixelShaderBlending, ResourceType.Textures, backbufferFormatsArray[i])))
						{
							continue;
						}
						Caps caps = deviceInfo.Caps;
						Format checkFormat = backbufferFormatsArray[i];
						if (Manager.CheckDeviceFormat(caps.AdapterOrdinal, caps.DeviceType, adapterFormat, Usage.QueryPostPixelShaderBlending, ResourceType.Textures, checkFormat))
						{
							EnumDeviceSettingsCombo enumDeviceSettingsCombo = new EnumDeviceSettingsCombo();
							enumDeviceSettingsCombo.AdapterOrdinal = adapterInfo.AdapterOrdinal;
							enumDeviceSettingsCombo.DeviceType = deviceInfo.DeviceType;
							enumDeviceSettingsCombo.AdapterFormat = adapterFormat;
							enumDeviceSettingsCombo.BackBufferFormat = backbufferFormatsArray[i];
							enumDeviceSettingsCombo.IsWindowed = supportsFormat;
							BuildDepthStencilFormatList(enumDeviceSettingsCombo);
							BuildMultiSampleTypeList(enumDeviceSettingsCombo);
							if (enumDeviceSettingsCombo.multiSampleTypeList.Count != 0)
							{
								BuildConflictList(enumDeviceSettingsCombo);
								BuildPresentIntervalList(deviceInfo, enumDeviceSettingsCombo);
								enumDeviceSettingsCombo.adapterInformation = adapterInfo;
								enumDeviceSettingsCombo.deviceInformation = deviceInfo;
								deviceInfo.deviceSettingsList.Add(enumDeviceSettingsCombo);
							}
						}
					}
				}
			}
		}

		private static void BuildDepthStencilFormatList(EnumDeviceSettingsCombo deviceCombo)
		{
			foreach (DepthFormat depthStencilPossible in depthStencilPossibleList)
			{
				if (Manager.CheckDeviceFormat((int)deviceCombo.AdapterOrdinal, deviceCombo.DeviceType, deviceCombo.AdapterFormat, Usage.DepthStencil, ResourceType.Surface, depthStencilPossible) && Manager.CheckDepthStencilMatch((int)deviceCombo.AdapterOrdinal, deviceCombo.DeviceType, deviceCombo.AdapterFormat, deviceCombo.BackBufferFormat, depthStencilPossible))
				{
					deviceCombo.depthStencilFormatList.Add(depthStencilPossible);
				}
			}
		}

		private static void BuildMultiSampleTypeList(EnumDeviceSettingsCombo deviceCombo)
		{
			foreach (MultiSampleType multiSampleType in multiSampleTypeList)
			{
				int result;
				int qualityLevels;
				if (Manager.CheckDeviceMultiSampleType((int)deviceCombo.AdapterOrdinal, deviceCombo.DeviceType, deviceCombo.BackBufferFormat, deviceCombo.IsWindowed, multiSampleType, out result, out qualityLevels))
				{
					deviceCombo.multiSampleTypeList.Add(multiSampleType);
					if (qualityLevels > multisampleQualityMax + 1)
					{
						qualityLevels = (int)(multisampleQualityMax + 1);
					}
					deviceCombo.multiSampleQualityList.Add(qualityLevels);
				}
			}
		}

		private static void BuildConflictList(EnumDeviceSettingsCombo deviceCombo)
		{
			foreach (DepthFormat depthStencilFormat in deviceCombo.depthStencilFormatList)
			{
				foreach (MultiSampleType multiSampleType in deviceCombo.multiSampleTypeList)
				{
					if (!Manager.CheckDeviceMultiSampleType((int)deviceCombo.AdapterOrdinal, deviceCombo.DeviceType, (Format)depthStencilFormat, deviceCombo.IsWindowed, multiSampleType))
					{
						EnumDepthStencilMultisampleConflict item = default(EnumDepthStencilMultisampleConflict);
						item.DepthStencilFormat = depthStencilFormat;
						item.MultisampleType = multiSampleType;
						deviceCombo.depthStencilConflictList.Add(item);
					}
				}
			}
		}

		private static void BuildPresentIntervalList(EnumDeviceInformation deviceInfo, EnumDeviceSettingsCombo deviceCombo)
		{
			for (int i = 0; i < presentIntervalList.Count; i++)
			{
				PresentInterval presentInterval = presentIntervalList[i];
				if ((!deviceCombo.IsWindowed || (presentInterval != PresentInterval.Two && presentInterval != PresentInterval.Three && presentInterval != PresentInterval.Four)) && (presentInterval == PresentInterval.Default || (deviceInfo.Caps.PresentationIntervals & presentInterval) != 0))
				{
					deviceCombo.presentIntervalList.Add(presentInterval);
				}
			}
		}

		public static void ResetPossibleDepthStencilFormats()
		{
			depthStencilPossibleList.Clear();
			depthStencilPossibleList.AddRange(new DepthFormat[6]
			{
				DepthFormat.D16,
				DepthFormat.D15S1,
				DepthFormat.D24X8,
				DepthFormat.D24S8,
				DepthFormat.D24X4S4,
				DepthFormat.D32
			});
		}

		public static void ResetPossibleMultisampleTypeList()
		{
			multiSampleTypeList.Clear();
			multiSampleTypeList.AddRange(new MultiSampleType[17]
			{
				MultiSampleType.None,
				MultiSampleType.NonMaskable,
				MultiSampleType.TwoSamples,
				MultiSampleType.ThreeSamples,
				MultiSampleType.FourSamples,
				MultiSampleType.FiveSamples,
				MultiSampleType.SixSamples,
				MultiSampleType.SevenSamples,
				MultiSampleType.EightSamples,
				MultiSampleType.NineSamples,
				MultiSampleType.TenSamples,
				MultiSampleType.ElevenSamples,
				MultiSampleType.TwelveSamples,
				MultiSampleType.ThirteenSamples,
				MultiSampleType.FourteenSamples,
				MultiSampleType.FifteenSamples,
				MultiSampleType.SixteenSamples
			});
		}

		public static void ResetPossiblePresentIntervalList()
		{
			presentIntervalList.Clear();
			presentIntervalList.AddRange(new PresentInterval[6]
			{
				PresentInterval.Immediate,
				PresentInterval.Default,
				PresentInterval.One,
				PresentInterval.Two,
				PresentInterval.Three,
				PresentInterval.Four
			});
		}

		public static void SetResolutionMinMax(uint minWidth, uint minHeight, uint maxWidth, uint maxHeight)
		{
			minimumWidth = minWidth;
			minimumHeight = minHeight;
			maximumWidth = maxHeight;
			maximumWidth = maxWidth;
		}

		public static void SetRefreshMinMax(uint minRefresh, uint maxRefresh)
		{
			minimumRefresh = minRefresh;
			maximumRefresh = maxRefresh;
		}

		public static EnumAdapterInformation GetAdapterInformation(uint ordinal)
		{
			foreach (EnumAdapterInformation adapterInformation in adapterInformationList)
			{
				if (adapterInformation.AdapterOrdinal == ordinal)
				{
					return adapterInformation;
				}
			}
			return null;
		}

		public static EnumDeviceInformation GetDeviceInfo(uint ordinal, DeviceType deviceType)
		{
			EnumAdapterInformation adapterInformation = GetAdapterInformation(ordinal);
			if (adapterInformation != null)
			{
				foreach (EnumDeviceInformation deviceInfo in adapterInformation.deviceInfoList)
				{
					if (deviceInfo.DeviceType == deviceType)
					{
						return deviceInfo;
					}
				}
			}
			return null;
		}

		public static EnumDeviceSettingsCombo GetDeviceSettingsCombo(uint ordinal, DeviceType deviceType, Format adapterFormat, Format backBufferFormat, bool isWindowed)
		{
			EnumDeviceInformation deviceInfo = GetDeviceInfo(ordinal, deviceType);
			if (deviceInfo != null)
			{
				foreach (EnumDeviceSettingsCombo deviceSettings in deviceInfo.deviceSettingsList)
				{
					if (deviceSettings.AdapterFormat == adapterFormat && deviceSettings.BackBufferFormat == backBufferFormat && deviceSettings.IsWindowed == isWindowed)
					{
						return deviceSettings;
					}
				}
			}
			return null;
		}

		public static EnumDeviceSettingsCombo GetDeviceSettingsCombo(DeviceSettings settings)
		{
			return GetDeviceSettingsCombo(settings.AdapterOrdinal, settings.DeviceType, settings.AdapterFormat, settings.PresentParameters.BackBufferFormat, settings.PresentParameters.Windowed);
		}
	}
}
