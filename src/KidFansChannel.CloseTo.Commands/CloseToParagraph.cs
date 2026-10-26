using System;
using System.Drawing;
using KidFansChannel.GalCore;
using KidFansChannel.GalCore.Commands;

namespace KidFansChannel.CloseTo.Commands
{
	internal class CloseToParagraph : Paragraph
	{
		protected void GameFont_FontChanged(object sender, EventArgs e)
		{
			if (Paragraph.ShownString != null && Paragraph.ShownString.Font != null)
			{
				Paragraph.ShownString.Font = ScriptCommand.GalCore.GameFont.GetFont(24);
			}
			if (Paragraph.CurrentString != null && Paragraph.CurrentString.Font != null)
			{
				Paragraph.CurrentString.Font = ScriptCommand.GalCore.GameFont.GetFont(24);
			}
		}

		protected override float GetSpeed()
		{
			switch (ScriptCommand.Options.TextSpeed)
			{
			case GameOptions.Level.High:
				return 1000f;
			case GameOptions.Level.Mid:
				return 500f;
			case GameOptions.Level.Low:
				return 250f;
			case GameOptions.Level.None:
				return 2500f;
			default:
				return 500f;
			}
		}

		public override void InitStaticData()
		{
			ScriptCommand.GalCore.GameFont.FontChanged += GameFont_FontChanged;
			Paragraph.ShownString = new String2D();
			Paragraph.ShownString.ClickLink += String_ClickLink;
			Paragraph.ShownString.MoveLink += String_MoveLink;
			Paragraph.CurrentString = new String2D();
			Paragraph.CurrentString.ClickLink += String_ClickLink;
			Paragraph.CurrentString.MoveLink += String_MoveLink;
			Paragraph.ShownString.Font = ScriptCommand.GalCore.GameFont.GetFont(24);
			Paragraph.ShownString.Location = new Point(40, 80);
			Paragraph.ShownString.LineSize = new Size(720, 36);
			Paragraph.ShownString.LineNum = 12;
			Paragraph.ShownString.DrawCharCallback = DrawChar.DrawCharBorder;
			Paragraph.CurrentString.Font = ScriptCommand.GalCore.GameFont.GetFont(24);
			Paragraph.CurrentString.Location = new Point(40, 80);
			Paragraph.CurrentString.LineSize = new Size(720, 36);
			Paragraph.CurrentString.LineNum = 12;
			Paragraph.CurrentString.DrawCharCallback = DrawChar.DrawCharBorder;
		}

		private void String_MoveLink(object sender, String2D.LinkArgs e)
		{
			if (e.Value == -1)
			{
				((CloseToCore)ScriptCommand.GalCore).CursorActive = false;
			}
			else
			{
				((CloseToCore)ScriptCommand.GalCore).CursorActive = true;
			}
		}

		private void String_ClickLink(object sender, String2D.LinkArgs e)
		{
			((CloseToCore)ScriptCommand.GalCore).CursorActive = false;
		}

		public override void FinalStaticData()
		{
			base.FinalCommand();
		}
	}
}
