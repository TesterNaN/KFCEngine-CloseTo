using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows.Forms;

namespace KidFansChannel.GameCore
{
	public class NativeMethods
	{
		public enum WindowMessage : uint
		{
			Destroy = 2u,
			Close = 16u,
			Quit = 18u,
			Paint = 15u,
			SetCursor = 32u,
			ActivateApplication = 28u,
			EnterMenuLoop = 529u,
			ExitMenuLoop = 530u,
			NonClientHitTest = 132u,
			PowerBroadcast = 536u,
			SystemCommand = 274u,
			GetMinMax = 36u,
			KeyDown = 256u,
			KeyUp = 257u,
			Character = 258u,
			SystemKeyDown = 260u,
			SystemKeyUp = 261u,
			SystemCharacter = 262u,
			MouseMove = 512u,
			LeftButtonDown = 513u,
			LeftButtonUp = 514u,
			LeftButtonDoubleClick = 515u,
			RightButtonDown = 516u,
			RightButtonUp = 517u,
			RightButtonDoubleClick = 518u,
			MiddleButtonDown = 519u,
			MiddleButtonUp = 520u,
			MiddleButtonDoubleClick = 521u,
			MouseWheel = 522u,
			XButtonDown = 523u,
			XButtonUp = 524u,
			XButtonDoubleClick = 525u,
			MouseFirst = LeftButtonDown,
			MouseLast = XButtonDoubleClick,
			EnterSizeMove = 561u,
			ExitSizeMove = 562u,
			Size = 5u
		}

		public enum MouseButtons
		{
			Left = 1,
			Right = 2,
			Middle = 0x10,
			Side1 = 0x20,
			Side2 = 0x40
		}

		public struct Message
		{
			public IntPtr hWnd;

			public WindowMessage msg;

			public IntPtr wParam;

			public IntPtr lParam;

			public uint time;

			public Point p;
		}

		public struct MinMaxInformation
		{
			public Point reserved;

			public Point MaxSize;

			public Point MaxPosition;

			public Point MinTrackSize;

			public Point MaxTrackSize;
		}

		public struct MonitorInformation
		{
			public uint Size;

			public Rectangle MonitorRectangle;

			public Rectangle WorkRectangle;

			public uint Flags;
		}

		[DllImport("winmm.dll")]
		[SuppressUnmanagedCodeSecurity]
		public static extern IntPtr timeBeginPeriod(uint period);

		[DllImport("kernel32")]
		[SuppressUnmanagedCodeSecurity]
		public static extern bool QueryPerformanceFrequency(ref long PerformanceFrequency);

		[DllImport("kernel32")]
		[SuppressUnmanagedCodeSecurity]
		public static extern bool QueryPerformanceCounter(ref long PerformanceCount);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		[SuppressUnmanagedCodeSecurity]
		public static extern bool GetMonitorInfo(IntPtr hWnd, ref MonitorInformation info);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		[SuppressUnmanagedCodeSecurity]
		public static extern IntPtr MonitorFromWindow(IntPtr hWnd, uint flags);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		[SuppressUnmanagedCodeSecurity]
		public static extern short GetAsyncKeyState(uint key);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		[SuppressUnmanagedCodeSecurity]
		public static extern IntPtr SetCapture(IntPtr handle);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		[SuppressUnmanagedCodeSecurity]
		public static extern bool ReleaseCapture();

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		[SuppressUnmanagedCodeSecurity]
		public static extern int GetCaretBlinkTime();

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		[SuppressUnmanagedCodeSecurity]
		public static extern bool PeekMessage(out Message msg, IntPtr hWnd, uint messageFilterMin, uint messageFilterMax, uint flags);

		private NativeMethods()
		{
		}

		public static short LoWord(uint l)
		{
			return (short)(l & 0xFFFF);
		}

		public static short HiWord(uint l)
		{
			return (short)(l >> 16);
		}

		public static uint MakeUInt32(short l, short r)
		{
			return (uint)((l & 0xFFFF) | ((r & 0xFFFF) << 16));
		}

		public static bool IsKeyDown(Keys key)
		{
			return (GetAsyncKeyState(16u) & 0x8000) != 0;
		}
	}
}
