using KidFansChannel.GalCore.Commands;
using KidFansChannel.GalCore.ExtBases;

namespace KidFansChannel.GalExt.TransformStyles
{
	public class WaveFall2Chara : TransformStyle<CharaDisplay>
	{
		private float? _x;

		private float? _y;

		public float X
		{
			get
			{
				if (!_x.HasValue)
				{
					return 0f;
				}
				return _x.Value;
			}
		}

		public float Y
		{
			get
			{
				if (!_y.HasValue)
				{
					return 0f;
				}
				return _y.Value;
			}
		}

		public override bool Finished
		{
			get
			{
				return false;
			}
		}

		public WaveFall2Chara()
		{
			Name = "wavefall2";
			Lasting = false;
		}

		public override void Init(CharaDisplay parent)
		{
			base.Init(parent);
			if (parent.TransformStyle != null && parent.TransformStyle is WaveFall1Chara)
			{
				WaveFall1Chara waveFall1Chara = (WaveFall1Chara)parent.TransformStyle;
				_x = waveFall1Chara.X;
				_y = waveFall1Chara.Y;
			}
		}

		public override void InitStyle()
		{
			if (!_x.HasValue)
			{
				_x = 0f;
			}
			if (!_y.HasValue)
			{
				_y = 0f;
			}
			Parent.Chara.Position = WaveFall1Chara.CalculatePos(_x.Value, _y.Value);
		}

		public override void UpdateStyle(float elaspedTime)
		{
			_y += elaspedTime * 15f;
			if (_y > 900f)
			{
				_y = 900f;
			}
			for (_x += elaspedTime; _x > 8f; _x -= 8f)
			{
			}
			Parent.Chara.Position = WaveFall1Chara.CalculatePos(_x.Value, _y.Value);
			if (Parent.Chara.Y < BgDisplay.ScreenSize.Height)
			{
				Parent.Chara.Y = BgDisplay.ScreenSize.Height;
			}
		}
	}
}
