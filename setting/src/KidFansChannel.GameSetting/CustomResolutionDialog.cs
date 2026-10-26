using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using KidFansChannel.GalCore;

namespace KidFansChannel.GameSetting
{
	public class CustomResolutionDialog : Form
	{
		private bool needPreview = true;

		private bool needCheck = true;

		private IContainer components;

		private GroupBox gpbScreen;

		private Label lblScreenX;

		private NumericUpDown nudScreenHeight;

		private Label lblScreenHeight;

		private NumericUpDown nudScreenWidth;

		private Label lblScreenWidth;

		private NumericUpDown nudScreenY;

		private Label lblScreenY;

		private NumericUpDown nudScreenX;

		private GroupBox gpbResolution;

		private NumericUpDown nudResolutionHeight;

		private Label lblResolutionHeight;

		private NumericUpDown nudResolutionWidth;

		private Label lblResolutionWidth;

		private GroupBox gpbState;

		private NumericUpDown nudStateHeight;

		private Label lblStateHeight;

		private NumericUpDown nudStateWidth;

		private Label lblStateWidth;

		private NumericUpDown nudStateY;

		private Label lblStateY;

		private NumericUpDown nudStateX;

		private Label lblStateX;

		private GroupBox gpbComment;

		private Label lblCommentWidth;

		private NumericUpDown nudCommentY;

		private Label lblCommentY;

		private NumericUpDown nudCommentX;

		private Label lblCommentX;

		private NumericUpDown nudCommentHeight;

		private Label lblCommentHeight;

		private NumericUpDown nudCommentWidth;

		private Button btnOk;

		private Button btnCancel;

		private PictureBox ptbPreview;

		public Resolution Resolution
		{
			get
			{
				Resolution result = default(Resolution);
				result.Size = new Size((int)nudResolutionWidth.Value, (int)nudResolutionHeight.Value);
				float num = (float)result.Size.Width / (float)result.Size.Height;
				if (Math.Abs(num - 1.3333334f) < 0.01f)
				{
					result.Proportion = new Size(4, 3);
				}
				else if (Math.Abs(num - 1.6f) < 0.01f)
				{
					result.Proportion = new Size(16, 10);
				}
				else if (Math.Abs(num - 1.7777778f) < 0.01f)
				{
					result.Proportion = new Size(16, 9);
				}
				else if (Math.Abs(num - 1.25f) < 0.01f)
				{
					result.Proportion = new Size(5, 4);
				}
				else if (Math.Abs(num - 1.6666666f) < 0.01f)
				{
					result.Proportion = new Size(5, 3);
				}
				else
				{
					result.Proportion = result.Size;
				}
				result.Bounds = new Rectangle((int)nudScreenX.Value, (int)nudScreenY.Value, (int)nudScreenWidth.Value, (int)nudScreenHeight.Value);
				if (nudStateWidth.Value == 0m || nudStateHeight.Value == 0m)
				{
					result.StateArea = Rectangle.Empty;
				}
				else
				{
					result.StateArea = new Rectangle((int)nudStateX.Value, (int)nudStateY.Value, (int)nudStateWidth.Value, (int)nudStateHeight.Value);
				}
				if (nudCommentWidth.Value == 0m || nudCommentHeight.Value == 0m)
				{
					result.CommentArea = Rectangle.Empty;
				}
				else
				{
					result.CommentArea = new Rectangle((int)nudCommentX.Value, (int)nudCommentY.Value, (int)nudCommentWidth.Value, (int)nudCommentHeight.Value);
				}
				result.FontScale = (float)result.Bounds.Width / 800f;
				return result;
			}
			set
			{
				needPreview = false;
				nudResolutionWidth.Value = value.Size.Width;
				nudResolutionHeight.Value = value.Size.Height;
				nudScreenX.Value = value.Bounds.X;
				nudScreenY.Value = value.Bounds.Y;
				nudScreenWidth.Value = value.Bounds.Width;
				nudScreenHeight.Value = value.Bounds.Height;
				nudStateX.Value = value.StateArea.X;
				nudStateY.Value = value.StateArea.Y;
				nudStateWidth.Value = value.StateArea.Width;
				nudStateHeight.Value = value.StateArea.Height;
				nudCommentX.Value = value.CommentArea.X;
				nudCommentY.Value = value.CommentArea.Y;
				nudCommentWidth.Value = value.CommentArea.Width;
				nudCommentHeight.Value = value.CommentArea.Height;
				needPreview = true;
				Preview();
			}
		}

		public CustomResolutionDialog()
		{
			InitializeComponent();
			ptbPreview.BackgroundImage = new Bitmap(ptbPreview.Width, ptbPreview.Height);
		}

		private void Preview()
		{
			if (!needPreview)
			{
				return;
			}
			using (Graphics graphics = Graphics.FromImage(ptbPreview.BackgroundImage))
			{
				graphics.Clear(Color.Transparent);
				Rectangle rect = default(Rectangle);
				double val = (double)ptbPreview.Width / (double)(int)nudResolutionWidth.Value;
				double val2 = (double)ptbPreview.Height / (double)(int)nudResolutionHeight.Value;
				double num = Math.Min(val, val2);
				rect.Width = (int)((double)(int)nudResolutionWidth.Value * num + 0.5);
				rect.Height = (int)((double)(int)nudResolutionHeight.Value * num + 0.5);
				rect.X = (ptbPreview.Width - rect.Width) / 2;
				rect.Y = (ptbPreview.Height - rect.Height) / 2;
				graphics.FillRectangle(new SolidBrush(Color.Black), rect);
				Rectangle rect2 = default(Rectangle);
				rect2.X = (int)((double)(int)nudScreenX.Value * num + (double)rect.X + 0.5);
				rect2.Y = (int)((double)(int)nudScreenY.Value * num + (double)rect.Y + 0.5);
				rect2.Width = (int)((double)(int)nudScreenWidth.Value * num + 0.5);
				rect2.Height = (int)((double)(int)nudScreenHeight.Value * num + 0.5);
				graphics.FillRectangle(new SolidBrush(Color.Gray), rect2);
				if (nudStateWidth.Value != 0m && nudStateHeight.Value != 0m)
				{
					Rectangle rectangle = default(Rectangle);
					rectangle.X = (int)((double)(int)nudStateX.Value * num + (double)rect.X + 0.5);
					rectangle.Y = (int)((double)(int)nudStateY.Value * num + (double)rect.Y + 0.5);
					rectangle.Width = (int)((double)(int)nudStateWidth.Value * num + 0.5);
					rectangle.Height = (int)((double)(int)nudStateHeight.Value * num + 0.5);
					if (rectangle.Width / 2 != 0)
					{
						graphics.FillRectangle(new LinearGradientBrush(new Point(rectangle.X, rectangle.Y), new Point(rectangle.X + rectangle.Width / 2, rectangle.Y), Color.White, Color.Transparent), rectangle.X, rectangle.Y, rectangle.Width / 2, rectangle.Height);
						graphics.FillRectangle(new LinearGradientBrush(new Point(rectangle.X + rectangle.Width, rectangle.Y), new Point(rectangle.X + rectangle.Width / 2 - 1, rectangle.Y), Color.White, Color.Transparent), rectangle.X + rectangle.Width / 2, rectangle.Y, rectangle.Width / 2, rectangle.Height);
					}
				}
				if (nudCommentWidth.Value != 0m && nudCommentHeight.Value != 0m)
				{
					Rectangle rect3 = default(Rectangle);
					rect3.X = (int)((double)(int)nudCommentX.Value * num + (double)rect.X + 0.5);
					rect3.Y = (int)((double)(int)nudCommentY.Value * num + (double)rect.Y + 0.5);
					rect3.Width = (int)((double)(int)nudCommentWidth.Value * num + 0.5);
					rect3.Height = (int)((double)(int)nudCommentHeight.Value * num + 0.5);
					if (rect3.Height != 0)
					{
						graphics.FillRectangle(new LinearGradientBrush(rect3, Color.Transparent, Color.White, LinearGradientMode.Vertical), rect3);
					}
				}
			}
			ptbPreview.Refresh();
		}

		private void Resolution_ValueChanged(object sender, EventArgs e)
		{
			if (needCheck)
			{
				needCheck = false;
				decimal value = ((NumericUpDown)sender).Value;
				if (sender == nudScreenWidth)
				{
					nudScreenHeight.Value = (int)(nudScreenWidth.Value * 3m / 4m + 0.5m);
				}
				else if (sender == nudScreenHeight)
				{
					nudScreenWidth.Value = (int)(nudScreenHeight.Value * 4m / 3m + 0.5m);
				}
				if (sender == nudScreenX && value > nudResolutionWidth.Value - nudScreenWidth.Value)
				{
					if (value > nudResolutionWidth.Value)
					{
						nudScreenX.Value = nudResolutionWidth.Value;
						nudScreenWidth.Value = 0m;
					}
					else
					{
						nudScreenWidth.Value = nudResolutionWidth.Value - nudScreenX.Value;
					}
				}
				else if (sender == nudStateX && value > nudResolutionWidth.Value - nudStateWidth.Value)
				{
					if (value > nudResolutionWidth.Value)
					{
						nudStateX.Value = nudResolutionWidth.Value;
						nudStateWidth.Value = 0m;
					}
					else
					{
						nudStateWidth.Value = nudResolutionWidth.Value - nudStateX.Value;
					}
				}
				else if (sender == nudCommentX && value > nudResolutionWidth.Value - nudCommentWidth.Value)
				{
					if (value > nudResolutionWidth.Value)
					{
						nudCommentX.Value = nudResolutionWidth.Value;
						nudCommentWidth.Value = 0m;
					}
					else
					{
						nudCommentWidth.Value = nudResolutionWidth.Value - nudCommentX.Value;
					}
				}
				if (sender == nudScreenY && value > nudResolutionHeight.Value - nudScreenHeight.Value)
				{
					if (value > nudResolutionHeight.Value)
					{
						nudScreenY.Value = nudResolutionHeight.Value;
						nudScreenHeight.Value = 0m;
					}
					else
					{
						nudScreenHeight.Value = nudResolutionHeight.Value - nudScreenY.Value;
					}
				}
				else if (sender == nudStateY && value > nudResolutionHeight.Value - nudStateHeight.Value)
				{
					if (value > nudResolutionHeight.Value)
					{
						nudStateY.Value = nudResolutionHeight.Value;
						nudStateHeight.Value = 0m;
					}
					else
					{
						nudStateHeight.Value = nudResolutionHeight.Value - nudStateY.Value;
					}
				}
				else if (sender == nudCommentY && value > nudResolutionHeight.Value - nudCommentHeight.Value)
				{
					if (value > nudResolutionHeight.Value)
					{
						nudCommentY.Value = nudResolutionHeight.Value;
						nudCommentHeight.Value = 0m;
					}
					else
					{
						nudCommentHeight.Value = nudResolutionHeight.Value - nudCommentY.Value;
					}
				}
				else if (sender == nudScreenWidth && ((NumericUpDown)sender).Value > nudResolutionWidth.Value - nudScreenX.Value)
				{
					((NumericUpDown)sender).Value = nudResolutionWidth.Value - nudScreenX.Value;
				}
				else if (sender == nudStateWidth && ((NumericUpDown)sender).Value > nudResolutionWidth.Value - nudStateX.Value)
				{
					((NumericUpDown)sender).Value = nudResolutionWidth.Value - nudStateX.Value;
				}
				else if (sender == nudCommentWidth && ((NumericUpDown)sender).Value > nudResolutionWidth.Value - nudCommentX.Value)
				{
					((NumericUpDown)sender).Value = nudResolutionWidth.Value - nudCommentX.Value;
				}
				else if (sender == nudScreenHeight && ((NumericUpDown)sender).Value > nudResolutionHeight.Value - nudScreenY.Value)
				{
					((NumericUpDown)sender).Value = nudResolutionHeight.Value - nudScreenY.Value;
				}
				else if (sender == nudStateHeight && ((NumericUpDown)sender).Value > nudResolutionHeight.Value - nudStateY.Value)
				{
					((NumericUpDown)sender).Value = nudResolutionHeight.Value - nudStateY.Value;
				}
				else if (sender == nudCommentHeight && ((NumericUpDown)sender).Value > nudResolutionHeight.Value - nudCommentY.Value)
				{
					((NumericUpDown)sender).Value = nudResolutionHeight.Value - nudCommentY.Value;
				}
				else if (sender == nudResolutionWidth)
				{
					if (nudScreenX.Value > value)
					{
						nudScreenX.Value = value;
					}
					if (nudScreenWidth.Value > value - nudScreenX.Value)
					{
						nudScreenWidth.Value = value - nudScreenX.Value;
					}
					if (nudStateX.Value > value)
					{
						nudStateX.Value = value;
					}
					if (nudStateWidth.Value > value - nudStateX.Value)
					{
						nudStateWidth.Value = value - nudStateX.Value;
					}
					if (nudCommentX.Value > value)
					{
						nudCommentX.Value = value;
					}
					if (nudCommentWidth.Value > value - nudCommentX.Value)
					{
						nudCommentWidth.Value = value - nudCommentX.Value;
					}
				}
				else if (sender == nudResolutionHeight)
				{
					if (nudScreenY.Value > value)
					{
						nudScreenY.Value = value;
					}
					if (nudScreenHeight.Value > value - nudScreenY.Value)
					{
						nudScreenHeight.Value = value - nudScreenY.Value;
					}
					if (nudStateY.Value > value)
					{
						nudStateY.Value = value;
					}
					if (nudStateHeight.Value > value - nudStateY.Value)
					{
						nudStateHeight.Value = value - nudStateY.Value;
					}
					if (nudCommentY.Value > value)
					{
						nudCommentY.Value = value;
					}
					if (nudCommentHeight.Value > value - nudCommentY.Value)
					{
						nudCommentHeight.Value = value - nudCommentY.Value;
					}
				}
				if (nudScreenWidth.Value * 3m < nudScreenHeight.Value * 4m)
				{
					nudScreenHeight.Value = (int)(nudScreenWidth.Value * 3m / 4m + 0.5m);
				}
				else if (nudScreenWidth.Value * 3m > nudScreenHeight.Value * 4m)
				{
					nudScreenWidth.Value = (int)(nudScreenHeight.Value * 4m / 3m + 0.5m);
				}
				needCheck = true;
			}
			Preview();
		}

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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(KidFansChannel.GameSetting.CustomResolutionDialog));
			this.gpbScreen = new System.Windows.Forms.GroupBox();
			this.nudScreenHeight = new System.Windows.Forms.NumericUpDown();
			this.lblScreenHeight = new System.Windows.Forms.Label();
			this.nudScreenWidth = new System.Windows.Forms.NumericUpDown();
			this.lblScreenWidth = new System.Windows.Forms.Label();
			this.nudScreenY = new System.Windows.Forms.NumericUpDown();
			this.lblScreenY = new System.Windows.Forms.Label();
			this.nudScreenX = new System.Windows.Forms.NumericUpDown();
			this.lblScreenX = new System.Windows.Forms.Label();
			this.gpbResolution = new System.Windows.Forms.GroupBox();
			this.nudResolutionHeight = new System.Windows.Forms.NumericUpDown();
			this.lblResolutionHeight = new System.Windows.Forms.Label();
			this.nudResolutionWidth = new System.Windows.Forms.NumericUpDown();
			this.lblResolutionWidth = new System.Windows.Forms.Label();
			this.gpbState = new System.Windows.Forms.GroupBox();
			this.nudStateHeight = new System.Windows.Forms.NumericUpDown();
			this.lblStateHeight = new System.Windows.Forms.Label();
			this.nudStateWidth = new System.Windows.Forms.NumericUpDown();
			this.lblStateWidth = new System.Windows.Forms.Label();
			this.nudStateY = new System.Windows.Forms.NumericUpDown();
			this.lblStateY = new System.Windows.Forms.Label();
			this.nudStateX = new System.Windows.Forms.NumericUpDown();
			this.lblStateX = new System.Windows.Forms.Label();
			this.gpbComment = new System.Windows.Forms.GroupBox();
			this.nudCommentHeight = new System.Windows.Forms.NumericUpDown();
			this.lblCommentHeight = new System.Windows.Forms.Label();
			this.nudCommentWidth = new System.Windows.Forms.NumericUpDown();
			this.lblCommentWidth = new System.Windows.Forms.Label();
			this.nudCommentY = new System.Windows.Forms.NumericUpDown();
			this.lblCommentY = new System.Windows.Forms.Label();
			this.nudCommentX = new System.Windows.Forms.NumericUpDown();
			this.lblCommentX = new System.Windows.Forms.Label();
			this.btnOk = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.ptbPreview = new System.Windows.Forms.PictureBox();
			this.gpbScreen.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.nudScreenHeight).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.nudScreenWidth).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.nudScreenY).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.nudScreenX).BeginInit();
			this.gpbResolution.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.nudResolutionHeight).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.nudResolutionWidth).BeginInit();
			this.gpbState.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.nudStateHeight).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.nudStateWidth).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.nudStateY).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.nudStateX).BeginInit();
			this.gpbComment.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.nudCommentHeight).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.nudCommentWidth).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.nudCommentY).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.nudCommentX).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.ptbPreview).BeginInit();
			base.SuspendLayout();
			this.gpbScreen.AccessibleDescription = null;
			this.gpbScreen.AccessibleName = null;
			resources.ApplyResources(this.gpbScreen, "gpbScreen");
			this.gpbScreen.BackgroundImage = null;
			this.gpbScreen.Controls.Add(this.nudScreenHeight);
			this.gpbScreen.Controls.Add(this.lblScreenHeight);
			this.gpbScreen.Controls.Add(this.nudScreenWidth);
			this.gpbScreen.Controls.Add(this.lblScreenWidth);
			this.gpbScreen.Controls.Add(this.nudScreenY);
			this.gpbScreen.Controls.Add(this.lblScreenY);
			this.gpbScreen.Controls.Add(this.nudScreenX);
			this.gpbScreen.Controls.Add(this.lblScreenX);
			this.gpbScreen.Font = null;
			this.gpbScreen.Name = "gpbScreen";
			this.gpbScreen.TabStop = false;
			this.nudScreenHeight.AccessibleDescription = null;
			this.nudScreenHeight.AccessibleName = null;
			resources.ApplyResources(this.nudScreenHeight, "nudScreenHeight");
			this.nudScreenHeight.Font = null;
			this.nudScreenHeight.Maximum = new decimal(new int[4] { 9999, 0, 0, 0 });
			this.nudScreenHeight.Name = "nudScreenHeight";
			this.nudScreenHeight.ValueChanged += new System.EventHandler(this.Resolution_ValueChanged);
			this.lblScreenHeight.AccessibleDescription = null;
			this.lblScreenHeight.AccessibleName = null;
			resources.ApplyResources(this.lblScreenHeight, "lblScreenHeight");
			this.lblScreenHeight.Font = null;
			this.lblScreenHeight.Name = "lblScreenHeight";
			this.nudScreenWidth.AccessibleDescription = null;
			this.nudScreenWidth.AccessibleName = null;
			resources.ApplyResources(this.nudScreenWidth, "nudScreenWidth");
			this.nudScreenWidth.Font = null;
			this.nudScreenWidth.Maximum = new decimal(new int[4] { 9999, 0, 0, 0 });
			this.nudScreenWidth.Name = "nudScreenWidth";
			this.nudScreenWidth.ValueChanged += new System.EventHandler(this.Resolution_ValueChanged);
			this.lblScreenWidth.AccessibleDescription = null;
			this.lblScreenWidth.AccessibleName = null;
			resources.ApplyResources(this.lblScreenWidth, "lblScreenWidth");
			this.lblScreenWidth.Font = null;
			this.lblScreenWidth.Name = "lblScreenWidth";
			this.nudScreenY.AccessibleDescription = null;
			this.nudScreenY.AccessibleName = null;
			resources.ApplyResources(this.nudScreenY, "nudScreenY");
			this.nudScreenY.Font = null;
			this.nudScreenY.Maximum = new decimal(new int[4] { 9999, 0, 0, 0 });
			this.nudScreenY.Name = "nudScreenY";
			this.nudScreenY.ValueChanged += new System.EventHandler(this.Resolution_ValueChanged);
			this.lblScreenY.AccessibleDescription = null;
			this.lblScreenY.AccessibleName = null;
			resources.ApplyResources(this.lblScreenY, "lblScreenY");
			this.lblScreenY.Font = null;
			this.lblScreenY.Name = "lblScreenY";
			this.nudScreenX.AccessibleDescription = null;
			this.nudScreenX.AccessibleName = null;
			resources.ApplyResources(this.nudScreenX, "nudScreenX");
			this.nudScreenX.Font = null;
			this.nudScreenX.Maximum = new decimal(new int[4] { 9999, 0, 0, 0 });
			this.nudScreenX.Name = "nudScreenX";
			this.nudScreenX.ValueChanged += new System.EventHandler(this.Resolution_ValueChanged);
			this.lblScreenX.AccessibleDescription = null;
			this.lblScreenX.AccessibleName = null;
			resources.ApplyResources(this.lblScreenX, "lblScreenX");
			this.lblScreenX.Font = null;
			this.lblScreenX.Name = "lblScreenX";
			this.gpbResolution.AccessibleDescription = null;
			this.gpbResolution.AccessibleName = null;
			resources.ApplyResources(this.gpbResolution, "gpbResolution");
			this.gpbResolution.BackgroundImage = null;
			this.gpbResolution.Controls.Add(this.nudResolutionHeight);
			this.gpbResolution.Controls.Add(this.lblResolutionHeight);
			this.gpbResolution.Controls.Add(this.nudResolutionWidth);
			this.gpbResolution.Controls.Add(this.lblResolutionWidth);
			this.gpbResolution.Font = null;
			this.gpbResolution.Name = "gpbResolution";
			this.gpbResolution.TabStop = false;
			this.nudResolutionHeight.AccessibleDescription = null;
			this.nudResolutionHeight.AccessibleName = null;
			resources.ApplyResources(this.nudResolutionHeight, "nudResolutionHeight");
			this.nudResolutionHeight.Font = null;
			this.nudResolutionHeight.Maximum = new decimal(new int[4] { 9999, 0, 0, 0 });
			this.nudResolutionHeight.Name = "nudResolutionHeight";
			this.nudResolutionHeight.ValueChanged += new System.EventHandler(this.Resolution_ValueChanged);
			this.lblResolutionHeight.AccessibleDescription = null;
			this.lblResolutionHeight.AccessibleName = null;
			resources.ApplyResources(this.lblResolutionHeight, "lblResolutionHeight");
			this.lblResolutionHeight.Font = null;
			this.lblResolutionHeight.Name = "lblResolutionHeight";
			this.nudResolutionWidth.AccessibleDescription = null;
			this.nudResolutionWidth.AccessibleName = null;
			resources.ApplyResources(this.nudResolutionWidth, "nudResolutionWidth");
			this.nudResolutionWidth.Font = null;
			this.nudResolutionWidth.Maximum = new decimal(new int[4] { 9999, 0, 0, 0 });
			this.nudResolutionWidth.Name = "nudResolutionWidth";
			this.nudResolutionWidth.ValueChanged += new System.EventHandler(this.Resolution_ValueChanged);
			this.lblResolutionWidth.AccessibleDescription = null;
			this.lblResolutionWidth.AccessibleName = null;
			resources.ApplyResources(this.lblResolutionWidth, "lblResolutionWidth");
			this.lblResolutionWidth.Font = null;
			this.lblResolutionWidth.Name = "lblResolutionWidth";
			this.gpbState.AccessibleDescription = null;
			this.gpbState.AccessibleName = null;
			resources.ApplyResources(this.gpbState, "gpbState");
			this.gpbState.BackgroundImage = null;
			this.gpbState.Controls.Add(this.nudStateHeight);
			this.gpbState.Controls.Add(this.lblStateHeight);
			this.gpbState.Controls.Add(this.nudStateWidth);
			this.gpbState.Controls.Add(this.lblStateWidth);
			this.gpbState.Controls.Add(this.nudStateY);
			this.gpbState.Controls.Add(this.lblStateY);
			this.gpbState.Controls.Add(this.nudStateX);
			this.gpbState.Controls.Add(this.lblStateX);
			this.gpbState.Font = null;
			this.gpbState.Name = "gpbState";
			this.gpbState.TabStop = false;
			this.nudStateHeight.AccessibleDescription = null;
			this.nudStateHeight.AccessibleName = null;
			resources.ApplyResources(this.nudStateHeight, "nudStateHeight");
			this.nudStateHeight.Font = null;
			this.nudStateHeight.Maximum = new decimal(new int[4] { 9999, 0, 0, 0 });
			this.nudStateHeight.Name = "nudStateHeight";
			this.nudStateHeight.ValueChanged += new System.EventHandler(this.Resolution_ValueChanged);
			this.lblStateHeight.AccessibleDescription = null;
			this.lblStateHeight.AccessibleName = null;
			resources.ApplyResources(this.lblStateHeight, "lblStateHeight");
			this.lblStateHeight.Font = null;
			this.lblStateHeight.Name = "lblStateHeight";
			this.nudStateWidth.AccessibleDescription = null;
			this.nudStateWidth.AccessibleName = null;
			resources.ApplyResources(this.nudStateWidth, "nudStateWidth");
			this.nudStateWidth.Font = null;
			this.nudStateWidth.Maximum = new decimal(new int[4] { 9999, 0, 0, 0 });
			this.nudStateWidth.Name = "nudStateWidth";
			this.nudStateWidth.ValueChanged += new System.EventHandler(this.Resolution_ValueChanged);
			this.lblStateWidth.AccessibleDescription = null;
			this.lblStateWidth.AccessibleName = null;
			resources.ApplyResources(this.lblStateWidth, "lblStateWidth");
			this.lblStateWidth.Font = null;
			this.lblStateWidth.Name = "lblStateWidth";
			this.nudStateY.AccessibleDescription = null;
			this.nudStateY.AccessibleName = null;
			resources.ApplyResources(this.nudStateY, "nudStateY");
			this.nudStateY.Font = null;
			this.nudStateY.Maximum = new decimal(new int[4] { 9999, 0, 0, 0 });
			this.nudStateY.Name = "nudStateY";
			this.nudStateY.ValueChanged += new System.EventHandler(this.Resolution_ValueChanged);
			this.lblStateY.AccessibleDescription = null;
			this.lblStateY.AccessibleName = null;
			resources.ApplyResources(this.lblStateY, "lblStateY");
			this.lblStateY.Font = null;
			this.lblStateY.Name = "lblStateY";
			this.nudStateX.AccessibleDescription = null;
			this.nudStateX.AccessibleName = null;
			resources.ApplyResources(this.nudStateX, "nudStateX");
			this.nudStateX.Font = null;
			this.nudStateX.Maximum = new decimal(new int[4] { 9999, 0, 0, 0 });
			this.nudStateX.Name = "nudStateX";
			this.nudStateX.ValueChanged += new System.EventHandler(this.Resolution_ValueChanged);
			this.lblStateX.AccessibleDescription = null;
			this.lblStateX.AccessibleName = null;
			resources.ApplyResources(this.lblStateX, "lblStateX");
			this.lblStateX.Font = null;
			this.lblStateX.Name = "lblStateX";
			this.gpbComment.AccessibleDescription = null;
			this.gpbComment.AccessibleName = null;
			resources.ApplyResources(this.gpbComment, "gpbComment");
			this.gpbComment.BackgroundImage = null;
			this.gpbComment.Controls.Add(this.nudCommentHeight);
			this.gpbComment.Controls.Add(this.lblCommentHeight);
			this.gpbComment.Controls.Add(this.nudCommentWidth);
			this.gpbComment.Controls.Add(this.lblCommentWidth);
			this.gpbComment.Controls.Add(this.nudCommentY);
			this.gpbComment.Controls.Add(this.lblCommentY);
			this.gpbComment.Controls.Add(this.nudCommentX);
			this.gpbComment.Controls.Add(this.lblCommentX);
			this.gpbComment.Font = null;
			this.gpbComment.Name = "gpbComment";
			this.gpbComment.TabStop = false;
			this.nudCommentHeight.AccessibleDescription = null;
			this.nudCommentHeight.AccessibleName = null;
			resources.ApplyResources(this.nudCommentHeight, "nudCommentHeight");
			this.nudCommentHeight.Font = null;
			this.nudCommentHeight.Maximum = new decimal(new int[4] { 9999, 0, 0, 0 });
			this.nudCommentHeight.Name = "nudCommentHeight";
			this.nudCommentHeight.ValueChanged += new System.EventHandler(this.Resolution_ValueChanged);
			this.lblCommentHeight.AccessibleDescription = null;
			this.lblCommentHeight.AccessibleName = null;
			resources.ApplyResources(this.lblCommentHeight, "lblCommentHeight");
			this.lblCommentHeight.Font = null;
			this.lblCommentHeight.Name = "lblCommentHeight";
			this.nudCommentWidth.AccessibleDescription = null;
			this.nudCommentWidth.AccessibleName = null;
			resources.ApplyResources(this.nudCommentWidth, "nudCommentWidth");
			this.nudCommentWidth.Font = null;
			this.nudCommentWidth.Maximum = new decimal(new int[4] { 9999, 0, 0, 0 });
			this.nudCommentWidth.Name = "nudCommentWidth";
			this.nudCommentWidth.ValueChanged += new System.EventHandler(this.Resolution_ValueChanged);
			this.lblCommentWidth.AccessibleDescription = null;
			this.lblCommentWidth.AccessibleName = null;
			resources.ApplyResources(this.lblCommentWidth, "lblCommentWidth");
			this.lblCommentWidth.Font = null;
			this.lblCommentWidth.Name = "lblCommentWidth";
			this.nudCommentY.AccessibleDescription = null;
			this.nudCommentY.AccessibleName = null;
			resources.ApplyResources(this.nudCommentY, "nudCommentY");
			this.nudCommentY.Font = null;
			this.nudCommentY.Maximum = new decimal(new int[4] { 9999, 0, 0, 0 });
			this.nudCommentY.Name = "nudCommentY";
			this.nudCommentY.ValueChanged += new System.EventHandler(this.Resolution_ValueChanged);
			this.lblCommentY.AccessibleDescription = null;
			this.lblCommentY.AccessibleName = null;
			resources.ApplyResources(this.lblCommentY, "lblCommentY");
			this.lblCommentY.Font = null;
			this.lblCommentY.Name = "lblCommentY";
			this.nudCommentX.AccessibleDescription = null;
			this.nudCommentX.AccessibleName = null;
			resources.ApplyResources(this.nudCommentX, "nudCommentX");
			this.nudCommentX.Font = null;
			this.nudCommentX.Maximum = new decimal(new int[4] { 9999, 0, 0, 0 });
			this.nudCommentX.Name = "nudCommentX";
			this.nudCommentX.ValueChanged += new System.EventHandler(this.Resolution_ValueChanged);
			this.lblCommentX.AccessibleDescription = null;
			this.lblCommentX.AccessibleName = null;
			resources.ApplyResources(this.lblCommentX, "lblCommentX");
			this.lblCommentX.Font = null;
			this.lblCommentX.Name = "lblCommentX";
			this.btnOk.AccessibleDescription = null;
			this.btnOk.AccessibleName = null;
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.BackgroundImage = null;
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOk.Font = null;
			this.btnOk.Name = "btnOk";
			this.btnOk.UseVisualStyleBackColor = true;
			this.btnCancel.AccessibleDescription = null;
			this.btnCancel.AccessibleName = null;
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.BackgroundImage = null;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Font = null;
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.ptbPreview.AccessibleDescription = null;
			this.ptbPreview.AccessibleName = null;
			resources.ApplyResources(this.ptbPreview, "ptbPreview");
			this.ptbPreview.BackgroundImage = null;
			this.ptbPreview.Font = null;
			this.ptbPreview.ImageLocation = null;
			this.ptbPreview.Name = "ptbPreview";
			this.ptbPreview.TabStop = false;
			base.AcceptButton = this.btnOk;
			base.AccessibleDescription = null;
			base.AccessibleName = null;
			resources.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackgroundImage = null;
			base.CancelButton = this.btnCancel;
			base.Controls.Add(this.ptbPreview);
			base.Controls.Add(this.btnCancel);
			base.Controls.Add(this.btnOk);
			base.Controls.Add(this.gpbComment);
			base.Controls.Add(this.gpbState);
			base.Controls.Add(this.gpbResolution);
			base.Controls.Add(this.gpbScreen);
			this.Font = null;
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.Icon = null;
			base.MaximizeBox = false;
			base.Name = "CustomResolutionDialog";
			this.gpbScreen.ResumeLayout(false);
			this.gpbScreen.PerformLayout();
			((System.ComponentModel.ISupportInitialize)this.nudScreenHeight).EndInit();
			((System.ComponentModel.ISupportInitialize)this.nudScreenWidth).EndInit();
			((System.ComponentModel.ISupportInitialize)this.nudScreenY).EndInit();
			((System.ComponentModel.ISupportInitialize)this.nudScreenX).EndInit();
			this.gpbResolution.ResumeLayout(false);
			this.gpbResolution.PerformLayout();
			((System.ComponentModel.ISupportInitialize)this.nudResolutionHeight).EndInit();
			((System.ComponentModel.ISupportInitialize)this.nudResolutionWidth).EndInit();
			this.gpbState.ResumeLayout(false);
			this.gpbState.PerformLayout();
			((System.ComponentModel.ISupportInitialize)this.nudStateHeight).EndInit();
			((System.ComponentModel.ISupportInitialize)this.nudStateWidth).EndInit();
			((System.ComponentModel.ISupportInitialize)this.nudStateY).EndInit();
			((System.ComponentModel.ISupportInitialize)this.nudStateX).EndInit();
			this.gpbComment.ResumeLayout(false);
			this.gpbComment.PerformLayout();
			((System.ComponentModel.ISupportInitialize)this.nudCommentHeight).EndInit();
			((System.ComponentModel.ISupportInitialize)this.nudCommentWidth).EndInit();
			((System.ComponentModel.ISupportInitialize)this.nudCommentY).EndInit();
			((System.ComponentModel.ISupportInitialize)this.nudCommentX).EndInit();
			((System.ComponentModel.ISupportInitialize)this.ptbPreview).EndInit();
			base.ResumeLayout(false);
		}
	}
}
