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
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabGeneral = new System.Windows.Forms.TabPage();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.checkBox2 = new System.Windows.Forms.CheckBox();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.tabDevices = new System.Windows.Forms.TabPage();
			this.tabAccount = new System.Windows.Forms.TabPage();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.button3 = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.lnklblCancelName = new System.Windows.Forms.LinkLabel();
			this.lnklblCancelEmail = new System.Windows.Forms.LinkLabel();
			this.lnklblSaveName = new System.Windows.Forms.LinkLabel();
			this.lnklblSaveEmail = new System.Windows.Forms.LinkLabel();
			this.lnklblEditName = new System.Windows.Forms.LinkLabel();
			this.lnklblEditEmail = new System.Windows.Forms.LinkLabel();
			this.panel2 = new System.Windows.Forms.Panel();
			this.button2 = new System.Windows.Forms.Button();
			this.btnUnLink = new System.Windows.Forms.Button();
			this.lblName = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.lblEmail = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.tbxEmail = new System.Windows.Forms.TextBox();
			this.tbxName = new System.Windows.Forms.TextBox();
			this.tabConnections = new System.Windows.Forms.TabPage();
			this.systemIconControl1 = new Waveface.Stream.WindowsClient.SystemIconControl();
			this.panelEx1 = new Waveface.Stream.WindowsClient.PanelEx();
			this.lblDeviceName = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.lblDeviceConnectStatus = new System.Windows.Forms.Label();
			this.cmbDevice = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.pbxLogo = new System.Windows.Forms.PictureBox();
			this.lblSyncTransferStatus = new System.Windows.Forms.Label();
			this.lblSyncStatus = new System.Windows.Forms.Label();
			this.usageDetailControl1 = new Waveface.Stream.WindowsClient.Src.Control.UsageDetailControl();
			this.personalCloudStatusControl21 = new Waveface.Stream.WindowsClient.PersonalCloudStatusControl2();
			this.planBox1 = new Waveface.Stream.WindowsClient.PlanBox();
			this.serviceImportControl1 = new Waveface.Stream.WindowsClient.ServiceImportControl();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
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
			this.panelEx1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pbxLogo)).BeginInit();
			this.SuspendLayout();
			// 
			// linkLabel1
			// 
			this.linkLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.linkLabel1.AutoSize = true;
			this.linkLabel1.Location = new System.Drawing.Point(44, 349);
			this.linkLabel1.Name = "linkLabel1";
			this.linkLabel1.Size = new System.Drawing.Size(27, 12);
			this.linkLabel1.TabIndex = 1;
			this.linkLabel1.TabStop = true;
			this.linkLabel1.Text = "Help";
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(588, 345);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 21);
			this.btnOK.TabIndex = 2;
			this.btnOK.Text = "Close";
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
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
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
			this.splitContainer1.Size = new System.Drawing.Size(655, 325);
			this.splitContainer1.SplitterDistance = 138;
			this.splitContainer1.TabIndex = 0;
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabGeneral);
			this.tabControl1.Controls.Add(this.tabDevices);
			this.tabControl1.Controls.Add(this.tabAccount);
			this.tabControl1.Controls.Add(this.tabConnections);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.Location = new System.Drawing.Point(0, 0);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(513, 325);
			this.tabControl1.TabIndex = 0;
			// 
			// tabGeneral
			// 
			this.tabGeneral.Controls.Add(this.usageDetailControl1);
			this.tabGeneral.Controls.Add(this.groupBox3);
			this.tabGeneral.Location = new System.Drawing.Point(4, 22);
			this.tabGeneral.Name = "tabGeneral";
			this.tabGeneral.Padding = new System.Windows.Forms.Padding(3);
			this.tabGeneral.Size = new System.Drawing.Size(505, 299);
			this.tabGeneral.TabIndex = 3;
			this.tabGeneral.Text = "General";
			this.tabGeneral.UseVisualStyleBackColor = true;
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.checkBox2);
			this.groupBox3.Controls.Add(this.checkBox1);
			this.groupBox3.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.groupBox3.Location = new System.Drawing.Point(3, 214);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(499, 82);
			this.groupBox3.TabIndex = 0;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Import Wizard";
			// 
			// checkBox2
			// 
			this.checkBox2.AutoSize = true;
			this.checkBox2.Location = new System.Drawing.Point(10, 50);
			this.checkBox2.Name = "checkBox2";
			this.checkBox2.Size = new System.Drawing.Size(265, 16);
			this.checkBox2.TabIndex = 1;
			this.checkBox2.Text = "開啟PDF及PowerPoint檔案後，自動匯入該文件";
			this.checkBox2.UseVisualStyleBackColor = true;
			this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
			// 
			// checkBox1
			// 
			this.checkBox1.AutoSize = true;
			this.checkBox1.Location = new System.Drawing.Point(10, 28);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new System.Drawing.Size(240, 16);
			this.checkBox1.TabIndex = 0;
			this.checkBox1.Text = "插入外接裝置時，自動開啟檔案匯入精靈";
			this.checkBox1.UseVisualStyleBackColor = true;
			this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
			// 
			// tabDevices
			// 
			this.tabDevices.Controls.Add(this.personalCloudStatusControl21);
			this.tabDevices.Location = new System.Drawing.Point(4, 22);
			this.tabDevices.Name = "tabDevices";
			this.tabDevices.Padding = new System.Windows.Forms.Padding(3);
			this.tabDevices.Size = new System.Drawing.Size(505, 299);
			this.tabDevices.TabIndex = 2;
			this.tabDevices.Text = "Personal Cloud";
			this.tabDevices.UseVisualStyleBackColor = true;
			// 
			// tabAccount
			// 
			this.tabAccount.Controls.Add(this.groupBox2);
			this.tabAccount.Controls.Add(this.groupBox1);
			this.tabAccount.Location = new System.Drawing.Point(4, 22);
			this.tabAccount.Name = "tabAccount";
			this.tabAccount.Padding = new System.Windows.Forms.Padding(3);
			this.tabAccount.Size = new System.Drawing.Size(505, 299);
			this.tabAccount.TabIndex = 0;
			this.tabAccount.Text = "Account";
			this.tabAccount.UseVisualStyleBackColor = true;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.planBox1);
			this.groupBox2.Controls.Add(this.panel1);
			this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox2.Location = new System.Drawing.Point(3, 111);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(499, 185);
			this.groupBox2.TabIndex = 4;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Plan";
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.button3);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(3, 151);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(493, 31);
			this.panel1.TabIndex = 18;
			// 
			// button3
			// 
			this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button3.Location = new System.Drawing.Point(380, 3);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(110, 23);
			this.button3.TabIndex = 18;
			this.button3.Text = "Pick your Right Plan";
			this.button3.UseVisualStyleBackColor = true;
			this.button3.Click += new System.EventHandler(this.button3_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.lnklblCancelName);
			this.groupBox1.Controls.Add(this.lnklblCancelEmail);
			this.groupBox1.Controls.Add(this.lnklblSaveName);
			this.groupBox1.Controls.Add(this.lnklblSaveEmail);
			this.groupBox1.Controls.Add(this.lnklblEditName);
			this.groupBox1.Controls.Add(this.lnklblEditEmail);
			this.groupBox1.Controls.Add(this.panel2);
			this.groupBox1.Controls.Add(this.lblName);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.lblEmail);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.tbxEmail);
			this.groupBox1.Controls.Add(this.tbxName);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
			this.groupBox1.Location = new System.Drawing.Point(3, 3);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(499, 108);
			this.groupBox1.TabIndex = 3;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Account Info";
			// 
			// lnklblCancelName
			// 
			this.lnklblCancelName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lnklblCancelName.AutoSize = true;
			this.lnklblCancelName.Location = new System.Drawing.Point(269, 46);
			this.lnklblCancelName.Name = "lnklblCancelName";
			this.lnklblCancelName.Size = new System.Drawing.Size(37, 12);
			this.lnklblCancelName.TabIndex = 26;
			this.lnklblCancelName.TabStop = true;
			this.lnklblCancelName.Text = "Cancel";
			this.lnklblCancelName.Visible = false;
			this.lnklblCancelName.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnklblCancelName_LinkClicked);
			// 
			// lnklblCancelEmail
			// 
			this.lnklblCancelEmail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lnklblCancelEmail.AutoSize = true;
			this.lnklblCancelEmail.Location = new System.Drawing.Point(269, 18);
			this.lnklblCancelEmail.Name = "lnklblCancelEmail";
			this.lnklblCancelEmail.Size = new System.Drawing.Size(37, 12);
			this.lnklblCancelEmail.TabIndex = 25;
			this.lnklblCancelEmail.TabStop = true;
			this.lnklblCancelEmail.Text = "Cancel";
			this.lnklblCancelEmail.Visible = false;
			this.lnklblCancelEmail.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnklblCancelEmail_LinkClicked);
			// 
			// lnklblSaveName
			// 
			this.lnklblSaveName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lnklblSaveName.AutoSize = true;
			this.lnklblSaveName.Location = new System.Drawing.Point(236, 46);
			this.lnklblSaveName.Name = "lnklblSaveName";
			this.lnklblSaveName.Size = new System.Drawing.Size(27, 12);
			this.lnklblSaveName.TabIndex = 24;
			this.lnklblSaveName.TabStop = true;
			this.lnklblSaveName.Text = "Save";
			this.lnklblSaveName.Visible = false;
			this.lnklblSaveName.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnklblSaveName_LinkClicked);
			// 
			// lnklblSaveEmail
			// 
			this.lnklblSaveEmail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lnklblSaveEmail.AutoSize = true;
			this.lnklblSaveEmail.Location = new System.Drawing.Point(236, 18);
			this.lnklblSaveEmail.Name = "lnklblSaveEmail";
			this.lnklblSaveEmail.Size = new System.Drawing.Size(27, 12);
			this.lnklblSaveEmail.TabIndex = 23;
			this.lnklblSaveEmail.TabStop = true;
			this.lnklblSaveEmail.Text = "Save";
			this.lnklblSaveEmail.Visible = false;
			this.lnklblSaveEmail.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnklblSaveEmail_LinkClicked);
			// 
			// lnklblEditName
			// 
			this.lnklblEditName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lnklblEditName.AutoSize = true;
			this.lnklblEditName.Location = new System.Drawing.Point(133, 46);
			this.lnklblEditName.Name = "lnklblEditName";
			this.lnklblEditName.Size = new System.Drawing.Size(24, 12);
			this.lnklblEditName.TabIndex = 20;
			this.lnklblEditName.TabStop = true;
			this.lnklblEditName.Text = "Edit";
			this.lnklblEditName.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnklblName_LinkClicked);
			// 
			// lnklblEditEmail
			// 
			this.lnklblEditEmail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lnklblEditEmail.AutoSize = true;
			this.lnklblEditEmail.Location = new System.Drawing.Point(133, 18);
			this.lnklblEditEmail.Name = "lnklblEditEmail";
			this.lnklblEditEmail.Size = new System.Drawing.Size(24, 12);
			this.lnklblEditEmail.TabIndex = 3;
			this.lnklblEditEmail.TabStop = true;
			this.lnklblEditEmail.Text = "Edit";
			this.lnklblEditEmail.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnklblEmail_LinkClicked);
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.button2);
			this.panel2.Controls.Add(this.btnUnLink);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel2.Location = new System.Drawing.Point(3, 74);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(493, 31);
			this.panel2.TabIndex = 19;
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(380, 3);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(110, 23);
			this.button2.TabIndex = 16;
			this.button2.Text = "Delete Account";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// btnUnLink
			// 
			this.btnUnLink.Location = new System.Drawing.Point(226, 3);
			this.btnUnLink.Name = "btnUnLink";
			this.btnUnLink.Size = new System.Drawing.Size(148, 23);
			this.btnUnLink.TabIndex = 13;
			this.btnUnLink.Text = "Logout && Unlink this PC...";
			this.btnUnLink.UseVisualStyleBackColor = true;
			this.btnUnLink.Click += new System.EventHandler(this.btnUnLink_Click);
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
			// tbxEmail
			// 
			this.tbxEmail.Location = new System.Drawing.Point(65, 15);
			this.tbxEmail.Name = "tbxEmail";
			this.tbxEmail.Size = new System.Drawing.Size(165, 22);
			this.tbxEmail.TabIndex = 21;
			this.tbxEmail.Visible = false;
			// 
			// tbxName
			// 
			this.tbxName.Location = new System.Drawing.Point(65, 43);
			this.tbxName.Name = "tbxName";
			this.tbxName.Size = new System.Drawing.Size(165, 22);
			this.tbxName.TabIndex = 22;
			this.tbxName.Visible = false;
			// 
			// tabConnections
			// 
			this.tabConnections.Controls.Add(this.serviceImportControl1);
			this.tabConnections.Location = new System.Drawing.Point(4, 22);
			this.tabConnections.Name = "tabConnections";
			this.tabConnections.Padding = new System.Windows.Forms.Padding(3);
			this.tabConnections.Size = new System.Drawing.Size(505, 299);
			this.tabConnections.TabIndex = 1;
			this.tabConnections.Text = "Web Services";
			this.tabConnections.UseVisualStyleBackColor = true;
			// 
			// systemIconControl1
			// 
			this.systemIconControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.systemIconControl1.IconType = Waveface.Stream.WindowsClient.SystemIconControl.SystemIconType.Question;
			this.systemIconControl1.Location = new System.Drawing.Point(12, 343);
			this.systemIconControl1.Name = "systemIconControl1";
			this.systemIconControl1.Size = new System.Drawing.Size(26, 23);
			this.systemIconControl1.TabIndex = 2;
			this.systemIconControl1.Text = "systemIconControl1";
			// 
			// panelEx1
			// 
			this.panelEx1.Controls.Add(this.lblDeviceName);
			this.panelEx1.Controls.Add(this.label2);
			this.panelEx1.Controls.Add(this.lblDeviceConnectStatus);
			this.panelEx1.Controls.Add(this.cmbDevice);
			this.panelEx1.Controls.Add(this.label1);
			this.panelEx1.Controls.Add(this.pbxLogo);
			this.panelEx1.Controls.Add(this.lblSyncTransferStatus);
			this.panelEx1.Controls.Add(this.lblSyncStatus);
			this.panelEx1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelEx1.EnableLinearGradientBackground = true;
			this.panelEx1.LinearGradientEndColor = System.Drawing.Color.White;
			this.panelEx1.LinearGradientStartColor = System.Drawing.Color.SlateGray;
			this.panelEx1.Location = new System.Drawing.Point(0, 0);
			this.panelEx1.Name = "panelEx1";
			this.panelEx1.Size = new System.Drawing.Size(138, 325);
			this.panelEx1.TabIndex = 3;
			// 
			// lblDeviceName
			// 
			this.lblDeviceName.BackColor = System.Drawing.Color.Transparent;
			this.lblDeviceName.Font = new System.Drawing.Font("PMingLiU", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.lblDeviceName.Location = new System.Drawing.Point(4, 223);
			this.lblDeviceName.Name = "lblDeviceName";
			this.lblDeviceName.Size = new System.Drawing.Size(132, 23);
			this.lblDeviceName.TabIndex = 10;
			this.lblDeviceName.Text = "[Device Name]";
			this.lblDeviceName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.BackColor = System.Drawing.Color.Transparent;
			this.label2.Location = new System.Drawing.Point(14, 223);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(107, 57);
			this.label2.TabIndex = 9;
			this.label2.Text = "[Device Sync Status]";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// lblDeviceConnectStatus
			// 
			this.lblDeviceConnectStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lblDeviceConnectStatus.BackColor = System.Drawing.Color.Transparent;
			this.lblDeviceConnectStatus.Location = new System.Drawing.Point(14, 307);
			this.lblDeviceConnectStatus.Name = "lblDeviceConnectStatus";
			this.lblDeviceConnectStatus.Size = new System.Drawing.Size(107, 14);
			this.lblDeviceConnectStatus.TabIndex = 8;
			this.lblDeviceConnectStatus.Text = "[Device Connect Status]";
			this.lblDeviceConnectStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// cmbDevice
			// 
			this.cmbDevice.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.cmbDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbDevice.FormattingEnabled = true;
			this.cmbDevice.Location = new System.Drawing.Point(16, 284);
			this.cmbDevice.Name = "cmbDevice";
			this.cmbDevice.Size = new System.Drawing.Size(105, 20);
			this.cmbDevice.TabIndex = 7;
			this.cmbDevice.TextChanged += new System.EventHandler(this.cmbDevice_TextChanged);
			// 
			// label1
			// 
			this.label1.BackColor = System.Drawing.Color.Transparent;
			this.label1.Font = new System.Drawing.Font("PMingLiU", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			this.label1.Location = new System.Drawing.Point(4, 100);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(132, 23);
			this.label1.TabIndex = 6;
			this.label1.Text = "AOStream Station";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// pbxLogo
			// 
			this.pbxLogo.BackColor = System.Drawing.Color.Transparent;
			this.pbxLogo.Location = new System.Drawing.Point(21, 12);
			this.pbxLogo.Name = "pbxLogo";
			this.pbxLogo.Size = new System.Drawing.Size(100, 82);
			this.pbxLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pbxLogo.TabIndex = 6;
			this.pbxLogo.TabStop = false;
			// 
			// lblSyncTransferStatus
			// 
			this.lblSyncTransferStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lblSyncTransferStatus.BackColor = System.Drawing.Color.Transparent;
			this.lblSyncTransferStatus.Location = new System.Drawing.Point(14, 153);
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
			this.lblSyncStatus.Location = new System.Drawing.Point(0, 125);
			this.lblSyncStatus.Name = "lblSyncStatus";
			this.lblSyncStatus.Size = new System.Drawing.Size(138, 28);
			this.lblSyncStatus.TabIndex = 1;
			this.lblSyncStatus.Text = "[Sync Status]";
			this.lblSyncStatus.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// usageDetailControl1
			// 
			this.usageDetailControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.usageDetailControl1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.usageDetailControl1.CloudTotalUsage = "[Cloud Total Usage]";
			this.usageDetailControl1.LocalDocument = "---";
			this.usageDetailControl1.LocalPhoto = "---";
			this.usageDetailControl1.Location = new System.Drawing.Point(4, 6);
			this.usageDetailControl1.Name = "usageDetailControl1";
			this.usageDetailControl1.ResourcePath = "[Resource Folder]";
			this.usageDetailControl1.Size = new System.Drawing.Size(495, 202);
			this.usageDetailControl1.TabIndex = 1;
			this.usageDetailControl1.TotalDocument = ((long)(0));
			this.usageDetailControl1.TotalPhoto = ((long)(0));
			this.usageDetailControl1.TotalWeb = ((long)(0));
			this.usageDetailControl1.ChangeResourcePathButtonClick += new System.EventHandler(this.usageDetailControl1_ChangeResourcePathButtonClick);
			// 
			// personalCloudStatusControl21
			// 
			this.personalCloudStatusControl21.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.personalCloudStatusControl21.Dock = System.Windows.Forms.DockStyle.Fill;
			this.personalCloudStatusControl21.EnableAutoRefreshStatus = false;
			this.personalCloudStatusControl21.Location = new System.Drawing.Point(3, 3);
			this.personalCloudStatusControl21.Name = "personalCloudStatusControl21";
			this.personalCloudStatusControl21.Size = new System.Drawing.Size(499, 293);
			this.personalCloudStatusControl21.TabIndex = 0;
			// 
			// planBox1
			// 
			this.planBox1.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.planBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.planBox1.Location = new System.Drawing.Point(3, 18);
			this.planBox1.Name = "planBox1";
			this.planBox1.Size = new System.Drawing.Size(493, 133);
			this.planBox1.TabIndex = 19;
			this.planBox1.Text = "planBox1";
			this.planBox1.Type = Waveface.Stream.WindowsClient.PlanBox.PlanType.Free;
			// 
			// serviceImportControl1
			// 
			this.serviceImportControl1.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.serviceImportControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.serviceImportControl1.Location = new System.Drawing.Point(3, 3);
			this.serviceImportControl1.Name = "serviceImportControl1";
			this.serviceImportControl1.Size = new System.Drawing.Size(499, 293);
			this.serviceImportControl1.TabIndex = 0;
			// 
			// PreferencesDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(679, 375);
			this.Controls.Add(this.systemIconControl1);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.linkLabel1);
			this.Controls.Add(this.splitContainer1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PreferencesDialog";
			this.Text = "AOStream Preferences";
			this.Load += new System.EventHandler(this.PreferencesDialog_Load);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
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
			this.panelEx1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pbxLogo)).EndInit();
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
		private System.Windows.Forms.Label lblEmail;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TabPage tabDevices;
		private System.Windows.Forms.TabPage tabGeneral;
		private System.Windows.Forms.Label lblName;
		private ServiceImportControl serviceImportControl1;
		private PersonalCloudStatusControl2 personalCloudStatusControl21;
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
		private System.Windows.Forms.LinkLabel lnklblEditName;
		private System.Windows.Forms.LinkLabel lnklblEditEmail;
		private System.Windows.Forms.TextBox tbxEmail;
		private System.Windows.Forms.TextBox tbxName;
		private System.Windows.Forms.LinkLabel lnklblCancelName;
		private System.Windows.Forms.LinkLabel lnklblCancelEmail;
		private System.Windows.Forms.LinkLabel lnklblSaveName;
		private System.Windows.Forms.LinkLabel lnklblSaveEmail;
		private System.Windows.Forms.CheckBox checkBox2;
		private System.Windows.Forms.Label lblDeviceConnectStatus;
		private System.Windows.Forms.ComboBox cmbDevice;
		private System.Windows.Forms.Label lblDeviceName;
		private System.Windows.Forms.Label label2;
	}
}