using System;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;

namespace KidFansChannel.InputCore
{
	public class KeyBoardInput : IDisposable
	{
		public enum Key
		{
			A = 30,
			AbntC1 = 115,
			AbntC2 = 126,
			Add = 78,
			Apostrophe = 40,
			Apps = 221,
			At = 145,
			AX = 150,
			B = 48,
			Back = 14,
			BackSlash = 43,
			BackSpace = Back,
			C = 46,
			Calculator = 161,
			Capital = 58,
			CapsLock = Capital,
			Circumflex = 144,
			Colon = 146,
			Comma = 51,
			Convert = 121,
			D = 32,
			D0 = 11,
			D1 = 2,
			D2 = 3,
			D3 = 4,
			D4 = 5,
			D5 = 6,
			D6 = 7,
			D7 = 8,
			D8 = 9,
			D9 = 10,
			Decimal = 83,
			Delete = 211,
			Divide = 181,
			Down = 208,
			DownArrow = Down,
			E = 18,
			End = 207,
			Equals = 13,
			Escape = 1,
			F = 33,
			F1 = 59,
			F10 = 68,
			F11 = 87,
			F12 = 88,
			F13 = 100,
			F14 = 101,
			F15 = 102,
			F2 = 60,
			F3 = 61,
			F4 = 62,
			F5 = 63,
			F6 = 64,
			F7 = 65,
			F8 = 66,
			F9 = 67,
			G = 34,
			Grave = 41,
			H = 35,
			Home = 199,
			I = 23,
			Insert = 210,
			J = 36,
			K = 37,
			Kana = 112,
			Kanji = 148,
			L = 38,
			Left = 203,
			LeftAlt = 56,
			LeftArrow = Left,
			LeftBracket = 26,
			LeftControl = 29,
			LeftMenu = LeftAlt,
			LeftShift = 42,
			LeftWindows = 219,
			M = 50,
			Mail = 236,
			MediaSelect = 237,
			MediaStop = 164,
			Minus = 12,
			Multiply = 55,
			Mute = 160,
			MyComputer = 235,
			N = 49,
			Next = 209,
			NextTrack = 153,
			NoConvert = 123,
			Numlock = 69,
			NumPad0 = 82,
			NumPad1 = 79,
			NumPad2 = 80,
			NumPad3 = 81,
			NumPad4 = 75,
			NumPad5 = 76,
			NumPad6 = 77,
			NumPad7 = 71,
			NumPad8 = 72,
			NumPad9 = 73,
			NumPadComma = 179,
			NumPadEnter = 156,
			NumPadEquals = 141,
			NumPadMinus = 74,
			NumPadPeriod = Decimal,
			NumPadPlus = Add,
			NumPadSlash = Divide,
			NumPadStar = Multiply,
			O = 24,
			OEM102 = 86,
			P = 25,
			PageDown = Next,
			PageUp = 201,
			Pause = 197,
			Period = 52,
			PlayPause = 162,
			Power = 222,
			PrevTrack = Circumflex,
			Prior = PageUp,
			Q = 16,
			R = 19,
			Return = 28,
			Right = 205,
			RightAlt = 184,
			RightArrow = Right,
			RightBracket = 27,
			RightControl = 157,
			RightMenu = RightAlt,
			RightShift = 54,
			RightWindows = 220,
			S = 31,
			Scroll = 70,
			SemiColon = 39,
			Slash = 53,
			Sleep = 223,
			Space = 57,
			Stop = 149,
			Subtract = NumPadMinus,
			SysRq = 183,
			T = 20,
			Tab = 15,
			U = 22,
			Underline = 147,
			Unlabeled = 151,
			Up = 200,
			UpArrow = Up,
			V = 47,
			VolumeDown = 174,
			VolumeUp = 176,
			W = 17,
			Wake = 227,
			WebBack = 234,
			WebFavorites = 230,
			WebForward = 233,
			WebHome = 178,
			WebRefresh = 231,
			WebSearch = 229,
			WebStop = 232,
			X = 45,
			Y = 21,
			Yen = 125,
			Z = 44
		}

		private Device directInputDevice;

		private KeyboardState state;

		public void Initialize(Form form)
		{
			directInputDevice = new Device(SystemGuid.Keyboard);
			if (directInputDevice != null)
			{
				try
				{
					directInputDevice.SetCooperativeLevel(form, CooperativeLevelFlags.Foreground | CooperativeLevelFlags.NonExclusive);
					directInputDevice.Acquire();
				}
				catch
				{
				}
			}
		}

		~KeyBoardInput()
		{
			Dispose();
		}

		public void Update()
		{
			if (!(directInputDevice != null))
			{
				return;
			}
			try
			{
				state = directInputDevice.GetCurrentKeyboardState();
			}
			catch (DirectXException)
			{
				while (true)
				{
					try
					{
						directInputDevice.Acquire();
						break;
					}
					catch (InputLostException)
					{
					}
					catch (InputException)
					{
						break;
					}
				}
			}
		}

		public bool GetKeyState(Key key)
		{
			if (state == null)
			{
				return false;
			}
			return state[(Microsoft.DirectX.DirectInput.Key)key];
		}

		public void Dispose()
		{
			if (directInputDevice != null)
			{
				try
				{
					directInputDevice.Unacquire();
					directInputDevice.Dispose();
					directInputDevice = null;
				}
				catch (Exception)
				{
				}
			}
		}
	}
}
