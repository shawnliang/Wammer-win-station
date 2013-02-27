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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginInputBox));
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
			resources.ApplyResources(this.loginInputPanel1, "loginInputPanel1");
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
			// 
			// tbxPassword
			// 
			resources.ApplyResources(this.tbxPassword, "tbxPassword");
			this.tbxPassword.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.tbxPassword.Name = "tbxPassword";
			this.tbxPassword.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbxPassword_KeyDown);
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
			// 
			// LoginInputBox
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.loginInputPanel1);
			this.Name = "LoginInputBox";
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
