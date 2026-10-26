using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using KidFansChannel.GalCore;
using KidFansChannel.GalCore.Screens;
using KidFansChannel.Game2DCore;

namespace KidFansChannel.CloseTo.Screens
{
	public class CloseToEncyContentScreen : EncyContentScreen
	{
		private Picture picNewBase;

		private Picture[] picNews = new Picture[4];

		private Picture picNewAnime;

		private Picture picMain;

		private Picture picBack;

		private Picture picIllustrate;

		private static int LineNum = 9;

		private String2D strTitle;

		private String2D strCatalogue;

		private String2D strContent;

		private Element eleScreen;

		private CloseToScrollBarElement scrollBar = new CloseToScrollBarElement();

		private bool back;

		private int totalLine;

		private int currentStartLine;

		public override bool DataReady
		{
			get
			{
				if (base.DataReady)
				{
					return true;
				}
				if (picNewBase != null && picNewBase.IsCreated && scrollBar.DataReady && picMain != null && picMain.IsCreated && picBack != null && picBack.IsCreated && picIllustrate != null && picIllustrate.IsCreated)
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

		public CloseToEncyContentScreen(int index, bool back)
			: base(index)
		{
			this.back = back;
		}

		public override void InitData()
		{
			base.InitData();
			picNewBase = GameManager.Data.LoadPicture("system", "ency_new.png");
			picMain = GameManager.Data.LoadPicture("system", "ency_content.png");
			if (back)
			{
				picBack = GameManager.Data.LoadPicture("system", "back.png");
			}
			else
			{
				picBack = GameManager.Graphics.CreatePicture(Color.FromArgb(128, Color.Black));
			}
			try
			{
				picIllustrate = GameManager.Data.LoadPicture("system", "ency\\" + Index.ToString("000") + ".png");
			}
			catch (FileNotFoundException)
			{
				picIllustrate = GameManager.Data.LoadPicture("system", "ency_none.png");
			}
			Font2D font = GameManager.GalCore.GameFont.GetFont(20);
			strTitle = new String2D();
			strTitle.Font = font;
			strTitle.Location = Point.Empty;
			strTitle.LineSize = new Size(500, 40);
			strTitle.LineNum = 2;
			strCatalogue = new String2D();
			strCatalogue.Font = font;
			strCatalogue.Location = Point.Empty;
			strCatalogue.LineSize = new Size(150, 40);
			strCatalogue.LineNum = 1;
			strContent = new String2D();
			strContent.Font = font;
			strContent.Location = Point.Empty;
			strContent.LineSize = new Size(700, 35);
			strContent.LineNum = LineNum;
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
			picIllustrate.Dispose();
			scrollBar.FinalData();
		}

		public override bool InitScreen()
		{
			if (!base.InitScreen())
			{
				return false;
			}
			Element element = new Element(picMain);
			eleScreen = new Element(picBack);
			eleScreen.Add(element, "main");
			element.Focused = true;
			FocusedElement = element;
			RootElement = eleScreen;
			Element element2 = new Element(picNewAnime);
			element2.Position = new Point(40, 75);
			element.Add(element2, "new", !View);
			strTitle.AddString(Title);
			Element element3 = new Element(strTitle);
			element3.Position = new Point(40, 100);
			strCatalogue.AddString(Catalogue);
			Element element4 = new Element(strCatalogue);
			element4.Position = new Point(40, 180);
			if (Content != null)
			{
				strContent.AddXmlString(Content, new NodeInfo());
			}
			Element element5 = new Element(strContent);
			element5.Position = new Point(35, 245);
			element.Add(element3, "title");
			element.Add(element4, "catalogue");
			element.Add(element5, "content");
			Element element6 = new Element(picIllustrate);
			element6.Position = new Point(550, 75);
			element.Add(element6, "illustrate");
			totalLine = strContent.TotalLine;
			scrollBar.Init();
			scrollBar.Position = new Point(760, 240);
			scrollBar.Size = 330;
			scrollBar.Length = (float)LineNum / (float)totalLine;
			scrollBar.ValueChanged += ScrollBar_ValueChanged;
			element.Add(scrollBar, "scrollbar");
			UpdateContent();
			return true;
		}

		private void ScrollBar_ValueChanged(object sender, EventArgs e)
		{
			currentStartLine = (int)(((ScrollBarElement)sender).Start * (float)totalLine + 0.5f);
			if (currentStartLine < 0)
			{
				currentStartLine = 0;
			}
			if (currentStartLine > totalLine - LineNum)
			{
				currentStartLine = totalLine - LineNum;
				if (currentStartLine < 0)
				{
					currentStartLine = 0;
				}
			}
			UpdateContent();
		}

		public override void OnKeyPress(Keys keyData)
		{
			base.OnKeyPress(keyData);
			switch (keyData)
			{
			case Keys.Up:
				if (currentStartLine > 0)
				{
					currentStartLine--;
				}
				UpdateContent();
				break;
			case Keys.Down:
				if (currentStartLine < totalLine - LineNum)
				{
					currentStartLine++;
				}
				UpdateContent();
				break;
			case Keys.Prior:
				currentStartLine -= LineNum;
				if (currentStartLine < 0)
				{
					currentStartLine = 0;
				}
				UpdateContent();
				break;
			case Keys.Next:
				currentStartLine += LineNum;
				if (currentStartLine > totalLine - LineNum)
				{
					currentStartLine = totalLine - LineNum;
					if (currentStartLine < 0)
					{
						currentStartLine = 0;
					}
				}
				UpdateContent();
				break;
			}
		}

		public override void OnMouseWheel(int delta)
		{
			base.OnMouseWheel(delta);
			if (delta > 0)
			{
				currentStartLine -= delta / 120;
				if (currentStartLine < 0)
				{
					currentStartLine = 0;
				}
				UpdateContent();
			}
			else
			{
				if (delta >= 0)
				{
					return;
				}
				currentStartLine -= delta / 120;
				if (currentStartLine > totalLine - LineNum)
				{
					currentStartLine = totalLine - LineNum;
					if (currentStartLine < 0)
					{
						currentStartLine = 0;
					}
				}
				UpdateContent();
			}
		}

		public override bool FinalScreen()
		{
			if (!base.FinalScreen())
			{
				return false;
			}
			for (int i = 0; i < eleScreen["main"].Count; i++)
			{
				if (eleScreen["main"][i] != scrollBar)
				{
					eleScreen["main"][i].Dispose();
				}
			}
			scrollBar.Final();
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

		private void UpdateContent()
		{
			strContent.ClearString();
			if (Content != null)
			{
				strContent.AddXmlString(Content, new NodeInfo());
			}
			strContent.CutLine(currentStartLine, currentStartLine + LineNum - 1);
			scrollBar.Start = (float)currentStartLine / (float)totalLine;
		}
	}
}
