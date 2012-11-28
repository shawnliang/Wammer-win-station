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
			this.forgotPwdLabel = new System.Windows.Forms.LinkLabel();
			this.loginInputPanel1 = new LoginInputPanel();
			this.txtPassword = new CueTextBox();
			this.tbxEMail = new CueTextBox();
			this.fbLoginButton1 = new FBLoginButton();
			this.label1 = new System.Windows.Forms.Label();
			this.loginButton = new System.Windows.Forms.Button();
			this.loginInputPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// forgotPwdLabel
			// 
			this.forgotPwdLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.forgotPwdLabel.AutoSize = true;
			this.forgotPwdLabel.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.forgotPwdLabel.Location = new System.Drawing.Point(224, 304);
			this.forgotPwdLabel.Name = "forgotPwdLabel";
			this.forgotPwdLabel.Size = new System.Drawing.Size(103, 14);
			this.forgotPwdLabel.TabIndex = 15;
			this.forgotPwdLabel.TabStop = true;
			this.forgotPwdLabel.Text = "Forgot password?";
			this.forgotPwdLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.forgotPwdLabel_LinkClicked);
			// 
			// loginInputPanel1
			// 
			this.loginInputPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.loginInputPanel1.Controls.Add(this.txtPassword);
			this.loginInputPanel1.Controls.Add(this.tbxEMail);
			this.loginInputPanel1.Location = new System.Drawing.Point(115, 188);
			this.loginInputPanel1.Name = "loginInputPanel1";
			this.loginInputPanel1.Size = new System.Drawing.Size(324, 103);
			this.loginInputPanel1.TabIndex = 13;
			// 
			// txtPassword
			// 
			this.txtPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtPassword.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.txtPassword.CueText = "Password";
			this.txtPassword.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtPassword.Location = new System.Drawing.Point(22, 65);
			this.txtPassword.Name = "txtPassword";
			this.txtPassword.PasswordChar = '*';
			this.txtPassword.Size = new System.Drawing.Size(267, 24);
			this.txtPassword.TabIndex = 2;
			this.txtPassword.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPassword_KeyPress);
			// 
			// tbxEMail
			// 
			this.tbxEMail.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbxEMail.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.tbxEMail.CueText = "Email";
			this.tbxEMail.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tbxEMail.Location = new System.Drawing.Point(22, 19);
			this.tbxEMail.Name = "tbxEMail";
			this.tbxEMail.Size = new System.Drawing.Size(267, 24);
			this.tbxEMail.TabIndex = 0;
			// 
			// fbLoginButton1
			// 
			this.fbLoginButton1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.fbLoginButton1.AutoSize = true;
			this.fbLoginButton1.DisplayText = "Login with Facebook";
			this.fbLoginButton1.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.fbLoginButton1.Location = new System.Drawing.Point(115, 64);
			this.fbLoginButton1.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
			this.fbLoginButton1.Name = "fbLoginButton1";
			this.fbLoginButton1.Size = new System.Drawing.Size(324, 49);
			this.fbLoginButton1.TabIndex = 1;
			this.fbLoginButton1.Click += new System.EventHandler(this.fbLoginButton1_Click);
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(115, 122);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(324, 23);
			this.label1.TabIndex = 16;
			this.label1.Text = "OR";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// loginButton
			// 
			this.loginButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.loginButton.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.loginButton.Location = new System.Drawing.Point(115, 344);
			this.loginButton.Name = "loginButton";
			this.loginButton.Size = new System.Drawing.Size(324, 32);
			this.loginButton.TabIndex = 17;
			this.loginButton.Text = "Login";
			this.loginButton.UseVisualStyleBackColor = true;
			this.loginButton.Click += new System.EventHandler(this.loginButton1_Click);
			// 
			// LoginControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.Controls.Add(this.loginButton);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.forgotPwdLabel);
			this.Controls.Add(this.loginInputPanel1);
			this.Controls.Add(this.fbLoginButton1);
			this.Font = new System.Drawing.Font("Calibri", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Name = "LoginControl";
			this.Size = new System.Drawing.Size(555, 400);
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
