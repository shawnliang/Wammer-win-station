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
			this.panel2 = new System.Windows.Forms.Panel();
			this.button2 = new System.Windows.Forms.Button();
			this.btnUnLink = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.tbxEmail = new System.Windows.Forms.TextBox();
			this.tbxName = new System.Windows.Forms.TextBox();
			this.tabConnections = new System.Windows.Forms.TabPage();
			this.tabAbout = new System.Windows.Forms.TabPage();
			this.linkLabel1 = new System.Windows.Forms.LinkLabel();
			this.btnOK = new System.Windows.Forms.Button();
			this.refreshStatusTimer = new System.Windows.Forms.Timer(this.components);
			this.btnCancel = new System.Windows.Forms.Button();
			this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
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
			this.aboutControl1 = new Waveface.Stream.WindowsClient.Src.Control.AboutControl();
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
			this.tabAbout.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
			this.panelEx1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pbxLogo)).BeginInit();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			resources.ApplyResources(this.splitContainer1, "splitContainer1");
			this.errorProvider1.SetError(this.splitContainer1, resources.GetString("splitContainer1.Error"));
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.errorProvider1.SetIconAlignment(this.splitContainer1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("splitContainer1.IconAlignment"))));
			this.errorProvider1.SetIconPadding(this.splitContainer1, ((int)(resources.GetObject("splitContainer1.IconPadding"))));
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			resources.ApplyResources(this.splitContainer1.Panel1, "splitContainer1.Panel1");
			this.splitContainer1.Panel1.Controls.Add(this.panelEx1);
			this.errorProvider1.SetError(this.splitContainer1.Panel1, resources.GetString("splitContainer1.Panel1.Error"));
			this.errorProvider1.SetIconAlignment(this.splitContainer1.Panel1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("splitContainer1.Panel1.IconAlignment"))));
			this.errorProvider1.SetIconPadding(this.splitContainer1.Panel1, ((int)(resources.GetObject("splitContainer1.Panel1.IconPadding"))));
			// 
			// splitContainer1.Panel2
			// 
			resources.ApplyResources(this.splitContainer1.Panel2, "splitContainer1.Panel2");
			this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
			this.errorProvider1.SetError(this.splitContainer1.Panel2, resources.GetString("splitContainer1.Panel2.Error"));
			this.errorProvider1.SetIconAlignment(this.splitContainer1.Panel2, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("splitContainer1.Panel2.IconAlignment"))));
			this.errorProvider1.SetIconPadding(this.splitContainer1.Panel2, ((int)(resources.GetObject("splitContainer1.Panel2.IconPadding"))));
			// 
			// tabControl1
			// 
			resources.ApplyResources(this.tabControl1, "tabControl1");
			this.tabControl1.Controls.Add(this.tabGeneral);
			this.tabControl1.Controls.Add(this.tabDevices);
			this.tabControl1.Controls.Add(this.tabAccount);
			this.tabControl1.Controls.Add(this.tabConnections);
			this.tabControl1.Controls.Add(this.tabAbout);
			this.errorProvider1.SetError(this.tabControl1, resources.GetString("tabControl1.Error"));
			this.errorProvider1.SetIconAlignment(this.tabControl1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("tabControl1.IconAlignment"))));
			this.errorProvider1.SetIconPadding(this.tabControl1, ((int)(resources.GetObject("tabControl1.IconPadding"))));
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
			// 
			// tabGeneral
			// 
			resources.ApplyResources(this.tabGeneral, "tabGeneral");
			this.tabGeneral.Controls.Add(this.usageDetailControl1);
			this.tabGeneral.Controls.Add(this.groupBox3);
			this.errorProvider1.SetError(this.tabGeneral, resources.GetString("tabGeneral.Error"));
			this.errorProvider1.SetIconAlignment(this.tabGeneral, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("tabGeneral.IconAlignment"))));
			this.errorProvider1.SetIconPadding(this.tabGeneral, ((int)(resources.GetObject("tabGeneral.IconPadding"))));
			this.tabGeneral.Name = "tabGeneral";
			this.tabGeneral.UseVisualStyleBackColor = true;
			// 
			// groupBox3
			// 
			resources.ApplyResources(this.groupBox3, "groupBox3");
			this.groupBox3.Controls.Add(this.checkBox2);
			this.groupBox3.Controls.Add(this.checkBox1);
			this.errorProvider1.SetError(this.groupBox3, resources.GetString("groupBox3.Error"));
			this.errorProvider1.SetIconAlignment(this.groupBox3, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("groupBox3.IconAlignment"))));
			this.errorProvider1.SetIconPadding(this.groupBox3, ((int)(resources.GetObject("groupBox3.IconPadding"))));
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.TabStop = false;
			// 
			// checkBox2
			// 
			resources.ApplyResources(this.checkBox2, "checkBox2");
			this.errorProvider1.SetError(this.checkBox2, resources.GetString("checkBox2.Error"));
			this.errorProvider1.SetIconAlignment(this.checkBox2, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("checkBox2.IconAlignment"))));
			this.errorProvider1.SetIconPadding(this.checkBox2, ((int)(resources.GetObject("checkBox2.IconPadding"))));
			this.checkBox2.Name = "checkBox2";
			this.checkBox2.UseVisualStyleBackColor = true;
			// 
			// checkBox1
			// 
			resources.ApplyResources(this.checkBox1, "checkBox1");
			this.errorProvider1.SetError(this.checkBox1, resources.GetString("checkBox1.Error"));
			this.errorProvider1.SetIconAlignment(this.checkBox1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("checkBox1.IconAlignment"))));
			this.errorProvider1.SetIconPadding(this.checkBox1, ((int)(resources.GetObject("checkBox1.IconPadding"))));
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.UseVisualStyleBackColor = true;
			// 
			// tabDevices
			// 
			resources.ApplyResources(this.tabDevices, "tabDevices");
			this.tabDevices.Controls.Add(this.personalCloudStatusControl21);
			this.errorProvider1.SetError(this.tabDevices, resources.GetString("tabDevices.Error"));
			this.errorProvider1.SetIconAlignment(this.tabDevices, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("tabDevices.IconAlignment"))));
			this.errorProvider1.SetIconPadding(this.tabDevices, ((int)(resources.GetObject("tabDevices.IconPadding"))));
			this.tabDevices.Name = "tabDevices";
			this.tabDevices.UseVisualStyleBackColor = true;
			// 
			// tabAccount
			// 
			resources.ApplyResources(this.tabAccount, "tabAccount");
			this.tabAccount.Controls.Add(this.groupBox2);
			this.tabAccount.Controls.Add(this.groupBox1);
			this.errorProvider1.SetError(this.tabAccount, resources.GetString("tabAccount.Error"));
			this.errorProvider1.SetIconAlignment(this.tabAccount, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("tabAccount.IconAlignment"))));
			this.errorProvider1.SetIconPadding(this.tabAccount, ((int)(resources.GetObject("tabAccount.IconPadding"))));
			this.tabAccount.Name = "tabAccount";
			this.tabAccount.UseVisualStyleBackColor = true;
			// 
			// groupBox2
			// 
			resources.ApplyResources(this.groupBox2, "groupBox2");
			this.groupBox2.Controls.Add(this.planBox1);
			this.groupBox2.Controls.Add(this.panel1);
			this.errorProvider1.SetError(this.groupBox2, resources.GetString("groupBox2.Error"));
			this.errorProvider1.SetIconAlignment(this.groupBox2, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("groupBox2.IconAlignment"))));
			this.errorProvider1.SetIconPadding(this.groupBox2, ((int)(resources.GetObject("groupBox2.IconPadding"))));
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.TabStop = false;
			// 
			// panel1
			// 
			resources.ApplyResources(this.panel1, "panel1");
			this.panel1.Controls.Add(this.button3);
			this.errorProvider1.SetError(this.panel1, resources.GetString("panel1.Error"));
			this.errorProvider1.SetIconAlignment(this.panel1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("panel1.IconAlignment"))));
			this.errorProvider1.SetIconPadding(this.panel1, ((int)(resources.GetObject("panel1.IconPadding"))));
			this.panel1.Name = "panel1";
			// 
			// button3
			// 
			resources.ApplyResources(this.button3, "button3");
			this.errorProvider1.SetError(this.button3, resources.GetString("button3.Error"));
			this.errorProvider1.SetIconAlignment(this.button3, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("button3.IconAlignment"))));
			this.errorProvider1.SetIconPadding(this.button3, ((int)(resources.GetObject("button3.IconPadding"))));
			this.button3.Name = "button3";
			this.button3.UseVisualStyleBackColor = true;
			this.button3.Click += new System.EventHandler(this.button3_Click);
			// 
			// groupBox1
			// 
			resources.ApplyResources(this.groupBox1, "groupBox1");
			this.groupBox1.Controls.Add(this.panel2);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.tbxEmail);
			this.groupBox1.Controls.Add(this.tbxName);
			this.errorProvider1.SetError(this.groupBox1, resources.GetString("groupBox1.Error"));
			this.errorProvider1.SetIconAlignment(this.groupBox1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("groupBox1.IconAlignment"))));
			this.errorProvider1.SetIconPadding(this.groupBox1, ((int)(resources.GetObject("groupBox1.IconPadding"))));
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.TabStop = false;
			// 
			// panel2
			// 
			resources.ApplyResources(this.panel2, "panel2");
			this.panel2.Controls.Add(this.button2);
			this.panel2.Controls.Add(this.btnUnLink);
			this.errorProvider1.SetError(this.panel2, resources.GetString("panel2.Error"));
			this.errorProvider1.SetIconAlignment(this.panel2, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("panel2.IconAlignment"))));
			this.errorProvider1.SetIconPadding(this.panel2, ((int)(resources.GetObject("panel2.IconPadding"))));
			this.panel2.Name = "panel2";
			// 
			// button2
			// 
			resources.ApplyResources(this.button2, "button2");
			this.errorProvider1.SetError(this.button2, resources.GetString("button2.Error"));
			this.errorProvider1.SetIconAlignment(this.button2, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("button2.IconAlignment"))));
			this.errorProvider1.SetIconPadding(this.button2, ((int)(resources.GetObject("button2.IconPadding"))));
			this.button2.Name = "button2";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// btnUnLink
			// 
			resources.ApplyResources(this.btnUnLink, "btnUnLink");
			this.errorProvider1.SetError(this.btnUnLink, resources.GetString("btnUnLink.Error"));
			this.errorProvider1.SetIconAlignment(this.btnUnLink, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("btnUnLink.IconAlignment"))));
			this.errorProvider1.SetIconPadding(this.btnUnLink, ((int)(resources.GetObject("btnUnLink.IconPadding"))));
			this.btnUnLink.Name = "btnUnLink";
			this.btnUnLink.UseVisualStyleBackColor = true;
			this.btnUnLink.Click += new System.EventHandler(this.btnUnLink_Click);
			// 
			// label4
			// 
			resources.ApplyResources(this.label4, "label4");
			this.errorProvider1.SetError(this.label4, resources.GetString("label4.Error"));
			this.errorProvider1.SetIconAlignment(this.label4, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label4.IconAlignment"))));
			this.errorProvider1.SetIconPadding(this.label4, ((int)(resources.GetObject("label4.IconPadding"))));
			this.label4.Name = "label4";
			// 
			// label5
			// 
			resources.ApplyResources(this.label5, "label5");
			this.errorProvider1.SetError(this.label5, resources.GetString("label5.Error"));
			this.errorProvider1.SetIconAlignment(this.label5, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label5.IconAlignment"))));
			this.errorProvider1.SetIconPadding(this.label5, ((int)(resources.GetObject("label5.IconPadding"))));
			this.label5.Name = "label5";
			// 
			// tbxEmail
			// 
			resources.ApplyResources(this.tbxEmail, "tbxEmail");
			this.errorProvider1.SetError(this.tbxEmail, resources.GetString("tbxEmail.Error"));
			this.errorProvider1.SetIconAlignment(this.tbxEmail, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("tbxEmail.IconAlignment"))));
			this.errorProvider1.SetIconPadding(this.tbxEmail, ((int)(resources.GetObject("tbxEmail.IconPadding"))));
			this.tbxEmail.Name = "tbxEmail";
			this.tbxEmail.Leave += new System.EventHandler(this.tbxEmail_Leave);
			// 
			// tbxName
			// 
			resources.ApplyResources(this.tbxName, "tbxName");
			this.errorProvider1.SetError(this.tbxName, resources.GetString("tbxName.Error"));
			this.errorProvider1.SetIconAlignment(this.tbxName, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("tbxName.IconAlignment"))));
			this.errorProvider1.SetIconPadding(this.tbxName, ((int)(resources.GetObject("tbxName.IconPadding"))));
			this.tbxName.Name = "tbxName";
			this.tbxName.Leave += new System.EventHandler(this.tbxName_Leave);
			// 
			// tabConnections
			// 
			resources.ApplyResources(this.tabConnections, "tabConnections");
			this.tabConnections.Controls.Add(this.serviceImportControl1);
			this.errorProvider1.SetError(this.tabConnections, resources.GetString("tabConnections.Error"));
			this.errorProvider1.SetIconAlignment(this.tabConnections, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("tabConnections.IconAlignment"))));
			this.errorProvider1.SetIconPadding(this.tabConnections, ((int)(resources.GetObject("tabConnections.IconPadding"))));
			this.tabConnections.Name = "tabConnections";
			this.tabConnections.UseVisualStyleBackColor = true;
			// 
			// tabAbout
			// 
			resources.ApplyResources(this.tabAbout, "tabAbout");
			this.tabAbout.Controls.Add(this.aboutControl1);
			this.errorProvider1.SetError(this.tabAbout, resources.GetString("tabAbout.Error"));
			this.errorProvider1.SetIconAlignment(this.tabAbout, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("tabAbout.IconAlignment"))));
			this.errorProvider1.SetIconPadding(this.tabAbout, ((int)(resources.GetObject("tabAbout.IconPadding"))));
			this.tabAbout.Name = "tabAbout";
			this.tabAbout.UseVisualStyleBackColor = true;
			// 
			// linkLabel1
			// 
			resources.ApplyResources(this.linkLabel1, "linkLabel1");
			this.errorProvider1.SetError(this.linkLabel1, resources.GetString("linkLabel1.Error"));
			this.errorProvider1.SetIconAlignment(this.linkLabel1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("linkLabel1.IconAlignment"))));
			this.errorProvider1.SetIconPadding(this.linkLabel1, ((int)(resources.GetObject("linkLabel1.IconPadding"))));
			this.linkLabel1.Name = "linkLabel1";
			this.linkLabel1.TabStop = true;
			// 
			// btnOK
			// 
			resources.ApplyResources(this.btnOK, "btnOK");
			this.errorProvider1.SetError(this.btnOK, resources.GetString("btnOK.Error"));
			this.errorProvider1.SetIconAlignment(this.btnOK, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("btnOK.IconAlignment"))));
			this.errorProvider1.SetIconPadding(this.btnOK, ((int)(resources.GetObject("btnOK.IconPadding"))));
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
			this.errorProvider1.SetError(this.btnCancel, resources.GetString("btnCancel.Error"));
			this.errorProvider1.SetIconAlignment(this.btnCancel, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("btnCancel.IconAlignment"))));
			this.errorProvider1.SetIconPadding(this.btnCancel, ((int)(resources.GetObject("btnCancel.IconPadding"))));
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// errorProvider1
			// 
			this.errorProvider1.ContainerControl = this;
			resources.ApplyResources(this.errorProvider1, "errorProvider1");
			// 
			// systemIconControl1
			// 
			resources.ApplyResources(this.systemIconControl1, "systemIconControl1");
			this.errorProvider1.SetError(this.systemIconControl1, resources.GetString("systemIconControl1.Error"));
			this.errorProvider1.SetIconAlignment(this.systemIconControl1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("systemIconControl1.IconAlignment"))));
			this.errorProvider1.SetIconPadding(this.systemIconControl1, ((int)(resources.GetObject("systemIconControl1.IconPadding"))));
			this.systemIconControl1.IconType = Waveface.Stream.WindowsClient.SystemIconControl.SystemIconType.Question;
			this.systemIconControl1.Name = "systemIconControl1";
			// 
			// panelEx1
			// 
			resources.ApplyResources(this.panelEx1, "panelEx1");
			this.panelEx1.Controls.Add(this.lblDeviceName);
			this.panelEx1.Controls.Add(this.label2);
			this.panelEx1.Controls.Add(this.lblDeviceConnectStatus);
			this.panelEx1.Controls.Add(this.cmbDevice);
			this.panelEx1.Controls.Add(this.label1);
			this.panelEx1.Controls.Add(this.pbxLogo);
			this.panelEx1.Controls.Add(this.lblSyncTransferStatus);
			this.panelEx1.Controls.Add(this.lblSyncStatus);
			this.panelEx1.EnableLinearGradientBackground = true;
			this.errorProvider1.SetError(this.panelEx1, resources.GetString("panelEx1.Error"));
			this.errorProvider1.SetIconAlignment(this.panelEx1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("panelEx1.IconAlignment"))));
			this.errorProvider1.SetIconPadding(this.panelEx1, ((int)(resources.GetObject("panelEx1.IconPadding"))));
			this.panelEx1.LinearGradientEndColor = System.Drawing.Color.White;
			this.panelEx1.LinearGradientStartColor = System.Drawing.Color.SlateGray;
			this.panelEx1.Name = "panelEx1";
			// 
			// lblDeviceName
			// 
			resources.ApplyResources(this.lblDeviceName, "lblDeviceName");
			this.lblDeviceName.BackColor = System.Drawing.Color.Transparent;
			this.errorProvider1.SetError(this.lblDeviceName, resources.GetString("lblDeviceName.Error"));
			this.errorProvider1.SetIconAlignment(this.lblDeviceName, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("lblDeviceName.IconAlignment"))));
			this.errorProvider1.SetIconPadding(this.lblDeviceName, ((int)(resources.GetObject("lblDeviceName.IconPadding"))));
			this.lblDeviceName.Name = "lblDeviceName";
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.BackColor = System.Drawing.Color.Transparent;
			this.errorProvider1.SetError(this.label2, resources.GetString("label2.Error"));
			this.errorProvider1.SetIconAlignment(this.label2, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label2.IconAlignment"))));
			this.errorProvider1.SetIconPadding(this.label2, ((int)(resources.GetObject("label2.IconPadding"))));
			this.label2.Name = "label2";
			// 
			// lblDeviceConnectStatus
			// 
			resources.ApplyResources(this.lblDeviceConnectStatus, "lblDeviceConnectStatus");
			this.lblDeviceConnectStatus.BackColor = System.Drawing.Color.Transparent;
			this.errorProvider1.SetError(this.lblDeviceConnectStatus, resources.GetString("lblDeviceConnectStatus.Error"));
			this.errorProvider1.SetIconAlignment(this.lblDeviceConnectStatus, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("lblDeviceConnectStatus.IconAlignment"))));
			this.errorProvider1.SetIconPadding(this.lblDeviceConnectStatus, ((int)(resources.GetObject("lblDeviceConnectStatus.IconPadding"))));
			this.lblDeviceConnectStatus.Name = "lblDeviceConnectStatus";
			// 
			// cmbDevice
			// 
			resources.ApplyResources(this.cmbDevice, "cmbDevice");
			this.cmbDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.errorProvider1.SetError(this.cmbDevice, resources.GetString("cmbDevice.Error"));
			this.cmbDevice.FormattingEnabled = true;
			this.errorProvider1.SetIconAlignment(this.cmbDevice, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("cmbDevice.IconAlignment"))));
			this.errorProvider1.SetIconPadding(this.cmbDevice, ((int)(resources.GetObject("cmbDevice.IconPadding"))));
			this.cmbDevice.Name = "cmbDevice";
			this.cmbDevice.TextChanged += new System.EventHandler(this.cmbDevice_TextChanged);
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.BackColor = System.Drawing.Color.Transparent;
			this.errorProvider1.SetError(this.label1, resources.GetString("label1.Error"));
			this.errorProvider1.SetIconAlignment(this.label1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label1.IconAlignment"))));
			this.errorProvider1.SetIconPadding(this.label1, ((int)(resources.GetObject("label1.IconPadding"))));
			this.label1.Name = "label1";
			// 
			// pbxLogo
			// 
			resources.ApplyResources(this.pbxLogo, "pbxLogo");
			this.pbxLogo.BackColor = System.Drawing.Color.Transparent;
			this.errorProvider1.SetError(this.pbxLogo, resources.GetString("pbxLogo.Error"));
			this.errorProvider1.SetIconAlignment(this.pbxLogo, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("pbxLogo.IconAlignment"))));
			this.errorProvider1.SetIconPadding(this.pbxLogo, ((int)(resources.GetObject("pbxLogo.IconPadding"))));
			this.pbxLogo.Name = "pbxLogo";
			this.pbxLogo.TabStop = false;
			// 
			// lblSyncTransferStatus
			// 
			resources.ApplyResources(this.lblSyncTransferStatus, "lblSyncTransferStatus");
			this.lblSyncTransferStatus.BackColor = System.Drawing.Color.Transparent;
			this.errorProvider1.SetError(this.lblSyncTransferStatus, resources.GetString("lblSyncTransferStatus.Error"));
			this.errorProvider1.SetIconAlignment(this.lblSyncTransferStatus, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("lblSyncTransferStatus.IconAlignment"))));
			this.errorProvider1.SetIconPadding(this.lblSyncTransferStatus, ((int)(resources.GetObject("lblSyncTransferStatus.IconPadding"))));
			this.lblSyncTransferStatus.Name = "lblSyncTransferStatus";
			// 
			// lblSyncStatus
			// 
			resources.ApplyResources(this.lblSyncStatus, "lblSyncStatus");
			this.lblSyncStatus.BackColor = System.Drawing.Color.Transparent;
			this.errorProvider1.SetError(this.lblSyncStatus, resources.GetString("lblSyncStatus.Error"));
			this.errorProvider1.SetIconAlignment(this.lblSyncStatus, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("lblSyncStatus.IconAlignment"))));
			this.errorProvider1.SetIconPadding(this.lblSyncStatus, ((int)(resources.GetObject("lblSyncStatus.IconPadding"))));
			this.lblSyncStatus.Name = "lblSyncStatus";
			// 
			// usageDetailControl1
			// 
			resources.ApplyResources(this.usageDetailControl1, "usageDetailControl1");
			this.usageDetailControl1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.usageDetailControl1.CloudTotalUsage = "[Cloud Total Usage]";
			this.errorProvider1.SetError(this.usageDetailControl1, resources.GetString("usageDetailControl1.Error"));
			this.errorProvider1.SetIconAlignment(this.usageDetailControl1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("usageDetailControl1.IconAlignment"))));
			this.errorProvider1.SetIconPadding(this.usageDetailControl1, ((int)(resources.GetObject("usageDetailControl1.IconPadding"))));
			this.usageDetailControl1.LocalDocument = "---";
			this.usageDetailControl1.LocalPhoto = "---";
			this.usageDetailControl1.Name = "usageDetailControl1";
			this.usageDetailControl1.ResourcePath = "[Resource Folder]";
			this.usageDetailControl1.TotalDocument = ((long)(0));
			this.usageDetailControl1.TotalPhoto = ((long)(0));
			this.usageDetailControl1.TotalWeb = ((long)(0));
			this.usageDetailControl1.ChangeResourcePathButtonClick += new System.EventHandler(this.usageDetailControl1_ChangeResourcePathButtonClick);
			// 
			// personalCloudStatusControl21
			// 
			resources.ApplyResources(this.personalCloudStatusControl21, "personalCloudStatusControl21");
			this.personalCloudStatusControl21.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.personalCloudStatusControl21.EnableAutoRefreshStatus = false;
			this.errorProvider1.SetError(this.personalCloudStatusControl21, resources.GetString("personalCloudStatusControl21.Error"));
			this.errorProvider1.SetIconAlignment(this.personalCloudStatusControl21, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("personalCloudStatusControl21.IconAlignment"))));
			this.errorProvider1.SetIconPadding(this.personalCloudStatusControl21, ((int)(resources.GetObject("personalCloudStatusControl21.IconPadding"))));
			this.personalCloudStatusControl21.Name = "personalCloudStatusControl21";
			this.personalCloudStatusControl21.RefreshInterval = 15000;
			// 
			// planBox1
			// 
			resources.ApplyResources(this.planBox1, "planBox1");
			this.planBox1.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.errorProvider1.SetError(this.planBox1, resources.GetString("planBox1.Error"));
			this.planBox1.HeaderVisibile = false;
			this.errorProvider1.SetIconAlignment(this.planBox1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("planBox1.IconAlignment"))));
			this.errorProvider1.SetIconPadding(this.planBox1, ((int)(resources.GetObject("planBox1.IconPadding"))));
			this.planBox1.Name = "planBox1";
			this.planBox1.Type = Waveface.Stream.WindowsClient.PlanBox.PlanType.Free;
			// 
			// serviceImportControl1
			// 
			resources.ApplyResources(this.serviceImportControl1, "serviceImportControl1");
			this.serviceImportControl1.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.errorProvider1.SetError(this.serviceImportControl1, resources.GetString("serviceImportControl1.Error"));
			this.errorProvider1.SetIconAlignment(this.serviceImportControl1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("serviceImportControl1.IconAlignment"))));
			this.errorProvider1.SetIconPadding(this.serviceImportControl1, ((int)(resources.GetObject("serviceImportControl1.IconPadding"))));
			this.serviceImportControl1.Name = "serviceImportControl1";
			// 
			// aboutControl1
			// 
			resources.ApplyResources(this.aboutControl1, "aboutControl1");
			this.errorProvider1.SetError(this.aboutControl1, resources.GetString("aboutControl1.Error"));
			this.errorProvider1.SetIconAlignment(this.aboutControl1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("aboutControl1.IconAlignment"))));
			this.errorProvider1.SetIconPadding(this.aboutControl1, ((int)(resources.GetObject("aboutControl1.IconPadding"))));
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
			this.tabAbout.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
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
		private System.Windows.Forms.Label lblDeviceName;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TabPage tabAbout;
		private Src.Control.AboutControl aboutControl1;
		private System.Windows.Forms.Button btnCancel;
		private PersonalCloudStatusControl2 personalCloudStatusControl21;
		private System.Windows.Forms.ErrorProvider errorProvider1;
	}
}