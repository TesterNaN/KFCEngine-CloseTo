using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using KidFansChannel.DataCore;
using KidFansChannel.GalCore;
using KidFansChannel.GalCore.Screens;
using KidFansChannel.Game2DCore;

namespace KidFansChannel.CloseTo.Screens
{
	public class CloseToAlbumScreen : AlbumScreen
	{
		private int _page;

		private Picture picSelectBase;

		private Picture[] picSelects = new Picture[4];

		private Picture picMarkBase;

		private Picture[] picMarks = new Picture[2];

		private Picture picArrowBase;

		private Picture[] picArrows = new Picture[2];

		private Picture[] picMenus = new Picture[8];

		private Picture picNone;

		private Picture picMain;

		private Picture picBack;

		private Picture picBlack;

		private Element eleScreen;

		private Element eleLeft;

		private Element eleRight;

		private Picture picAlbum;

		private Picture picChange;

		private String2D strNumber;

		private String2D strName;

		private CloseToMovieScreen movieScreen;

		private Picture picListBase;

		private Picture[] picLists = new Picture[5];

		private float changeTime;

		private string fileName;

		private bool listInOut;

		private bool[,] hasGroup = new bool[8, 20]
		{
			{
				true, true, true, true, true, true, true, true, true, true,
				true, true, true, true, true, true, true, true, true, true
			},
			{
				true, true, true, true, true, true, false, false, false, false,
				false, false, false, false, false, false, false, false, false, false
			},
			{
				true, true, true, true, true, true, true, true, true, true,
				true, true, true, true, false, false, false, false, false, false
			},
			{
				true, true, true, true, true, true, true, true, true, true,
				true, true, true, true, false, false, false, false, false, false
			},
			{
				true, true, true, true, true, true, true, true, true, true,
				true, true, true, true, false, false, false, false, false, false
			},
			{
				true, true, true, true, true, true, true, true, true, true,
				true, true, true, true, true, true, true, true, true, true
			},
			{
				true, true, true, true, true, true, true, true, true, true,
				true, true, true, true, true, false, false, false, false, false
			},
			{
				true, true, true, true, true, true, true, true, true, true,
				true, true, true, true, true, false, false, false, false, false
			}
		};

		private float centerBack;

		public int Page
		{
			get
			{
				return _page;
			}
		}

		public override bool DataReady
		{
			get
			{
				if (base.DataReady)
				{
					return true;
				}
				if (picSelectBase != null && picSelectBase.IsCreated && picMarkBase != null && picMarkBase.IsCreated && picArrowBase != null && picArrowBase.IsCreated && picNone != null && picNone.IsCreated && picMenus[0] != null && picMenus[0].IsCreated && picMenus[1] != null && picMenus[1].IsCreated && picMenus[2] != null && picMenus[2].IsCreated && picMenus[3] != null && picMenus[3].IsCreated && picMenus[4] != null && picMenus[4].IsCreated && picMenus[5] != null && picMenus[5].IsCreated && picMenus[6] != null && picMenus[6].IsCreated && picMenus[7] != null && picMenus[7].IsCreated && picMain != null && picMain.IsCreated && picBack != null && picBack.IsCreated && picBlack != null && picBlack.IsCreated && picListBase != null && picListBase.IsCreated)
				{
					for (int i = 0; i < 4; i++)
					{
						picSelects[i] = GameManager.Graphics.CreateSubPicture(picSelectBase, new Rectangle(i % 2 * 20, i / 2 * 20, 20, 20));
						picSelects[i].Center = new Point((i % 2 == 0) ? 70 : (-50), (i / 2 == 0) ? 50 : (-35));
					}
					for (int j = 0; j < 5; j++)
					{
						picLists[j] = GameManager.Graphics.CreateSubPicture(picListBase, new Rectangle(j * 130, 0, 130, 490));
					}
					picMarks[0] = GameManager.Graphics.CreateSubPicture(picMarkBase, new Rectangle(0, 25, 25, 25));
					picMarks[1] = GameManager.Graphics.CreateSubPicture(picMarkBase, new Rectangle(0, 0, 25, 25));
					picArrows[0] = GameManager.Graphics.CreateSubPicture(picArrowBase, new Rectangle(0, 0, 40, 40));
					picArrows[1] = GameManager.Graphics.CreateSubPicture(picArrowBase, new Rectangle(40, 0, 40, 40));
					DataReady = true;
					return true;
				}
				return false;
			}
		}

		public CloseToAlbumScreen()
		{
			movieScreen = new CloseToMovieScreen();
		}

		public override void InitData()
		{
			base.InitData();
			movieScreen.InitData();
			picSelectBase = GameManager.Data.LoadPicture("system", "select.png");
			picMarkBase = GameManager.Data.LoadPicture("system", "mark.png");
			picArrowBase = GameManager.Data.LoadPicture("system", "arrow.png");
			for (int i = 0; i < 8; i++)
			{
				picMenus[i] = GameManager.Data.LoadPicture("system", "album_page" + i + ".png");
			}
			picNone = GameManager.Data.LoadPicture("system", "album_none.png");
			picMain = GameManager.Data.LoadPicture("system", "album.png");
			picBack = GameManager.Data.LoadPicture("system", "screenback2.png");
			picListBase = GameManager.Data.LoadPicture("system", "album_menu.png");
			picBlack = GameManager.Graphics.CreatePicture(Color.Black);
			picBlack.Size = new Size(800, 600);
			Font2D font = GameManager.GalCore.GameFont.GetFont(24);
			strNumber = new String2D();
			strNumber.Font = font;
			strNumber.Location = Point.Empty;
			strNumber.LineSize = new Size(60, 40);
			strNumber.LineNum = 1;
			strNumber.DrawCharCallback = DrawChar.DrawCharBorder;
			strName = new String2D();
			strName.Font = font;
			strName.Location = Point.Empty;
			strName.LineSize = new Size(500, 40);
			strName.LineNum = 2;
			strName.DrawCharCallback = DrawChar.DrawCharBorder;
		}

		public override void FinalData()
		{
			base.FinalData();
			Picture[] array = picSelects;
			foreach (Picture picture in array)
			{
				picture.Dispose();
			}
			picSelectBase.Dispose();
			for (int j = 0; j < 2; j++)
			{
				picMarks[j].Dispose();
			}
			picMarkBase.Dispose();
			for (int k = 0; k < 2; k++)
			{
				picArrows[k].Dispose();
			}
			picArrowBase.Dispose();
			for (int l = 0; l < 8; l++)
			{
				picMenus[l].Dispose();
			}
			picNone.Dispose();
			picMain.Dispose();
			picBack.Dispose();
			picBlack.Dispose();
			for (int m = 0; m < 5; m++)
			{
				picLists[m].Dispose();
			}
			picListBase.Dispose();
		}

		public override bool InitScreen()
		{
			if (!base.InitScreen())
			{
				return false;
			}
			Element element = new Element();
			eleScreen = new Element(picBack);
			eleScreen.Add(new Element(picMain), "title", false);
			eleScreen.Add(element, "main");
			Element element2 = new Element();
			eleScreen.Add(element2, "change");
			element.Add(new Element(), "menu");
			element2.Add(new Element(), "menu");
			Element item = new Element(picBlack);
			eleScreen.Add(item, "black");
			eleScreen.SetVisible("black", false);
			RootElement = eleScreen;
			for (int i = 0; i < 20; i++)
			{
				Element element3 = new Element();
				element3.Position = new Point(30 + i % 5 * 150, 100 + i / 5 * 120);
				element3.Region = new Rectangle(0, 0, 140, 105);
				element.Add(element3, i.ToString("00"));
				element2.Add((Element)element3.Clone(), i.ToString("00"));
				for (int j = 0; j < 4; j++)
				{
					Element element4 = new Element();
					element4.Position = new Point(30 + i % 5 * 150 + j * 25, 180 + i / 5 * 120);
					element.Add(element4, i.ToString("00") + j);
					element2.Add((Element)element4.Clone(), i.ToString("00") + j);
				}
			}
			for (int k = 0; k < 20; k++)
			{
				element[k.ToString("00")].MouseClick += Unit_MouseClick;
				element[k.ToString("00")].MouseEnter += Unit_MouseEnter;
				element[k.ToString("00")].MouseLeave += Unit_MouseLeave;
			}
			Element element5 = new Element(strNumber);
			element5.Position = new Point(200, 20);
			element.Add(element5, "number");
			Element element6 = new Element(strName);
			element6.Position = new Point(280, 20);
			element.Add(element6, "name");
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
			eleScreen.Add(new Element(), "album", false);
			eleScreen["album"].Region = new Rectangle(0, 0, 800, 600);
			eleScreen["album"].Add(new Element(), "change");
			eleScreen["album"].MouseClick += Album_MouseClick;
			for (int l = 0; l < 4; l++)
			{
				element.Add(new Element(picSelects[l]), "select" + l);
			}
			Element element7 = new Element();
			for (int m = 0; m < 5; m++)
			{
				Element element8 = new Element(picLists[m]);
				element8.Region = new Rectangle(0, 0, 130, 490);
				element8.Position = new Point(45 + m * 145, 80);
				element8.Value = m;
				element8.MouseClick += List_MouseClick;
				element8.MouseEnter += List_MouseEnter;
				element8.MouseLeave += List_MouseLeave;
				element7.Add(element8, "list" + m);
				picLists[m].Alpha = 128;
			}
			for (int n = 0; n < 5; n++)
			{
				element7["list" + n].Left = element7["list" + (n + 4) % 5];
				element7["list" + n].Right = element7["list" + (n + 1) % 5];
			}
			element7.ChildLeft = element7["list4"];
			element7.ChildRight = element7["list0"];
			eleScreen.Add(element7, "listpage");
			eleScreen.SetVisible("listpage", false);
			element7.Focused = true;
			FocusedElement = element7;
			picBack.Alpha = 0;
			picBack.BeginTransform(0.5f);
			picBack.AlphaTo(255);
			picBack.EndTransform();
			RootElement.SetVisible("main", false);
			GameManager.Sound.StopBgm();
			return true;
		}

		private void List_MouseEnter(object sender, EventArgs e)
		{
			Element element = (Element)sender;
			if (!listInOut)
			{
				int num = (int)element.Value;
				picLists[num].BeginTransform(0.5f);
				picLists[num].AlphaTo(255);
				picLists[num].EndTransform();
				GameManager.Data.LoadAndPlaySystemSound("cursor.ogg");
			}
		}

		private void List_MouseLeave(object sender, EventArgs e)
		{
			Element element = (Element)sender;
			if (!listInOut)
			{
				int num = (int)element.Value;
				picLists[num].BeginTransform(0.5f);
				picLists[num].AlphaTo(128);
				picLists[num].EndTransform();
			}
		}

		private void List_MouseClick(object sender, EventArgs e)
		{
			Element element = (Element)sender;
			if (!listInOut)
			{
				int num = (int)element.Value;
				int[] array = new int[5] { 0, 2, 3, 4, 5 };
				_page = array[num];
				UpdatePage("main");
				for (int i = 0; i < 5; i++)
				{
					picLists[i].BeginTransform(0.5f);
					picLists[i].AlphaTo(0);
					picLists[i].EndTransform();
				}
				listInOut = true;
				element.ClearSelected();
				GameManager.Data.LoadAndPlaySystemSound("ok.ogg");
			}
		}

		private void Album_MouseClick(object sender, EventArgs e)
		{
			if (picChange != null)
			{
				return;
			}
			int num = (int)Math.Floor((float)((Element)sender).Value) / 10;
			int num2 = (int)Math.Floor((float)((Element)sender).Value) % 10;
			int num3 = num2;
			if ((double)(float)((Element)sender).Value > Math.Floor((float)((Element)sender).Value))
			{
				GameManager.Data.LoadAndPlaySystemSound("ok.ogg");
				fileName = Files[_page][num][num3] + ".png";
				picChange = GameManager.Data.LoadPictureInstant("bg", fileName);
				picChange.Scale = new SizeF(Math.Max(800f / (float)picChange.Width, 600f / (float)picChange.Height), Math.Max(800f / (float)picChange.Width, 600f / (float)picChange.Height));
				eleScreen["album"]["change"].ElementPicture = picChange;
				picChange.Alpha = 0;
				picChange.BeginTransform(0.5f);
				picChange.AlphaTo(255);
				picChange.EndTransform();
				eleScreen["album"].Value = (float)(num * 10 + num3);
				return;
			}
			for (int i = num2 + 1; i < Opens[_page][num].Length; i++)
			{
				if (Opens[_page][num][i])
				{
					num3 = i;
					break;
				}
			}
			if (num3 == num2)
			{
				fileName = null;
				picChange = picAlbum;
				picAlbum = null;
				eleScreen["album"].ElementPicture = null;
				eleScreen["album"]["change"].ElementPicture = picChange;
				picChange.Alpha = 255;
				picChange.BeginTransform(0.5f);
				picChange.AlphaTo(0);
				picChange.EndTransform();
				eleScreen.SetVisible("black", false);
				picBlack.Alpha = 255;
				picBlack.BeginTransform(0.5f);
				picBlack.AlphaTo(0);
				picBlack.EndTransform();
			}
			else
			{
				GameManager.Data.LoadAndPlaySystemSound("ok.ogg");
				fileName = Files[_page][num][num3] + ".png";
				picChange = GameManager.Data.LoadPictureInstant("bg", fileName);
				picChange.Scale = new SizeF(Math.Min(800f / (float)picChange.Width, 600f / (float)picChange.Height), Math.Min(800f / (float)picChange.Width, 600f / (float)picChange.Height));
				picChange.Position = new Point((int)((800f - (float)picChange.Width * picChange.Scale.Width) / 2f), (int)((600f - (float)picChange.Height * picChange.Scale.Height) / 2f));
				eleScreen["album"]["change"].ElementPicture = picChange;
				picChange.Alpha = 0;
				picChange.BeginTransform(0.5f);
				picChange.AlphaTo(255);
				picChange.EndTransform();
				eleScreen["album"].Value = (float)(num * 10 + num3);
			}
		}

		private void Unit_MouseClick(object sender, EventArgs e)
		{
			if (changeTime > 0f)
			{
				return;
			}
			Element element = (Element)sender;
			int num = (int)element.Value;
			if (_page == 7)
			{
				if (movieScreen.GetOpens()[num])
				{
					movieScreen.Play(num);
				}
			}
			else
			{
				if (!GroupOpens[_page][num])
				{
					return;
				}
				GameManager.Data.LoadAndPlaySystemSound("ok.ogg");
				int num2 = 0;
				for (int i = 0; i < Opens[_page][num].Length; i++)
				{
					if (Opens[_page][num][i])
					{
						num2 = i;
						break;
					}
				}
				fileName = Files[_page][num][num2] + ".png";
				picChange = GameManager.Data.LoadPictureInstant("bg", fileName);
				picChange.Scale = new SizeF(Math.Min(800f / (float)picChange.Width, 600f / (float)picChange.Height), Math.Min(800f / (float)picChange.Width, 600f / (float)picChange.Height));
				picChange.Position = new Point((int)((800f - (float)picChange.Width * picChange.Scale.Width) / 2f), (int)((600f - (float)picChange.Height * picChange.Scale.Height) / 2f));
				eleScreen["album"]["change"].ElementPicture = picChange;
				eleScreen.SetVisible("album", true);
				picChange.Alpha = 0;
				picChange.BeginTransform(0.5f);
				picChange.AlphaTo(255);
				picChange.EndTransform();
				eleScreen.SetVisible("black", true);
				picBlack.Alpha = 0;
				picBlack.BeginTransform(0.5f);
				picBlack.AlphaTo(255);
				picBlack.EndTransform();
				if (800f / (float)picChange.Width == 600f / (float)picChange.Height)
				{
					eleScreen["album"].Value = (float)(num * 10 + num2);
				}
				else
				{
					eleScreen["album"].Value = (float)(num * 10 + num2) + 0.5f;
				}
				FocusedElement = eleScreen;
				eleScreen.Focused = true;
				eleScreen["main"].Focused = false;
				eleScreen["album"].Selected = true;
				eleScreen["main"].SetVisible("number", false);
				eleScreen["main"].SetVisible("name", false);
			}
		}

		private void Unit_MouseEnter(object sender, EventArgs e)
		{
			if (changeTime > 0f)
			{
				return;
			}
			Element element = (Element)sender;
			for (int i = 0; i < 4; i++)
			{
				picSelects[i].BeginTransform(0.5f);
				picSelects[i].PositionTo(new Point(element.Position.X + 70, element.Position.Y + 50));
				picSelects[i].ScaleTo(new SizeF(1f, 1f), new SizeF(1.1f, 1.1f), 0.5f);
				picSelects[i].EndTransform();
			}
			int num = (int)element.Value;
			if (_page == 7)
			{
				if (movieScreen.GetOpens()[num])
				{
					strNumber.ClearString();
					strNumber.AddString(num + 1 + " :", ContentAlignment.TopRight);
					strName.ClearString();
					strName.AddString(movieScreen.GetName(num));
				}
			}
			else if (GroupOpens[_page][num])
			{
				strNumber.ClearString();
				strNumber.AddString(num + 1 + " :", ContentAlignment.TopRight);
				strName.ClearString();
				strName.AddString(Names[_page][num]);
			}
			GameManager.Data.LoadAndPlaySystemSound("cursor.ogg");
		}

		private void Unit_MouseLeave(object sender, EventArgs e)
		{
			for (int i = 0; i < 4; i++)
			{
				picSelects[i].BeginTransform(0.5f);
				picSelects[i].PositionTo(new Point(picSelects[i].CenterX + ((i % 2 == 0) ? (-20) : 800), picSelects[i].CenterY + ((i / 2 == 0) ? (-20) : 600)));
				picSelects[i].EndTransform();
			}
			strNumber.ClearString();
			strName.ClearString();
		}

		private void Left_MouseClick(object sender, EventArgs e)
		{
			MoveLeftScreen();
		}

		private void MoveLeftScreen()
		{
			if (!(picBack.Time > 0f) && !(changeTime > 0f))
			{
				UpdatePage("change");
				_page = (_page + 7) % 8;
				eleScreen["main"].SetVisible("left", false);
				eleScreen["main"].SetVisible("right", false);
				eleScreen.SetVisible("change", true);
				UpdatePage("main");
				eleScreen["main"].Position = new Point(-800, 0);
				eleScreen["change"].Position = new Point(0, 0);
				changeTime = 0.5f;
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
			picArrows[0].BeginTransform(1f);
			picArrows[0].PositionTo(new Point(0, 0));
			picArrows[0].EndTransform();
		}

		private void Right_MouseClick(object sender, EventArgs e)
		{
			MoveRightScreen();
		}

		private void MoveRightScreen()
		{
			if (!(picBack.Time > 0f) && !(changeTime > 0f))
			{
				UpdatePage("change");
				_page = (_page + 1) % 8;
				eleScreen["main"].SetVisible("left", false);
				eleScreen["main"].SetVisible("right", false);
				eleScreen.SetVisible("change", true);
				UpdatePage("main");
				eleScreen["main"].Position = new Point(800, 0);
				eleScreen["change"].Position = new Point(0, 0);
				changeTime = 0.5f;
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

		private void UpdatePage(string screen)
		{
			eleScreen[screen]["menu"].ElementPicture = picMenus[_page];
			strNumber.ClearString();
			strName.ClearString();
			int num = 0;
			int num2 = -1;
			int num3 = 0;
			for (int i = 0; i < 20; i++)
			{
				if (hasGroup[_page, i])
				{
					if (num2 == -1)
					{
						num2 = i;
					}
					num3 = i;
					eleScreen[screen].SetVisible(i.ToString("00"), true);
					eleScreen[screen][i.ToString("00")].ElementPicture = (((_page == 7) ? movieScreen.GetOpens()[num] : GroupOpens[_page][num]) ? null : picNone);
					eleScreen[screen][i.ToString("00")].Value = num;
					if (_page == 7)
					{
						eleScreen[screen][i.ToString("00") + "0"].ElementPicture = (((_page == 7) ? movieScreen.GetOpens()[num] : GroupOpens[_page][num]) ? picMarks[0] : picMarks[1]);
						for (int j = 1; j < 4; j++)
						{
							eleScreen[screen][i.ToString("00") + j].ElementPicture = null;
						}
					}
					else
					{
						for (int k = 0; k < Opens[_page][num].Length; k++)
						{
							eleScreen[screen][i.ToString("00") + k].ElementPicture = (Opens[_page][num][k] ? picMarks[0] : picMarks[1]);
						}
						for (int l = Opens[_page][num].Length; l < 4; l++)
						{
							eleScreen[screen][i.ToString("00") + l].ElementPicture = null;
						}
					}
					num++;
				}
				else
				{
					eleScreen[screen].SetVisible(i.ToString("00"), false);
					eleScreen[screen][i.ToString("00")].Value = -1;
					for (int m = 0; m < 4; m++)
					{
						eleScreen[screen][i.ToString("00") + m].ElementPicture = null;
					}
				}
			}
			for (int n = 0; n < 20; n++)
			{
				if (hasGroup[_page, n])
				{
					int num4 = (n + 15) % 20;
					int num5 = (n + 5) % 20;
					int num6 = (n + 4) % 5 + n / 5 * 5;
					int num7 = (n + 1) % 5 + n / 5 * 5;
					while (!eleScreen[screen].GetVisible(num4.ToString("00")))
					{
						num4 = (num4 + 15) % 20;
					}
					eleScreen[screen][n.ToString("00")].Up = eleScreen[screen][num4.ToString("00")];
					while (!eleScreen[screen].GetVisible(num5.ToString("00")))
					{
						num5 = (num5 + 5) % 20;
					}
					eleScreen[screen][n.ToString("00")].Down = eleScreen[screen][num5.ToString("00")];
					while (!eleScreen[screen].GetVisible(num6.ToString("00")))
					{
						num6 = (num6 + 4) % 5 + num6 / 5 * 5;
					}
					eleScreen[screen][n.ToString("00")].Left = eleScreen[screen][num6.ToString("00")];
					while (!eleScreen[screen].GetVisible(num7.ToString("00")))
					{
						num7 = (num7 + 1) % 5 + num7 / 5 * 5;
					}
					eleScreen[screen][n.ToString("00")].Right = eleScreen[screen][num7.ToString("00")];
				}
				else
				{
					Element element = eleScreen[screen][n.ToString("00")];
					Element element2 = eleScreen[screen][n.ToString("00")];
					Element element3 = eleScreen[screen][n.ToString("00")];
					Element element4 = (eleScreen[screen][n.ToString("00")].Right = null);
					Element element6 = (element3.Left = element4);
					Element up = (element2.Down = element6);
					element.Up = up;
				}
			}
			for (int num8 = 0; num8 < 4; num8++)
			{
				picSelects[num8].Position = new Point(picSelects[num8].CenterX + ((num8 % 2 == 0) ? (-20) : 800), picSelects[num8].CenterY + ((num8 / 2 == 0) ? (-20) : 600));
			}
			eleScreen[screen].ClearSelected();
			Element element9 = eleScreen[screen];
			Element childRight = (eleScreen[screen].ChildDown = eleScreen[screen][num2.ToString("00")]);
			element9.ChildRight = childRight;
			Element element11 = eleScreen[screen];
			Element childLeft = (eleScreen[screen].ChildUp = eleScreen[screen][num3.ToString("00")]);
			element11.ChildLeft = childLeft;
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
			if (eleScreen.GetVisible("album") && picChange != null && picChange.Time == 0f)
			{
				if (picChange.Alpha == 255)
				{
					if (picAlbum != null)
					{
						picAlbum.Dispose();
					}
					picAlbum = picChange;
					picChange = null;
					eleScreen["album"].ElementPicture = picAlbum;
					eleScreen["album"]["change"].ElementPicture = null;
				}
				else
				{
					picChange.Dispose();
					picChange = null;
					eleScreen["album"]["change"].ElementPicture = null;
					eleScreen.SetVisible("album", false);
					eleScreen["album"].Selected = false;
					FocusedElement = eleScreen["main"];
					eleScreen["main"].Focused = true;
					eleScreen.Focused = false;
					int num = -1;
					for (int i = 0; i < 20; i++)
					{
						if ((int)eleScreen["main"][i.ToString("00")].Value == (int)Math.Floor((float)eleScreen["album"].Value) / 10)
						{
							num = i;
						}
					}
					if (num != -1)
					{
						eleScreen["main"][num.ToString("00")].Selected = true;
					}
					eleScreen["main"].SetVisible("number", true);
					eleScreen["main"].SetVisible("name", true);
				}
			}
			if (eleScreen.GetVisible("change"))
			{
				if (changeTime > 0f)
				{
					changeTime -= elaspedTime;
					if (changeTime < 0f)
					{
						changeTime = 0f;
					}
					if (eleScreen["main"].Position.X < 0)
					{
						eleScreen["main"].Position = new Point(-(int)(changeTime * 1600f), 0);
						eleScreen["change"].Position = new Point(-(int)(changeTime * 1600f) + 800, 0);
					}
					else
					{
						eleScreen["main"].Position = new Point((int)(changeTime * 1600f), 0);
						eleScreen["change"].Position = new Point((int)(changeTime * 1600f) - 800, 0);
					}
				}
				if (changeTime == 0f)
				{
					eleScreen["main"].SetVisible("left", true);
					eleScreen["main"].SetVisible("right", true);
					UpdatePage("main");
					eleScreen.SetVisible("change", false);
				}
			}
			if (listInOut && picLists[0].Time == 0f)
			{
				if (picLists[0].Alpha == 0)
				{
					eleScreen.SetVisible("listpage", false);
					eleScreen.SetVisible("main", true);
					eleScreen["listpage"].Focused = false;
					eleScreen["main"].Focused = true;
					FocusedElement = eleScreen["main"];
				}
				listInOut = false;
			}
			if (picBack.Time == 0f && !eleScreen.GetVisible("main"))
			{
				if (picBack.Alpha == 255)
				{
					eleScreen.SetVisible("listpage", true);
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
				if ((!eleScreen.GetVisible("main") && !eleScreen.GetVisible("listpage")) || listInOut || picChange != null)
				{
					return;
				}
				if (picAlbum != null)
				{
					picChange = picAlbum;
					picAlbum = null;
					eleScreen["album"].ElementPicture = null;
					eleScreen["album"]["change"].ElementPicture = picChange;
					picChange.Alpha = 255;
					picChange.BeginTransform(0.5f);
					picChange.AlphaTo(0);
					picChange.EndTransform();
					eleScreen.SetVisible("black", false);
					picBlack.Alpha = 255;
					picBlack.BeginTransform(0.5f);
					picBlack.AlphaTo(0);
					picBlack.EndTransform();
					GameManager.Data.LoadAndPlaySystemSound("cancel.ogg");
				}
				else if (FocusedElement == eleScreen["main"])
				{
					eleScreen.SetVisible("listpage", true);
					eleScreen.SetVisible("main", false);
					eleScreen["listpage"].Focused = true;
					eleScreen["main"].Focused = false;
					FocusedElement = eleScreen["listpage"];
					for (int i = 0; i < 5; i++)
					{
						picLists[i].BeginTransform(0.5f);
						picLists[i].AlphaTo(128);
						picLists[i].EndTransform();
					}
					listInOut = true;
					GameManager.Data.LoadAndPlaySystemSound("cancel.ogg");
				}
				else
				{
					eleScreen.SetVisible("listpage", false);
					eleScreen.SetVisible("title", false);
					picBack.BeginTransform(0.5f);
					picBack.AlphaTo(0);
					picBack.EndTransform();
					GameManager.Data.LoadAndPlaySystemSound("cancel.ogg");
				}
				return;
			}
			base.OnKeyPress(keyData);
			switch (keyData)
			{
			case Keys.Up:
				if (eleScreen.GetVisible("album") && picAlbum != null && (float)picAlbum.Height * picAlbum.ScaleY > 800f)
				{
					picAlbum.Y += 200;
					if (picAlbum.Y > 0)
					{
						picAlbum.Y = 0;
					}
				}
				break;
			case Keys.Down:
				if (eleScreen.GetVisible("album") && picAlbum != null && (float)picAlbum.Height * picAlbum.ScaleY > 800f)
				{
					picAlbum.Y -= 200;
					if ((float)picAlbum.Y + (float)picAlbum.Height * picAlbum.ScaleY < 600f)
					{
						picAlbum.Y = (int)(600f - (float)picAlbum.Height * picAlbum.ScaleY);
					}
				}
				break;
			case Keys.Left:
				if (eleScreen.GetVisible("album") && picAlbum != null && (float)picAlbum.Width * picAlbum.ScaleX > 800f)
				{
					picAlbum.X += 200;
					if (picAlbum.X > 0)
					{
						picAlbum.X = 0;
					}
				}
				break;
			case Keys.Right:
				if (eleScreen.GetVisible("album") && picAlbum != null && (float)picAlbum.Width * picAlbum.ScaleX > 800f)
				{
					picAlbum.X -= 200;
					if ((float)picAlbum.X + (float)picAlbum.Width * picAlbum.ScaleX < 800f)
					{
						picAlbum.X = (int)(800f - (float)picAlbum.Width * picAlbum.ScaleX);
					}
				}
				break;
			case Keys.Prior:
				MoveLeftScreen();
				break;
			case Keys.Next:
				MoveRightScreen();
				break;
			case Keys.F5:
			{
				if (!eleScreen.GetVisible("album") || picAlbum == null || !GameManager.Graphics.IsWindowed)
				{
					break;
				}
				using (SaveFileDialog saveFileDialog = new SaveFileDialog())
				{
					saveFileDialog.AddExtension = true;
					try
					{
						UpgradeDialog(saveFileDialog);
					}
					catch (Exception)
					{
					}
					saveFileDialog.DefaultExt = "png";
					saveFileDialog.Filter = "PNG Image|*.png";
					saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
					saveFileDialog.FileName = fileName;
					saveFileDialog.OverwritePrompt = true;
					saveFileDialog.RestoreDirectory = true;
					saveFileDialog.ValidateNames = true;
					if (saveFileDialog.ShowDialog() != DialogResult.OK)
					{
						break;
					}
					switch (GameManager.Data.CheckData("bg", fileName, KidFansChannel.DataCore.DataCore.DataType.Package))
					{
					case KidFansChannel.DataCore.DataCore.DataType.Package:
					{
						using (FileStream fileStream = new FileStream(saveFileDialog.FileName, FileMode.Create))
						{
							using (Stream stream = GameManager.Data.GetStream())
							{
								byte[] array = new byte[stream.Length];
								stream.Read(array, 0, array.Length);
								fileStream.Write(array, 0, array.Length);
								break;
							}
						}
					}
					case KidFansChannel.DataCore.DataCore.DataType.File:
						File.Copy(GameManager.Data.GetFile(), saveFileDialog.FileName);
						break;
					}
					break;
				}
			}
			}
		}

		private void UpgradeDialog(SaveFileDialog sfd)
		{
			sfd.AutoUpgradeEnabled = true;
			sfd.CustomPlaces.Add(Path.Combine(GameManager.GameState.SaveFullPath, "Snaps"));
		}

		public override void OnMouseWheel(int delta)
		{
			base.OnMouseWheel(delta);
			if (!eleScreen.GetVisible("album") || picChange != null)
			{
				return;
			}
			if ((float)picAlbum.Width * picAlbum.ScaleX > 800f)
			{
				if (delta > 0)
				{
					picAlbum.X += delta / 120 * 200;
					if (picAlbum.X > 0)
					{
						picAlbum.X = 0;
					}
				}
				else
				{
					picAlbum.X += delta / 120 * 200;
					if ((float)picAlbum.X + (float)picAlbum.Width * picAlbum.ScaleX < 800f)
					{
						picAlbum.X = (int)(800f - (float)picAlbum.Width * picAlbum.ScaleX);
					}
				}
			}
			else
			{
				if (!((float)picAlbum.Height * picAlbum.ScaleY > 600f))
				{
					return;
				}
				if (delta > 0)
				{
					picAlbum.Y += delta / 120 * 200;
					if (picAlbum.Y > 0)
					{
						picAlbum.Y = 0;
					}
				}
				else
				{
					picAlbum.Y += delta / 120 * 200;
					if ((float)picAlbum.Y + (float)picAlbum.Height * picAlbum.ScaleY < 600f)
					{
						picAlbum.Y = (int)(600f - (float)picAlbum.Height * picAlbum.ScaleY);
					}
				}
			}
		}
	}
}
