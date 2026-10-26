using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Xml;
using KidFansChannel.DataCore;
using KidFansChannel.GalCore;
using KidFansChannel.GalCore.Commands;
using KidFansChannel.Game2DCore;
using KidFansChannel.InputCore;

namespace KidFansChannel.CloseTo.Commands.Room
{
	public class RoomSelect : IInstanceDataUser
	{
		protected Select Parent;

		private string _fileStr;

		private List<int> objs = new List<int>();

		private List<List<string>> cases = new List<List<string>>();

		private List<string> objStrs = new List<string>();

		private List<List<string>> cmdStrs = new List<List<string>>();

		private Bitmap selectBmp;

		private RoomBg bg;

		private Picture picArrowBase;

		private Picture[] picArrows = new Picture[2];

		private Picture picTimeBack;

		private Picture picTimeBar;

		private Picture picTimeTextBase;

		private Picture[] picTimeTexts = new Picture[13];

		private Picture picPowerBase;

		private Picture[] picPowers = new Picture[4];

		private Picture picHintBase;

		private Picture[] picHints = new Picture[4];

		private AnimePicture picHintAnime;

		private bool dataReady;

		private int[] timeTexts = new int[5];

		private float[] powerTimes;

		private bool? moveLeft = null;

		private readonly int[,] objColors = new int[48, 3]
		{
			{ 104, 0, 8 },
			{ 216, 0, 40 },
			{ 104, 0, 56 },
			{ 224, 0, 120 },
			{ 192, 144, 184 },
			{ 144, 120, 144 },
			{ 64, 0, 56 },
			{ 40, 56, 0 },
			{ 56, 0, 184 },
			{ 192, 152, 144 },
			{ 144, 128, 48 },
			{ 0, 104, 176 },
			{ 112, 8, 0 },
			{ 160, 48, 0 },
			{ 104, 0, 104 },
			{ 72, 0, 0 },
			{ 136, 200, 200 },
			{ 152, 0, 24 },
			{ 0, 208, 80 },
			{ 176, 0, 104 },
			{ 0, 176, 24 },
			{ 16, 192, 112 },
			{ 184, 88, 8 },
			{ 208, 192, 32 },
			{ 192, 224, 152 },
			{ 40, 240, 0 },
			{ 168, 160, 0 },
			{ 120, 112, 0 },
			{ 248, 248, 152 },
			{ 232, 72, 0 },
			{ 112, 128, 0 },
			{ 0, 0, 0 },
			{ 240, 64, 80 },
			{ 112, 40, 0 },
			{ 152, 64, 8 },
			{ 224, 168, 24 },
			{ 240, 152, 112 },
			{ 32, 248, 8 },
			{ 248, 56, 232 },
			{ 120, 112, 248 },
			{ 192, 96, 176 },
			{ 64, 152, 32 },
			{ 152, 120, 8 },
			{ 48, 0, 104 },
			{ 120, 0, 64 },
			{ 0, 152, 72 },
			{ 0, 0, 0 },
			{ 0, 0, 0 }
		};

		private readonly float[,] hintPoss = new float[48, 2]
		{
			{ 0f, 0f },
			{ 0.0293f, 0.5804f },
			{ 0f, 0f },
			{ 0.8057f, 0.4911f },
			{ 0.7158f, 0.7813f },
			{ 0.6611f, 0.2924f },
			{ 0.6611f, 0.1696f },
			{ 0.6328f, 0.808f },
			{ 0.7031f, 0.8705f },
			{ 0.5928f, 0.5513f },
			{ 0.5264f, 0.4643f },
			{ 0.5654f, 0.4621f },
			{ 0.5537f, 0.5513f },
			{ 0.4805f, 0.3705f },
			{ 0.502f, 0.2946f },
			{ 0.4932f, 0.1094f },
			{ 0.4932f, 0.5513f },
			{ 0.4199f, 0.7589f },
			{ 0.4512f, 0.7522f },
			{ 0.4365f, 0.558f },
			{ 0.416f, 0.4821f },
			{ 0.4385f, 0.6563f },
			{ 0.3496f, 0.7478f },
			{ 0.4395f, 0.1585f },
			{ 0.4395f, 0.2589f },
			{ 0.3496f, 0.5848f },
			{ 0.3496f, 0.5f },
			{ 0.3496f, 0.4152f },
			{ 0.3496f, 0.6674f },
			{ 0.3135f, 0.8013f },
			{ 0.3936f, 0.2924f },
			{ 0f, 0f },
			{ 0.3799f, 0.3371f },
			{ 0.2588f, 0.2411f },
			{ 0.249f, 0.3795f },
			{ 0.3516f, 0.3013f },
			{ 0.1855f, 0.4732f },
			{ 0.4912f, 0.4464f },
			{ 0.4033f, 0.9576f },
			{ 0.6074f, 0.4777f },
			{ 0.4434f, 0.4286f },
			{ 0.8877f, 0.7969f },
			{ 0.6523f, 0.9286f },
			{ 0.3525f, 0.8304f },
			{ 0.1182f, 0.2232f },
			{ 0.1953f, 0.4688f },
			{ 0.5254f, 0.7366f },
			{ 0f, 0f }
		};

		private bool? leftArrow;

		private float[] posArrow = new float[2];

		private bool transparent;

		private Point labelPoint;

		private float moveTime;

		private XmlDocument doc = new XmlDocument();

		private int currentObj = -1;

		private bool objStage = true;

		public string FileStr
		{
			get
			{
				return _fileStr;
			}
			set
			{
				_fileStr = value;
			}
		}

		public bool DataReady
		{
			get
			{
				if (dataReady)
				{
					return true;
				}
				if (picArrowBase.IsCreated && picTimeBack.IsCreated && picTimeBar.IsCreated && picTimeTextBase.IsCreated && picPowerBase.IsCreated && picHintBase.IsCreated)
				{
					picArrows[0] = GameManager.Graphics.CreateSubPicture(picArrowBase, new Rectangle(0, 0, 40, 40));
					picArrows[1] = GameManager.Graphics.CreateSubPicture(picArrowBase, new Rectangle(40, 0, 40, 40));
					for (int i = 0; i < 13; i++)
					{
						picTimeTexts[i] = GameManager.Graphics.CreateSubPicture(picTimeTextBase, new Rectangle(i % 5 * 20, i / 5 * 30, 20, 30));
					}
					for (int j = 0; j < 4; j++)
					{
						picPowers[j] = GameManager.Graphics.CreateSubPicture(picPowerBase, new Rectangle(j % 2 * 40, j / 2 * 40, 40, 40));
					}
					for (int k = 0; k < 4; k++)
					{
						picHints[k] = GameManager.Graphics.CreateSubPicture(picHintBase, new Rectangle(k % 2 * 40, k / 2 * 40, 40, 40));
					}
					picHintAnime = GameManager.Graphics.CreateAnimePicture(new Size(40, 40));
					for (int l = 0; l < 4; l++)
					{
						picHintAnime.AddFrame(picHints[l], 0.125f);
					}
					picHintAnime.Alpha = 192;
					picHintAnime.Center = new Point(20, 20);
					dataReady = true;
					return true;
				}
				return false;
			}
		}

		public RoomSelect(Select parent)
		{
			Parent = parent;
		}

		public void ConvertXml(XmlNode node, NodeInfo info)
		{
			_fileStr = ScriptHelper.GetArgumentString(node, info, "file");
			for (int i = 0; i < Parent.Cases.Count; i++)
			{
				int result;
				if (!Parent.Cases[i].StartsWith("Obj") || !int.TryParse(Parent.Cases[i].Substring(3, 2), out result))
				{
					throw new ScriptArgumentException(node, info, "Case must start with obj + 2 digits number");
				}
				if (objs.Contains(result))
				{
					int index = objs.IndexOf(result);
					cases[index].Add(Parent.Cases[i]);
					cmdStrs[index].Add(Parent.ItemStrings[i].String.Split('_')[1]);
					continue;
				}
				int count = objs.Count;
				objs.Add(result);
				cases.Add(new List<string>());
				cases[count].Add(Parent.Cases[i]);
				objStrs.Add(Parent.ItemStrings[i].String.Split('_')[0]);
				cmdStrs.Add(new List<string>());
				cmdStrs[count].Add(Parent.ItemStrings[i].String.Split('_')[1]);
			}
		}

		public void InitInstanceData()
		{
			foreach (ScriptCommand currentCommand in GameManager.GalCore.CurrentCommandList)
			{
				if (currentCommand is BgDisplay && ((CloseToBgDisplay)currentCommand).Room != null)
				{
					bg = ((CloseToBgDisplay)currentCommand).Room;
				}
			}
			if (bg == null)
			{
				throw new RuntimeException(Parent.Node, Parent.Info, "No Room BgDisplay.");
			}
			switch (GameManager.Data.CheckData("bg", "atr\\" + _fileStr + ".png"))
			{
			case KidFansChannel.DataCore.DataCore.DataType.Package:
				selectBmp = new Bitmap(GameManager.Data.GetStream());
				break;
			case KidFansChannel.DataCore.DataCore.DataType.File:
				selectBmp = new Bitmap(GameManager.Data.GetFile());
				break;
			default:
				throw new FileNotFoundException(null, "bg\\atr\\" + _fileStr + ".png");
			}
			picArrowBase = GameManager.Data.LoadPicture("system", "arrow.png");
			picTimeBack = GameManager.Data.LoadPicture("system", "roomtime_back.png");
			picTimeBar = GameManager.Data.LoadPicture("system", "roomtime_bar.png");
			picTimeTextBase = GameManager.Data.LoadPicture("system", "roomtime_text.png");
			picPowerBase = GameManager.Data.LoadPicture("system", "power.png");
			picHintBase = GameManager.Data.LoadPicture("system", "hint.png");
		}

		public void FinalInstanceData()
		{
			selectBmp.Dispose();
			for (int i = 0; i < 2; i++)
			{
				picArrows[i].Dispose();
			}
			picArrowBase.Dispose();
			picTimeBack.Dispose();
			picTimeBar.Dispose();
			for (int j = 0; j < 13; j++)
			{
				picTimeTexts[j].Dispose();
			}
			picTimeTextBase.Dispose();
			for (int k = 0; k < 4; k++)
			{
				picPowers[k].Dispose();
			}
			picPowerBase.Dispose();
			picHintAnime.Dispose();
			for (int l = 0; l < 4; l++)
			{
				picHints[l].Dispose();
			}
			picHintBase.Dispose();
		}

		public void Init()
		{
		}

		public void Final()
		{
			((CloseToCore)GameManager.GalCore).CursorActive = false;
		}

		public void Show()
		{
			picArrows[0].Position = new Point(-40, 280);
			picArrows[0].Alpha = 0;
			picArrows[0].BeginTransform(0.5f);
			picArrows[0].PositionTo(new Point(0, 280));
			picArrows[0].AlphaTo(192);
			picArrows[0].EndTransform();
			picArrows[1].Position = new Point(800, 280);
			picArrows[1].Alpha = 0;
			picArrows[1].BeginTransform(0.5f);
			picArrows[1].PositionTo(new Point(760, 280));
			picArrows[1].AlphaTo(192);
			picArrows[1].EndTransform();
			picTimeBack.Position = new Point(615, -35);
			picTimeBack.Alpha = 0;
			picTimeBack.BeginTransform(0.5f);
			picTimeBack.PositionTo(new Point(615, 15));
			picTimeBack.AlphaTo(255);
			picTimeBack.EndTransform();
			picTimeBar.Position = new Point(615, -35);
			picTimeBar.Alpha = 0;
			picTimeBar.BeginTransform(0.5f);
			picTimeBar.PositionTo(new Point(615, 15));
			picTimeBar.AlphaTo(255);
			picTimeBar.EndTransform();
			for (int i = 0; i < 13; i++)
			{
				picTimeTexts[i].Alpha = 0;
				picTimeTexts[i].BeginTransform(0.5f);
				picTimeTexts[i].AlphaTo(255);
				picTimeTexts[i].EndTransform();
			}
			((CloseToCore)GameManager.GalCore).AlwaysShowCursor = true;
			string text = GameManager.GameState.CurrentGameState.GameDateTime.Substring(6, 5);
			if (text == "??:??")
			{
				text = "00:00";
			}
			int hour = int.Parse(text.Substring(0, 2));
			if (hour < 12)
			{
				timeTexts[0] = 11;
			}
			else
			{
				timeTexts[0] = 12;
				hour -= 12;
			}
			if (hour < 10)
			{
				timeTexts[1] = 0;
			}
			else
			{
				timeTexts[1] = 1;
				hour -= 10;
			}
			timeTexts[2] = hour;
			int minute = int.Parse(text.Substring(3, 2));
			timeTexts[3] = minute / 10;
			timeTexts[4] = minute % 10;
			int value = GameManager.GameState.CurrentGameState.VarDefines["Power"].Value;
			powerTimes = new float[value];
			Random random = new Random();
			for (int j = 0; j < powerTimes.Length; j++)
			{
				powerTimes[j] = (float)(random.NextDouble() * 2.0);
			}
			for (int k = 0; k < 4; k++)
			{
				picPowers[k].Position = new Point(120, -15);
				picPowers[k].Alpha = 0;
				picPowers[k].BeginTransform(0.5f);
				picPowers[k].PositionTo(new Point(120, 35));
				picPowers[k].AlphaTo(255);
				picPowers[k].EndTransform();
			}
		}

		public void Hide()
		{
			picArrows[0].BeginTransform(0.5f);
			picArrows[0].PositionTo(new Point(-40, 280));
			picArrows[0].AlphaTo(0);
			picArrows[0].EndTransform();
			picArrows[1].BeginTransform(0.5f);
			picArrows[1].PositionTo(new Point(800, 280));
			picArrows[1].AlphaTo(0);
			picArrows[1].EndTransform();
			picTimeBack.BeginTransform(0.5f);
			picTimeBack.PositionTo(new Point(615, -35));
			picTimeBack.AlphaTo(0);
			picTimeBack.EndTransform();
			picTimeBar.BeginTransform(0.5f);
			picTimeBar.PositionTo(new Point(615, -35));
			picTimeBar.AlphaTo(0);
			picTimeBar.EndTransform();
			for (int i = 0; i < 13; i++)
			{
				picTimeTexts[i].BeginTransform(0.5f);
				picTimeTexts[i].AlphaTo(0);
				picTimeTexts[i].EndTransform();
			}
			for (int j = 0; j < 4; j++)
			{
				picPowers[j].BeginTransform(0.5f);
				picPowers[j].PositionTo(new Point(120, -15));
				picPowers[j].AlphaTo(0);
				picPowers[j].EndTransform();
			}
			((CloseToCore)GameManager.GalCore).AlwaysShowCursor = false;
		}

		public bool CheckShown()
		{
			if (picTimeBack.Time == 0f)
			{
				return picTimeBack.Alpha == 255;
			}
			return false;
		}

		public bool CheckHiden()
		{
			if (picTimeBack.Time == 0f)
			{
				return picTimeBack.Alpha == 0;
			}
			return false;
		}

		public void DrawBack(ICanvas canvas)
		{
			for (int i = 0; i < 2; i++)
			{
				canvas.DrawPicture(picArrows[i]);
			}
			canvas.DrawPicture(picTimeBack);
			canvas.DrawPicture(picTimeBar);
			int y = picTimeBack.Y + 45;
			for (int j = 0; j < 3; j++)
			{
				picTimeTexts[timeTexts[j]].Position = new Point(picTimeBack.X + 25 + j * 20, y);
				canvas.DrawPicture(picTimeTexts[timeTexts[j]]);
			}
			picTimeTexts[10].Position = new Point(picTimeBack.X + 80, y);
			canvas.DrawPicture(picTimeTexts[10]);
			for (int k = 3; k < 5; k++)
			{
				picTimeTexts[timeTexts[k]].Position = new Point(picTimeBack.X + 35 + k * 20, y);
				canvas.DrawPicture(picTimeTexts[timeTexts[k]]);
			}
			for (int l = 0; l < powerTimes.Length; l++)
			{
				int hour = (int)(powerTimes[l] / 0.5f);
				picPowers[hour].X = 120 + 40 * l;
				canvas.DrawPicture(picPowers[hour]);
			}
			if (GameManager.GalCore.GameOptions.HintShow)
			{
				for (int m = 0; m < objs.Count; m++)
				{
					picHintAnime.Position = RoomBg.PictureToScreen(new PointF(hintPoss[objs[m], 0], hintPoss[objs[m], 1]), bg.CamPos);
					canvas.DrawPicture(picHintAnime);
				}
			}
			if (currentObj != -1)
			{
				Label.Draw();
			}
		}

		public void UpdateBack(float elaspedTime)
		{
			for (int i = 0; i < 2; i++)
			{
				picArrows[i].Update(elaspedTime);
			}
			picTimeBack.Update(elaspedTime);
			picTimeBar.Update(elaspedTime);
			for (int j = 0; j < 13; j++)
			{
				picTimeTexts[j].Update(elaspedTime);
			}
			for (int k = 0; k < 4; k++)
			{
				picPowers[k].Update(elaspedTime);
			}
			for (int l = 0; l < powerTimes.Length; l++)
			{
				powerTimes[l] += elaspedTime;
				while (powerTimes[l] >= 2f)
				{
					powerTimes[l] -= 2f;
				}
			}
			picHintAnime.Update(elaspedTime);
			if (!moveLeft.HasValue)
			{
				return;
			}
			bool found = false;
			if (moveTime > 0f)
			{
				if (moveTime > elaspedTime)
				{
					moveTime -= elaspedTime;
				}
				else
				{
					elaspedTime = moveTime;
					moveTime = 0f;
					found = true;
				}
			}
			if (moveLeft.Value)
			{
				bg.MoveLeft(elaspedTime);
			}
			else
			{
				bg.MoveRight(elaspedTime);
			}
			if (found)
			{
				moveLeft = null;
			}
			GameManager.GalCore.GameInput.RefreshMouse();
		}

		public void UpdateItem(float elaspedTime)
		{
			if (objStage)
			{
				if (currentObj >= 0)
				{
					((CloseToCore)GameManager.GalCore).CursorActive = true;
				}
				else
				{
					((CloseToCore)GameManager.GalCore).CursorActive = false;
				}
			}
			else if (Parent.CurrentSelected >= 0)
			{
				((CloseToCore)GameManager.GalCore).CursorActive = true;
			}
			else
			{
				((CloseToCore)GameManager.GalCore).CursorActive = false;
			}
		}

		public void OnKey()
		{
			if (Parent.ItemStrings.Count == 0)
			{
				return;
			}
			if (objStage)
			{
				if (moveTime != 0f)
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
				}
				else
				{
					moveLeft = null;
					if (GameManager.GalCore.GameInput.GetDirection() == KidFansChannel.InputCore.InputCore.Direction.Left)
					{
						moveLeft = true;
					}
					else if (GameManager.GalCore.GameInput.GetDirection() == KidFansChannel.InputCore.InputCore.Direction.Right)
					{
						moveLeft = false;
					}
				}
			}
			else if (GameManager.GalCore.GameInput.CheckDirectionPress(2))
			{
				if (Label.StringBase.CurrentPointedLinkValue == -1)
				{
					Label.StringBase.CurrentPointedLinkValue = cases[currentObj].Count;
				}
				Label.StringBase.CurrentPointedLinkValue--;
			}
			else if (GameManager.GalCore.GameInput.CheckDirectionPress(3))
			{
				Label.StringBase.CurrentPointedLinkValue++;
				if (Label.StringBase.CurrentPointedLinkValue == cases[currentObj].Count)
				{
					Label.StringBase.CurrentPointedLinkValue = -1;
				}
			}
		}

		public void OnMove(Point location)
		{
			Parent.CurrentSelected = -1;
			if (objStage)
			{
				bool? arrowChanged = leftArrow;
				leftArrow = null;
				if (location.Y >= 280 && location.Y < 320)
				{
					if (location.X >= 0 && location.X < 40)
					{
						leftArrow = true;
					}
					else if (location.X >= 760 && location.X < 800)
					{
						leftArrow = false;
					}
				}
				if (arrowChanged != leftArrow)
				{
					if (arrowChanged.HasValue)
					{
						if (arrowChanged.Value)
						{
							picArrows[0].BeginTransform(0.5f);
							picArrows[0].PositionTo(new Point(0, 280));
							picArrows[0].EndTransform();
						}
						else
						{
							picArrows[1].BeginTransform(0.5f);
							picArrows[1].PositionTo(new Point(760, 280));
							picArrows[1].EndTransform();
						}
					}
					if (leftArrow.HasValue)
					{
						if (leftArrow.Value)
						{
							picArrows[0].BeginTransform(0f);
							picArrows[0].PositionTo(new Point(-10, 280), new Point(10, 280), 0.5f);
							picArrows[0].EndTransform();
						}
						else
						{
							picArrows[1].BeginTransform(0f);
							picArrows[1].PositionTo(new Point(770, 280), new Point(750, 280), 0.5f);
							picArrows[1].EndTransform();
						}
						return;
					}
				}
				if (location.Y < 120)
				{
					if (!transparent)
					{
						transparent = true;
						picTimeBack.BeginTransform(0.5f);
						picTimeBack.AlphaTo(128);
						picTimeBack.EndTransform();
						picTimeBar.BeginTransform(0.5f);
						picTimeBar.AlphaTo(128);
						picTimeBar.EndTransform();
						for (int i = 0; i < 13; i++)
						{
							picTimeTexts[i].BeginTransform(0.5f);
							picTimeTexts[i].AlphaTo(128);
							picTimeTexts[i].EndTransform();
						}
						for (int j = 0; j < 4; j++)
						{
							picPowers[j].BeginTransform(0.5f);
							picPowers[j].AlphaTo(128);
							picPowers[j].EndTransform();
						}
					}
				}
				else if (transparent)
				{
					transparent = false;
					picTimeBack.BeginTransform(0.5f);
					picTimeBack.AlphaTo(255);
					picTimeBack.EndTransform();
					picTimeBar.BeginTransform(0.5f);
					picTimeBar.AlphaTo(255);
					picTimeBar.EndTransform();
					for (int k = 0; k < 13; k++)
					{
						picTimeTexts[k].BeginTransform(0.5f);
						picTimeTexts[k].AlphaTo(255);
						picTimeTexts[k].EndTransform();
					}
					for (int l = 0; l < 4; l++)
					{
						picPowers[l].BeginTransform(0.5f);
						picPowers[l].AlphaTo(255);
						picPowers[l].EndTransform();
					}
				}
				currentObj = -1;
				int hour;
				int pixelY;
				if (GameManager.GalCore.GameOptions.Graphics != GameOptions.Level.Low)
				{
					PointF pointF = RoomBg.ScreenToPicture(location, bg.CamPos);
					hour = (int)(pointF.X * (float)selectBmp.Width + 0.5f);
					pixelY = (int)(pointF.Y * (float)selectBmp.Height + 0.5f);
				}
				else
				{
					hour = (int)(((float)location.X + (bg.CamPos - 630f / 4095f) * 1200f * 39f / 27f) / 2000f * (float)selectBmp.Width + 0.5f);
					pixelY = (int)((float)location.Y / 600f * (float)selectBmp.Height + 0.5f);
				}
				if (hour < 0 || hour >= selectBmp.Width || pixelY < 0 || pixelY >= selectBmp.Height)
				{
					return;
				}
				Color pixel = selectBmp.GetPixel(hour, pixelY);
				if (pixel != Color.FromArgb(248, 248, 248))
				{
					for (int m = 0; m < objs.Count; m++)
					{
						if (pixel == Color.FromArgb(objColors[objs[m], 0], objColors[objs[m], 1], objColors[objs[m], 2]))
						{
							currentObj = m;
							break;
						}
					}
				}
				if (currentObj != -1)
				{
					Label.Update(location, objStrs[currentObj]);
				}
			}
			else
			{
				Label.StringBase.OnItemMove(location);
			}
		}

		public void OnClick()
		{
			if (objStage)
			{
				if (leftArrow.HasValue)
				{
					moveTime = 9f / 26f;
					moveLeft = leftArrow.Value;
				}
				else if (currentObj != -1)
				{
					string text = objStrs[currentObj];
					for (int i = 0; i < cmdStrs[currentObj].Count; i++)
					{
						text = text + "\n " + cmdStrs[currentObj][i];
					}
					labelPoint = Label.Location;
					Label.Update(labelPoint, text);
					Label.StringBase.ClearString();
					string text2 = "<string>" + objStrs[currentObj];
					for (int j = 0; j < cmdStrs[currentObj].Count; j++)
					{
						string text3 = text2;
						text2 = text3 + "<br /><l /><e value=\"" + j + "\">" + cmdStrs[currentObj][j] + "</e>";
					}
					text2 += "</string>";
					doc.LoadXml(text2);
					Label.StringBase.AddXmlString(doc["string"].ChildNodes, new NodeInfo());
					objStage = false;
					GameManager.Data.LoadAndPlaySystemSound("ok.ogg");
				}
			}
			else if (Label.StringBase.OnItemClick())
			{
				Parent.CurrentSelected = Parent.Cases.IndexOf(cases[currentObj][Label.StringBase.CurrentPointedLinkValue]);
				GameManager.Data.LoadAndPlaySystemSound("ok.ogg");
				((CloseToCore)GameManager.GalCore).CursorActive = false;
			}
			else
			{
				currentObj = -1;
				objStage = true;
			}
		}
	}
}
