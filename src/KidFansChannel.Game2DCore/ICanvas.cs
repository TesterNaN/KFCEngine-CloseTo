using System.Drawing;

namespace KidFansChannel.Game2DCore
{
	public interface ICanvas
	{
		void DrawBegin();

		void DrawPicture(Picture picture);

		void DrawString(Font2D font, string text, int x, int y, Color color);

		void DrawLine(int x1, int y1, int x2, int y2, Color color, int width);

		void DrawEnd();
	}
}
