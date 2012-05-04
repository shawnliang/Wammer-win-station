using StationSystemTray.Control;
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
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.menuQuit = new System.Windows.Forms.ToolStripMenuItem();
			this.checkStationTimer = new System.Windows.Forms.Timer(this.components);
			this.btnSignIn = new System.Windows.Forms.Button();
			this.tabControl = new StationSystemTray.Control.TabControlEx();
			this.tabSignIn = new System.Windows.Forms.TabPage();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.fbLoginButton1 = new StationSystemTray.Control.FBLoginButton();
			this.lblSignIn = new System.Windows.Forms.Label();
			this.lblEmail = new System.Windows.Forms.Label();
			this.lblSignUp = new System.Windows.Forms.LinkLabel();
			this.txtPassword = new System.Windows.Forms.TextBox();
			this.cmbEmail = new System.Windows.Forms.ComboBox();
			this.lblPassword = new System.Windows.Forms.Label();
			this.chkRememberPassword = new System.Windows.Forms.CheckBox();
			this.tabMainStationSetup = new System.Windows.Forms.TabPage();
			this.btnOK = new System.Windows.Forms.Button();
			this.lblWelcome = new System.Windows.Forms.Label();
			this.lblMainStationSetup = new System.Windows.Forms.Label();
			this.TrayMenu.SuspendLayout();
			this.tabControl.SuspendLayout();
			this.tabSignIn.SuspendLayout();
			this.tabMainStationSetup.SuspendLayout();
			this.SuspendLayout();
			// 
			// TrayIcon
			// 
			this.TrayIcon.ContextMenuStrip = this.TrayMenu;
			resources.ApplyResources(this.TrayIcon, "TrayIcon");
			this.TrayIcon.DoubleClick += new System.EventHandler(this.menuPreference_Click);
			// 
			// TrayMenu
			// 
			this.TrayMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuServiceAction,
            this.toolStripSeparator1,
            this.tsmiOpenStream,
            this.menuSignIn,
            this.toolStripSeparator3,
            this.menuQuit});
			this.TrayMenu.Name = "TrayMenu";
			resources.ApplyResources(this.TrayMenu, "TrayMenu");
			this.TrayMenu.Opening += new System.ComponentModel.CancelEventHandler(this.TrayMenu_Opening);
			this.TrayMenu.VisibleChanged += new System.EventHandler(this.TrayMenu_VisibleChanged);
			// 
			// menuServiceAction
			// 
			this.menuServiceAction.Name = "menuServiceAction";
			resources.ApplyResources(this.menuServiceAction, "menuServiceAction");
			this.menuServiceAction.Click += new System.EventHandler(this.menuServiceAction_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
			// 
			// tsmiOpenStream
			// 
			this.tsmiOpenStream.Name = "tsmiOpenStream";
			resources.ApplyResources(this.tsmiOpenStream, "tsmiOpenStream");
			this.tsmiOpenStream.Click += new System.EventHandler(this.tsmiOpenStream_Click);
			// 
			// menuSignIn
			// 
			this.menuSignIn.Name = "menuSignIn";
			resources.ApplyResources(this.menuSignIn, "menuSignIn");
			this.menuSignIn.Click += new System.EventHandler(this.menuSignIn_Click);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
			// 
			// menuQuit
			// 
			this.menuQuit.Name = "menuQuit";
			resources.ApplyResources(this.menuQuit, "menuQuit");
			this.menuQuit.Click += new System.EventHandler(this.menuQuit_Click);
			// 
			// checkStationTimer
			// 
			this.checkStationTimer.Interval = 3000;
			this.checkStationTimer.Tick += new System.EventHandler(this.checkStationTimer_Tick);
			// 
			// btnSignIn
			// 
			resources.ApplyResources(this.btnSignIn, "btnSignIn");
			this.btnSignIn.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.btnSignIn.Name = "btnSignIn";
			this.btnSignIn.UseVisualStyleBackColor = false;
			this.btnSignIn.Click += new System.EventHandler(this.btnSignIn_Click);
			// 
			// tabControl
			// 
			this.tabControl.Controls.Add(this.tabSignIn);
			this.tabControl.Controls.Add(this.tabMainStationSetup);
			resources.ApplyResources(this.tabControl, "tabControl");
			this.tabControl.HideTabs = true;
			this.tabControl.Multiline = true;
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.TabStop = false;
			// 
			// tabSignIn
			// 
			this.tabSignIn.BackColor = System.Drawing.SystemColors.Control;
			this.tabSignIn.Controls.Add(this.label2);
			this.tabSignIn.Controls.Add(this.label1);
			this.tabSignIn.Controls.Add(this.fbLoginButton1);
			this.tabSignIn.Controls.Add(this.lblSignIn);
			this.tabSignIn.Controls.Add(this.lblEmail);
			this.tabSignIn.Controls.Add(this.btnSignIn);
			this.tabSignIn.Controls.Add(this.lblSignUp);
			this.tabSignIn.Controls.Add(this.txtPassword);
			this.tabSignIn.Controls.Add(this.cmbEmail);
			this.tabSignIn.Controls.Add(this.lblPassword);
			this.tabSignIn.Controls.Add(this.chkRememberPassword);
			resources.ApplyResources(this.tabSignIn, "tabSignIn");
			this.tabSignIn.Name = "tabSignIn";
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			// 
			// label1
			// 
			this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// fbLoginButton1
			// 
			resources.ApplyResources(this.fbLoginButton1, "fbLoginButton1");
			this.fbLoginButton1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(120)))), ((int)(((byte)(171)))));
			this.fbLoginButton1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.fbLoginButton1.Name = "fbLoginButton1";
			this.fbLoginButton1.Click += new System.EventHandler(this.fbLoginButton1_Click);
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
			// lblSignUp
			// 
			resources.ApplyResources(this.lblSignUp, "lblSignUp");
			this.lblSignUp.Name = "lblSignUp";
			this.lblSignUp.TabStop = true;
			this.lblSignUp.UseCompatibleTextRendering = true;
			this.lblSignUp.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblSignUp_LinkClicked);
			// 
			// txtPassword
			// 
			resources.ApplyResources(this.txtPassword, "txtPassword");
			this.txtPassword.Name = "txtPassword";
			this.txtPassword.UseSystemPasswordChar = true;
			// 
			// cmbEmail
			// 
			this.cmbEmail.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.cmbEmail.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
			this.cmbEmail.FormattingEnabled = true;
			resources.ApplyResources(this.cmbEmail, "cmbEmail");
			this.cmbEmail.Name = "cmbEmail";
			this.cmbEmail.TextChanged += new System.EventHandler(this.cmbEmail_TextChanged);
			// 
			// lblPassword
			// 
			resources.ApplyResources(this.lblPassword, "lblPassword");
			this.lblPassword.Name = "lblPassword";
			// 
			// chkRememberPassword
			// 
			resources.ApplyResources(this.chkRememberPassword, "chkRememberPassword");
			this.chkRememberPassword.Name = "chkRememberPassword";
			this.chkRememberPassword.UseVisualStyleBackColor = true;
			// 
			// tabMainStationSetup
			// 
			this.tabMainStationSetup.BackColor = System.Drawing.SystemColors.Control;
			this.tabMainStationSetup.Controls.Add(this.btnOK);
			this.tabMainStationSetup.Controls.Add(this.lblWelcome);
			this.tabMainStationSetup.Controls.Add(this.lblMainStationSetup);
			resources.ApplyResources(this.tabMainStationSetup, "tabMainStationSetup");
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
			// MainForm
			// 
			this.AcceptButton = this.btnSignIn;
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.tabControl);
			this.DoubleBuffered = true;
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
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.NotifyIcon TrayIcon;
		private System.Windows.Forms.ContextMenuStrip TrayMenu;
		private System.Windows.Forms.ToolStripMenuItem menuServiceAction;
		private System.Windows.Forms.ToolStripMenuItem menuQuit;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.Timer checkStationTimer;
		private System.Windows.Forms.Label lblSignIn;
		private System.Windows.Forms.Label lblEmail;
		private System.Windows.Forms.Label lblPassword;
		private System.Windows.Forms.Button btnSignIn;
		private System.Windows.Forms.CheckBox chkRememberPassword;
		private System.Windows.Forms.LinkLabel lblSignUp;
        private TabControlEx tabControl;
		private System.Windows.Forms.TabPage tabSignIn;
		private System.Windows.Forms.TabPage tabMainStationSetup;
		private System.Windows.Forms.Label lblMainStationSetup;
		private System.Windows.Forms.Label lblWelcome;
        private System.Windows.Forms.Button btnOK;
        internal System.Windows.Forms.TextBox txtPassword;
        internal System.Windows.Forms.ComboBox cmbEmail;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripMenuItem menuSignIn;
		private System.Windows.Forms.ToolStripMenuItem tsmiOpenStream;
		private FBLoginButton fbLoginButton1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
	}
}

