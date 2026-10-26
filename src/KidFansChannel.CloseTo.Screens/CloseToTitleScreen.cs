using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using KidFansChannel.DataCore;
using KidFansChannel.GalCore;
using KidFansChannel.GalCore.Screens;
using KidFansChannel.Game2DCore;

namespace KidFansChannel.CloseTo.Screens
{
	public class CloseToTitleScreen : TitleScreen
	{
		private Picture picMainBase;

		private SubPicture[] picMainFrames = new SubPicture[60];

		private Picture picTop;

		private Picture picMenuOnBase;

		private Picture[] picMenuOns;

		private Picture picMenuOffBase;

		private Picture[] picMenuOffs;

		private Picture[] picLogos = new Picture[3];

		private Picture picCompany;

		private Picture picChara;

		private Element eleScreen;

		private string[] values = new string[14]
		{
			"Null", "New Game", "Continue", "Options", "Special", "Exit", "Last Game", "Load", "Short Cut", "Album",
			"Jukebox", "Clear List", "Room Mode", "Encyclopedia"
		};

		private List<MenuItem> MenuList = new List<MenuItem>();

		private float moveTime;

		internal string Chara;

		private string command;

		private float mainTime;

		private float nullTime;

		public override bool DataReady
		{
			get
			{
				if (base.DataReady)
				{
					return true;
				}
				if (picMainBase != null && picMainBase.IsCreated && picLogos[0] != null && picLogos[0].IsCreated && picLogos[1] != null && picLogos[1].IsCreated && picLogos[2] != null && picLogos[2].IsCreated && picMenuOnBase != null && picMenuOnBase.IsCreated && picMenuOffBase != null && picMenuOffBase.IsCreated && picTop != null && picTop.IsCreated && picCompany != null && picCompany.IsCreated && (Chara == null || (picChara != null && picChara.IsCreated)))
				{
					for (int i = 0; i < 10; i++)
					{
						for (int j = 0; j < 6; j++)
						{
							picMainFrames[i * 6 + j] = GameManager.Graphics.CreateSubPicture(picMainBase, new Rectangle(j * 64, i * 48, 64, 48));
						}
					}
					for (int k = 0; k < 60; k++)
					{
						picMainFrames[k].Scale = new SizeF(13f, 12.5f);
					}
					for (int l = 0; l < 14; l++)
					{
						picMenuOns[l] = GameManager.Graphics.CreateSubPicture(picMenuOnBase, new Rectangle(0, 40 * l, 320, 40));
						picMenuOffs[l] = GameManager.Graphics.CreateSubPicture(picMenuOffBase, new Rectangle(0, 40 * l, 320, 40));
					}
					for (int m = 0; m < values.Length; m++)
					{
						picMenuOns[m].CenterX = 160;
						picMenuOffs[m].CenterX = 160;
					}
					for (int n = 0; n < 3; n++)
					{
						picLogos[n].CenterX = picLogos[n].Width / 2;
					}
					picCompany.CenterX = picCompany.Width / 2;
					DataReady = true;
					return true;
				}
				return false;
			}
		}

		protected override Point GetItemPosition(int index, int level)
		{
			return new Point(0, index * 40);
		}

		public override void InitData()
		{
			base.InitData();
			picMainBase = GameManager.Data.LoadPicture("system", "title.png");
			picTop = GameManager.Graphics.CreatePicture(Color.Black);
			picTop.Size = GameManager.Graphics.BaseSize;
			picMenuOnBase = GameManager.Data.LoadPicture("system", "title_menu0.png");
			picMenuOffBase = GameManager.Data.LoadPicture("system", "title_menu1.png");
			for (int i = 0; i < 3; i++)
			{
				picLogos[i] = GameManager.Data.LoadPicture("system", "title_logo" + i + ".png");
			}
			picCompany = GameManager.Data.LoadPicture("system", "company.png");
			if (Chara != null)
			{
				picChara = GameManager.Data.LoadPicture("system", "title_" + Chara + ".png");
			}
			picMenuOns = new Picture[values.Length];
			picMenuOffs = new Picture[values.Length];
		}

		public override void FinalData()
		{
			base.FinalData();
			for (int i = 0; i < values.Length; i++)
			{
				picMenuOns[i].Dispose();
				picMenuOffs[i].Dispose();
			}
			picMenuOnBase.Dispose();
			picMenuOffBase.Dispose();
			for (int j = 0; j < 3; j++)
			{
				picLogos[j].Dispose();
			}
			picCompany.Dispose();
			for (int k = 0; k < 60; k++)
			{
				picMainFrames[k].Dispose();
			}
			picMainBase.Dispose();
			picTop.Dispose();
			if (picChara != null)
			{
				picChara.Dispose();
			}
		}

		public override bool InitScreen()
		{
			if (!base.InitScreen())
			{
				return false;
			}
			for (int i = 0; i < 5; i++)
			{
				MenuItem menuItem = new MenuItem();
				menuItem.Add(new Element(picMenuOffs[i + 1]), "off", true);
				menuItem.Add(new Element(picMenuOns[i + 1]), "on", false);
				menuItem.Value = values[i + 1];
				RootItem.Add(menuItem, (string)menuItem.Value);
				MenuList.Add(menuItem);
			}
			for (int j = ((!File.Exists(GameManager.GameState.GetSaveFileName(90))) ? 1 : 0); j < 3; j++)
			{
				MenuItem menuItem2 = new MenuItem();
				menuItem2.Add(new Element(picMenuOffs[j + 6]), "off", true);
				menuItem2.Add(new Element(picMenuOns[j + 6]), "on", false);
				menuItem2.Value = values[j + 6];
				RootItem[1].Add(menuItem2, (string)menuItem2.Value);
				MenuList.Add(menuItem2);
			}
			bool menuDone = false;
			for (int k = 0; k < 7; k++)
			{
				if (GameManager.GameState.CheckEnding(k))
				{
					menuDone = true;
					break;
				}
			}
			for (int l = 0; l < 3; l++)
			{
				MenuItem menuItem3 = new MenuItem();
				if (menuDone)
				{
					menuItem3.Add(new Element(picMenuOffs[l + 9]), "off", true);
					menuItem3.Add(new Element(picMenuOns[l + 9]), "on", false);
				}
				else
				{
					menuItem3.Add(new Element(picMenuOffs[0]), "off", true);
					menuItem3.Add(new Element(picMenuOns[0]), "on", false);
				}
				if (menuDone)
				{
					menuItem3.Value = values[l + 9];
				}
				else
				{
					menuItem3.Value = values[0] + values[l + 9];
				}
				RootItem[3].Add(menuItem3, (string)menuItem3.Value);
				MenuList.Add(menuItem3);
			}
			Element element = new Element();
			eleScreen = new Element();
			eleScreen.Add(element, "main");
			element.Add(new Element(picMainFrames[0]), "old");
			element.Add(new Element(picMainFrames[1]), "new");
			picMainFrames[1].Alpha = 0;
			mainTime = 0f;
			for (int m = 0; m < 3; m++)
			{
				Element element2 = new Element(picLogos[m]);
				element2.Position = new Point(400, 0);
				eleScreen.Add(element2, "logo" + m);
			}
			Element element3 = new Element(picCompany);
			element3.Position = new Point(400, 540);
			eleScreen.Add(element3, "company");
			if (Chara != null)
			{
				Element element4 = new Element(picChara);
				element4.Position = new Point(500, 150);
				eleScreen.Add(element4, "chara");
			}
			ItemElement.Position = new Point(400, 340);
			eleScreen.Add(ItemElement, "menu");
			eleScreen.Add(new Element(picTop), "top");
			RootElement = eleScreen;
			foreach (MenuItem menu in MenuList)
			{
				if (menu.Region.Height != 60)
				{
					menu.Region = new Rectangle(-160, 0, 320, 40);
				}
				menu.MouseEnter += MenuItem_MouseEnter;
				menu.MouseLeave += MenuItem_MouseLeave;
				menu.MouseClick += MenuItem_MouseClick;
				menu.MenuEvent += MenuItem_MenuEvent;
				menu.Initial += MenuItem_Initial;
			}
			bool menuReady = InitMenu();
			for (int n = 0; n < 3; n++)
			{
				picLogos[n].Alpha = 0;
			}
			for (int menuIndex = 0; menuIndex < values.Length; menuIndex++)
			{
				picMenuOns[menuIndex].Alpha = 0;
				picMenuOffs[menuIndex].Alpha = 0;
			}
			picTop.BeginTransform(0.2f);
			picTop.AlphaTo(0);
			picTop.EndTransform();
			if (Chara != null)
			{
				picChara.Alpha = 0;
			}
			if (!menuReady)
			{
				switch (GameManager.Data.CheckData("bgm", "bgm07.ogg", KidFansChannel.DataCore.DataCore.DataType.File))
				{
				case KidFansChannel.DataCore.DataCore.DataType.File:
					GameManager.Sound.OpenBgm(GameManager.Data.GetFile(), true);
					break;
				case KidFansChannel.DataCore.DataCore.DataType.Package:
					GameManager.Sound.OpenBgm(GameManager.Data.GetStream(), true);
					break;
				default:
					return true;
				}
				GameManager.Sound.PlayBgm(0f, 72f);
				GameManager.Sound.VolumeBgm = 1f;
			}
			if (moveTime > 0f)
			{
				moveTime = 0f;
			}
			return true;
		}

		private void MenuItem_Initial(object sender, EventArgs e)
		{
			Element element = (Element)sender;
			element.SetVisible("on", false);
			element.SetVisible("off", true);
		}

		private void MenuItem_MouseEnter(object sender, EventArgs e)
		{
			if (!(picTop.Time > 0f))
			{
				Element element = (Element)sender;
				element.SetVisible("on", true);
				element.SetVisible("off", false);
				GameManager.Data.LoadAndPlaySystemSound("cursor.ogg");
			}
		}

		private void MenuItem_MouseLeave(object sender, EventArgs e)
		{
			if (!(picTop.Time > 0f))
			{
				Element element = (Element)sender;
				element.SetVisible("on", false);
				element.SetVisible("off", true);
			}
		}

		private void MenuItem_MouseClick(object sender, EventArgs e)
		{
			if (picTop.Time > 0f)
			{
				return;
			}
			string text = (string)((Element)sender).Value;
			if (text.StartsWith(values[0]) || picTop.Time > 0f)
			{
				return;
			}
			switch (text)
			{
			case "New Game":
			case "Last Game":
			case "Load":
			case "Short Cut":
			case "Options":
			case "Album":
			case "Jukebox":
			case "Clear List":
			case "Exit":
				command = text;
				picTop.BeginTransform(0.2f);
				picTop.AlphaTo(255);
				picTop.EndTransform();
				break;
			}
			int itemCount = 0;
			for (int i = 0; i < ItemElement.Count; i++)
			{
				if (((string)ItemElement[i].Value).StartsWith(values[0]))
				{
					itemCount++;
				}
			}
			for (int j = 0; j < ItemElement.Count; j++)
			{
				if (ItemElement[j] == sender)
				{
					int y = 60 - ItemElement[j].Position.Y;
					ItemElement[j]["on"].ElementPicture.BeginTransform(0.25f);
					ItemElement[j]["on"].ElementPicture.AlphaTo(0);
					ItemElement[j]["on"].ElementPicture.PositionTo(new Point(0, y));
					ItemElement[j]["on"].ElementPicture.ScaleTo(new SizeF(1.5f, 1.5f));
					ItemElement[j]["on"].ElementPicture.EndTransform();
					ItemElement[j]["off"].ElementPicture.BeginTransform(0.25f);
					ItemElement[j]["off"].ElementPicture.AlphaTo(0);
					ItemElement[j]["off"].ElementPicture.PositionTo(new Point(0, y));
					ItemElement[j]["off"].ElementPicture.ScaleTo(new SizeF(1.5f, 1.5f));
					ItemElement[j]["off"].ElementPicture.EndTransform();
				}
				else
				{
					bool highlighted = ((string)ItemElement[j].Value).StartsWith(values[0]);
					ItemElement[j]["on"].ElementPicture.BeginTransform(highlighted ? (0.25f * (float)itemCount) : 0.25f);
					ItemElement[j]["on"].ElementPicture.AlphaTo(0);
					ItemElement[j]["on"].ElementPicture.PositionTo(new Point(0, -40));
					ItemElement[j]["on"].ElementPicture.ScaleTo(new SizeF(0.5f, 0.5f));
					ItemElement[j]["on"].ElementPicture.EndTransform();
					ItemElement[j]["off"].ElementPicture.BeginTransform(highlighted ? (0.25f * (float)itemCount) : 0.25f);
					ItemElement[j]["off"].ElementPicture.AlphaTo(0);
					ItemElement[j]["off"].ElementPicture.PositionTo(new Point(0, -40));
					ItemElement[j]["off"].ElementPicture.ScaleTo(new SizeF(0.5f, 0.5f));
					ItemElement[j]["off"].ElementPicture.EndTransform();
				}
			}
			if (text != "Exit")
			{
				GameManager.Data.LoadAndPlaySystemSound("ok.ogg");
			}
		}

		private void MenuItem_MenuEvent(object sender, MenuItem.MenuEventArg e)
		{
			Element element = (Element)sender;
			e.Enabled = element["on"].ElementPicture.Time == 0f && element["on"].ElementPicture.Alpha == 255;
		}

		public override bool FinalScreen()
		{
			if (!base.FinalScreen())
			{
				return false;
			}
			foreach (MenuItem menu in MenuList)
			{
				menu["on"].Dispose();
				menu["off"].Dispose();
			}
			MenuList.Clear();
			FinalMenu();
			eleScreen["main"].Dispose();
			for (int i = 0; i < 3; i++)
			{
				eleScreen["logo" + i].Dispose();
			}
			eleScreen["top"].Dispose();
			if (Chara != null)
			{
				eleScreen["chara"].Dispose();
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
			mainTime += elaspedTime;
			while (mainTime >= 30f)
			{
				mainTime -= 30f;
			}
			int frameIndex = (int)(mainTime * 2f);
			float framePhase = mainTime * 2f - (float)frameIndex;
			eleScreen["main"]["old"].ElementPicture = picMainFrames[frameIndex];
			eleScreen["main"]["new"].ElementPicture = picMainFrames[(frameIndex + 1) % 60];
			picMainFrames[frameIndex].Alpha = 255;
			picMainFrames[frameIndex].X = (int)((0f - framePhase) * 16f + 0.5f) - 16;
			picMainFrames[(frameIndex + 1) % 60].Alpha = (int)(framePhase * 255f + 0.5f);
			picMainFrames[(frameIndex + 1) % 60].X = (int)(16f - framePhase * 16f + 0.5f) - 16;
			for (int i = 0; i < 3; i++)
			{
				if (picLogos[i].Time == 0f && picLogos[i].Alpha == 0)
				{
					picLogos[i].BeginTransform(0.5f);
					picLogos[i].AlphaTo(255);
					picLogos[i].EndTransform();
					break;
				}
				if (picLogos[i].Time > 0f)
				{
					break;
				}
			}
			if (Chara != null && picLogos[2].Time == 0f && picLogos[2].Alpha == 255 && picChara.Time == 0f && picChara.Alpha == 0)
			{
				picChara.BeginTransform(1f);
				picChara.AlphaTo(255);
				picChara.EndTransform();
			}
			if (picTop.Time == 0f && picTop.Alpha == 255)
			{
				if (command == "New Game")
				{
					StartNewGame("CloseTo");
				}
				else if (command == "Last Game")
				{
					GameManager.GameScript.LoadStory(90);
					FinalScreen();
				}
				else if (command == "Load")
				{
					GameManager.GalCore.ShowSaveLoadScreen(false);
				}
				else if (command == "Short Cut")
				{
					GameManager.GalCore.ShowShortcutScreen();
				}
				else if (command == "Options")
				{
					GameManager.GalCore.ShowOptionsScreen();
				}
				else if (command == "Album")
				{
					GameManager.GalCore.ShowAlbumScreen();
				}
				else if (command == "Jukebox")
				{
					GameManager.GalCore.ShowMusicScreen();
				}
				else if (command == "Clear List")
				{
					GameManager.GalCore.ShowClearListScreen();
				}
				else if (!(command == "Encyclopedia") && !(command == "Room Mode") && command == "Exit")
				{
					ExitGame();
				}
				return;
			}
			for (int j = 0; j < ItemElement.Count; j++)
			{
				if (!ItemElement[j].GetVisible("on"))
				{
					ItemElement[j]["on"].ElementPicture.Update(elaspedTime);
				}
				if (!ItemElement[j].GetVisible("off"))
				{
					ItemElement[j]["off"].ElementPicture.Update(elaspedTime);
				}
			}
			if (nullTime > 0f)
			{
				nullTime -= elaspedTime;
				if (nullTime < 0f)
				{
					nullTime = 0f;
				}
				for (int k = 0; k < ItemElement.Count; k++)
				{
					if (((string)ItemElement[k].Value).StartsWith(values[0]))
					{
						if (nullTime > elaspedTime)
						{
							ItemElement[k]["on"].Position = new Point(0, (int)((float)(-k * 40 - 40) * nullTime / 0.35f + 0.5f));
							ItemElement[k]["off"].Position = new Point(0, (int)((float)(-k * 40 - 40) * nullTime / 0.35f + 0.5f));
						}
						else
						{
							ItemElement[k]["on"].Position = Point.Empty;
							ItemElement[k]["off"].Position = Point.Empty;
						}
					}
				}
			}
			if (ItemElement.Count > 0 && ItemElement[0]["on"].ElementPicture.Time == 0f && ItemElement[0]["on"].ElementPicture.Alpha == 0)
			{
				ChangeItem();
				int itemCount = 0;
				for (int l = 0; l < ItemElement.Count; l++)
				{
					if (((string)ItemElement[l].Value).StartsWith(values[0]))
					{
						itemCount++;
					}
				}
				if (itemCount > 0)
				{
					nullTime = 0.35f;
				}
				for (int m = 0; m < ItemElement.Count; m++)
				{
					bool highlighted = ((string)ItemElement[m].Value).StartsWith(values[0]);
					ItemElement[m]["on"].ElementPicture.Alpha = 1;
					if (highlighted)
					{
						ItemElement[m]["on"].Position = new Point(0, -ItemElement[m].Position.Y - 40);
						ItemElement[m]["on"].ElementPicture.Y = 0;
					}
					else
					{
						ItemElement[m]["on"].ElementPicture.Y = -ItemElement[m].Position.Y - 40;
					}
					ItemElement[m]["on"].ElementPicture.Scale = new SizeF(1f, 1f);
					ItemElement[m]["on"].ElementPicture.BeginTransform(highlighted ? (0.35f * (float)itemCount) : 0.35f);
					ItemElement[m]["on"].ElementPicture.AlphaTo(255);
					if (!highlighted)
					{
						ItemElement[m]["on"].ElementPicture.PositionTo(Point.Empty);
					}
					ItemElement[m]["on"].ElementPicture.EndTransform();
					ItemElement[m]["off"].ElementPicture.Alpha = 1;
					if (highlighted)
					{
						ItemElement[m]["off"].Position = new Point(0, -ItemElement[m].Position.Y - 40);
						ItemElement[m]["off"].ElementPicture.Y = 0;
					}
					else
					{
						ItemElement[m]["off"].ElementPicture.Y = -ItemElement[m].Position.Y - 40;
					}
					ItemElement[m]["off"].ElementPicture.Scale = new SizeF(1f, 1f);
					ItemElement[m]["off"].ElementPicture.BeginTransform(highlighted ? (0.35f * (float)itemCount) : 0.35f);
					ItemElement[m]["off"].ElementPicture.AlphaTo(255);
					if (!highlighted)
					{
						ItemElement[m]["off"].ElementPicture.PositionTo(Point.Empty);
					}
					ItemElement[m]["off"].ElementPicture.EndTransform();
				}
			}
			if (moveTime == -1f)
			{
				moveTime = 0f;
				switch (GameManager.Data.CheckData("bgm", "bgm07.ogg", KidFansChannel.DataCore.DataCore.DataType.File))
				{
				default:
					return;
				case KidFansChannel.DataCore.DataCore.DataType.File:
					GameManager.Sound.OpenBgm(GameManager.Data.GetFile(), true);
					break;
				case KidFansChannel.DataCore.DataCore.DataType.Package:
					GameManager.Sound.OpenBgm(GameManager.Data.GetStream(), true);
					break;
				}
				GameManager.Sound.PlayBgm(0f, 72f);
				GameManager.Sound.VolumeBgm = 1f;
			}
			else
			{
				moveTime += elaspedTime;
			}
			if (!(moveTime > 20f))
			{
				return;
			}
			if (GameManager.GameState.CheckMovie(0))
			{
				KidFansChannel.DataCore.DataCore.DataType dataType = GameManager.Data.CheckData("movie", "op.wmv", KidFansChannel.DataCore.DataCore.DataType.File);
				if (dataType == KidFansChannel.DataCore.DataCore.DataType.File)
				{
					string text = GameManager.Data.GetFile();
					if (!GameManager.GalCore.GameOptions.SubtitleShow && text.EndsWith(GameManager.Data.LanguageExt + "\\op.wmv") && File.Exists(text.Replace(GameManager.Data.LanguageExt + "\\op.wmv", "op.wmv")))
					{
						text = text.Replace(GameManager.Data.LanguageExt + "\\op.wmv", "op.wmv");
					}
					GameManager.Sound.StopBgm();
					if (GameManager.GalCore.GameVideo != null)
					{
						GameManager.GalCore.GameVideo.Play(text);
					}
					moveTime = -1f;
				}
				else
				{
					moveTime = 0f;
				}
			}
			else
			{
				moveTime = 0f;
			}
		}

		public override void OnKeyPress(Keys keyData)
		{
			if (moveTime > 0f)
			{
				moveTime = 0f;
			}
			if (picTop.Time != 0f || picTop.Alpha == 255)
			{
				return;
			}
			base.OnKeyPress(keyData);
			if (ItemToBack == null)
			{
				return;
			}
			int itemCount = 0;
			for (int i = 0; i < ItemElement.Count; i++)
			{
				if (((string)ItemElement[i].Value).StartsWith(values[0]))
				{
					itemCount++;
				}
			}
			for (int j = 0; j < ItemElement.Count; j++)
			{
				bool highlighted = ((string)ItemElement[j].Value).StartsWith(values[0]);
				ItemElement[j]["on"].ElementPicture.BeginTransform(highlighted ? (0.25f * (float)itemCount) : 0.25f);
				ItemElement[j]["on"].ElementPicture.AlphaTo(0);
				ItemElement[j]["on"].ElementPicture.PositionTo(new Point(0, -40));
				ItemElement[j]["on"].ElementPicture.ScaleTo(new SizeF(0.5f, 0.5f));
				ItemElement[j]["on"].ElementPicture.EndTransform();
				ItemElement[j]["off"].ElementPicture.BeginTransform(highlighted ? (0.25f * (float)itemCount) : 0.25f);
				ItemElement[j]["off"].ElementPicture.AlphaTo(0);
				ItemElement[j]["off"].ElementPicture.PositionTo(new Point(0, -40));
				ItemElement[j]["off"].ElementPicture.ScaleTo(new SizeF(0.5f, 0.5f));
				ItemElement[j]["off"].ElementPicture.EndTransform();
			}
			GameManager.Data.LoadAndPlaySystemSound("cancel.ogg");
		}

		public override void OnMouseDown(Point location)
		{
			if (moveTime > 0f)
			{
				moveTime = 0f;
			}
			if (picTop.Time == 0f && picTop.Alpha != 255)
			{
				base.OnMouseDown(location);
			}
		}

		public override void OnMouseMove(Point location)
		{
			if (moveTime > 0f)
			{
				moveTime = 0f;
			}
			if (picTop.Time == 0f && picTop.Alpha != 255)
			{
				base.OnMouseMove(location);
			}
		}

		public override void OnMouseWheel(int delta)
		{
			if (moveTime > 0f)
			{
				moveTime = 0f;
			}
			if (picTop.Time == 0f && picTop.Alpha != 255)
			{
				base.OnMouseWheel(delta);
			}
		}
	}
}
