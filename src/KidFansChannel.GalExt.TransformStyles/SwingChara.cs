using System;
using KidFansChannel.GalCore.Commands;
using KidFansChannel.GalCore.ExtBases;

namespace KidFansChannel.GalExt.TransformStyles
{
	public class SwingChara : TransformStyle<CharaDisplay>
	{
		private float transformTime;

		public override bool Finished
		{
			get
			{
				return transformTime == 1.2f;
			}
		}

		public SwingChara()
		{
			Name = "swing";
			Lasting = false;
		}

		public override void InitStyle()
		{
			Parent.Chara.Rotate = 0f;
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
			if (transformTime > 1.2f)
			{
				transformTime = 1.2f;
			}
			if (transformTime < 0.25f)
			{
				Parent.Chara.Rotate = (float)((double)transformTime * Math.PI / 8.0);
			}
			else if (transformTime < 0.3f)
			{
				Parent.Chara.Rotate = (float)Math.PI / 32f;
			}
			else if (transformTime < 0.55f)
			{
				Parent.Chara.Rotate = (float)(Math.PI / 32.0 - ((double)transformTime - 0.3) * Math.PI / 4.0);
			}
			else if (transformTime < 0.6f)
			{
				Parent.Chara.Rotate = -(float)Math.PI / 32f;
			}
			else if (transformTime < 0.85f)
			{
				Parent.Chara.Rotate = (float)(-Math.PI / 32.0 + ((double)transformTime - 0.6) * Math.PI / 4.0);
			}
			else if (transformTime < 0.9f)
			{
				Parent.Chara.Rotate = (float)Math.PI / 32f;
			}
			else if (transformTime < 1.15f)
			{
				Parent.Chara.Rotate = (float)(Math.PI / 32.0 - ((double)transformTime - 0.9) * Math.PI / 8.0);
			}
			else
			{
				Parent.Chara.Rotate = 0f;
			}
		}
	}
}
