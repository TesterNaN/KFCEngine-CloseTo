using System;
using System.Drawing;
using System.Windows.Forms;
using KidFansChannel.GalCore;
using KidFansChannel.GalCore.Screens;
using KidFansChannel.Game2DCore;

namespace KidFansChannel.CloseTo.Screens
{
	public class CloseToOptionsScreen : OptionsScreen
	{
		private Picture[] picMains = new Picture[5];

		private Picture picBack;

		private Picture picSelectBase;

		private Picture[] picSelects = new Picture[2];

		private Picture picIconBase;

		private Picture[] picIcons;

		private Element eleScreen;

		private String2D strIntro;

		private String2D[][] strItems;

		private CloseToScrollBarElement scrollBar = new CloseToScrollBarElement();

		private bool back;

		private float centerBack;

		private int optionsStart;

		private int optionsLength = 7;

		public override bool DataReady
		{
			get
			{
				if (base.DataReady)
				{
					return true;
				}
				if (picMains[0] != null && picMains[0].IsCreated && picMains[1] != null && picMains[1].IsCreated && picMains[2] != null && picMains[2].IsCreated && picMains[3] != null && picMains[3].IsCreated && picMains[4] != null && picMains[4].IsCreated && picBack != null && picBack.IsCreated && picSelectBase != null && picSelectBase.IsCreated && picIconBase != null && picIconBase.IsCreated && scrollBar.DataReady)
				{
					picSelects[0] = GameManager.Graphics.CreateSubPicture(picSelectBase, new Rectangle(0, 0, 25, 25));
					picSelects[1] = GameManager.Graphics.CreateSubPicture(picSelectBase, new Rectangle(0, 25, 25, 25));
					picSelects[0].CenterX = 25;
					picSelects[1].CenterX = 25;
					picIcons = new Picture[ItemTitles.Count];
					for (int i = 0; i < ItemTitles.Count; i++)
					{
						picIcons[i] = GameManager.Graphics.CreateSubPicture(picIconBase, new Rectangle(ItemIcon[i] % 2 * 200, ItemIcon[i] / 2 * 40, 40, 40));
					}
					DataReady = true;
					return true;
				}
				return false;
			}
		}

		public CloseToOptionsScreen(bool back)
		{
			this.back = back;
		}

		public override void InitData()
		{
			base.InitData();
			for (int i = 0; i < 5; i++)
			{
				picMains[i] = GameManager.Data.LoadPicture("system", "options" + i + ".png");
			}
			picSelectBase = GameManager.Data.LoadPicture("system", "mark.png");
			picIconBase = GameManager.Data.LoadPicture("system", "options_icon.png");
			if (back)
			{
				picBack = GameManager.Data.LoadPicture("system", "screenback4.png");
			}
			else
			{
				picBack = GameManager.Graphics.CreatePicture(Color.FromArgb(128, Color.Black));
			}
			Font2D font = GameManager.GalCore.GameFont.GetFont(24);
			Font2D font2 = GameManager.GalCore.GameFont.GetFont(20);
			strIntro = new String2D();
			strIntro.Font = font;
			strIntro.Location = new Point(0, 0);
			strIntro.LineSize = new Size(700, 40);
			strIntro.LineNum = 4;
			strItems = new String2D[ItemValues.Count][];
			for (int j = 0; j < ItemValues.Count; j++)
			{
				strItems[j] = new String2D[ItemValues[j].Count];
				for (int k = 0; k < ItemValues[j].Count; k++)
				{
					strItems[j][k] = new String2D();
					strItems[j][k].Font = font2;
					strItems[j][k].Location = new Point(0, 0);
					strItems[j][k].LineSize = new Size(600 / ItemValues[j].Count, 40);
					strItems[j][k].LineNum = 1;
					strItems[j][k].AddString(ItemValues[j][k], ContentAlignment.TopCenter);
				}
			}
			scrollBar.InitData();
		}

		public override void FinalData()
		{
			base.FinalData();
			for (int i = 0; i < 5; i++)
			{
				picMains[i].Dispose();
			}
			picBack.Dispose();
			for (int j = 0; j < 2; j++)
			{
				picSelects[j].Dispose();
			}
			picSelectBase.Dispose();
			for (int k = 0; k < ItemTitles.Count; k++)
			{
				picIcons[k].Dispose();
			}
			picIconBase.Dispose();
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
			element.Add(new Element(), "main0");
			Element element2 = new Element(strIntro);
			element2.Position = new Point(50, 400);
			eleScreen["main"].Add(element2, "intro");
			for (int i = 0; i < ItemValues.Count; i++)
			{
				for (int j = 0; j < strItems[i].Length; j++)
				{
					Element element3 = new Element(strItems[i][j]);
					int num = 600 / strItems[i].Length;
					element3.Position = new Point(150 + num * j, 80 + 40 * i);
					element3.Region = new Rectangle((num - strItems[i][j].Width) / 2, 0, strItems[i][j].Width, 40);
					Element element4 = new Element(picSelects[0]);
					element4.Position = new Point(element3.Region.Left - 5, 0);
					element3.Add(element4, "no", false);
					Element element5 = new Element(picSelects[1]);
					element5.Position = new Point(element3.Region.Left - 5, 0);
					element3.Add(element5, "yes", false);
					eleScreen["main"].Add(element3, i.ToString("00") + j);
				}
				Element element6 = new Element(picIcons[i]);
				element6.Position = new Point(80, 80 + 40 * i);
				eleScreen["main"].Add(element6, "icon" + i.ToString("00"));
			}
			for (int k = 0; k < ItemValues.Count; k++)
			{
				for (int l = 0; l < strItems[k].Length; l++)
				{
					if (k < ItemValues.Count - 1)
					{
						int num2 = (int)((double)((float)l * (float)(strItems[k + 1].Length - 1) / (float)(strItems[k].Length - 1)) + 0.5);
						eleScreen["main"][k.ToString("00") + l].Down = eleScreen["main"][(k + 1).ToString("00") + num2];
					}
					if (k > 0)
					{
						int num3 = (int)((double)((float)l * (float)(strItems[k - 1].Length - 1) / (float)(strItems[k].Length - 1)) + 0.5);
						eleScreen["main"][k.ToString("00") + l].Up = eleScreen["main"][(k - 1).ToString("00") + num3];
					}
					eleScreen["main"][k.ToString("00") + l].Right = eleScreen["main"][k.ToString("00") + (l + 1) % strItems[k].Length];
					eleScreen["main"][k.ToString("00") + (l + 1) % strItems[k].Length].Left = eleScreen["main"][k.ToString("00") + l];
					eleScreen["main"][k.ToString("00") + l].Value = (k << 8) + l;
					eleScreen["main"][k.ToString("00") + l].MouseEnter += OptionsValue_MouseEnter;
					eleScreen["main"][k.ToString("00") + l].MouseLeave += OptionsValue_MouseLeave;
					eleScreen["main"][k.ToString("00") + l].MouseClick += OptionsValue_MouseClick;
				}
			}
			Element element7 = eleScreen["main"];
			Element childUp = (eleScreen["main"].ChildLeft = eleScreen["main"][(ItemValues.Count - 1).ToString("00") + (strItems[ItemValues.Count - 1].Length - 1)]);
			element7.ChildUp = childUp;
			Element element9 = eleScreen["main"];
			Element childDown = (eleScreen["main"].ChildRight = eleScreen["main"]["000"]);
			element9.ChildDown = childDown;
			scrollBar.Init();
			scrollBar.Position = new Point(755, 75);
			scrollBar.Size = 290;
			element.Add(scrollBar, "scrollbar");
			scrollBar.Length = (float)optionsLength / (float)ItemTitles.Count;
			scrollBar.ValueChanged += ScrollBar_ValueChanged;
			UpdateOptions();
			eleScreen["main"].Focused = true;
			FocusedElement = eleScreen["main"];
			RootElement = eleScreen;
			InitValue();
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

		private void ScrollBar_ValueChanged(object sender, EventArgs e)
		{
			optionsStart = (int)(((ScrollBarElement)sender).Start * (float)ItemTitles.Count + 0.5f);
			if (optionsStart < 0)
			{
				optionsStart = 0;
			}
			if (optionsStart > ItemTitles.Count - optionsLength)
			{
				optionsStart = ItemTitles.Count - optionsLength;
				if (optionsStart < 0)
				{
					optionsStart = 0;
				}
			}
			eleScreen["main"].ClearSelected();
			UpdateOptions();
		}

		private void InitValue()
		{
			for (int i = 0; i < ItemTitles.Count; i++)
			{
				int optionsValue = GetOptionsValue(i);
				if (optionsValue != -1)
				{
					UpdateValue(i, optionsValue);
				}
			}
		}

		private void UpdateValue(int index, int value)
		{
			eleScreen["main"][index.ToString("00") + value].SetVisible("yes", true);
			eleScreen["main"]["icon" + index.ToString("00")].ElementPicture = (picIcons[index] = GameManager.Graphics.CreateSubPicture(picIconBase, new Rectangle(ItemIcon[index] % 2 * 200 + value * 40, ItemIcon[index] / 2 * 40, 40, 40)));
			if (OptionsTitles[index] == "DialogueColor")
			{
				if (OptionsValues[index][value] == "Blue")
				{
					eleScreen["main"]["main0"].ElementPicture = picMains[1];
				}
				else if (OptionsValues[index][value] == "Green")
				{
					eleScreen["main"]["main0"].ElementPicture = picMains[2];
				}
				else if (OptionsValues[index][value] == "Pink")
				{
					eleScreen["main"]["main0"].ElementPicture = picMains[3];
				}
				else if (OptionsValues[index][value] == "Gray")
				{
					eleScreen["main"]["main0"].ElementPicture = picMains[4];
				}
			}
			else if (OptionsTitles[index] == "DialogueAlpha")
			{
				eleScreen["main"].ElementPicture.Alpha = (int)(float.Parse(OptionsValues[index][value]) * 255f + 0.5f);
			}
		}

		private void OptionsValue_MouseEnter(object sender, EventArgs e)
		{
			Element element = (Element)sender;
			int num = (int)element.Value >> 8;
			element.SetVisible("no", true);
			if (num < optionsStart)
			{
				optionsStart = num;
				UpdateOptions();
			}
			else if (num >= optionsStart + optionsLength)
			{
				optionsStart = num - optionsLength + 1;
				UpdateOptions();
			}
			strIntro.ClearString();
			strIntro.AddString(ItemTitles[num] + "\n", ContentAlignment.TopCenter);
			strIntro.AddXmlString(ItemIntros[num], new NodeInfo(), ContentAlignment.TopLeft);
			GameManager.Data.LoadAndPlaySystemSound("cursor.ogg");
		}

		private void OptionsValue_MouseLeave(object sender, EventArgs e)
		{
			Element element = (Element)sender;
			element.SetVisible("no", false);
		}

		private void OptionsValue_MouseClick(object sender, EventArgs e)
		{
			Element element = (Element)sender;
			int num = (int)element.Value >> 8;
			int value = (int)element.Value & 0xF;
			SetOptionsValue(num, value);
			for (int i = 0; i < strItems[num].Length; i++)
			{
				eleScreen["main"][num.ToString("00") + i].SetVisible("yes", false);
			}
			value = GetOptionsValue(num);
			UpdateValue(num, value);
			GameManager.Data.LoadAndPlaySystemSound("ok.ogg");
		}

		public override bool FinalScreen()
		{
			if (!base.FinalScreen())
			{
				return false;
			}
			scrollBar.Final();
			for (int i = 0; i < eleScreen["main"].Count; i++)
			{
				eleScreen["main"][i].Dispose();
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
			centerBack += elaspedTime * 80f;
			while (centerBack >= 80f)
			{
				centerBack -= 80f;
			}
			picBack.Center = new Point((int)(centerBack + 0.5f), (int)(centerBack + 0.5f));
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

		public override void OnMouseWheel(int delta)
		{
			base.OnMouseWheel(delta);
			eleScreen["main"].ClearSelected();
			if (delta > 0)
			{
				optionsStart -= delta / 120;
				if (optionsStart < 0)
				{
					optionsStart = 0;
				}
				UpdateOptions();
			}
			else
			{
				if (delta >= 0)
				{
					return;
				}
				optionsStart -= delta / 120;
				if (optionsStart > ItemTitles.Count - optionsLength)
				{
					optionsStart = ItemTitles.Count - optionsLength;
					if (optionsStart < 0)
					{
						optionsStart = 0;
					}
				}
				UpdateOptions();
			}
		}

		public void UpdateOptions()
		{
			for (int i = 0; i < optionsStart; i++)
			{
				for (int j = 0; j < ItemValues[i].Count; j++)
				{
					eleScreen["main"].SetVisible(i.ToString("00") + j, false);
				}
				eleScreen["main"].SetVisible("icon" + i.ToString("00"), false);
			}
			for (int k = 0; k < optionsLength; k++)
			{
				for (int l = 0; l < ItemValues[k + optionsStart].Count; l++)
				{
					eleScreen["main"].SetVisible((k + optionsStart).ToString("00") + l, true);
					Element element = eleScreen["main"][(k + optionsStart).ToString("00") + l];
					element.Position = new Point(element.Position.X, 80 + 40 * k);
				}
				eleScreen["main"].SetVisible("icon" + (k + optionsStart).ToString("00"), true);
				Element element2 = eleScreen["main"]["icon" + (k + optionsStart).ToString("00")];
				element2.Position = new Point(element2.Position.X, 80 + 40 * k);
			}
			for (int m = optionsStart + optionsLength; m < ItemTitles.Count; m++)
			{
				for (int n = 0; n < ItemValues[m].Count; n++)
				{
					eleScreen["main"].SetVisible(m.ToString("00") + n, false);
				}
				eleScreen["main"].SetVisible("icon" + m.ToString("00"), false);
			}
			scrollBar.Start = (float)optionsStart / (float)ItemTitles.Count;
		}
	}
}
