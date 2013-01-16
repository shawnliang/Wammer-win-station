namespace Waveface.Stream.WindowsClient
{
	partial class ControlPanelDialog
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
			this.linkLabel1 = new System.Windows.Forms.LinkLabel();
			this.btnOK = new System.Windows.Forms.Button();
			this.refreshStatusTimer = new System.Windows.Forms.Timer(this.components);
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.panelEx1 = new Waveface.Stream.WindowsClient.PanelEx();
			this.lblSyncTransferStatus = new System.Windows.Forms.Label();
			this.lblSyncStatus = new System.Windows.Forms.Label();
			this.lblLocalProcessStatus = new System.Windows.Forms.Label();
			this.progressBar1 = new System.Windows.Forms.ProgressBar();
			this.btnImport = new System.Windows.Forms.Button();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.usageBar1 = new Waveface.Stream.WindowsClient.Src.Control.UsageBar();
			this.lblUsageStatus = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.lblPackage = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.lblName = new System.Windows.Forms.Label();
			this.btnUnLink = new System.Windows.Forms.Button();
			this.chkSubscribed = new System.Windows.Forms.CheckBox();
			this.label4 = new System.Windows.Forms.Label();
			this.lblEmail = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.serviceImportControl1 = new Waveface.Stream.WindowsClient.ServiceImportControl();
			this.tabPage3 = new System.Windows.Forms.TabPage();
			this.personalCloudStatusControl21 = new Waveface.Stream.WindowsClient.PersonalCloudStatusControl2();
			this.tabPage4 = new System.Windows.Forms.TabPage();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.lblResorcePath = new System.Windows.Forms.Label();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.btnUpdate = new System.Windows.Forms.Button();
			this.lblVersion = new System.Windows.Forms.Label();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.panelEx1.SuspendLayout();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.tabPage3.SuspendLayout();
			this.tabPage4.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.SuspendLayout();
			// 
			// linkLabel1
			// 
			this.linkLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.linkLabel1.AutoSize = true;
			this.linkLabel1.Location = new System.Drawing.Point(12, 326);
			this.linkLabel1.Name = "linkLabel1";
			this.linkLabel1.Size = new System.Drawing.Size(60, 12);
			this.linkLabel1.TabIndex = 1;
			this.linkLabel1.TabStop = true;
			this.linkLabel1.Text = "Learn More";
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(526, 322);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 21);
			this.btnOK.TabIndex = 2;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			// 
			// refreshStatusTimer
			// 
			this.refreshStatusTimer.Interval = 1000;
			this.refreshStatusTimer.Tick += new System.EventHandler(this.refreshStatusTimer_Tick);
			// 
			// splitContainer1
			// 
			this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainer1.IsSplitterFixed = true;
			this.splitContainer1.Location = new System.Drawing.Point(12, 12);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.panelEx1);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
			this.splitContainer1.Size = new System.Drawing.Size(593, 302);
			this.splitContainer1.SplitterDistance = 138;
			this.splitContainer1.TabIndex = 0;
			// 
			// panelEx1
			// 
			this.panelEx1.Controls.Add(this.lblSyncTransferStatus);
			this.panelEx1.Controls.Add(this.lblSyncStatus);
			this.panelEx1.Controls.Add(this.lblLocalProcessStatus);
			this.panelEx1.Controls.Add(this.progressBar1);
			this.panelEx1.Controls.Add(this.btnImport);
			this.panelEx1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelEx1.EnableLinearGradientBackground = true;
			this.panelEx1.LinearGradientEndColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(223)))), ((int)(((byte)(183)))));
			this.panelEx1.LinearGradientStartColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(124)))), ((int)(((byte)(55)))));
			this.panelEx1.Location = new System.Drawing.Point(0, 0);
			this.panelEx1.Name = "panelEx1";
			this.panelEx1.Size = new System.Drawing.Size(138, 302);
			this.panelEx1.TabIndex = 3;
			// 
			// lblSyncTransferStatus
			// 
			this.lblSyncTransferStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lblSyncTransferStatus.BackColor = System.Drawing.Color.Transparent;
			this.lblSyncTransferStatus.ForeColor = System.Drawing.Color.White;
			this.lblSyncTransferStatus.Location = new System.Drawing.Point(14, 39);
			this.lblSyncTransferStatus.Name = "lblSyncTransferStatus";
			this.lblSyncTransferStatus.Size = new System.Drawing.Size(107, 57);
			this.lblSyncTransferStatus.TabIndex = 2;
			this.lblSyncTransferStatus.Text = "[Sync Transfer Status]";
			// 
			// lblSyncStatus
			// 
			this.lblSyncStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lblSyncStatus.BackColor = System.Drawing.Color.Transparent;
			this.lblSyncStatus.ForeColor = System.Drawing.Color.White;
			this.lblSyncStatus.Location = new System.Drawing.Point(0, 11);
			this.lblSyncStatus.Name = "lblSyncStatus";
			this.lblSyncStatus.Size = new System.Drawing.Size(138, 11);
			this.lblSyncStatus.TabIndex = 1;
			this.lblSyncStatus.Text = "[Sync Status]";
			this.lblSyncStatus.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// lblLocalProcessStatus
			// 
			this.lblLocalProcessStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lblLocalProcessStatus.BackColor = System.Drawing.Color.Transparent;
			this.lblLocalProcessStatus.ForeColor = System.Drawing.Color.Black;
			this.lblLocalProcessStatus.Location = new System.Drawing.Point(14, 137);
			this.lblLocalProcessStatus.Name = "lblLocalProcessStatus";
			this.lblLocalProcessStatus.Size = new System.Drawing.Size(107, 106);
			this.lblLocalProcessStatus.TabIndex = 0;
			this.lblLocalProcessStatus.Text = "[Process Status]";
			this.lblLocalProcessStatus.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// progressBar1
			// 
			this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.progressBar1.Location = new System.Drawing.Point(14, 246);
			this.progressBar1.Name = "progressBar1";
			this.progressBar1.Size = new System.Drawing.Size(107, 12);
			this.progressBar1.TabIndex = 0;
			this.progressBar1.Value = 50;
			// 
			// btnImport
			// 
			this.btnImport.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.btnImport.Location = new System.Drawing.Point(14, 264);
			this.btnImport.Name = "btnImport";
			this.btnImport.Size = new System.Drawing.Size(107, 21);
			this.btnImport.TabIndex = 0;
			this.btnImport.Text = "Import Photos";
			this.btnImport.UseVisualStyleBackColor = true;
			this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Controls.Add(this.tabPage3);
			this.tabControl1.Controls.Add(this.tabPage4);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.Location = new System.Drawing.Point(0, 0);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(451, 302);
			this.tabControl1.TabIndex = 0;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.groupBox2);
			this.tabPage1.Controls.Add(this.groupBox1);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(443, 276);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Account";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.usageBar1);
			this.groupBox2.Controls.Add(this.lblUsageStatus);
			this.groupBox2.Controls.Add(this.label6);
			this.groupBox2.Controls.Add(this.lblPackage);
			this.groupBox2.Controls.Add(this.label8);
			this.groupBox2.Location = new System.Drawing.Point(3, 140);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(431, 128);
			this.groupBox2.TabIndex = 4;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Service Status";
			// 
			// usageBar1
			// 
			this.usageBar1.Location = new System.Drawing.Point(6, 76);
			this.usageBar1.Maximum = 100;
			this.usageBar1.Minimum = 0;
			this.usageBar1.Name = "usageBar1";
			this.usageBar1.Size = new System.Drawing.Size(419, 32);
			this.usageBar1.TabIndex = 12;
			this.usageBar1.Unit = "MB";
			this.usageBar1.Value = 0;
			// 
			// lblUsageStatus
			// 
			this.lblUsageStatus.AutoSize = true;
			this.lblUsageStatus.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lblUsageStatus.Location = new System.Drawing.Point(89, 46);
			this.lblUsageStatus.Name = "lblUsageStatus";
			this.lblUsageStatus.Size = new System.Drawing.Size(71, 12);
			this.lblUsageStatus.TabIndex = 11;
			this.lblUsageStatus.Text = "[Usage Status]";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.label6.Location = new System.Drawing.Point(6, 46);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(64, 12);
			this.label6.TabIndex = 10;
			this.label6.Text = "Cloud Space";
			// 
			// lblPackage
			// 
			this.lblPackage.AutoSize = true;
			this.lblPackage.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lblPackage.Location = new System.Drawing.Point(89, 18);
			this.lblPackage.Name = "lblPackage";
			this.lblPackage.Size = new System.Drawing.Size(75, 12);
			this.lblPackage.TabIndex = 9;
			this.lblPackage.Text = "[User Package]";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.label8.Location = new System.Drawing.Point(6, 18);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(43, 12);
			this.label8.TabIndex = 8;
			this.label8.Text = "Package";
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.lblName);
			this.groupBox1.Controls.Add(this.btnUnLink);
			this.groupBox1.Controls.Add(this.chkSubscribed);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.lblEmail);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Location = new System.Drawing.Point(6, 6);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(431, 128);
			this.groupBox1.TabIndex = 3;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Account Info";
			// 
			// lblName
			// 
			this.lblName.AutoSize = true;
			this.lblName.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lblName.Location = new System.Drawing.Point(63, 46);
			this.lblName.Name = "lblName";
			this.lblName.Size = new System.Drawing.Size(64, 12);
			this.lblName.TabIndex = 14;
			this.lblName.Text = "[User Name]";
			// 
			// btnUnLink
			// 
			this.btnUnLink.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.btnUnLink.Location = new System.Drawing.Point(277, 72);
			this.btnUnLink.Name = "btnUnLink";
			this.btnUnLink.Size = new System.Drawing.Size(148, 23);
			this.btnUnLink.TabIndex = 13;
			this.btnUnLink.Text = "Logout && Unlink this PC...";
			this.btnUnLink.UseVisualStyleBackColor = true;
			this.btnUnLink.Click += new System.EventHandler(this.btnUnLink_Click);
			// 
			// chkSubscribed
			// 
			this.chkSubscribed.AutoSize = true;
			this.chkSubscribed.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.chkSubscribed.Location = new System.Drawing.Point(6, 76);
			this.chkSubscribed.Name = "chkSubscribed";
			this.chkSubscribed.Size = new System.Drawing.Size(132, 16);
			this.chkSubscribed.TabIndex = 12;
			this.chkSubscribed.Text = "Subscribe to AOStream";
			this.chkSubscribed.UseVisualStyleBackColor = true;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.label4.Location = new System.Drawing.Point(6, 46);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(35, 12);
			this.label4.TabIndex = 10;
			this.label4.Text = "Name ";
			// 
			// lblEmail
			// 
			this.lblEmail.AutoSize = true;
			this.lblEmail.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lblEmail.Location = new System.Drawing.Point(63, 18);
			this.lblEmail.Name = "lblEmail";
			this.lblEmail.Size = new System.Drawing.Size(64, 12);
			this.lblEmail.TabIndex = 9;
			this.lblEmail.Text = "[User Email]";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.label5.Location = new System.Drawing.Point(6, 18);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(35, 12);
			this.label5.TabIndex = 8;
			this.label5.Text = "Email ";
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.serviceImportControl1);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(443, 276);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Web Services";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// serviceImportControl1
			// 
			this.serviceImportControl1.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.serviceImportControl1.CustomLabelForNextStep = null;
			this.serviceImportControl1.CustomSize = new System.Drawing.Size(0, 0);
			this.serviceImportControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.serviceImportControl1.Location = new System.Drawing.Point(3, 3);
			this.serviceImportControl1.Name = "serviceImportControl1";
			this.serviceImportControl1.PageTitle = null;
			this.serviceImportControl1.Size = new System.Drawing.Size(437, 270);
			this.serviceImportControl1.TabIndex = 0;
			this.serviceImportControl1.WizardControl = null;
			// 
			// tabPage3
			// 
			this.tabPage3.Controls.Add(this.personalCloudStatusControl21);
			this.tabPage3.Location = new System.Drawing.Point(4, 22);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage3.Size = new System.Drawing.Size(443, 276);
			this.tabPage3.TabIndex = 2;
			this.tabPage3.Text = "Personal Cloud";
			this.tabPage3.UseVisualStyleBackColor = true;
			// 
			// personalCloudStatusControl21
			// 
			this.personalCloudStatusControl21.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.personalCloudStatusControl21.CustomLabelForNextStep = null;
			this.personalCloudStatusControl21.CustomSize = new System.Drawing.Size(0, 0);
			this.personalCloudStatusControl21.Dock = System.Windows.Forms.DockStyle.Fill;
			this.personalCloudStatusControl21.Location = new System.Drawing.Point(3, 3);
			this.personalCloudStatusControl21.Name = "personalCloudStatusControl21";
			this.personalCloudStatusControl21.PageTitle = null;
			this.personalCloudStatusControl21.Size = new System.Drawing.Size(437, 270);
			this.personalCloudStatusControl21.TabIndex = 0;
			this.personalCloudStatusControl21.WizardControl = null;
			// 
			// tabPage4
			// 
			this.tabPage4.Controls.Add(this.groupBox3);
			this.tabPage4.Controls.Add(this.groupBox4);
			this.tabPage4.Location = new System.Drawing.Point(4, 22);
			this.tabPage4.Name = "tabPage4";
			this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage4.Size = new System.Drawing.Size(443, 276);
			this.tabPage4.TabIndex = 3;
			this.tabPage4.Text = "General";
			this.tabPage4.UseVisualStyleBackColor = true;
			// 
			// groupBox3
			// 
			this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox3.Controls.Add(this.lblResorcePath);
			this.groupBox3.Location = new System.Drawing.Point(6, 6);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(431, 66);
			this.groupBox3.TabIndex = 4;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Location storage location";
			// 
			// lblResorcePath
			// 
			this.lblResorcePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lblResorcePath.AutoEllipsis = true;
			this.lblResorcePath.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lblResorcePath.Location = new System.Drawing.Point(8, 27);
			this.lblResorcePath.Name = "lblResorcePath";
			this.lblResorcePath.Size = new System.Drawing.Size(414, 18);
			this.lblResorcePath.TabIndex = 0;
			this.lblResorcePath.Text = "[Resource Path]";
			// 
			// groupBox4
			// 
			this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox4.Controls.Add(this.btnUpdate);
			this.groupBox4.Controls.Add(this.lblVersion);
			this.groupBox4.Location = new System.Drawing.Point(6, 78);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(431, 63);
			this.groupBox4.TabIndex = 5;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Software infomation";
			// 
			// btnUpdate
			// 
			this.btnUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnUpdate.ForeColor = System.Drawing.Color.Black;
			this.btnUpdate.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btnUpdate.Location = new System.Drawing.Point(317, 25);
			this.btnUpdate.Name = "btnUpdate";
			this.btnUpdate.Size = new System.Drawing.Size(108, 23);
			this.btnUpdate.TabIndex = 1;
			this.btnUpdate.Text = "Check Updates";
			this.btnUpdate.UseVisualStyleBackColor = true;
			this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
			// 
			// lblVersion
			// 
			this.lblVersion.AutoSize = true;
			this.lblVersion.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lblVersion.Location = new System.Drawing.Point(8, 30);
			this.lblVersion.Name = "lblVersion";
			this.lblVersion.Size = new System.Drawing.Size(93, 12);
			this.lblVersion.TabIndex = 0;
			this.lblVersion.Text = "[Software Version]";
			// 
			// ControlPanelDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(617, 352);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.linkLabel1);
			this.Controls.Add(this.splitContainer1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ControlPanelDialog";
			this.Text = "AOStream Control Center";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ControlPanelDialog_FormClosing);
			this.Load += new System.EventHandler(this.ControlPanelDialog_Load);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.panelEx1.ResumeLayout(false);
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.tabPage2.ResumeLayout(false);
			this.tabPage3.ResumeLayout(false);
			this.tabPage4.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.groupBox4.ResumeLayout(false);
			this.groupBox4.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.LinkLabel linkLabel1;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Label lblSyncTransferStatus;
		private System.Windows.Forms.Label lblSyncStatus;
		private System.Windows.Forms.Label lblLocalProcessStatus;
		private System.Windows.Forms.ProgressBar progressBar1;
		private System.Windows.Forms.Button btnImport;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label lblUsageStatus;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label lblPackage;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Button btnUnLink;
		private System.Windows.Forms.CheckBox chkSubscribed;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label lblEmail;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TabPage tabPage3;
		private System.Windows.Forms.TabPage tabPage4;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Label lblResorcePath;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.Button btnUpdate;
		private System.Windows.Forms.Label lblVersion;
		private System.Windows.Forms.Label lblName;
		private Src.Control.UsageBar usageBar1;
		private ServiceImportControl serviceImportControl1;
		private PersonalCloudStatusControl2 personalCloudStatusControl21;
		private PanelEx panelEx1;
		private System.Windows.Forms.Timer refreshStatusTimer;
	}
}