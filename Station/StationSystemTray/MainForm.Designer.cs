namespace StationSystemTray
{
	partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.TrayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.TrayMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuRelogin = new System.Windows.Forms.ToolStripMenuItem();
            this.menuServiceAction = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuSignInOut = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.menuQuit = new System.Windows.Forms.ToolStripMenuItem();
            this.checkStationTimer = new System.Windows.Forms.Timer(this.components);
            this.lblSignIn = new System.Windows.Forms.Label();
            this.lblEmail = new System.Windows.Forms.Label();
            this.lblPassword = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.cmbEmail = new System.Windows.Forms.ComboBox();
            this.btnSignIn = new System.Windows.Forms.Button();
            this.chkRememberPassword = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblSignUp = new System.Windows.Forms.LinkLabel();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabSignIn = new System.Windows.Forms.TabPage();
            this.tabMainStationSetup = new System.Windows.Forms.TabPage();
            this.btnOK = new System.Windows.Forms.Button();
            this.lblWelcome = new System.Windows.Forms.Label();
            this.lblMainStationSetup = new System.Windows.Forms.Label();
            this.tabSignUp = new System.Windows.Forms.TabPage();
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.TrayMenu.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabSignIn.SuspendLayout();
            this.tabMainStationSetup.SuspendLayout();
            this.tabSignUp.SuspendLayout();
            this.SuspendLayout();
            // 
            // TrayIcon
            // 
            resources.ApplyResources(this.TrayIcon, "TrayIcon");
            this.TrayIcon.ContextMenuStrip = this.TrayMenu;
            this.TrayIcon.DoubleClick += new System.EventHandler(this.menuPreference_Click);
            // 
            // TrayMenu
            // 
            resources.ApplyResources(this.TrayMenu, "TrayMenu");
            this.TrayMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuRelogin,
            this.menuServiceAction,
            this.toolStripSeparator1,
            this.menuSignInOut,
            this.toolStripSeparator3,
            this.menuQuit});
            this.TrayMenu.Name = "TrayMenu";
            // 
            // menuRelogin
            // 
            resources.ApplyResources(this.menuRelogin, "menuRelogin");
            this.menuRelogin.Name = "menuRelogin";
            this.menuRelogin.Click += new System.EventHandler(this.menuRelogin_Click);
            // 
            // menuServiceAction
            // 
            resources.ApplyResources(this.menuServiceAction, "menuServiceAction");
            this.menuServiceAction.Name = "menuServiceAction";
            this.menuServiceAction.Click += new System.EventHandler(this.menuServiceAction_Click);
            // 
            // toolStripSeparator1
            // 
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            // 
            // menuSignInOut
            // 
            resources.ApplyResources(this.menuSignInOut, "menuSignInOut");
            this.menuSignInOut.Name = "menuSignInOut";
            this.menuSignInOut.Click += new System.EventHandler(this.menuSignIn_Click);
            // 
            // toolStripSeparator3
            // 
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            // 
            // menuQuit
            // 
            resources.ApplyResources(this.menuQuit, "menuQuit");
            this.menuQuit.Name = "menuQuit";
            this.menuQuit.Click += new System.EventHandler(this.menuQuit_Click);
            // 
            // checkStationTimer
            // 
            this.checkStationTimer.Interval = 3000;
            this.checkStationTimer.Tick += new System.EventHandler(this.checkStationTimer_Tick);
            // 
            // lblSignIn
            // 
            resources.ApplyResources(this.lblSignIn, "lblSignIn");
            this.lblSignIn.Name = "lblSignIn";
            // 
            // lblEmail
            // 
            resources.ApplyResources(this.lblEmail, "lblEmail");
            this.lblEmail.Name = "lblEmail";
            // 
            // lblPassword
            // 
            resources.ApplyResources(this.lblPassword, "lblPassword");
            this.lblPassword.Name = "lblPassword";
            // 
            // txtPassword
            // 
            resources.ApplyResources(this.txtPassword, "txtPassword");
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.UseSystemPasswordChar = true;
            // 
            // cmbEmail
            // 
            resources.ApplyResources(this.cmbEmail, "cmbEmail");
            this.cmbEmail.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.cmbEmail.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbEmail.FormattingEnabled = true;
            this.cmbEmail.Name = "cmbEmail";
            // 
            // btnSignIn
            // 
            resources.ApplyResources(this.btnSignIn, "btnSignIn");
            this.btnSignIn.Name = "btnSignIn";
            this.btnSignIn.UseVisualStyleBackColor = true;
            this.btnSignIn.Click += new System.EventHandler(this.btnSignIn_Click);
            // 
            // chkRememberPassword
            // 
            resources.ApplyResources(this.chkRememberPassword, "chkRememberPassword");
            this.chkRememberPassword.Name = "chkRememberPassword";
            this.chkRememberPassword.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // lblSignUp
            // 
            resources.ApplyResources(this.lblSignUp, "lblSignUp");
            this.lblSignUp.Name = "lblSignUp";
            this.lblSignUp.TabStop = true;
            this.lblSignUp.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblSignUp_LinkClicked);
            // 
            // tabControl
            // 
            resources.ApplyResources(this.tabControl, "tabControl");
            this.tabControl.Controls.Add(this.tabSignIn);
            this.tabControl.Controls.Add(this.tabMainStationSetup);
            this.tabControl.Controls.Add(this.tabSignUp);
            this.tabControl.Multiline = true;
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.TabStop = false;
            this.tabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl_SelectedIndexChanged);
            // 
            // tabSignIn
            // 
            resources.ApplyResources(this.tabSignIn, "tabSignIn");
            this.tabSignIn.BackColor = System.Drawing.SystemColors.Control;
            this.tabSignIn.Controls.Add(this.lblSignIn);
            this.tabSignIn.Controls.Add(this.lblEmail);
            this.tabSignIn.Controls.Add(this.btnSignIn);
            this.tabSignIn.Controls.Add(this.label1);
            this.tabSignIn.Controls.Add(this.lblSignUp);
            this.tabSignIn.Controls.Add(this.txtPassword);
            this.tabSignIn.Controls.Add(this.cmbEmail);
            this.tabSignIn.Controls.Add(this.lblPassword);
            this.tabSignIn.Controls.Add(this.chkRememberPassword);
            this.tabSignIn.Name = "tabSignIn";
            // 
            // tabMainStationSetup
            // 
            resources.ApplyResources(this.tabMainStationSetup, "tabMainStationSetup");
            this.tabMainStationSetup.BackColor = System.Drawing.SystemColors.Control;
            this.tabMainStationSetup.Controls.Add(this.btnOK);
            this.tabMainStationSetup.Controls.Add(this.lblWelcome);
            this.tabMainStationSetup.Controls.Add(this.lblMainStationSetup);
            this.tabMainStationSetup.Name = "tabMainStationSetup";
            // 
            // btnOK
            // 
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.Name = "btnOK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // lblWelcome
            // 
            resources.ApplyResources(this.lblWelcome, "lblWelcome");
            this.lblWelcome.Name = "lblWelcome";
            // 
            // lblMainStationSetup
            // 
            resources.ApplyResources(this.lblMainStationSetup, "lblMainStationSetup");
            this.lblMainStationSetup.Name = "lblMainStationSetup";
            // 
            // tabSignUp
            // 
            resources.ApplyResources(this.tabSignUp, "tabSignUp");
            this.tabSignUp.Controls.Add(this.webBrowser1);
            this.tabSignUp.Name = "tabSignUp";
            this.tabSignUp.UseVisualStyleBackColor = true;
            // 
            // webBrowser1
            // 
            resources.ApplyResources(this.webBrowser1, "webBrowser1");
            this.webBrowser1.Name = "webBrowser1";
            // 
            // MainForm
            // 
            this.AcceptButton = this.btnSignIn;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.tabControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.TrayMenu.ResumeLayout(false);
            this.tabControl.ResumeLayout(false);
            this.tabSignIn.ResumeLayout(false);
            this.tabSignIn.PerformLayout();
            this.tabMainStationSetup.ResumeLayout(false);
            this.tabMainStationSetup.PerformLayout();
            this.tabSignUp.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.NotifyIcon TrayIcon;
        private System.Windows.Forms.ContextMenuStrip TrayMenu;
		private System.Windows.Forms.ToolStripMenuItem menuServiceAction;
		private System.Windows.Forms.ToolStripMenuItem menuQuit;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.Timer checkStationTimer;
		private System.Windows.Forms.ToolStripMenuItem menuRelogin;
		private System.Windows.Forms.ToolStripMenuItem menuSignInOut;
		private System.Windows.Forms.Label lblSignIn;
		private System.Windows.Forms.Label lblEmail;
		private System.Windows.Forms.Label lblPassword;
		private System.Windows.Forms.Button btnSignIn;
		private System.Windows.Forms.CheckBox chkRememberPassword;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.LinkLabel lblSignUp;
		private System.Windows.Forms.TabControl tabControl;
		private System.Windows.Forms.TabPage tabSignIn;
		private System.Windows.Forms.TabPage tabMainStationSetup;
		private System.Windows.Forms.Label lblMainStationSetup;
		private System.Windows.Forms.Label lblWelcome;
		private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.TabPage tabSignUp;
        private System.Windows.Forms.WebBrowser webBrowser1;
        internal System.Windows.Forms.TextBox txtPassword;
        internal System.Windows.Forms.ComboBox cmbEmail;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
	}
}

