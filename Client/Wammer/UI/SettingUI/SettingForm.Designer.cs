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
			this.btnUpdate = new System.Windows.Forms.Button();
			this.bgworkerUpdate = new System.ComponentModel.BackgroundWorker();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.groupStations = new System.Windows.Forms.GroupBox();
			this.flowPanelComputerName = new System.Windows.Forms.FlowLayoutPanel();
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this.groupBox1.SuspendLayout();
			this.groupStations.SuspendLayout();
			this.flowLayoutPanel1.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
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
			// btnUpdate
			// 
			resources.ApplyResources(this.btnUpdate, "btnUpdate");
			this.btnUpdate.Name = "btnUpdate";
			this.btnUpdate.UseVisualStyleBackColor = true;
			this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
			// 
			// bgworkerUpdate
			// 
			this.bgworkerUpdate.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgworkerUpdate_DoWork);
			this.bgworkerUpdate.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgworkerUpdate_RunWorkerCompleted);
			// 
			// groupBox1
			// 
			resources.ApplyResources(this.groupBox1, "groupBox1");
			this.groupBox1.Controls.Add(this.btnUpdate);
			this.groupBox1.Controls.Add(this.lblVersion);
			this.groupBox1.Controls.Add(this.lblVersionTitle);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.TabStop = false;
			// 
			// groupStations
			// 
			resources.ApplyResources(this.groupStations, "groupStations");
			this.groupStations.Controls.Add(this.flowPanelComputerName);
			this.groupStations.Name = "groupStations";
			this.groupStations.TabStop = false;
			// 
			// flowPanelComputerName
			// 
			resources.ApplyResources(this.flowPanelComputerName, "flowPanelComputerName");
			this.flowPanelComputerName.Name = "flowPanelComputerName";
			// 
			// flowLayoutPanel1
			// 
			resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
			this.flowLayoutPanel1.Controls.Add(this.groupStations);
			this.flowLayoutPanel1.Controls.Add(this.groupBox1);
			this.flowLayoutPanel1.Controls.Add(this.lblCopyRight);
			this.flowLayoutPanel1.Controls.Add(this.tableLayoutPanel2);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			// 
			// tableLayoutPanel2
			// 
			resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
			this.tableLayoutPanel2.Controls.Add(this.linkLegalNotice, 0, 0);
			this.tableLayoutPanel2.Controls.Add(this.btnOK, 1, 0);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			// 
			// SettingForm
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.flowLayoutPanel1);
			this.Controls.Add(this.label_DaysLeftValue);
			this.Controls.Add(this.label_UsedCountValue);
			this.Controls.Add(this.label_MonthlyLimitValue);
			this.Controls.Add(this.lblCloudStorageUsed);
			this.Controls.Add(this.label_UsedCount);
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
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupStations.ResumeLayout(false);
			this.groupStations.PerformLayout();
			this.flowLayoutPanel1.ResumeLayout(false);
			this.flowLayoutPanel1.PerformLayout();
			this.tableLayoutPanel2.ResumeLayout(false);
			this.tableLayoutPanel2.PerformLayout();
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
		private System.Windows.Forms.Button btnUpdate;
		private System.ComponentModel.BackgroundWorker bgworkerUpdate;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupStations;
		private System.Windows.Forms.FlowLayoutPanel flowPanelComputerName;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    }
}
