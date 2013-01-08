namespace Waveface.Stream.WindowsClient
{
	partial class LoginControl
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginControl));
			this.forgotPwdLabel = new System.Windows.Forms.LinkLabel();
			this.loginInputPanel1 = new Waveface.Stream.WindowsClient.LoginInputPanel();
			this.txtPassword = new Waveface.Stream.WindowsClient.CueTextBox();
			this.tbxEMail = new Waveface.Stream.WindowsClient.CueTextBox();
			this.fbLoginButton1 = new Waveface.Stream.WindowsClient.FBLoginButton();
			this.label1 = new System.Windows.Forms.Label();
			this.loginButton = new System.Windows.Forms.Button();
			this.loginInputPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// forgotPwdLabel
			// 
			resources.ApplyResources(this.forgotPwdLabel, "forgotPwdLabel");
			this.forgotPwdLabel.Name = "forgotPwdLabel";
			this.forgotPwdLabel.TabStop = true;
			this.forgotPwdLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.forgotPwdLabel_LinkClicked);
			// 
			// loginInputPanel1
			// 
			resources.ApplyResources(this.loginInputPanel1, "loginInputPanel1");
			this.loginInputPanel1.Controls.Add(this.txtPassword);
			this.loginInputPanel1.Controls.Add(this.tbxEMail);
			this.loginInputPanel1.Name = "loginInputPanel1";
			// 
			// txtPassword
			// 
			resources.ApplyResources(this.txtPassword, "txtPassword");
			this.txtPassword.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.txtPassword.Name = "txtPassword";
			this.txtPassword.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPassword_KeyPress);
			// 
			// tbxEMail
			// 
			resources.ApplyResources(this.tbxEMail, "tbxEMail");
			this.tbxEMail.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.tbxEMail.Name = "tbxEMail";
			// 
			// fbLoginButton1
			// 
			resources.ApplyResources(this.fbLoginButton1, "fbLoginButton1");
			this.fbLoginButton1.Name = "fbLoginButton1";
			this.fbLoginButton1.Click += new System.EventHandler(this.fbLoginButton1_Click);
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// loginButton
			// 
			resources.ApplyResources(this.loginButton, "loginButton");
			this.loginButton.Name = "loginButton";
			this.loginButton.UseVisualStyleBackColor = true;
			this.loginButton.Click += new System.EventHandler(this.loginButton1_Click);
			// 
			// LoginControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.Controls.Add(this.loginButton);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.forgotPwdLabel);
			this.Controls.Add(this.loginInputPanel1);
			this.Controls.Add(this.fbLoginButton1);
			this.Name = "LoginControl";
			this.loginInputPanel1.ResumeLayout(false);
			this.loginInputPanel1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private FBLoginButton fbLoginButton1;
		private LoginInputPanel loginInputPanel1;
		internal CueTextBox txtPassword;
		internal CueTextBox tbxEMail;
		private System.Windows.Forms.LinkLabel forgotPwdLabel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button loginButton;
	}
}
