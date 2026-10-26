using System;
using System.Drawing;
using System.Xml;
using KidFansChannel.GalCore;
using KidFansChannel.GalCore.Commands;

namespace KidFansChannel.CloseTo.Commands
{
	public class CloseToShowChapter : ShowChapter
	{
		public CloseToShowChapter()
		{
			ShowHideTime = 0.5f;
			LastingTime = 3f;
		}

		protected void GameFont_FontChanged(object sender, EventArgs e)
		{
			if (Chapter != null && Chapter.Font != null)
			{
				Chapter.Font = ScriptCommand.GalCore.GameFont.GetFont(20);
			}
		}

		protected override void SetFontStyle(String2D chapter)
		{
			chapter.Font = ScriptCommand.GalCore.GameFont.GetFont(20);
			chapter.LineSize = new Size(600, 30);
			chapter.Location = new Point(85, 30);
			chapter.DrawCharCallback = DrawChar.DrawCharShadow;
		}

		public override void ConvertXml(XmlNode node, NodeInfo info)
		{
			base.ConvertXml(node, info);
			Chapter.ClearString();
			Chapter.AddString("～" + node.FirstChild.Value + "～", ContentAlignment.TopLeft);
		}
	}
}
