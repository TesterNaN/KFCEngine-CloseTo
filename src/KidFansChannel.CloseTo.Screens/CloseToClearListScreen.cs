using System;
using System.Drawing;
using System.Windows.Forms;
using KidFansChannel.GalCore;
using KidFansChannel.GalCore.Screens;
using KidFansChannel.Game2DCore;

namespace KidFansChannel.CloseTo.Screens
{
	public class CloseToClearListScreen : ClearListScreen
	{
		private Picture[] picClearLists = new Picture[2];

		private Picture[] picEndings = new Picture[2];

		private Picture[] picSceneTitles = new Picture[2];

		private Picture picBack;

		private Picture picTitle;

		private String2D strClearListItem;

		private String2D strClearListValue;

		private String2D strSceneTitle;

		private String2D strScenePercentage;

		private String2D strEnding;

		private Picture picArrowBase;

		private Picture[] picArrows = new Picture[2];

		private Element eleScreen;

		private Element eleLeft;

		private Element eleRight;

		private CloseToScrollBarElement scrollBarScene = new CloseToScrollBarElement();

		private CloseToScrollBarElement scrollBarEnding = new CloseToScrollBarElement();

		private int LineNum = 14;

		private int currentScreen;

		private int lastScreen = -1;

		private string[] screens = new string[3] { "clearlist", "ending", "scenetitle" };

		private float centerBack;

		private int currentEndingStartId;

		private int currentSceneStartId;

		public override bool DataReady
		{
			get
			{
				if (base.DataReady)
				{
					return true;
				}
				if (picArrowBase != null && picArrowBase.IsCreated && picEndings[0] != null && picEndings[0].IsCreated && picSceneTitles[0] != null && picSceneTitles[0].IsCreated && picClearLists[0] != null && picClearLists[0].IsCreated && picEndings[1] != null && picEndings[1].IsCreated && picSceneTitles[1] != null && picSceneTitles[1].IsCreated && picClearLists[1] != null && picClearLists[1].IsCreated && picBack != null && picBack.IsCreated && scrollBarScene.DataReady && scrollBarEnding.DataReady && picTitle != null && picTitle.IsCreated)
				{
					picClearLists[0].Alpha = (int)(GameManager.GalCore.GameOptions.DialogueAlpha * 255f + 0.5f);
					picEndings[0].Alpha = (int)(GameManager.GalCore.GameOptions.DialogueAlpha * 255f + 0.5f);
					picSceneTitles[0].Alpha = (int)(GameManager.GalCore.GameOptions.DialogueAlpha * 255f + 0.5f);
					picArrows[0] = GameManager.Graphics.CreateSubPicture(picArrowBase, new Rectangle(0, 0, 40, 40));
					picArrows[1] = GameManager.Graphics.CreateSubPicture(picArrowBase, new Rectangle(40, 0, 40, 40));
					DataReady = true;
					return true;
				}
				return false;
			}
		}

		public override void InitData()
		{
			base.InitData();
			int paletteIndex;
			switch (GameManager.GalCore.GameOptions.DialogueColor)
			{
			case "Blue":
				paletteIndex = 1;
				break;
			case "Green":
				paletteIndex = 2;
				break;
			case "Pink":
				paletteIndex = 3;
				break;
			case "Gray":
				paletteIndex = 4;
				break;
			default:
				throw new Exception("Unbelievable!");
			}
			picClearLists[0] = GameManager.Data.LoadPicture("system", "clearlist0.png");
			picClearLists[1] = GameManager.Data.LoadPicture("system", "clearlist" + paletteIndex + ".png");
			picEndings[0] = GameManager.Data.LoadPicture("system", "ending0.png");
			picEndings[1] = GameManager.Data.LoadPicture("system", "ending" + paletteIndex + ".png");
			picSceneTitles[0] = GameManager.Data.LoadPicture("system", "scenetitle0.png");
			picSceneTitles[1] = GameManager.Data.LoadPicture("system", "scenetitle" + paletteIndex + ".png");
			picBack = GameManager.Data.LoadPicture("system", "screenback3.png");
			picArrowBase = GameManager.Data.LoadPicture("system", "arrow.png");
			picTitle = GameManager.Data.LoadPicture("system", "clearlist.png");
			Font2D font = GameManager.GalCore.GameFont.GetFont(24);
			Font2D font2 = GameManager.GalCore.GameFont.GetFont(20);
			strClearListItem = new String2D();
			strClearListItem.Font = font;
			strClearListItem.Location = Point.Empty;
			strClearListItem.LineSize = new Size(500, 55);
			strClearListItem.LineNum = 9;
			strClearListValue = new String2D();
			strClearListValue.Font = font;
			strClearListValue.Location = Point.Empty;
			strClearListValue.LineSize = new Size(200, 55);
			strClearListValue.LineNum = 9;
			strSceneTitle = new String2D();
			strSceneTitle.Font = font2;
			strSceneTitle.Location = Point.Empty;
			strSceneTitle.LineSize = new Size(550, 35);
			strSceneTitle.LineNum = LineNum;
			strScenePercentage = new String2D();
			strScenePercentage.Font = font2;
			strScenePercentage.Location = Point.Empty;
			strScenePercentage.LineSize = new Size(175, 35);
			strScenePercentage.LineNum = LineNum;
			strEnding = new String2D();
			strEnding.Font = font2;
			strEnding.Location = Point.Empty;
			strEnding.LineSize = new Size(725, 35);
			strEnding.LineNum = LineNum;
			scrollBarEnding.InitData();
			scrollBarScene.InitData();
		}

		public override void FinalData()
		{
			base.FinalData();
			for (int i = 0; i < 2; i++)
			{
				picArrows[i].Dispose();
			}
			picArrowBase.Dispose();
			for (int j = 0; j < 2; j++)
			{
				picClearLists[j].Dispose();
				picEndings[j].Dispose();
				picSceneTitles[j].Dispose();
			}
			picBack.Dispose();
			picTitle.Dispose();
			scrollBarEnding.FinalData();
			scrollBarScene.FinalData();
		}

		public override bool InitScreen()
		{
			if (!base.InitScreen())
			{
				return false;
			}
			eleScreen = new Element(picBack);
			RootElement = eleScreen;
			eleScreen.Add(new Element(picTitle), "title");
			Element element = new Element(picClearLists[0]);
			eleScreen.Add(element, "clearlist");
			element.Add(new Element(picClearLists[1]), "main0");
			element.Focused = true;
			FocusedElement = element;
			Element element2 = new Element(strClearListItem);
			element2.Position = new Point(50, 80);
			element.Add(element2, "item");
			Element element3 = new Element(strClearListValue);
			element3.Position = new Point(550, 80);
			element.Add(element3, "value");
			InitClearList();
			eleLeft = new Element(picArrows[0]);
			eleLeft.Position = new Point(0, 290);
			eleLeft.Region = new Rectangle(0, 0, 40, 40);
			eleRight = new Element(picArrows[1]);
			eleRight.Position = new Point(760, 290);
			eleRight.Region = new Rectangle(0, 0, 40, 40);
			element.Add(eleLeft, "left");
			element.Add(eleRight, "right");
			eleLeft.MouseClick += Left_MouseClick;
			eleLeft.MouseEnter += Left_MouseEnter;
			eleLeft.MouseLeave += Left_MouseLeave;
			eleRight.MouseClick += Right_MouseClick;
			eleRight.MouseEnter += Right_MouseEnter;
			eleRight.MouseLeave += Right_MouseLeave;
			Element element4 = new Element(picEndings[0]);
			eleScreen.Add(element4, "ending", false);
			element4.Add(new Element(picEndings[1]), "main0");
			Element element5 = new Element(strEnding);
			element5.Position = new Point(50, 75);
			element4.Add(element5, "item");
			scrollBarEnding.Init();
			scrollBarEnding.Position = new Point(755, 75);
			scrollBarEnding.Size = 490;
			element4.Add(scrollBarEnding, "scrollbar");
			scrollBarEnding.Length = (float)LineNum / (float)EndingTitles.Length;
			scrollBarEnding.ValueChanged += ScrollBarEnding_ValueChanged;
			UpdateEnding();
			Element element6 = new Element(picSceneTitles[0]);
			eleScreen.Add(element6, "scenetitle", false);
			element6.Add(new Element(picSceneTitles[1]), "main0");
			Element element7 = new Element(strSceneTitle);
			element7.Position = new Point(50, 75);
			element6.Add(element7, "item");
			Element element8 = new Element(strScenePercentage);
			element8.Position = new Point(550, 75);
			element6.Add(element8, "value");
			scrollBarScene.Init();
			scrollBarScene.Position = new Point(755, 75);
			scrollBarScene.Size = 490;
			element6.Add(scrollBarScene, "scrollbar");
			scrollBarScene.Length = (float)LineNum / (float)SceneTitles.Length;
			scrollBarScene.ValueChanged += ScrollBarScene_ValueChanged;
			UpdateSceneTitle();
			picBack.Alpha = 0;
			picBack.BeginTransform(0.5f);
			picBack.AlphaTo(255);
			picBack.EndTransform();
			RootElement.SetVisible("clearlist", false);
			RootElement.SetVisible("title", false);
			return true;
		}

		private void ScrollBarEnding_ValueChanged(object sender, EventArgs e)
		{
			currentEndingStartId = (int)(((ScrollBarElement)sender).Start * (float)EndingTitles.Length + 0.5f);
			if (currentEndingStartId < 0)
			{
				currentEndingStartId = 0;
			}
			if (currentEndingStartId > EndingTitles.Length - LineNum)
			{
				currentEndingStartId = EndingTitles.Length - LineNum;
				if (currentEndingStartId < 0)
				{
					currentEndingStartId = 0;
				}
			}
			UpdateEnding();
		}

		private void ScrollBarScene_ValueChanged(object sender, EventArgs e)
		{
			currentSceneStartId = (int)(((ScrollBarElement)sender).Start * (float)SceneTitles.Length + 0.5f);
			if (currentSceneStartId < 0)
			{
				currentSceneStartId = 0;
			}
			if (currentSceneStartId > SceneTitles.Length - LineNum)
			{
				currentSceneStartId = SceneTitles.Length - LineNum;
				if (currentSceneStartId < 0)
				{
					currentSceneStartId = 0;
				}
			}
			UpdateSceneTitle();
		}

		private void Left_MouseClick(object sender, EventArgs e)
		{
			MoveLeftScreen();
		}

		private void MoveLeftScreen()
		{
			if (lastScreen < 0 || !(eleScreen[screens[lastScreen]].ElementPicture.Time > 0f))
			{
				lastScreen = currentScreen;
				currentScreen = (currentScreen + 1) % 3;
				eleScreen[screens[lastScreen]].ClearSelected();
				eleScreen[screens[lastScreen]].Remove(eleLeft);
				eleScreen[screens[lastScreen]].Remove(eleRight);
				for (int i = 0; i < eleScreen[screens[lastScreen]].Count; i++)
				{
					eleScreen[screens[lastScreen]].SetVisible(i, false);
				}
				eleScreen[screens[lastScreen]].SetVisible("main0", true);
				eleScreen.SetVisible(screens[currentScreen], true);
				for (int j = 0; j < eleScreen[screens[currentScreen]].Count; j++)
				{
					eleScreen[screens[currentScreen]].SetVisible(j, false);
				}
				eleScreen[screens[currentScreen]].SetVisible("main0", true);
				eleScreen[screens[currentScreen]].Focused = true;
				FocusedElement = eleScreen[screens[currentScreen]];
				Picture elementPicture = eleScreen[screens[lastScreen]].ElementPicture;
				elementPicture.BeginTransform(0.5f);
				elementPicture.PositionTo(new Point(800, 0));
				elementPicture.EndTransform();
				Picture elementPicture2 = eleScreen[screens[lastScreen]]["main0"].ElementPicture;
				elementPicture2.BeginTransform(0.5f);
				elementPicture2.PositionTo(new Point(800, 0));
				elementPicture2.EndTransform();
				Picture elementPicture3 = eleScreen[screens[currentScreen]].ElementPicture;
				elementPicture3.Position = new Point(-800, 0);
				elementPicture3.BeginTransform(0.5f);
				elementPicture3.PositionTo(new Point(0, 0));
				elementPicture3.EndTransform();
				Picture elementPicture4 = eleScreen[screens[currentScreen]]["main0"].ElementPicture;
				elementPicture4.Position = new Point(-800, 0);
				elementPicture4.BeginTransform(0.5f);
				elementPicture4.PositionTo(new Point(0, 0));
				elementPicture4.EndTransform();
			}
		}

		private void Left_MouseEnter(object sender, EventArgs e)
		{
			picArrows[0].BeginTransform(0f);
			picArrows[0].PositionTo(new Point(-10, 0), new Point(0, 0), 0.5f);
			picArrows[0].EndTransform();
		}

		private void Left_MouseLeave(object sender, EventArgs e)
		{
			picArrows[0].BeginTransform(0.5f);
			picArrows[0].PositionTo(new Point(0, 0));
			picArrows[0].EndTransform();
		}

		private void Right_MouseClick(object sender, EventArgs e)
		{
			MoveRightScreen();
		}

		private void MoveRightScreen()
		{
			if (lastScreen < 0 || !(eleScreen[screens[lastScreen]].ElementPicture.Time > 0f))
			{
				lastScreen = currentScreen;
				currentScreen = (currentScreen + 2) % 3;
				eleScreen[screens[lastScreen]].ClearSelected();
				eleScreen[screens[lastScreen]].Remove(eleLeft);
				eleScreen[screens[lastScreen]].Remove(eleRight);
				for (int i = 0; i < eleScreen[screens[lastScreen]].Count; i++)
				{
					eleScreen[screens[lastScreen]].SetVisible(i, false);
				}
				eleScreen[screens[lastScreen]].SetVisible("main0", true);
				eleScreen.SetVisible(screens[currentScreen], true);
				for (int j = 0; j < eleScreen[screens[currentScreen]].Count; j++)
				{
					eleScreen[screens[currentScreen]].SetVisible(j, false);
				}
				eleScreen[screens[currentScreen]].SetVisible("main0", true);
				eleScreen[screens[currentScreen]].Focused = true;
				FocusedElement = eleScreen[screens[currentScreen]];
				Picture elementPicture = eleScreen[screens[lastScreen]].ElementPicture;
				elementPicture.BeginTransform(0.5f);
				elementPicture.PositionTo(new Point(-800, 0));
				elementPicture.EndTransform();
				Picture elementPicture2 = eleScreen[screens[lastScreen]]["main0"].ElementPicture;
				elementPicture2.BeginTransform(0.5f);
				elementPicture2.PositionTo(new Point(-800, 0));
				elementPicture2.EndTransform();
				Picture elementPicture3 = eleScreen[screens[currentScreen]].ElementPicture;
				elementPicture3.Position = new Point(800, 0);
				elementPicture3.BeginTransform(0.5f);
				elementPicture3.PositionTo(new Point(0, 0));
				elementPicture3.EndTransform();
				Picture elementPicture4 = eleScreen[screens[currentScreen]]["main0"].ElementPicture;
				elementPicture4.Position = new Point(800, 0);
				elementPicture4.BeginTransform(0.5f);
				elementPicture4.PositionTo(new Point(0, 0));
				elementPicture4.EndTransform();
			}
		}

		private void Right_MouseEnter(object sender, EventArgs e)
		{
			picArrows[1].BeginTransform(0f);
			picArrows[1].PositionTo(new Point(10, 0), new Point(0, 0), 0.5f);
			picArrows[1].EndTransform();
		}

		private void Right_MouseLeave(object sender, EventArgs e)
		{
			picArrows[1].BeginTransform(0.5f);
			picArrows[1].PositionTo(new Point(0, 0));
			picArrows[1].EndTransform();
		}

		protected void InitClearList()
		{
			strClearListItem.AddString(GameManager.GalCore.GameLanguage.ResourceManager.GetString("ClearListStr"));
			string empty = string.Empty;
			empty = empty + GameState.CountEndings() + " / 7\n";
			empty = empty + GameState.CountChapters() + " / 157\n";
			empty = empty + GameState.CountAlbums() + " / 136\n";
			empty = empty + GameState.CountMusics() + " / 33\n";
			empty = empty + GameState.CountMovies() + " / 14\n";
			empty = empty + ((float)GameState.CountStringsRead() / (float)GameState.CountStringsTotal()).ToString("P") + "\n";
			empty = empty + ((float)GameState.CountSelectsRead() / (float)GameState.CountSelectsTotal()).ToString("P") + "\n";
			empty += new TimeSpan((long)(int)GameState.GetTime() * 10000000L).ToString();
			strClearListValue.AddString(empty, ContentAlignment.TopRight);
		}

		public override bool FinalScreen()
		{
			if (!base.FinalScreen())
			{
				return false;
			}
			for (int i = 0; i < eleScreen["clearlist"].Count; i++)
			{
				eleScreen["clearlist"][i].Dispose();
			}
			scrollBarEnding.Final();
			for (int j = 0; j < eleScreen["ending"].Count; j++)
			{
				eleScreen["ending"][j].Dispose();
			}
			scrollBarScene.Final();
			for (int k = 0; k < eleScreen["scenetitle"].Count; k++)
			{
				eleScreen["scenetitle"][k].Dispose();
			}
			for (int l = 0; l < eleScreen.Count; l++)
			{
				eleScreen[l].Dispose();
			}
			eleScreen.Dispose();
			eleScreen = null;
			FinalData();
			Finished = true;
			FocusedElement = null;
			RootElement = null;
			return true;
		}

		public override void Update(float elaspedTime)
		{
			base.Update(elaspedTime);
			centerBack += elaspedTime * 80f;
			while (centerBack >= 80f)
			{
				centerBack -= 80f;
			}
			picBack.Center = new Point((int)(centerBack + 0.5f), (int)(centerBack + 0.5f));
			if (lastScreen >= 0 && eleScreen[screens[lastScreen]].ElementPicture.Time == 0f)
			{
				eleScreen.SetVisible(screens[lastScreen], false);
				lastScreen = -1;
				for (int i = 0; i < eleScreen[screens[currentScreen]].Count; i++)
				{
					eleScreen[screens[currentScreen]].SetVisible(i, true);
				}
				eleScreen[screens[currentScreen]].Add(eleLeft, "left");
				eleScreen[screens[currentScreen]].Add(eleRight, "right");
			}
			if (picBack.Time == 0f && !eleScreen.GetVisible(screens[currentScreen]))
			{
				if (picBack.Alpha == 255)
				{
					eleScreen.SetVisible(screens[currentScreen], true);
					eleScreen.SetVisible("title", true);
				}
				else
				{
					FinalScreen();
				}
			}
		}

		public override void OnKeyPress(Keys keyData)
		{
			if (keyData == Keys.Space)
			{
				if (eleScreen.GetVisible(screens[currentScreen]))
				{
					eleScreen.SetVisible(screens[currentScreen], false);
					eleScreen.SetVisible("title", false);
					picBack.BeginTransform(0.5f);
					picBack.AlphaTo(0);
					picBack.EndTransform();
				}
				GameManager.Data.LoadAndPlaySystemSound("cancel.ogg");
				return;
			}
			base.OnKeyPress(keyData);
			switch (keyData)
			{
			case Keys.Up:
				if (lastScreen != -1)
				{
					break;
				}
				if (currentScreen == 1)
				{
					if (currentEndingStartId > 0)
					{
						currentEndingStartId--;
					}
					UpdateEnding();
				}
				else if (currentScreen == 2)
				{
					if (currentSceneStartId > 0)
					{
						currentSceneStartId--;
					}
					UpdateSceneTitle();
				}
				break;
			case Keys.Down:
				if (lastScreen != -1)
				{
					break;
				}
				if (currentScreen == 1)
				{
					if (currentEndingStartId < EndingTitles.Length - LineNum)
					{
						currentEndingStartId++;
					}
					UpdateEnding();
				}
				else if (currentScreen == 2)
				{
					if (currentSceneStartId < SceneTitles.Length - LineNum)
					{
						currentSceneStartId++;
					}
					UpdateSceneTitle();
				}
				break;
			case Keys.Prior:
				if (lastScreen != -1)
				{
					break;
				}
				if (currentScreen == 1)
				{
					currentEndingStartId -= LineNum;
					if (currentEndingStartId < 0)
					{
						currentEndingStartId = 0;
					}
					UpdateEnding();
				}
				else if (currentScreen == 2)
				{
					currentSceneStartId -= LineNum;
					if (currentSceneStartId < 0)
					{
						currentSceneStartId = 0;
					}
					UpdateSceneTitle();
				}
				break;
			case Keys.Next:
				if (lastScreen != -1)
				{
					break;
				}
				if (currentScreen == 1)
				{
					currentEndingStartId += LineNum;
					if (currentEndingStartId > EndingTitles.Length - LineNum)
					{
						currentEndingStartId = EndingTitles.Length - LineNum;
						if (currentEndingStartId < 0)
						{
							currentEndingStartId = 0;
						}
					}
					UpdateEnding();
				}
				else
				{
					if (currentScreen != 2)
					{
						break;
					}
					currentSceneStartId += LineNum;
					if (currentSceneStartId > SceneTitles.Length - LineNum)
					{
						currentSceneStartId = SceneTitles.Length - LineNum;
						if (currentSceneStartId < 0)
						{
							currentSceneStartId = 0;
						}
					}
					UpdateSceneTitle();
				}
				break;
			case Keys.Left:
				MoveLeftScreen();
				break;
			case Keys.Right:
				MoveRightScreen();
				break;
			case Keys.End:
			case Keys.Home:
				break;
			}
		}

		public override void OnMouseWheel(int delta)
		{
			base.OnMouseWheel(delta);
			if (lastScreen != -1)
			{
				return;
			}
			if (currentScreen == 1)
			{
				if (delta > 0)
				{
					currentEndingStartId -= delta / 120;
					if (currentEndingStartId < 0)
					{
						currentEndingStartId = 0;
					}
					UpdateEnding();
				}
				else
				{
					if (delta >= 0)
					{
						return;
					}
					currentEndingStartId -= delta / 120;
					if (currentEndingStartId > EndingTitles.Length - LineNum)
					{
						currentEndingStartId = EndingTitles.Length - LineNum;
						if (currentEndingStartId < 0)
						{
							currentEndingStartId = 0;
						}
					}
					UpdateEnding();
				}
			}
			else
			{
				if (currentScreen != 2)
				{
					return;
				}
				if (delta > 0)
				{
					currentSceneStartId -= delta / 120;
					if (currentSceneStartId < 0)
					{
						currentSceneStartId = 0;
					}
					UpdateSceneTitle();
				}
				else
				{
					if (delta >= 0)
					{
						return;
					}
					currentSceneStartId -= delta / 120;
					if (currentSceneStartId > SceneTitles.Length - LineNum)
					{
						currentSceneStartId = SceneTitles.Length - LineNum;
						if (currentSceneStartId < 0)
						{
							currentSceneStartId = 0;
						}
					}
					UpdateSceneTitle();
				}
			}
		}

		protected void UpdateEnding()
		{
			string line = string.Empty;
			for (int i = 0; i < LineNum; i++)
			{
				int paletteIndex = i + currentEndingStartId;
				if (paletteIndex == EndingTitles.Length)
				{
					break;
				}
				line = line + (paletteIndex + 1) + ". ";
				if (EndingOpens[paletteIndex])
				{
					line = line + EndingTitles[paletteIndex] + "\n";
					continue;
				}
				for (int j = 0; j < EndingTitles[paletteIndex].Length; j++)
				{
					line += "?";
				}
				line += "\n";
			}
			strEnding.ClearString();
			strEnding.AddString(line);
			scrollBarEnding.Start = (float)currentEndingStartId / (float)EndingTitles.Length;
		}

		protected void UpdateSceneTitle()
		{
			string line = string.Empty;
			string detail = string.Empty;
			for (int i = 0; i < LineNum; i++)
			{
				int paletteIndex = i + currentSceneStartId;
				if (paletteIndex == SceneTitles.Length)
				{
					break;
				}
				line = line + (paletteIndex + 1).ToString("000") + ". ";
				if (SceneOpens[paletteIndex])
				{
					line = line + SceneTitles[paletteIndex] + "\n";
					detail = detail + Percentages[paletteIndex].ToString("P") + "\n";
					continue;
				}
				for (int j = 0; j < SceneTitles[paletteIndex].Length; j++)
				{
					line += "?";
				}
				line += "\n";
				detail += "\n";
			}
			strSceneTitle.ClearString();
			strSceneTitle.AddString(line);
			strScenePercentage.ClearString();
			strScenePercentage.AddString(detail, ContentAlignment.TopRight);
			scrollBarScene.Start = (float)currentSceneStartId / (float)SceneTitles.Length;
		}
	}
}
