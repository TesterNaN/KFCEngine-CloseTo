using System.ComponentModel;
using System.Windows.Forms;
using KidFansChannel.GameSetting;

namespace KidFansChannel.CloseToSetting
{
	public class CloseToSetting : Setting
	{
		private IContainer components;

		private PictureBox ptbTitle;

		protected override string SettingFolder
		{
			get
			{
				return "KID\\CloseTo";
			}
		}

		protected override string SaveFolder
		{
			get
			{
				return "KID\\CloseTo";
			}
		}

		public CloseToSetting()
		{
			InitializeComponent();
			StartPosition = FormStartPosition.CenterScreen;
			lblLanguage.Visible = false;
			cbbLanguage.Visible = false;
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(KidFansChannel.CloseToSetting.CloseToSetting));
			this.ptbTitle = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)this.ptbTitle).BeginInit();
			base.SuspendLayout();
			base.lblLanguage.AccessibleDescription = null;
			base.lblLanguage.AccessibleName = null;
			resources.ApplyResources(base.lblLanguage, "lblLanguage");
			base.lblLanguage.Font = null;
			base.cbbLanguage.AccessibleDescription = null;
			base.cbbLanguage.AccessibleName = null;
			resources.ApplyResources(base.cbbLanguage, "cbbLanguage");
			base.cbbLanguage.BackgroundImage = null;
			base.cbbLanguage.Font = null;
			this.ptbTitle.AccessibleDescription = null;
			this.ptbTitle.AccessibleName = null;
			resources.ApplyResources(this.ptbTitle, "ptbTitle");
			this.ptbTitle.BackgroundImage = null;
			this.ptbTitle.Font = null;
			this.ptbTitle.ImageLocation = null;
			this.ptbTitle.Name = "ptbTitle";
			this.ptbTitle.TabStop = false;
			base.AccessibleDescription = null;
			base.AccessibleName = null;
			resources.ApplyResources(this, "$this");
			this.BackgroundImage = null;
			base.Controls.Add(this.ptbTitle);
			this.Font = null;
			base.Icon = null;
			base.Name = "CloseToSetting";
			base.Controls.SetChildIndex(this.ptbTitle, 0);
			((System.ComponentModel.ISupportInitialize)this.ptbTitle).EndInit();
			base.ResumeLayout(false);
		}
	}
}
