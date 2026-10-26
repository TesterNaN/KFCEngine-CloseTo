using System;
using System.Drawing;
using KidFansChannel.GalCore.Commands;
using KidFansChannel.GalCore.ExtBases;

namespace KidFansChannel.GalExt.TransformStyles
{
	public class Shock2Chara : TransformStyle<CharaDisplay>
	{
		private float time;

		private bool side;

		private Point baseLocation;

		public override bool Finished
		{
			get
			{
				return time == 0f;
			}
		}

		public Shock2Chara()
		{
			Name = "shock2";
			Lasting = true;
		}

		public override void InitStyle()
		{
			time = 0.5f;
			baseLocation = Parent.Chara.Position;
			side = false;
		}

		public override void UpdateStyle(float elaspedTime)
		{
			if (ScriptCommand.PlayModeQuicking)
			{
				time -= elaspedTime * (float)ScriptCommand.QuickFactor;
			}
			else
			{
				time -= elaspedTime;
			}
			if (time < 0f)
			{
				time = 0f;
			}
			Parent.Chara.Y = baseLocation.Y + (int)((double)(time * 40f) * Math.Sin((double)time * Math.PI * 12.0) + 0.5);
			Parent.Chara.X = baseLocation.X + (int)(time * (float)(side ? 80 : (-80)));
			side = !side;
		}
	}
}
