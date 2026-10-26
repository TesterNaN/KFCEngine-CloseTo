using System;
using System.Drawing;
using KidFansChannel.GalCore.Commands;
using KidFansChannel.GalCore.ExtBases;

namespace KidFansChannel.GalExt.TransformStyles
{
	public class Shock1Bg : TransformStyle<BgDisplay>
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

		public Shock1Bg()
		{
			Name = "shock1";
			Lasting = true;
		}

		public override void InitStyle()
		{
			time = 0.2f;
			baseLocation = Parent.Bg.Position;
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
			Parent.Bg.Y = baseLocation.Y + (int)((double)(time * 40f) * Math.Sin((double)time * Math.PI * 12.0) + 0.5);
			Parent.Bg.X = baseLocation.X + (int)(time * (float)(side ? 80 : (-80)));
			side = !side;
		}
	}
}
