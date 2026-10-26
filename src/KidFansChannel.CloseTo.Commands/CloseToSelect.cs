using System;
using System.Drawing;
using System.Xml;
using KidFansChannel.CloseTo.Commands.Map;
using KidFansChannel.CloseTo.Commands.Room;
using KidFansChannel.GalCore;
using KidFansChannel.GalCore.Commands;
using KidFansChannel.Game2DCore;

namespace KidFansChannel.CloseTo.Commands
{
	public class CloseToSelect : Select, IInstanceDataUser
	{
		private MapSelect _map;

		private RoomSelect _room;

		private static int currentColorIndex;

		private static string currentColor;

		private static Picture selectBar;

		private static Picture iconBase;

		private static SubPicture[] icons = new SubPicture[5];

		private static AnimePicture iconAnime;

		private bool lastShowHint;

		private int labelId;

		protected int lastSelected = -1;

		protected float iconTime;

		protected int iconPasses;

		protected bool iconDirection;

		protected float iconX;

		private string hintChara;

		private string[] _hints;

		private Picture hintBase;

		private Picture[] hintPics = new Picture[4];

		private AnimePicture[] hintAnimes = new AnimePicture[2];

		private bool hintCreated;

		public MapSelect Map
		{
			get
			{
				return _map;
			}
		}

		internal RoomSelect Room
		{
			get
			{
				return _room;
			}
			set
			{
				_room = value;
			}
		}

		public string[] Hints
		{
			get
			{
				return _hints;
			}
			set
			{
				_hints = value;
			}
		}

		public bool DataReady
		{
			get
			{
				if (_map != null)
				{
					return _map.DataReady;
				}
				if (_room != null)
				{
					return _room.DataReady;
				}
				if (!hintCreated)
				{
					if (hintChara == null)
					{
						hintCreated = true;
						return true;
					}
					if (hintBase.IsCreated)
					{
						for (int i = 0; i < 4; i++)
						{
							hintPics[i] = ScriptCommand.Graphics.CreateSubPicture(hintBase, new Rectangle(i % 2 * 80, i / 2 * 80, 80, 80));
						}
						for (int j = 0; j < 2; j++)
						{
							hintAnimes[j] = ScriptCommand.Graphics.CreateAnimePicture(new Size(80, 80));
							hintAnimes[j].AddFrame(hintPics[j * 2], 0.5f);
							hintAnimes[j].AddFrame(hintPics[j * 2 + 1], 0.5f);
							hintAnimes[j].Center = new Point(40, 40);
						}
						hintCreated = true;
						return true;
					}
					return false;
				}
				return true;
			}
		}

		public override bool CanHide
		{
			get
			{
				if (_map != null)
				{
					return false;
				}
				if (_room != null)
				{
					return false;
				}
				return base.CanHide;
			}
		}

		public CloseToSelect()
		{
			Select.SelectCenterY = 245;
		}

		protected void GameFont_FontChanged(object sender, EventArgs e)
		{
			foreach (String2D itemString in ItemStrings)
			{
				if (itemString != null && itemString.Font != null)
				{
					itemString.Font = ScriptCommand.GalCore.GameFont.GetFont(24);
				}
			}
		}

		protected override void SetBackStyle()
		{
		}

		protected override void SetFontStyle(String2D itemString, bool chosen)
		{
			itemString.Font = ScriptCommand.GalCore.GameFont.GetFont(24);
			itemString.LineSize = new Size(720, 40);
			itemString.LineNum = 1;
			if (ScriptCommand.Options.SelectedShow && chosen)
			{
				itemString.DefaultColor = Color.FromArgb(96, 96, 96);
			}
			else
			{
				itemString.DefaultColor = Color.White;
			}
			itemString.DrawCharCallback = DrawChar.DrawCharShadow;
		}

		public override void InitStaticData()
		{
			ScriptCommand.GalCore.GameFont.FontChanged += GameFont_FontChanged;
			selectBar = ScriptCommand.Data.LoadPictureInstant("system", "select_bar.png");
			selectBar.Center = new Point(80, 20);
			selectBar.X = 400;
			iconBase = ScriptCommand.Data.LoadPictureInstant("system", "click.png");
			for (int i = 0; i < 5; i++)
			{
				icons[i] = ScriptCommand.Graphics.CreateSubPicture(iconBase, new Rectangle(i % 3 * 40, i / 3 * 40, 40, 40));
			}
			iconAnime = ScriptCommand.Graphics.CreateAnimePicture(new Size(40, 40));
			for (int j = 0; j < 5; j++)
			{
				iconAnime.AddFrame(icons[j], 1f / 12f);
			}
			iconAnime.Center = new Point(20, 20);
			currentColor = ScriptCommand.Options.DialogueColor;
			GetColorIndex();
			labelId = Label.GenerateId();
		}

		public override void FinalStaticData()
		{
			selectBar.Dispose();
			selectBar = null;
			iconAnime.Dispose();
			for (int i = 0; i < 5; i++)
			{
				icons[i].Dispose();
				icons[i] = null;
			}
			iconBase.Dispose();
			iconBase = null;
			Label.ReleaseId(labelId);
		}

		protected override void Show()
		{
			base.Show();
			if (_map != null)
			{
				_map.Show();
				return;
			}
			if (_room != null)
			{
				_room.Show();
				return;
			}
			int boxHeight = ItemStrings.Count * 40 + 50;
			int boxWidth = 80;
			for (int i = 0; i < ItemStrings.Count; i++)
			{
				if (ItemStrings[i].Width + 80 > boxWidth)
				{
					boxWidth = ItemStrings[i].Width + 80;
				}
			}
			if (hintChara != null && ScriptCommand.Options.HintShow)
			{
				selectBar.X = 350;
				boxWidth += 100;
				for (int j = 0; j < ItemStrings.Count; j++)
				{
					ItemStrings[j].Location = new Point(ItemStrings[j].Location.X - 50, ItemStrings[j].Location.Y);
				}
				lastShowHint = true;
			}
			else
			{
				selectBar.X = 400;
			}
			Label.CurrentId = labelId;
			for (int k = 0; k < 9; k++)
			{
				Label.PicBacks[0, k].Alpha = (int)(ScriptCommand.Options.DialogueAlpha * 255f);
			}
			for (int l = 0; l < 5; l++)
			{
				Label.PicBacks[l, 0].Center = new Point(boxWidth / 2, boxHeight / 2);
				Label.PicBacks[l, 1].Center = new Point(10, boxHeight / 2);
				Label.PicBacks[l, 2].Center = new Point(30 - boxWidth / 2, boxHeight / 2);
				Label.PicBacks[l, 3].Center = new Point(boxWidth / 2, 5);
				Label.PicBacks[l, 4].Center = new Point(10, 5);
				Label.PicBacks[l, 5].Center = new Point(30 - boxWidth / 2, 5);
				Label.PicBacks[l, 6].Center = new Point(boxWidth / 2, 25 - boxHeight / 2);
				Label.PicBacks[l, 7].Center = new Point(10, 25 - boxHeight / 2);
				Label.PicBacks[l, 8].Center = new Point(30 - boxWidth / 2, 25 - boxHeight / 2);
				for (int m = 0; m < 9; m++)
				{
					Label.PicBacks[l, m].Position = new Point(400, Select.SelectCenterY);
					Label.PicBacks[l, m].Scale = SizeF.Empty;
				}
				for (int n = 0; n < 9; n++)
				{
					Label.PicBacks[l, n].BeginTransform(0.1f);
				}
				Label.PicBacks[l, 0].ScaleTo(new SizeF(1f, 1f));
				Label.PicBacks[l, 1].ScaleTo(new SizeF((float)(boxWidth - 60) / 20f, 1f));
				Label.PicBacks[l, 2].ScaleTo(new SizeF(1f, 1f));
				Label.PicBacks[l, 3].ScaleTo(new SizeF(1f, (float)(boxHeight - 50) / 10f));
				Label.PicBacks[l, 4].ScaleTo(new SizeF((float)(boxWidth - 60) / 20f, (float)(boxHeight - 50) / 10f));
				Label.PicBacks[l, 5].ScaleTo(new SizeF(1f, (float)(boxHeight - 50) / 10f));
				Label.PicBacks[l, 6].ScaleTo(new SizeF(1f, 1f));
				Label.PicBacks[l, 7].ScaleTo(new SizeF((float)(boxWidth - 60) / 20f, 1f));
				Label.PicBacks[l, 8].ScaleTo(new SizeF(1f, 1f));
				for (int num3 = 0; num3 < 9; num3++)
				{
					Label.PicBacks[l, num3].EndTransform();
				}
			}
		}

		protected override void Hide()
		{
			base.Hide();
			if (_map != null)
			{
				_map.Hide();
				return;
			}
			if (_room != null)
			{
				_room.Hide();
				return;
			}
			Label.CurrentId = labelId;
			Picture[,] picBacks = Label.PicBacks;
			foreach (Picture picture in picBacks)
			{
				picture.BeginTransform(0.1f);
				picture.ScaleTo(SizeF.Empty);
				picture.EndTransform();
			}
		}

		protected override void UpdateBack(float elaspedTime)
		{
			if (_map != null)
			{
				_map.UpdateBack(elaspedTime);
				return;
			}
			if (_room != null)
			{
				_room.UpdateBack(elaspedTime);
				return;
			}
			Label.CurrentId = labelId;
			Picture[,] picBacks = Label.PicBacks;
			foreach (Picture picture in picBacks)
			{
				picture.Update(elaspedTime);
			}
		}

		protected override void UpdateItem(float elaspedTime)
		{
			if (_map != null)
			{
				_map.UpdateItem(elaspedTime);
				return;
			}
			if (_room != null)
			{
				_room.UpdateItem(elaspedTime);
				return;
			}
			iconAnime.Update(elaspedTime);
			if (hintChara != null && lastShowHint != ScriptCommand.Options.HintShow)
			{
				lastShowHint = ScriptCommand.Options.HintShow;
				int height = ItemStrings.Count * 40 + 50;
				int boxHeight = 80;
				for (int i = 0; i < ItemStrings.Count; i++)
				{
					if (ItemStrings[i].Width + 80 > boxHeight)
					{
						boxHeight = ItemStrings[i].Width + 80;
					}
				}
				if (lastShowHint)
				{
					selectBar.X = 350;
					boxHeight += 100;
					for (int j = 0; j < ItemStrings.Count; j++)
					{
						ItemStrings[j].Location = new Point(ItemStrings[j].Location.X - 50, ItemStrings[j].Location.Y);
					}
				}
				else
				{
					selectBar.X = 400;
					for (int k = 0; k < ItemStrings.Count; k++)
					{
						ItemStrings[k].Location = new Point(ItemStrings[k].Location.X + 50, ItemStrings[k].Location.Y);
					}
				}
				Label.CurrentId = labelId;
				Label.SetBackSize(boxHeight, height);
			}
			if (lastSelected != CurrentSelected)
			{
				if (CurrentSelected != -1)
				{
					if (lastSelected == -1)
					{
						selectBar.Scale = new SizeF((float)(ItemStrings[CurrentSelected].Width + 80) / (float)selectBar.Width, 1f);
						selectBar.Position = new Point(selectBar.Position.X, ItemStrings[CurrentSelected].Location.Y + 18);
						selectBar.Alpha = 0;
						selectBar.BeginTransform(0.1f);
						selectBar.AlphaTo(255);
						selectBar.EndTransform();
						iconTime = 0.5f;
						iconPasses = 1;
						iconDirection = true;
						iconAnime.Position = new Point(400, ItemStrings[CurrentSelected].Location.Y + 18);
						iconX = -ItemStrings[CurrentSelected].Width / 2 - 15;
					}
					else
					{
						selectBar.BeginTransform(0.1f);
						selectBar.ScaleTo(new SizeF((float)(ItemStrings[CurrentSelected].Width + 80) / (float)selectBar.Width, 1f));
						selectBar.PositionTo(new Point(selectBar.Position.X, ItemStrings[CurrentSelected].Location.Y + 18));
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
						iconAnime.BeginTransform(iconTime);
						iconAnime.PositionTo(new Point(400, ItemStrings[CurrentSelected].Location.Y + 18));
						iconAnime.EndTransform();
					}
				}
				else
				{
					iconTime = 0f;
					iconPasses = 0;
					iconDirection = false;
					iconX = -ItemStrings[lastSelected].Width / 2 - 15;
				}
				if (CurrentSelected != -1)
				{
					int boxWidth = 80;
					for (int l = 0; l < ItemStrings.Count; l++)
					{
						if (ItemStrings[l].Width + 80 > boxWidth)
						{
							boxWidth = ItemStrings[l].Width + 80;
						}
					}
					if (hintChara != null)
					{
						for (int m = 0; m < 2; m++)
						{
							hintAnimes[m].Position = new Point(BgDisplay.ScreenSize.Width / 2 + boxWidth / 2 - 30, Select.SelectCenterY);
						}
					}
				}
				lastSelected = CurrentSelected;
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
					iconX = (iconDirection ? (ItemStrings[CurrentSelected].Width / 2 + 15) : (-(ItemStrings[CurrentSelected].Width / 2 + 15)));
				}
				else if (flag)
				{
					iconX = (float)(ItemStrings[CurrentSelected].Width + 30) * (0.5f - iconTime) / 0.5f;
					if (iconDirection)
					{
						iconX = iconX - (float)(ItemStrings[CurrentSelected].Width / 2) - 15f;
					}
					else
					{
						iconX = (float)(ItemStrings[CurrentSelected].Width / 2 + 15) - iconX;
					}
				}
				else
				{
					iconX += ((float)(iconDirection ? (ItemStrings[CurrentSelected].Width / 2 + 15) : (-(ItemStrings[CurrentSelected].Width / 2 + 15))) - iconX) * elaspedTime / (iconTime + elaspedTime);
				}
			}
			selectBar.Update(elaspedTime);
			if (hintBase != null)
			{
				for (int n = 0; n < 2; n++)
				{
					hintAnimes[n].Update(elaspedTime);
				}
			}
		}

		protected override bool CheckShown()
		{
			if (_map != null)
			{
				return _map.CheckShown();
			}
			if (_room != null)
			{
				return _room.CheckShown();
			}
			Label.CurrentId = labelId;
			if (Label.PicBacks[0, 0].Time == 0f)
			{
				return Label.PicBacks[0, 0].Scale == new SizeF(1f, 1f);
			}
			return false;
		}

		protected override bool CheckHiden()
		{
			if (_map != null)
			{
				return _map.CheckHiden();
			}
			if (_room != null)
			{
				return _room.CheckHiden();
			}
			Label.CurrentId = labelId;
			if (Label.PicBacks[0, 0].Time == 0f)
			{
				return Label.PicBacks[0, 0].Scale == SizeF.Empty;
			}
			return false;
		}

		protected override void DrawBackGround()
		{
			if (_map != null)
			{
				_map.DrawBack(ScriptCommand.Canvas);
				return;
			}
			if (_room != null)
			{
				_room.DrawBack(ScriptCommand.Canvas);
				return;
			}
			if (ScriptCommand.Options.DialogueColor != currentColor)
			{
				GetColorIndex();
			}
			Label.CurrentId = labelId;
			for (int i = 0; i < 9; i++)
			{
				ScriptCommand.Canvas.DrawPicture(Label.PicBacks[0, i]);
				ScriptCommand.Canvas.DrawPicture(Label.PicBacks[currentColorIndex, i]);
			}
		}

		protected override void DrawItem()
		{
			if (_map != null || _room != null)
			{
				return;
			}
			ScriptCommand.Canvas.DrawPicture(selectBar);
			if (CurrentSelected == -1)
			{
				return;
			}
			iconAnime.X = selectBar.X - (int)(iconX + 0.5f);
			ScriptCommand.Graphics.DrawPicture(iconAnime);
			iconAnime.X = selectBar.X + (int)(iconX + 0.5f);
			ScriptCommand.Graphics.DrawPicture(iconAnime);
			if (ScriptCommand.Options.HintShow && hintChara != null)
			{
				if (_hints[CurrentSelected].EndsWith("a"))
				{
					ScriptCommand.Canvas.DrawPicture(hintAnimes[0]);
				}
				else
				{
					ScriptCommand.Canvas.DrawPicture(hintAnimes[1]);
				}
			}
		}

		protected override void DrawString()
		{
			if (_map == null && _room == null)
			{
				base.DrawString();
			}
		}

		private void GetColorIndex()
		{
			currentColor = ScriptCommand.Options.DialogueColor;
			if (currentColor == "Blue")
			{
				currentColorIndex = 1;
				return;
			}
			if (currentColor == "Green")
			{
				currentColorIndex = 2;
				return;
			}
			if (currentColor == "Pink")
			{
				currentColorIndex = 3;
				return;
			}
			if (currentColor == "Gray")
			{
				currentColorIndex = 4;
				return;
			}
			throw new Exception("Unbelievable!");
		}

		public override void InitCommand()
		{
			base.InitCommand();
			if (_map != null)
			{
				_map.Init();
			}
			else if (_room != null)
			{
				_room.Init();
			}
			else if (ItemStrings.Count > 0)
			{
				GameManager.Data.LoadAndPlaySystemSound("ok.ogg");
			}
		}

		public override void ConvertXml(XmlNode node, NodeInfo info)
		{
			base.ConvertXml(node, info);
			if (ScriptHelper.GetArgumentString(node, info, "style", string.Empty) == "map")
			{
				_map = new MapSelect(this);
				_map.ConvertXml(node, info);
				return;
			}
			if (ScriptHelper.GetArgumentBool(node, info, "room", false))
			{
				_room = new RoomSelect(this);
				_room.ConvertXml(node, info);
				return;
			}
			_hints = new string[node.ChildNodes.Count];
			for (int i = 0; i < node.ChildNodes.Count; i++)
			{
				XmlNode node2 = node.ChildNodes[i];
				_hints[i] = ScriptHelper.GetArgumentString(node2, info, "hint", string.Empty);
			}
			for (int j = 0; j < _hints.Length; j++)
			{
				if (_hints[j] != string.Empty)
				{
					if (hintChara == null)
					{
						hintChara = _hints[j].Substring(0, 2);
					}
					else if (!_hints[j].StartsWith(hintChara))
					{
						throw new ScriptArgumentException(node, info, "hint");
					}
					if (_hints[j].Length != 3 || (!_hints[j].EndsWith("a") && !_hints[j].EndsWith("b")))
					{
						throw new ScriptArgumentException(node, info, "hint");
					}
				}
			}
		}

		public void InitInstanceData()
		{
			if (_map != null)
			{
				_map.InitInstanceData();
			}
			else if (_room != null)
			{
				_room.InitInstanceData();
			}
			else if (hintChara != null)
			{
				hintBase = ScriptCommand.Data.LoadPicture("system", "hint_" + hintChara + ".png");
			}
		}

		public void FinalInstanceData()
		{
			if (_map != null)
			{
				_map.FinalInstanceData();
			}
			else if (_room != null)
			{
				_room.FinalInstanceData();
			}
			else if (hintChara != null)
			{
				hintBase.Dispose();
				for (int i = 0; i < 4; i++)
				{
					hintPics[i].Dispose();
					hintPics[i] = null;
				}
				for (int j = 0; j < 2; j++)
				{
					hintAnimes[j].Dispose();
					hintAnimes[j] = null;
				}
			}
		}

		public override void FinalCommand()
		{
			base.FinalCommand();
			if (_map != null)
			{
				_map.Final();
				_map = null;
			}
			else if (_room != null)
			{
				_room.Final();
				_room = null;
			}
		}

		public override void OnItemMove(Point location)
		{
			if (_map != null)
			{
				_map.OnMove(location);
				return;
			}
			if (_room != null)
			{
				_room.OnMove(location);
				return;
			}
			int currentSelected = CurrentSelected;
			base.OnItemMove(location);
			((CloseToCore)GameManager.GalCore).CursorActive = CurrentSelected >= 0;
			if (CurrentSelected >= 0 && CurrentSelected != currentSelected)
			{
				GameManager.Data.LoadAndPlaySystemSound("cursor.ogg");
			}
		}

		public override bool OnItemClick()
		{
			if (_map != null)
			{
				if (base.OnItemClick())
				{
					_map.OnClick();
				}
				return true;
			}
			if (_room != null)
			{
				_room.OnClick();
				base.OnItemClick();
				return true;
			}
			GameManager.Data.LoadAndPlaySystemSound("ok.ogg");
			((CloseToCore)GameManager.GalCore).CursorActive = false;
			return base.OnItemClick();
		}

		protected override void CheckKey()
		{
			if (_room != null)
			{
				_room.OnKey();
			}
			else if (_map != null)
			{
				_map.OnKey();
			}
			else
			{
				base.CheckKey();
			}
		}
	}
}
