using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace KidFansChannel.InputCore
{
	public class InputCore : IDisposable
	{
		public enum Direction
		{
			None,
			Up,
			UpRight,
			Right,
			DownRight,
			Down,
			DownLeft,
			Left,
			UpLeft
		}

		private KeyBoardInput keyBoard;

		private GamePadInput gamePad;

		private XInput xInput;

		private Dictionary<int, KeyBoardInput.Key> keyBoardMap = new Dictionary<int, KeyBoardInput.Key>();

		private Dictionary<int, int> gamePadMap = new Dictionary<int, int>();

		private List<int> indexList = new List<int>();

		private bool[] keyPress;

		private bool[] keyDown;

		private bool[] directionPress = new bool[4];

		private bool[] directionDown = new bool[4];

		private PointF position;

		[CompilerGenerated]
		private bool UseXInput__BackingField;

		public bool UseXInput
		{
			[CompilerGenerated]
			get
			{
				return UseXInput__BackingField;
			}
			[CompilerGenerated]
			set
			{
				UseXInput__BackingField = value;
			}
		}

		public InputCore()
		{
			keyBoard = new KeyBoardInput();
			xInput = new XInput();
			gamePad = new GamePadInput();
		}

		public List<GamePadInput.GamePadInstance> GetGamePadList()
		{
			if (!UseXInput)
			{
				return gamePad.GetGamePadList();
			}
			return null;
		}

		public virtual void Initialize(GamePadInput.GamePadInstance gamePadInstance, Form form)
		{
			form.Show();
			keyBoard.Initialize(form);
			if (!UseXInput && gamePadInstance != null)
			{
				gamePad.Initialize(gamePadInstance, form);
			}
		}

		~InputCore()
		{
			Dispose();
		}

		public void Dispose()
		{
			keyBoard.Dispose();
			gamePad.Dispose();
		}

		public void StartShock()
		{
			if (UseXInput)
			{
				xInput.StartShock();
			}
			else
			{
				gamePad.StartShock();
			}
		}

		public void StopShock()
		{
			if (UseXInput)
			{
				xInput.StopShock();
			}
			else
			{
				gamePad.StopShock();
			}
		}

		public void AddMap(int index, KeyBoardInput.Key key)
		{
			if (key == KeyBoardInput.Key.Up || key == KeyBoardInput.Key.Down || key == KeyBoardInput.Key.Left || key == KeyBoardInput.Key.Right)
			{
				throw new ArgumentOutOfRangeException();
			}
			keyBoardMap.Add(index, key);
			if (!indexList.Contains(index))
			{
				indexList.Add(index);
			}
		}

		public void AddMap(int index, int button)
		{
			if (button >= 0 && button < 12)
			{
				gamePadMap.Add(index, button);
				if (!indexList.Contains(index))
				{
					indexList.Add(index);
				}
			}
		}

		public void FinishMap()
		{
			keyPress = new bool[indexList.Count];
			keyDown = new bool[indexList.Count];
		}

		public void Update()
		{
			keyBoard.Update();
			if (UseXInput)
			{
				xInput.Update();
			}
			else
			{
				gamePad.Update();
			}
			for (int i = 0; i < indexList.Count; i++)
			{
				bool flag = false;
				if (keyBoardMap.ContainsKey(indexList[i]) && keyBoard.GetKeyState(keyBoardMap[indexList[i]]))
				{
					flag = true;
				}
				if (UseXInput)
				{
					if (gamePadMap.ContainsKey(indexList[i]) && xInput.GetButtonState(gamePadMap[indexList[i]]))
					{
						flag = true;
					}
				}
				else if (gamePadMap.ContainsKey(indexList[i]) && gamePad.GetButtonState(gamePadMap[indexList[i]]))
				{
					flag = true;
				}
				if (flag != keyDown[i])
				{
					keyPress[i] = flag;
					keyDown[i] = flag;
				}
			}
			GetDirection();
			if (UseXInput)
			{
				position = xInput.GetPosition();
			}
			else
			{
				position = gamePad.GetPosition();
			}
		}

		public void Clear()
		{
			for (int i = 0; i < indexList.Count; i++)
			{
				keyPress[i] = false;
				keyDown[i] = false;
			}
			for (int j = 0; j < 4; j++)
			{
				directionDown[j] = false;
				directionPress[j] = false;
			}
			position = PointF.Empty;
		}

		public bool CheckPress(int index)
		{
			int num = indexList.IndexOf(index);
			if (num == -1)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (keyPress[num])
			{
				keyPress[num] = false;
				return true;
			}
			return false;
		}

		public bool CheckDown(int index)
		{
			int num = indexList.IndexOf(index);
			if (num == -1)
			{
				throw new ArgumentOutOfRangeException();
			}
			return keyDown[num];
		}

		public Direction GetDirection()
		{
			Direction[,] array = new Direction[3, 3]
			{
				{
					Direction.UpLeft,
					Direction.Left,
					Direction.DownLeft
				},
				{
					Direction.Up,
					Direction.None,
					Direction.Down
				},
				{
					Direction.UpRight,
					Direction.Right,
					Direction.DownRight
				}
			};
			int num = 0;
			int num2 = 0;
			bool[] array2 = ((!UseXInput) ? new bool[4]
			{
				keyBoard.GetKeyState(KeyBoardInput.Key.Left) || gamePad.GetButtonState(12),
				keyBoard.GetKeyState(KeyBoardInput.Key.Right) || gamePad.GetButtonState(13),
				keyBoard.GetKeyState(KeyBoardInput.Key.Up) || gamePad.GetButtonState(14),
				keyBoard.GetKeyState(KeyBoardInput.Key.Down) || gamePad.GetButtonState(15)
			} : new bool[4]
			{
				keyBoard.GetKeyState(KeyBoardInput.Key.Left) || xInput.GetButtonState(12),
				keyBoard.GetKeyState(KeyBoardInput.Key.Right) || xInput.GetButtonState(13),
				keyBoard.GetKeyState(KeyBoardInput.Key.Up) || xInput.GetButtonState(14),
				keyBoard.GetKeyState(KeyBoardInput.Key.Down) || xInput.GetButtonState(15)
			});
			bool[] array3 = (bool[])array2.Clone();
			if (array3[0] == array3[1])
			{
				array3[0] = false;
				array3[1] = false;
			}
			if (array3[2] == array3[3])
			{
				array3[2] = false;
				array3[3] = false;
			}
			for (int i = 0; i < 4; i++)
			{
				if (directionDown[i] != array3[i])
				{
					directionPress[i] = array3[i];
					directionDown[i] = array3[i];
				}
			}
			if (array2[0] == array2[1])
			{
				num = 0;
			}
			else if (array2[0])
			{
				num = -1;
			}
			else if (array2[1])
			{
				num = 1;
			}
			if (array2[2] == array2[3])
			{
				num2 = 0;
			}
			else if (array2[2])
			{
				num2 = -1;
			}
			else if (array2[3])
			{
				num2 = 1;
			}
			return array[num + 1, num2 + 1];
		}

		public bool CheckDirectionPress(int direction)
		{
			if (direction < 0 || direction >= 4)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (directionPress[direction])
			{
				directionPress[direction] = false;
				return true;
			}
			return false;
		}

		public PointF CheckPosition()
		{
			return position;
		}
	}
}
