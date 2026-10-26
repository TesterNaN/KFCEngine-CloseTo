using System;
using KidFansChannel.GalCore.Commands;
using KidFansChannel.GalCore.ExtBases;
using KidFansChannel.Game2DCore;

namespace KidFansChannel.GalExt.DisplayStyles
{
	public class CircleIn : DisplayStyle
	{
		public float rSpeed;

		public float r;

		public float max;

		private FAlphaPicture pic;

		public override bool Finished
		{
			get
			{
				return r == 0f;
			}
		}

		public override bool Ready
		{
			get
			{
				return true;
			}
		}

		public CircleIn()
		{
			Name = "circlein";
		}

		public override void InitStyle(float speed)
		{
			max = (float)Math.Sqrt(BgDisplay.ScreenSize.Width * BgDisplay.ScreenSize.Width + BgDisplay.ScreenSize.Height * BgDisplay.ScreenSize.Height) / 2f;
			rSpeed = max / speed;
			r = max;
			pic = ScriptCommand.Graphics.CreateFAlphaPicture(Parent.Picture, Func);
			pic.Hlsl = "\r\nfloat g_R;\r\n{0}\r\n    float x = (texCoord.x - 0.5) * " + BgDisplay.ScreenSize.Width + ";\r\n    float y = (texCoord.y - 0.5) * " + BgDisplay.ScreenSize.Height + ";\r\n    float r = sqrt(x * x + y * y);\r\n    color.a = step(0, r - g_R);\r\n{1}";
			pic.SetValue = SetValue;
		}

		public void SetValue()
		{
			pic.Effect.SetValue("g_R", r);
		}

		public override void UpdateStyle(float elaspedTime)
		{
			r -= elaspedTime * rSpeed;
			if (pic != null && r <= 0f)
			{
				r = 0f;
				pic.Dispose();
				pic = null;
			}
		}

		public override void Draw()
		{
			if (pic != null)
			{
				ScriptCommand.Graphics.DrawPicture(pic);
			}
		}

		public override void InitData()
		{
		}

		public override void FinalData()
		{
		}

		public byte Func(int x, int y)
		{
			float num = x - BgDisplay.ScreenSize.Width / 2;
			float num2 = y - BgDisplay.ScreenSize.Height / 2;
			float num3 = (float)Math.Sqrt(num * num + num2 * num2);
			if (num3 - r > 0f)
			{
				return byte.MaxValue;
			}
			return 0;
		}
	}
}
