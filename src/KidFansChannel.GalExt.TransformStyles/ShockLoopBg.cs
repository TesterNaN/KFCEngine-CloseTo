using System;
using System.Drawing;
using KidFansChannel.GalCore.Commands;
using KidFansChannel.GalCore.ExtBases;

namespace KidFansChannel.GalExt.TransformStyles
{
	public class ShockLoopBg : TransformStyle<BgDisplay>
	{
		private Point baseLocation;

		public override bool Finished
		{
			get
			{
				return false;
			}
		}

		public ShockLoopBg()
		{
			Name = "shockloop";
			Lasting = false;
		}

		public override void InitStyle()
		{
			baseLocation = Parent.Bg.Position;
		}

		public override void UpdateStyle(float elaspedTime)
		{
			if (Parent.Bg.Time > 0f)
			{
				Parent.Bg.Update(elaspedTime);
				return;
			}
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
	}
}
