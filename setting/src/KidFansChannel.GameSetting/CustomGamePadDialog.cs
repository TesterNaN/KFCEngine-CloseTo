using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using KidFansChannel.GalCore;
using KidFansChannel.InputCore;

namespace KidFansChannel.GameSetting
{
	public class CustomGamePadDialog : Form
	{
		private IContainer components;

		private Label lblMapOK;

		private Button btnMapOK;

		private Label lblMapCancel;

		private Button btnMapCancel;

		private Button btnMapSkipRead;

		private Label lblMapSkipRead;

		private Button btnMapSkipAll;

		private Label lblMapSkipAll;

		private Button btnMapAuto;

		private Label lblMapAuto;

		private Button btnMapQuick;

		private Label lblMapQuick;

		private Button btnMapPageUp;

		private Label lblMapPageUp;

		private Button btnMapPageDown;

		private Label lblMapPageDown;

		private Button btnCancel;

		private Button btnOk;

		private Button btnMapMessage;

		private Label lblMapMessge;

		private Button btnMapHide;

		private Label lblMapHide;

		private Timer tmrWait;

		private Timer tmrUpdate;

		private GamePadMap _map;

		private XInput xInput;

		private GamePadInput gamePadInput;

		private Button currentMapButton;

		[CompilerGenerated]
		private bool UseXInput__BackingField;

		[CompilerGenerated]
		private int GamePadIndex__BackingField;

		public GamePadMap Map
		{
			get
			{
				return _map;
			}
			set
			{
				_map = value;
			}
		}

		public bool UseXInput
		{
			[CompilerGenerated]
			get
			{
				return UseXInput__BackingField;
			}
			[CompilerGenerated]
			set
			{
				UseXInput__BackingField = value;
			}
		}

		public int GamePadIndex
		{
			[CompilerGenerated]
			get
			{
				return GamePadIndex__BackingField;
			}
			[CompilerGenerated]
			set
			{
				GamePadIndex__BackingField = value;
			}
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(KidFansChannel.GameSetting.CustomGamePadDialog));
			this.lblMapOK = new System.Windows.Forms.Label();
			this.btnMapOK = new System.Windows.Forms.Button();
			this.lblMapCancel = new System.Windows.Forms.Label();
			this.btnMapCancel = new System.Windows.Forms.Button();
			this.btnMapSkipRead = new System.Windows.Forms.Button();
			this.lblMapSkipRead = new System.Windows.Forms.Label();
			this.btnMapSkipAll = new System.Windows.Forms.Button();
			this.lblMapSkipAll = new System.Windows.Forms.Label();
			this.btnMapAuto = new System.Windows.Forms.Button();
			this.lblMapAuto = new System.Windows.Forms.Label();
			this.btnMapQuick = new System.Windows.Forms.Button();
			this.lblMapQuick = new System.Windows.Forms.Label();
			this.btnMapPageUp = new System.Windows.Forms.Button();
			this.lblMapPageUp = new System.Windows.Forms.Label();
			this.btnMapPageDown = new System.Windows.Forms.Button();
			this.lblMapPageDown = new System.Windows.Forms.Label();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOk = new System.Windows.Forms.Button();
			this.btnMapMessage = new System.Windows.Forms.Button();
			this.lblMapMessge = new System.Windows.Forms.Label();
			this.btnMapHide = new System.Windows.Forms.Button();
			this.lblMapHide = new System.Windows.Forms.Label();
			this.tmrWait = new System.Windows.Forms.Timer(this.components);
			this.tmrUpdate = new System.Windows.Forms.Timer(this.components);
			base.SuspendLayout();
			resources.ApplyResources(this.lblMapOK, "lblMapOK");
			this.lblMapOK.Name = "lblMapOK";
			resources.ApplyResources(this.btnMapOK, "btnMapOK");
			this.btnMapOK.Name = "btnMapOK";
			this.btnMapOK.UseVisualStyleBackColor = true;
			this.btnMapOK.Click += new System.EventHandler(this.btnMap_Click);
			resources.ApplyResources(this.lblMapCancel, "lblMapCancel");
			this.lblMapCancel.Name = "lblMapCancel";
			resources.ApplyResources(this.btnMapCancel, "btnMapCancel");
			this.btnMapCancel.Name = "btnMapCancel";
			this.btnMapCancel.UseVisualStyleBackColor = true;
			this.btnMapCancel.Click += new System.EventHandler(this.btnMap_Click);
			resources.ApplyResources(this.btnMapSkipRead, "btnMapSkipRead");
			this.btnMapSkipRead.Name = "btnMapSkipRead";
			this.btnMapSkipRead.UseVisualStyleBackColor = true;
			this.btnMapSkipRead.Click += new System.EventHandler(this.btnMap_Click);
			resources.ApplyResources(this.lblMapSkipRead, "lblMapSkipRead");
			this.lblMapSkipRead.Name = "lblMapSkipRead";
			resources.ApplyResources(this.btnMapSkipAll, "btnMapSkipAll");
			this.btnMapSkipAll.Name = "btnMapSkipAll";
			this.btnMapSkipAll.UseVisualStyleBackColor = true;
			this.btnMapSkipAll.Click += new System.EventHandler(this.btnMap_Click);
			resources.ApplyResources(this.lblMapSkipAll, "lblMapSkipAll");
			this.lblMapSkipAll.Name = "lblMapSkipAll";
			resources.ApplyResources(this.btnMapAuto, "btnMapAuto");
			this.btnMapAuto.Name = "btnMapAuto";
			this.btnMapAuto.UseVisualStyleBackColor = true;
			this.btnMapAuto.Click += new System.EventHandler(this.btnMap_Click);
			resources.ApplyResources(this.lblMapAuto, "lblMapAuto");
			this.lblMapAuto.Name = "lblMapAuto";
			resources.ApplyResources(this.btnMapQuick, "btnMapQuick");
			this.btnMapQuick.Name = "btnMapQuick";
			this.btnMapQuick.UseVisualStyleBackColor = true;
			this.btnMapQuick.Click += new System.EventHandler(this.btnMap_Click);
			resources.ApplyResources(this.lblMapQuick, "lblMapQuick");
			this.lblMapQuick.Name = "lblMapQuick";
			resources.ApplyResources(this.btnMapPageUp, "btnMapPageUp");
			this.btnMapPageUp.Name = "btnMapPageUp";
			this.btnMapPageUp.UseVisualStyleBackColor = true;
			this.btnMapPageUp.Click += new System.EventHandler(this.btnMap_Click);
			resources.ApplyResources(this.lblMapPageUp, "lblMapPageUp");
			this.lblMapPageUp.Name = "lblMapPageUp";
			resources.ApplyResources(this.btnMapPageDown, "btnMapPageDown");
			this.btnMapPageDown.Name = "btnMapPageDown";
			this.btnMapPageDown.UseVisualStyleBackColor = true;
			this.btnMapPageDown.Click += new System.EventHandler(this.btnMap_Click);
			resources.ApplyResources(this.lblMapPageDown, "lblMapPageDown");
			this.lblMapPageDown.Name = "lblMapPageDown";
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.Name = "btnOk";
			this.btnOk.UseVisualStyleBackColor = true;
			resources.ApplyResources(this.btnMapMessage, "btnMapMessage");
			this.btnMapMessage.Name = "btnMapMessage";
			this.btnMapMessage.UseVisualStyleBackColor = true;
			this.btnMapMessage.Click += new System.EventHandler(this.btnMap_Click);
			resources.ApplyResources(this.lblMapMessge, "lblMapMessge");
			this.lblMapMessge.Name = "lblMapMessge";
			resources.ApplyResources(this.btnMapHide, "btnMapHide");
			this.btnMapHide.Name = "btnMapHide";
			this.btnMapHide.UseVisualStyleBackColor = true;
			this.btnMapHide.Click += new System.EventHandler(this.btnMap_Click);
			resources.ApplyResources(this.lblMapHide, "lblMapHide");
			this.lblMapHide.Name = "lblMapHide";
			this.tmrWait.Interval = 3000;
			this.tmrWait.Tick += new System.EventHandler(this.tmrWait_Tick);
			this.tmrUpdate.Tick += new System.EventHandler(this.tmrUpdate_Tick);
			base.AcceptButton = this.btnOk;
			resources.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.CancelButton = this.btnCancel;
			base.Controls.Add(this.btnMapMessage);
			base.Controls.Add(this.lblMapMessge);
			base.Controls.Add(this.btnMapHide);
			base.Controls.Add(this.lblMapHide);
			base.Controls.Add(this.btnCancel);
			base.Controls.Add(this.btnOk);
			base.Controls.Add(this.btnMapPageDown);
			base.Controls.Add(this.lblMapPageDown);
			base.Controls.Add(this.btnMapPageUp);
			base.Controls.Add(this.lblMapPageUp);
			base.Controls.Add(this.btnMapQuick);
			base.Controls.Add(this.lblMapQuick);
			base.Controls.Add(this.btnMapAuto);
			base.Controls.Add(this.lblMapAuto);
			base.Controls.Add(this.btnMapSkipAll);
			base.Controls.Add(this.lblMapSkipAll);
			base.Controls.Add(this.btnMapSkipRead);
			base.Controls.Add(this.lblMapSkipRead);
			base.Controls.Add(this.btnMapCancel);
			base.Controls.Add(this.lblMapCancel);
			base.Controls.Add(this.btnMapOK);
			base.Controls.Add(this.lblMapOK);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.Name = "CustomGamePadDialog";
			base.Load += new System.EventHandler(this.CustomGamePadDialog_Load);
			base.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.CustomGamePadDialog_FormClosed);
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		public CustomGamePadDialog()
		{
			InitializeComponent();
		}

		private void CustomGamePadDialog_Load(object sender, EventArgs e)
		{
			RefreshButton();
			if (UseXInput)
			{
				xInput = new XInput();
				return;
			}
			gamePadInput = new GamePadInput();
			gamePadInput.Initialize(gamePadInput.GetGamePadList()[GamePadIndex], this);
		}

		private void RefreshButton()
		{
			if (_map.OK != -1)
			{
				btnMapOK.Text = "B" + (_map.OK + 1);
			}
			else
			{
				btnMapOK.Text = string.Empty;
			}
			if (_map.Cancel != -1)
			{
				btnMapCancel.Text = "B" + (_map.Cancel + 1);
			}
			else
			{
				btnMapCancel.Text = string.Empty;
			}
			if (_map.SkipRead != -1)
			{
				btnMapSkipRead.Text = "B" + (_map.SkipRead + 1);
			}
			else
			{
				btnMapSkipRead.Text = string.Empty;
			}
			if (_map.SkipAll != -1)
			{
				btnMapSkipAll.Text = "B" + (_map.SkipAll + 1);
			}
			else
			{
				btnMapSkipAll.Text = string.Empty;
			}
			if (_map.Auto != -1)
			{
				btnMapAuto.Text = "B" + (_map.Auto + 1);
			}
			else
			{
				btnMapAuto.Text = string.Empty;
			}
			if (_map.Quick != -1)
			{
				btnMapQuick.Text = "B" + (_map.Quick + 1);
			}
			else
			{
				btnMapQuick.Text = string.Empty;
			}
			if (_map.Hide != -1)
			{
				btnMapHide.Text = "B" + (_map.Hide + 1);
			}
			else
			{
				btnMapHide.Text = string.Empty;
			}
			if (_map.Message != -1)
			{
				btnMapMessage.Text = "B" + (_map.Message + 1);
			}
			else
			{
				btnMapMessage.Text = string.Empty;
			}
			if (_map.PageUp != -1)
			{
				btnMapPageUp.Text = "B" + (_map.PageUp + 1);
			}
			else
			{
				btnMapPageUp.Text = string.Empty;
			}
			if (_map.PageDown != -1)
			{
				btnMapPageDown.Text = "B" + (_map.PageDown + 1);
			}
			else
			{
				btnMapPageDown.Text = string.Empty;
			}
		}

		private void btnMap_Click(object sender, EventArgs e)
		{
			if (currentMapButton == null || currentMapButton == (Button)sender)
			{
				if (currentMapButton == (Button)sender)
				{
					tmrWait.Stop();
					tmrUpdate.Stop();
					RefreshButton();
					SetEnable(true);
					currentMapButton = null;
				}
				else
				{
					currentMapButton = (Button)sender;
					currentMapButton.Text = "???";
					SetEnable(false);
					currentMapButton.Enabled = true;
					currentMapButton.Focus();
					tmrWait.Start();
					tmrUpdate.Start();
				}
			}
		}

		private void tmrWait_Tick(object sender, EventArgs e)
		{
			tmrWait.Stop();
			tmrUpdate.Stop();
			SetMapValue(currentMapButton, -1);
			RefreshButton();
			SetEnable(true);
			currentMapButton = null;
		}

		private void tmrUpdate_Tick(object sender, EventArgs e)
		{
			if (UseXInput)
			{
				xInput.Update();
			}
			else
			{
				gamePadInput.Update();
			}
			for (int i = 0; i < 12; i++)
			{
				if (UseXInput ? xInput.GetButtonState(i) : gamePadInput.GetButtonState(i))
				{
					tmrWait.Stop();
					tmrUpdate.Stop();
					SetMapValue(currentMapButton, i);
					RefreshButton();
					SetEnable(true);
					currentMapButton = null;
					break;
				}
			}
		}

		private void SetEnable(bool value)
		{
			btnMapOK.Enabled = value;
			btnMapCancel.Enabled = value;
			btnMapSkipAll.Enabled = value;
			btnMapSkipRead.Enabled = value;
			btnMapAuto.Enabled = value;
			btnMapQuick.Enabled = value;
			btnMapHide.Enabled = value;
			btnMapMessage.Enabled = value;
			btnMapPageUp.Enabled = value;
			btnMapPageDown.Enabled = value;
		}

		private void SetMapValue(Button button, int value)
		{
			if (button == btnMapOK)
			{
				_map.OK = value;
			}
			else if (button == btnMapCancel)
			{
				_map.Cancel = value;
			}
			else if (button == btnMapSkipRead)
			{
				_map.SkipRead = value;
			}
			else if (button == btnMapSkipAll)
			{
				_map.SkipAll = value;
			}
			else if (button == btnMapAuto)
			{
				_map.Auto = value;
			}
			else if (button == btnMapQuick)
			{
				_map.Quick = value;
			}
			else if (button == btnMapHide)
			{
				_map.Hide = value;
			}
			else if (button == btnMapMessage)
			{
				_map.Message = value;
			}
			else if (button == btnMapPageUp)
			{
				_map.PageUp = value;
			}
			else if (button == btnMapPageDown)
			{
				_map.PageDown = value;
			}
			if (button != btnMapOK && _map.OK == value)
			{
				_map.OK = -1;
			}
			if (button != btnMapCancel && _map.Cancel == value)
			{
				_map.Cancel = -1;
			}
			if (button != btnMapSkipRead && _map.SkipRead == value)
			{
				_map.SkipRead = -1;
			}
			if (button != btnMapSkipAll && _map.SkipAll == value)
			{
				_map.SkipAll = -1;
			}
			if (button != btnMapAuto && _map.Auto == value)
			{
				_map.Auto = -1;
			}
			if (button != btnMapQuick && _map.Quick == value)
			{
				_map.Quick = -1;
			}
			if (button != btnMapHide && _map.Hide == value)
			{
				_map.Hide = -1;
			}
			if (button != btnMapMessage && _map.Message == value)
			{
				_map.Message = -1;
			}
			if (button != btnMapPageUp && _map.PageUp == value)
			{
				_map.PageUp = -1;
			}
			if (button != btnMapPageDown && _map.PageDown == value)
			{
				_map.PageDown = -1;
			}
		}

		private void CustomGamePadDialog_FormClosed(object sender, FormClosedEventArgs e)
		{
			if (!UseXInput)
			{
				gamePadInput.Dispose();
			}
		}
	}
}
