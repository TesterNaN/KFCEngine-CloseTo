using System;
using System.Drawing;
using System.Windows.Forms;
using KidFansChannel.GalCore;
using KidFansChannel.GalCore.Screens;
using KidFansChannel.Game2DCore;

namespace KidFansChannel.CloseTo.Screens
{
	public class CloseToShortcutScreen : ShortcutScreen
	{
		private Picture[] picMains = new Picture[2];

		private Picture picBack;

		private Picture picBar;

		private String2D strInfo;

		private String2D[] strMenus = new String2D[14];

		private Element eleScreen;

		private int[] order = new int[14]
		{
			0, 1, 2, 4, 3, 5, 7, 6, 8, 10,
			9, 11, 12, 13
		};

		private float centerBack;

		public override bool DataReady
		{
			get
			{
				if (base.DataReady)
				{
					return true;
				}
				if (picBar != null && picBar.IsCreated && picMains[0] != null && picMains[0].IsCreated && picMains[1] != null && picMains[1].IsCreated && picBack != null && picBack.IsCreated)
				{
					picMains[0].Alpha = (int)(GameManager.GalCore.GameOptions.DialogueAlpha * 255f + 0.5f);
					DataReady = true;
					return true;
				}
				return false;
			}
		}

		public override void InitData()
		{
			base.InitData();
			Opens[0] = (Opens[1] = GameManager.GameState.GlobalVarDefines["Clr_Mai"].Value == 1 || GameManager.GameState.GlobalVarDefines["Clr_Yuna"].Value == 1 || GameManager.GameState.GlobalVarDefines["Clr_Shoko_Scut"].Value == 1 || GameManager.GameState.GlobalVarDefines["Clr_Koyuki_Scut"].Value == 1);
			Opens[2] = (Opens[3] = GameManager.GameState.GlobalVarDefines["Clr_Yuna"].Value == 1);
			Opens[4] = GameManager.GameState.GlobalVarDefines["Clr_Shoko_Scut"].Value == 1;
			Opens[5] = GameManager.GameState.GlobalVarDefines["Clr_Shoko_Good"].Value == 1;
			Opens[6] = GameManager.GameState.GlobalVarDefines["Clr_Shoko_Bad"].Value == 1;
			Opens[7] = GameManager.GameState.GlobalVarDefines["Clr_Koyuki_Scut"].Value == 1;
			Opens[8] = GameManager.GameState.GlobalVarDefines["Clr_Koyuki_B"].Value == 1;
			Opens[9] = GameManager.GameState.GlobalVarDefines["Clr_Koyuki_A"].Value == 1;
			Opens[10] = (Opens[11] = GameManager.GameState.GlobalVarDefines["Clr_Mai"].Value == 1);
			Opens[12] = (Opens[13] = GameManager.GameState.GlobalVarDefines["Clr_Ghost"].Value == 1);
			picMains[0] = GameManager.Data.LoadPicture("system", "shortcut0.png");
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
			picMains[1] = GameManager.Data.LoadPicture("system", "shortcut" + num + ".png");
			picBar = GameManager.Data.LoadPicture("system", "select_bar.png");
			picBack = GameManager.Data.LoadPicture("system", "screenback0.png");
			Font2D font = GameManager.GalCore.GameFont.GetFont(24);
			Font2D font2 = GameManager.GalCore.GameFont.GetFont(20);
			for (int i = 0; i < 14; i++)
			{
				strMenus[i] = new String2D();
				strMenus[i].Font = font2;
				strMenus[i].Location = new Point(0, 5);
				strMenus[i].LineSize = new Size(350, 40);
				strMenus[i].LineNum = 1;
			}
			strInfo = new String2D();
			strInfo.Font = font;
			strInfo.Location = Point.Empty;
			strInfo.LineSize = new Size(700, 36);
			strInfo.LineNum = 4;
		}

		public override void FinalData()
		{
			base.FinalData();
			picBack.Dispose();
			for (int i = 0; i < 2; i++)
			{
				picMains[i].Dispose();
			}
			picBar.Dispose();
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
			Element item = new Element(picBar);
			picBar.ScaleX = 2.1875f;
			for (int i = 0; i < 14; i++)
			{
				Element element2 = new Element(strMenus[i]);
				element2.Position = new Point(order[i] % 2 * 350 + 50, order[i] / 2 * 40 + 80);
				element2.Add(item, "bar", false);
				element2.Region = new Rectangle(0, 0, 350, 40);
				element2.Value = i;
				strMenus[i].ClearString();
				if (Opens[i])
				{
					strMenus[i].AddString(Names[i], ContentAlignment.TopCenter);
				}
				else
				{
					string text = string.Empty;
					for (int j = 0; j < Names[i].Length; j++)
					{
						text += "?";
					}
					strMenus[i].AddString(text, ContentAlignment.TopCenter);
				}
				element.Add(element2, i.ToString("00"));
			}
			Element element3 = new Element(strInfo);
			element3.Position = new Point(50, 400);
			element.Add(element3, "info");
			for (int k = 0; k < 14; k++)
			{
				element[order[k].ToString("00")].Up = element[order[(k + 12) % 14].ToString("00")];
				element[order[k].ToString("00")].Down = element[order[(k + 2) % 14].ToString("00")];
				element[order[k].ToString("00")].Left = element[order[k / 2 * 2 + 1 - k % 2].ToString("00")];
				element[order[k].ToString("00")].Right = element[order[k / 2 * 2 + 1 - k % 2].ToString("00")];
			}
			Element childDown = (element.ChildRight = element["00"]);
			element.ChildDown = childDown;
			Element childUp = (element.ChildLeft = element["13"]);
			element.ChildUp = childUp;
			for (int l = 0; l < 14; l++)
			{
				element[l.ToString("00")].MouseClick += Unit_MouseClick;
				element[l.ToString("00")].MouseEnter += Unit_MouseEnter;
				element[l.ToString("00")].MouseLeave += Unit_MouseLeave;
			}
			picBack.Alpha = 0;
			picBack.BeginTransform(0.5f);
			picBack.AlphaTo(255);
			picBack.EndTransform();
			RootElement.SetVisible("main", false);
			return true;
		}

		private void Unit_MouseClick(object sender, EventArgs e)
		{
			Element element = (Element)sender;
			int num = (int)element.Value;
			if (Opens[num])
			{
				GameManager.Data.LoadAndPlaySystemSound("ok.ogg");
				StartShortcut(num);
			}
		}

		private void Unit_MouseEnter(object sender, EventArgs e)
		{
			Element element = (Element)sender;
			element.SetVisible("bar", true);
			int num = (int)element.Value;
			strInfo.ClearString();
			if (Opens[num])
			{
				strInfo.AddString(Names[num] + "\n" + Infos[num]);
			}
			GameManager.Data.LoadAndPlaySystemSound("cursor.ogg");
		}

		private void Unit_MouseLeave(object sender, EventArgs e)
		{
			Element element = (Element)sender;
			element.SetVisible("bar", false);
		}

		public override bool FinalScreen()
		{
			if (!base.FinalScreen())
			{
				return false;
			}
			for (int i = 0; i < 14; i++)
			{
				eleScreen["main"][i.ToString("00")]["bar"].Dispose();
			}
			for (int j = 0; j < eleScreen["main"].Count; j++)
			{
				eleScreen["main"][j].Dispose();
			}
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

		protected override void StartShortcut(int index)
		{
			base.StartShortcut(index);
			switch (index)
			{
			case 0:
				if (GameManager.GameState.GlobalVarDefines["Clr_Ghost"].Value == 1)
				{
					GameManager.GameState.CurrentGameState.SelectDefines["Ko_Ghost1"].Value = GameManager.GameState.CurrentGameState.SelectDefines["Ko_Ghost1"].Case.IndexOf("Yes");
				}
				break;
			case 4:
				GameManager.GameState.CurrentGameState.GameDateTime = "04/15 ??:??";
				break;
			case 12:
				GameManager.GameState.CurrentGameState.GameDateTime = "04/13 ??:??";
				break;
			case 13:
				GameManager.GameState.CurrentGameState.GameDateTime = "04/14 ??:??";
				break;
			}
		}
	}
}
