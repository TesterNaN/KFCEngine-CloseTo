using System;
using System.Drawing;
using System.Windows.Forms;
using KidFansChannel.GalCore;
using KidFansChannel.GalCore.Screens;
using KidFansChannel.Game2DCore;

namespace KidFansChannel.CloseTo.Screens
{
	public class CloseToSaveLoadScreen : SaveLoadScreen
	{
		private Picture picSLIcon;

		private Picture picSLNoneIcon;

		private Picture[] picSLIcons = new Picture[7];

		private Picture picSLSelectIcon;

		private bool isSave;

		private bool back;

		private Picture[] picMains = new Picture[2];

		private Picture picBack;

		private Picture picBlack;

		private String2D strLevel;

		private String2D strIndex;

		private String2D strTime;

		private String2D strComment;

		private Element eleScreen;

		private float centerBack;

		public override bool DataReady
		{
			get
			{
				if (base.DataReady)
				{
					return true;
				}
				if (picMains[0] != null && picMains[0].IsCreated && picMains[1] != null && picMains[1].IsCreated && picBack != null && picBack.IsCreated && picSLIcon != null && picSLIcon.IsCreated && picBlack != null && picBlack.IsCreated)
				{
					picBlack.Size = new Size(100, 100);
					picBlack.Center = new Point(50, 50);
					picMains[0].Alpha = (int)(GameManager.GalCore.GameOptions.DialogueAlpha * 255f + 0.5f);
					picSLNoneIcon = GameManager.Graphics.CreateSubPicture(picSLIcon, new Rectangle(0, 50, 50, 50));
					for (int i = 0; i < 4; i++)
					{
						picSLIcons[i] = GameManager.Graphics.CreateSubPicture(picSLIcon, new Rectangle(50 * i, 0, 50, 50));
					}
					for (int j = 0; j < 3; j++)
					{
						picSLIcons[j + 4] = GameManager.Graphics.CreateSubPicture(picSLIcon, new Rectangle(50 * (j + 1), 50, 50, 50));
					}
					picSLSelectIcon = GameManager.Graphics.CreateSubPicture(picSLIcon, new Rectangle(200, 50, 50, 50));
					DataReady = true;
					return true;
				}
				return false;
			}
		}

		public override bool IsSave
		{
			get
			{
				return isSave;
			}
		}

		public override bool CanDrop
		{
			get
			{
				return picBack.Alpha == 255;
			}
		}

		public bool Back
		{
			get
			{
				return back;
			}
		}

		public CloseToSaveLoadScreen(bool isSave, bool back)
		{
			this.isSave = isSave;
			this.back = back;
		}

		public override void InitData()
		{
			base.InitData();
			picSLIcon = GameManager.Data.LoadPicture("system", "sl_icon.png");
			picMains[0] = GameManager.Data.LoadPicture("system", (isSave ? "save" : "load") + "0.png");
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
			picMains[1] = GameManager.Data.LoadPicture("system", (isSave ? "save" : "load") + paletteIndex + ".png");
			if (back)
			{
				picBack = GameManager.Data.LoadPicture("system", "screenback" + (isSave ? "1" : "0") + ".png");
			}
			else
			{
				picBack = GameManager.Graphics.CreatePicture(Color.FromArgb(128, Color.Black));
			}
			picBlack = GameManager.Graphics.CreatePicture(Color.FromArgb(128, Color.Black));
			Font2D font = GameManager.GalCore.GameFont.GetFont(20);
			strLevel = new String2D();
			strLevel.Font = font;
			strLevel.Location = Point.Empty;
			strLevel.LineSize = new Size(250, 40);
			strLevel.LineNum = 1;
			strIndex = new String2D();
			strIndex.Font = font;
			strIndex.Location = Point.Empty;
			strIndex.LineSize = new Size(45, 40);
			strIndex.LineNum = 1;
			strTime = new String2D();
			strTime.Font = font;
			strTime.Location = Point.Empty;
			strTime.LineSize = new Size(200, 40);
			strTime.LineNum = 1;
			strComment = new String2D();
			strComment.Font = font;
			strComment.Location = Point.Empty;
			strComment.LineSize = new Size(500, 35);
			strComment.LineNum = 4;
		}

		public override void FinalData()
		{
			base.FinalData();
			picSLNoneIcon.Dispose();
			picSLNoneIcon = null;
			for (int i = 0; i < 7; i++)
			{
				picSLIcons[i].Dispose();
				picSLIcons[i] = null;
			}
			picSLSelectIcon.Dispose();
			picSLSelectIcon = null;
			picSLIcon.Dispose();
			picSLIcon = null;
			for (int j = 0; j < 2; j++)
			{
				picMains[j].Dispose();
			}
			if (picSnap != null)
			{
				picSnap.Dispose();
				picSnap = null;
			}
			picBack.Dispose();
			picBack = null;
			picBlack.Dispose();
			picBlack = null;
		}

		public override bool InitScreen()
		{
			if (!base.InitScreen())
			{
				return false;
			}
			Element item = new Element(picSLSelectIcon);
			Element[] icons = new Element[7];
			for (int i = 0; i < 7; i++)
			{
				icons[i] = new Element(picSLIcons[i]);
			}
			Element item2 = new Element(picSLNoneIcon);
			Element element = new Element();
			element.Add(item, "select", false);
			element.Add(item2, "none", false);
			for (int j = 0; j < 7; j++)
			{
				element.Add(icons[j], "icon" + j, false);
			}
			element.Region = new Rectangle(0, 0, 50, 50);
			Element element2 = new Element(picMains[0]);
			eleScreen = new Element(picBack);
			eleScreen.Add(element2, "main");
			element2.Add(new Element(picMains[1]), "main0");
			for (int k = 0; k < 60; k++)
			{
				Element element3 = (Element)element.Clone();
				element3.Position = new Point(25 + k % 10 * 50, 70 + k / 10 * 50);
				element3.Value = Icons[k];
				if ((int)element3.Value != -1)
				{
					element3.SetVisible("icon" + (int)element3.Value, true);
				}
				else
				{
					element3.SetVisible("none", true);
				}
				eleScreen["main"].Add(element3, "sl" + k.ToString("00"));
			}
			for (int l = 0; l < 30; l++)
			{
				Element element4 = (Element)element.Clone();
				element4.Position = new Point(525 + l % 5 * 50, 70 + l / 5 * 50);
				int slotIndex = l;
				slotIndex = ((GameManager.GalCore.GameOptions.QuickSaveIndex != -1) ? (slotIndex + GameManager.GalCore.GameOptions.QuickSaveIndex) : (slotIndex + GameManager.GameScript.QuickIndexStart));
				if (slotIndex >= GameManager.GameScript.QuickIndexEnd)
				{
					slotIndex -= GameManager.GameScript.QuickIndexEnd - GameManager.GameScript.QuickIndexStart;
				}
				element4.Value = Icons[slotIndex];
				if ((int)element4.Value != -1)
				{
					element4.SetVisible("icon" + (int)element4.Value, true);
				}
				else
				{
					element4.SetVisible("none", true);
				}
				eleScreen["main"].Add(element4, "slquick" + l.ToString("00"));
			}
			element.Dispose();
			eleScreen["main"].Focused = true;
			FocusedElement = eleScreen["main"];
			RootElement = eleScreen;
			for (int m = 0; m < 10; m++)
			{
				for (int n = 0; n < 6; n++)
				{
					eleScreen["main"]["sl" + (m + n * 10).ToString("00")].Left = eleScreen["main"]["sl" + ((m + 9) % 10 + n * 10).ToString("00")];
					eleScreen["main"]["sl" + (m + n * 10).ToString("00")].Right = eleScreen["main"]["sl" + ((m + 1) % 10 + n * 10).ToString("00")];
					eleScreen["main"]["sl" + (m + n * 10).ToString("00")].Up = eleScreen["main"]["sl" + (m + (n + 5) % 6 * 10).ToString("00")];
					eleScreen["main"]["sl" + (m + n * 10).ToString("00")].Down = eleScreen["main"]["sl" + (m + (n + 1) % 6 * 10).ToString("00")];
				}
			}
			if (!isSave)
			{
				for (int col = 0; col < 5; col++)
				{
					for (int row = 0; row < 6; row++)
					{
						eleScreen["main"]["slquick" + (col + row * 5).ToString("00")].Left = eleScreen["main"]["slquick" + ((col + 4) % 5 + row * 5).ToString("00")];
						eleScreen["main"]["slquick" + (col + row * 5).ToString("00")].Right = eleScreen["main"]["slquick" + ((col + 1) % 5 + row * 5).ToString("00")];
						eleScreen["main"]["slquick" + (col + row * 5).ToString("00")].Up = eleScreen["main"]["slquick" + (col + (row + 5) % 6 * 5).ToString("00")];
						eleScreen["main"]["slquick" + (col + row * 5).ToString("00")].Down = eleScreen["main"]["slquick" + (col + (row + 1) % 6 * 5).ToString("00")];
					}
				}
				for (int row = 0; row < 6; row++)
				{
					eleScreen["main"]["sl" + (row * 10).ToString("00")].Left = eleScreen["main"]["slquick" + (4 + row * 5).ToString("00")];
					eleScreen["main"]["slquick" + (row * 5).ToString("00")].Left = eleScreen["main"]["sl" + (9 + row * 10).ToString("00")];
					eleScreen["main"]["slquick" + (4 + row * 5).ToString("00")].Right = eleScreen["main"]["sl" + (row * 10).ToString("00")];
					eleScreen["main"]["sl" + (9 + row * 10).ToString("00")].Right = eleScreen["main"]["slquick" + (row * 5).ToString("00")];
				}
				Element element5 = eleScreen["main"];
				Element childLeft = (eleScreen["main"].ChildUp = eleScreen["main"]["slquick" + (GameManager.GameScript.QuickIndexEnd - GameManager.GameScript.QuickIndexStart - 1).ToString("00")]);
				element5.ChildLeft = childLeft;
			}
			else
			{
				Element element7 = eleScreen["main"];
				Element childLeft2 = (eleScreen["main"].ChildUp = eleScreen["main"]["sl" + (GameManager.GameScript.QuickIndexStart - 1).ToString("00")]);
				element7.ChildLeft = childLeft2;
			}
			Element element9 = eleScreen["main"];
			Element childRight = (eleScreen["main"].ChildDown = eleScreen["main"]["sl00"]);
			element9.ChildRight = childRight;
			for (int slot = 0; slot < 60; slot++)
			{
				eleScreen["main"]["sl" + slot.ToString("00")].MouseClick += SaveLoadUnit_MouseClick;
				eleScreen["main"]["sl" + slot.ToString("00")].MouseEnter += SaveLoadUnit_MouseEnter;
				eleScreen["main"]["sl" + slot.ToString("00")].MouseLeave += SaveLoadUnit_MouseLeave;
			}
			if (!isSave)
			{
				for (int quickSlot = 0; quickSlot < 30; quickSlot++)
				{
					eleScreen["main"]["slquick" + quickSlot.ToString("00")].MouseClick += SaveLoadUnit_MouseClick;
					eleScreen["main"]["slquick" + quickSlot.ToString("00")].MouseEnter += SaveLoadUnit_MouseEnter;
					eleScreen["main"]["slquick" + quickSlot.ToString("00")].MouseLeave += SaveLoadUnit_MouseLeave;
				}
			}
			Element element11 = new Element();
			element11.Position = new Point(522, 347);
			eleScreen["main"].Add(element11, "snap");
			Element element12 = new Element(strLevel);
			element12.Position = new Point(75, 385);
			eleScreen["main"].Add(element12, "level");
			Element element13 = new Element(strIndex);
			element13.Position = new Point(30, 388);
			eleScreen["main"].Add(element13, "index");
			Element element14 = new Element(strTime);
			element14.Position = new Point(325, 388);
			eleScreen["main"].Add(element14, "time");
			Element element15 = new Element(strComment);
			element15.Position = new Point(30, 425);
			eleScreen["main"].Add(element15, "comment");
			Element element16 = new Element(picBlack);
			for (int iconIndex = 0; iconIndex < 4; iconIndex++)
			{
				Element element17 = new Element();
				element17.Add(item, "select", false);
				element17.Add(icons[iconIndex], "icon", true);
				element17.Region = new Rectangle(0, 0, 50, 50);
				element17.Position = new Point(iconIndex % 2 * 50 - 50, iconIndex / 2 * 50 - 50);
				element17.Value = iconIndex;
				element16.Add(element17, "icon" + iconIndex);
				element16["icon" + iconIndex].MouseEnter += SaveIcon_MouseEnter;
				element16["icon" + iconIndex].MouseLeave += SaveIcon_MouseLeave;
				element16["icon" + iconIndex].MouseClick += SaveIcon_MouseClick;
			}
			for (int iconIndex = 0; iconIndex < 4; iconIndex++)
			{
				Element element18 = element16["icon" + iconIndex];
				Element up = (element16["icon" + iconIndex].Up = element16["icon" + (iconIndex + 2) % 4]);
				element18.Up = up;
				Element element20 = element16["icon" + iconIndex];
				Element childLeft = (element16["icon" + iconIndex].Right = element16["icon" + ((iconIndex + 1) % 2 + iconIndex / 2 * 2)]);
				element20.Left = childLeft;
			}
			element16.ChildLeft = element16["icon3"];
			element16.ChildRight = element16["icon0"];
			element16.ChildUp = element16["icon2"];
			element16.ChildDown = element16["icon1"];
			element16.Region = new Rectangle(-50, -50, 100, 100);
			eleScreen["main"].Add(element16, "iconpanel");
			eleScreen["main"].SetVisible("iconpanel", false);
			if (back)
			{
				picBack.Alpha = 0;
				picBack.BeginTransform(0.5f);
				picBack.AlphaTo(255);
				picBack.EndTransform();
				RootElement.SetVisible("main", false);
			}
			return true;
		}

		private void SaveIcon_MouseClick(object sender, EventArgs e)
		{
			Element element = (Element)sender;
			int slotIndex = (int)eleScreen["main"]["iconpanel"].Value;
			int iconIndex = (int)element.Value;
			eleScreen["main"]["iconpanel"].Value = null;
			eleScreen["main"].SetVisible("iconpanel", false);
			eleScreen["main"]["iconpanel"].Focused = false;
			eleScreen["main"].Focused = true;
			FocusedElement = eleScreen["main"];
			if (Icons[slotIndex] >= 0)
			{
				GameManager.GalCore.ShowMessageBox(new CloseToMessageBoxScreen(GameManager.GalCore.GameLanguage.ResourceManager.GetString("SaveStr"), MessageBoxButtons.YesNo, CloseToSave, new object[2] { slotIndex, iconIndex }));
			}
			else
			{
				CloseToSave(new object[2] { slotIndex, iconIndex });
				GameManager.Data.LoadAndPlaySystemSound("ok.ogg");
			}
		}

		private void SaveLoadUnit_MouseClick(object sender, EventArgs e)
		{
			Element element = (Element)sender;
			if (!isSave && (int)element.Value == -1)
			{
				return;
			}
			string text = eleScreen["main"].NameOf(element);
			int slotIndex;
			if (text.Length == 4)
			{
				slotIndex = int.Parse(text.Substring(2));
			}
			else
			{
				slotIndex = int.Parse(text.Substring(7));
				slotIndex = ((GameManager.GalCore.GameOptions.QuickSaveIndex != -1) ? (slotIndex + GameManager.GalCore.GameOptions.QuickSaveIndex) : (slotIndex + GameManager.GameScript.QuickIndexStart));
				if (slotIndex >= GameManager.GameScript.QuickIndexEnd)
				{
					slotIndex -= GameManager.GameScript.QuickIndexEnd - GameManager.GameScript.QuickIndexStart;
				}
			}
			if (isSave)
			{
				eleScreen["main"].SetVisible("iconpanel", true);
				eleScreen["main"]["iconpanel"].Position = new Point(element.Position.X + 25, element.Position.Y + 25);
				eleScreen["main"]["iconpanel"].Focused = true;
				eleScreen["main"].Focused = false;
				FocusedElement = eleScreen["main"]["iconpanel"];
				eleScreen["main"]["iconpanel"].Value = slotIndex;
				GameManager.Data.LoadAndPlaySystemSound("ok.ogg");
			}
			else
			{
				GameManager.GalCore.ShowMessageBox(new CloseToMessageBoxScreen(GameManager.GalCore.GameLanguage.ResourceManager.GetString("LoadStr"), MessageBoxButtons.YesNo, CloseToLoad, new object[1] { slotIndex }));
			}
		}

		private void CloseToSave(object[] indexAndIcon)
		{
			Save((int)indexAndIcon[0], null, (int)indexAndIcon[1]);
		}

		private void CloseToLoad(object[] index)
		{
			Load((int)index[0]);
			if (back)
			{
				GameManager.GalCore.HideTitleScreen();
			}
		}

		private void SaveIcon_MouseEnter(object sender, EventArgs e)
		{
			Element element = (Element)sender;
			element.SetVisible("select", true);
			GameManager.Data.LoadAndPlaySystemSound("cursor.ogg");
		}

		private void SaveIcon_MouseLeave(object sender, EventArgs e)
		{
			Element element = (Element)sender;
			element.SetVisible("select", false);
		}

		private void SaveLoadUnit_MouseEnter(object sender, EventArgs e)
		{
			Element element = (Element)sender;
			element.SetVisible("select", true);
			if ((int)element.Value != -1)
			{
				string text = eleScreen["main"].NameOf(element);
				int slotIndex;
				if (text.Length == 4)
				{
					slotIndex = int.Parse(text.Substring(2));
				}
				else
				{
					slotIndex = int.Parse(text.Substring(7));
					slotIndex = ((GameManager.GalCore.GameOptions.QuickSaveIndex != -1) ? (slotIndex + GameManager.GalCore.GameOptions.QuickSaveIndex) : (slotIndex + GameManager.GameScript.QuickIndexStart));
					if (slotIndex >= GameManager.GameScript.QuickIndexEnd)
					{
						slotIndex -= GameManager.GameScript.QuickIndexEnd - GameManager.GameScript.QuickIndexStart;
					}
				}
				GetInfo(slotIndex);
				eleScreen["main"]["snap"].ElementPicture = picSnap;
				strLevel.ClearString();
				strIndex.ClearString();
				strTime.ClearString();
				strComment.ClearString();
				if (Level != null)
				{
					string value = Level.Substring(Level.IndexOf("/") + 2) + "\n";
					strLevel.AddString(value, ContentAlignment.TopCenter);
				}
				strIndex.AddString(slotIndex.ToString("00"));
				strTime.AddString((SaveTime.Year % 100).ToString("00") + "/" + SaveTime.Month.ToString("00") + "/" + SaveTime.Day.ToString("00") + " " + SaveTime.Hour.ToString("00") + ":" + SaveTime.Minute.ToString("00"), ContentAlignment.TopRight);
				if (Comment != null)
				{
					strComment.AddString(Comment, ContentAlignment.TopLeft);
				}
			}
			GameManager.Data.LoadAndPlaySystemSound("cursor.ogg");
		}

		private void SaveLoadUnit_MouseLeave(object sender, EventArgs e)
		{
			Element element = (Element)sender;
			element.SetVisible("select", false);
			if ((int)element.Value != -1)
			{
				eleScreen["main"]["snap"].ElementPicture = null;
				strLevel.ClearString();
				strIndex.ClearString();
				strTime.ClearString();
				strComment.ClearString();
			}
		}

		public override bool FinalScreen()
		{
			if (!base.FinalScreen())
			{
				return false;
			}
			Element element = eleScreen["main"]["sl00"];
			for (int i = 0; i < element.Count; i++)
			{
				element[i].Dispose();
			}
			for (int j = 0; j < eleScreen["main"]["iconpanel"].Count; j++)
			{
				eleScreen["main"]["iconpanel"][j].Dispose();
			}
			for (int k = 0; k < eleScreen["main"].Count; k++)
			{
				eleScreen["main"][k].Dispose();
			}
			eleScreen["main"].Dispose();
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
			if (back)
			{
				centerBack += elaspedTime * 80f;
				while (centerBack >= 80f)
				{
					centerBack -= 80f;
				}
				picBack.Center = new Point((int)(centerBack + 0.5f), (int)(centerBack + 0.5f));
			}
			if (picBack.Time == 0f && !eleScreen.GetVisible("main"))
			{
				if (picBack.Alpha == 255)
				{
					eleScreen.SetVisible("main", true);
				}
				else
				{
					FinalScreen();
				}
			}
		}

		public override void OnKeyPress(Keys keyData)
		{
			if (keyData == Keys.Space && back)
			{
				if (eleScreen.GetVisible("main"))
				{
					eleScreen.SetVisible("main", false);
					picBack.BeginTransform(0.5f);
					picBack.AlphaTo(0);
					picBack.EndTransform();
				}
				GameManager.Data.LoadAndPlaySystemSound("cancel.ogg");
			}
			else
			{
				base.OnKeyPress(keyData);
			}
		}
	}
}
