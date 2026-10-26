using System;
using KidFansChannel.GalCore.Commands;
using KidFansChannel.GalCore.ExtBases;

namespace KidFansChannel.GalExt.TransformStyles
{
	public class BottomWaveChara : TransformStyle<CharaDisplay>
	{
		private float transformTime;

		public override bool Finished
		{
			get
			{
				return false;
			}
		}

		public BottomWaveChara()
		{
			Name = "bottomwave";
			Lasting = false;
		}

		public override void InitStyle()
		{
			Parent.Chara.X = 1200;
			Parent.Chara.Y = 870;
			Parent.Chara.Rotate = (float)Math.PI / 8f;
			transformTime = 0f;
		}

		public override void UpdateStyle(float elaspedTime)
		{
			transformTime += elaspedTime;
			while (transformTime >= 12f)
			{
				transformTime -= 12f;
			}
			if (transformTime < 6f)
			{
				Parent.Chara.Rotate = (float)Math.PI / 8f;
				Parent.Chara.X = (int)((double)(1200f - transformTime * 240f) + 0.5);
				Parent.Chara.Y = (int)(885.0 + 15.0 * Math.Cos((double)transformTime * Math.PI) + 0.5);
			}
			else
			{
				Parent.Chara.Rotate = -(float)Math.PI / 8f;
				Parent.Chara.X = (int)((double)(-400f + (transformTime - 6f) * 240f) + 0.5);
				Parent.Chara.Y = (int)(885.0 + 15.0 * Math.Cos((double)(transformTime - 6f) * Math.PI) + 0.5);
			}
		}
	}
}
