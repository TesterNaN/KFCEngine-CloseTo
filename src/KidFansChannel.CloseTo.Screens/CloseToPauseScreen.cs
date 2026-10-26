using System;
using System.Drawing;
using System.Windows.Forms;
using KidFansChannel.GalCore;
using KidFansChannel.GalCore.Screens;
using KidFansChannel.Game2DCore;

namespace KidFansChannel.CloseTo.Screens
{
	public class CloseToPauseScreen : PauseScreen
	{
		private String2D strItem;

		private String2D strValue;

		private Picture[] picMains = new Picture[5];

		private Picture picBack;

		private String2D[] strMenuItem = new String2D[8];

		private Element eleScreen;

		private static Picture selectBar;

		private static Picture iconBase;

		private static SubPicture[] icons = new SubPicture[5];

		private static AnimePicture[] iconAnimes = new AnimePicture[2];

		private int CurrentSelected;

		protected int lastSelected = -1;

		protected float iconTime;

		protected int iconPasses;

		protected bool iconDirection;

		protected float iconX1;

		protected float iconX2;

		public override bool DataReady
		{
			get
			{
				if (base.DataReady)
				{
					return true;
				}
				if (picMains[0] != null && picMains[0].IsCreated && picMains[1] != null && picMains[1].IsCreated && picMains[2] != null && picMains[2].IsCreated && picMains[3] != null && picMains[3].IsCreated && picMains[4] != null && picMains[4].IsCreated && picBack != null && picBack.IsCreated && selectBar != null && selectBar.IsCreated && iconBase != null && iconBase.IsCreated)
				{
					selectBar.Center = new Point(0, 20);
					for (int i = 0; i < 5; i++)
					{
						icons[i] = GameManager.Graphics.CreateSubPicture(iconBase, new Rectangle(i % 3 * 40, i / 3 * 40, 40, 40));
					}
					for (int j = 0; j < 2; j++)
					{
						iconAnimes[j] = GameManager.Graphics.CreateAnimePicture(new Size(40, 40));
						for (int k = 0; k < 5; k++)
						{
							iconAnimes[j].AddFrame(icons[k], 1f / 12f);
						}
						iconAnimes[j].Center = new Point(20, 20);
					}
					DataReady = true;
					return true;
				}
				return false;
			}
		}

		public override void InitData()
		{
			base.InitData();
			for (int i = 0; i < 5; i++)
			{
				picMains[i] = GameManager.Data.LoadPicture("system", "pause" + i + ".png");
			}
			picBack = GameManager.Graphics.CreatePicture(Color.FromArgb(128, Color.Black));
			Font2D font = GameManager.GalCore.GameFont.GetFont(24);
			Font2D font2 = GameManager.GalCore.GameFont.GetFont(20);
			strItem = new String2D();
			strItem.Font = font2;
			strItem.Location = new Point(250, 300);
			strItem.LineSize = new Size(200, 36);
			strItem.LineNum = 7;
			strValue = new String2D();
			strValue.Font = font2;
			strValue.Location = new Point(450, 300);
			strValue.LineSize = new Size(350, 36);
			strValue.LineNum = 7;
			for (int j = 0; j < 8; j++)
			{
				strMenuItem[j] = new String2D();
				strMenuItem[j].Font = font;
				strMenuItem[j].Location = new Point(30, 0);
				strMenuItem[j].LineSize = new Size(190, 40);
				strMenuItem[j].LineNum = 1;
			}
			string[] menuTexts = GameManager.GalCore.GameLanguage.ResourceManager.GetString("MenuItemStr").Split('\n');
			for (int k = 0; k < 8; k++)
			{
				strMenuItem[k].AddString(menuTexts[k]);
			}
			if (!GameManager.GameState.CurrentGameState.CanSave)
			{
				strMenuItem[0].DefaultColor = Color.FromArgb(96, 96, 96);
				strMenuItem[2].DefaultColor = Color.FromArgb(96, 96, 96);
			}
			selectBar = GameManager.Data.LoadPicture("system", "select_bar.png");
			iconBase = GameManager.Data.LoadPicture("system", "click.png");
		}

		public override void FinalData()
		{
			base.FinalData();
			for (int i = 0; i < 5; i++)
			{
				picMains[i].Dispose();
			}
			picBack.Dispose();
			selectBar.Dispose();
			for (int j = 0; j < 2; j++)
			{
				iconAnimes[j].Dispose();
			}
			for (int k = 0; k < 5; k++)
			{
				icons[k].Dispose();
			}
			iconBase.Dispose();
		}

		public override bool InitScreen()
		{
			if (!base.InitScreen())
			{
				return false;
			}
			Element element = new Element();
			string[] menuTexts = new string[4] { "Blue", "Green", "Pink", "Gray" };
			for (int i = 0; i < 5; i++)
			{
				Element item = new Element(picMains[i]);
				element.Add(item, "sub" + i);
				if (i > 0)
				{
					element.SetVisible("sub" + i, GameManager.GalCore.GameOptions.DialogueColor == menuTexts[i - 1]);
				}
			}
			picMains[0].Alpha = (int)(GameManager.GalCore.GameOptions.DialogueAlpha * 255f + 0.5f);
			eleScreen = new Element(picBack);
			element.Position = new Point(10, 25);
			eleScreen.Add(element, "main");
			eleScreen.Add(new Element(strItem), "item");
			eleScreen.Add(new Element(strValue), "value");
			for (int j = 0; j < 8; j++)
			{
				Element element2 = new Element(strMenuItem[j]);
				element2.Position = new Point(5, 20 + j * 40);
				element2.Value = j;
				element2.Region = new Rectangle(0, 0, 250, 40);
				element.Add(element2, "menuitem" + j.ToString("00"));
			}
			Element element3 = new Element(selectBar);
			element3.Position = new Point(-5, 20);
			element.Add(element3, "bar");
			Element element4 = new Element(iconAnimes[0]);
			element4.Position = new Point(20, 20);
			element.Add(element4, "icon0");
			Element element5 = new Element(iconAnimes[1]);
			element5.Position = new Point(20, 20);
			element.Add(element5, "icon1");
			element.SetVisible("bar", false);
			element.SetVisible("icon0", false);
			element.SetVisible("icon1", false);
			element.Focused = true;
			element.ChildDown = element["menuitem00"];
			element.ChildUp = element["menuitem07"];
			FocusedElement = element;
			RootElement = eleScreen;
			string value = GameManager.GalCore.GameLanguage.ResourceManager.GetString("StateStr");
			string value2 = ((Item.Chapter == null) ? "------" : Item.Chapter) + "\n" + ((Item.BgName == null) ? "------" : Item.BgName) + "\n" + ((Item.BgmName == null) ? "------" : Item.BgmName) + "\n" + Item.ReadPercentage.ToString("P") + "\n" + Item.GameDateTime + "\n" + new TimeSpan((long)Item.CurrentTime * 10000000L).ToString() + "\n" + new TimeSpan((long)Item.TotalTime * 10000000L).ToString();
			strItem.AddString(value, ContentAlignment.TopRight);
			strValue.AddString(value2, ContentAlignment.TopLeft);
			for (int k = 0; k < 7; k++)
			{
				element["menuitem" + k.ToString("00")].Down = element["menuitem" + (k + 1).ToString("00")];
				element["menuitem" + (k + 1).ToString("00")].Up = element["menuitem" + k.ToString("00")];
			}
			for (int l = 0; l < 8; l++)
			{
				element["menuitem" + l.ToString("00")].MouseClick += MenuItem_MouseClick;
				element["menuitem" + l.ToString("00")].MouseEnter += MenuItem_MouseEnter;
				element["menuitem" + l.ToString("00")].MouseLeave += MenuItem_MouseLeave;
			}
			CurrentSelected = -1;
			GameManager.Data.LoadAndPlaySystemSound("cancel.ogg");
			return true;
		}

		private void MenuItem_MouseClick(object sender, EventArgs e)
		{
			Element element = (Element)sender;
			switch ((int)element.Value)
			{
			case 0:
				if (GameManager.GameState.CurrentGameState.CanSave)
				{
					GameManager.GalCore.ShowSaveLoadScreen(true);
				}
				break;
			case 1:
				GameManager.GalCore.ShowSaveLoadScreen(false);
				break;
			case 2:
				if (GameManager.GameState.CurrentGameState.CanSave)
				{
					GameManager.GameScript.QuickSave(QuickSaveType.Quick);
				}
				FinalScreen();
				break;
			case 3:
				GameManager.GameScript.QuickLoad();
				FinalScreen();
				break;
			case 4:
				GameManager.GalCore.ShowOptionsScreen();
				break;
			case 5:
				GameManager.GalCore.ShowMessageScreen();
				break;
			case 6:
				GameManager.GalCore.ShowMessageBox(new CloseToMessageBoxScreen(GameManager.GalCore.GameLanguage.ResourceManager.GetString("ExitStr"), MessageBoxButtons.YesNo, CloseToExitGame, null));
				break;
			case 7:
				FinalScreen();
				break;
			}
			GameManager.Data.LoadAndPlaySystemSound("ok.ogg");
		}

		private void CloseToExitGame(object[] empty)
		{
			ExitGame();
		}

		private void MenuItem_MouseEnter(object sender, EventArgs e)
		{
			Element element = (Element)sender;
			CurrentSelected = (int)element.Value;
			eleScreen["main"].SetVisible("bar", true);
			eleScreen["main"].SetVisible("icon0", true);
			eleScreen["main"].SetVisible("icon1", true);
			GameManager.Data.LoadAndPlaySystemSound("cursor.ogg");
		}

		private void MenuItem_MouseLeave(object sender, EventArgs e)
		{
			Element element = (Element)sender;
			CurrentSelected = -1;
			eleScreen["main"].SetVisible("bar", false);
			eleScreen["main"].SetVisible("icon0", false);
			eleScreen["main"].SetVisible("icon1", false);
		}

		public override bool FinalScreen()
		{
			if (!base.FinalScreen())
			{
				return false;
			}
			for (int i = 0; i < eleScreen["main"].Count; i++)
			{
				eleScreen["main"][i].Dispose();
			}
			for (int j = 0; j < eleScreen.Count; j++)
			{
				eleScreen[j].Dispose();
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
			for (int i = 0; i < 2; i++)
			{
				iconAnimes[i].Update(elaspedTime);
			}
			if (lastSelected != CurrentSelected)
			{
				if (CurrentSelected != -1)
				{
					if (lastSelected == -1)
					{
						selectBar.Scale = new SizeF((float)(strMenuItem[CurrentSelected].Width + 80) / (float)selectBar.Width, 1f);
						selectBar.Position = new Point(0, CurrentSelected * 40 + 20);
						selectBar.Alpha = 0;
						selectBar.BeginTransform(0.1f);
						selectBar.AlphaTo(255);
						selectBar.EndTransform();
						iconTime = 0.5f;
						iconPasses = 1;
						iconDirection = true;
						for (int j = 0; j < 2; j++)
						{
							iconAnimes[j].Position = new Point(0, CurrentSelected * 40 + 20);
						}
						iconX1 = 0f;
						iconX2 = strMenuItem[CurrentSelected].Width + 30;
					}
					else
					{
						selectBar.BeginTransform(0.1f);
						selectBar.ScaleTo(new SizeF((float)(strMenuItem[CurrentSelected].Width + 80) / (float)selectBar.Width, 1f));
						selectBar.PositionTo(new Point(0, CurrentSelected * 40 + 20));
						selectBar.EndTransform();
						if (iconPasses == 0 && iconTime == 0f)
						{
							iconTime = 0.5f;
							iconPasses = 1;
							iconDirection = true;
							if (iconX1 > iconX2)
							{
								float iconX = iconX1;
								iconX1 = iconX2;
								iconX2 = iconX;
							}
						}
						else if (iconPasses == 0)
						{
							iconPasses = 1;
						}
						for (int k = 0; k < 2; k++)
						{
							iconAnimes[k].BeginTransform(iconTime);
							iconAnimes[k].PositionTo(new Point(0, CurrentSelected * 40 + 20));
							iconAnimes[k].EndTransform();
						}
					}
				}
				else
				{
					iconTime = 0f;
					iconPasses = 0;
					iconDirection = false;
					iconX1 = 0f;
					iconX2 = strMenuItem[lastSelected].Width + 30;
				}
				lastSelected = CurrentSelected;
			}
			if (iconPasses >= 0 && iconTime > 0f)
			{
				iconTime -= elaspedTime;
				bool selected = false;
				while (iconTime < 0f)
				{
					selected = true;
					iconTime += 0.5f;
					iconPasses--;
					iconDirection = !iconDirection;
				}
				if (iconPasses < 0)
				{
					iconTime = 0f;
					iconPasses = 0;
					if (iconDirection)
					{
						iconX1 = strMenuItem[CurrentSelected].Width + 30;
						iconX2 = 0f;
					}
					else
					{
						iconX1 = 0f;
						iconX2 = strMenuItem[CurrentSelected].Width + 30;
					}
				}
				else if (selected)
				{
					iconX1 = (iconX2 = (float)(strMenuItem[CurrentSelected].Width + 30) * (0.5f - iconTime) / 0.5f);
					if (iconDirection)
					{
						iconX2 = (float)(strMenuItem[CurrentSelected].Width + 30) - iconX2;
					}
					else
					{
						iconX1 = (float)(strMenuItem[CurrentSelected].Width + 30) - iconX1;
					}
				}
				else
				{
					iconX1 += ((float)(iconDirection ? (strMenuItem[CurrentSelected].Width + 30) : 0) - iconX1) * elaspedTime / (iconTime + elaspedTime);
					iconX2 += ((float)((!iconDirection) ? (strMenuItem[CurrentSelected].Width + 30) : 0) - iconX2) * elaspedTime / (iconTime + elaspedTime);
				}
				eleScreen["main"]["icon0"].Position = new Point((int)(20f + iconX1 + 0.5f), 20);
				eleScreen["main"]["icon1"].Position = new Point((int)(20f + iconX2 + 0.5f), 20);
			}
			selectBar.Update(elaspedTime);
		}
	}
}
