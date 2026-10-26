using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.DirectX.Direct3D;

namespace KidFansChannel.GameCore
{
	internal class GameCoreData
	{
		private Device device;

		private DeviceSettings currentDeviceSettings;

		private SurfaceDescription backBufferSurfaceDesc;

		private Caps caps;

		private Form window;

		private IntPtr adapterMonitor;

		private double currentTime;

		private float elapsedTime;

		private Size windowClientSize;

		private Size fullScreenClientSize;

		private Point windowLocation;

		private MainMenu windowMenu;

		private double lastStatsUpdateTime;

		private uint lastStatsUpdateFrames;

		private float frameRate;

		private int currentFrameNumber;

		private bool isShowingMsgBoxOnError;

		private int applicationExitCode;

		private bool isInited;

		private bool wasWindowCreated;

		private bool wasDeviceCreated;

		private bool isInitCalled;

		private bool isWindowCreateCalled;

		private bool isDeviceCreateCalled;

		private bool isInsideDeviceCallback;

		private bool isInsideMainloop;

		private bool isActive;

		private bool isTimePaused;

		private bool isRenderingPaused;

		private int pauseRenderingCount;

		private int pauseTimeCount;

		private bool isDeviceLost;

		private bool isMinimized;

		private WndProcCallback wndFunc;

		private List<GameTimer> timerList = new List<GameTimer>();

		private bool vsync;

		public Device Device
		{
			get
			{
				return device;
			}
			set
			{
				device = value;
			}
		}

		public DeviceSettings CurrentDeviceSettings
		{
			get
			{
				return currentDeviceSettings;
			}
			set
			{
				currentDeviceSettings = value;
			}
		}

		public SurfaceDescription BackBufferSurfaceDesc
		{
			get
			{
				return backBufferSurfaceDesc;
			}
			set
			{
				backBufferSurfaceDesc = value;
			}
		}

		public Caps Caps
		{
			get
			{
				return caps;
			}
			set
			{
				caps = value;
			}
		}

		public Form Window
		{
			get
			{
				return window;
			}
			set
			{
				window = value;
			}
		}

		public IntPtr AdapterMonitor
		{
			get
			{
				return adapterMonitor;
			}
			set
			{
				adapterMonitor = value;
			}
		}

		public double CurrentTime
		{
			get
			{
				return currentTime;
			}
			set
			{
				currentTime = value;
			}
		}

		public float ElapsedTime
		{
			get
			{
				return elapsedTime;
			}
			set
			{
				elapsedTime = value;
			}
		}

		public Size FullScreenClientSize
		{
			get
			{
				return fullScreenClientSize;
			}
			set
			{
				fullScreenClientSize = value;
			}
		}

		public Size WindowClientSize
		{
			get
			{
				return windowClientSize;
			}
			set
			{
				windowClientSize = value;
			}
		}

		public Point WindowLocation
		{
			get
			{
				return windowLocation;
			}
			set
			{
				windowLocation = value;
			}
		}

		public MainMenu Menu
		{
			get
			{
				return windowMenu;
			}
			set
			{
				windowMenu = value;
			}
		}

		public double LastStatsUpdateTime
		{
			get
			{
				return lastStatsUpdateTime;
			}
			set
			{
				lastStatsUpdateTime = value;
			}
		}

		public uint LastStatsUpdateFrames
		{
			get
			{
				return lastStatsUpdateFrames;
			}
			set
			{
				lastStatsUpdateFrames = value;
			}
		}

		public float CurrentFrameRate
		{
			get
			{
				return frameRate;
			}
			set
			{
				frameRate = value;
			}
		}

		public int CurrentFrameNumber
		{
			get
			{
				return currentFrameNumber;
			}
			set
			{
				currentFrameNumber = value;
			}
		}

		public bool IsShowingMsgBoxOnError
		{
			get
			{
				return isShowingMsgBoxOnError;
			}
			set
			{
				isShowingMsgBoxOnError = value;
			}
		}

		public int ApplicationExitCode
		{
			get
			{
				return applicationExitCode;
			}
			set
			{
				applicationExitCode = value;
			}
		}

		public bool IsInited
		{
			get
			{
				return isInited;
			}
			set
			{
				isInited = value;
			}
		}

		public bool WasWindowCreated
		{
			get
			{
				return wasWindowCreated;
			}
			set
			{
				wasWindowCreated = value;
			}
		}

		public bool WasDeviceCreated
		{
			get
			{
				return wasDeviceCreated;
			}
			set
			{
				wasDeviceCreated = value;
			}
		}

		public bool WasInitCalled
		{
			get
			{
				return isInitCalled;
			}
			set
			{
				isInitCalled = value;
			}
		}

		public bool WasWindowCreateCalled
		{
			get
			{
				return isWindowCreateCalled;
			}
			set
			{
				isWindowCreateCalled = value;
			}
		}

		public bool WasDeviceCreateCalled
		{
			get
			{
				return isDeviceCreateCalled;
			}
			set
			{
				isDeviceCreateCalled = value;
			}
		}

		public bool IsInsideDeviceCallback
		{
			get
			{
				return isInsideDeviceCallback;
			}
			set
			{
				isInsideDeviceCallback = value;
			}
		}

		public bool IsInsideMainloop
		{
			get
			{
				return isInsideMainloop;
			}
			set
			{
				isInsideMainloop = value;
			}
		}

		public bool IsActive
		{
			get
			{
				return isActive;
			}
			set
			{
				isActive = value;
			}
		}

		public bool IsTimePaused
		{
			get
			{
				return isTimePaused;
			}
			set
			{
				isTimePaused = value;
			}
		}

		public bool IsRenderingPaused
		{
			get
			{
				return isRenderingPaused;
			}
			set
			{
				isRenderingPaused = value;
			}
		}

		public int PauseRenderingCount
		{
			get
			{
				return pauseRenderingCount;
			}
			set
			{
				pauseRenderingCount = value;
			}
		}

		public int PauseTimeCount
		{
			get
			{
				return pauseTimeCount;
			}
			set
			{
				pauseTimeCount = value;
			}
		}

		public bool IsDeviceLost
		{
			get
			{
				return isDeviceLost;
			}
			set
			{
				isDeviceLost = value;
			}
		}

		public bool IsMinimized
		{
			get
			{
				return isMinimized;
			}
			set
			{
				isMinimized = value;
			}
		}

		public WndProcCallback WndProcFunction
		{
			get
			{
				return wndFunc;
			}
			set
			{
				wndFunc = value;
			}
		}

		public List<GameTimer> Timers
		{
			get
			{
				return timerList;
			}
			set
			{
				timerList = value;
			}
		}

		public bool VSync
		{
			get
			{
				return vsync;
			}
			set
			{
				vsync = value;
			}
		}

		public GameCoreData()
		{
			isShowingMsgBoxOnError = true;
			isActive = true;
			windowClientSize = new Size(800, 600);
		}
	}
}
