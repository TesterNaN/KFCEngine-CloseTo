using System;
using System.Drawing;
using System.Windows.Forms;
using KidFansChannel.GalCore;
using KidFansChannel.GalCore.Screens;
using KidFansChannel.Game2DCore;

namespace KidFansChannel.CloseTo.Screens
{
	internal class CloseToMessageBoxScreen : MessageBoxScreen
	{
		private BindingPicture picBack;

		private static Picture selectBar;

		private static Picture iconBase;

		private static SubPicture[] icons = new SubPicture[5];

		private static AnimePicture[] iconAnimes = new AnimePicture[2];

		private int labelId;

		protected int currentSelected = -1;

		protected int lastSelected = -1;

		protected float iconTime;

		protected int iconPasses;

		protected bool iconDirection;

		protected float iconX;

		private int[] widths = new int[2];

		private int[] heights = new int[2];

		public override bool DataReady
		{
			get
			{
				if (base.DataReady)
				{
					return true;
				}
				if (selectBar != null && selectBar.IsCreated && iconBase != null && iconBase.IsCreated)
				{
					selectBar.Center = new Point(80, 20);
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

		public CloseToMessageBoxScreen(string text, MessageBoxButtons buttons, MessageBoxCallback callBack, object[] callBackArgs)
			: base(text, buttons, callBack, callBackArgs)
		{
		}

		public CloseToMessageBoxScreen(string text, MessageBoxButtons buttons, bool pauseBack, MessageBoxCallback callBack, object[] callBackArgs)
			: base(text, buttons, pauseBack, callBack, callBackArgs)
		{
		}

		protected override void SetButtonFontStyle(String2D button)
		{
			button.Font = GameManager.GalCore.GameFont.GetFont(24);
			button.Location = Point.Empty;
			button.LineSize = new Size(160, 40);
			button.LineNum = 1;
		}

		protected override void SetTextFontStyle(String2D text)
		{
			text.Font = GameManager.GalCore.GameFont.GetFont(24);
			text.Location = Point.Empty;
			text.LineSize = new Size(720, 40);
			text.LineNum = 3;
		}

		public override void InitData()
		{
			base.InitData();
			selectBar = GameManager.Data.LoadPicture("system", "select_bar.png");
			iconBase = GameManager.Data.LoadPicture("system", "click.png");
			labelId = Label.GenerateId();
		}

		public override void FinalData()
		{
			base.FinalData();
			picBack = null;
			selectBar.Dispose();
			for (int i = 0; i < 2; i++)
			{
				iconAnimes[i].Dispose();
			}
			for (int j = 0; j < 5; j++)
			{
				icons[j].Dispose();
			}
			iconBase.Dispose();
			Label.ReleaseId(labelId);
		}

		public override bool InitScreen()
		{
			if (!base.InitScreen())
			{
				return false;
			}
			int totalLine = eleScreen["text"].ElementString.TotalLine;
			int width = eleScreen["text"].ElementString.Width + 80;
			int num = (eleScreen["text"].ElementString.TotalLine + ((Buttons == MessageBoxButtons.OK) ? 1 : 2)) * 40 + 50;
			Label.CurrentId = labelId;
			Label.SetBackSize(width, num);
			int colorIndex = Label.GetColorIndex();
			picBack = GameManager.Graphics.CreateBindingPicture();
			for (int i = 0; i < 9; i++)
			{
				picBack.AddPicture(Label.PicBacks[0, i], Point.Empty);
				picBack.AddPicture(Label.PicBacks[colorIndex, i], Point.Empty);
			}
			eleScreen.ElementPicture = picBack;
			eleScreen.Position = new Point(400, ((Buttons == MessageBoxButtons.OK) ? 360 : 400) - num / 2);
			eleScreen["text"].Position = new Point(-360, -num / 2 + 25);
			if (Buttons == MessageBoxButtons.OK)
			{
				eleScreen["ok"].Position = new Point(-80, num / 2 - 60);
				eleScreen["ok"].Region = new Rectangle(0, 0, 160, 40);
				eleScreen["ok"].MouseEnter += Button_MouseEnter;
				eleScreen["ok"].MouseLeave += Button_MouseLeave;
				widths[0] = eleScreen["ok"]["text"].ElementString.Width;
				heights[0] = num / 2 - 56;
			}
			else
			{
				eleScreen["ok"].Position = new Point(-80, num / 2 - 100);
				eleScreen["ok"].Region = new Rectangle(0, 0, 160, 40);
				eleScreen["ok"].MouseEnter += Button_MouseEnter;
				eleScreen["ok"].MouseLeave += Button_MouseLeave;
				eleScreen["cancel"].Position = new Point(-80, num / 2 - 60);
				eleScreen["cancel"].Region = new Rectangle(0, 0, 160, 40);
				eleScreen["cancel"].MouseEnter += Button_MouseEnter;
				eleScreen["cancel"].MouseLeave += Button_MouseLeave;
				widths[0] = eleScreen["ok"]["text"].ElementString.Width;
				widths[1] = eleScreen["cancel"]["text"].ElementString.Width;
				heights[0] = num / 2 - 100;
				heights[1] = num / 2 - 60;
			}
			Element element = new Element(selectBar);
			element.Position = new Point(0, 20);
			eleScreen.Add(element, "bar");
			Element element2 = new Element(iconAnimes[0]);
			element2.Position = new Point(0, 0);
			eleScreen.Add(element2, "icon0");
			Element element3 = new Element(iconAnimes[1]);
			element3.Position = new Point(0, 0);
			eleScreen.Add(element3, "icon1");
			eleScreen.SetVisible("bar", false);
			eleScreen.SetVisible("icon0", false);
			eleScreen.SetVisible("icon1", false);
			GameManager.Data.LoadAndPlaySystemSound("warning.ogg");
			return true;
		}

		private void Button_MouseEnter(object sender, EventArgs e)
		{
			Element element = (Element)sender;
			if (Buttons == MessageBoxButtons.OK)
			{
				currentSelected = 0;
			}
			else if (element == eleScreen["ok"])
			{
				currentSelected = 0;
			}
			else if (element == eleScreen["cancel"])
			{
				currentSelected = 1;
			}
			eleScreen.SetVisible("bar", true);
			eleScreen.SetVisible("icon0", true);
			eleScreen.SetVisible("icon1", true);
			GameManager.Data.LoadAndPlaySystemSound("cursor.ogg");
		}

		private void Button_MouseLeave(object sender, EventArgs e)
		{
			Element element = (Element)sender;
			currentSelected = -1;
			eleScreen.SetVisible("bar", false);
			eleScreen.SetVisible("icon0", false);
			eleScreen.SetVisible("icon1", false);
		}

		public override void Update(float elaspedTime)
		{
			Label.CurrentId = labelId;
			base.Update(elaspedTime);
			for (int i = 0; i < 2; i++)
			{
				iconAnimes[i].Update(elaspedTime);
			}
			if (lastSelected != currentSelected)
			{
				if (currentSelected != -1)
				{
					if (lastSelected == -1)
					{
						selectBar.Scale = new SizeF((float)(widths[currentSelected] + 80) / (float)selectBar.Width, 1f);
						selectBar.Position = new Point(0, heights[currentSelected]);
						selectBar.Alpha = 0;
						selectBar.BeginTransform(0.1f);
						selectBar.AlphaTo(255);
						selectBar.EndTransform();
						iconTime = 0.5f;
						iconPasses = 1;
						iconDirection = true;
						for (int j = 0; j < 2; j++)
						{
							iconAnimes[j].Position = new Point(0, heights[currentSelected]);
						}
						iconX = -widths[currentSelected] / 2 - 15;
					}
					else
					{
						selectBar.BeginTransform(0.1f);
						selectBar.ScaleTo(new SizeF((float)(widths[currentSelected] + 80) / (float)selectBar.Width, 1f));
						selectBar.PositionTo(new Point(0, heights[currentSelected]));
						selectBar.EndTransform();
						if (iconPasses == 0 && iconTime == 0f)
						{
							iconTime = 0.5f;
							iconPasses = 1;
							iconDirection = true;
							if (iconX > 0f)
							{
								iconX = 0f - iconX;
							}
						}
						else if (iconPasses == 0)
						{
							iconPasses = 1;
						}
						for (int k = 0; k < 2; k++)
						{
							iconAnimes[k].BeginTransform(iconTime);
							iconAnimes[k].PositionTo(new Point(0, heights[currentSelected]));
							iconAnimes[k].EndTransform();
						}
					}
				}
				else
				{
					iconTime = 0f;
					iconPasses = 0;
					iconDirection = false;
					iconX = -widths[lastSelected] / 2 - 15;
				}
				lastSelected = currentSelected;
			}
			if (iconPasses >= 0 && iconTime > 0f)
			{
				iconTime -= elaspedTime;
				bool flag = false;
				while (iconTime < 0f)
				{
					flag = true;
					iconTime += 0.5f;
					iconPasses--;
					iconDirection = !iconDirection;
				}
				if (iconPasses < 0)
				{
					iconTime = 0f;
					iconPasses = 0;
					iconX = (iconDirection ? (widths[currentSelected] / 2 + 15) : (-(widths[currentSelected] / 2 + 15)));
				}
				else if (flag)
				{
					iconX = (float)(widths[currentSelected] + 30) * (0.5f - iconTime) / 0.5f;
					if (iconDirection)
					{
						iconX = iconX - (float)(widths[currentSelected] / 2) - 15f;
					}
					else
					{
						iconX = (float)(widths[currentSelected] / 2 + 15) - iconX;
					}
				}
				else
				{
					iconX += ((float)(iconDirection ? (widths[currentSelected] / 2 + 15) : (-(widths[currentSelected] / 2 + 15))) - iconX) * elaspedTime / (iconTime + elaspedTime);
				}
				eleScreen["icon0"].Position = new Point((int)(iconX + 0.5f), 20);
				eleScreen["icon1"].Position = new Point((int)(0f - iconX + 0.5f), 20);
			}
			selectBar.Update(elaspedTime);
		}

		public override void Draw()
		{
			Label.CurrentId = labelId;
			base.Draw();
		}

		protected override void OnOK()
		{
			base.OnOK();
			GameManager.Data.LoadAndPlaySystemSound("ok.ogg");
		}

		protected override void OnCancel()
		{
			base.OnCancel();
			GameManager.Data.LoadAndPlaySystemSound("cancel.ogg");
		}
	}
}
