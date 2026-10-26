using KidFansChannel.GalCore.Commands;
using KidFansChannel.GalCore.ExtBases;
using KidFansChannel.Game2DCore;

namespace KidFansChannel.GalExt.DisplayStyles
{
	public class BlindLeft : DisplayStyle
	{
		public float xSpeed;

		public float xPos;

		public float width;

		public float dw;

		public int PartNum = 22;

		private FAlphaPicture pic;

		public override bool Finished
		{
			get
			{
				return xPos == 2f * width;
			}
		}

		public override bool Ready
		{
			get
			{
				return true;
			}
		}

		public BlindLeft()
		{
			Name = "blindleft";
		}

		public override void InitStyle(float speed)
		{
			width = (float)BgDisplay.ScreenSize.Width / (float)PartNum;
			dw = width / (float)PartNum;
			xSpeed = width / speed * 2f;
			xPos = 0f;
			pic = ScriptCommand.Graphics.CreateFAlphaPicture(Parent.Picture, Func);
			pic.Hlsl = "\r\nfloat g_Width;\r\nfloat g_Pos;\r\nfloat g_Dw;\r\nfloat g_Num;\r\n{0}\r\n    float x = texCoord.x * " + BgDisplay.ScreenSize.Width + ";\r\n    color.a = step(0, clamp(g_Pos - g_Dw * (g_Num - floor(x / g_Width)), 0, g_Width) - g_Width + fmod(x, g_Width));\r\n{1}";
			pic.SetValue = SetValue;
		}

		public void SetValue()
		{
			pic.Effect.SetValue("g_Width", width);
			pic.Effect.SetValue("g_Pos", xPos);
			pic.Effect.SetValue("g_Dw", dw);
			pic.Effect.SetValue("g_Num", PartNum);
		}

		public override void UpdateStyle(float elaspedTime)
		{
			xPos += elaspedTime * xSpeed;
			if (pic != null && xPos >= 2f * width)
			{
				xPos = 2f * width;
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
			float num = xPos - dw * (float)(PartNum - (int)((float)x / width));
			if (num < 0f)
			{
				num = 0f;
			}
			if (num > width)
			{
				num = width;
			}
			if (num - width + ((float)x - (float)(int)((float)x / width) * width) >= 0f)
			{
				return byte.MaxValue;
			}
			return 0;
		}
	}
}
