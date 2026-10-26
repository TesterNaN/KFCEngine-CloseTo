using System;
using System.Drawing;
using KidFansChannel.GalCore.Commands;
using KidFansChannel.GalCore.ExtBases;

namespace KidFansChannel.GalExt.TransformStyles
{
	public class WaveFall1Chara : TransformStyle<CharaDisplay>
	{
		private float _x;

		private float _y;

		public float X
		{
			get
			{
				return _x;
			}
		}

		public float Y
		{
			get
			{
				return _y;
			}
		}

		public override bool Finished
		{
			get
			{
				return false;
			}
		}

		public WaveFall1Chara()
		{
			Name = "wavefall1";
			Lasting = false;
		}

		public override void InitStyle()
		{
			_x = 0f;
			_y = 0f;
			Parent.Chara.Position = CalculatePos(_x, _y);
		}

		public static Point CalculatePos(float x, float y)
		{
			Point result = default(Point);
			if (x <= 1f)
			{
				result.X = (int)(100f * x);
			}
			else if (x <= 3f)
			{
				result.X = (int)(100f * x - 50f * (x - 1f) * (x - 1f));
			}
			else if (x <= 5f)
			{
				result.X = (int)(100f - 100f * (x - 3f));
			}
			else if (x <= 7f)
			{
				result.X = (int)(-100f + -100f * (x - 5f) + 50f * (x - 5f) * (x - 5f));
			}
			else
			{
				result.X = (int)(-100f + 100f * (x - 7f));
			}
			result.X += BgDisplay.ScreenSize.Width / 2;
			result.Y = (int)((double)(y - 20f) + 20.0 * Math.Cos((double)(x / 2f) * Math.PI) + (double)BgDisplay.ScreenSize.Height);
			return result;
		}

		public override void UpdateStyle(float elaspedTime)
		{
			_y += elaspedTime * 15f;
			if (_y > 450f)
			{
				_y = 450f;
			}
			_x += elaspedTime;
			while (_x > 8f)
			{
				_x -= 8f;
			}
			Parent.Chara.Position = CalculatePos(_x, _y);
			if (Parent.Chara.Y < BgDisplay.ScreenSize.Height)
			{
				Parent.Chara.Y = BgDisplay.ScreenSize.Height;
			}
		}
	}
}
