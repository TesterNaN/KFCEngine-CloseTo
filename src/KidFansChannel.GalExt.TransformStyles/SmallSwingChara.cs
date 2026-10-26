using KidFansChannel.GalCore.Commands;
using KidFansChannel.GalCore.ExtBases;

namespace KidFansChannel.GalExt.TransformStyles
{
	public class SmallSwingChara : TransformStyle<CharaDisplay>
	{
		private float transformTime;

		private int baseX;

		public override bool Finished
		{
			get
			{
				return transformTime >= 0.9f;
			}
		}

		public SmallSwingChara()
		{
			Name = "smallswing";
			Lasting = true;
		}

		public override void InitStyle()
		{
			transformTime = 0f;
			baseX = Parent.Chara.X;
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
			if (transformTime < 0.25f)
			{
				Parent.Chara.X = baseX - (int)(80f * transformTime + 0.5f);
			}
			else if (transformTime < 0.65f)
			{
				Parent.Chara.X = baseX - 20 + (int)(100f * (transformTime - 0.25f) + 0.5f);
			}
			else if (transformTime < 0.9f)
			{
				Parent.Chara.X = baseX + 20 - (int)(80f * (transformTime - 0.65f) + 0.5f);
			}
			else
			{
				Parent.Chara.X = baseX;
			}
		}
	}
}
