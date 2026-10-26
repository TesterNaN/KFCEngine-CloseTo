using System;
using System.Drawing;

namespace KidFansChannel.Game2DCore
{
	public class SubPicture : Picture
	{
		private Picture _refPicture;

		private Rectangle _rectangle;

		public Picture RefPicture
		{
			get
			{
				return _refPicture;
			}
		}

		public Rectangle Rectangle
		{
			get
			{
				return _rectangle;
			}
			set
			{
				_rectangle = value;
				Textured = false;
				Size = _rectangle.Size;
				Textured = true;
				CalculateBounds();
			}
		}

		public SubPicture(Game2DCore core, Picture refPicture, Rectangle rect)
		{
			if (core == null)
			{
				throw new ArgumentNullException("Core must not be null.");
			}
			Core = core;
			core.PictureList.Add(this);
			Size = rect.Size;
			Textured = true;
			_refPicture = refPicture;
			_rectangle = rect;
			Texture = refPicture.Texture;
			CalculateBounds();
			IsCreated = true;
		}

		protected void CalculateBounds()
		{
			RectangleF boundRect = default(RectangleF);
			boundRect.X = (float)_rectangle.X / (float)_refPicture.Width;
			boundRect.Y = (float)_rectangle.Y / (float)_refPicture.Height;
			boundRect.Width = (float)_rectangle.Width / (float)_refPicture.Width;
			boundRect.Height = (float)_rectangle.Height / (float)_refPicture.Height;
			BoundRect = boundRect;
		}

		public override void Dispose()
		{
			base.Dispose();
			Texture = null;
		}
	}
}
