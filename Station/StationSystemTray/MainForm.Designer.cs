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
			this.tabSecondStationSetup = new System.Windows.Forms.TabPage();
			this.btnOK2 = new System.Windows.Forms.Button();
			this.lblSecondStationSetup = new System.Windows.Forms.Label();
			this.lblWelcome2 = new System.Windows.Forms.Label();
			this.TrayMenu.SuspendLayout();
			this.tabControl.SuspendLayout();
			this.tabSignIn.SuspendLayout();
			this.tabMainStationSetup.SuspendLayout();
			this.tabSecondStationSetup.SuspendLayout();
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
            this.menuServiceAction,
            this.toolStripSeparator1,
            this.tsmiOpenStream,
            this.menuSignIn,
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
			this.tabSignIn.BackColor = System.Drawing.Color.AliceBlue;
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
			this.tabSignIn.Name = "tabSignIn";
			this.tabSignIn.Click += new System.EventHandler(this.tabSignIn_Click);
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
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
			resources.ApplyResources(this.cmbEmail, "cmbEmail");
			this.cmbEmail.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.cmbEmail.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
			this.cmbEmail.FormattingEnabled = true;
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
			resources.ApplyResources(this.tabMainStationSetup, "tabMainStationSetup");
			this.tabMainStationSetup.BackColor = System.Drawing.Color.AliceBlue;
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
			this.tabSecondStationSetup.BackColor = System.Drawing.Color.AliceBlue;
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
			this.AcceptButton = this.btnSignIn;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
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
		private System.Windows.Forms.TabPage tabSecondStationSetup;
		private System.Windows.Forms.Label lblWelcome2;
		private System.Windows.Forms.Label lblSecondStationSetup;
		private System.Windows.Forms.Button btnOK2;
		private FBLoginButton fbLoginButton1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
	}
}

