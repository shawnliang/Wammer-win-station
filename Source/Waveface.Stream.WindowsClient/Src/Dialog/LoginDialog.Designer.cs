
namespace Waveface.Stream.WindowsClient
{
	partial class LoginDialog
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginDialog));
			this.tabControl = new Waveface.Stream.WindowsClient.TabControlEx();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.button2 = new System.Windows.Forms.Button();
			this.button1 = new System.Windows.Forms.Button();
			this.pictureBox3 = new System.Windows.Forms.PictureBox();
			this.tabSignIn = new System.Windows.Forms.TabPage();
			this.loginInputBox1 = new Waveface.Stream.WindowsClient.LoginInputBox();
			this.devVersionTag = new System.Windows.Forms.Label();
			this.pictureBox2 = new System.Windows.Forms.PictureBox();
			this.linkLabel1 = new System.Windows.Forms.LinkLabel();
			this.fbLoginButton1 = new Waveface.Stream.WindowsClient.FBLoginButton();
			this.loginButton1 = new Waveface.Stream.WindowsClient.LoginButton();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.label1 = new System.Windows.Forms.Label();
			this.lblSignUp = new System.Windows.Forms.LinkLabel();
			this.chkRememberPassword = new System.Windows.Forms.CheckBox();
			this.label2 = new System.Windows.Forms.Label();
			this.tabMainStationSetup = new System.Windows.Forms.TabPage();
			this.btnOK = new System.Windows.Forms.Button();
			this.lblWelcome = new System.Windows.Forms.Label();
			this.lblMainStationSetup = new System.Windows.Forms.Label();
			this.tabSecondStationSetup = new System.Windows.Forms.TabPage();
			this.btnOK2 = new System.Windows.Forms.Button();
			this.lblSecondStationSetup = new System.Windows.Forms.Label();
			this.lblWelcome2 = new System.Windows.Forms.Label();
			this.tabControl.SuspendLayout();
			this.tabPage1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
			this.tabSignIn.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.tabMainStationSetup.SuspendLayout();
			this.tabSecondStationSetup.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControl
			// 
			this.tabControl.Controls.Add(this.tabPage1);
			this.tabControl.Controls.Add(this.tabSignIn);
			this.tabControl.Controls.Add(this.tabMainStationSetup);
			this.tabControl.Controls.Add(this.tabSecondStationSetup);
			resources.ApplyResources(this.tabControl, "tabControl");
			this.tabControl.HideTabs = true;
			this.tabControl.Multiline = true;
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.TabStop = false;
			// 
			// tabPage1
			// 
			this.tabPage1.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.tabPage1.Controls.Add(this.button2);
			this.tabPage1.Controls.Add(this.button1);
			this.tabPage1.Controls.Add(this.pictureBox3);
			resources.ApplyResources(this.tabPage1, "tabPage1");
			this.tabPage1.Name = "tabPage1";
			// 
			// button2
			// 
			resources.ApplyResources(this.button2, "button2");
			this.button2.Name = "button2";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// button1
			// 
			resources.ApplyResources(this.button1, "button1");
			this.button1.Name = "button1";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// pictureBox3
			// 
			resources.ApplyResources(this.pictureBox3, "pictureBox3");
			this.pictureBox3.Name = "pictureBox3";
			this.pictureBox3.TabStop = false;
			// 
			// tabSignIn
			// 
			this.tabSignIn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(225)))), ((int)(((byte)(225)))));
			this.tabSignIn.Controls.Add(this.loginInputBox1);
			this.tabSignIn.Controls.Add(this.devVersionTag);
			this.tabSignIn.Controls.Add(this.pictureBox2);
			this.tabSignIn.Controls.Add(this.linkLabel1);
			this.tabSignIn.Controls.Add(this.fbLoginButton1);
			this.tabSignIn.Controls.Add(this.loginButton1);
			this.tabSignIn.Controls.Add(this.pictureBox1);
			this.tabSignIn.Controls.Add(this.label1);
			this.tabSignIn.Controls.Add(this.lblSignUp);
			this.tabSignIn.Controls.Add(this.chkRememberPassword);
			this.tabSignIn.Controls.Add(this.label2);
			resources.ApplyResources(this.tabSignIn, "tabSignIn");
			this.tabSignIn.Name = "tabSignIn";
			// 
			// loginInputBox1
			// 
			resources.ApplyResources(this.loginInputBox1, "loginInputBox1");
			this.loginInputBox1.Name = "loginInputBox1";
			// 
			// devVersionTag
			// 
			resources.ApplyResources(this.devVersionTag, "devVersionTag");
			this.devVersionTag.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.devVersionTag.ForeColor = System.Drawing.Color.Red;
			this.devVersionTag.Name = "devVersionTag";
			// 
			// pictureBox2
			// 
			resources.ApplyResources(this.pictureBox2, "pictureBox2");
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.TabStop = false;
			// 
			// linkLabel1
			// 
			resources.ApplyResources(this.linkLabel1, "linkLabel1");
			this.linkLabel1.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(165)))), ((int)(((byte)(212)))));
			this.linkLabel1.Name = "linkLabel1";
			this.linkLabel1.TabStop = true;
			this.linkLabel1.VisitedLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(165)))), ((int)(((byte)(212)))));
			// 
			// fbLoginButton1
			// 
			resources.ApplyResources(this.fbLoginButton1, "fbLoginButton1");
			this.fbLoginButton1.Name = "fbLoginButton1";
			// 
			// loginButton1
			// 
			resources.ApplyResources(this.loginButton1, "loginButton1");
			this.loginButton1.MaximumSize = new System.Drawing.Size(110, 40);
			this.loginButton1.Name = "loginButton1";
			this.loginButton1.Click += new System.EventHandler(this.loginButton1_Click);
			// 
			// pictureBox1
			// 
			resources.ApplyResources(this.pictureBox1, "pictureBox1");
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.TabStop = false;
			// 
			// label1
			// 
			this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// lblSignUp
			// 
			resources.ApplyResources(this.lblSignUp, "lblSignUp");
			this.lblSignUp.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(48)))), ((int)(((byte)(255)))));
			this.lblSignUp.Name = "lblSignUp";
			this.lblSignUp.TabStop = true;
			this.lblSignUp.UseCompatibleTextRendering = true;
			this.lblSignUp.VisitedLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(48)))), ((int)(((byte)(255)))));
			// 
			// chkRememberPassword
			// 
			resources.ApplyResources(this.chkRememberPassword, "chkRememberPassword");
			this.chkRememberPassword.Name = "chkRememberPassword";
			this.chkRememberPassword.UseVisualStyleBackColor = true;
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(146)))), ((int)(((byte)(146)))), ((int)(((byte)(146)))));
			this.label2.Name = "label2";
			// 
			// tabMainStationSetup
			// 
			this.tabMainStationSetup.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(225)))), ((int)(((byte)(225)))));
			this.tabMainStationSetup.Controls.Add(this.btnOK);
			this.tabMainStationSetup.Controls.Add(this.lblWelcome);
			this.tabMainStationSetup.Controls.Add(this.lblMainStationSetup);
			resources.ApplyResources(this.tabMainStationSetup, "tabMainStationSetup");
			this.tabMainStationSetup.Name = "tabMainStationSetup";
			// 
			// btnOK
			// 
			resources.ApplyResources(this.btnOK, "btnOK");
			this.btnOK.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.btnOK.Name = "btnOK";
			this.btnOK.UseVisualStyleBackColor = false;
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
			this.tabSecondStationSetup.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(225)))), ((int)(((byte)(225)))));
			this.tabSecondStationSetup.Controls.Add(this.btnOK2);
			this.tabSecondStationSetup.Controls.Add(this.lblSecondStationSetup);
			this.tabSecondStationSetup.Controls.Add(this.lblWelcome2);
			resources.ApplyResources(this.tabSecondStationSetup, "tabSecondStationSetup");
			this.tabSecondStationSetup.Name = "tabSecondStationSetup";
			// 
			// btnOK2
			// 
			resources.ApplyResources(this.btnOK2, "btnOK2");
			this.btnOK2.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.btnOK2.Name = "btnOK2";
			this.btnOK2.UseVisualStyleBackColor = false;
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
			// LoginDialog
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.tabControl);
			this.DoubleBuffered = true;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "LoginDialog";
			this.tabControl.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
			this.tabSignIn.ResumeLayout(false);
			this.tabSignIn.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.tabMainStationSetup.ResumeLayout(false);
			this.tabMainStationSetup.PerformLayout();
			this.tabSecondStationSetup.ResumeLayout(false);
			this.tabSecondStationSetup.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.CheckBox chkRememberPassword;
		private System.Windows.Forms.LinkLabel lblSignUp;
        private TabControlEx tabControl;
		private System.Windows.Forms.TabPage tabSignIn;
		private System.Windows.Forms.TabPage tabMainStationSetup;
		private System.Windows.Forms.Label lblMainStationSetup;
		private System.Windows.Forms.Label lblWelcome;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.TabPage tabSecondStationSetup;
		private System.Windows.Forms.Label lblWelcome2;
		private System.Windows.Forms.Label lblSecondStationSetup;
		private System.Windows.Forms.Button btnOK2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.PictureBox pictureBox2;
		private FBLoginButton fbLoginButton1;
		private System.Windows.Forms.LinkLabel linkLabel1;
		private System.Windows.Forms.Label label2;
		private LoginButton loginButton1;
		private System.Windows.Forms.Label devVersionTag;
		private LoginInputBox loginInputBox1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.PictureBox pictureBox3;
	}
}

