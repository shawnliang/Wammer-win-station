namespace Waveface.SettingUI
{
    partial class PreferenceForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PreferenceForm));
			this.btnOK = new System.Windows.Forms.Button();
			this.lblUserNameTitle = new System.Windows.Forms.Label();
			this.lblUserName = new System.Windows.Forms.Label();
			this.lblDeviceNameTitle = new System.Windows.Forms.Label();
			this.lblDeviceName = new System.Windows.Forms.Label();
			this.lblDropboxStorageAccountTitle = new System.Windows.Forms.Label();
			this.btnUnlinkDropbox = new System.Windows.Forms.Button();
			this.lblCloudUsageTitle = new System.Windows.Forms.Label();
			this.lblLimit = new System.Windows.Forms.Label();
			this.barCloudUsage = new System.Windows.Forms.ProgressBar();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.button2 = new System.Windows.Forms.Button();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.lblLocalStorageUsage = new System.Windows.Forms.Label();
			this.lblLocalStorageUsageTitle = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.panel_DropboxNotInUse = new System.Windows.Forms.Panel();
			this.label_notConnected = new System.Windows.Forms.Label();
			this.btnConnectDropbox = new System.Windows.Forms.Button();
			this.label_switchAccount = new System.Windows.Forms.LinkLabel();
			this.panel_DropboxInUse = new System.Windows.Forms.Panel();
			this.label_dropboxAccount = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.lblStartTime = new System.Windows.Forms.Label();
			this.lblCloudStorageLimit = new System.Windows.Forms.Label();
			this.lblLimit2 = new System.Windows.Forms.Label();
			this.lblLimit1 = new System.Windows.Forms.Label();
			this.btnEditAccount = new System.Windows.Forms.Button();
			this.lblVersionTitle = new System.Windows.Forms.Label();
			this.lblVersion = new System.Windows.Forms.Label();
			this.lblCopyRight = new System.Windows.Forms.Label();
			this.linkLegalNotice = new System.Windows.Forms.LinkLabel();
			this.bgworkerGetAllData = new System.ComponentModel.BackgroundWorker();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.panel_DropboxNotInUse.SuspendLayout();
			this.panel_DropboxInUse.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnOK
			// 
			resources.ApplyResources(this.btnOK, "btnOK");
			this.btnOK.Name = "btnOK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// lblUserNameTitle
			// 
			resources.ApplyResources(this.lblUserNameTitle, "lblUserNameTitle");
			this.lblUserNameTitle.Name = "lblUserNameTitle";
			// 
			// lblUserName
			// 
			resources.ApplyResources(this.lblUserName, "lblUserName");
			this.lblUserName.Name = "lblUserName";
			// 
			// lblDeviceNameTitle
			// 
			resources.ApplyResources(this.lblDeviceNameTitle, "lblDeviceNameTitle");
			this.lblDeviceNameTitle.Name = "lblDeviceNameTitle";
			// 
			// lblDeviceName
			// 
			resources.ApplyResources(this.lblDeviceName, "lblDeviceName");
			this.lblDeviceName.Name = "lblDeviceName";
			// 
			// lblDropboxStorageAccountTitle
			// 
			resources.ApplyResources(this.lblDropboxStorageAccountTitle, "lblDropboxStorageAccountTitle");
			this.lblDropboxStorageAccountTitle.Name = "lblDropboxStorageAccountTitle";
			// 
			// btnUnlinkDropbox
			// 
			resources.ApplyResources(this.btnUnlinkDropbox, "btnUnlinkDropbox");
			this.btnUnlinkDropbox.Name = "btnUnlinkDropbox";
			this.btnUnlinkDropbox.UseVisualStyleBackColor = true;
			this.btnUnlinkDropbox.Click += new System.EventHandler(this.btnUnlinkDropbox_Click);
			// 
			// lblCloudUsageTitle
			// 
			resources.ApplyResources(this.lblCloudUsageTitle, "lblCloudUsageTitle");
			this.lblCloudUsageTitle.Name = "lblCloudUsageTitle";
			// 
			// lblLimit
			// 
			resources.ApplyResources(this.lblLimit, "lblLimit");
			this.lblLimit.Name = "lblLimit";
			// 
			// barCloudUsage
			// 
			resources.ApplyResources(this.barCloudUsage, "barCloudUsage");
			this.barCloudUsage.Name = "barCloudUsage";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.button2);
			this.groupBox1.Controls.Add(this.checkBox1);
			this.groupBox1.Controls.Add(this.lblLocalStorageUsage);
			this.groupBox1.Controls.Add(this.lblLocalStorageUsageTitle);
			resources.ApplyResources(this.groupBox1, "groupBox1");
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.TabStop = false;
			// 
			// button2
			// 
			resources.ApplyResources(this.button2, "button2");
			this.button2.Name = "button2";
			this.button2.UseVisualStyleBackColor = true;
			// 
			// checkBox1
			// 
			resources.ApplyResources(this.checkBox1, "checkBox1");
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.UseVisualStyleBackColor = true;
			// 
			// lblLocalStorageUsage
			// 
			resources.ApplyResources(this.lblLocalStorageUsage, "lblLocalStorageUsage");
			this.lblLocalStorageUsage.Name = "lblLocalStorageUsage";
			// 
			// lblLocalStorageUsageTitle
			// 
			resources.ApplyResources(this.lblLocalStorageUsageTitle, "lblLocalStorageUsageTitle");
			this.lblLocalStorageUsageTitle.Name = "lblLocalStorageUsageTitle";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.panel_DropboxNotInUse);
			this.groupBox2.Controls.Add(this.label_switchAccount);
			this.groupBox2.Controls.Add(this.panel_DropboxInUse);
			this.groupBox2.Controls.Add(this.label2);
			this.groupBox2.Controls.Add(this.lblStartTime);
			this.groupBox2.Controls.Add(this.lblCloudStorageLimit);
			this.groupBox2.Controls.Add(this.lblLimit2);
			this.groupBox2.Controls.Add(this.lblLimit1);
			this.groupBox2.Controls.Add(this.btnEditAccount);
			this.groupBox2.Controls.Add(this.lblCloudUsageTitle);
			this.groupBox2.Controls.Add(this.barCloudUsage);
			this.groupBox2.Controls.Add(this.lblLimit);
			this.groupBox2.Controls.Add(this.lblUserNameTitle);
			this.groupBox2.Controls.Add(this.lblUserName);
			this.groupBox2.Controls.Add(this.lblDeviceNameTitle);
			this.groupBox2.Controls.Add(this.lblDeviceName);
			this.groupBox2.Controls.Add(this.lblDropboxStorageAccountTitle);
			resources.ApplyResources(this.groupBox2, "groupBox2");
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.TabStop = false;
			// 
			// panel_DropboxNotInUse
			// 
			this.panel_DropboxNotInUse.Controls.Add(this.label_notConnected);
			this.panel_DropboxNotInUse.Controls.Add(this.btnConnectDropbox);
			resources.ApplyResources(this.panel_DropboxNotInUse, "panel_DropboxNotInUse");
			this.panel_DropboxNotInUse.Name = "panel_DropboxNotInUse";
			// 
			// label_notConnected
			// 
			resources.ApplyResources(this.label_notConnected, "label_notConnected");
			this.label_notConnected.Name = "label_notConnected";
			// 
			// btnConnectDropbox
			// 
			resources.ApplyResources(this.btnConnectDropbox, "btnConnectDropbox");
			this.btnConnectDropbox.Name = "btnConnectDropbox";
			this.btnConnectDropbox.UseVisualStyleBackColor = true;
			this.btnConnectDropbox.Click += new System.EventHandler(this.btnConnectDropbox_Click);
			// 
			// label_switchAccount
			// 
			resources.ApplyResources(this.label_switchAccount, "label_switchAccount");
			this.label_switchAccount.Name = "label_switchAccount";
			this.label_switchAccount.TabStop = true;
			this.label_switchAccount.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.label_switchAccount_LinkClicked);
			// 
			// panel_DropboxInUse
			// 
			this.panel_DropboxInUse.Controls.Add(this.label_dropboxAccount);
			this.panel_DropboxInUse.Controls.Add(this.btnUnlinkDropbox);
			resources.ApplyResources(this.panel_DropboxInUse, "panel_DropboxInUse");
			this.panel_DropboxInUse.Name = "panel_DropboxInUse";
			// 
			// label_dropboxAccount
			// 
			resources.ApplyResources(this.label_dropboxAccount, "label_dropboxAccount");
			this.label_dropboxAccount.Name = "label_dropboxAccount";
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			// 
			// lblStartTime
			// 
			resources.ApplyResources(this.lblStartTime, "lblStartTime");
			this.lblStartTime.Name = "lblStartTime";
			// 
			// lblCloudStorageLimit
			// 
			resources.ApplyResources(this.lblCloudStorageLimit, "lblCloudStorageLimit");
			this.lblCloudStorageLimit.Name = "lblCloudStorageLimit";
			// 
			// lblLimit2
			// 
			resources.ApplyResources(this.lblLimit2, "lblLimit2");
			this.lblLimit2.Name = "lblLimit2";
			// 
			// lblLimit1
			// 
			resources.ApplyResources(this.lblLimit1, "lblLimit1");
			this.lblLimit1.Name = "lblLimit1";
			// 
			// btnEditAccount
			// 
			resources.ApplyResources(this.btnEditAccount, "btnEditAccount");
			this.btnEditAccount.Name = "btnEditAccount";
			this.btnEditAccount.UseVisualStyleBackColor = true;
			// 
			// lblVersionTitle
			// 
			resources.ApplyResources(this.lblVersionTitle, "lblVersionTitle");
			this.lblVersionTitle.Name = "lblVersionTitle";
			// 
			// lblVersion
			// 
			resources.ApplyResources(this.lblVersion, "lblVersion");
			this.lblVersion.Name = "lblVersion";
			// 
			// lblCopyRight
			// 
			resources.ApplyResources(this.lblCopyRight, "lblCopyRight");
			this.lblCopyRight.Name = "lblCopyRight";
			// 
			// linkLegalNotice
			// 
			resources.ApplyResources(this.linkLegalNotice, "linkLegalNotice");
			this.linkLegalNotice.Name = "linkLegalNotice";
			this.linkLegalNotice.TabStop = true;
			// 
			// bgworkerGetAllData
			// 
			this.bgworkerGetAllData.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgworkerGetAllData_DoWork);
			this.bgworkerGetAllData.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgworkerGetAllData_RunWorkerCompleted);
			// 
			// PreferenceForm
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.linkLegalNotice);
			this.Controls.Add(this.lblCopyRight);
			this.Controls.Add(this.lblVersion);
			this.Controls.Add(this.lblVersionTitle);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.groupBox2);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PreferenceForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Load += new System.EventHandler(this.PreferenceForm_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.panel_DropboxNotInUse.ResumeLayout(false);
			this.panel_DropboxNotInUse.PerformLayout();
			this.panel_DropboxInUse.ResumeLayout(false);
			this.panel_DropboxInUse.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Label lblUserNameTitle;
		private System.Windows.Forms.Label lblUserName;
		private System.Windows.Forms.Label lblDeviceNameTitle;
		private System.Windows.Forms.Label lblDeviceName;
		private System.Windows.Forms.Label lblDropboxStorageAccountTitle;
		private System.Windows.Forms.Button btnUnlinkDropbox;
		private System.Windows.Forms.Label lblCloudUsageTitle;
		private System.Windows.Forms.Label lblLimit;
		private System.Windows.Forms.ProgressBar barCloudUsage;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label lblLocalStorageUsageTitle;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.CheckBox checkBox1;
		private System.Windows.Forms.Label lblLocalStorageUsage;
        private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Button btnEditAccount;
		private System.Windows.Forms.Label lblVersionTitle;
		private System.Windows.Forms.Label lblVersion;
		private System.Windows.Forms.Label lblCopyRight;
		private System.Windows.Forms.Label lblLimit2;
		private System.Windows.Forms.Label lblLimit1;
		private System.Windows.Forms.Label lblStartTime;
        private System.Windows.Forms.Label lblCloudStorageLimit;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.LinkLabel linkLegalNotice;
		private System.ComponentModel.BackgroundWorker bgworkerGetAllData;
        private System.Windows.Forms.Panel panel_DropboxInUse;
        private System.Windows.Forms.Label label_dropboxAccount;
        private System.Windows.Forms.Panel panel_DropboxNotInUse;
        private System.Windows.Forms.Button btnConnectDropbox;
        private System.Windows.Forms.Label label_notConnected;
        private System.Windows.Forms.LinkLabel label_switchAccount;
    }
}
