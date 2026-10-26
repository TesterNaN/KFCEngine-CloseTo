using System;
using System.Drawing;
using System.Threading;
using KidFansChannel.Game2DCore;
using KidFansChannel.VideoCore;

namespace KidFansChannel.GalCore
{
	public class GameVideo : KidFansChannel.VideoCore.VideoCore
	{
		private GalCore _galCore;

		private FreePicture VideoPicture;

		private bool readyToDraw;

		private PointF[] uvPoints = new PointF[4];

		public void SetCore(GalCore galCore)
		{
			_galCore = galCore;
			VideoPicture = _galCore.GameGraphics.CreateFreePicture();
			VideoPicture.Size = new Size(800, 600);
			Initial(_galCore.GameGraphics.Device, _galCore.GameGraphics.GameWindow.Handle);
			Stopping += GameVideo_Stopping;
		}

		~GameVideo()
		{
			Final();
			VideoPicture.Dispose();
			VideoPicture = null;
		}

		public override void Play(string fileName)
		{
			GC.Collect();
			Thread.CurrentThread.Priority = ThreadPriority.Lowest;
			readyToDraw = false;
			_galCore.GameGraphics.ToggleManual(true);
			base.Play(fileName);
		}

		public override void Stop()
		{
			readyToDraw = false;
			base.Stop();
			Thread.CurrentThread.Priority = ThreadPriority.Normal;
			_galCore.GameGraphics.ToggleManual(false);
		}

		public void Update()
		{
			if (Texture != null)
			{
				VideoPicture.SetTexture(Texture);
				for (int i = 0; i < 4; i++)
				{
					uvPoints[i].X = (((i & 1) == 0) ? 0f : UV.Width);
					uvPoints[i].Y = ((i >> 1 == 0) ? 0f : UV.Height);
				}
				VideoPicture.SetUV(uvPoints);
				readyToDraw = true;
			}
			else
			{
				readyToDraw = false;
			}
		}

		public void Draw()
		{
			if (readyToDraw)
			{
				_galCore.GameGraphics.DrawPicture(VideoPicture);
			}
			readyToDraw = false;
		}

		protected override void OnRender(object sender, EventArgs e)
		{
			base.OnRender(sender, e);
			_galCore.GameGraphics.RenderEnvironment();
		}

		private void GameVideo_Stopping(object sender, EventArgs e)
		{
			Stop();
		}
	}
}
