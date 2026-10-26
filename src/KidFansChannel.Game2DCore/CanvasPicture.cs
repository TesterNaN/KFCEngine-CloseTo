using System;
using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace KidFansChannel.Game2DCore
{
	public class CanvasPicture : Picture, ICanvas
	{
		private Surface renderSurface;

		private RenderToSurface rts;

		internal CanvasPicture(Game2DCore core, Size size)
		{
			if (core == null)
			{
				throw new ArgumentNullException("Core must not be null.");
			}
			Core = core;
			core.PictureList.Add(this);
			Size = size;
			Textured = true;
			RefreshVerts();
			rts = new RenderToSurface(core.Device, size.Width, size.Height, Format.X8R8G8B8, true, DepthFormat.D16);
			Texture = new Texture(core.Device, size.Width, size.Height, 1, Usage.RenderTarget, Format.X8R8G8B8, Pool.Default);
			renderSurface = Texture.GetSurfaceLevel(0);
			IsCreated = true;
		}

		public override void Dispose()
		{
			base.Dispose();
			if (renderSurface != null)
			{
				renderSurface.Dispose();
				renderSurface = null;
			}
			if (rts != null)
			{
				rts.Dispose();
				rts = null;
			}
		}

		~CanvasPicture()
		{
			Dispose();
		}

		internal void OnDeviceLost()
		{
			if (rts != null && !rts.Disposed)
			{
				rts.Dispose();
				rts = null;
			}
			if (renderSurface != null && !renderSurface.Disposed)
			{
				renderSurface.Dispose();
				renderSurface = null;
			}
			if (Texture != null && !Texture.Disposed)
			{
				Texture.Dispose();
				Texture = null;
			}
		}

		internal void OnDeviceReset()
		{
			rts = new RenderToSurface(Core.Device, Size.Width, Size.Height, Format.X8R8G8B8, true, DepthFormat.D16);
			Texture = new Texture(Core.Device, Size.Width, Size.Height, 1, Usage.RenderTarget, Format.X8R8G8B8, Pool.Default);
			renderSurface = Texture.GetSurfaceLevel(0);
		}

		public void Save(string fileName)
		{
		}

		public void DrawBegin()
		{
			rts.BeginScene(renderSurface);
			Core.Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1f, 0);
		}

		public void DrawPicture(Picture picture)
		{
			if (picture != null)
			{
				picture.Draw(Core.Device, Core.VB, false);
			}
		}

		public void DrawString(Font2D font, string text, int x, int y, Color color)
		{
			if (font != null)
			{
				font.DrawText(text, x, y, color, false);
			}
		}

		public void DrawLine(int x1, int y1, int x2, int y2, Color color, int width)
		{
			Core.Line.Width = width;
			Core.Line.Draw(new Vector2[2]
			{
				new Vector2(x1, y1),
				new Vector2(x2, y2)
			}, color);
		}

		public void DrawEnd()
		{
			rts.EndScene(Filter.None);
		}
	}
}
