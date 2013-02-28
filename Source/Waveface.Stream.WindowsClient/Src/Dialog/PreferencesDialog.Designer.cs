namespace Waveface.Stream.WindowsClient
{
	partial class PreferencesDialog
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PreferencesDialog));
			this.linkLabel1 = new System.Windows.Forms.LinkLabel();
			this.btnOK = new System.Windows.Forms.Button();
			this.refreshStatusTimer = new System.Windows.Forms.Timer(this.components);
			this.btnCancel = new System.Windows.Forms.Button();
			this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
			this.systemIconControl1 = new Waveface.Stream.WindowsClient.SystemIconControl();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.panelEx1 = new Waveface.Stream.WindowsClient.PanelEx();
			this.devNameCtl = new Waveface.Stream.WindowsClient.Src.Control.DeviceNameControl();
			this.label2 = new System.Windows.Forms.Label();
			this.lblDeviceConnectStatus = new System.Windows.Forms.Label();
			this.cmbDevice = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.pbxLogo = new System.Windows.Forms.PictureBox();
			this.lblSyncTransferStatus = new System.Windows.Forms.Label();
			this.lblSyncStatus = new System.Windows.Forms.Label();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabGeneral = new System.Windows.Forms.TabPage();
			this.usageDetailControl1 = new Waveface.Stream.WindowsClient.Src.Control.UsageDetailControl();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.checkBox2 = new System.Windows.Forms.CheckBox();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.tabDevices = new System.Windows.Forms.TabPage();
			this.personalCloudStatusControl21 = new Waveface.Stream.WindowsClient.PersonalCloudStatusControl2();
			this.tabAccount = new System.Windows.Forms.TabPage();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.planBox1 = new Waveface.Stream.WindowsClient.PlanBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.button3 = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.panel2 = new System.Windows.Forms.Panel();
			this.button2 = new System.Windows.Forms.Button();
			this.btnUnLink = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.tbxEmail = new System.Windows.Forms.TextBox();
			this.tbxName = new System.Windows.Forms.TextBox();
			this.tabConnections = new System.Windows.Forms.TabPage();
			this.serviceImportControl1 = new Waveface.Stream.WindowsClient.ServiceImportControl();
			this.tabAbout = new System.Windows.Forms.TabPage();
			this.aboutControl1 = new Waveface.Stream.WindowsClient.Src.Control.AboutControl();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.panelEx1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pbxLogo)).BeginInit();
			this.tabControl1.SuspendLayout();
			this.tabGeneral.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.tabDevices.SuspendLayout();
			this.tabAccount.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.panel1.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.tabConnections.SuspendLayout();
			this.tabAbout.SuspendLayout();
			this.SuspendLayout();
			// 
			// linkLabel1
			// 
			resources.ApplyResources(this.linkLabel1, "linkLabel1");
			this.errorProvider1.SetIconAlignment(this.linkLabel1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("linkLabel1.IconAlignment"))));
			this.linkLabel1.Name = "linkLabel1";
			this.linkLabel1.TabStop = true;
			this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
			// 
			// btnOK
			// 
			resources.ApplyResources(this.btnOK, "btnOK");
			this.errorProvider1.SetIconAlignment(this.btnOK, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("btnOK.IconAlignment"))));
			this.btnOK.Name = "btnOK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// refreshStatusTimer
			// 
			this.refreshStatusTimer.Interval = 1000;
			this.refreshStatusTimer.Tick += new System.EventHandler(this.refreshStatusTimer_Tick);
			// 
			// btnCancel
			// 
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.errorProvider1.SetIconAlignment(this.btnCancel, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("btnCancel.IconAlignment"))));
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// errorProvider1
			// 
			this.errorProvider1.ContainerControl = this;
			// 
			// systemIconControl1
			// 
			resources.ApplyResources(this.systemIconControl1, "systemIconControl1");
			this.errorProvider1.SetIconAlignment(this.systemIconControl1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("systemIconControl1.IconAlignment"))));
			this.systemIconControl1.IconType = Waveface.Stream.WindowsClient.SystemIconControl.SystemIconType.Question;
			this.systemIconControl1.Name = "systemIconControl1";
			// 
			// splitContainer1
			// 
			resources.ApplyResources(this.splitContainer1, "splitContainer1");
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.errorProvider1.SetIconAlignment(this.splitContainer1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("splitContainer1.IconAlignment"))));
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.panelEx1);
			this.errorProvider1.SetIconAlignment(this.splitContainer1.Panel1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("splitContainer1.Panel1.IconAlignment"))));
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
			this.errorProvider1.SetIconAlignment(this.splitContainer1.Panel2, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("splitContainer1.Panel2.IconAlignment"))));
			// 
			// panelEx1
			// 
			this.panelEx1.Controls.Add(this.devNameCtl);
			this.panelEx1.Controls.Add(this.label2);
			this.panelEx1.Controls.Add(this.lblDeviceConnectStatus);
			this.panelEx1.Controls.Add(this.cmbDevice);
			this.panelEx1.Controls.Add(this.label1);
			this.panelEx1.Controls.Add(this.pbxLogo);
			this.panelEx1.Controls.Add(this.lblSyncTransferStatus);
			this.panelEx1.Controls.Add(this.lblSyncStatus);
			resources.ApplyResources(this.panelEx1, "panelEx1");
			this.panelEx1.EnableLinearGradientBackground = true;
			this.errorProvider1.SetIconAlignment(this.panelEx1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("panelEx1.IconAlignment"))));
			this.panelEx1.LinearGradientEndColor = System.Drawing.Color.White;
			this.panelEx1.LinearGradientStartColor = System.Drawing.Color.SlateGray;
			this.panelEx1.Name = "panelEx1";
			// 
			// devNameCtl
			// 
			this.devNameCtl.BackColor = System.Drawing.Color.Transparent;
			this.devNameCtl.DeviceName = "[DeviceName]";
			resources.ApplyResources(this.devNameCtl, "devNameCtl");
			this.devNameCtl.Name = "devNameCtl";
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.BackColor = System.Drawing.Color.Transparent;
			this.errorProvider1.SetIconAlignment(this.label2, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label2.IconAlignment"))));
			this.label2.Name = "label2";
			// 
			// lblDeviceConnectStatus
			// 
			resources.ApplyResources(this.lblDeviceConnectStatus, "lblDeviceConnectStatus");
			this.lblDeviceConnectStatus.BackColor = System.Drawing.Color.Transparent;
			this.errorProvider1.SetIconAlignment(this.lblDeviceConnectStatus, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("lblDeviceConnectStatus.IconAlignment"))));
			this.lblDeviceConnectStatus.Name = "lblDeviceConnectStatus";
			// 
			// cmbDevice
			// 
			resources.ApplyResources(this.cmbDevice, "cmbDevice");
			this.cmbDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbDevice.FormattingEnabled = true;
			this.errorProvider1.SetIconAlignment(this.cmbDevice, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("cmbDevice.IconAlignment"))));
			this.cmbDevice.Name = "cmbDevice";
			this.cmbDevice.TextChanged += new System.EventHandler(this.cmbDevice_TextChanged);
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.BackColor = System.Drawing.Color.Transparent;
			this.errorProvider1.SetIconAlignment(this.label1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label1.IconAlignment"))));
			this.label1.Name = "label1";
			// 
			// pbxLogo
			// 
			resources.ApplyResources(this.pbxLogo, "pbxLogo");
			this.pbxLogo.BackColor = System.Drawing.Color.Transparent;
			this.errorProvider1.SetIconAlignment(this.pbxLogo, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("pbxLogo.IconAlignment"))));
			this.pbxLogo.Name = "pbxLogo";
			this.pbxLogo.TabStop = false;
			// 
			// lblSyncTransferStatus
			// 
			resources.ApplyResources(this.lblSyncTransferStatus, "lblSyncTransferStatus");
			this.lblSyncTransferStatus.BackColor = System.Drawing.Color.Transparent;
			this.errorProvider1.SetIconAlignment(this.lblSyncTransferStatus, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("lblSyncTransferStatus.IconAlignment"))));
			this.lblSyncTransferStatus.Name = "lblSyncTransferStatus";
			// 
			// lblSyncStatus
			// 
			resources.ApplyResources(this.lblSyncStatus, "lblSyncStatus");
			this.lblSyncStatus.BackColor = System.Drawing.Color.Transparent;
			this.errorProvider1.SetIconAlignment(this.lblSyncStatus, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("lblSyncStatus.IconAlignment"))));
			this.lblSyncStatus.Name = "lblSyncStatus";
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabGeneral);
			this.tabControl1.Controls.Add(this.tabDevices);
			this.tabControl1.Controls.Add(this.tabAccount);
			this.tabControl1.Controls.Add(this.tabConnections);
			this.tabControl1.Controls.Add(this.tabAbout);
			resources.ApplyResources(this.tabControl1, "tabControl1");
			this.errorProvider1.SetIconAlignment(this.tabControl1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("tabControl1.IconAlignment"))));
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
			// 
			// tabGeneral
			// 
			this.tabGeneral.Controls.Add(this.usageDetailControl1);
			this.tabGeneral.Controls.Add(this.groupBox3);
			this.errorProvider1.SetIconAlignment(this.tabGeneral, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("tabGeneral.IconAlignment"))));
			resources.ApplyResources(this.tabGeneral, "tabGeneral");
			this.tabGeneral.Name = "tabGeneral";
			this.tabGeneral.UseVisualStyleBackColor = true;
			// 
			// usageDetailControl1
			// 
			resources.ApplyResources(this.usageDetailControl1, "usageDetailControl1");
			this.usageDetailControl1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.usageDetailControl1.CloudTotalUsage = "[Cloud Total Usage]";
			this.errorProvider1.SetIconAlignment(this.usageDetailControl1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("usageDetailControl1.IconAlignment"))));
			this.usageDetailControl1.LocalDocument = "---";
			this.usageDetailControl1.LocalPhoto = "---";
			this.usageDetailControl1.Name = "usageDetailControl1";
			this.usageDetailControl1.ResourcePath = "[Resource Folder]";
			this.usageDetailControl1.TotalDocument = ((long)(500));
			this.usageDetailControl1.TotalPhoto = ((long)(500));
			this.usageDetailControl1.TotalWeb = ((long)(500));
			this.usageDetailControl1.ChangeResourcePathButtonClick += new System.EventHandler(this.usageDetailControl1_ChangeResourcePathButtonClick);
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.checkBox2);
			this.groupBox3.Controls.Add(this.checkBox1);
			resources.ApplyResources(this.groupBox3, "groupBox3");
			this.errorProvider1.SetIconAlignment(this.groupBox3, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("groupBox3.IconAlignment"))));
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.TabStop = false;
			// 
			// checkBox2
			// 
			resources.ApplyResources(this.checkBox2, "checkBox2");
			this.errorProvider1.SetIconAlignment(this.checkBox2, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("checkBox2.IconAlignment"))));
			this.checkBox2.Name = "checkBox2";
			this.checkBox2.UseVisualStyleBackColor = true;
			// 
			// checkBox1
			// 
			resources.ApplyResources(this.checkBox1, "checkBox1");
			this.errorProvider1.SetIconAlignment(this.checkBox1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("checkBox1.IconAlignment"))));
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.UseVisualStyleBackColor = true;
			// 
			// tabDevices
			// 
			this.tabDevices.Controls.Add(this.personalCloudStatusControl21);
			this.errorProvider1.SetIconAlignment(this.tabDevices, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("tabDevices.IconAlignment"))));
			resources.ApplyResources(this.tabDevices, "tabDevices");
			this.tabDevices.Name = "tabDevices";
			this.tabDevices.UseVisualStyleBackColor = true;
			// 
			// personalCloudStatusControl21
			// 
			this.personalCloudStatusControl21.BackColor = System.Drawing.SystemColors.ControlLightLight;
			resources.ApplyResources(this.personalCloudStatusControl21, "personalCloudStatusControl21");
			this.personalCloudStatusControl21.EnableAutoRefreshStatus = false;
			this.errorProvider1.SetIconAlignment(this.personalCloudStatusControl21, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("personalCloudStatusControl21.IconAlignment"))));
			this.personalCloudStatusControl21.Name = "personalCloudStatusControl21";
			this.personalCloudStatusControl21.RefreshInterval = 15000;
			// 
			// tabAccount
			// 
			this.tabAccount.Controls.Add(this.groupBox2);
			this.tabAccount.Controls.Add(this.groupBox1);
			this.errorProvider1.SetIconAlignment(this.tabAccount, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("tabAccount.IconAlignment"))));
			resources.ApplyResources(this.tabAccount, "tabAccount");
			this.tabAccount.Name = "tabAccount";
			this.tabAccount.UseVisualStyleBackColor = true;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.planBox1);
			this.groupBox2.Controls.Add(this.panel1);
			resources.ApplyResources(this.groupBox2, "groupBox2");
			this.errorProvider1.SetIconAlignment(this.groupBox2, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("groupBox2.IconAlignment"))));
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.TabStop = false;
			// 
			// planBox1
			// 
			this.planBox1.BackColor = System.Drawing.SystemColors.ControlLightLight;
			resources.ApplyResources(this.planBox1, "planBox1");
			this.planBox1.HeaderVisibile = false;
			this.errorProvider1.SetIconAlignment(this.planBox1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("planBox1.IconAlignment"))));
			this.planBox1.Name = "planBox1";
			this.planBox1.Type = Waveface.Stream.WindowsClient.PlanBox.PlanType.Free;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.button3);
			resources.ApplyResources(this.panel1, "panel1");
			this.errorProvider1.SetIconAlignment(this.panel1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("panel1.IconAlignment"))));
			this.panel1.Name = "panel1";
			// 
			// button3
			// 
			resources.ApplyResources(this.button3, "button3");
			this.errorProvider1.SetIconAlignment(this.button3, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("button3.IconAlignment"))));
			this.button3.Name = "button3";
			this.button3.UseVisualStyleBackColor = true;
			this.button3.Click += new System.EventHandler(this.button3_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.panel2);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.tbxEmail);
			this.groupBox1.Controls.Add(this.tbxName);
			resources.ApplyResources(this.groupBox1, "groupBox1");
			this.errorProvider1.SetIconAlignment(this.groupBox1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("groupBox1.IconAlignment"))));
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.TabStop = false;
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.button2);
			this.panel2.Controls.Add(this.btnUnLink);
			resources.ApplyResources(this.panel2, "panel2");
			this.errorProvider1.SetIconAlignment(this.panel2, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("panel2.IconAlignment"))));
			this.panel2.Name = "panel2";
			// 
			// button2
			// 
			resources.ApplyResources(this.button2, "button2");
			this.errorProvider1.SetIconAlignment(this.button2, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("button2.IconAlignment"))));
			this.button2.Name = "button2";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// btnUnLink
			// 
			resources.ApplyResources(this.btnUnLink, "btnUnLink");
			this.errorProvider1.SetIconAlignment(this.btnUnLink, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("btnUnLink.IconAlignment"))));
			this.btnUnLink.Name = "btnUnLink";
			this.btnUnLink.UseVisualStyleBackColor = true;
			this.btnUnLink.Click += new System.EventHandler(this.btnUnLink_Click);
			// 
			// label4
			// 
			resources.ApplyResources(this.label4, "label4");
			this.errorProvider1.SetIconAlignment(this.label4, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label4.IconAlignment"))));
			this.label4.Name = "label4";
			// 
			// label5
			// 
			resources.ApplyResources(this.label5, "label5");
			this.errorProvider1.SetIconAlignment(this.label5, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label5.IconAlignment"))));
			this.label5.Name = "label5";
			// 
			// tbxEmail
			// 
			this.errorProvider1.SetIconAlignment(this.tbxEmail, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("tbxEmail.IconAlignment"))));
			resources.ApplyResources(this.tbxEmail, "tbxEmail");
			this.tbxEmail.Name = "tbxEmail";
			this.tbxEmail.Leave += new System.EventHandler(this.tbxEmail_Leave);
			// 
			// tbxName
			// 
			this.errorProvider1.SetIconAlignment(this.tbxName, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("tbxName.IconAlignment"))));
			resources.ApplyResources(this.tbxName, "tbxName");
			this.tbxName.Name = "tbxName";
			this.tbxName.Leave += new System.EventHandler(this.tbxName_Leave);
			// 
			// tabConnections
			// 
			this.tabConnections.Controls.Add(this.serviceImportControl1);
			this.errorProvider1.SetIconAlignment(this.tabConnections, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("tabConnections.IconAlignment"))));
			resources.ApplyResources(this.tabConnections, "tabConnections");
			this.tabConnections.Name = "tabConnections";
			this.tabConnections.UseVisualStyleBackColor = true;
			// 
			// serviceImportControl1
			// 
			this.serviceImportControl1.BackColor = System.Drawing.SystemColors.ControlLightLight;
			resources.ApplyResources(this.serviceImportControl1, "serviceImportControl1");
			this.errorProvider1.SetIconAlignment(this.serviceImportControl1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("serviceImportControl1.IconAlignment"))));
			this.serviceImportControl1.Name = "serviceImportControl1";
			// 
			// tabAbout
			// 
			this.tabAbout.Controls.Add(this.aboutControl1);
			this.errorProvider1.SetIconAlignment(this.tabAbout, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("tabAbout.IconAlignment"))));
			resources.ApplyResources(this.tabAbout, "tabAbout");
			this.tabAbout.Name = "tabAbout";
			this.tabAbout.UseVisualStyleBackColor = true;
			// 
			// aboutControl1
			// 
			resources.ApplyResources(this.aboutControl1, "aboutControl1");
			this.errorProvider1.SetIconAlignment(this.aboutControl1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("aboutControl1.IconAlignment"))));
			this.aboutControl1.Name = "aboutControl1";
			// 
			// PreferencesDialog
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.systemIconControl1);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.linkLabel1);
			this.Controls.Add(this.splitContainer1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PreferencesDialog";
			this.Load += new System.EventHandler(this.PreferencesDialog_Load);
			((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.panelEx1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pbxLogo)).EndInit();
			this.tabControl1.ResumeLayout(false);
			this.tabGeneral.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.tabDevices.ResumeLayout(false);
			this.tabAccount.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.panel2.ResumeLayout(false);
			this.tabConnections.ResumeLayout(false);
			this.tabAbout.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabAccount;
		private System.Windows.Forms.TabPage tabConnections;
		private System.Windows.Forms.LinkLabel linkLabel1;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Label lblSyncTransferStatus;
		private System.Windows.Forms.Label lblSyncStatus;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Button btnUnLink;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TabPage tabDevices;
		private System.Windows.Forms.TabPage tabGeneral;
		private ServiceImportControl serviceImportControl1;
		private PanelEx panelEx1;
		private System.Windows.Forms.Timer refreshStatusTimer;
		private SystemIconControl systemIconControl1;
		private System.Windows.Forms.PictureBox pbxLogo;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.CheckBox checkBox1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button button3;
		private PlanBox planBox1;
		private System.Windows.Forms.Panel panel2;
		private Src.Control.UsageDetailControl usageDetailControl1;
		private System.Windows.Forms.TextBox tbxEmail;
		private System.Windows.Forms.TextBox tbxName;
		private System.Windows.Forms.CheckBox checkBox2;
		private System.Windows.Forms.Label lblDeviceConnectStatus;
		private System.Windows.Forms.ComboBox cmbDevice;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TabPage tabAbout;
		private Src.Control.AboutControl aboutControl1;
		private System.Windows.Forms.Button btnCancel;
		private PersonalCloudStatusControl2 personalCloudStatusControl21;
		private System.Windows.Forms.ErrorProvider errorProvider1;
		private Src.Control.DeviceNameControl devNameCtl;
	}
}