namespace Waveface.Stream.WindowsClient
{
	partial class LoginInputBox
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
			this.loginInputPanel1 = new Waveface.Stream.WindowsClient.LoginInputPanel();
			this.button1 = new System.Windows.Forms.Button();
			this.tbxPassword = new Waveface.Stream.WindowsClient.CueTextBox();
			this.tbxEMail = new Waveface.Stream.WindowsClient.CueTextBox();
			this.cmbEmail = new System.Windows.Forms.ComboBox();
			this.loginInputPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// loginInputPanel1
			// 
			this.loginInputPanel1.Controls.Add(this.button1);
			this.loginInputPanel1.Controls.Add(this.tbxPassword);
			this.loginInputPanel1.Controls.Add(this.tbxEMail);
			this.loginInputPanel1.Controls.Add(this.cmbEmail);
			this.loginInputPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.loginInputPanel1.Location = new System.Drawing.Point(0, 0);
			this.loginInputPanel1.Name = "loginInputPanel1";
			this.loginInputPanel1.Size = new System.Drawing.Size(279, 103);
			this.loginInputPanel1.TabIndex = 3;
			// 
			// button1
			// 
			this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button1.BackColor = System.Drawing.Color.White;
			this.button1.CausesValidation = false;
			this.button1.FlatAppearance.BorderSize = 0;
			this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.button1.Location = new System.Drawing.Point(235, 8);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(21, 37);
			this.button1.TabIndex = 1;
			this.button1.TabStop = false;
			this.button1.Text = "▼";
			this.button1.UseVisualStyleBackColor = false;
			// 
			// tbxPassword
			// 
			this.tbxPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbxPassword.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.tbxPassword.CueText = "Password";
			this.tbxPassword.Font = new System.Drawing.Font("Arial", 15F);
			this.tbxPassword.Location = new System.Drawing.Point(22, 65);
			this.tbxPassword.Name = "tbxPassword";
			this.tbxPassword.PasswordChar = '*';
			this.tbxPassword.Size = new System.Drawing.Size(235, 23);
			this.tbxPassword.TabIndex = 2;
			// 
			// tbxEMail
			// 
			this.tbxEMail.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbxEMail.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.tbxEMail.CueText = "Email";
			this.tbxEMail.Font = new System.Drawing.Font("Arial", 15F);
			this.tbxEMail.Location = new System.Drawing.Point(22, 15);
			this.tbxEMail.Name = "tbxEMail";
			this.tbxEMail.Size = new System.Drawing.Size(212, 23);
			this.tbxEMail.TabIndex = 0;
			// 
			// cmbEmail
			// 
			this.cmbEmail.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.cmbEmail.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.cmbEmail.Font = new System.Drawing.Font("Arial", 15F);
			this.cmbEmail.FormattingEnabled = true;
			this.cmbEmail.Location = new System.Drawing.Point(22, 15);
			this.cmbEmail.Name = "cmbEmail";
			this.cmbEmail.Size = new System.Drawing.Size(235, 31);
			this.cmbEmail.Sorted = true;
			this.cmbEmail.TabIndex = 1;
			this.cmbEmail.Visible = false;
			// 
			// LoginInputBox
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.loginInputPanel1);
			this.Name = "LoginInputBox";
			this.Size = new System.Drawing.Size(279, 105);
			this.SizeChanged += new System.EventHandler(this.LoginInputBox_SizeChanged);
			this.loginInputPanel1.ResumeLayout(false);
			this.loginInputPanel1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		internal System.Windows.Forms.ComboBox cmbEmail;
		internal CueTextBox tbxEMail;
		internal CueTextBox tbxPassword;
		private System.Windows.Forms.Button button1;
		private LoginInputPanel loginInputPanel1;

	}
}
