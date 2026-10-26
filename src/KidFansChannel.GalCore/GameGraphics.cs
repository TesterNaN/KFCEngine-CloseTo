using System;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using KidFansChannel.Game2DCore;
using KidFansChannel.GameCore;

namespace KidFansChannel.GalCore
{
	public class GameGraphics : KidFansChannel.Game2DCore.Game2DCore
	{
		private GalCore galCore;

		private GameOptions.Level graphicsLevel = GameOptions.Level.None;

		public readonly Size BaseSize = new Size(800, 600);

		public new RenderEventHandler FrameRender;

		public RenderEventHandler PeripheryRender;

		public RenderEventHandler FramePrepare;

		private Picture[] picBorder = new Picture[4];

		private bool _shocking;

		protected Point originalLocation;

		private float shockingTime;

		protected CanvasPicture CapturePicture;

		private CanvasPicture tempPicture;

		[CompilerGenerated]
		private Resolution WindowResolution__BackingField;

		[CompilerGenerated]
		private Resolution FullscreenResolution__BackingField;

		public Resolution Resolution
		{
			get
			{
				if (IsWindowed)
				{
					return WindowResolution;
				}
				if (FullscreenResolution == Resolution.Empty)
				{
					int adapterOrdinal = Device.DeviceCaps.AdapterOrdinal;
					Resolution result = default(Resolution);
					result.Size = Screen.AllScreens[adapterOrdinal].Bounds.Size;
					result.Proportion = result.Size;
					result.FontScale = Math.Min((float)result.Size.Width / (float)BaseSize.Width, (float)result.Size.Height / (float)BaseSize.Height);
					Size size = new Size((int)((float)BaseSize.Width * result.FontScale), (int)((float)BaseSize.Height * result.FontScale));
					result.Bounds = new Rectangle((result.Size.Width - size.Width) / 2, (result.Size.Height - size.Height) / 2, size.Width, size.Height);
					result.StateArea = Rectangle.Empty;
					result.CommentArea = new Rectangle(result.Bounds.X, result.Bounds.Y, result.Bounds.Width, (int)Math.Ceiling((double)result.Bounds.Height / 15.0));
					return result;
				}
				return FullscreenResolution;
			}
		}

		public Resolution WindowResolution
		{
			[CompilerGenerated]
			get
			{
				return WindowResolution__BackingField;
			}
			[CompilerGenerated]
			set
			{
				WindowResolution__BackingField = value;
			}
		}

		public Resolution FullscreenResolution
		{
			[CompilerGenerated]
			get
			{
				return FullscreenResolution__BackingField;
			}
			[CompilerGenerated]
			set
			{
				FullscreenResolution__BackingField = value;
			}
		}

		public event EventHandler FullscreenToggled;

		public event RenderEventHandler FrameCapture;

		~GameGraphics()
		{
			if (CapturePicture != null)
			{
				CapturePicture.Dispose();
			}
			if (tempPicture != null)
			{
				tempPicture.Dispose();
			}
		}

		public void SetCore(GalCore gc)
		{
			galCore = gc;
			galCore.GameOptions.OptionsUpdate += OptionsUpdate;
		}

		private void OptionsUpdate(object sender, EventArgs e)
		{
			if (graphicsLevel == galCore.GameOptions.Graphics)
			{
				return;
			}
			if (galCore.GameOptions.Graphics == GameOptions.Level.High && Device.DeviceCaps.PixelShaderVersion < new Version(2, 0))
			{
				if (graphicsLevel != GameOptions.Level.None)
				{
					galCore.GameOptions.Graphics = graphicsLevel;
				}
				else
				{
					galCore.GameOptions.Graphics = GameOptions.Level.Mid;
				}
				return;
			}
			graphicsLevel = galCore.GameOptions.Graphics;
			if (graphicsLevel == GameOptions.Level.High)
			{
				MixPicture.SetShader(Device);
				FAlphaPicture.SetShader(Device);
				HlslPicture.SetShader(Device);
			}
			else
			{
				MixPicture.ClearShader();
				FAlphaPicture.ClearShader();
				HlslPicture.ClearShader();
			}
		}

		internal void GraphicsInit()
		{
			GameWindow.KeyDown += OnKeyDown;
			base.FrameRender += OnFrameRender;
			for (int i = 0; i < 4; i++)
			{
				picBorder[i] = CreatePicture(Color.Black);
			}
			PrepareBorder();
			if (!IsWindowed && FullscreenToggled != null)
			{
				FullscreenToggled(this, EventArgs.Empty);
			}
			CapturePicture = CreateCanvasPicture(BaseSize);
			tempPicture = CreateCanvasPicture(BaseSize);
		}

		private void OnFrameRender(object sender, RenderEventArgs e)
		{
			if (FramePrepare != null)
			{
				FramePrepare(sender, e);
			}
			DrawBegin();
			if (FrameRender != null)
			{
				FrameRender(sender, e);
			}
			if (galCore.GameVideo == null || !galCore.GameVideo.IsPlaying)
			{
				for (int i = 0; i < 4; i++)
				{
					DrawPicture(picBorder[i]);
				}
				if (PeripheryRender != null)
				{
					PeripheryRender(sender, e);
				}
			}
			DrawEnd();
		}

		private void OnKeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyData == (Keys.Return | Keys.Alt))
			{
				try
				{
					ToggleFullscreen();
				}
				catch
				{
					MessageBox.Show("Can't toggle full screen, wrong resolution?");
				}
			}
		}

		public override void ToggleFullscreen()
		{
			if (galCore.GameVideo == null || !galCore.GameVideo.IsPlaying)
			{
				if (_shocking && IsWindowed)
				{
					GameWindow.Location = originalLocation;
					shockingTime = 0f;
				}
				base.ToggleFullscreen();
				PrepareBorder();
				if (FullscreenToggled != null)
				{
					FullscreenToggled(this, EventArgs.Empty);
				}
			}
		}

		protected void PrepareBorder()
		{
			TargetBounds = new RectangleF(Resolution.Bounds.X, Resolution.Bounds.Y, (float)Resolution.Bounds.Width / (float)BaseSize.Width, (float)Resolution.Bounds.Height / (float)BaseSize.Height);
			FontScale = Resolution.FontScale;
			RectangleF clientBounds = GetClientBounds();
			picBorder[0].X = (int)Math.Floor(clientBounds.X);
			picBorder[0].Y = (int)Math.Floor(clientBounds.Y);
			picBorder[0].Width = (int)Math.Ceiling(clientBounds.Width);
			picBorder[0].Height = (int)Math.Ceiling(0f - clientBounds.Y);
			picBorder[1].X = (int)Math.Floor(clientBounds.X);
			picBorder[1].Y = BaseSize.Height;
			picBorder[1].Width = (int)Math.Ceiling(clientBounds.Width);
			picBorder[1].Height = (int)Math.Ceiling(clientBounds.Height - (float)BaseSize.Height);
			picBorder[2].X = (int)Math.Floor(clientBounds.X);
			picBorder[2].Y = 0;
			picBorder[2].Width = (int)Math.Ceiling(0f - clientBounds.X);
			picBorder[2].Height = BaseSize.Height;
			picBorder[3].X = BaseSize.Width;
			picBorder[3].Y = 0;
			picBorder[3].Width = (int)Math.Ceiling(clientBounds.Width - (float)BaseSize.Width);
			picBorder[3].Height = BaseSize.Height;
			for (int i = 0; i < 4; i++)
			{
				picBorder[i].Z = 0f;
			}
		}

		public void StartShock()
		{
			_shocking = true;
			if (IsWindowed)
			{
				originalLocation = GameWindow.Location;
				shockingTime = 0f;
			}
		}

		public void StopShock()
		{
			_shocking = false;
			if (IsWindowed)
			{
				GameWindow.Location = originalLocation;
			}
		}

		public void Update(double appTime, float elaspedTime)
		{
			if (_shocking && IsWindowed)
			{
				shockingTime += elaspedTime;
				while (shockingTime > 0.2f)
				{
					shockingTime -= 0.2f;
				}
				if (shockingTime < 0.025f)
				{
					GameWindow.Left = originalLocation.X - (int)(80f * shockingTime);
					GameWindow.Top = originalLocation.Y - (int)(80f * shockingTime);
				}
				else if (shockingTime < 0.075f)
				{
					GameWindow.Left = originalLocation.X - 2;
					GameWindow.Top = originalLocation.Y - 2 + (int)(80f * (shockingTime - 0.025f));
				}
				else if (shockingTime < 0.125f)
				{
					GameWindow.Left = originalLocation.X - 2 + (int)(80f * (shockingTime - 0.075f));
					GameWindow.Top = originalLocation.Y + 2 - (int)(80f * (shockingTime - 0.075f));
				}
				else if (shockingTime < 0.175f)
				{
					GameWindow.Left = originalLocation.X + 2;
					GameWindow.Top = originalLocation.Y - 2 + (int)(80f * (shockingTime - 0.125f));
				}
				else
				{
					GameWindow.Left = originalLocation.X + 2 - (int)(80f * (shockingTime - 0.175f));
					GameWindow.Top = originalLocation.Y + 2 - (int)(80f * (shockingTime - 0.175f));
				}
			}
		}

		public Stream Capture(Size size)
		{
			ICanvas currentCanvas = galCore.CurrentCanvas;
			galCore.CurrentCanvas = tempPicture;
			tempPicture.DrawBegin();
			if (FrameCapture != null)
			{
				FrameCapture(this, new RenderEventArgs(0.0, 0f));
			}
			tempPicture.DrawEnd();
			tempPicture.Scale = new SizeF((float)size.Width / (float)tempPicture.Width, (float)size.Height / (float)tempPicture.Height);
			CapturePicture.DrawBegin();
			CapturePicture.DrawPicture(tempPicture);
			CapturePicture.DrawEnd();
			galCore.CurrentCanvas = currentCanvas;
			return SaveToStream(CapturePicture, new Rectangle(Point.Empty, size));
		}
	}
}
