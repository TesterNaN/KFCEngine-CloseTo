using System;
using System.Drawing;
using KidFansChannel.Game2DCore;

namespace KidFansChannel.CloseTo
{
	public static class DrawChar
	{
		public static void DrawCharBorder(Point location, char c, float alpha, Color color, ICanvas canvas, Font2D font)
		{
			Color baseColor = Color.FromArgb(255 - color.R, 255 - color.B, 255 - color.G);
			Color color2 = Color.FromArgb((int)(Math.Pow((double)(int)color.A / 255.0, 4.0) * 255.0 + 0.5), baseColor);
			if (alpha == 1f)
			{
				canvas.DrawString(font, c.ToString(), location.X + 1, location.Y, color2);
				canvas.DrawString(font, c.ToString(), location.X - 1, location.Y, color2);
				canvas.DrawString(font, c.ToString(), location.X, location.Y + 1, color2);
				canvas.DrawString(font, c.ToString(), location.X, location.Y - 1, color2);
				canvas.DrawString(font, c.ToString(), location.X, location.Y, color);
			}
			else
			{
				canvas.DrawString(font, c.ToString(), location.X + 1, location.Y, Color.FromArgb((int)(alpha * alpha * (float)(int)color2.A + 0.5f), baseColor));
				canvas.DrawString(font, c.ToString(), location.X - 1, location.Y, Color.FromArgb((int)(alpha * alpha * (float)(int)color2.A + 0.5f), baseColor));
				canvas.DrawString(font, c.ToString(), location.X, location.Y + 1, Color.FromArgb((int)(alpha * alpha * (float)(int)color2.A + 0.5f), baseColor));
				canvas.DrawString(font, c.ToString(), location.X, location.Y - 1, Color.FromArgb((int)(alpha * alpha * (float)(int)color2.A + 0.5f), baseColor));
				canvas.DrawString(font, c.ToString(), location.X, location.Y, Color.FromArgb((int)(alpha * (float)(int)color.A + 0.5f), color));
			}
		}

		public static void DrawCharShadow(Point location, char c, float alpha, Color color, ICanvas canvas, Font2D font)
		{
			Color baseColor = Color.FromArgb(255 - color.R, 255 - color.B, 255 - color.G);
			Color color2 = Color.FromArgb((int)(Math.Pow((double)(int)color.A / 255.0, 2.0) * 255.0 + 0.5), baseColor);
			if (alpha == 1f)
			{
				canvas.DrawString(font, c.ToString(), location.X + 2, location.Y + 2, color2);
				canvas.DrawString(font, c.ToString(), location.X + 1, location.Y + 1, color2);
				canvas.DrawString(font, c.ToString(), location.X, location.Y, color);
			}
			else
			{
				canvas.DrawString(font, c.ToString(), location.X + 2, location.Y + 2, Color.FromArgb((int)(alpha * alpha * (float)(int)color2.A + 0.5f), baseColor));
				canvas.DrawString(font, c.ToString(), location.X + 1, location.Y + 1, Color.FromArgb((int)(alpha * alpha * (float)(int)color2.A + 0.5f), baseColor));
				canvas.DrawString(font, c.ToString(), location.X, location.Y, Color.FromArgb((int)(alpha * (float)(int)color.A + 0.5f), color));
			}
		}
	}
}
