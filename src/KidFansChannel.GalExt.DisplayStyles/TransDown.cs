using System;
using KidFansChannel.GalCore.Commands;
using KidFansChannel.GalCore.ExtBases;
using KidFansChannel.Game2DCore;

namespace KidFansChannel.GalExt.DisplayStyles
{
	public class TransDown : DisplayStyle
	{
		public static int Height = 100;

		public float yFinish;

		public float ySpeed;

		public int top;

		public int bottom;

		protected static byte[] values;

		private FAlphaPicture pic;

		public override bool Finished
		{
			get
			{
				return yFinish == 0f;
			}
		}

		public override bool Ready
		{
			get
			{
				return true;
			}
		}

		public TransDown()
		{
			Name = "transdown";
		}

		public override void InitStyle(float speed)
		{
			values = new byte[Height];
			for (int i = 0; i < Height; i++)
			{
				values[i] = (byte)((Math.Sin(((double)((float)i / (float)Height) - 0.5) * Math.PI) + 1.0) * 128.0);
			}
			yFinish = BgDisplay.ScreenSize.Height + Height;
			bottom = BgDisplay.ScreenSize.Height;
			top = BgDisplay.ScreenSize.Height + Height;
			ySpeed = (float)(Height + BgDisplay.ScreenSize.Height) / speed;
			pic = ScriptCommand.Graphics.CreateFAlphaPicture(Parent.Picture, Func);
			pic.Hlsl = "\r\nfloat g_Top;\r\nfloat g_Bottom;\r\n{0}\r\n    float y = texCoord.y * " + BgDisplay.ScreenSize.Height + ";\r\n    if (y < g_Top)\r\n        color.a = 0;\r\n    else if (y < g_Bottom)\r\n        color.a = (sin(((y - g_Top) / " + Height.ToString() + " - 0.5) * " + Math.PI.ToString() + ") + 1) / 2;\r\n    else\r\n        color.a = 1;\r\n{1}";
			pic.SetValue = SetValue;
		}

		public void SetValue()
		{
			pic.Effect.SetValue("g_Top", top);
			pic.Effect.SetValue("g_Bottom", bottom);
		}

		public override void UpdateStyle(float elaspedTime)
		{
			yFinish -= elaspedTime * ySpeed;
			bottom = (int)yFinish;
			top = (int)(yFinish - (float)Height);
			if (pic != null && yFinish <= 0f)
			{
				yFinish = 0f;
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
			if (y < top)
			{
				return 0;
			}
			if (y < bottom)
			{
				return values[y - top];
			}
			return byte.MaxValue;
		}
	}
}
