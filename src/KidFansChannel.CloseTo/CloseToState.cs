using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using KidFansChannel.CloseTo.Commands;
using KidFansChannel.CloseTo.Screens;
using KidFansChannel.GalCore;
using KidFansChannel.GalCore.Commands;
using KidFansChannel.Game2DCore;

namespace KidFansChannel.CloseTo
{
	public class CloseToState : GameState
	{
		public static readonly List<List<string>> RoomIndex;

		public static readonly List<string> RoomName;

		protected void GameFont_FontChanged(object sender, EventArgs e)
		{
			if (StateString != null && StateString.Font != null)
			{
				StateString.Font = GalCore.GameFont.GetFont(18);
			}
		}

		protected override void InitDrawString()
		{
			if (!(GalCore.GameGraphics.Resolution.StateArea == Rectangle.Empty))
			{
				if (StateString == null)
				{
					GalCore.GameFont.FontChanged += GameFont_FontChanged;
					StateString = new String2D();
					StateString.Font = GalCore.GameFont.GetFont(20);
					StateString.DefaultColor = Color.White;
				}
				RectangleF targetBounds = GalCore.GameGraphics.TargetBounds;
				Rectangle stateArea = GalCore.GameGraphics.Resolution.StateArea;
				StateString.Location = new Point((int)(((float)stateArea.X - targetBounds.X) / targetBounds.Width + 0.5f), (int)(((float)stateArea.Y - targetBounds.Y) / targetBounds.Height + 0.5f));
				StateString.LineSize = new Size((int)((float)stateArea.Width / targetBounds.Width + 0.5f), 32);
				StateString.LineNum = (int)((float)stateArea.Height / targetBounds.Height + 0.5f) / 32;
			}
		}

		public override bool DrawState()
		{
			if (!base.DrawState())
			{
				return false;
			}
			StateString.Canvas = GalCore.CurrentCanvas;
			StateString.ClearString();
			StateString.AddString("\n", ContentAlignment.TopCenter);
			StateString.AddString("~ " + ((Item.Chapter == null) ? "------" : Item.Chapter) + " ~\n\n", ContentAlignment.TopCenter);
			StateString.CutLine(0, 2);
			StateString.AddString(((Item.BgName == null) ? "------" : Item.BgName) + "\n\n", ContentAlignment.TopCenter);
			StateString.CutLine(0, 4);
			StateString.AddString(((Item.BgmName == null) ? "------" : Item.BgmName) + "\n\n", ContentAlignment.TopCenter);
			StateString.CutLine(0, 6);
			StateString.AddString(Item.ReadPercentage.ToString("P") + "\n", ContentAlignment.TopLeft);
			StateString.AddString(Item.GameDateTime + "\n", ContentAlignment.TopLeft);
			StateString.AddString(new TimeSpan((long)Item.CurrentTime * 10000000L).ToString() + "\n", ContentAlignment.TopLeft);
			StateString.AddString(new TimeSpan((long)Item.TotalTime * 10000000L).ToString() + "\n", ContentAlignment.TopLeft);
			StateString.DrawString();
			return true;
		}

		public override void GetSaveInfo(int index, out int icon)
		{
			base.GetSaveInfo(index, out icon);
			if (icon >= 7)
			{
				icon = GalCore.GameScript.QuickIcon;
			}
		}

		public override void GetSaveInfo(int index, out int icon, out string comment, out string level, out DateTime saveTime, out Picture snap)
		{
			base.GetSaveInfo(index, out icon, out comment, out level, out saveTime, out snap);
			if (icon >= 7)
			{
				icon = GalCore.GameScript.QuickIcon;
			}
		}

		public void SaveStateNoPrompt()
		{
			base.SaveState();
		}

		public override void SaveState()
		{
			if (IsCreatingState)
			{
				base.SaveState();
			}
			else
			{
				GalCore.ShowMessageBox(new CloseToMessageBoxScreen(GalCore.GameLanguage.ResourceManager.GetString("SaveStateStr"), MessageBoxButtons.YesNo, true, CloseToSaveState, new object[0]));
			}
		}

		private void CloseToSaveState(object[] param)
		{
			base.SaveState();
		}

		public override int GetChapterNumber()
		{
			return base.GetChapterNumber() - 26 + 5;
		}

		public override bool CheckChapterOpen(string story, string chapter)
		{
			return GetStringPercentage(story, chapter) > 0f;
		}

		public override float GetStringPercentage(string story, string chapter)
		{
			if (story != "CloseTo")
			{
				return base.GetStringPercentage(story, chapter);
			}
			if (chapter.StartsWith("Room"))
			{
				int index = -1;
				for (int i = 0; i < RoomIndex.Count; i++)
				{
					if (RoomIndex[i].Contains(chapter.Substring(4)))
					{
						index = i;
						break;
					}
				}
				return GetRoomPercentage(index);
			}
			if (RoomName.Contains(chapter))
			{
				return GetRoomPercentage(RoomName.IndexOf(chapter));
			}
			return base.GetStringPercentage(story, chapter);
		}

		private float GetRoomPercentage(int index)
		{
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < RoomIndex[index].Count; i++)
			{
				num += StringReadNum["CloseTo_Room" + RoomIndex[index][i]];
				num2 += StringReads["CloseTo_Room" + RoomIndex[index][i]].Length;
			}
			return (float)num / (float)num2;
		}

		public override void UpdateDrawState()
		{
			base.UpdateDrawState();
			foreach (ScriptCommand currentCommand in GalCore.CurrentCommandList)
			{
				if (currentCommand is BgDisplay && ((BgDisplay)currentCommand).Layer == 0 && ((CloseToBgDisplay)currentCommand).Room != null)
				{
					Item.BgName = GameManager.GalCore.GameLanguage.ResourceManager.GetString("RoomBgStr");
				}
			}
		}

		public override int CountChapters()
		{
			int num = 0;
			foreach (KeyValuePair<string, int> item in StringReadNum)
			{
				if (!item.Key.StartsWith("CloseTo_Room") && item.Value > 0)
				{
					num++;
				}
			}
			for (int i = 0; i < RoomName.Count; i++)
			{
				if (CheckChapterOpen("CloseTo", RoomName[i]))
				{
					num++;
				}
			}
			return num;
		}

		static CloseToState()
		{
			List<List<string>> roomGroups = new List<List<string>>();
			List<string> roomIds = new List<string>();
			roomIds.Add("02");
			roomIds.Add("03");
			roomIds.Add("05");
			roomIds.Add("06");
			roomIds.Add("04");
			roomIds.Add("07a");
			roomIds.Add("07b");
			roomIds.Add("07c");
			roomIds.Add("08");
			roomIds.Add("09");
			roomIds.Add("10");
			roomIds.Add("11");
			roomIds.Add("12");
			roomIds.Add("13");
			roomIds.Add("14");
			roomGroups.Add(roomIds);
			List<string> list3 = new List<string>();
			list3.Add("24");
			list3.Add("30");
			list3.Add("22");
			list3.Add("23");
			roomGroups.Add(list3);
			List<string> list4 = new List<string>();
			list4.Add("20");
			roomGroups.Add(list4);
			List<string> list5 = new List<string>();
			list5.Add("25");
			list5.Add("26");
			list5.Add("27");
			list5.Add("31");
			list5.Add("21");
			roomGroups.Add(list5);
			List<string> list6 = new List<string>();
			list6.Add("29");
			list6.Add("28");
			roomGroups.Add(list6);
			RoomIndex = roomGroups;
			List<string> list7 = new List<string>();
			list7.Add("Common_2");
			list7.Add("Yuna_3");
			list7.Add("Shoko_4");
			list7.Add("Koyuki_5");
			list7.Add("Ghost_154");
			RoomName = list7;
		}
	}
}
