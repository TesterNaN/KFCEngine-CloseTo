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
			int readLines = 0;
			int totalLines = 0;
			for (int i = 0; i < RoomIndex[index].Count; i++)
			{
				readLines += StringReadNum["CloseTo_Room" + RoomIndex[index][i]];
				totalLines += StringReads["CloseTo_Room" + RoomIndex[index][i]].Length;
			}
			return (float)readLines / (float)totalLines;
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
			int count = 0;
			foreach (KeyValuePair<string, int> item in StringReadNum)
			{
				if (!item.Key.StartsWith("CloseTo_Room") && item.Value > 0)
				{
					count++;
				}
			}
			for (int i = 0; i < RoomName.Count; i++)
			{
				if (CheckChapterOpen("CloseTo", RoomName[i]))
				{
					count++;
				}
			}
			return count;
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
			List<string> group2 = new List<string>();
			group2.Add("24");
			group2.Add("30");
			group2.Add("22");
			group2.Add("23");
			roomGroups.Add(group2);
			List<string> group3 = new List<string>();
			group3.Add("20");
			roomGroups.Add(group3);
			List<string> group4 = new List<string>();
			group4.Add("25");
			group4.Add("26");
			group4.Add("27");
			group4.Add("31");
			group4.Add("21");
			roomGroups.Add(group4);
			List<string> group5 = new List<string>();
			group5.Add("29");
			group5.Add("28");
			roomGroups.Add(group5);
			RoomIndex = roomGroups;
			List<string> roomNames = new List<string>();
			roomNames.Add("Common_2");
			roomNames.Add("Yuna_3");
			roomNames.Add("Shoko_4");
			roomNames.Add("Koyuki_5");
			roomNames.Add("Ghost_154");
			RoomName = roomNames;
		}
	}
}
