namespace StationSystemTray
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
			this.pictureBox2 = new System.Windows.Forms.PictureBox();
			this.label1 = new System.Windows.Forms.Label();
			this.loginButton1 = new StationSystemTray.LoginButton();
			this.loginInputPanel1 = new StationSystemTray.LoginInputPanel();
			this.txtPassword = new StationSystemTray.CueTextBox();
			this.tbxEMail = new StationSystemTray.CueTextBox();
			this.fbLoginButton1 = new StationSystemTray.FBLoginButton();
			this.forgotPwdLabel = new System.Windows.Forms.LinkLabel();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
			this.loginInputPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// pictureBox2
			// 
			this.pictureBox2.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.pictureBox2.BackColor = System.Drawing.SystemColors.Control;
			this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
			this.pictureBox2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.pictureBox2.Location = new System.Drawing.Point(229, 103);
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.Size = new System.Drawing.Size(26, 25);
			this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.pictureBox2.TabIndex = 12;
			this.pictureBox2.TabStop = false;
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.label1.Location = new System.Drawing.Point(50, 115);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(383, 1);
			this.label1.TabIndex = 11;
			// 
			// loginButton1
			// 
			this.loginButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.loginButton1.Font = new System.Drawing.Font("Arial", 12.75F, System.Drawing.FontStyle.Bold);
			this.loginButton1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.loginButton1.Location = new System.Drawing.Point(323, 290);
			this.loginButton1.Margin = new System.Windows.Forms.Padding(0);
			this.loginButton1.MaximumSize = new System.Drawing.Size(110, 40);
			this.loginButton1.Name = "loginButton1";
			this.loginButton1.Size = new System.Drawing.Size(110, 40);
			this.loginButton1.TabIndex = 14;
			this.loginButton1.Text = "Login";
			this.loginButton1.Click += new System.EventHandler(this.loginButton1_Click);
			// 
			// loginInputPanel1
			// 
			this.loginInputPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.loginInputPanel1.Controls.Add(this.txtPassword);
			this.loginInputPanel1.Controls.Add(this.tbxEMail);
			this.loginInputPanel1.Location = new System.Drawing.Point(50, 162);
			this.loginInputPanel1.Name = "loginInputPanel1";
			this.loginInputPanel1.Size = new System.Drawing.Size(383, 103);
			this.loginInputPanel1.TabIndex = 13;
			// 
			// txtPassword
			// 
			this.txtPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtPassword.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.txtPassword.CueText = "Password";
			this.txtPassword.Font = new System.Drawing.Font("Arial", 15F);
			this.txtPassword.Location = new System.Drawing.Point(22, 65);
			this.txtPassword.Name = "txtPassword";
			this.txtPassword.PasswordChar = '*';
			this.txtPassword.Size = new System.Drawing.Size(326, 23);
			this.txtPassword.TabIndex = 2;
			this.txtPassword.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPassword_KeyPress);
			// 
			// tbxEMail
			// 
			this.tbxEMail.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbxEMail.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.tbxEMail.CueText = "Email";
			this.tbxEMail.Font = new System.Drawing.Font("Arial", 15F);
			this.tbxEMail.Location = new System.Drawing.Point(22, 19);
			this.tbxEMail.Name = "tbxEMail";
			this.tbxEMail.Size = new System.Drawing.Size(326, 23);
			this.tbxEMail.TabIndex = 0;
			// 
			// fbLoginButton1
			// 
			this.fbLoginButton1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.fbLoginButton1.AutoSize = true;
			this.fbLoginButton1.DisplayText = "Login with Facebook";
			this.fbLoginButton1.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold);
			this.fbLoginButton1.Location = new System.Drawing.Point(50, 39);
			this.fbLoginButton1.Margin = new System.Windows.Forms.Padding(6, 5, 6, 5);
			this.fbLoginButton1.Name = "fbLoginButton1";
			this.fbLoginButton1.Size = new System.Drawing.Size(383, 49);
			this.fbLoginButton1.TabIndex = 1;
			this.fbLoginButton1.Click += new System.EventHandler(this.fbLoginButton1_Click);
			// 
			// forgotPwdLabel
			// 
			this.forgotPwdLabel.AutoSize = true;
			this.forgotPwdLabel.Font = new System.Drawing.Font("Arial", 9F);
			this.forgotPwdLabel.Location = new System.Drawing.Point(47, 290);
			this.forgotPwdLabel.Name = "forgotPwdLabel";
			this.forgotPwdLabel.Size = new System.Drawing.Size(107, 15);
			this.forgotPwdLabel.TabIndex = 15;
			this.forgotPwdLabel.TabStop = true;
			this.forgotPwdLabel.Text = "Forgot password?";
			this.forgotPwdLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.forgotPwdLabel_LinkClicked);
			// 
			// LoginControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.forgotPwdLabel);
			this.Controls.Add(this.loginButton1);
			this.Controls.Add(this.loginInputPanel1);
			this.Controls.Add(this.pictureBox2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.fbLoginButton1);
			this.Name = "LoginControl";
			this.Size = new System.Drawing.Size(494, 388);
			this.Load += new System.EventHandler(this.LoginControl_Load);
			this.SizeChanged += new System.EventHandler(this.LoginControl_SizeChanged);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
			this.loginInputPanel1.ResumeLayout(false);
			this.loginInputPanel1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private FBLoginButton fbLoginButton1;
		private System.Windows.Forms.PictureBox pictureBox2;
		private System.Windows.Forms.Label label1;
		private LoginInputPanel loginInputPanel1;
		internal CueTextBox txtPassword;
		internal CueTextBox tbxEMail;
		private LoginButton loginButton1;
		private System.Windows.Forms.LinkLabel forgotPwdLabel;
	}
}
