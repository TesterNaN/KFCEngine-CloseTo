using System;
using KidFansChannel.GalCore.Commands;
using KidFansChannel.GalCore.ExtBases;

namespace KidFansChannel.GalExt.TransformStyles
{
	public class BottomWaveOneChara : TransformStyle<CharaDisplay>
	{
		private float transformTime;

		public override bool Finished
		{
			get
			{
				return transformTime > 6f;
			}
		}

		public BottomWaveOneChara()
		{
			Name = "bottomwaveone";
			Lasting = false;
		}

		public override void InitStyle()
		{
			Parent.Chara.X = -400;
			Parent.Chara.Y = 870;
			Parent.Chara.Rotate = -(float)Math.PI / 8f;
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
			if (transformTime < 6f)
			{
				Parent.Chara.X = (int)((double)(-400f + transformTime * 240f) + 0.5);
				Parent.Chara.Y = (int)(885.0 + 15.0 * Math.Cos((double)transformTime * Math.PI) + 0.5);
			}
			else
			{
				Parent.Chara.X = 1200;
				Parent.Chara.Y = 885;
			}
		}
	}
}
