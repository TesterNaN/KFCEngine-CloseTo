using System;
using System.Drawing;
using System.Windows.Forms;
using KidFansChannel.DataCore;
using KidFansChannel.GalCore;
using KidFansChannel.GalCore.Screens;
using KidFansChannel.Game2DCore;

namespace KidFansChannel.CloseTo.Screens
{
	public class CloseToMusicScreen : MusicScreen
	{
		private Picture picDisc;

		private Picture[] picMains = new Picture[2];

		private Picture picBack;

		private Picture picBar;

		private static int LineNum = 8;

		private String2D[] strNames = new String2D[LineNum];

		private String2D[] strTimes = new String2D[LineNum];

		private String2D strName;

		private String2D strTime;

		private String2D strInfo;

		private String2D strLyricBack;

		private String2D strLyricFore;

		private Element eleScreen;

		private CloseToScrollBarElement scrollBar = new CloseToScrollBarElement();

		private int currentPlayId = -1;

		private int showId = -1;

		private float playTime;

		private bool showCurrentTime;

		private float currentLyricProcess;

		private float centerBack;

		private int currentStartId;

		public override bool DataReady
		{
			get
			{
				if (base.DataReady)
				{
					return true;
				}
				if (picDisc != null && picDisc.IsCreated && scrollBar.DataReady && picMains[0] != null && picMains[0].IsCreated && picMains[1] != null && picMains[1].IsCreated && picBack != null && picBack.IsCreated && picBar != null && picBar.IsCreated)
				{
					picMains[0].Alpha = (int)(GameManager.GalCore.GameOptions.DialogueAlpha * 255f + 0.5f);
					picDisc.Center = new Point(20, 20);
					DataReady = true;
					return true;
				}
				return false;
			}
		}

		public override void InitData()
		{
			base.InitData();
			picDisc = GameManager.Data.LoadPicture("system", "music_disc.png");
			picMains[0] = GameManager.Data.LoadPicture("system", "music0.png");
			int num;
			switch (GameManager.GalCore.GameOptions.DialogueColor)
			{
			case "Blue":
				num = 1;
				break;
			case "Green":
				num = 2;
				break;
			case "Pink":
				num = 3;
				break;
			case "Gray":
				num = 4;
				break;
			default:
				throw new Exception("Unbelievable!");
			}
			picMains[1] = GameManager.Data.LoadPicture("system", "music" + num + ".png");
			picBar = GameManager.Data.LoadPicture("system", "select_bar.png");
			picBack = GameManager.Data.LoadPicture("system", "screenback1.png");
			Font2D font = GameManager.GalCore.GameFont.GetFont(24);
			Font2D font2 = GameManager.GalCore.GameFont.GetFont(20);
			for (int i = 0; i < LineNum; i++)
			{
				strNames[i] = new String2D();
				strNames[i].Font = font;
				strNames[i].Location = Point.Empty;
				strNames[i].LineSize = new Size(600, 40);
				strNames[i].LineNum = 1;
			}
			for (int j = 0; j < LineNum; j++)
			{
				strTimes[j] = new String2D();
				strTimes[j].Font = font;
				strTimes[j].Location = Point.Empty;
				strTimes[j].LineSize = new Size(100, 40);
				strTimes[j].LineNum = 1;
			}
			strName = new String2D();
			strName.Font = font;
			strName.Location = Point.Empty;
			strName.LineSize = new Size(600, 40);
			strName.LineNum = 1;
			strTime = new String2D();
			strTime.Font = font2;
			strTime.Location = Point.Empty;
			strTime.LineSize = new Size(100, 36);
			strTime.LineNum = 1;
			strInfo = new String2D();
			strInfo.Font = font2;
			strInfo.Location = Point.Empty;
			strInfo.LineSize = new Size(500, 36);
			strInfo.LineNum = 2;
			strLyricBack = new String2D();
			strLyricBack.Font = font2;
			strLyricBack.Location = Point.Empty;
			strLyricBack.LineSize = new Size(650, 36);
			strLyricBack.LineNum = 2;
			strLyricFore = new String2D();
			strLyricFore.Font = font2;
			strLyricFore.Location = new Point(100, 135);
			strLyricFore.LineSize = new Size(650, 36);
			strLyricFore.LineNum = 2;
			strLyricFore.DefaultColor = Color.Aqua;
			strLyricFore.Canvas = GameManager.Graphics;
			scrollBar.InitData();
		}

		public override void FinalData()
		{
			base.FinalData();
			picBack.Dispose();
			picDisc.Dispose();
			for (int i = 0; i < 2; i++)
			{
				picMains[i].Dispose();
			}
			picBar.Dispose();
			scrollBar.FinalData();
		}

		public override bool InitScreen()
		{
			if (!base.InitScreen())
			{
				return false;
			}
			Element element = new Element(picMains[0]);
			eleScreen = new Element(picBack);
			eleScreen.Add(element, "main");
			element.Add(new Element(picMains[1]), "main0");
			element.Focused = true;
			FocusedElement = element;
			RootElement = eleScreen;
			Element element2 = new Element(picDisc);
			element2.Position = new Point(45, 260);
			element.Add(element2, "disc", false);
			Element element3 = new Element(picBar);
			element3.Position = new Point(0, -5);
			picBar.ScaleX = 4.5f;
			for (int i = 0; i < LineNum; i++)
			{
				Element element4 = new Element(strNames[i]);
				element4.Position = new Point(40, 0);
				Element element5 = new Element(strTimes[i]);
				element5.Position = new Point(600, 0);
				Element element6 = new Element();
				element6.Add(element3, "bar", false);
				element6.Add(element4, "name", true);
				element6.Add(element5, "time", true);
				element6.Position = new Point(25, 245 + i * 40);
				element6.Region = new Rectangle(0, 0, 720, 40);
				element.Add(element6, i.ToString("00"));
			}
			Element element7 = new Element(strName);
			element7.Position = new Point(50, 90);
			element.Add(element7, "name");
			Element element8 = new Element(strTime);
			element8.Position = new Point(650, 90);
			element.Add(element8, "time");
			Element element9 = new Element(strInfo);
			element9.Position = new Point(100, 135);
			element.Add(element9, "info");
			Element element10 = new Element(strLyricBack);
			element10.Position = new Point(100, 135);
			element.Add(element10, "lyricback");
			element.SetVisible("lyricback", false);
			scrollBar.Init();
			scrollBar.Position = new Point(755, 245);
			scrollBar.Size = 320;
			element.Add(scrollBar, "scrollbar");
			scrollBar.Length = (float)LineNum / (float)Names.Length;
			scrollBar.ValueChanged += ScrollBar_ValueChanged;
			for (int j = 0; j < LineNum - 1; j++)
			{
				element[j.ToString("00")].Down = element[(j + 1).ToString("00")];
				element[(j + 1).ToString("00")].Up = element[j.ToString("00")];
			}
			element.ChildDown = element["00"];
			element.ChildUp = element[(LineNum - 1).ToString("00")];
			element.Add(new Element(), "up");
			element.Add(new Element(), "down");
			element["00"].Up = element["up"];
			element[(LineNum - 1).ToString("00")].Down = element["down"];
			for (int k = 0; k < LineNum; k++)
			{
				element[k.ToString("00")].MouseClick += Unit_MouseClick;
				element[k.ToString("00")].MouseEnter += Unit_MouseEnter;
				element[k.ToString("00")].MouseLeave += Unit_MouseLeave;
				element[k.ToString("00")].KeyPress += Unit_KeyPress;
			}
			element["up"].MouseEnter += Up_MouseEnter;
			element["down"].MouseEnter += Down_MouseEnter;
			UpdateMusic();
			picBack.Alpha = 0;
			picBack.BeginTransform(0.5f);
			picBack.AlphaTo(255);
			picBack.EndTransform();
			RootElement.SetVisible("main", false);
			GameManager.Sound.StopBgm();
			return true;
		}

		private void ScrollBar_ValueChanged(object sender, EventArgs e)
		{
			currentStartId = (int)(((ScrollBarElement)sender).Start * (float)Names.Length + 0.5f);
			if (currentStartId < 0)
			{
				currentStartId = 0;
			}
			if (currentStartId > Names.Length - LineNum)
			{
				currentStartId = Names.Length - LineNum;
				if (currentStartId < 0)
				{
					currentStartId = 0;
				}
			}
			UpdateMusic();
		}

		private void Unit_MouseClick(object sender, EventArgs e)
		{
			Element element = (Element)sender;
			int num = (int)element.Value;
			if (!Opens[num])
			{
				return;
			}
			if (currentPlayId == num)
			{
				StopMusic();
				currentPlayId = -1;
				eleScreen["main"].SetVisible("disc", false);
				eleScreen["main"].SetVisible("lyricback", false);
				eleScreen["main"].SetVisible("info", true);
				return;
			}
			PlayMusic(Files[num]);
			currentPlayId = num;
			eleScreen["main"].SetVisible("disc", true);
			if (PrepareLyric(Files[num]))
			{
				eleScreen["main"].SetVisible("lyricback", true);
				eleScreen["main"].SetVisible("info", false);
			}
			eleScreen["main"]["disc"].Position = new Point(45, 260 + (num - currentStartId) * 40);
			playTime = 0f;
		}

		private void Unit_MouseEnter(object sender, EventArgs e)
		{
			Element element = (Element)sender;
			element.SetVisible("bar", true);
			int num = (int)element.Value;
			strName.ClearString();
			strTime.ClearString();
			strInfo.ClearString();
			if (Opens[num])
			{
				strName.AddString((num + 1).ToString("00") + ". " + Names[num]);
				strTime.AddString(Times[num], ContentAlignment.TopRight);
				strInfo.AddString(Infos[num]);
			}
			showId = num;
			showCurrentTime = false;
			GameManager.Data.LoadAndPlaySystemSound("cursor.ogg");
		}

		private void Unit_MouseLeave(object sender, EventArgs e)
		{
			Element element = (Element)sender;
			element.SetVisible("bar", false);
			showId = -1;
		}

		private void Unit_KeyPress(object sender, KeyEventArgs e)
		{
			switch (e.KeyData)
			{
			case Keys.Prior:
				currentStartId -= LineNum;
				if (currentStartId < 0)
				{
					currentStartId = 0;
				}
				UpdateMusic();
				eleScreen["main"]["00"].Selected = true;
				break;
			case Keys.Next:
				currentStartId += LineNum;
				if (currentStartId > Names.Length - LineNum)
				{
					currentStartId = Names.Length - LineNum;
				}
				UpdateMusic();
				eleScreen["main"][(LineNum - 1).ToString("00")].Selected = true;
				break;
			}
		}

		private void Up_MouseEnter(object sender, EventArgs e)
		{
			if (currentStartId > 0)
			{
				currentStartId--;
			}
			eleScreen["main"]["00"].Selected = true;
			UpdateMusic();
			eleScreen["main"]["00"].Selected = true;
		}

		private void Down_MouseEnter(object sender, EventArgs e)
		{
			if (currentStartId < Names.Length - LineNum)
			{
				currentStartId++;
			}
			eleScreen["main"][(LineNum - 1).ToString("00")].Selected = true;
			UpdateMusic();
			eleScreen["main"][(LineNum - 1).ToString("00")].Selected = true;
		}

		public override void OnMouseWheel(int delta)
		{
			base.OnMouseWheel(delta);
			if (delta > 0)
			{
				currentStartId -= delta / 120;
				if (currentStartId < 0)
				{
					currentStartId = 0;
				}
				UpdateMusic();
			}
			else
			{
				if (delta >= 0)
				{
					return;
				}
				currentStartId -= delta / 120;
				if (currentStartId > Names.Length - LineNum)
				{
					currentStartId = Names.Length - LineNum;
					if (currentStartId < 0)
					{
						currentStartId = 0;
					}
				}
				UpdateMusic();
			}
		}

		public override bool FinalScreen()
		{
			if (!base.FinalScreen())
			{
				return false;
			}
			eleScreen["main"]["00"]["bar"].Dispose();
			for (int i = 0; i < LineNum; i++)
			{
				eleScreen["main"][i.ToString("00")]["name"].Dispose();
				eleScreen["main"][i.ToString("00")]["time"].Dispose();
			}
			for (int j = 0; j < eleScreen["main"].Count; j++)
			{
				if (eleScreen["main"][j] != scrollBar)
				{
					eleScreen["main"][j].Dispose();
				}
			}
			scrollBar.Final();
			for (int k = 0; k < eleScreen.Count; k++)
			{
				eleScreen[k].Dispose();
			}
			eleScreen.Dispose();
			eleScreen = null;
			FinalData();
			Finished = true;
			FocusedElement = null;
			RootElement = null;
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
			currentLyricProcess = 0f;
			if (currentPlayId != -1)
			{
				if (!IsMusicing)
				{
					currentPlayId = -1;
					eleScreen["main"].SetVisible("disc", false);
					eleScreen["main"].SetVisible("lyricback", false);
					eleScreen["main"].SetVisible("info", true);
				}
				else
				{
					playTime += elaspedTime;
					if (currentPlayId == showId)
					{
						int num = (int)playTime / 60;
						int num2 = (int)playTime % 60;
						strTime.ClearString();
						strTime.AddString(num + ":" + num2.ToString("00"), ContentAlignment.TopRight);
						showCurrentTime = true;
						if (Lyrics != null)
						{
							if (eleScreen["main"].GetVisible("info"))
							{
								eleScreen["main"].SetVisible("lyricback", true);
								eleScreen["main"].SetVisible("info", false);
							}
							int num3 = -1;
							for (int i = 0; i < Lyrics.Length; i++)
							{
								if (Starts[i] <= playTime && Ends[i] > playTime)
								{
									num3 = i;
									break;
								}
							}
							strLyricBack.ClearString();
							strLyricFore.ClearString();
							if (num3 != -1)
							{
								strLyricBack.AddString(Lyrics[num3]);
								strLyricFore.AddString(Lyrics[num3]);
								currentLyricProcess = (playTime - Starts[num3]) / (Ends[num3] - Starts[num3]);
							}
						}
					}
					else if (showId == -1)
					{
						if (showCurrentTime)
						{
							int num4 = (int)playTime / 60;
							int num5 = (int)playTime % 60;
							strTime.ClearString();
							strTime.AddString(num4 + ":" + num5.ToString("00"), ContentAlignment.TopRight);
						}
					}
					else if (!eleScreen["main"].GetVisible("info"))
					{
						eleScreen["main"].SetVisible("lyricback", false);
						eleScreen["main"].SetVisible("info", true);
					}
				}
			}
			picDisc.Rotate += elaspedTime * 4f;
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

		protected void UpdateMusic()
		{
			bool flag = false;
			for (int i = 0; i < LineNum; i++)
			{
				int num = i + currentStartId;
				strNames[i].ClearString();
				strTimes[i].ClearString();
				if (Opens[num])
				{
					strNames[i].AddString((num + 1).ToString("00") + ". " + Names[num]);
					strTimes[i].AddString(Times[num], ContentAlignment.TopRight);
				}
				else
				{
					for (int j = 0; j < Names[num].Length; j++)
					{
						strNames[i].AddString("?");
					}
				}
				eleScreen["main"][i.ToString("00")].Value = num;
				if (num == currentPlayId)
				{
					eleScreen["main"].SetVisible("disc", true);
					eleScreen["main"]["disc"].Position = new Point(45, 260 + i * 40);
					flag = true;
				}
			}
			scrollBar.Start = (float)currentStartId / (float)Names.Length;
			if (eleScreen["main"].SelectedChild != null && eleScreen["main"].SelectedChild != scrollBar)
			{
				int num2 = (int)eleScreen["main"].SelectedChild.Value;
				strName.ClearString();
				strTime.ClearString();
				strInfo.ClearString();
				if (Opens[num2])
				{
					strName.AddString((num2 + 1).ToString("00") + ". " + Names[num2]);
					strTime.AddString(Times[num2], ContentAlignment.TopRight);
					strInfo.AddString(Infos[num2]);
				}
				showId = num2;
				showCurrentTime = false;
			}
			else
			{
				showId = -1;
			}
			if (!flag)
			{
				eleScreen["main"].SetVisible("disc", false);
			}
		}

		public override void OnKeyPress(Keys keyData)
		{
			if (keyData == Keys.Space)
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

		public override void Draw()
		{
			base.Draw();
			if (eleScreen != null && eleScreen.IndexOf("main") != -1 && eleScreen.GetVisible("main") && currentLyricProcess > 0f)
			{
				strLyricFore.DrawPartString(currentLyricProcess);
			}
		}
	}
}
