
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
			this.menuServiceAction = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.tsmiOpenStream = new System.Windows.Forms.ToolStripMenuItem();
			this.menuSignIn = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.settingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.menuQuit = new System.Windows.Forms.ToolStripMenuItem();
			this.checkStationTimer = new System.Windows.Forms.Timer(this.components);
			this.tabControl = new StationSystemTray.TabControlEx();
			this.tabSignIn = new System.Windows.Forms.TabPage();
			this.loginInputPanel1 = new StationSystemTray.LoginInputPanel();
			this.button1 = new System.Windows.Forms.Button();
			this.txtPassword = new StationSystemTray.CueTextBox();
			this.tbxEMail = new StationSystemTray.CueTextBox();
			this.cmbEmail = new System.Windows.Forms.ComboBox();
			this.fbLoginButton1 = new StationSystemTray.FBLoginButton();
			this.loginButton1 = new StationSystemTray.LoginButton();
			this.pictureBox2 = new System.Windows.Forms.PictureBox();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.label1 = new System.Windows.Forms.Label();
			this.lblSignUp = new System.Windows.Forms.LinkLabel();
			this.chkRememberPassword = new System.Windows.Forms.CheckBox();
			this.tabMainStationSetup = new System.Windows.Forms.TabPage();
			this.btnOK = new System.Windows.Forms.Button();
			this.lblWelcome = new System.Windows.Forms.Label();
			this.lblMainStationSetup = new System.Windows.Forms.Label();
			this.tabSecondStationSetup = new System.Windows.Forms.TabPage();
			this.btnOK2 = new System.Windows.Forms.Button();
			this.lblSecondStationSetup = new System.Windows.Forms.Label();
			this.lblWelcome2 = new System.Windows.Forms.Label();
			this.TrayMenu.SuspendLayout();
			this.tabControl.SuspendLayout();
			this.tabSignIn.SuspendLayout();
			this.loginInputPanel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.tabMainStationSetup.SuspendLayout();
			this.tabSecondStationSetup.SuspendLayout();
			this.SuspendLayout();
			// 
			// TrayIcon
			// 
			resources.ApplyResources(this.TrayIcon, "TrayIcon");
			this.TrayIcon.ContextMenuStrip = this.TrayMenu;
			this.TrayIcon.DoubleClick += new System.EventHandler(this.trayIcon_DoubleClicked);
			// 
			// TrayMenu
			// 
			resources.ApplyResources(this.TrayMenu, "TrayMenu");
			this.TrayMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuServiceAction,
            this.toolStripSeparator1,
            this.tsmiOpenStream,
            this.menuSignIn,
            this.toolStripMenuItem1,
            this.settingToolStripMenuItem,
            this.toolStripSeparator3,
            this.menuQuit});
			this.TrayMenu.Name = "TrayMenu";
			this.TrayMenu.Opening += new System.ComponentModel.CancelEventHandler(this.TrayMenu_Opening);
			this.TrayMenu.VisibleChanged += new System.EventHandler(this.TrayMenu_VisibleChanged);
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
			// tsmiOpenStream
			// 
			resources.ApplyResources(this.tsmiOpenStream, "tsmiOpenStream");
			this.tsmiOpenStream.Name = "tsmiOpenStream";
			this.tsmiOpenStream.Click += new System.EventHandler(this.tsmiOpenStream_Click);
			// 
			// menuSignIn
			// 
			resources.ApplyResources(this.menuSignIn, "menuSignIn");
			this.menuSignIn.Name = "menuSignIn";
			this.menuSignIn.Click += new System.EventHandler(this.menuSignIn_Click);
			// 
			// toolStripMenuItem1
			// 
			resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			// 
			// settingToolStripMenuItem
			// 
			resources.ApplyResources(this.settingToolStripMenuItem, "settingToolStripMenuItem");
			this.settingToolStripMenuItem.Name = "settingToolStripMenuItem";
			this.settingToolStripMenuItem.Click += new System.EventHandler(this.settingToolStripMenuItem_Click);
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
			// tabControl
			// 
			resources.ApplyResources(this.tabControl, "tabControl");
			this.tabControl.Controls.Add(this.tabSignIn);
			this.tabControl.Controls.Add(this.tabMainStationSetup);
			this.tabControl.Controls.Add(this.tabSecondStationSetup);
			this.tabControl.HideTabs = true;
			this.tabControl.Multiline = true;
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.TabStop = false;
			// 
			// tabSignIn
			// 
			resources.ApplyResources(this.tabSignIn, "tabSignIn");
			this.tabSignIn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(225)))), ((int)(((byte)(225)))));
			this.tabSignIn.Controls.Add(this.loginInputPanel1);
			this.tabSignIn.Controls.Add(this.fbLoginButton1);
			this.tabSignIn.Controls.Add(this.loginButton1);
			this.tabSignIn.Controls.Add(this.pictureBox2);
			this.tabSignIn.Controls.Add(this.pictureBox1);
			this.tabSignIn.Controls.Add(this.label1);
			this.tabSignIn.Controls.Add(this.lblSignUp);
			this.tabSignIn.Controls.Add(this.chkRememberPassword);
			this.tabSignIn.Name = "tabSignIn";
			this.tabSignIn.Click += new System.EventHandler(this.tabSignIn_Click);
			// 
			// loginInputPanel1
			// 
			resources.ApplyResources(this.loginInputPanel1, "loginInputPanel1");
			this.loginInputPanel1.Controls.Add(this.button1);
			this.loginInputPanel1.Controls.Add(this.txtPassword);
			this.loginInputPanel1.Controls.Add(this.tbxEMail);
			this.loginInputPanel1.Controls.Add(this.cmbEmail);
			this.loginInputPanel1.Name = "loginInputPanel1";
			// 
			// button1
			// 
			resources.ApplyResources(this.button1, "button1");
			this.button1.BackColor = System.Drawing.Color.White;
			this.button1.CausesValidation = false;
			this.button1.FlatAppearance.BorderSize = 0;
			this.button1.Name = "button1";
			this.button1.TabStop = false;
			this.button1.UseVisualStyleBackColor = false;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// txtPassword
			// 
			resources.ApplyResources(this.txtPassword, "txtPassword");
			this.txtPassword.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.txtPassword.Name = "txtPassword";
			// 
			// tbxEMail
			// 
			resources.ApplyResources(this.tbxEMail, "tbxEMail");
			this.tbxEMail.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.tbxEMail.Name = "tbxEMail";
			// 
			// cmbEmail
			// 
			resources.ApplyResources(this.cmbEmail, "cmbEmail");
			this.cmbEmail.FormattingEnabled = true;
			this.cmbEmail.Name = "cmbEmail";
			this.cmbEmail.Sorted = true;
			this.cmbEmail.TextChanged += new System.EventHandler(this.cmbEmail_TextChanged);
			// 
			// fbLoginButton1
			// 
			resources.ApplyResources(this.fbLoginButton1, "fbLoginButton1");
			this.fbLoginButton1.Name = "fbLoginButton1";
			this.fbLoginButton1.Click += new System.EventHandler(this.fbLoginButton1_Click);
			// 
			// loginButton1
			// 
			resources.ApplyResources(this.loginButton1, "loginButton1");
			this.loginButton1.Name = "loginButton1";
			this.loginButton1.Click += new System.EventHandler(this.btnSignIn_Click);
			// 
			// pictureBox2
			// 
			resources.ApplyResources(this.pictureBox2, "pictureBox2");
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.TabStop = false;
			// 
			// pictureBox1
			// 
			resources.ApplyResources(this.pictureBox1, "pictureBox1");
			this.pictureBox1.BackgroundImage = global::StationSystemTray.Properties.Resources.stream_logo;
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.TabStop = false;
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.label1.Name = "label1";
			this.label1.Paint += new System.Windows.Forms.PaintEventHandler(this.label1_Paint);
			// 
			// lblSignUp
			// 
			resources.ApplyResources(this.lblSignUp, "lblSignUp");
			this.lblSignUp.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(48)))), ((int)(((byte)(255)))));
			this.lblSignUp.Name = "lblSignUp";
			this.lblSignUp.TabStop = true;
			this.lblSignUp.UseCompatibleTextRendering = true;
			this.lblSignUp.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblSignUp_LinkClicked);
			// 
			// chkRememberPassword
			// 
			resources.ApplyResources(this.chkRememberPassword, "chkRememberPassword");
			this.chkRememberPassword.Name = "chkRememberPassword";
			this.chkRememberPassword.UseVisualStyleBackColor = true;
			// 
			// tabMainStationSetup
			// 
			resources.ApplyResources(this.tabMainStationSetup, "tabMainStationSetup");
			this.tabMainStationSetup.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(225)))), ((int)(((byte)(225)))));
			this.tabMainStationSetup.Controls.Add(this.btnOK);
			this.tabMainStationSetup.Controls.Add(this.lblWelcome);
			this.tabMainStationSetup.Controls.Add(this.lblMainStationSetup);
			this.tabMainStationSetup.Name = "tabMainStationSetup";
			// 
			// btnOK
			// 
			resources.ApplyResources(this.btnOK, "btnOK");
			this.btnOK.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.btnOK.Name = "btnOK";
			this.btnOK.UseVisualStyleBackColor = false;
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
			// tabSecondStationSetup
			// 
			resources.ApplyResources(this.tabSecondStationSetup, "tabSecondStationSetup");
			this.tabSecondStationSetup.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(225)))), ((int)(((byte)(225)))));
			this.tabSecondStationSetup.Controls.Add(this.btnOK2);
			this.tabSecondStationSetup.Controls.Add(this.lblSecondStationSetup);
			this.tabSecondStationSetup.Controls.Add(this.lblWelcome2);
			this.tabSecondStationSetup.Name = "tabSecondStationSetup";
			// 
			// btnOK2
			// 
			resources.ApplyResources(this.btnOK2, "btnOK2");
			this.btnOK2.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.btnOK2.Name = "btnOK2";
			this.btnOK2.UseVisualStyleBackColor = false;
			this.btnOK2.Click += new System.EventHandler(this.btnOK2_Click);
			// 
			// lblSecondStationSetup
			// 
			resources.ApplyResources(this.lblSecondStationSetup, "lblSecondStationSetup");
			this.lblSecondStationSetup.Name = "lblSecondStationSetup";
			// 
			// lblWelcome2
			// 
			resources.ApplyResources(this.lblWelcome2, "lblWelcome2");
			this.lblWelcome2.Name = "lblWelcome2";
			// 
			// MainForm
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.Controls.Add(this.tabControl);
			this.DoubleBuffered = true;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "MainForm";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.TrayMenu.ResumeLayout(false);
			this.tabControl.ResumeLayout(false);
			this.tabSignIn.ResumeLayout(false);
			this.tabSignIn.PerformLayout();
			this.loginInputPanel1.ResumeLayout(false);
			this.loginInputPanel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.tabMainStationSetup.ResumeLayout(false);
			this.tabMainStationSetup.PerformLayout();
			this.tabSecondStationSetup.ResumeLayout(false);
			this.tabSecondStationSetup.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.NotifyIcon TrayIcon;
		private System.Windows.Forms.ContextMenuStrip TrayMenu;
		private System.Windows.Forms.ToolStripMenuItem menuServiceAction;
		private System.Windows.Forms.ToolStripMenuItem menuQuit;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.Timer checkStationTimer;
		private System.Windows.Forms.CheckBox chkRememberPassword;
		private System.Windows.Forms.LinkLabel lblSignUp;
        private TabControlEx tabControl;
		private System.Windows.Forms.TabPage tabSignIn;
		private System.Windows.Forms.TabPage tabMainStationSetup;
		private System.Windows.Forms.Label lblMainStationSetup;
		private System.Windows.Forms.Label lblWelcome;
        private System.Windows.Forms.Button btnOK;
        internal CueTextBox txtPassword;
        internal System.Windows.Forms.ComboBox cmbEmail;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripMenuItem menuSignIn;
		private System.Windows.Forms.ToolStripMenuItem tsmiOpenStream;
		private System.Windows.Forms.TabPage tabSecondStationSetup;
		private System.Windows.Forms.Label lblWelcome2;
		private System.Windows.Forms.Label lblSecondStationSetup;
		private System.Windows.Forms.Button btnOK2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.PictureBox pictureBox2;
		private LoginButton loginButton1;
		private FBLoginButton fbLoginButton1;
		private LoginInputPanel loginInputPanel1;
		private System.Windows.Forms.Button button1;
		internal CueTextBox tbxEMail;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem settingToolStripMenuItem;
	}
}

