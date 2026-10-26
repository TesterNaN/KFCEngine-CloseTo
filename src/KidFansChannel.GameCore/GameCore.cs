using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Microsoft.DirectX.Direct3D;

namespace KidFansChannel.GameCore
{
	public class GameCore : IDisposable
	{
		private delegate void DisposeDelegate();

		protected static readonly IntPtr TrueIntPtr = new IntPtr(1);

		private GameCoreData State;

		private Exception lastDisplayedMessage;

		private bool isDisposed;

		private bool showFps;

		private Microsoft.DirectX.Direct3D.Font fpsFont;

		private Surface RenderTarget;

		private double nextRenderTime;

		private float nowElapsedTime;

		public EventHandler OnPause;

		public EventHandler OnResume;

		public EventHandler ApplicationIdleHandler;

		private bool isManual;

		public int ExitCode
		{
			get
			{
				return State.ApplicationExitCode;
			}
		}

		public Device Device
		{
			get
			{
				return State.Device;
			}
		}

		public SurfaceDescription BackBufferSurfaceDescription
		{
			get
			{
				return State.BackBufferSurfaceDesc;
			}
		}

		public Window GameWindow
		{
			get
			{
				return (Window)State.Window;
			}
		}

		public Size WindowClientSize
		{
			get
			{
				return State.WindowClientSize;
			}
		}

		public Size FullScreenClientSize
		{
			get
			{
				return State.FullScreenClientSize;
			}
		}

		public Size ClientSize
		{
			get
			{
				if (!IsWindowed)
				{
					return State.FullScreenClientSize;
				}
				return State.WindowClientSize;
			}
		}

		public bool IsWindowed
		{
			get
			{
				DeviceSettings currentDeviceSettings = State.CurrentDeviceSettings;
				if (currentDeviceSettings != null && currentDeviceSettings.PresentParameters != null)
				{
					return currentDeviceSettings.PresentParameters.Windowed;
				}
				return false;
			}
		}

		public float FPS
		{
			get
			{
				return State.CurrentFrameRate;
			}
		}

		public bool ShowFps
		{
			get
			{
				return showFps;
			}
			set
			{
				showFps = value;
			}
		}

		public int CurrentFrameNumber
		{
			get
			{
				return State.CurrentFrameNumber;
			}
		}

		public bool IsShowingMsgBoxOnError
		{
			get
			{
				return State.IsShowingMsgBoxOnError;
			}
		}

		public bool WindowActive
		{
			get
			{
				return State.IsActive;
			}
		}

		private bool AppStillIdle
		{
			get
			{
				if (GameWindow.WindowState == FormWindowState.Minimized)
				{
					return false;
				}
				NativeMethods.Message msg;
				if (NativeMethods.PeekMessage(out msg, IntPtr.Zero, 0u, 0u, 0u))
				{
					return false;
				}
				return true;
			}
		}

		public bool Disposed
		{
			get
			{
				return isDisposed;
			}
			set
			{
				isDisposed = value;
			}
		}

		public event RenderEventHandler FrameMove;

		public event RenderEventHandler FrameRender;

		public event EventHandler Disposing;

		public event EventHandler DeviceLost;

		public event DeviceEventHandler DeviceCreated;

		public event DeviceEventHandler DeviceReset;

		public event MouseEventHandler MouseMove;

		public event MouseEventHandler MouseDown;

		public event MouseEventHandler MouseUp;

		public event MouseEventHandler MouseWheel;

		public GameCore()
		{
			State = new GameCoreData();
		}

		public void SetWndProcCallback(WndProcCallback callback)
		{
			State.WndProcFunction = callback;
		}

		public void Initialize(bool showMessageBoxOnError)
		{
			State.WasInitCalled = true;
			Device.IsUsingEventHandlers = false;
			State.IsShowingMsgBoxOnError = showMessageBoxOnError;
			try
			{
				NativeMethods.timeBeginPeriod(1u);
			}
			catch
			{
				Debugger.Log(9, string.Empty, "Could not set time period.\r\n");
			}
			GameCoreTimer.Reset();
			State.IsInited = true;
		}

		public void CreateWindow(int windowWidth, int windowHeight)
		{
			if (State.IsInsideDeviceCallback)
			{
				throw new InvalidOperationException("You cannot create a window from inside a callback.");
			}
			State.WasWindowCreateCalled = true;
			if (!State.IsInited)
			{
				if (State.WasInitCalled)
				{
					throw new InvalidOperationException("Initialize was already called and failed.");
				}
				Initialize(true);
			}
			if (State.Window == null)
			{
				Window window = new Window(this);
				HookEvents(window);
				Size clientSize = new Size(windowWidth, windowHeight);
				window.ClientSize = clientSize;
				window.Visible = false;
				window.CreateControl();
				State.WindowClientSize = window.ClientSize;
				State.WasWindowCreated = true;
				State.Window = window;
			}
		}

		public void CreateDevice(uint adapterOrdinal, bool windowed, int fullscreenWidth, int fullscreenHeight, bool vsync)
		{
			if (State.IsInsideDeviceCallback)
			{
				throw new InvalidOperationException("You cannot create a window from inside a callback.");
			}
			if (fullscreenWidth == 0 || fullscreenHeight == 0)
			{
				fullscreenWidth = Screen.AllScreens[adapterOrdinal].Bounds.Width;
				fullscreenHeight = Screen.AllScreens[adapterOrdinal].Bounds.Height;
			}
			State.FullScreenClientSize = new Size(fullscreenWidth, fullscreenHeight);
			State.VSync = vsync;
			State.WasDeviceCreateCalled = true;
			if (!State.WasWindowCreated)
			{
				if (State.WasWindowCreateCalled)
				{
					throw new InvalidOperationException("CreateWindow was already called and failed.");
				}
				CreateWindow(State.WindowClientSize.Width, State.WindowClientSize.Height);
			}
			Enumeration.Enumerate();
			MatchOptions match = default(MatchOptions);
			match.AdapterOrdinal = MatchType.PreserveInput;
			match.DeviceType = MatchType.IgnoreInput;
			match.Windowed = MatchType.PreserveInput;
			match.AdapterFormat = MatchType.IgnoreInput;
			match.VertexProcessing = MatchType.IgnoreInput;
			match.Resolution = MatchType.PreserveInput;
			match.BackBufferFormat = MatchType.IgnoreInput;
			match.BackBufferCount = MatchType.IgnoreInput;
			match.MultiSample = MatchType.IgnoreInput;
			match.SwapEffect = MatchType.IgnoreInput;
			match.DepthFormat = MatchType.IgnoreInput;
			match.StencilFormat = MatchType.IgnoreInput;
			match.PresentFlags = MatchType.IgnoreInput;
			match.RefreshRate = MatchType.IgnoreInput;
			match.PresentInterval = MatchType.PreserveInput;
			DeviceSettings deviceSettings = new DeviceSettings();
			deviceSettings.PresentParameters = new PresentParameters();
			if (Enumeration.AdapterInformationList.Count > adapterOrdinal)
			{
				deviceSettings.AdapterOrdinal = adapterOrdinal;
			}
			else
			{
				deviceSettings.AdapterOrdinal = 0u;
			}
			deviceSettings.PresentParameters.Windowed = windowed;
			deviceSettings.PresentParameters.BackBufferWidth = (windowed ? State.WindowClientSize.Width : State.FullScreenClientSize.Width);
			deviceSettings.PresentParameters.BackBufferHeight = (windowed ? State.WindowClientSize.Height : State.FullScreenClientSize.Height);
			deviceSettings.PresentParameters.PresentationInterval = (vsync ? PresentInterval.One : PresentInterval.Default);
			try
			{
				deviceSettings = FindValidDeviceSettings(deviceSettings, match);
			}
			catch (Exception e)
			{
				if (deviceSettings.PresentParameters.Windowed)
				{
					DisplayErrorMessage(e);
					throw;
				}
				try
				{
					deviceSettings.PresentParameters.Windowed = true;
					deviceSettings.PresentParameters.BackBufferWidth = State.WindowClientSize.Width;
					deviceSettings.PresentParameters.BackBufferHeight = State.WindowClientSize.Height;
					deviceSettings = FindValidDeviceSettings(deviceSettings, match);
				}
				catch (Exception e2)
				{
					DisplayErrorMessage(e2);
					throw;
				}
			}
			Caps deviceCaps = Manager.GetDeviceCaps((int)deviceSettings.AdapterOrdinal, deviceSettings.DeviceType);
			ModifyDeviceSettings(deviceSettings, deviceCaps);
			GameWindow.SetScreen((int)adapterOrdinal);
			State.WindowLocation = State.Window.Location;
			ChangeDevice(deviceSettings);
		}

		private void ModifyDeviceSettings(DeviceSettings settings, Caps caps)
		{
			if (!caps.DeviceCaps.SupportsHardwareTransformAndLight || caps.VertexShaderVersion < new Version(1, 1))
			{
				settings.BehaviorFlags = CreateFlags.SoftwareVertexProcessing;
			}
			else
			{
				settings.BehaviorFlags = CreateFlags.HardwareVertexProcessing;
			}
			if (caps.DeviceCaps.SupportsPureDevice && (settings.BehaviorFlags & CreateFlags.HardwareVertexProcessing) != 0)
			{
				settings.BehaviorFlags |= CreateFlags.PureDevice;
			}
		}

		public virtual void ToggleFullscreen()
		{
			Pause(true, true);
			DeviceSettings deviceSettings = State.CurrentDeviceSettings.Clone();
			deviceSettings.PresentParameters.Windowed = !deviceSettings.PresentParameters.Windowed;
			MatchOptions match = default(MatchOptions);
			match.AdapterOrdinal = MatchType.PreserveInput;
			match.DeviceType = MatchType.ClosestToInput;
			match.Windowed = MatchType.PreserveInput;
			match.AdapterFormat = MatchType.IgnoreInput;
			match.VertexProcessing = MatchType.ClosestToInput;
			match.BackBufferFormat = MatchType.IgnoreInput;
			match.BackBufferCount = MatchType.ClosestToInput;
			match.MultiSample = MatchType.ClosestToInput;
			match.SwapEffect = MatchType.ClosestToInput;
			match.DepthFormat = MatchType.ClosestToInput;
			match.StencilFormat = MatchType.ClosestToInput;
			match.PresentFlags = MatchType.ClosestToInput;
			match.RefreshRate = MatchType.IgnoreInput;
			match.PresentInterval = MatchType.PreserveInput;
			Size size = ((!deviceSettings.PresentParameters.Windowed) ? State.FullScreenClientSize : State.WindowClientSize);
			if (size.Width > 0 && size.Height > 0)
			{
				match.Resolution = MatchType.PreserveInput;
				deviceSettings.PresentParameters.BackBufferWidth = size.Width;
				deviceSettings.PresentParameters.BackBufferHeight = size.Height;
			}
			else
			{
				match.Resolution = MatchType.IgnoreInput;
			}
			try
			{
				deviceSettings = FindValidDeviceSettings(deviceSettings, match);
				ChangeDevice(deviceSettings);
				if (IsWindowed)
				{
					GameWindow.ClientSize = State.WindowClientSize;
					GameWindow.Location = State.WindowLocation;
					GameWindow.TopMost = false;
				}
				else
				{
					State.WindowLocation = GameWindow.Location;
				}
				State.CurrentDeviceSettings = deviceSettings;
			}
			finally
			{
				Pause(false, false);
			}
		}

		private DeviceSettings FindValidDeviceSettings(DeviceSettings settings, MatchOptions match)
		{
			DeviceSettings settings2 = BuildOptimalDeviceSettings(settings, match);
			float bestScore = -1f;
			EnumDeviceSettingsCombo deviceCombo = new EnumDeviceSettingsCombo();
			for (int i = 0; i < Enumeration.AdapterInformationList.Count; i++)
			{
				EnumAdapterInformation enumAdapterInformation = Enumeration.AdapterInformationList[i];
				DisplayMode currentDisplayMode = Manager.Adapters[(int)enumAdapterInformation.AdapterOrdinal].CurrentDisplayMode;
				for (int j = 0; j < enumAdapterInformation.deviceInfoList.Count; j++)
				{
					EnumDeviceInformation enumDeviceInformation = enumAdapterInformation.deviceInfoList[j];
					for (int k = 0; k < enumDeviceInformation.deviceSettingsList.Count; k++)
					{
						EnumDeviceSettingsCombo enumDeviceSettingsCombo = enumDeviceInformation.deviceSettingsList[k];
						if ((!enumDeviceSettingsCombo.IsWindowed || enumDeviceSettingsCombo.AdapterFormat == currentDisplayMode.Format) && DoesDeviceComboMatchPreserveOptions(enumDeviceSettingsCombo, settings, match))
						{
							float bestCandidate = RankDeviceCombo(enumDeviceSettingsCombo, settings2, currentDisplayMode);
							if (bestCandidate > bestScore)
							{
								deviceCombo = enumDeviceSettingsCombo;
								bestScore = bestCandidate;
							}
						}
					}
				}
			}
			if (bestScore == -1f)
			{
				throw new NoCompatibleDevicesException();
			}
			return BuildValidDeviceSettings(deviceCombo, settings, match);
		}

		private DeviceSettings BuildOptimalDeviceSettings(DeviceSettings settings, MatchOptions match)
		{
			DeviceSettings deviceSettings = new DeviceSettings();
			deviceSettings.PresentParameters = new PresentParameters();
			if (match.AdapterOrdinal == MatchType.IgnoreInput)
			{
				deviceSettings.AdapterOrdinal = 0u;
			}
			else
			{
				deviceSettings.AdapterOrdinal = settings.AdapterOrdinal;
			}
			if (match.DeviceType == MatchType.IgnoreInput)
			{
				deviceSettings.DeviceType = DeviceType.Hardware;
			}
			else
			{
				deviceSettings.DeviceType = settings.DeviceType;
			}
			if (match.Windowed == MatchType.IgnoreInput)
			{
				deviceSettings.PresentParameters.Windowed = true;
			}
			else
			{
				deviceSettings.PresentParameters.Windowed = settings.PresentParameters.Windowed;
			}
			if (match.AdapterFormat == MatchType.IgnoreInput)
			{
				DisplayMode currentDisplayMode = Manager.Adapters[(int)deviceSettings.AdapterOrdinal].CurrentDisplayMode;
				if (deviceSettings.PresentParameters.Windowed || ManagedUtility.GetColorChannelBits(currentDisplayMode.Format) >= 8)
				{
					deviceSettings.AdapterFormat = currentDisplayMode.Format;
				}
				else
				{
					deviceSettings.AdapterFormat = Format.X8R8G8B8;
				}
			}
			else
			{
				deviceSettings.AdapterFormat = settings.AdapterFormat;
			}
			if (match.VertexProcessing == MatchType.IgnoreInput)
			{
				deviceSettings.BehaviorFlags = CreateFlags.HardwareVertexProcessing;
			}
			else
			{
				deviceSettings.BehaviorFlags = settings.BehaviorFlags;
			}
			if (match.Resolution == MatchType.IgnoreInput)
			{
				if (deviceSettings.PresentParameters.Windowed)
				{
					deviceSettings.PresentParameters.BackBufferWidth = State.WindowClientSize.Width;
					deviceSettings.PresentParameters.BackBufferHeight = State.WindowClientSize.Height;
				}
				else
				{
					DisplayMode currentDisplayMode2 = Manager.Adapters[(int)deviceSettings.AdapterOrdinal].CurrentDisplayMode;
					deviceSettings.PresentParameters.BackBufferWidth = currentDisplayMode2.Width;
					deviceSettings.PresentParameters.BackBufferHeight = currentDisplayMode2.Height;
				}
			}
			else
			{
				deviceSettings.PresentParameters.BackBufferWidth = settings.PresentParameters.BackBufferWidth;
				deviceSettings.PresentParameters.BackBufferHeight = settings.PresentParameters.BackBufferHeight;
			}
			if (match.BackBufferFormat == MatchType.IgnoreInput)
			{
				deviceSettings.PresentParameters.BackBufferFormat = deviceSettings.AdapterFormat;
			}
			else
			{
				deviceSettings.PresentParameters.BackBufferFormat = settings.PresentParameters.BackBufferFormat;
			}
			if (match.BackBufferCount == MatchType.IgnoreInput)
			{
				deviceSettings.PresentParameters.BackBufferCount = 2;
			}
			else
			{
				deviceSettings.PresentParameters.BackBufferCount = settings.PresentParameters.BackBufferCount;
			}
			if (match.MultiSample == MatchType.IgnoreInput)
			{
				deviceSettings.PresentParameters.MultiSampleQuality = 0;
			}
			else
			{
				deviceSettings.PresentParameters.MultiSampleQuality = settings.PresentParameters.MultiSampleQuality;
			}
			if (match.SwapEffect == MatchType.IgnoreInput)
			{
				deviceSettings.PresentParameters.SwapEffect = SwapEffect.Discard;
			}
			else
			{
				deviceSettings.PresentParameters.SwapEffect = settings.PresentParameters.SwapEffect;
			}
			if (match.DepthFormat == MatchType.IgnoreInput && match.StencilFormat == MatchType.IgnoreInput)
			{
				uint colorChannelBits = ManagedUtility.GetColorChannelBits(deviceSettings.PresentParameters.BackBufferFormat);
				if (colorChannelBits >= 8)
				{
					deviceSettings.PresentParameters.AutoDepthStencilFormat = DepthFormat.D32;
				}
				else
				{
					deviceSettings.PresentParameters.AutoDepthStencilFormat = DepthFormat.D16;
				}
			}
			else
			{
				deviceSettings.PresentParameters.AutoDepthStencilFormat = settings.PresentParameters.AutoDepthStencilFormat;
			}
			if (match.PresentFlags == MatchType.IgnoreInput)
			{
				deviceSettings.PresentParameters.PresentFlag = PresentFlag.DiscardDepthStencil;
			}
			else
			{
				deviceSettings.PresentParameters.PresentFlag = settings.PresentParameters.PresentFlag;
			}
			if (match.RefreshRate == MatchType.IgnoreInput)
			{
				deviceSettings.PresentParameters.FullScreenRefreshRateInHz = 0;
			}
			else
			{
				deviceSettings.PresentParameters.FullScreenRefreshRateInHz = settings.PresentParameters.FullScreenRefreshRateInHz;
			}
			if (match.PresentInterval == MatchType.IgnoreInput)
			{
				deviceSettings.PresentParameters.PresentationInterval = PresentInterval.Immediate;
			}
			else
			{
				deviceSettings.PresentParameters.PresentationInterval = settings.PresentParameters.PresentationInterval;
			}
			return deviceSettings;
		}

		private DeviceSettings BuildValidDeviceSettings(EnumDeviceSettingsCombo deviceCombo, DeviceSettings settings, MatchOptions match)
		{
			DeviceSettings deviceSettings = new DeviceSettings();
			DisplayMode currentDisplayMode = Manager.Adapters[(int)deviceCombo.AdapterOrdinal].CurrentDisplayMode;
			CreateFlags createFlags = (CreateFlags)0;
			if (match.VertexProcessing == MatchType.PreserveInput)
			{
				createFlags = settings.BehaviorFlags;
			}
			else if (match.VertexProcessing == MatchType.IgnoreInput)
			{
				createFlags = ((!deviceCombo.deviceInformation.Caps.DeviceCaps.SupportsHardwareTransformAndLight) ? (createFlags | CreateFlags.SoftwareVertexProcessing) : (createFlags | CreateFlags.HardwareVertexProcessing));
			}
			else
			{
				createFlags = settings.BehaviorFlags;
				if (!deviceCombo.deviceInformation.Caps.DeviceCaps.SupportsHardwareTransformAndLight && ((createFlags & CreateFlags.HardwareVertexProcessing) != 0 || (createFlags & CreateFlags.MixedVertexProcessing) != 0))
				{
					createFlags &= ~CreateFlags.HardwareVertexProcessing;
					createFlags &= ~CreateFlags.MixedVertexProcessing;
					createFlags |= CreateFlags.SoftwareVertexProcessing;
				}
				if ((createFlags & CreateFlags.HardwareVertexProcessing) == 0 && (createFlags & CreateFlags.MixedVertexProcessing) == 0 && (createFlags & CreateFlags.SoftwareVertexProcessing) == 0)
				{
					createFlags = ((!deviceCombo.deviceInformation.Caps.DeviceCaps.SupportsHardwareTransformAndLight) ? (createFlags | CreateFlags.SoftwareVertexProcessing) : (createFlags | CreateFlags.HardwareVertexProcessing));
				}
			}
			DisplayMode displayMode = new DisplayMode();
			if (match.Resolution == MatchType.PreserveInput)
			{
				displayMode.Width = settings.PresentParameters.BackBufferWidth;
				displayMode.Height = settings.PresentParameters.BackBufferHeight;
			}
			else
			{
				DisplayMode displayMode2 = new DisplayMode();
				if (match.Resolution == MatchType.ClosestToInput && settings.PresentParameters.BackBufferWidth != 0 && settings.PresentParameters.BackBufferWidth != 0)
				{
					displayMode2.Width = settings.PresentParameters.BackBufferWidth;
					displayMode2.Height = settings.PresentParameters.BackBufferHeight;
				}
				else if (deviceCombo.IsWindowed)
				{
					displayMode2.Width = State.WindowClientSize.Width;
					displayMode2.Height = State.WindowClientSize.Height;
				}
				else
				{
					displayMode2.Width = currentDisplayMode.Width;
					displayMode2.Height = currentDisplayMode.Height;
				}
				displayMode = FindValidResolution(deviceCombo, displayMode2);
			}
			uint backBufferCount;
			if (match.BackBufferCount == MatchType.PreserveInput)
			{
				backBufferCount = (uint)settings.PresentParameters.BackBufferCount;
			}
			else if (match.BackBufferCount == MatchType.IgnoreInput)
			{
				backBufferCount = 2u;
			}
			else
			{
				backBufferCount = (uint)settings.PresentParameters.BackBufferCount;
				if (backBufferCount > 3)
				{
					backBufferCount = 3u;
				}
				if (backBufferCount < 1)
				{
					backBufferCount = 1u;
				}
			}
			MultiSampleType multiSampleType;
			uint multiSampleQuality;
			if (settings.PresentParameters.SwapEffect != SwapEffect.Discard)
			{
				multiSampleType = MultiSampleType.None;
				multiSampleQuality = 0u;
			}
			else if (match.BackBufferCount == MatchType.PreserveInput)
			{
				multiSampleType = settings.PresentParameters.MultiSample;
				multiSampleQuality = (uint)settings.PresentParameters.MultiSampleQuality;
			}
			else if (match.BackBufferCount == MatchType.IgnoreInput)
			{
				multiSampleType = MultiSampleType.None;
				multiSampleQuality = 0u;
			}
			else if (match.BackBufferCount == MatchType.ClosestToInput)
			{
				multiSampleType = MultiSampleType.None;
				multiSampleQuality = 0u;
				for (int i = 0; i < deviceCombo.multiSampleTypeList.Count; i++)
				{
					MultiSampleType multiSampleType2 = deviceCombo.multiSampleTypeList[i];
					uint qualityLevels = (uint)deviceCombo.multiSampleQualityList[i];
					if (Math.Abs(multiSampleType2 - settings.PresentParameters.MultiSample) < Math.Abs(multiSampleType - settings.PresentParameters.MultiSample))
					{
						multiSampleType = multiSampleType2;
						multiSampleQuality = (uint)Math.Min(qualityLevels - 1, settings.PresentParameters.MultiSampleQuality);
					}
				}
			}
			else
			{
				multiSampleType = MultiSampleType.None;
				multiSampleQuality = 0u;
			}
			SwapEffect swapEffect;
			if (match.SwapEffect == MatchType.PreserveInput)
			{
				swapEffect = settings.PresentParameters.SwapEffect;
			}
			else if (match.SwapEffect == MatchType.IgnoreInput)
			{
				swapEffect = SwapEffect.Discard;
			}
			else
			{
				swapEffect = settings.PresentParameters.SwapEffect;
				if (swapEffect != SwapEffect.Discard && swapEffect != SwapEffect.Flip && swapEffect != SwapEffect.Copy)
				{
					swapEffect = SwapEffect.Discard;
				}
			}
			int[] scores = new int[deviceCombo.depthStencilFormatList.Count];
			uint colorChannelBits = ManagedUtility.GetColorChannelBits(deviceCombo.BackBufferFormat);
			uint depthBits = ManagedUtility.GetDepthBits(settings.PresentParameters.AutoDepthStencilFormat);
			for (int j = 0; j < deviceCombo.depthStencilFormatList.Count; j++)
			{
				DepthFormat format = deviceCombo.depthStencilFormatList[j];
				uint depthBits2 = ManagedUtility.GetDepthBits(format);
				int depthScore;
				if (match.DepthFormat != MatchType.PreserveInput)
				{
					depthScore = ((match.DepthFormat != MatchType.IgnoreInput) ? Math.Abs((int)(depthBits2 - depthBits)) : Math.Abs((int)(depthBits2 - colorChannelBits * 4)));
				}
				else
				{
					depthScore = ((depthBits2 != depthBits) ? 10000 : 0);
				}
				scores[j] = depthScore;
			}
			uint stencilBits = ManagedUtility.GetStencilBits(settings.PresentParameters.AutoDepthStencilFormat);
			for (int k = 0; k < deviceCombo.depthStencilFormatList.Count; k++)
			{
				DepthFormat format2 = deviceCombo.depthStencilFormatList[k];
				int stencilScore = scores[k];
				uint stencilBits2 = ManagedUtility.GetStencilBits(format2);
				if (match.StencilFormat != MatchType.PreserveInput)
				{
					stencilScore = ((match.StencilFormat != MatchType.IgnoreInput) ? (stencilScore + Math.Abs((int)(stencilBits2 - stencilBits))) : (stencilScore + (int)stencilBits2));
				}
				else
				{
					stencilScore = ((stencilBits2 != stencilBits) ? (stencilScore + 10000) : stencilScore);
				}
				scores[k] = stencilScore;
			}
			int bestScore = 100000;
			int bestIndex = -1;
			for (int l = 0; l < scores.Length; l++)
			{
				if (scores[l] < bestScore)
				{
					bestScore = scores[l];
					bestIndex = l;
				}
			}
			DepthFormat autoDepthStencilFormat;
			bool depthFound;
			if (bestIndex >= 0)
			{
				autoDepthStencilFormat = deviceCombo.depthStencilFormatList[bestIndex];
				depthFound = true;
			}
			else
			{
				autoDepthStencilFormat = DepthFormat.Unknown;
				depthFound = false;
			}
			PresentFlag presentFlag;
			if (match.PresentFlags == MatchType.PreserveInput)
			{
				presentFlag = settings.PresentParameters.PresentFlag;
			}
			else if (match.PresentFlags == MatchType.IgnoreInput)
			{
				presentFlag = PresentFlag.None;
				if (depthFound)
				{
					presentFlag = PresentFlag.DiscardDepthStencil;
				}
			}
			else
			{
				presentFlag = settings.PresentParameters.PresentFlag;
				if (depthFound)
				{
					presentFlag |= PresentFlag.DiscardDepthStencil;
				}
			}
			if (deviceCombo.IsWindowed)
			{
				displayMode.RefreshRate = 0;
			}
			else if (match.RefreshRate == MatchType.PreserveInput)
			{
				displayMode.RefreshRate = settings.PresentParameters.FullScreenRefreshRateInHz;
			}
			else
			{
				uint wantedRefresh = (uint)((match.RefreshRate != MatchType.ClosestToInput) ? currentDisplayMode.RefreshRate : settings.PresentParameters.FullScreenRefreshRateInHz);
				displayMode.RefreshRate = 0;
				if (wantedRefresh != 0)
				{
					int closestDiff = 100000;
					for (int m = 0; m < deviceCombo.adapterInformation.displayModeList.Count; m++)
					{
						DisplayMode candidateMode = deviceCombo.adapterInformation.displayModeList[m];
						if (candidateMode.Format != deviceCombo.AdapterFormat || candidateMode.Height != displayMode.Height || candidateMode.Width != displayMode.Width)
						{
							continue;
						}
						int refreshDiff = Math.Abs(candidateMode.RefreshRate - (int)wantedRefresh);
						if (refreshDiff < closestDiff)
						{
							displayMode.RefreshRate = candidateMode.RefreshRate;
							closestDiff = refreshDiff;
							if (closestDiff == 0)
							{
								break;
							}
						}
					}
				}
			}
			PresentInterval presentationInterval;
			if (match.PresentInterval == MatchType.PreserveInput)
			{
				presentationInterval = settings.PresentParameters.PresentationInterval;
			}
			else if (match.PresentInterval != MatchType.IgnoreInput)
			{
				presentationInterval = (deviceCombo.presentIntervalList.Contains(settings.PresentParameters.PresentationInterval) ? settings.PresentParameters.PresentationInterval : (deviceCombo.IsWindowed ? PresentInterval.Immediate : PresentInterval.Default));
			}
			else
			{
				presentationInterval = PresentInterval.Immediate;
			}
			deviceSettings.AdapterOrdinal = deviceCombo.AdapterOrdinal;
			deviceSettings.DeviceType = deviceCombo.DeviceType;
			deviceSettings.AdapterFormat = deviceCombo.AdapterFormat;
			deviceSettings.BehaviorFlags = createFlags;
			deviceSettings.PresentParameters = new PresentParameters();
			deviceSettings.PresentParameters.BackBufferWidth = displayMode.Width;
			deviceSettings.PresentParameters.BackBufferHeight = displayMode.Height;
			deviceSettings.PresentParameters.BackBufferFormat = deviceCombo.BackBufferFormat;
			deviceSettings.PresentParameters.BackBufferCount = (int)backBufferCount;
			deviceSettings.PresentParameters.MultiSample = multiSampleType;
			deviceSettings.PresentParameters.MultiSampleQuality = (int)multiSampleQuality;
			deviceSettings.PresentParameters.SwapEffect = swapEffect;
			deviceSettings.PresentParameters.DeviceWindow = State.Window;
			deviceSettings.PresentParameters.Windowed = deviceCombo.IsWindowed;
			deviceSettings.PresentParameters.EnableAutoDepthStencil = depthFound;
			deviceSettings.PresentParameters.AutoDepthStencilFormat = autoDepthStencilFormat;
			deviceSettings.PresentParameters.PresentFlag = presentFlag;
			deviceSettings.PresentParameters.FullScreenRefreshRateInHz = displayMode.RefreshRate;
			deviceSettings.PresentParameters.PresentationInterval = presentationInterval;
			deviceSettings.PresentParameters.ForceNoMultiThreadedFlag = false;
			return deviceSettings;
		}

		private bool DoesDeviceComboMatchPreserveOptions(EnumDeviceSettingsCombo deviceCombo, DeviceSettings settings, MatchOptions match)
		{
			if (match.AdapterOrdinal == MatchType.PreserveInput && deviceCombo.AdapterOrdinal != settings.AdapterOrdinal)
			{
				return false;
			}
			if (match.DeviceType == MatchType.PreserveInput && deviceCombo.DeviceType != settings.DeviceType)
			{
				return false;
			}
			if (match.Windowed == MatchType.PreserveInput && deviceCombo.IsWindowed != settings.PresentParameters.Windowed)
			{
				return false;
			}
			if (match.AdapterFormat == MatchType.PreserveInput && deviceCombo.AdapterFormat != settings.AdapterFormat)
			{
				return false;
			}
			if (match.VertexProcessing == MatchType.PreserveInput && (settings.BehaviorFlags & CreateFlags.HardwareVertexProcessing) != 0 && deviceCombo.deviceInformation.Caps.DeviceCaps.SupportsHardwareTransformAndLight)
			{
				return false;
			}
			if (match.Resolution == MatchType.PreserveInput && !deviceCombo.IsWindowed)
			{
				bool formatSupported = false;
				for (int i = 0; i < deviceCombo.adapterInformation.displayModeList.Count; i++)
				{
					DisplayMode displayMode = deviceCombo.adapterInformation.displayModeList[i];
					if (displayMode.Format == deviceCombo.AdapterFormat && displayMode.Width == settings.PresentParameters.BackBufferWidth && displayMode.Height == settings.PresentParameters.BackBufferHeight)
					{
						formatSupported = true;
						break;
					}
				}
				if (!formatSupported)
				{
					return false;
				}
			}
			if (match.BackBufferFormat == MatchType.PreserveInput && deviceCombo.BackBufferFormat != settings.PresentParameters.BackBufferFormat)
			{
				return false;
			}
			if (match.MultiSample == MatchType.PreserveInput)
			{
				bool sampleSupported = false;
				for (int j = 0; j < deviceCombo.multiSampleTypeList.Count; j++)
				{
					MultiSampleType multiSampleType = deviceCombo.multiSampleTypeList[j];
					uint quality = (uint)deviceCombo.multiSampleQualityList[j];
					if (multiSampleType == settings.PresentParameters.MultiSample && quality >= settings.PresentParameters.MultiSampleQuality)
					{
						sampleSupported = true;
						break;
					}
				}
				if (!sampleSupported)
				{
					return false;
				}
			}
			if (match.DepthFormat == MatchType.PreserveInput && match.StencilFormat == MatchType.PreserveInput && settings.PresentParameters.AutoDepthStencilFormat != DepthFormat.Unknown && !deviceCombo.depthStencilFormatList.Contains(settings.PresentParameters.AutoDepthStencilFormat))
			{
				return false;
			}
			if (match.DepthFormat == MatchType.PreserveInput && settings.PresentParameters.AutoDepthStencilFormat != DepthFormat.Unknown)
			{
				bool depthSupported = false;
				uint depthBits = ManagedUtility.GetDepthBits(settings.PresentParameters.AutoDepthStencilFormat);
				for (int k = 0; k < deviceCombo.depthStencilFormatList.Count; k++)
				{
					DepthFormat format = deviceCombo.depthStencilFormatList[k];
					uint depthBits2 = ManagedUtility.GetDepthBits(format);
					if (depthBits2 - depthBits == 0)
					{
						depthSupported = true;
					}
				}
				if (!depthSupported)
				{
					return false;
				}
			}
			if (match.StencilFormat == MatchType.PreserveInput && settings.PresentParameters.AutoDepthStencilFormat != DepthFormat.Unknown)
			{
				bool stencilSupported = false;
				uint stencilBits = ManagedUtility.GetStencilBits(settings.PresentParameters.AutoDepthStencilFormat);
				for (int l = 0; l < deviceCombo.depthStencilFormatList.Count; l++)
				{
					DepthFormat format2 = deviceCombo.depthStencilFormatList[l];
					uint stencilBits2 = ManagedUtility.GetStencilBits(format2);
					if (stencilBits2 - stencilBits == 0)
					{
						stencilSupported = true;
					}
				}
				if (!stencilSupported)
				{
					return false;
				}
			}
			if (match.RefreshRate == MatchType.PreserveInput)
			{
				bool refreshSupported = false;
				for (int m = 0; m < deviceCombo.adapterInformation.displayModeList.Count; m++)
				{
					DisplayMode displayMode2 = deviceCombo.adapterInformation.displayModeList[m];
					if (displayMode2.Format == deviceCombo.AdapterFormat && displayMode2.RefreshRate == settings.PresentParameters.FullScreenRefreshRateInHz)
					{
						refreshSupported = true;
						break;
					}
				}
				if (!refreshSupported)
				{
					return false;
				}
			}
			if (match.PresentInterval == MatchType.PreserveInput && !deviceCombo.presentIntervalList.Contains(settings.PresentParameters.PresentationInterval))
			{
				return false;
			}
			return true;
		}

		private float RankDeviceCombo(EnumDeviceSettingsCombo deviceCombo, DeviceSettings settings, DisplayMode adapterMode)
		{
			float score = 0f;
			if (deviceCombo.AdapterOrdinal == settings.AdapterOrdinal)
			{
				score += 1000f;
			}
			if (deviceCombo.DeviceType == settings.DeviceType)
			{
				score += 100f;
			}
			if (deviceCombo.DeviceType == DeviceType.Hardware)
			{
				score += 0.1f;
			}
			if (deviceCombo.IsWindowed == settings.PresentParameters.Windowed)
			{
				score += 10f;
			}
			if (deviceCombo.AdapterFormat == settings.AdapterFormat)
			{
				score++;
			}
			else
			{
				int colorDiff = Math.Abs((int)(ManagedUtility.GetColorChannelBits(deviceCombo.AdapterFormat) - ManagedUtility.GetColorChannelBits(settings.AdapterFormat)));
				float colorBonus = Math.Max(0.9f - (float)colorDiff * 0.2f, 0f);
				score += colorBonus * 1f;
			}
			if (!deviceCombo.IsWindowed && ((ManagedUtility.GetColorChannelBits(adapterMode.Format) < 8) ? (deviceCombo.AdapterFormat == Format.X8R8G8B8) : (deviceCombo.AdapterFormat == adapterMode.Format)))
			{
				score += 0.1f;
			}
			if (((settings.BehaviorFlags & CreateFlags.HardwareVertexProcessing) != 0 || (settings.BehaviorFlags & CreateFlags.MixedVertexProcessing) != 0) && deviceCombo.deviceInformation.Caps.DeviceCaps.SupportsHardwareTransformAndLight)
			{
				score++;
			}
			if (deviceCombo.deviceInformation.Caps.DeviceCaps.SupportsHardwareTransformAndLight)
			{
				score += 0.1f;
			}
			bool resolutionMatched = false;
			for (int i = 0; i < deviceCombo.adapterInformation.displayModeList.Count; i++)
			{
				DisplayMode displayMode = deviceCombo.adapterInformation.displayModeList[i];
				if (displayMode.Format == deviceCombo.AdapterFormat && displayMode.Width == settings.PresentParameters.BackBufferWidth && displayMode.Height == settings.PresentParameters.BackBufferHeight)
				{
					resolutionMatched = true;
				}
			}
			if (resolutionMatched)
			{
				score++;
			}
			if (deviceCombo.BackBufferFormat == settings.PresentParameters.BackBufferFormat)
			{
				score++;
			}
			else
			{
				int bufferColorDiff = Math.Abs((int)(ManagedUtility.GetColorChannelBits(deviceCombo.BackBufferFormat) - ManagedUtility.GetColorChannelBits(settings.PresentParameters.BackBufferFormat)));
				float bufferColorBonus = Math.Max(0.9f - (float)bufferColorDiff * 0.2f, 0f);
				score += bufferColorBonus * 1f;
			}
			if (deviceCombo.BackBufferFormat == deviceCombo.AdapterFormat)
			{
				score += 0.1f;
			}
			bool flag2 = false;
			for (int j = 0; j < deviceCombo.multiSampleTypeList.Count; j++)
			{
				MultiSampleType multiSampleType = deviceCombo.multiSampleTypeList[j];
				uint num6 = (uint)deviceCombo.multiSampleQualityList[j];
				if (multiSampleType == settings.PresentParameters.MultiSample && num6 >= settings.PresentParameters.MultiSampleQuality)
				{
					flag2 = true;
					break;
				}
			}
			if (flag2)
			{
				score++;
			}
			if (deviceCombo.depthStencilFormatList.Contains(settings.PresentParameters.AutoDepthStencilFormat))
			{
				score++;
			}
			bool flag3 = false;
			for (int k = 0; k < deviceCombo.adapterInformation.displayModeList.Count; k++)
			{
				DisplayMode displayMode2 = deviceCombo.adapterInformation.displayModeList[k];
				if (displayMode2.Format == deviceCombo.AdapterFormat && displayMode2.RefreshRate == settings.PresentParameters.FullScreenRefreshRateInHz)
				{
					flag3 = true;
				}
			}
			if (flag3)
			{
				score++;
			}
			if (deviceCombo.presentIntervalList.Contains(settings.PresentParameters.PresentationInterval))
			{
				score++;
			}
			return score;
		}

		private DisplayMode FindValidResolution(EnumDeviceSettingsCombo deviceCombo, DisplayMode displayMode)
		{
			DisplayMode result = displayMode;
			if (deviceCombo.IsWindowed)
			{
				EnumAdapterInformation adapterInformation = Enumeration.GetAdapterInformation(deviceCombo.AdapterOrdinal);
				Screen screen = null;
				string text = adapterInformation.AdapterInformation.DeviceName.ToLower();
				Screen[] allScreens = Screen.AllScreens;
				foreach (Screen screen2 in allScreens)
				{
					string text2 = screen2.DeviceName.ToLower();
					if (text2.IndexOf("\0") > 0)
					{
						text2 = text2.Substring(0, text2.IndexOf("\0"));
					}
					if (text2 == text)
					{
						screen = screen2;
						break;
					}
				}
				if (result.Width > screen.WorkingArea.Width)
				{
					result.Width = screen.WorkingArea.Width;
				}
				if (result.Height > screen.WorkingArea.Height)
				{
					result.Height = screen.WorkingArea.Height;
				}
			}
			else
			{
				int num = 100000;
				for (int j = 0; j < deviceCombo.adapterInformation.displayModeList.Count; j++)
				{
					DisplayMode displayMode2 = deviceCombo.adapterInformation.displayModeList[j];
					if (displayMode2.Format != deviceCombo.AdapterFormat)
					{
						continue;
					}
					int num2 = Math.Abs(displayMode2.Width - displayMode.Width) + Math.Abs(displayMode2.Height - displayMode.Height);
					if (num2 < num)
					{
						result = displayMode;
						num = num2;
						if (num == 0)
						{
							break;
						}
					}
				}
				if (result.Width == 0)
				{
					throw new NoCompatibleDevicesException();
				}
			}
			return result;
		}

		private void ChangeDevice(DeviceSettings newDeviceSettings)
		{
			if (isDisposed)
			{
				return;
			}
			DeviceSettings deviceSettings = null;
			if (State.CurrentDeviceSettings != null)
			{
				deviceSettings = State.CurrentDeviceSettings.Clone();
			}
			Pause(true, true);
			newDeviceSettings.BehaviorFlags |= CreateFlags.MultiThreaded;
			if (newDeviceSettings.PresentParameters.Windowed && State.Window != null)
			{
				Size windowClientSize = State.WindowClientSize;
				windowClientSize = new Size(newDeviceSettings.PresentParameters.BackBufferWidth, newDeviceSettings.PresentParameters.BackBufferHeight);
				State.WindowClientSize = windowClientSize;
			}
			State.CurrentDeviceSettings = newDeviceSettings;
			if (deviceSettings != null && deviceSettings.AdapterOrdinal == newDeviceSettings.AdapterOrdinal && deviceSettings.DeviceType == newDeviceSettings.DeviceType && deviceSettings.BehaviorFlags == newDeviceSettings.BehaviorFlags)
			{
				try
				{
					ResetEnvironment();
				}
				catch
				{
					Pause(false, false);
					State.IsDeviceLost = true;
				}
			}
			else
			{
				if (deviceSettings != null)
				{
					CleanupEnvironment(false);
				}
				Device device;
				try
				{
					State.Window.TopMost = true;
					device = new Device((int)newDeviceSettings.AdapterOrdinal, newDeviceSettings.DeviceType, State.Window, newDeviceSettings.BehaviorFlags, newDeviceSettings.PresentParameters);
					RenderTarget = device.GetRenderTarget(0);
					State.Window.TopMost = false;
				}
				catch (Exception inner)
				{
					Pause(false, false);
					CreatingDeviceException e = new CreatingDeviceException(inner);
					DisplayErrorMessage(e);
					throw;
				}
				device.DeviceLost += OnDeviceLost;
				device.DeviceReset += OnDeviceReset;
				device.Disposing += OnDeviceDisposing;
				State.Device = device;
				try
				{
					InitializeEnvironment();
				}
				catch
				{
					Pause(false, false);
					throw;
				}
				Enumeration.Enumerate();
				Enumeration.GetAdapterInformation(newDeviceSettings.AdapterOrdinal);
			}
			State.AdapterMonitor = Manager.GetAdapterMonitor((int)newDeviceSettings.AdapterOrdinal);
			if (!newDeviceSettings.PresentParameters.Windowed)
			{
				State.FullScreenClientSize = new Size(newDeviceSettings.PresentParameters.BackBufferWidth, newDeviceSettings.PresentParameters.BackBufferHeight);
			}
			Pause(false, false);
			State.WasDeviceCreated = true;
		}

		public void ResetEnvironment()
		{
			Device device = State.Device;
			if (device == null)
			{
				throw new InvalidOperationException("The device should not be null.");
			}
			if (DeviceLost != null)
			{
				DeviceLost(this, EventArgs.Empty);
			}
			AdjustWindowStyle(GameWindow, IsWindowed);
			try
			{
				if (RenderTarget != null)
				{
					RenderTarget.Dispose();
					RenderTarget = null;
				}
				device.Reset(State.CurrentDeviceSettings.PresentParameters);
				RenderTarget = device.GetRenderTarget(0);
			}
			catch (Exception ex)
			{
				if (ex is MediaNotFoundException)
				{
					DisplayErrorMessage(ex as MediaNotFoundException);
				}
				else if (!(ex is DeviceLostException))
				{
					DisplayErrorMessage(new ResettingDeviceException(ex));
				}
				OnDeviceLost(null, EventArgs.Empty);
				if (!(ex is DeviceLostException))
				{
					throw;
				}
			}
		}

		public List<string> GetAdaptersList()
		{
			List<string> list = new List<string>();
			Enumeration.Enumerate();
			List<EnumAdapterInformation> adapterInformationList = Enumeration.AdapterInformationList;
			foreach (EnumAdapterInformation item in adapterInformationList)
			{
				list.Add(item.UniqueDescription);
			}
			return list;
		}

		public List<Size> GetResolutionsList(int adapterOrdinal)
		{
			Enumeration.Enumerate();
			if (Enumeration.AdapterInformationList.Count <= adapterOrdinal)
			{
				return null;
			}
			List<Size> list = new List<Size>();
			List<DisplayMode> displayModeList = Enumeration.GetAdapterInformation((uint)adapterOrdinal).displayModeList;
			foreach (DisplayMode item2 in displayModeList)
			{
				if (item2.Format == Format.X8R8G8B8)
				{
					Size item = new Size(item2.Width, item2.Height);
					if (!list.Contains(item))
					{
						list.Add(item);
					}
				}
			}
			return list;
		}

		public void RenderEnvironment()
		{
			lock (this)
			{
				Device device = State.Device;
				if (device == null)
				{
					return;
				}
				if (State.IsDeviceLost || State.IsRenderingPaused)
				{
					Thread.Sleep(100);
				}
				if (!State.IsActive)
				{
					Thread.Sleep(20);
				}
				if (State.IsDeviceLost && !State.IsRenderingPaused)
				{
					int result;
					if (!device.CheckCooperativeLevel(out result))
					{
						if (result == -2005530520)
						{
							Thread.Sleep(50);
							return;
						}
						if (IsWindowed)
						{
							DisplayMode currentDisplayMode = Manager.Adapters[(int)State.CurrentDeviceSettings.AdapterOrdinal].CurrentDisplayMode;
							if (State.CurrentDeviceSettings.AdapterFormat != currentDisplayMode.Format)
							{
								MatchOptions match = default(MatchOptions);
								match.AdapterOrdinal = MatchType.PreserveInput;
								match.DeviceType = MatchType.PreserveInput;
								match.Windowed = MatchType.PreserveInput;
								match.AdapterFormat = MatchType.PreserveInput;
								match.VertexProcessing = MatchType.ClosestToInput;
								match.Resolution = MatchType.PreserveInput;
								match.BackBufferFormat = MatchType.ClosestToInput;
								match.BackBufferCount = MatchType.ClosestToInput;
								match.MultiSample = MatchType.ClosestToInput;
								match.SwapEffect = MatchType.ClosestToInput;
								match.DepthFormat = MatchType.ClosestToInput;
								match.StencilFormat = MatchType.ClosestToInput;
								match.PresentFlags = MatchType.ClosestToInput;
								match.RefreshRate = MatchType.ClosestToInput;
								match.PresentInterval = MatchType.PreserveInput;
								DeviceSettings currentDeviceSettings = State.CurrentDeviceSettings;
								currentDeviceSettings.AdapterFormat = currentDisplayMode.Format;
								try
								{
									currentDeviceSettings = FindValidDeviceSettings(currentDeviceSettings, match);
									ChangeDevice(currentDeviceSettings);
									return;
								}
								catch
								{
									DisplayErrorMessage(new NoCompatibleDevicesException());
									Dispose();
									return;
								}
							}
						}
						try
						{
							ResetEnvironment();
						}
						catch (DeviceLostException)
						{
							Thread.Sleep(50);
							return;
						}
						catch
						{
							DeviceSettings currentDeviceSettings2 = State.CurrentDeviceSettings;
							try
							{
								ChangeDevice(currentDeviceSettings2);
							}
							catch (Exception inner)
							{
								ResettingDeviceException e = new ResettingDeviceException(inner);
								DisplayErrorMessage(e);
								Dispose();
								throw;
							}
						}
					}
					State.IsDeviceLost = false;
				}
				double time = GameCoreTimer.GetTime();
				float num = (float)GameCoreTimer.GetElapsedTime();
				if (num == 0f)
				{
					return;
				}
				if (time < nextRenderTime)
				{
					nowElapsedTime += num;
					Thread.Sleep((int)((nextRenderTime - time) * 1000.0));
					return;
				}
				do
				{
					nextRenderTime += 1.0 / (double)device.DisplayMode.RefreshRate;
				}
				while (time >= nextRenderTime);
				num += nowElapsedTime;
				nowElapsedTime = 0f;
				State.CurrentTime = time;
				State.ElapsedTime = num;
				HandleTimers();
				UpdateFrameStats();
				if (FrameMove != null)
				{
					FrameMove(device, new RenderEventArgs(time, num));
					device = State.Device;
					if (device == null)
					{
						return;
					}
				}
				if (!State.IsRenderingPaused)
				{
					if (RenderTarget != null)
					{
						device.SetRenderTarget(0, RenderTarget);
					}
					device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1f, 0);
					if (showFps && fpsFont != null)
					{
						fpsFont.DrawText(null, State.CurrentFrameRate.ToString("f0"), 0, 0, Color.White);
					}
					if (FrameRender != null)
					{
						FrameRender(device, new RenderEventArgs(time, num));
					}
					device = State.Device;
					if (device == null)
					{
						return;
					}
				}
				if (!State.IsRenderingPaused)
				{
					try
					{
						device.Present();
					}
					catch (DeviceLostException)
					{
						State.IsDeviceLost = true;
					}
					catch (DriverInternalErrorException)
					{
						State.IsDeviceLost = true;
					}
				}
				State.CurrentFrameNumber++;
			}
		}

		private void Shutdown()
		{
			CleanupEnvironment(true);
			if (GameWindow != null)
			{
				GameWindow.Close();
			}
		}

		public void Pause(bool pauseTime, bool pauseRendering)
		{
			State.PauseTimeCount += (pauseTime ? 1 : (-1));
			if (State.PauseTimeCount < 0)
			{
				State.PauseTimeCount = 0;
			}
			State.PauseRenderingCount += (pauseRendering ? 1 : (-1));
			if (State.PauseRenderingCount < 0)
			{
				State.PauseRenderingCount = 0;
			}
			if (State.PauseTimeCount > 0)
			{
				GameCoreTimer.Stop();
				if (OnPause != null)
				{
					OnPause(this, EventArgs.Empty);
				}
			}
			else
			{
				GameCoreTimer.Start();
				if (OnResume != null)
				{
					OnResume(this, EventArgs.Empty);
				}
			}
			State.IsRenderingPaused = State.PauseRenderingCount > 0;
			State.IsTimePaused = State.PauseTimeCount > 0;
		}

		public void DisplayErrorMessage(Exception e)
		{
			if (e == lastDisplayedMessage)
			{
				return;
			}
			lastDisplayedMessage = e;
			State.ApplicationExitCode = 1;
			bool flag = true;
			if (e is ApplicationException)
			{
				if (e is NoDirect3DException)
				{
					State.ApplicationExitCode = 2;
				}
				else if (e is MediaNotFoundException)
				{
					State.ApplicationExitCode = 4;
				}
				else if (e is CreatingDeviceException)
				{
					State.ApplicationExitCode = 6;
				}
				else if (e is ResettingDeviceException)
				{
					State.ApplicationExitCode = 7;
				}
				else if (e is CreatingDeviceObjectsException)
				{
					State.ApplicationExitCode = 8;
				}
				else if (e is ResettingDeviceObjectsException)
				{
					State.ApplicationExitCode = 9;
				}
				else if (e is NoCompatibleDevicesException)
				{
					State.ApplicationExitCode = 3;
				}
				else
				{
					flag = false;
				}
			}
			else
			{
				flag = false;
			}
			string empty = string.Empty;
			if (flag && State.IsShowingMsgBoxOnError)
			{
				empty = e.Message;
				if (e.InnerException != null)
				{
					empty = empty + "\r\n\r\n" + e.InnerException.ToString();
					lastDisplayedMessage = e.InnerException;
				}
				MessageBox.Show(empty, "DirectX Application", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
			else if (!flag && State.IsShowingMsgBoxOnError)
			{
				empty = e.ToString();
				MessageBox.Show(empty, "DirectX Application", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
		}

		private void CleanupEnvironment(bool releaseSettings)
		{
			using (State.Device)
			{
				if (RenderTarget != null)
				{
					RenderTarget.Dispose();
					RenderTarget = null;
				}
				if (releaseSettings)
				{
					State.CurrentDeviceSettings = null;
				}
				State.BackBufferSurfaceDesc = new SurfaceDescription();
				State.Caps = new Caps();
				State.WasDeviceCreated = false;
			}
			State.Device = null;
		}

		private void InitializeEnvironment()
		{
			Device device = State.Device;
			if (device == null)
			{
				throw new InvalidOperationException("The device should not be null.");
			}
			bool flag = true;
			try
			{
				AdjustWindowStyle(GameWindow, IsWindowed);
				PrepareDevice(device);
				try
				{
					State.IsInsideDeviceCallback = true;
					fpsFont = new Microsoft.DirectX.Direct3D.Font(State.Device, new System.Drawing.Font(FontFamily.GenericSansSerif, 18f));
					if (DeviceCreated != null)
					{
						DeviceCreated(this, new DeviceEventArgs(device, State.BackBufferSurfaceDesc));
					}
					State.IsInsideDeviceCallback = false;
				}
				catch (MediaNotFoundException e)
				{
					DisplayErrorMessage(e);
					flag = false;
					throw;
				}
				catch (Exception inner)
				{
					DisplayErrorMessage(new CreatingDeviceException(inner));
					flag = false;
					throw;
				}
				try
				{
					State.IsInsideDeviceCallback = true;
					if (DeviceReset != null)
					{
						DeviceReset(this, new DeviceEventArgs(device, State.BackBufferSurfaceDesc));
					}
					State.IsInsideDeviceCallback = false;
				}
				catch (MediaNotFoundException e2)
				{
					DisplayErrorMessage(e2);
					flag = false;
					throw;
				}
				catch (Exception inner2)
				{
					DisplayErrorMessage(new CreatingDeviceException(inner2));
					flag = false;
					throw;
				}
			}
			finally
			{
				if (!flag)
				{
					CleanupEnvironment(true);
				}
			}
		}

		private void PrepareDevice(Device device)
		{
			using (Surface surface = device.GetBackBuffer(0, 0, BackBufferType.Mono))
			{
				State.BackBufferSurfaceDesc = surface.Description;
			}
			State.Caps = device.DeviceCaps;
		}

		private void AdjustWindowStyle(Form window, bool isWindowed)
		{
			if (window != null)
			{
				if (isWindowed)
				{
					window.FormBorderStyle = FormBorderStyle.FixedSingle;
				}
				else
				{
					window.FormBorderStyle = FormBorderStyle.None;
				}
			}
		}

		private void UpdateFrameStats()
		{
			double absoluteTime = GameCoreTimer.GetAbsoluteTime();
			State.LastStatsUpdateFrames++;
			if (absoluteTime - State.LastStatsUpdateTime > 0.1)
			{
				float currentFrameRate = (float)((double)State.LastStatsUpdateFrames / (absoluteTime - State.LastStatsUpdateTime));
				State.CurrentFrameRate = currentFrameRate;
				State.LastStatsUpdateFrames = 0u;
				State.LastStatsUpdateTime = absoluteTime;
			}
		}

		public void ResetState()
		{
			State = new GameCoreData();
		}

		public int SetTimer(TimerCallback callbackTimer, float timeoutInSeconds)
		{
			if (callbackTimer == null)
			{
				throw new ArgumentNullException("callbackTimer", "You must pass in a valid timer callback .");
			}
			GameTimer item = default(GameTimer);
			item.Callback = callbackTimer;
			item.Countdown = timeoutInSeconds;
			item.TimeoutInSecs = timeoutInSeconds;
			item.IsEnabled = true;
			State.Timers.Add(item);
			return State.Timers.Count;
		}

		public void KillTimer(int id)
		{
			if (id < State.Timers.Count)
			{
				GameTimer value = State.Timers[id];
				value.IsEnabled = false;
				State.Timers[id] = value;
				return;
			}
			throw new ArgumentException("An invalid timer ID was passed in", "id");
		}

		private void HandleTimers()
		{
			float elapsedTime = State.ElapsedTime;
			for (int i = 0; i < State.Timers.Count; i++)
			{
				GameTimer value = State.Timers[i];
				if (value.IsEnabled)
				{
					value.Countdown -= elapsedTime;
					if (value.Countdown < 0f)
					{
						value.Callback((uint)i);
						value.Countdown = value.TimeoutInSecs;
					}
					State.Timers[i] = value;
				}
			}
		}

		public void CloseWindow()
		{
			if (GameWindow != null)
			{
				GameWindow.Close();
			}
		}

		private void OnPaint(object sender, PaintEventArgs e)
		{
			if (State.Device != null && State.IsRenderingPaused)
			{
				double currentTime = State.CurrentTime;
				float elapsedTime = State.ElapsedTime;
				if (FrameRender != null)
				{
					FrameRender(State.Device, new RenderEventArgs(currentTime, elapsedTime));
				}
				try
				{
					State.Device.Present();
				}
				catch (DeviceLostException)
				{
					State.IsDeviceLost = true;
				}
				catch (DriverInternalErrorException)
				{
					State.IsDeviceLost = true;
				}
			}
		}

		private void OnResize(object sender, EventArgs e)
		{
			Form form = sender as Form;
			if (form == null)
			{
				return;
			}
			if (form.WindowState == FormWindowState.Minimized)
			{
				State.IsMinimized = true;
				Pause(true, true);
				return;
			}
			if (State.IsMinimized)
			{
				Pause(false, false);
			}
			State.IsMinimized = false;
		}

		private void OnFormClosed(object sender, EventArgs e)
		{
			State.Window = null;
		}

		private void OnFormActivated(object sender, EventArgs e)
		{
			State.IsActive = true;
		}

		private void OnFormDeactivated(object sender, EventArgs e)
		{
			State.IsActive = false;
		}

		private void OnMenuStart(object sender, EventArgs e)
		{
			Pause(true, true);
		}

		private void OnMenuEnd(object sender, EventArgs e)
		{
			Pause(false, false);
		}

		private void HookEvents(Form form)
		{
			form.Paint += OnPaint;
			form.Resize += OnResize;
			form.Closed += OnFormClosed;
			form.Activated += OnFormActivated;
			form.Deactivate += OnFormDeactivated;
			form.MenuStart += OnMenuStart;
			form.MenuComplete += OnMenuEnd;
			form.MouseMove += OnMouseMove;
			form.MouseDown += OnMouseDown;
			form.MouseUp += OnMouseUp;
			form.MouseWheel += OnMouseWheel;
		}

		protected virtual void OnMouseMove(object sender, MouseEventArgs e)
		{
			if (MouseMove != null)
			{
				MouseMove(this, e);
			}
		}

		protected virtual void OnMouseDown(object sender, MouseEventArgs e)
		{
			if (MouseDown != null)
			{
				MouseDown(this, e);
			}
		}

		protected virtual void OnMouseUp(object sender, MouseEventArgs e)
		{
			if (MouseUp != null)
			{
				MouseUp(this, e);
			}
		}

		protected virtual void OnMouseWheel(object sender, MouseEventArgs e)
		{
			if (MouseWheel != null)
			{
				MouseWheel(this, e);
			}
		}

		internal void WindowsProcedure(ref Message m)
		{
			if (State.WndProcFunction != null)
			{
				bool NoFurtherProcessing = false;
				IntPtr result = State.WndProcFunction(m.HWnd, (NativeMethods.WindowMessage)m.Msg, m.WParam, m.LParam, ref NoFurtherProcessing);
				if (NoFurtherProcessing)
				{
					m.Result = result;
				}
			}
			switch ((NativeMethods.WindowMessage)m.Msg)
			{
			case NativeMethods.WindowMessage.PowerBroadcast:
				if (m.WParam == IntPtr.Zero)
				{
					m.Result = TrueIntPtr;
				}
				if (m.WParam.ToInt32() == 7)
				{
					GameCoreTimer.Reset();
					State.LastStatsUpdateTime = 0.0;
					m.Result = TrueIntPtr;
				}
				break;
			case NativeMethods.WindowMessage.SystemCommand:
				switch (m.WParam.ToInt32())
				{
				case 61440:
				case 61456:
				case 61488:
				case 61696:
				case 61760:
				case 61808:
					m.Result = TrueIntPtr;
					break;
				}
				break;
			}
		}

		public void MainLoop()
		{
			if (isDisposed)
			{
				return;
			}
			if (State.IsInsideDeviceCallback || State.IsInsideMainloop)
			{
				State.ApplicationExitCode = 1;
				throw new InvalidOperationException("You cannot call this method from a callback, or reenter the method.");
			}
			State.IsInsideMainloop = true;
			if (!State.WasDeviceCreated)
			{
				if (State.WasDeviceCreateCalled)
				{
					State.ApplicationExitCode = 1;
					throw new InvalidOperationException("The device was never created.");
				}
				try
				{
					CreateDevice(0u, true, State.WindowClientSize.Width, State.WindowClientSize.Height, State.VSync);
				}
				catch
				{
					State.ApplicationExitCode = 1;
					throw;
				}
			}
			if (!State.IsInited || !State.WasWindowCreated || !State.WasDeviceCreated)
			{
				State.ApplicationExitCode = 1;
				throw new InvalidOperationException("The framework was not initialized, cannot continue.");
			}
			ApplicationIdleHandler = OnApplicationIdle;
			Application.Idle += ApplicationIdleHandler;
			isManual = false;
			Application.Run(GameWindow);
			State.IsInsideMainloop = false;
		}

		public void ToggleManual(bool manual)
		{
			if (manual != isManual)
			{
				if (manual)
				{
					Application.Idle -= ApplicationIdleHandler;
				}
				else
				{
					Application.Idle += ApplicationIdleHandler;
				}
				isManual = manual;
			}
		}

		private void OnApplicationIdle(object sender, EventArgs e)
		{
			while (AppStillIdle)
			{
				RenderEnvironment();
			}
		}

		private void OnDeviceDisposing(object sender, EventArgs e)
		{
			State.IsInsideDeviceCallback = true;
			if (fpsFont != null)
			{
				fpsFont.Dispose();
				fpsFont = null;
			}
			if (Disposing != null)
			{
				Disposing(this, e);
			}
			State.IsInsideDeviceCallback = false;
		}

		private void OnDeviceLost(object sender, EventArgs e)
		{
			State.IsInsideDeviceCallback = true;
			if (fpsFont != null && !fpsFont.Disposed)
			{
				fpsFont.OnLostDevice();
			}
			if (DeviceLost != null)
			{
				DeviceLost(this, e);
			}
			State.IsInsideDeviceCallback = false;
		}

		private void OnDeviceReset(object sender, EventArgs e)
		{
			Device device = sender as Device;
			State.IsInsideDeviceCallback = true;
			PrepareDevice(device);
			if (fpsFont != null)
			{
				fpsFont.OnResetDevice();
			}
			if (DeviceReset != null)
			{
				DeviceReset(this, new DeviceEventArgs(device, State.BackBufferSurfaceDesc));
			}
			State.IsInsideDeviceCallback = false;
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);
			if (!isDisposed)
			{
				if (Disposing != null)
				{
					Disposing(this, EventArgs.Empty);
				}
				Shutdown();
			}
			isDisposed = true;
		}

		~GameCore()
		{
			Dispose();
		}
	}
}
