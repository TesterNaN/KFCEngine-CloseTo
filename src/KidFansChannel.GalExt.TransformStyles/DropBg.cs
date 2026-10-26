using System;
using KidFansChannel.GalCore.Commands;
using KidFansChannel.GalCore.ExtBases;

namespace KidFansChannel.GalExt.TransformStyles
{
	public class DropBg : TransformStyle<BgDisplay>
	{
		public override bool Finished
		{
			get
			{
				return Parent.Bg.Time == 0f;
			}
		}

		public DropBg()
		{
			Name = "drop";
			Lasting = true;
		}

		public override void InitStyle()
		{
			Parent.Bg.X = 0;
			Parent.Bg.Y = -360;
			Parent.Bg.BeginTransform(1f / 3f + (float)Math.Sqrt(6.0) * 2f / 9f + (float)Math.Sqrt(3.0) * 2f / 9f);
			Parent.Bg.PositionTo(DropX, DropY);
			Parent.Bg.EndTransform();
		}

		public float DropX(float time)
		{
			return 0f;
		}

		public float DropY(float time)
		{
			if (time < 1f / 3f)
			{
				return -360f + 3240f * time * time;
			}
			if ((double)time < 1.0 / 3.0 + Math.Sqrt(6.0) * 2.0 / 9.0)
			{
				return -240f + 3240f * (time - 1f / 3f - (float)Math.Sqrt(6.0) / 9f) * (time - 1f / 3f - (float)Math.Sqrt(6.0) / 9f);
			}
			if ((double)time < 1.0 / 3.0 + Math.Sqrt(6.0) * 2.0 / 9.0 + Math.Sqrt(3.0) * 2.0 / 9.0)
			{
				return -120f + 3240f * (time - 1f / 3f - (float)Math.Sqrt(6.0) * 2f / 9f - (float)Math.Sqrt(3.0) / 9f) * (time - 1f / 3f - (float)Math.Sqrt(6.0) * 2f / 9f - (float)Math.Sqrt(3.0) / 9f);
			}
			return 0f;
		}

		public override void UpdateStyle(float elaspedTime)
		{
			if (ScriptCommand.PlayModeQuicking)
			{
				Parent.Bg.Update(elaspedTime * (float)ScriptCommand.QuickFactor);
			}
			else
			{
				Parent.Bg.Update(elaspedTime);
			}
		}
	}
}
