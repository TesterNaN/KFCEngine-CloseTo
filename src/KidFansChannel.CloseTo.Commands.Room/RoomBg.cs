using System;
using System.Drawing;
using KidFansChannel.GalCore;
using KidFansChannel.GalCore.Commands;
using KidFansChannel.Game2DCore;

namespace KidFansChannel.CloseTo.Commands.Room
{
	public class RoomBg
	{
		private const int FreePicNum = 80;

		protected BgDisplay Parent;

		private Picture picSrc;

		private HlslPicture picHlsl;

		private SubPicture picSub;

		private FreePicture[] picFrees = new FreePicture[80];

		private float _camPos = 0.5f;

		public float CamPos
		{
			get
			{
				return _camPos;
			}
			set
			{
				_camPos = value;
			}
		}

		public RoomBg(BgDisplay parent)
		{
			Parent = parent;
		}

		public void Init()
		{
			picSrc = Parent.Bg;
			picHlsl = GameManager.Graphics.CreateHlslPicture(picSrc);
			picHlsl.Hlsl = "\r\nfloat g_CamPos;\r\n{0}\r\n\tfloat ang = atan((texCoord.x * 2.0f - 1.0f) * 0.57735035f);\r\n\tfloat rate = 1.0f / cos(ang);\r\n\tfloat x = (ang / 3.14159265f * 360.0f / 390.0f) + g_CamPos;\r\n\tfloat y = ((texCoord.y - 0.5f) / rate) + 0.5f;\r\n    color = tex2D(TextureSampler, float2(x, y));\r\n{1}";
			picHlsl.SetValue = SetValue;
			picHlsl.ScaleX = (float)BgDisplay.ScreenSize.Width / (float)picHlsl.Width;
			picHlsl.ScaleY = (float)BgDisplay.ScreenSize.Height / (float)picHlsl.Height;
			picSub = GameManager.Graphics.CreateSubPicture(picSrc, new Rectangle(600, 0, 800, 600));
			for (int i = 0; i < 80; i++)
			{
				picFrees[i] = GameManager.Graphics.CreateFreePicture();
				picFrees[i].Size = new Size(BgDisplay.ScreenSize.Width / 80, BgDisplay.ScreenSize.Height);
				picFrees[i].Position = new Point(i * BgDisplay.ScreenSize.Width / 80, 0);
				picFrees[i].SetTexture(picSrc.Texture);
			}
		}

		public void SetValue()
		{
			picHlsl.Effect.SetValue("g_CamPos", _camPos);
		}

		public void Final()
		{
			picSrc = null;
			picHlsl.Dispose();
			picHlsl = null;
			picSub.Dispose();
			picSub = null;
			for (int i = 0; i < 80; i++)
			{
				picFrees[i].ClearTexture();
				picFrees[i].Dispose();
				picFrees[i] = null;
			}
		}

		public void MoveLeft(float time)
		{
			_camPos -= time * 0.5f;
			if (_camPos < 630f / 4095f)
			{
				_camPos = 630f / 4095f;
			}
		}

		public void MoveRight(float time)
		{
			_camPos += time * 0.5f;
			if (_camPos > 3465f / 4095f)
			{
				_camPos = 3465f / 4095f;
			}
		}

		public static PointF ScreenToPicture(Point location, float canPos)
		{
			double angle = Math.Atan(((double)((float)location.X / (float)BgDisplay.ScreenSize.Width) * 2.0 - 1.0) * Math.Tan(Math.PI / 6.0));
			double perspective = 1.0 / Math.Cos(angle);
			double panX = angle / Math.PI * 360.0 / 390.0 + (double)canPos;
			double screenY = ((double)((float)location.Y / (float)BgDisplay.ScreenSize.Height) - 0.5) / perspective + 0.5;
			return new PointF((float)panX, (float)screenY);
		}

		public static Point PictureToScreen(PointF location, float canPos)
		{
			double angle = (double)(location.X - canPos) * 390.0 / 360.0 * Math.PI;
			double perspective = 1.0 / Math.Cos(angle);
			double panX = (((double)location.Y - 0.5) * perspective + 0.5) * (double)BgDisplay.ScreenSize.Height;
			double screenY = (Math.Tan(angle) / Math.Tan(Math.PI / 6.0) + 1.0) / 2.0 * (double)BgDisplay.ScreenSize.Width;
			return new Point((int)(screenY + 0.5), (int)(panX + 0.5));
		}

		public void Draw(ICanvas canvas)
		{
			if (GameManager.GalCore.GameOptions.Graphics == GameOptions.Level.High)
			{
				canvas.DrawPicture(picHlsl);
			}
			else if (GameManager.GalCore.GameOptions.Graphics == GameOptions.Level.Mid)
			{
				for (int i = 0; i < 80; i++)
				{
					PointF[] array = new PointF[4];
					for (int j = 0; j < 4; j++)
					{
						array[j] = ScreenToPicture(new Point(((j & 1) == 0) ? (i * BgDisplay.ScreenSize.Width / 80) : ((i + 1) * BgDisplay.ScreenSize.Width / 80), (j >> 1 != 0) ? BgDisplay.ScreenSize.Height : 0), CamPos);
					}
					picFrees[i].SetUV(array);
					canvas.DrawPicture(picFrees[i]);
				}
			}
			else
			{
				float angle = (_camPos - 630f / 4095f) * 1200f * 39f / 27f;
				picSub.Rectangle = new Rectangle((int)(angle + 0.5f), 0, 800, 600);
				canvas.DrawPicture(picSub);
			}
		}
	}
}
