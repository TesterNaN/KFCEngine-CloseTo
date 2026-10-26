using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace KidFansChannel.InputCore
{
	public class XInput
	{
		protected enum GamePadButton : ushort
		{
			DPad_Up = 1,
			DPad_Down = 2,
			DPad_Left = 4,
			DPad_Right = 8,
			Start = 0x10,
			Back = 0x20,
			Left_Thumb = 0x40,
			Right_Thumb = 0x80,
			Left_Shoulder = 0x100,
			Right_Shoulder = 0x200,
			A = 0x1000,
			B = 0x2000,
			X = 0x4000,
			Y = 0x8000
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		protected struct GamePad
		{
			public GamePadButton Buttons;

			public byte LeftTrigger;

			public byte RightTrigger;

			public short ThumbLX;

			public short ThumbLY;

			public short ThumbRX;

			public short ThumbRY;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		protected struct State
		{
			public uint PacketNumber;

			public GamePad Gamepad;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		protected struct Vibration
		{
			public ushort LeftMotorSpeed;

			public ushort RightMotorSpeed;
		}

		public const int ButtonNumber = 16;

		private bool[] buttonPressed = new bool[16];

		private bool hasController;

		private Point rightStick;

		[DllImport("xinput9_1_0.dll", CharSet = CharSet.Unicode, EntryPoint = "XInputGetState")]
		protected static extern int GetState(int userIndex, out State state);

		[DllImport("xinput9_1_0.dll", CharSet = CharSet.Unicode, EntryPoint = "XInputSetState")]
		protected static extern int SetState(int userIndex, ref Vibration vibration);

		public void Update()
		{
			State state;
			if (GetState(0, out state) == 0)
			{
				hasController = true;
				buttonPressed[0] = (state.Gamepad.Buttons & GamePadButton.A) == GamePadButton.A;
				buttonPressed[1] = (state.Gamepad.Buttons & GamePadButton.B) == GamePadButton.B;
				buttonPressed[2] = (state.Gamepad.Buttons & GamePadButton.X) == GamePadButton.X;
				buttonPressed[3] = (state.Gamepad.Buttons & GamePadButton.Y) == GamePadButton.Y;
				buttonPressed[4] = (state.Gamepad.Buttons & GamePadButton.Left_Shoulder) == GamePadButton.Left_Shoulder;
				buttonPressed[5] = (state.Gamepad.Buttons & GamePadButton.Right_Shoulder) == GamePadButton.Right_Shoulder;
				buttonPressed[6] = state.Gamepad.LeftTrigger >= 32;
				buttonPressed[7] = state.Gamepad.RightTrigger >= 32;
				buttonPressed[8] = (state.Gamepad.Buttons & GamePadButton.Back) == GamePadButton.Back;
				buttonPressed[9] = (state.Gamepad.Buttons & GamePadButton.Start) == GamePadButton.Start;
				buttonPressed[10] = (state.Gamepad.Buttons & GamePadButton.Left_Thumb) == GamePadButton.Left_Thumb;
				buttonPressed[11] = (state.Gamepad.Buttons & GamePadButton.Right_Thumb) == GamePadButton.Right_Thumb;
				for (int i = 12; i < 16; i++)
				{
					buttonPressed[i] = false;
				}
				int thumbLX = state.Gamepad.ThumbLX;
				int thumbLY = state.Gamepad.ThumbLY;
				if ((thumbLX < -8096 && thumbLX < -Math.Abs(thumbLY)) || (state.Gamepad.Buttons & GamePadButton.DPad_Left) == GamePadButton.DPad_Left)
				{
					buttonPressed[12] = true;
				}
				else if ((thumbLX > 8096 && thumbLX > Math.Abs(thumbLY)) || (state.Gamepad.Buttons & GamePadButton.DPad_Right) == GamePadButton.DPad_Right)
				{
					buttonPressed[13] = true;
				}
				else if ((thumbLY > 8096 && thumbLY >= Math.Abs(thumbLX)) || (state.Gamepad.Buttons & GamePadButton.DPad_Up) == GamePadButton.DPad_Up)
				{
					buttonPressed[14] = true;
				}
				else if ((thumbLY < -8096 && thumbLY <= -Math.Abs(thumbLX)) || (state.Gamepad.Buttons & GamePadButton.DPad_Down) == GamePadButton.DPad_Down)
				{
					buttonPressed[15] = true;
				}
				rightStick = new Point(state.Gamepad.ThumbRX, -state.Gamepad.ThumbRY);
			}
			else
			{
				hasController = false;
				for (int j = 0; j < 16; j++)
				{
					buttonPressed[j] = false;
				}
			}
		}

		public bool GetButtonState(int index)
		{
			if (index < 0 || index >= 16)
			{
				throw new ArgumentOutOfRangeException();
			}
			return buttonPressed[index];
		}

		public void StartShock()
		{
			if (hasController)
			{
				Vibration vibration = default(Vibration);
				vibration.LeftMotorSpeed = 32768;
				vibration.RightMotorSpeed = 32768;
				Vibration vibration2 = vibration;
				SetState(0, ref vibration2);
			}
		}

		public void StopShock()
		{
			if (hasController)
			{
				Vibration vibration = default(Vibration);
				vibration.LeftMotorSpeed = 0;
				vibration.RightMotorSpeed = 0;
				Vibration vibration2 = vibration;
				SetState(0, ref vibration2);
			}
		}

		public PointF GetPosition()
		{
			return new PointF((Math.Abs(rightStick.X) < 8096) ? 0f : ((float)(Math.Sign(rightStick.X) * (Math.Abs(rightStick.X) - 8096)) / 24288f), (Math.Abs(rightStick.Y) < 8096) ? 0f : ((float)(Math.Sign(rightStick.Y) * (Math.Abs(rightStick.Y) - 8096)) / 24288f));
		}
	}
}
