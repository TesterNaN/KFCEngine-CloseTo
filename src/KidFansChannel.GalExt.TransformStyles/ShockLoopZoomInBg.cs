using System;
using System.Drawing;
using KidFansChannel.GalCore.Commands;
using KidFansChannel.GalCore.ExtBases;

namespace KidFansChannel.GalExt.TransformStyles
{
	public class ShockLoopZoomInBg : TransformStyle<BgDisplay>
	{
		private Point baseLocation;

		private SizeF baseScale;

		private float transformTime;

		public override bool Finished
		{
			get
			{
				return transformTime == 1f;
			}
		}

		public ShockLoopZoomInBg()
		{
			Name = "shockloopzoomin";
			Lasting = false;
		}

		public override void InitStyle()
		{
			baseLocation = Parent.Bg.Position;
			baseScale = Parent.Bg.Scale;
			transformTime = 0f;
		}

		public override void UpdateStyle(float elaspedTime)
		{
			if (ScriptCommand.PlayModeQuicking)
			{
				transformTime += elaspedTime * (float)ScriptCommand.QuickFactor;
			}
			else
			{
				transformTime += elaspedTime;
			}
			if (transformTime > 1f)
			{
				transformTime = 1f;
			}
			if (Parent.Bg.Time > 0f)
			{
				Parent.Bg.Update(elaspedTime);
			}
			else
			{
				Parent.Bg.BeginTransform(0.1f);
				switch (new Random().Next(4))
				{
				case 0:
					Parent.Bg.PositionTo(new Point(baseLocation.X - 6, baseLocation.Y));
					break;
				case 1:
					Parent.Bg.PositionTo(new Point(baseLocation.X + 6, baseLocation.Y));
					break;
				case 2:
					Parent.Bg.PositionTo(new Point(baseLocation.X, baseLocation.Y - 6));
					break;
				case 3:
					Parent.Bg.PositionTo(new Point(baseLocation.X, baseLocation.Y + 6));
					break;
				}
				Parent.Bg.EndTransform();
			}
			float scaleFactor = 1f - transformTime * 0.2f;
			Parent.Bg.Scale = new SizeF(baseScale.Width * scaleFactor, baseScale.Height * scaleFactor);
			Parent.Bg.X -= (int)((float)Parent.Bg.Width * baseScale.Width * transformTime * 0.1f);
			Parent.Bg.Y -= (int)((float)Parent.Bg.Height * baseScale.Height * transformTime * 0.1f);
		}
	}
}
