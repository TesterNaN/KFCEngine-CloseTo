using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;

namespace KidFansChannel.InputCore
{
	public class GamePadInput : IDisposable
	{
		public class GamePadInstance
		{
			public string Name;

			public bool ForceFeedback;

			public Guid Guid;
		}

		public const int ButtonNumber = 16;

		private Device directInputDevice;

		private bool[] buttonPressed = new bool[16];

		private EffectObject effectObject;

		private Point rightStick;

		public List<GamePadInstance> GetGamePadList()
		{
			List<GamePadInstance> pads = new List<GamePadInstance>();
			DeviceList devices = Manager.GetDevices(DeviceClass.GameControl, EnumDevicesFlags.AttachedOnly);
			foreach (DeviceInstance item in devices)
			{
				GamePadInstance gamePadInstance = new GamePadInstance();
				gamePadInstance.Name = item.InstanceName;
				gamePadInstance.Guid = item.InstanceGuid;
				Device device = new Device(item.InstanceGuid);
				gamePadInstance.ForceFeedback = device.Caps.ForceFeedback;
				device.Dispose();
				pads.Add(gamePadInstance);
			}
			return pads;
		}

		public void Initialize(GamePadInstance gamePadInstance, Form form)
		{
			directInputDevice = new Device(gamePadInstance.Guid);
			if (!(directInputDevice != null))
			{
				return;
			}
			try
			{
				directInputDevice.SetDataFormat(DeviceDataFormat.Joystick);
				directInputDevice.SetCooperativeLevel(form, CooperativeLevelFlags.Foreground | CooperativeLevelFlags.Exclusive);
				foreach (DeviceObjectInstance @object in directInputDevice.Objects)
				{
					if ((@object.ObjectId & 3) != 0)
					{
						directInputDevice.Properties.SetRange(ParameterHow.ById, @object.ObjectId, new InputRange(-1000, 1000));
					}
				}
				if (gamePadInstance.ForceFeedback)
				{
					int[] axes = null;
					foreach (DeviceObjectInstance object2 in directInputDevice.GetObjects(DeviceObjectTypeFlags.Axis))
					{
						if ((object2.Flags & 1) != 0)
						{
							if (axes != null)
							{
								int[] grownAxes = new int[axes.Length + 1];
								axes.CopyTo(grownAxes, 0);
								axes = grownAxes;
							}
							else
							{
								axes = new int[1];
							}
							axes[axes.Length - 1] = object2.Offset;
							if (2 == axes.Length)
							{
								break;
							}
						}
					}
					foreach (EffectInformation effect in directInputDevice.GetEffects(EffectType.All))
					{
						if (DInputHelper.GetTypeCode(effect.EffectType) == 1)
						{
							Effect die = new Effect();
							die.SetDirection(new int[axes.Length]);
							die.SetAxes(new int[axes.Length]);
							die.EffectType = EffectType.ConstantForce;
							die.ConditionStruct = new Condition[axes.Length];
							die.Duration = -1;
							die.Gain = 10000;
							die.SamplePeriod = 0;
							die.TriggerButton = -1;
							die.TriggerRepeatInterval = -1;
							die.Flags = EffectFlags.Cartesian | EffectFlags.ObjectOffsets;
							die.Constant.Magnitude = 5000;
							die.SetAxes(axes);
							effectObject = new EffectObject(effect.EffectGuid, die, directInputDevice);
						}
					}
				}
				else
				{
					effectObject = null;
				}
				directInputDevice.Acquire();
			}
			catch
			{
			}
		}

		~GamePadInput()
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
				directInputDevice.Poll();
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
			JoystickState currentJoystickState;
			try
			{
				currentJoystickState = directInputDevice.CurrentJoystickState;
			}
			catch (InputException)
			{
				return;
			}
			byte[] buttons = currentJoystickState.GetButtons();
			for (int i = 0; i < 12; i++)
			{
				if (i < buttons.Length)
				{
					buttonPressed[i] = (buttons[i] & 0x80) == 128;
				}
				else
				{
					buttonPressed[i] = false;
				}
			}
			for (int j = 12; j < 16; j++)
			{
				buttonPressed[j] = false;
			}
			int[] pointOfView = currentJoystickState.GetPointOfView();
			int povAngle = ((pointOfView.Length != 0) ? pointOfView[0] : (-1));
			int x = currentJoystickState.X;
			int y = currentJoystickState.Y;
			if ((x < -500 && x < -Math.Abs(y)) || povAngle == 27000 || povAngle == 31500 || povAngle == 22500)
			{
				buttonPressed[12] = true;
			}
			else if ((x > 500 && x > Math.Abs(y)) || povAngle == 9000 || povAngle == 13500 || povAngle == 4500)
			{
				buttonPressed[13] = true;
			}
			else if ((y < -500 && y <= -Math.Abs(x)) || povAngle == 0 || povAngle == 4500 || povAngle == 31500)
			{
				buttonPressed[14] = true;
			}
			else if ((y > 500 && y >= Math.Abs(x)) || povAngle == 18000 || povAngle == 22500 || povAngle == 13500)
			{
				buttonPressed[15] = true;
			}
			rightStick = new Point(currentJoystickState.Rx, currentJoystickState.Ry);
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
			if (effectObject != null)
			{
				try
				{
					effectObject.Start(1);
				}
				catch (Exception)
				{
				}
			}
		}

		public void StopShock()
		{
			if (!(effectObject != null))
			{
				return;
			}
			try
			{
				if (effectObject.EffectStatus.Playing)
				{
					effectObject.Stop();
				}
			}
			catch (Exception)
			{
			}
		}

		public PointF GetPosition()
		{
			return new PointF((Math.Abs(rightStick.X) < 250) ? 0f : ((float)(Math.Sign(rightStick.X) * (Math.Abs(rightStick.X) - 250)) / 750f), (Math.Abs(rightStick.Y) < 250) ? 0f : ((float)(Math.Sign(rightStick.Y) * (Math.Abs(rightStick.Y) - 250)) / 750f));
		}

		public void Dispose()
		{
			if (!(directInputDevice != null))
			{
				return;
			}
			try
			{
				if (null != effectObject)
				{
					effectObject.Stop();
					effectObject = null;
				}
				directInputDevice.Unacquire();
				directInputDevice.Properties.AutoCenter = true;
				directInputDevice.Dispose();
				directInputDevice = null;
			}
			catch (Exception)
			{
			}
		}
	}
}
