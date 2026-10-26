using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace KidFansChannel.GameCore
{
	public class Window : Form
	{
		private IContainer components;

		private GameCore gameCore;

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			base.SuspendLayout();
			base.ClientSize = new System.Drawing.Size(284, 264);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			base.MaximizeBox = false;
			base.Name = "Window";
			base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			base.ResumeLayout(false);
		}

		public Window(GameCore gc)
		{
			gameCore = gc;
			InitializeComponent();
			BackColor = Color.Black;
		}

		protected override void WndProc(ref Message m)
		{
			gameCore.WindowsProcedure(ref m);
			if (!(m.Result == new IntPtr(1)))
			{
				base.WndProc(ref m);
			}
		}

		public void SetScreen(int index)
		{
			Point position = default(Point);
			Rectangle bounds = Screen.AllScreens[index].Bounds;
			position.X = (bounds.Left + bounds.Right) / 2;
			position.Y = (bounds.Top + bounds.Bottom) / 2;
			Cursor.Position = position;
			CenterToScreen();
		}
	}
}
