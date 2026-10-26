using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using KidFansChannel.GameCore;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace KidFansChannel.Game2DCore
{
	public class Game2DCore : KidFansChannel.GameCore.GameCore, ICanvas
	{
		internal VertexBuffer VB;

		private RectangleF _targetBounds = new RectangleF(0f, 0f, 1f, 1f);

		private float _fontScale = 1f;

		internal List<Picture> PictureList = new List<Picture>();

		internal List<Font2D> FontList = new List<Font2D>();

		internal Line Line;

		private Surface snapBackBuffer;

		public RectangleF TargetBounds
		{
			get
			{
				return _targetBounds;
			}
			set
			{
				_targetBounds = value;
			}
		}

		public float FontScale
		{
			get
			{
				return _fontScale;
			}
			set
			{
				_fontScale = value;
			}
		}

		public RectangleF GetClientBounds()
		{
			RectangleF result = default(RectangleF);
			result.X = (0f - _targetBounds.X) / _targetBounds.Width;
			result.Y = (0f - _targetBounds.Y) / _targetBounds.Height;
			result.Width = (float)ClientSize.Width / _targetBounds.Width;
			result.Height = (float)ClientSize.Height / _targetBounds.Height;
			return result;
		}

		public Game2DCore()
		{
			DeviceCreated += OnDeviceCreated2D;
			DeviceReset += OnDeviceReset2D;
			DeviceLost += OnDeviceLost2D;
			Disposing += OnDisposing2D;
		}

		private void OnDeviceCreated2D(object sender, DeviceEventArgs e)
		{
			VB = new VertexBuffer(typeof(CustomVertex.TransformedColoredTextured), 4, e.Device, Usage.None, VertexFormats.Texture1 | VertexFormats.Diffuse | VertexFormats.Transformed, Pool.Managed);
			Line = new Line(e.Device);
		}

		private void OnDeviceReset2D(object sender, DeviceEventArgs e)
		{
			e.Device.RenderState.CullMode = Cull.None;
			e.Device.RenderState.AlphaBlendEnable = true;
			e.Device.RenderState.SourceBlend = Blend.SourceAlpha;
			e.Device.RenderState.DestinationBlend = Blend.InvSourceAlpha;
			foreach (Font2D font in FontList)
			{
				font.OnResetDevice();
			}
			foreach (Picture picture in PictureList)
			{
				if (picture is CanvasPicture)
				{
					((CanvasPicture)picture).OnDeviceReset();
				}
				if (picture is FAlphaPicture && ((FAlphaPicture)picture).Effect != null)
				{
					((FAlphaPicture)picture).Effect.OnResetDevice();
				}
				if (picture is HlslPicture && ((HlslPicture)picture).Effect != null)
				{
					((HlslPicture)picture).Effect.OnResetDevice();
				}
			}
			if (MixPicture.Effect != null)
			{
				MixPicture.Effect.OnResetDevice();
			}
			if (!Line.Disposed)
			{
				Line.OnResetDevice();
			}
		}

		private void OnDeviceLost2D(object sender, EventArgs e)
		{
			foreach (Font2D font in FontList)
			{
				font.OnLostDevice();
			}
			foreach (Picture picture in PictureList)
			{
				if (picture is CanvasPicture)
				{
					((CanvasPicture)picture).OnDeviceLost();
				}
				if (picture is FAlphaPicture && ((FAlphaPicture)picture).Effect != null)
				{
					((FAlphaPicture)picture).Effect.OnLostDevice();
				}
				if (picture is HlslPicture && ((HlslPicture)picture).Effect != null)
				{
					((HlslPicture)picture).Effect.OnLostDevice();
				}
			}
			if (MixPicture.Effect != null)
			{
				try
				{
					MixPicture.Effect.OnLostDevice();
				}
				catch
				{
				}
			}
			if (!Line.Disposed)
			{
				Line.OnLostDevice();
			}
		}

		private void OnDisposing2D(object sender, EventArgs e)
		{
			while (PictureList.Count > 0)
			{
				PictureList[0].Dispose();
			}
			while (FontList.Count > 0)
			{
				FontList[0].Dispose();
			}
			if (!Line.Disposed)
			{
				Line.Dispose();
			}
			VB.Dispose();
		}

		public Picture CreatePicture(string fileName)
		{
			if (Device != null)
			{
				return new Picture(this, fileName);
			}
			return null;
		}

		public Picture CreatePicture(Stream stream)
		{
			if (Device != null)
			{
				return new Picture(this, stream);
			}
			return null;
		}

		public Picture CreatePicture(Color color)
		{
			if (Device != null)
			{
				return new Picture(this, color);
			}
			return null;
		}

		public Picture CreatePictureInstant(string fileName)
		{
			if (Device != null)
			{
				return Picture.CreateInstant(this, fileName);
			}
			return null;
		}

		public Picture CreatePictureInstant(Stream stream)
		{
			if (Device != null)
			{
				return Picture.CreateInstant(this, stream);
			}
			return null;
		}

		public CanvasPicture CreateCanvasPicture(Size size)
		{
			if (Device != null)
			{
				return new CanvasPicture(this, size);
			}
			return null;
		}

		public BindingPicture CreateBindingPicture()
		{
			if (Device != null)
			{
				return new BindingPicture(this);
			}
			return null;
		}

		public SubPicture CreateSubPicture(Picture refPicture, Rectangle rect)
		{
			if (Device != null)
			{
				return new SubPicture(this, refPicture, rect);
			}
			return null;
		}

		public MixPicture CreateMixPicture(Picture pictureA, Picture pictureB)
		{
			if (Device != null)
			{
				return new MixPicture(this, pictureA, pictureB);
			}
			return null;
		}

		public FAlphaPicture CreateFAlphaPicture(Picture pictureSource, FAlphaPicture.AlphaFunc func)
		{
			if (Device != null)
			{
				return new FAlphaPicture(this, pictureSource, func);
			}
			return null;
		}

		public HlslPicture CreateHlslPicture(Picture pictureSource)
		{
			if (Device != null)
			{
				return new HlslPicture(this, pictureSource);
			}
			return null;
		}

		public AnimePicture CreateAnimePicture(Size size)
		{
			if (Device != null)
			{
				return new AnimePicture(this, size);
			}
			return null;
		}

		public FreePicture CreateFreePicture()
		{
			if (Device != null)
			{
				return new FreePicture(this);
			}
			return null;
		}

		public Font2D CreateFont(System.Drawing.Font font)
		{
			if (Device != null)
			{
				return new Font2D(this, font);
			}
			return null;
		}

		public void Snap(string fileName)
		{
			snapBackBuffer = Device.GetBackBuffer(0, 0, BackBufferType.Mono);
			new Thread(SnapThreadProc).Start(fileName);
		}

		protected void SnapThreadProc(object fileName)
		{
			lock (this)
			{
				SurfaceLoader.Save((string)fileName, ImageFileFormat.Png, snapBackBuffer);
				snapBackBuffer.Dispose();
				snapBackBuffer = null;
			}
		}

		protected Stream SaveToStream(Picture picture, Rectangle rect)
		{
			return SurfaceLoader.SaveToStream(ImageFileFormat.Png, picture.Texture.GetSurfaceLevel(0), rect);
		}

		public void DrawBegin()
		{
			Device.BeginScene();
			Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1f, 0);
		}

		public void DrawPicture(Picture picture)
		{
			if (picture != null)
			{
				picture.Draw(Device, VB);
			}
		}

		public void DrawString(Font2D font, string text, int x, int y, Color color)
		{
			if (font != null)
			{
				font.DrawText(text, (int)(_targetBounds.X + (float)x * _targetBounds.Width + 0.5f), (int)(_targetBounds.Y + (float)y * _targetBounds.Height + 0.5f), color, true);
			}
		}

		public void DrawLine(int x1, int y1, int x2, int y2, Color color, int width)
		{
			Line.Width = (float)width * _targetBounds.Height;
			Line.Draw(new Vector2[2]
			{
				new Vector2(_targetBounds.X + (float)x1 * _targetBounds.Width, _targetBounds.Y + (float)y1 * _targetBounds.Height),
				new Vector2(_targetBounds.X + (float)x2 * _targetBounds.Width, _targetBounds.Y + (float)y2 * _targetBounds.Height)
			}, color);
		}

		public void DrawEnd()
		{
			Device.EndScene();
		}

		protected override void OnMouseDown(object sender, MouseEventArgs e)
		{
			MouseEventArgs e2 = new MouseEventArgs(e.Button, e.Clicks, (int)(((float)e.X - _targetBounds.X) / _targetBounds.Width + 0.5f), (int)(((float)e.Y - _targetBounds.Y) / _targetBounds.Height + 0.5f), e.Delta);
			base.OnMouseDown(sender, e2);
		}

		protected override void OnMouseUp(object sender, MouseEventArgs e)
		{
			MouseEventArgs e2 = new MouseEventArgs(e.Button, e.Clicks, (int)(((float)e.X - _targetBounds.X) / _targetBounds.Width + 0.5f), (int)(((float)e.Y - _targetBounds.Y) / _targetBounds.Height + 0.5f), e.Delta);
			base.OnMouseUp(sender, e2);
		}

		protected override void OnMouseMove(object sender, MouseEventArgs e)
		{
			MouseEventArgs e2 = new MouseEventArgs(e.Button, e.Clicks, (int)(((float)e.X - _targetBounds.X) / _targetBounds.Width + 0.5f), (int)(((float)e.Y - _targetBounds.Y) / _targetBounds.Height + 0.5f), e.Delta);
			base.OnMouseMove(sender, e2);
		}

		protected override void OnMouseWheel(object sender, MouseEventArgs e)
		{
			MouseEventArgs e2 = new MouseEventArgs(e.Button, e.Clicks, (int)(((float)e.X - _targetBounds.X) / _targetBounds.Width + 0.5f), (int)(((float)e.Y - _targetBounds.Y) / _targetBounds.Height + 0.5f), e.Delta);
			base.OnMouseWheel(sender, e2);
		}
	}
}
