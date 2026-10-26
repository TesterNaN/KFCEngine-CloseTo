using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using KidFansChannel.GalCore;
using KidFansChannel.GalCore.Screens;
using KidFansChannel.Game2DCore;

namespace KidFansChannel.CloseTo.Screens
{
	public class CloseToEncyListScreen : EncyListScreen
	{
		private Picture picNewBase;

		private Picture[] picNews = new Picture[4];

		private Picture picNewAnime;

		private Picture picMain;

		private Picture picBack;

		private Picture picBar;

		private static int LineNum = 14;

		private String2D[] strIds = new String2D[LineNum];

		private String2D[] strTitles = new String2D[LineNum];

		private String2D[] strCatalogues = new String2D[LineNum];

		private Element eleScreen;

		private CloseToScrollBarElement scrollBar = new CloseToScrollBarElement();

		private bool back;

		private bool returnFromContent;

		private int[] order;

		private int currentStartId;

		public override bool DataReady
		{
			get
			{
				if (base.DataReady)
				{
					return true;
				}
				if (picNewBase != null && picNewBase.IsCreated && scrollBar.DataReady && picMain != null && picMain.IsCreated && picBack != null && picBack.IsCreated && picBar != null && picBar.IsCreated)
				{
					for (int i = 0; i < 4; i++)
					{
						picNews[i] = GameManager.Graphics.CreateSubPicture(picNewBase, new Rectangle(0, i * 25, 40, 25));
					}
					picNewAnime = GameManager.Graphics.CreateAnimePicture(new Size(40, 25));
					for (int j = 0; j < 4; j++)
					{
						((AnimePicture)picNewAnime).AddFrame(picNews[j], 0.5f);
					}
					DataReady = true;
					return true;
				}
				return false;
			}
		}

		public CloseToEncyListScreen(bool back)
		{
			this.back = back;
		}

		public override void InitData()
		{
			base.InitData();
			picNewBase = GameManager.Data.LoadPicture("system", "ency_new.png");
			picMain = GameManager.Data.LoadPicture("system", "ency_list.png");
			picBar = GameManager.Data.LoadPicture("system", "bar.png");
			if (back)
			{
				picBack = GameManager.Data.LoadPicture("system", "back.png");
			}
			else
			{
				picBack = GameManager.Graphics.CreatePicture(Color.FromArgb(128, Color.Black));
			}
			Font2D font = GameManager.GalCore.GameFont.GetFont(20);
			for (int i = 0; i < LineNum; i++)
			{
				strIds[i] = new String2D();
				strIds[i].Font = font;
				strIds[i].Location = Point.Empty;
				strIds[i].LineSize = new Size(60, 35);
				strIds[i].LineNum = 1;
			}
			for (int j = 0; j < LineNum; j++)
			{
				strTitles[j] = new String2D();
				strTitles[j].Font = font;
				strTitles[j].Location = Point.Empty;
				strTitles[j].LineSize = new Size(450, 35);
				strTitles[j].LineNum = 1;
			}
			for (int k = 0; k < LineNum; k++)
			{
				strCatalogues[k] = new String2D();
				strCatalogues[k].Font = font;
				strCatalogues[k].Location = Point.Empty;
				strCatalogues[k].LineSize = new Size(150, 35);
				strCatalogues[k].LineNum = 1;
			}
			scrollBar.InitData();
		}

		public override void FinalData()
		{
			base.FinalData();
			picBack.Dispose();
			picNewAnime.Dispose();
			for (int i = 0; i < 4; i++)
			{
				picNews[i].Dispose();
			}
			picNewBase.Dispose();
			picMain.Dispose();
			picBar.Dispose();
			scrollBar.FinalData();
		}

		public override bool InitScreen()
		{
			if (!base.InitScreen())
			{
				return false;
			}
			switch (GameManager.GalCore.GameOptions.EncyOrder)
			{
			case 0:
			{
				order = new int[Titles.Length];
				for (int k = 0; k < Titles.Length; k++)
				{
					order[k] = k;
				}
				break;
			}
			case 1:
				order = new int[100]
				{
					8, 9, 12, 13, 14, 19, 20, 24, 25, 27,
					29, 30, 33, 34, 35, 36, 38, 43, 41, 42,
					48, 58, 61, 62, 65, 66, 67, 73, 75, 76,
					81, 0, 1, 4, 6, 7, 11, 16, 18, 21,
					22, 23, 26, 28, 31, 32, 37, 40, 45, 49,
					50, 52, 54, 55, 57, 59, 71, 64, 68, 72,
					74, 77, 78, 79, 80, 82, 97, 98, 99, 3,
					39, 2, 44, 60, 95, 96, 53, 56, 94, 10,
					63, 5, 46, 51, 69, 70, 17, 47, 15, 83,
					84, 85, 86, 87, 88, 89, 90, 91, 92, 93
				};
				break;
			case 2:
			{
				SortedList<string, int> sortedList = new SortedList<string, int>();
				for (int i = 0; i < Titles.Length; i++)
				{
					sortedList.Add(Titles[i], i);
				}
				order = new int[Titles.Length];
				for (int j = 0; j < Titles.Length; j++)
				{
					order[j] = sortedList.Values[j];
				}
				break;
			}
			}
			Element element = new Element(picMain);
			eleScreen = new Element(picBack);
			eleScreen.Add(element, "main");
			element.Focused = true;
			FocusedElement = element;
			RootElement = eleScreen;
			Element element2 = new Element(picNewAnime);
			element2.Position = new Point(100, 5);
			Element element3 = new Element(picBar);
			element3.Position = new Point(0, -5);
			picBar.ScaleX = 4.5f;
			for (int l = 0; l < LineNum; l++)
			{
				Element element4 = new Element(strIds[l]);
				element4.Position = new Point(25, 0);
				Element element5 = new Element(strTitles[l]);
				element5.Position = new Point(100, 0);
				Element element6 = new Element(strCatalogues[l]);
				element6.Position = new Point(550, 0);
				Element element7 = new Element();
				element7.Add(element3, "bar", false);
				element7.Add(element2, "new", false);
				element7.Add(element4, "id", true);
				element7.Add(element5, "title", true);
				element7.Add(element6, "catalogue", true);
				element7.Position = new Point(25, 75 + l * 35);
				element7.Region = new Rectangle(0, 0, 720, 35);
				element.Add(element7, l.ToString("00"));
			}
			scrollBar.Init();
			scrollBar.Position = new Point(760, 70);
			scrollBar.Size = 500;
			element.Add(scrollBar, "scrollbar");
			scrollBar.Length = (float)LineNum / (float)Titles.Length;
			scrollBar.ValueChanged += ScrollBar_ValueChanged;
			for (int m = 0; m < LineNum - 1; m++)
			{
				element[m.ToString("00")].Down = element[(m + 1).ToString("00")];
				element[(m + 1).ToString("00")].Up = element[m.ToString("00")];
			}
			element.ChildDown = element["00"];
			element.ChildUp = element[(LineNum - 1).ToString("00")];
			element.Add(new Element(), "up");
			element.Add(new Element(), "down");
			element["00"].Up = element["up"];
			element[(LineNum - 1).ToString("00")].Down = element["down"];
			for (int n = 0; n < LineNum; n++)
			{
				element[n.ToString("00")].MouseClick += Unit_MouseClick;
				element[n.ToString("00")].MouseEnter += Unit_MouseEnter;
				element[n.ToString("00")].MouseLeave += Unit_MouseLeave;
				element[n.ToString("00")].KeyPress += Unit_KeyPress;
			}
			element["up"].MouseEnter += Up_MouseEnter;
			element["down"].MouseEnter += Down_MouseEnter;
			UpdateList();
			if (back && !returnFromContent)
			{
				picBack.Alpha = 0;
				picBack.BeginTransform(0.5f);
				picBack.AlphaTo(255);
				picBack.EndTransform();
				RootElement.SetVisible("main", false);
			}
			return true;
		}

		private void ScrollBar_ValueChanged(object sender, EventArgs e)
		{
			currentStartId = (int)(((ScrollBarElement)sender).Start * (float)Titles.Length + 0.5f);
			if (currentStartId < 0)
			{
				currentStartId = 0;
			}
			if (currentStartId > Titles.Length - LineNum)
			{
				currentStartId = Titles.Length - LineNum;
				if (currentStartId < 0)
				{
					currentStartId = 0;
				}
			}
			UpdateList();
		}

		private void Unit_MouseClick(object sender, EventArgs e)
		{
			Element element = (Element)sender;
			int entryId = (int)element.Value;
			if (Opens[order[entryId]])
			{
				GameManager.Data.LoadAndPlaySystemSound("ok.ogg");
				GameManager.GalCore.ShowEncyContentScreen(order[entryId]);
				returnFromContent = true;
			}
		}

		private void Unit_MouseEnter(object sender, EventArgs e)
		{
			Element element = (Element)sender;
			element.SetVisible("bar", true);
		}

		private void Unit_MouseLeave(object sender, EventArgs e)
		{
			Element element = (Element)sender;
			element.SetVisible("bar", false);
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
				UpdateList();
				eleScreen["main"]["00"].Selected = true;
				break;
			case Keys.Next:
				currentStartId += LineNum;
				if (currentStartId > Titles.Length - LineNum)
				{
					currentStartId = Titles.Length - LineNum;
				}
				UpdateList();
				eleScreen["main"][(LineNum - 1).ToString()].Selected = true;
				break;
			}
		}

		private void Up_MouseEnter(object sender, EventArgs e)
		{
			if (currentStartId > 0)
			{
				currentStartId--;
			}
			UpdateList();
			eleScreen["main"]["00"].Selected = true;
		}

		private void Down_MouseEnter(object sender, EventArgs e)
		{
			if (currentStartId < Titles.Length - LineNum)
			{
				currentStartId++;
			}
			UpdateList();
			eleScreen["main"][(LineNum - 1).ToString()].Selected = true;
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
				UpdateList();
			}
			else
			{
				if (delta >= 0)
				{
					return;
				}
				currentStartId -= delta / 120;
				if (currentStartId > Titles.Length - LineNum)
				{
					currentStartId = Titles.Length - LineNum;
					if (currentStartId < 0)
					{
						currentStartId = 0;
					}
				}
				UpdateList();
			}
		}

		public override bool FinalScreen()
		{
			if (!base.FinalScreen())
			{
				return false;
			}
			eleScreen["main"]["00"]["bar"].Dispose();
			eleScreen["main"]["00"]["new"].Dispose();
			for (int i = 0; i < LineNum; i++)
			{
				eleScreen["main"][i.ToString("00")]["id"].Dispose();
				eleScreen["main"][i.ToString("00")]["title"].Dispose();
				eleScreen["main"][i.ToString("00")]["catalogue"].Dispose();
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
			return true;
		}

		public override void Update(float elaspedTime)
		{
			base.Update(elaspedTime);
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

		protected void UpdateList()
		{
			for (int i = 0; i < LineNum; i++)
			{
				int entryId = i + currentStartId;
				strIds[i].ClearString();
				strIds[i].AddString((entryId + 1).ToString("000"));
				strTitles[i].ClearString();
				strCatalogues[i].ClearString();
				if (Opens[order[entryId]])
				{
					strTitles[i].AddString(Titles[order[entryId]]);
					eleScreen["main"][i.ToString("00")]["title"].Position = new Point(Views[order[entryId]] ? 100 : 150, 0);
					eleScreen["main"][i.ToString("00")].SetVisible("new", !Views[order[entryId]]);
					strCatalogues[i].AddString(Catalogues[order[entryId]], ContentAlignment.TopRight);
				}
				else
				{
					for (int j = 0; j < Titles[order[entryId]].Length; j++)
					{
						strTitles[i].AddString("?");
					}
					eleScreen["main"][i.ToString("00")]["title"].Position = new Point(100, 0);
					eleScreen["main"][i.ToString("00")].SetVisible("new", false);
				}
				eleScreen["main"][i.ToString("00")].Value = entryId;
			}
			scrollBar.Start = (float)currentStartId / (float)Titles.Length;
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
			}
			else
			{
				base.OnKeyPress(keyData);
			}
		}
	}
}
