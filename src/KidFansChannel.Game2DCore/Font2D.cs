using System;
using System.Drawing;
using Microsoft.DirectX.Direct3D;

namespace KidFansChannel.Game2DCore
{
	public class Font2D : IDisposable
	{
		private Microsoft.DirectX.Direct3D.Font _font;

		private Microsoft.DirectX.Direct3D.Font _drawFont;

		private System.Drawing.Font originalFont;

		private Game2DCore _core;

		private float lastScale;

		internal Microsoft.DirectX.Direct3D.Font Font
		{
			get
			{
				return _font;
			}
		}

		internal Microsoft.DirectX.Direct3D.Font DrawFont
		{
			get
			{
				return _drawFont;
			}
		}

		public Game2DCore Core
		{
			get
			{
				return _core;
			}
		}

		public float Size
		{
			get
			{
				return originalFont.Size;
			}
		}

		internal Font2D(Game2DCore core, System.Drawing.Font font)
		{
			_core = core;
			core.FontList.Add(this);
			_font = new Microsoft.DirectX.Direct3D.Font(Core.Device, new System.Drawing.Font(font.FontFamily, font.Size));
			_drawFont = new Microsoft.DirectX.Direct3D.Font(Core.Device, new System.Drawing.Font(font.FontFamily, font.Size * Core.FontScale));
			originalFont = font;
			lastScale = core.FontScale;
		}

		public Size Measure(string s)
		{
			return _font.MeasureString(null, s, DrawTextFormat.None, 0).Size;
		}

		public void DrawText(string text, int x, int y, Color color, bool target)
		{
			if (lastScale != Core.FontScale)
			{
				if (_drawFont != null)
				{
					_drawFont.Dispose();
				}
				_drawFont = new Microsoft.DirectX.Direct3D.Font(Core.Device, new System.Drawing.Font(originalFont.FontFamily, originalFont.Size * Core.FontScale));
				lastScale = Core.FontScale;
			}
			if (_drawFont != null && !_drawFont.Disposed)
			{
				if (target)
				{
					_drawFont.DrawText(null, text, x, y, color);
				}
				else
				{
					_font.DrawText(null, text, x, y, color);
				}
			}
		}

		public void OnResetDevice()
		{
			if (_font != null && !_font.Disposed)
			{
				_font.OnResetDevice();
			}
			if (_drawFont != null && !_drawFont.Disposed)
			{
				_drawFont.OnResetDevice();
			}
		}

		public void OnLostDevice()
		{
			if (!_font.Disposed)
			{
				_font.OnLostDevice();
			}
			if (!_drawFont.Disposed)
			{
				_drawFont.OnLostDevice();
			}
		}

		public void Dispose()
		{
			if (_font != null)
			{
				_font.Dispose();
				_font = null;
			}
			if (_drawFont != null)
			{
				_drawFont.Dispose();
				_drawFont = null;
			}
			if (originalFont != null)
			{
				originalFont.Dispose();
				originalFont = null;
			}
			Core.FontList.Remove(this);
		}
	}
}
