using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using KidFansChannel.GalCore;
using KidFansChannel.GameCore;
using Microsoft.DirectX.DirectInput;
using Microsoft.DirectX.DirectSound;

namespace KidFansChannel.GameSetting
{
	public class Setting : Form
	{
		protected const int PATH_MAX = 260;

		protected const int CSIDL_PROFILE = 40;

		private IContainer components;

		private TabControl tcClass;

		private TabPage tpGraphics;

		private Label lblGraphicsDevice;

		private CheckBox ckbFullScreen;

		private ComboBox cbbResolution;

		private ComboBox cbbGraphicsDevice;

		private Label lblResolution;

		private TabPage tpSound;

		private TabPage pbInput;

		private TabPage ptMisc;

		private System.Windows.Forms.Button btnCancel;

		private System.Windows.Forms.Button btnOk;

		private System.Windows.Forms.Button btnCustomResoultion;

		private Label lblSoundDevice;

		private ComboBox cbbSoundDevice;

		private Label lblInputDevice;

		private ComboBox cbbInputDevice;

		private GroupBox gpbShock;

		private CheckBox ckbFontDefault;

		private System.Windows.Forms.Button btnFont;

		private FontDialog fontDialog;

		protected Label lblLanguage;

		protected ComboBox cbbLanguage;

		private Label lblCaution;

		private System.Windows.Forms.Button btnSave;

		private CheckBox ckbXInput;

		private CheckBox ckbGamepad;

		private CheckBox ckbWindow;

		private System.Windows.Forms.Button btnCustomGamePad;

		private CheckBox ckbVSync;

		private string language = Thread.CurrentThread.CurrentUICulture.Name;

		private GameOptions gameOptions = new GameOptions();

		private Resolution _windowResolution;

		private Resolution _fullscreenResolution;

		private GamePadMap _gamePadMap;

		protected virtual string SettingFolder
		{
			get
			{
				return string.Empty;
			}
		}

		protected int GraphicsDevice
		{
			get
			{
				return cbbGraphicsDevice.SelectedIndex;
			}
			set
			{
				if (value < cbbGraphicsDevice.Items.Count)
				{
					cbbGraphicsDevice.SelectedIndex = value;
				}
				else
				{
					cbbGraphicsDevice.SelectedIndex = 0;
				}
			}
		}

		protected Resolution WindowResolution
		{
			get
			{
				return _windowResolution;
			}
			set
			{
				_windowResolution = value;
				UpdateResolutionComboBox();
			}
		}

		protected Resolution FullscreenResolution
		{
			get
			{
				return _fullscreenResolution;
			}
			set
			{
				_fullscreenResolution = value;
				UpdateResolutionComboBox();
			}
		}

		protected bool FullScreen
		{
			get
			{
				return ckbFullScreen.Checked;
			}
			set
			{
				ckbFullScreen.Checked = value;
			}
		}

		protected bool VSync
		{
			get
			{
				return ckbVSync.Checked;
			}
			set
			{
				ckbVSync.Checked = value;
			}
		}

		protected int SoundDevice
		{
			get
			{
				return cbbSoundDevice.SelectedIndex;
			}
			set
			{
				if (value < cbbSoundDevice.Items.Count)
				{
					cbbSoundDevice.SelectedIndex = value;
				}
				else
				{
					cbbSoundDevice.SelectedIndex = 0;
				}
			}
		}

		protected int InputDevice
		{
			get
			{
				if (cbbInputDevice.Items.Count == 0)
				{
					return 0;
				}
				return cbbInputDevice.SelectedIndex;
			}
			set
			{
				if (value < cbbInputDevice.Items.Count)
				{
					cbbInputDevice.SelectedIndex = value;
				}
				else if (cbbInputDevice.Items.Count > 0)
				{
					cbbInputDevice.SelectedIndex = 0;
				}
			}
		}

		protected bool XInput
		{
			get
			{
				return ckbXInput.Checked;
			}
			set
			{
				ckbXInput.Checked = value;
			}
		}

		public GamePadMap GamePadMap
		{
			get
			{
				return _gamePadMap;
			}
			set
			{
				_gamePadMap = value;
			}
		}

		protected GameOptions.ShockMethod Shock
		{
			get
			{
				if (!ckbWindow.Checked && !ckbGamepad.Checked)
				{
					return GameOptions.ShockMethod.None;
				}
				if (!ckbWindow.Checked && ckbGamepad.Checked)
				{
					return GameOptions.ShockMethod.Gamepad;
				}
				if (ckbWindow.Checked && !ckbGamepad.Checked)
				{
					return GameOptions.ShockMethod.Window;
				}
				return GameOptions.ShockMethod.GamepadAndWindow;
			}
			set
			{
				switch (value)
				{
				case GameOptions.ShockMethod.None:
					ckbWindow.Checked = false;
					ckbGamepad.Checked = false;
					break;
				case GameOptions.ShockMethod.Gamepad:
					ckbWindow.Checked = false;
					ckbGamepad.Checked = true;
					break;
				case GameOptions.ShockMethod.Window:
					ckbWindow.Checked = true;
					ckbGamepad.Checked = false;
					break;
				case GameOptions.ShockMethod.GamepadAndWindow:
					ckbWindow.Checked = true;
					ckbGamepad.Checked = true;
					break;
				}
			}
		}

		protected string GameFont
		{
			get
			{
				if (ckbFontDefault.Checked)
				{
					return "@default";
				}
				return fontDialog.Font.FontFamily.Name;
			}
			set
			{
				if (value == "@default")
				{
					ckbFontDefault.Checked = true;
				}
				else
				{
					fontDialog.Font = new Font(value, 11f);
				}
			}
		}

		protected virtual string SaveFolder
		{
			get
			{
				return string.Empty;
			}
		}

		protected string Language
		{
			get
			{
				return (string)cbbLanguage.SelectedItem;
			}
			set
			{
				cbbLanguage.SelectedItem = value;
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(KidFansChannel.GameSetting.Setting));
			this.tcClass = new System.Windows.Forms.TabControl();
			this.tpGraphics = new System.Windows.Forms.TabPage();
			this.lblCaution = new System.Windows.Forms.Label();
			this.btnCustomResoultion = new System.Windows.Forms.Button();
			this.ckbVSync = new System.Windows.Forms.CheckBox();
			this.lblGraphicsDevice = new System.Windows.Forms.Label();
			this.ckbFullScreen = new System.Windows.Forms.CheckBox();
			this.cbbResolution = new System.Windows.Forms.ComboBox();
			this.cbbGraphicsDevice = new System.Windows.Forms.ComboBox();
			this.lblResolution = new System.Windows.Forms.Label();
			this.tpSound = new System.Windows.Forms.TabPage();
			this.lblSoundDevice = new System.Windows.Forms.Label();
			this.cbbSoundDevice = new System.Windows.Forms.ComboBox();
			this.pbInput = new System.Windows.Forms.TabPage();
			this.btnCustomGamePad = new System.Windows.Forms.Button();
			this.ckbXInput = new System.Windows.Forms.CheckBox();
			this.gpbShock = new System.Windows.Forms.GroupBox();
			this.ckbGamepad = new System.Windows.Forms.CheckBox();
			this.ckbWindow = new System.Windows.Forms.CheckBox();
			this.lblInputDevice = new System.Windows.Forms.Label();
			this.cbbInputDevice = new System.Windows.Forms.ComboBox();
			this.ptMisc = new System.Windows.Forms.TabPage();
			this.btnSave = new System.Windows.Forms.Button();
			this.lblLanguage = new System.Windows.Forms.Label();
			this.cbbLanguage = new System.Windows.Forms.ComboBox();
			this.btnFont = new System.Windows.Forms.Button();
			this.ckbFontDefault = new System.Windows.Forms.CheckBox();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOk = new System.Windows.Forms.Button();
			this.fontDialog = new System.Windows.Forms.FontDialog();
			this.tcClass.SuspendLayout();
			this.tpGraphics.SuspendLayout();
			this.tpSound.SuspendLayout();
			this.pbInput.SuspendLayout();
			this.gpbShock.SuspendLayout();
			this.ptMisc.SuspendLayout();
			base.SuspendLayout();
			this.tcClass.Controls.Add(this.tpGraphics);
			this.tcClass.Controls.Add(this.tpSound);
			this.tcClass.Controls.Add(this.pbInput);
			this.tcClass.Controls.Add(this.ptMisc);
			resources.ApplyResources(this.tcClass, "tcClass");
			this.tcClass.Name = "tcClass";
			this.tcClass.SelectedIndex = 0;
			this.tpGraphics.Controls.Add(this.lblCaution);
			this.tpGraphics.Controls.Add(this.btnCustomResoultion);
			this.tpGraphics.Controls.Add(this.ckbVSync);
			this.tpGraphics.Controls.Add(this.lblGraphicsDevice);
			this.tpGraphics.Controls.Add(this.ckbFullScreen);
			this.tpGraphics.Controls.Add(this.cbbResolution);
			this.tpGraphics.Controls.Add(this.cbbGraphicsDevice);
			this.tpGraphics.Controls.Add(this.lblResolution);
			resources.ApplyResources(this.tpGraphics, "tpGraphics");
			this.tpGraphics.Name = "tpGraphics";
			this.tpGraphics.UseVisualStyleBackColor = true;
			resources.ApplyResources(this.lblCaution, "lblCaution");
			this.lblCaution.ForeColor = System.Drawing.Color.Red;
			this.lblCaution.Name = "lblCaution";
			resources.ApplyResources(this.btnCustomResoultion, "btnCustomResoultion");
			this.btnCustomResoultion.Name = "btnCustomResoultion";
			this.btnCustomResoultion.UseVisualStyleBackColor = true;
			this.btnCustomResoultion.Click += new System.EventHandler(this.btnCustomResoultion_Click);
			resources.ApplyResources(this.ckbVSync, "ckbVSync");
			this.ckbVSync.Name = "ckbVSync";
			this.ckbVSync.UseVisualStyleBackColor = true;
			resources.ApplyResources(this.lblGraphicsDevice, "lblGraphicsDevice");
			this.lblGraphicsDevice.Name = "lblGraphicsDevice";
			resources.ApplyResources(this.ckbFullScreen, "ckbFullScreen");
			this.ckbFullScreen.Name = "ckbFullScreen";
			this.ckbFullScreen.UseVisualStyleBackColor = true;
			this.ckbFullScreen.CheckedChanged += new System.EventHandler(this.ckbFullScreen_CheckedChanged);
			this.cbbResolution.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
			this.cbbResolution.DropDownHeight = 212;
			this.cbbResolution.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.cbbResolution, "cbbResolution");
			this.cbbResolution.Items.AddRange(new object[1] { resources.GetString("cbbResolution.Items") });
			this.cbbResolution.Name = "cbbResolution";
			this.cbbResolution.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.cbbResolution_DrawItem);
			this.cbbResolution.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.cbbResolution_MeasureItem);
			this.cbbResolution.SelectedIndexChanged += new System.EventHandler(this.cbbResolution_SelectedIndexChanged);
			this.cbbGraphicsDevice.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
			this.cbbGraphicsDevice.DropDownHeight = 212;
			this.cbbGraphicsDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.cbbGraphicsDevice, "cbbGraphicsDevice");
			this.cbbGraphicsDevice.Name = "cbbGraphicsDevice";
			this.cbbGraphicsDevice.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.cbbGraphicsDevice_DrawItem);
			this.cbbGraphicsDevice.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.cbbGraphicsDevice_MeasureItem);
			resources.ApplyResources(this.lblResolution, "lblResolution");
			this.lblResolution.Name = "lblResolution";
			this.tpSound.Controls.Add(this.lblSoundDevice);
			this.tpSound.Controls.Add(this.cbbSoundDevice);
			resources.ApplyResources(this.tpSound, "tpSound");
			this.tpSound.Name = "tpSound";
			this.tpSound.UseVisualStyleBackColor = true;
			resources.ApplyResources(this.lblSoundDevice, "lblSoundDevice");
			this.lblSoundDevice.Name = "lblSoundDevice";
			this.cbbSoundDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.cbbSoundDevice, "cbbSoundDevice");
			this.cbbSoundDevice.Name = "cbbSoundDevice";
			this.pbInput.Controls.Add(this.btnCustomGamePad);
			this.pbInput.Controls.Add(this.ckbXInput);
			this.pbInput.Controls.Add(this.gpbShock);
			this.pbInput.Controls.Add(this.lblInputDevice);
			this.pbInput.Controls.Add(this.cbbInputDevice);
			resources.ApplyResources(this.pbInput, "pbInput");
			this.pbInput.Name = "pbInput";
			this.pbInput.UseVisualStyleBackColor = true;
			resources.ApplyResources(this.btnCustomGamePad, "btnCustomGamePad");
			this.btnCustomGamePad.Name = "btnCustomGamePad";
			this.btnCustomGamePad.UseVisualStyleBackColor = true;
			this.btnCustomGamePad.Click += new System.EventHandler(this.btnCustomGamePad_Click);
			resources.ApplyResources(this.ckbXInput, "ckbXInput");
			this.ckbXInput.Name = "ckbXInput";
			this.ckbXInput.UseVisualStyleBackColor = true;
			this.ckbXInput.CheckedChanged += new System.EventHandler(this.ckbXInput_CheckedChanged);
			this.gpbShock.Controls.Add(this.ckbGamepad);
			this.gpbShock.Controls.Add(this.ckbWindow);
			resources.ApplyResources(this.gpbShock, "gpbShock");
			this.gpbShock.Name = "gpbShock";
			this.gpbShock.TabStop = false;
			resources.ApplyResources(this.ckbGamepad, "ckbGamepad");
			this.ckbGamepad.Name = "ckbGamepad";
			this.ckbGamepad.UseVisualStyleBackColor = true;
			resources.ApplyResources(this.ckbWindow, "ckbWindow");
			this.ckbWindow.Name = "ckbWindow";
			this.ckbWindow.UseVisualStyleBackColor = true;
			resources.ApplyResources(this.lblInputDevice, "lblInputDevice");
			this.lblInputDevice.Name = "lblInputDevice";
			this.cbbInputDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.cbbInputDevice, "cbbInputDevice");
			this.cbbInputDevice.Name = "cbbInputDevice";
			this.ptMisc.Controls.Add(this.btnSave);
			this.ptMisc.Controls.Add(this.lblLanguage);
			this.ptMisc.Controls.Add(this.cbbLanguage);
			this.ptMisc.Controls.Add(this.btnFont);
			this.ptMisc.Controls.Add(this.ckbFontDefault);
			resources.ApplyResources(this.ptMisc, "ptMisc");
			this.ptMisc.Name = "ptMisc";
			this.ptMisc.UseVisualStyleBackColor = true;
			resources.ApplyResources(this.btnSave, "btnSave");
			this.btnSave.Name = "btnSave";
			this.btnSave.UseVisualStyleBackColor = true;
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			resources.ApplyResources(this.lblLanguage, "lblLanguage");
			this.lblLanguage.Name = "lblLanguage";
			this.cbbLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			resources.ApplyResources(this.cbbLanguage, "cbbLanguage");
			this.cbbLanguage.Name = "cbbLanguage";
			resources.ApplyResources(this.btnFont, "btnFont");
			this.btnFont.Name = "btnFont";
			this.btnFont.UseVisualStyleBackColor = true;
			this.btnFont.Click += new System.EventHandler(this.btnFont_Click);
			resources.ApplyResources(this.ckbFontDefault, "ckbFontDefault");
			this.ckbFontDefault.Name = "ckbFontDefault";
			this.ckbFontDefault.UseVisualStyleBackColor = true;
			this.ckbFontDefault.CheckedChanged += new System.EventHandler(this.ckbFontDefault_CheckedChanged);
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.Name = "btnOk";
			this.btnOk.UseVisualStyleBackColor = true;
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			base.AcceptButton = this.btnOk;
			resources.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.CancelButton = this.btnCancel;
			base.Controls.Add(this.btnCancel);
			base.Controls.Add(this.btnOk);
			base.Controls.Add(this.tcClass);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			base.MaximizeBox = false;
			base.Name = "Setting";
			this.tcClass.ResumeLayout(false);
			this.tpGraphics.ResumeLayout(false);
			this.tpGraphics.PerformLayout();
			this.tpSound.ResumeLayout(false);
			this.tpSound.PerformLayout();
			this.pbInput.ResumeLayout(false);
			this.pbInput.PerformLayout();
			this.gpbShock.ResumeLayout(false);
			this.gpbShock.PerformLayout();
			this.ptMisc.ResumeLayout(false);
			this.ptMisc.PerformLayout();
			base.ResumeLayout(false);
		}

		public Setting()
		{
			Environment.CurrentDirectory = Path.GetDirectoryName(Application.ExecutablePath);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load("language.xml");
			XmlNode documentElement = xmlDocument.DocumentElement;
			if (documentElement.Name != "language")
			{
				throw new InvalidDataException();
			}
			IEnumerator enumerator = documentElement.ChildNodes.GetEnumerator();
			try
			{
				if (enumerator.MoveNext())
				{
					XmlNode xmlNode = (XmlNode)enumerator.Current;
					if (xmlNode.Name != "item" || !xmlNode.HasChildNodes || xmlNode.FirstChild.Name != "name" || !xmlNode.FirstChild.HasChildNodes || xmlNode.FirstChild.ChildNodes.Count != 1 || xmlNode.FirstChild.FirstChild.NodeType != XmlNodeType.Text)
					{
						throw new InvalidDataException();
					}
					language = xmlNode.FirstChild.FirstChild.Value;
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			try
			{
				Thread.CurrentThread.CurrentUICulture = new CultureInfo(language);
			}
			catch (Exception)
			{
			}
			InitializeComponent();
			gameOptions.SettingFolder = SettingFolder;
			gameOptions.Load();
			InitGraphics();
			InitSound();
			InitInput();
			InitMisc();
		}

		protected void InitGraphics()
		{
			AddGraphicsDevice();
			AddResolutions();
			GraphicsDevice = gameOptions.GraphicsDevice;
			WindowResolution = gameOptions.WindowResolution;
			FullscreenResolution = gameOptions.FullscreenResolution;
			FullScreen = gameOptions.FullScreen;
			VSync = gameOptions.VSync;
		}

		protected void AddGraphicsDevice()
		{
			cbbGraphicsDevice.Items.Clear();
			Enumeration.Enumerate();
			List<EnumAdapterInformation> adapterInformationList = Enumeration.AdapterInformationList;
			foreach (EnumAdapterInformation item in adapterInformationList)
			{
				cbbGraphicsDevice.Items.Add(item.UniqueDescription);
			}
			if (cbbGraphicsDevice.Items.Count == 0)
			{
				MessageBox.Show("No Graphics Device can be found!");
				Application.Exit();
			}
			cbbGraphicsDevice.SelectedIndex = 0;
		}

		protected virtual void AddResolutions()
		{
			cbbResolution.Items.Clear();
			cbbResolution.Items.Add(new Resolution(new Size(800, 600), new Size(4, 3), new Rectangle(0, 0, 800, 600), 1f, Rectangle.Empty, new Rectangle(0, 0, 800, 40)));
			cbbResolution.Items.Add(new Resolution(new Size(960, 720), new Size(4, 3), new Rectangle(0, 0, 960, 720), 1.2f, Rectangle.Empty, new Rectangle(0, 0, 960, 48)));
			cbbResolution.Items.Add(new Resolution(new Size(1024, 768), new Size(4, 3), new Rectangle(0, 0, 1024, 768), 1.28f, Rectangle.Empty, new Rectangle(0, 0, 1024, 52)));
			cbbResolution.Items.Add(new Resolution(new Size(1152, 864), new Size(4, 3), new Rectangle(0, 0, 1152, 864), 1.44f, Rectangle.Empty, new Rectangle(0, 0, 1152, 58)));
			cbbResolution.Items.Add(new Resolution(new Size(1280, 960), new Size(4, 3), new Rectangle(0, 0, 1280, 960), 1.6f, Rectangle.Empty, new Rectangle(0, 0, 1280, 64)));
			cbbResolution.Items.Add(new Resolution(new Size(960, 600), new Size(16, 10), new Rectangle(80, 0, 800, 600), 1f, Rectangle.Empty, new Rectangle(80, 0, 800, 40)));
			cbbResolution.Items.Add(new Resolution(new Size(1280, 800), new Size(16, 10), new Rectangle(108, 1, 1064, 798), 1.33f, Rectangle.Empty, new Rectangle(108, 0, 1064, 54)));
			cbbResolution.Items.Add(new Resolution(new Size(1440, 900), new Size(16, 10), new Rectangle(120, 0, 1200, 900), 1.5f, Rectangle.Empty, new Rectangle(120, 0, 1200, 60)));
			cbbResolution.Items.Add(new Resolution(new Size(1088, 612), new Size(16, 9), new Rectangle(136, 0, 816, 612), 1.02f, Rectangle.Empty, new Rectangle(136, 0, 816, 40)));
			cbbResolution.Items.Add(new Resolution(new Size(1176, 664), new Size(16, 9), new Rectangle(146, 0, 884, 663), 1.105f, Rectangle.Empty, new Rectangle(146, 0, 884, 44)));
			cbbResolution.Items.Add(new Resolution(new Size(1280, 720), new Size(16, 9), new Rectangle(160, 0, 960, 720), 1.2f, Rectangle.Empty, new Rectangle(160, 0, 960, 48)));
			cbbResolution.Items.Add(new Resolution(new Size(1360, 768), new Size(16, 9), new Rectangle(168, 0, 1024, 768), 1.28f, Rectangle.Empty, new Rectangle(168, 0, 1024, 52)));
			cbbResolution.Items.Add(new Resolution(new Size(1280, 1024), new Size(5, 4), new Rectangle(0, 32, 1280, 960), 1.6f, Rectangle.Empty, new Rectangle(0, 32, 1280, 64)));
			cbbResolution.Items.Add(new Resolution(new Size(1280, 768), new Size(5, 3), new Rectangle(128, 0, 1024, 768), 1.28f, Rectangle.Empty, new Rectangle(128, 0, 1024, 52)));
			cbbResolution.Items.Add(new Resolution(new Size(960, 600), new Size(16, 10), new Rectangle(0, 0, 800, 600), 1f, new Rectangle(800, 0, 160, 400), new Rectangle(800, 400, 160, 200)));
			cbbResolution.Items.Add(new Resolution(new Size(1280, 800), new Size(16, 10), new Rectangle(0, 1, 1064, 798), 1.33f, new Rectangle(1064, 1, 216, 532), new Rectangle(1064, 533, 216, 266)));
			cbbResolution.Items.Add(new Resolution(new Size(1440, 900), new Size(16, 10), new Rectangle(0, 0, 1200, 900), 1.5f, new Rectangle(1200, 0, 240, 600), new Rectangle(1200, 600, 240, 300)));
			cbbResolution.Items.Add(new Resolution(new Size(1088, 612), new Size(16, 9), new Rectangle(0, 0, 816, 612), 1.02f, new Rectangle(816, 0, 272, 408), new Rectangle(816, 408, 272, 204)));
			cbbResolution.Items.Add(new Resolution(new Size(1176, 664), new Size(16, 9), new Rectangle(0, 0, 884, 663), 1.105f, new Rectangle(884, 0, 292, 442), new Rectangle(884, 442, 292, 221)));
			cbbResolution.Items.Add(new Resolution(new Size(1280, 720), new Size(16, 9), new Rectangle(0, 0, 960, 720), 1.2f, new Rectangle(960, 0, 320, 480), new Rectangle(960, 480, 320, 240)));
			cbbResolution.Items.Add(new Resolution(new Size(1360, 768), new Size(16, 9), new Rectangle(0, 0, 1024, 768), 1.28f, new Rectangle(1024, 0, 336, 512), new Rectangle(1024, 512, 336, 256)));
			cbbResolution.Items.Add(new Resolution(new Size(1280, 1024), new Size(5, 4), new Rectangle(0, 0, 1280, 960), 1.6f, Rectangle.Empty, new Rectangle(0, 960, 1280, 64)));
			cbbResolution.Items.Add(new Resolution(new Size(1280, 768), new Size(5, 3), new Rectangle(0, 0, 1024, 768), 1.28f, new Rectangle(1024, 0, 256, 512), new Rectangle(1024, 512, 256, 256)));
			cbbResolution.SelectedIndex = 0;
		}

		private void cbbGraphicsDevice_MeasureItem(object sender, MeasureItemEventArgs e)
		{
			e.ItemHeight <<= 1;
		}

		private void cbbGraphicsDevice_DrawItem(object sender, DrawItemEventArgs e)
		{
			e.DrawBackground();
			if (e.Index >= 0)
			{
				e.Graphics.DrawString((string)((ComboBox)sender).Items[e.Index], e.Font, new SolidBrush(e.ForeColor), e.Bounds);
			}
		}

		private void cbbResolution_MeasureItem(object sender, MeasureItemEventArgs e)
		{
			e.ItemHeight <<= 1;
		}

		private void cbbResolution_DrawItem(object sender, DrawItemEventArgs e)
		{
			e.DrawBackground();
			if (e.Index < 0)
			{
				return;
			}
			Resolution resolution = (Resolution)((ComboBox)sender).Items[e.Index];
			if (resolution == Resolution.Empty)
			{
				e.Graphics.DrawString(new ComponentResourceManager(typeof(Setting)).GetString("cbbResolution.Items"), e.Font, new SolidBrush(e.ForeColor), e.Bounds);
				return;
			}
			string s = string.Format("{0}*{1}\n({2}:{3})", resolution.Size.Width, resolution.Size.Height, resolution.Proportion.Width, resolution.Proportion.Height);
			e.Graphics.DrawString(s, e.Font, new SolidBrush(e.ForeColor), e.Bounds);
			Rectangle rect = default(Rectangle);
			rect.Height = e.Bounds.Height;
			double num = (double)e.Bounds.Height / (double)resolution.Size.Height;
			rect.Width = (int)((double)resolution.Size.Width * num + 0.5);
			rect.X = e.Bounds.X + e.Bounds.Width - rect.Width;
			rect.Y = e.Bounds.Y;
			e.Graphics.FillRectangle(new SolidBrush(Color.Black), rect);
			Rectangle rect2 = default(Rectangle);
			rect2.X = (int)((double)resolution.Bounds.X * num + (double)rect.X + 0.5);
			rect2.Y = (int)((double)resolution.Bounds.Y * num + (double)rect.Y + 0.5);
			rect2.Width = (int)((double)resolution.Bounds.Width * num + 0.5);
			rect2.Height = (int)((double)resolution.Bounds.Height * num + 0.5);
			e.Graphics.FillRectangle(new SolidBrush(Color.Gray), rect2);
			if (resolution.StateArea != Rectangle.Empty)
			{
				Rectangle rectangle = default(Rectangle);
				rectangle.X = (int)((double)resolution.StateArea.X * num + (double)rect.X + 0.5);
				rectangle.Y = (int)((double)resolution.StateArea.Y * num + (double)rect.Y + 0.5);
				rectangle.Width = (int)((double)resolution.StateArea.Width * num + 0.5);
				rectangle.Height = (int)((double)resolution.StateArea.Height * num + 0.5);
				if (rectangle.Width / 2 != 0)
				{
					e.Graphics.FillRectangle(new LinearGradientBrush(new Point(rectangle.X, rectangle.Y), new Point(rectangle.X + rectangle.Width / 2, rectangle.Y), Color.White, Color.Transparent), rectangle.X, rectangle.Y, rectangle.Width / 2, rectangle.Height);
					e.Graphics.FillRectangle(new LinearGradientBrush(new Point(rectangle.X + rectangle.Width, rectangle.Y), new Point(rectangle.X + rectangle.Width / 2 - 1, rectangle.Y), Color.White, Color.Transparent), rectangle.X + rectangle.Width / 2, rectangle.Y, rectangle.Width / 2, rectangle.Height);
				}
			}
			if (resolution.CommentArea != Rectangle.Empty)
			{
				Rectangle rect3 = default(Rectangle);
				rect3.X = (int)((double)resolution.CommentArea.X * num + (double)rect.X + 0.5);
				rect3.Y = (int)((double)resolution.CommentArea.Y * num + (double)rect.Y + 0.5);
				rect3.Width = (int)((double)resolution.CommentArea.Width * num + 0.5);
				rect3.Height = (int)((double)resolution.CommentArea.Height * num + 0.5);
				if (rect3.Height != 0)
				{
					e.Graphics.FillRectangle(new LinearGradientBrush(rect3, Color.Transparent, Color.White, LinearGradientMode.Vertical), rect3);
				}
			}
		}

		private void btnCustomResoultion_Click(object sender, EventArgs e)
		{
			CustomResolutionDialog customResolutionDialog = new CustomResolutionDialog();
			customResolutionDialog.Resolution = (ckbFullScreen.Checked ? FullscreenResolution : WindowResolution);
			if (customResolutionDialog.ShowDialog() == DialogResult.OK)
			{
				if (ckbFullScreen.Checked)
				{
					FullscreenResolution = customResolutionDialog.Resolution;
				}
				else
				{
					WindowResolution = customResolutionDialog.Resolution;
				}
			}
		}

		protected void UpdateResolutionComboBox()
		{
			Resolution resolution = (ckbFullScreen.Checked ? _fullscreenResolution : _windowResolution);
			bool flag = false;
			for (int i = 0; i < cbbResolution.Items.Count; i++)
			{
				if ((Resolution)cbbResolution.Items[i] == resolution)
				{
					cbbResolution.SelectedIndex = i;
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				cbbResolution.Items.Add(resolution);
				cbbResolution.SelectedItem = resolution;
			}
			btnCustomResoultion.Enabled = resolution != Resolution.Empty;
		}

		private void cbbResolution_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (ckbFullScreen.Checked)
			{
				_fullscreenResolution = (Resolution)((ComboBox)sender).SelectedItem;
			}
			else
			{
				_windowResolution = (Resolution)((ComboBox)sender).SelectedItem;
			}
			btnCustomResoultion.Enabled = (ckbFullScreen.Checked ? _fullscreenResolution : _windowResolution) != Resolution.Empty;
		}

		private void ckbFullScreen_CheckedChanged(object sender, EventArgs e)
		{
			if (((CheckBox)sender).Checked)
			{
				if (!cbbResolution.Items.Contains(Resolution.Empty))
				{
					cbbResolution.Items.Insert(0, Resolution.Empty);
				}
			}
			else if (cbbResolution.Items.Contains(Resolution.Empty))
			{
				cbbResolution.Items.Remove(Resolution.Empty);
			}
			UpdateResolutionComboBox();
		}

		protected void InitSound()
		{
			AddSoundDevice();
			SoundDevice = gameOptions.SoundDevice;
		}

		protected void AddSoundDevice()
		{
			cbbSoundDevice.Items.Clear();
			DevicesCollection devicesCollection = new DevicesCollection();
			foreach (DeviceInformation item in devicesCollection)
			{
				cbbSoundDevice.Items.Add(item.Description);
			}
			if (cbbSoundDevice.Items.Count == 0)
			{
				MessageBox.Show("No Sound Device can be found!");
				Application.Exit();
			}
			cbbSoundDevice.SelectedIndex = 0;
		}

		protected void InitInput()
		{
			AddInputDevice();
			InputDevice = gameOptions.InputDevice;
			XInput = gameOptions.XInput;
			if (!File.Exists(Path.Combine(Environment.SystemDirectory, "XInput9_1_0.dll")) && XInput)
			{
				XInput = false;
				ckbXInput.Enabled = false;
			}
			Shock = gameOptions.Shock;
			GamePadMap = gameOptions.GamePadMap;
			if (!XInput && cbbInputDevice.Items.Count == 0)
			{
				btnCustomGamePad.Enabled = false;
			}
		}

		protected void AddInputDevice()
		{
			cbbInputDevice.Items.Clear();
			DeviceList devices = Manager.GetDevices(DeviceClass.GameControl, EnumDevicesFlags.AttachedOnly);
			foreach (DeviceInstance item in devices)
			{
				cbbInputDevice.Items.Add(item.InstanceName);
			}
			if (cbbInputDevice.Items.Count > 0)
			{
				cbbInputDevice.SelectedIndex = 0;
			}
		}

		private void ckbXInput_CheckedChanged(object sender, EventArgs e)
		{
			cbbInputDevice.Enabled = !((CheckBox)sender).Checked;
			if (cbbInputDevice.Items.Count == 0)
			{
				btnCustomGamePad.Enabled = ((CheckBox)sender).Checked;
			}
		}

		private void btnCustomGamePad_Click(object sender, EventArgs e)
		{
			CustomGamePadDialog customGamePadDialog = new CustomGamePadDialog();
			customGamePadDialog.Map = GamePadMap;
			customGamePadDialog.UseXInput = ckbXInput.Checked;
			if (!ckbXInput.Checked)
			{
				customGamePadDialog.GamePadIndex = cbbInputDevice.SelectedIndex;
			}
			if (customGamePadDialog.ShowDialog() == DialogResult.OK)
			{
				GamePadMap = customGamePadDialog.Map;
			}
		}

		protected void InitMisc()
		{
			AddLanguage();
			Language = gameOptions.Language;
			GameFont = gameOptions.Font;
		}

		protected void AddLanguage()
		{
			cbbLanguage.Items.Clear();
			AddLanguageList();
			cbbLanguage.SelectedIndex = 0;
		}

		protected virtual void AddLanguageList()
		{
			AddOneLanguage(language);
		}

		protected void AddOneLanguage(string language)
		{
			cbbLanguage.Items.Add(language);
		}

		private void ckbFontDefault_CheckedChanged(object sender, EventArgs e)
		{
			btnFont.Enabled = !((CheckBox)sender).Checked;
		}

		private void btnFont_Click(object sender, EventArgs e)
		{
			fontDialog.ShowDialog();
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			string fileName;
			if (Environment.OSVersion.Version.Major >= 6)
			{
				StringBuilder stringBuilder = new StringBuilder(260);
				SHGetFolderPath(IntPtr.Zero, 40, IntPtr.Zero, 0, stringBuilder);
				fileName = Path.Combine(stringBuilder.ToString() + "\\Saved Games", SaveFolder);
			}
			else
			{
				fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\My Games", SaveFolder);
			}
			Process.Start(fileName);
		}

		[DllImport("shfolder.dll", CharSet = CharSet.Unicode)]
		protected static extern int SHGetFolderPath(IntPtr hwndOwner, int nFolder, IntPtr hToken, int dwFlags, StringBuilder lpszPath);

		private void btnOk_Click(object sender, EventArgs e)
		{
			gameOptions.GraphicsDevice = GraphicsDevice;
			gameOptions.WindowResolution = WindowResolution;
			gameOptions.FullscreenResolution = FullscreenResolution;
			gameOptions.FullScreen = FullScreen;
			gameOptions.VSync = VSync;
			gameOptions.SoundDevice = SoundDevice;
			gameOptions.InputDevice = InputDevice;
			gameOptions.XInput = XInput;
			gameOptions.GamePadMap = GamePadMap;
			gameOptions.Shock = Shock;
			gameOptions.Language = Language;
			gameOptions.Font = GameFont;
			gameOptions.Save();
			Application.Exit();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}
	}
}
