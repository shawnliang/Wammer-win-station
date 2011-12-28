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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PreferenceForm));
            this.btnOK = new System.Windows.Forms.Button();
            this.lblUserNameTitle = new System.Windows.Forms.Label();
            this.lblUserName = new System.Windows.Forms.Label();
            this.lblDeviceNameTitle = new System.Windows.Forms.Label();
            this.lblDeviceName = new System.Windows.Forms.Label();
            this.lblDropboxStorageAccountTitle = new System.Windows.Forms.Label();
            this.btnDropboxAction = new System.Windows.Forms.Button();
            this.lblCloudUsageTitle = new System.Windows.Forms.Label();
            this.barCloudUsage = new System.Windows.Forms.ProgressBar();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.labelConnectionStatus = new System.Windows.Forms.Label();
            this.btnTestConnection = new System.Windows.Forms.Button();
            this.checkBox_autoStartWaveface = new System.Windows.Forms.CheckBox();
            this.lblLocalStorageUsage = new System.Windows.Forms.Label();
            this.lblLocalStorageUsageTitle = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label_DaysLeftValue = new System.Windows.Forms.Label();
            this.label_UsedCountValue = new System.Windows.Forms.Label();
            this.label_MonthlyLimitValue = new System.Windows.Forms.Label();
            this.lblCloudStorageUsed = new System.Windows.Forms.Label();
            this.label_UsedCount = new System.Windows.Forms.Label();
            this.label_dropboxAccount = new System.Windows.Forms.Label();
            this.label_DaysLeft = new System.Windows.Forms.Label();
            this.label_MonthlyLimit = new System.Windows.Forms.Label();
            this.label_switchAccount = new System.Windows.Forms.LinkLabel();
            this.label2 = new System.Windows.Forms.Label();
            this.lblDayLeft = new System.Windows.Forms.Label();
            this.lblCloudStorageLimit = new System.Windows.Forms.Label();
            this.btnEditAccount = new System.Windows.Forms.Button();
            this.lblVersionTitle = new System.Windows.Forms.Label();
            this.lblVersion = new System.Windows.Forms.Label();
            this.lblCopyRight = new System.Windows.Forms.Label();
            this.linkLegalNotice = new System.Windows.Forms.LinkLabel();
            this.bgworkerGetAllData = new System.ComponentModel.BackgroundWorker();
            this.cultureManager = new Waveface.Localization.CultureManager(this.components);
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
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
            // btnDropboxAction
            // 
            resources.ApplyResources(this.btnDropboxAction, "btnDropboxAction");
            this.btnDropboxAction.Name = "btnDropboxAction";
            this.btnDropboxAction.UseVisualStyleBackColor = true;
            // 
            // lblCloudUsageTitle
            // 
            resources.ApplyResources(this.lblCloudUsageTitle, "lblCloudUsageTitle");
            this.lblCloudUsageTitle.Name = "lblCloudUsageTitle";
            // 
            // barCloudUsage
            // 
            resources.ApplyResources(this.barCloudUsage, "barCloudUsage");
            this.barCloudUsage.Name = "barCloudUsage";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.labelConnectionStatus);
            this.groupBox1.Controls.Add(this.btnTestConnection);
            this.groupBox1.Controls.Add(this.checkBox_autoStartWaveface);
            this.groupBox1.Controls.Add(this.lblLocalStorageUsage);
            this.groupBox1.Controls.Add(this.lblLocalStorageUsageTitle);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // labelConnectionStatus
            // 
            resources.ApplyResources(this.labelConnectionStatus, "labelConnectionStatus");
            this.labelConnectionStatus.Name = "labelConnectionStatus";
            // 
            // btnTestConnection
            // 
            resources.ApplyResources(this.btnTestConnection, "btnTestConnection");
            this.btnTestConnection.Name = "btnTestConnection";
            this.btnTestConnection.UseVisualStyleBackColor = true;
            this.btnTestConnection.Click += new System.EventHandler(this.btnTestConnection_Click);
            // 
            // checkBox_autoStartWaveface
            // 
            resources.ApplyResources(this.checkBox_autoStartWaveface, "checkBox_autoStartWaveface");
            this.checkBox_autoStartWaveface.Name = "checkBox_autoStartWaveface";
            this.checkBox_autoStartWaveface.UseVisualStyleBackColor = true;
            this.checkBox_autoStartWaveface.Click += new System.EventHandler(this.checkBox_autoStartWaveface_Click);
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
            this.groupBox2.Controls.Add(this.label_DaysLeftValue);
            this.groupBox2.Controls.Add(this.label_UsedCountValue);
            this.groupBox2.Controls.Add(this.label_MonthlyLimitValue);
            this.groupBox2.Controls.Add(this.lblCloudStorageUsed);
            this.groupBox2.Controls.Add(this.label_UsedCount);
            this.groupBox2.Controls.Add(this.label_dropboxAccount);
            this.groupBox2.Controls.Add(this.label_DaysLeft);
            this.groupBox2.Controls.Add(this.btnDropboxAction);
            this.groupBox2.Controls.Add(this.label_MonthlyLimit);
            this.groupBox2.Controls.Add(this.label_switchAccount);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.lblDayLeft);
            this.groupBox2.Controls.Add(this.lblCloudStorageLimit);
            this.groupBox2.Controls.Add(this.btnEditAccount);
            this.groupBox2.Controls.Add(this.lblCloudUsageTitle);
            this.groupBox2.Controls.Add(this.barCloudUsage);
            this.groupBox2.Controls.Add(this.lblUserNameTitle);
            this.groupBox2.Controls.Add(this.lblUserName);
            this.groupBox2.Controls.Add(this.lblDeviceNameTitle);
            this.groupBox2.Controls.Add(this.lblDeviceName);
            this.groupBox2.Controls.Add(this.lblDropboxStorageAccountTitle);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            this.groupBox2.Enter += new System.EventHandler(this.groupBox2_Enter);
            // 
            // label_DaysLeftValue
            // 
            resources.ApplyResources(this.label_DaysLeftValue, "label_DaysLeftValue");
            this.label_DaysLeftValue.Name = "label_DaysLeftValue";
            // 
            // label_UsedCountValue
            // 
            resources.ApplyResources(this.label_UsedCountValue, "label_UsedCountValue");
            this.label_UsedCountValue.Name = "label_UsedCountValue";
            // 
            // label_MonthlyLimitValue
            // 
            resources.ApplyResources(this.label_MonthlyLimitValue, "label_MonthlyLimitValue");
            this.label_MonthlyLimitValue.Name = "label_MonthlyLimitValue";
            // 
            // lblCloudStorageUsed
            // 
            resources.ApplyResources(this.lblCloudStorageUsed, "lblCloudStorageUsed");
            this.lblCloudStorageUsed.Name = "lblCloudStorageUsed";
            // 
            // label_UsedCount
            // 
            resources.ApplyResources(this.label_UsedCount, "label_UsedCount");
            this.label_UsedCount.Name = "label_UsedCount";
            // 
            // label_dropboxAccount
            // 
            resources.ApplyResources(this.label_dropboxAccount, "label_dropboxAccount");
            this.label_dropboxAccount.Name = "label_dropboxAccount";
            // 
            // label_DaysLeft
            // 
            resources.ApplyResources(this.label_DaysLeft, "label_DaysLeft");
            this.label_DaysLeft.Name = "label_DaysLeft";
            // 
            // label_MonthlyLimit
            // 
            resources.ApplyResources(this.label_MonthlyLimit, "label_MonthlyLimit");
            this.label_MonthlyLimit.Name = "label_MonthlyLimit";
            // 
            // label_switchAccount
            // 
            resources.ApplyResources(this.label_switchAccount, "label_switchAccount");
            this.label_switchAccount.Name = "label_switchAccount";
            this.label_switchAccount.TabStop = true;
            this.label_switchAccount.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.label_switchAccount_LinkClicked);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // lblDayLeft
            // 
            resources.ApplyResources(this.lblDayLeft, "lblDayLeft");
            this.lblDayLeft.Name = "lblDayLeft";
            // 
            // lblCloudStorageLimit
            // 
            resources.ApplyResources(this.lblCloudStorageLimit, "lblCloudStorageLimit");
            this.lblCloudStorageLimit.Name = "lblCloudStorageLimit";
            // 
            // btnEditAccount
            // 
            resources.ApplyResources(this.btnEditAccount, "btnEditAccount");
            this.btnEditAccount.Name = "btnEditAccount";
            this.btnEditAccount.UseVisualStyleBackColor = true;
            this.btnEditAccount.Click += new System.EventHandler(this.btnEditAccount_Click);
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
            this.linkLegalNotice.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLegalNotice_LinkClicked);
            // 
            // bgworkerGetAllData
            // 
            this.bgworkerGetAllData.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgworkerGetAllData_DoWork);
            this.bgworkerGetAllData.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgworkerGetAllData_RunWorkerCompleted);
            // 
            // cultureManager
            // 
            this.cultureManager.ManagedControl = this;
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
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PreferenceForm_FormClosing);
            this.Load += new System.EventHandler(this.PreferenceForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
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
        private System.Windows.Forms.Button btnDropboxAction;
        private System.Windows.Forms.Label lblCloudUsageTitle;
        private System.Windows.Forms.ProgressBar barCloudUsage;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblLocalStorageUsageTitle;
        private System.Windows.Forms.Button btnTestConnection;
        private System.Windows.Forms.CheckBox checkBox_autoStartWaveface;
        private System.Windows.Forms.Label lblLocalStorageUsage;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnEditAccount;
        private System.Windows.Forms.Label lblVersionTitle;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Label lblCopyRight;
        private System.Windows.Forms.Label lblDayLeft;
        private System.Windows.Forms.Label lblCloudStorageLimit;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.LinkLabel linkLegalNotice;
        private System.ComponentModel.BackgroundWorker bgworkerGetAllData;
        private System.Windows.Forms.Label label_dropboxAccount;
        private System.Windows.Forms.LinkLabel label_switchAccount;
        private System.Windows.Forms.Label labelConnectionStatus;
        private System.Windows.Forms.Label label_MonthlyLimit;
        private System.Windows.Forms.Label label_DaysLeft;
        private System.Windows.Forms.Label label_UsedCount;
        private System.Windows.Forms.Label lblCloudStorageUsed;
		private Localization.CultureManager cultureManager;
        private System.Windows.Forms.Label label_DaysLeftValue;
        private System.Windows.Forms.Label label_UsedCountValue;
        private System.Windows.Forms.Label label_MonthlyLimitValue;
    }
}
