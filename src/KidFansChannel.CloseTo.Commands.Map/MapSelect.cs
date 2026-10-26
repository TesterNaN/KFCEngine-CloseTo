using System.Drawing;
using System.Xml;
using KidFansChannel.GalCore;
using KidFansChannel.GalCore.Commands;
using KidFansChannel.Game2DCore;

namespace KidFansChannel.CloseTo.Commands.Map
{
	public class MapSelect : IInstanceDataUser
	{
		protected Select Parent;

		private int[] _icons;

		private string[] _titles;

		private string _mapStr;

		private string[][] _hints;

		private Picture map;

		private Picture iconBase;

		private Picture[] iconPics;

		private Picture mask;

		private Picture[][] picHints;

		private SubPicture[][][] picHintSubs;

		private AnimePicture[][] picHintAnimes;

		private bool mapCreated;

		private int toSelected = -1;

		private static readonly Point[] iconLocations = new Point[12]
		{
			new Point(50, 120),
			new Point(240, 110),
			new Point(180, 330),
			new Point(220, 450),
			new Point(30, 370),
			new Point(630, 80),
			new Point(360, 430),
			new Point(600, 460),
			new Point(510, 360),
			new Point(460, 250),
			new Point(390, 100),
			new Point(510, 140)
		};

		private static readonly Rectangle[] iconRects = new Rectangle[12]
		{
			new Rectangle(80, 120, 90, 100),
			new Rectangle(250, 110, 130, 100),
			new Rectangle(190, 330, 120, 90),
			new Rectangle(220, 460, 140, 70),
			new Rectangle(30, 370, 150, 80),
			new Rectangle(640, 80, 120, 90),
			new Rectangle(380, 440, 120, 90),
			new Rectangle(610, 490, 120, 60),
			new Rectangle(520, 380, 130, 70),
			new Rectangle(460, 260, 140, 90),
			new Rectangle(420, 120, 100, 80),
			new Rectangle(540, 150, 90, 80)
		};

		private static readonly string[] iconTitles = new string[12]
		{
			"Church", "Hospital", "Observation", "Park", "Reservoir", "HighSchool", "PrimSchool", "Shop", "Housing", "Accident",
			"Shoko", "Room"
		};

		public int[] Icons
		{
			get
			{
				return _icons;
			}
			set
			{
				_icons = value;
			}
		}

		public string[] Titles
		{
			get
			{
				return _titles;
			}
			set
			{
				_titles = value;
			}
		}

		public string MapStr
		{
			get
			{
				return _mapStr;
			}
			set
			{
				_mapStr = value;
			}
		}

		protected string[][] Hints
		{
			get
			{
				return _hints;
			}
			set
			{
				_hints = value;
			}
		}

		public bool DataReady
		{
			get
			{
				if (mapCreated)
				{
					return true;
				}
				if (!map.IsCreated || !iconBase.IsCreated)
				{
					return false;
				}
				for (int i = 0; i < _hints.Length; i++)
				{
					for (int j = 0; j < _hints[i].Length; j++)
					{
						if (!picHints[i][j].IsCreated)
						{
							return false;
						}
					}
				}
				iconPics = new Picture[_icons.Length];
				for (int k = 0; k < _icons.Length; k++)
				{
					iconPics[k] = GameManager.Graphics.CreateSubPicture(iconBase, new Rectangle(_icons[k] % 3 * 150, _icons[k] / 3 * 100, 150, 100));
					iconPics[k].Position = iconLocations[_icons[k]];
				}
				for (int l = 0; l < _hints.Length; l++)
				{
					for (int m = 0; m < _hints[l].Length; m++)
					{
						picHintAnimes[l][m] = GameManager.Graphics.CreateAnimePicture(new Size(80, 80));
						for (int n = 0; n < 3; n++)
						{
							picHintSubs[l][m][n] = GameManager.Graphics.CreateSubPicture(picHints[l][m], new Rectangle(n * 80, 0, 80, 80));
						}
						for (int num = 0; num < 3; num++)
						{
							picHintAnimes[l][m].AddFrame(picHintSubs[l][m][0], 0.5f);
							picHintAnimes[l][m].AddFrame(picHintSubs[l][m][1], 0.5f);
						}
						picHintAnimes[l][m].AddFrame(picHintSubs[l][m][0], 0.5f);
						picHintAnimes[l][m].AddFrame(picHintSubs[l][m][2], 0.5f);
						picHintAnimes[l][m].Center = new Point(40, 40);
					}
				}
				mapCreated = true;
				return true;
			}
		}

		public MapSelect(Select parent)
		{
			Parent = parent;
		}

		public void ConvertXml(XmlNode node, NodeInfo info)
		{
			_icons = new int[Parent.Cases.Count];
			_titles = new string[Parent.ItemStrings.Count];
			for (int i = 0; i < Icons.Length; i++)
			{
				_icons[i] = -1;
				for (int j = 0; j < iconTitles.Length; j++)
				{
					if (iconTitles[j] == Parent.Cases[i])
					{
						_icons[i] = j;
						break;
					}
				}
				if (_icons[i] == -1)
				{
					throw new ScriptArgumentException(node.ChildNodes[i], info, "case", ArgumentType.String, "Not a valid Map Icon.");
				}
				_titles[i] = Parent.ItemStrings[i].String;
			}
			switch (ScriptHelper.GetArgumentString(node, info, "time", "day"))
			{
			case "day":
				_mapStr = "map_a";
				break;
			case "evening":
				_mapStr = "map_e";
				break;
			case "night":
				_mapStr = "map_n";
				break;
			default:
				throw new ScriptArgumentException(node, info, "time", ArgumentType.String, "Time can only be day, evening, night.");
			}
			_hints = new string[node.ChildNodes.Count][];
			for (int k = 0; k < node.ChildNodes.Count; k++)
			{
				XmlNode node2 = node.ChildNodes[k];
				string argumentString = ScriptHelper.GetArgumentString(node2, info, "hint", string.Empty);
				if (argumentString == string.Empty)
				{
					_hints[k] = new string[0];
					continue;
				}
				_hints[k] = argumentString.Split(';');
			}
		}

		public void Init()
		{
			mask = GameManager.Graphics.CreatePicture(Color.White);
			mask.Size = BgDisplay.ScreenSize;
			for (int i = 0; i < _hints.Length; i++)
			{
				Point point = new Point(iconRects[_icons[i]].X + iconRects[_icons[i]].Width / 2, iconRects[_icons[i]].Y + iconRects[_icons[i]].Height / 2);
				for (int j = 0; j < _hints[i].Length; j++)
				{
					picHintAnimes[i][j].Position = new Point(point.X - 40 * (_hints[i].Length - 1) + 80 * j, point.Y);
				}
			}
		}

		public void Final()
		{
			mask.Dispose();
			mask = null;
			((CloseToCore)GameManager.GalCore).CursorActive = false;
		}

		public void Show()
		{
			mask.Alpha = 255;
			mask.BeginTransform(1f / 3f);
			mask.AlphaTo(0);
			mask.EndTransform();
			((CloseToCore)GameManager.GalCore).AlwaysShowCursor = true;
		}

		public void Hide()
		{
			mask.BeginTransform(1f / 3f);
			mask.AlphaTo(255);
			mask.EndTransform();
			((CloseToCore)GameManager.GalCore).AlwaysShowCursor = false;
		}

		public bool CheckShown()
		{
			if (mask.Time == 0f)
			{
				return mask.Alpha == 0;
			}
			return false;
		}

		public bool CheckHiden()
		{
			if (mask.Time == 0f)
			{
				return mask.Alpha == 255;
			}
			return false;
		}

		public void DrawBack(ICanvas canvas)
		{
			canvas.DrawPicture(map);
			for (int i = 0; i < iconPics.Length; i++)
			{
				canvas.DrawPicture(iconPics[i]);
			}
			if (GameManager.GalCore.GameOptions.HintShow)
			{
				for (int j = 0; j < _hints.Length; j++)
				{
					for (int k = 0; k < _hints[j].Length; k++)
					{
						canvas.DrawPicture(picHintAnimes[j][k]);
					}
				}
			}
			if (Parent.CurrentSelected != -1 && mask.Alpha == 0 && (toSelected == -1 || toSelected == Parent.CurrentSelected))
			{
				Label.Draw();
			}
			canvas.DrawPicture(mask);
		}

		public void UpdateBack(float elaspedTime)
		{
			mask.Update(elaspedTime);
			for (int i = 0; i < _hints.Length; i++)
			{
				for (int j = 0; j < _hints[i].Length; j++)
				{
					picHintAnimes[i][j].Update(elaspedTime);
				}
			}
		}

		public void UpdateItem(float elaspedTime)
		{
			if (Parent.CurrentSelected != -1 && (toSelected == -1 || toSelected == Parent.CurrentSelected))
			{
				((CloseToCore)GameManager.GalCore).CursorActive = true;
			}
			else
			{
				((CloseToCore)GameManager.GalCore).CursorActive = false;
			}
		}

		public void InitInstanceData()
		{
			map = GameManager.Data.LoadPicture("system", _mapStr + ".png");
			iconBase = GameManager.Data.LoadPicture("system", "map_icon.png");
			picHints = new Picture[_hints.Length][];
			picHintSubs = new SubPicture[_hints.Length][][];
			picHintAnimes = new AnimePicture[_hints.Length][];
			for (int i = 0; i < _hints.Length; i++)
			{
				picHints[i] = new Picture[_hints[i].Length];
				picHintSubs[i] = new SubPicture[_hints[i].Length][];
				picHintAnimes[i] = new AnimePicture[_hints[i].Length];
				for (int j = 0; j < _hints[i].Length; j++)
				{
					picHints[i][j] = GameManager.Data.LoadPicture("system", "hint_" + _hints[i][j] + ".png");
					picHintSubs[i][j] = new SubPicture[3];
				}
			}
		}

		public void FinalInstanceData()
		{
			for (int i = 0; i < iconPics.Length; i++)
			{
				iconPics[i].Dispose();
				iconPics[i] = null;
			}
			iconBase.Dispose();
			iconBase = null;
			map.Dispose();
			map = null;
			for (int j = 0; j < _hints.Length; j++)
			{
				for (int k = 0; k < _hints[j].Length; k++)
				{
					for (int l = 0; l < 3; l++)
					{
						picHintSubs[j][k][l].Dispose();
						picHintSubs[j][k][l] = null;
					}
					picHintAnimes[j][k].Dispose();
					picHintAnimes[j][k] = null;
					picHints[j][k].Dispose();
					picHints[j][k] = null;
				}
			}
		}

		public void OnMove(Point location)
		{
			Parent.CurrentSelected = -1;
			for (int i = 0; i < iconPics.Length; i++)
			{
				if (location.X >= iconRects[_icons[i]].X && location.X < iconRects[_icons[i]].X + iconRects[_icons[i]].Width && location.Y >= iconRects[_icons[i]].Y && location.Y < iconRects[_icons[i]].Y + iconRects[_icons[i]].Height)
				{
					Parent.CurrentSelected = i;
					break;
				}
			}
			if (Parent.CurrentSelected != -1 && (toSelected == -1 || toSelected == Parent.CurrentSelected))
			{
				Label.Update(location, _titles[Parent.CurrentSelected]);
				toSelected = -1;
			}
		}

		public void OnClick()
		{
			GameManager.Data.LoadAndPlaySystemSound("ok.ogg");
			((CloseToCore)GameManager.GalCore).CursorActive = false;
		}

		public void OnKey()
		{
			if (Parent.ItemStrings.Count == 0 || toSelected != -1)
			{
				return;
			}
			if (GameManager.GalCore.GameInput.CheckDown(3) || GameManager.GalCore.GameInput.CheckDown(2))
			{
				if (GameManager.GalCore.GameInput.CheckDirectionPress(0))
				{
					((CloseToCore)GameManager.GalCore).MoveMouseOffset(new Point(-50, 0));
				}
				else if (GameManager.GalCore.GameInput.CheckDirectionPress(1))
				{
					((CloseToCore)GameManager.GalCore).MoveMouseOffset(new Point(50, 0));
				}
				else if (GameManager.GalCore.GameInput.CheckDirectionPress(2))
				{
					((CloseToCore)GameManager.GalCore).MoveMouseOffset(new Point(0, -50));
				}
				else if (GameManager.GalCore.GameInput.CheckDirectionPress(3))
				{
					((CloseToCore)GameManager.GalCore).MoveMouseOffset(new Point(0, 50));
				}
				return;
			}
			bool flag = false;
			if (GameManager.GalCore.GameInput.CheckDirectionPress(2))
			{
				if (Parent.CurrentSelected == -1)
				{
					toSelected = Parent.ItemStrings.Count - 1;
				}
				else
				{
					toSelected = Parent.CurrentSelected - 1;
					if (toSelected == -1)
					{
						toSelected = Parent.ItemStrings.Count - 1;
					}
				}
				flag = true;
			}
			else if (GameManager.GalCore.GameInput.CheckDirectionPress(3))
			{
				if (Parent.CurrentSelected == -1)
				{
					toSelected = 0;
				}
				else
				{
					toSelected = Parent.CurrentSelected + 1;
					if (toSelected == Parent.ItemStrings.Count)
					{
						toSelected = 0;
					}
				}
				flag = true;
			}
			if (flag)
			{
				Point location = new Point(iconRects[_icons[toSelected]].X + iconRects[_icons[toSelected]].Width / 2, iconRects[_icons[toSelected]].Y + iconRects[_icons[toSelected]].Height / 2);
				((CloseToCore)GameManager.GalCore).MoveMouse(location);
			}
		}
	}
}
