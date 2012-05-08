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
            this.bgworkerGetAllData = new System.ComponentModel.BackgroundWorker();
            this.cultureManager = new Waveface.Localization.CultureManager(this.components);
            this.bgworkerUpdate = new System.ComponentModel.BackgroundWorker();
            this.lblCopyRight = new System.Windows.Forms.Label();
            this.groupStations = new System.Windows.Forms.GroupBox();
            this.lblPrimaryStation = new System.Windows.Forms.Label();
            this.lblOriginDesc = new System.Windows.Forms.Label();
            this.btnUnlink = new System.Windows.Forms.Button();
            this.lblStorageUsageValue = new System.Windows.Forms.Label();
            this.lblLastSyncValue = new System.Windows.Forms.Label();
            this.lblStorageUsage = new System.Windows.Forms.Label();
            this.lblLastSync = new System.Windows.Forms.Label();
            this.lblLoadingStations = new System.Windows.Forms.Label();
            this.cmbStations = new System.Windows.Forms.ComboBox();
            this.lblVersionTitle = new System.Windows.Forms.Label();
            this.lblVersion = new System.Windows.Forms.Label();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.linkLegalNotice = new System.Windows.Forms.LinkLabel();
            this.btnOK = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupStations.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
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
            // bgworkerUpdate
            // 
            this.bgworkerUpdate.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgworkerUpdate_DoWork);
            this.bgworkerUpdate.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgworkerUpdate_RunWorkerCompleted);
            // 
            // lblCopyRight
            // 
            resources.ApplyResources(this.lblCopyRight, "lblCopyRight");
            this.lblCopyRight.Name = "lblCopyRight";
            // 
            // groupStations
            // 
            resources.ApplyResources(this.groupStations, "groupStations");
            this.groupStations.Controls.Add(this.lblPrimaryStation);
            this.groupStations.Controls.Add(this.lblOriginDesc);
            this.groupStations.Controls.Add(this.btnUnlink);
            this.groupStations.Controls.Add(this.lblStorageUsageValue);
            this.groupStations.Controls.Add(this.lblLastSyncValue);
            this.groupStations.Controls.Add(this.lblStorageUsage);
            this.groupStations.Controls.Add(this.lblLastSync);
            this.groupStations.Controls.Add(this.lblLoadingStations);
            this.groupStations.Controls.Add(this.cmbStations);
            this.groupStations.Name = "groupStations";
            this.groupStations.TabStop = false;
            // 
            // lblPrimaryStation
            // 
            resources.ApplyResources(this.lblPrimaryStation, "lblPrimaryStation");
            this.lblPrimaryStation.Name = "lblPrimaryStation";
            // 
            // lblOriginDesc
            // 
            resources.ApplyResources(this.lblOriginDesc, "lblOriginDesc");
            this.lblOriginDesc.Name = "lblOriginDesc";
            // 
            // btnUnlink
            // 
            resources.ApplyResources(this.btnUnlink, "btnUnlink");
            this.btnUnlink.Name = "btnUnlink";
            this.btnUnlink.UseVisualStyleBackColor = true;
            this.btnUnlink.Click += new System.EventHandler(this.btnUnlink_Click);
            // 
            // lblStorageUsageValue
            // 
            resources.ApplyResources(this.lblStorageUsageValue, "lblStorageUsageValue");
            this.lblStorageUsageValue.Name = "lblStorageUsageValue";
            // 
            // lblLastSyncValue
            // 
            resources.ApplyResources(this.lblLastSyncValue, "lblLastSyncValue");
            this.lblLastSyncValue.Name = "lblLastSyncValue";
            // 
            // lblStorageUsage
            // 
            resources.ApplyResources(this.lblStorageUsage, "lblStorageUsage");
            this.lblStorageUsage.Name = "lblStorageUsage";
            // 
            // lblLastSync
            // 
            resources.ApplyResources(this.lblLastSync, "lblLastSync");
            this.lblLastSync.Name = "lblLastSync";
            // 
            // lblLoadingStations
            // 
            resources.ApplyResources(this.lblLoadingStations, "lblLoadingStations");
            this.lblLoadingStations.Name = "lblLoadingStations";
            // 
            // cmbStations
            // 
            this.cmbStations.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStations.FormattingEnabled = true;
            resources.ApplyResources(this.cmbStations, "cmbStations");
            this.cmbStations.Name = "cmbStations";
            this.cmbStations.SelectedValueChanged += new System.EventHandler(this.cmbStations_SelectedValueChanged);
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
            // btnUpdate
            // 
            resources.ApplyResources(this.btnUpdate, "btnUpdate");
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // linkLegalNotice
            // 
            resources.ApplyResources(this.linkLegalNotice, "linkLegalNotice");
            this.linkLegalNotice.Name = "linkLegalNotice";
            this.linkLegalNotice.TabStop = true;
            this.linkLegalNotice.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLegalNotice_LinkClicked);
            // 
            // btnOK
            // 
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.Name = "btnOK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
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
            // SettingForm
            // 
            this.AcceptButton = this.btnOK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.linkLegalNotice);
            this.Controls.Add(this.groupStations);
            this.Controls.Add(this.lblCopyRight);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingForm";
            this.ShowInTaskbar = false;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SettingForm_FormClosing);
            this.Load += new System.EventHandler(this.PreferenceForm_Load);
            this.groupStations.ResumeLayout(false);
            this.groupStations.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.ComponentModel.BackgroundWorker bgworkerGetAllData;
        private Localization.CultureManager cultureManager;
        private System.ComponentModel.BackgroundWorker bgworkerUpdate;
        private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.LinkLabel linkLegalNotice;
		private System.Windows.Forms.GroupBox groupStations;
		private System.Windows.Forms.Label lblLoadingStations;
		private System.Windows.Forms.Label lblCopyRight;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button btnUpdate;
		private System.Windows.Forms.Label lblVersion;
		private System.Windows.Forms.Label lblVersionTitle;
		private System.Windows.Forms.ComboBox cmbStations;
        private System.Windows.Forms.Button btnUnlink;
		private System.Windows.Forms.Label lblStorageUsageValue;
		private System.Windows.Forms.Label lblLastSyncValue;
		private System.Windows.Forms.Label lblStorageUsage;
		private System.Windows.Forms.Label lblLastSync;
		private System.Windows.Forms.Label lblOriginDesc;
		private System.Windows.Forms.Label lblPrimaryStation;
    }
}
