using System;
using System.Drawing;
using Microsoft.DirectX.Direct3D;

namespace KidFansChannel.Game2DCore
{
	public class FreePicture : Picture
	{
		public FreePicture(Game2DCore core)
		{
			if (core == null)
			{
				throw new ArgumentNullException("Core must not be null.");
			}
			Core = core;
			core.PictureList.Add(this);
			IsCreated = true;
		}

		public void SetTexture(Texture texture)
		{
			Texture = texture;
			Textured = true;
		}

		public void ClearTexture()
		{
			Textured = false;
		}

		public void SetUV(PointF[] uv)
		{
			if (uv.Length != 4)
			{
				throw new ArgumentException();
			}
			Bounds = uv;
		}
	}
}
