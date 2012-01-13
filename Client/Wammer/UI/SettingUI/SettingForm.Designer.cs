namespace Waveface.SettingUI
{
    partial class SettingForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingForm));
            this.btnOK = new System.Windows.Forms.Button();
            this.lblUserNameTitle = new System.Windows.Forms.Label();
            this.lblUserName = new System.Windows.Forms.Label();
            this.lblCloudUsageTitle = new System.Windows.Forms.Label();
            this.barCloudUsage = new System.Windows.Forms.ProgressBar();
            this.label_DaysLeftValue = new System.Windows.Forms.Label();
            this.label_UsedCountValue = new System.Windows.Forms.Label();
            this.label_MonthlyLimitValue = new System.Windows.Forms.Label();
            this.lblCloudStorageUsed = new System.Windows.Forms.Label();
            this.label_UsedCount = new System.Windows.Forms.Label();
            this.label_DaysLeft = new System.Windows.Forms.Label();
            this.label_MonthlyLimit = new System.Windows.Forms.Label();
            this.lblDayLeft = new System.Windows.Forms.Label();
            this.lblCloudStorageLimit = new System.Windows.Forms.Label();
            this.btnEditAccount = new System.Windows.Forms.Button();
            this.lblVersionTitle = new System.Windows.Forms.Label();
            this.lblVersion = new System.Windows.Forms.Label();
            this.lblCopyRight = new System.Windows.Forms.Label();
            this.linkLegalNotice = new System.Windows.Forms.LinkLabel();
            this.bgworkerGetAllData = new System.ComponentModel.BackgroundWorker();
            this.cultureManager = new Waveface.Localization.CultureManager(this.components);
            this.panel = new System.Windows.Forms.Panel();
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
            // panel
            // 
            resources.ApplyResources(this.panel, "panel");
            this.panel.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panel.Name = "panel";
            // 
            // SettingForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel);
            this.Controls.Add(this.label_DaysLeftValue);
            this.Controls.Add(this.linkLegalNotice);
            this.Controls.Add(this.label_UsedCountValue);
            this.Controls.Add(this.lblCopyRight);
            this.Controls.Add(this.label_MonthlyLimitValue);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.lblCloudStorageUsed);
            this.Controls.Add(this.lblVersionTitle);
            this.Controls.Add(this.label_UsedCount);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.label_DaysLeft);
            this.Controls.Add(this.label_MonthlyLimit);
            this.Controls.Add(this.lblUserNameTitle);
            this.Controls.Add(this.lblDayLeft);
            this.Controls.Add(this.lblUserName);
            this.Controls.Add(this.lblCloudStorageLimit);
            this.Controls.Add(this.barCloudUsage);
            this.Controls.Add(this.btnEditAccount);
            this.Controls.Add(this.lblCloudUsageTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.PreferenceForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label lblUserNameTitle;
        private System.Windows.Forms.Label lblUserName;
        private System.Windows.Forms.Label lblCloudUsageTitle;
        private System.Windows.Forms.ProgressBar barCloudUsage;
        private System.Windows.Forms.Button btnEditAccount;
        private System.Windows.Forms.Label lblVersionTitle;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Label lblCopyRight;
        private System.Windows.Forms.Label lblDayLeft;
        private System.Windows.Forms.Label lblCloudStorageLimit;
        private System.Windows.Forms.LinkLabel linkLegalNotice;
        private System.ComponentModel.BackgroundWorker bgworkerGetAllData;
        private System.Windows.Forms.Label label_MonthlyLimit;
        private System.Windows.Forms.Label label_DaysLeft;
        private System.Windows.Forms.Label label_UsedCount;
        private System.Windows.Forms.Label lblCloudStorageUsed;
		private Localization.CultureManager cultureManager;
        private System.Windows.Forms.Label label_DaysLeftValue;
        private System.Windows.Forms.Label label_UsedCountValue;
        private System.Windows.Forms.Label label_MonthlyLimitValue;
        private System.Windows.Forms.Panel panel;
    }
}
