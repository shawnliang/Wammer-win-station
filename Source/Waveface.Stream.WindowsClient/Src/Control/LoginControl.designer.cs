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
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.panel1 = new System.Windows.Forms.Panel();
			this.forgotPwdLabel = new System.Windows.Forms.LinkLabel();
			this.label1 = new System.Windows.Forms.Label();
			this.loginButton = new System.Windows.Forms.Button();
			this.fbLoginButton1 = new Waveface.Stream.WindowsClient.FBLoginButton();
			this.loginInputBox1 = new Waveface.Stream.WindowsClient.LoginInputBox();
			this.tableLayoutPanel1.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
			this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 1);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.fbLoginButton1);
			this.panel1.Controls.Add(this.forgotPwdLabel);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Controls.Add(this.loginButton);
			this.panel1.Controls.Add(this.loginInputBox1);
			resources.ApplyResources(this.panel1, "panel1");
			this.panel1.Name = "panel1";
			// 
			// forgotPwdLabel
			// 
			resources.ApplyResources(this.forgotPwdLabel, "forgotPwdLabel");
			this.forgotPwdLabel.Name = "forgotPwdLabel";
			this.forgotPwdLabel.TabStop = true;
			this.forgotPwdLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.forgotPwdLabel_LinkClicked);
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
			// fbLoginButton1
			// 
			resources.ApplyResources(this.fbLoginButton1, "fbLoginButton1");
			this.fbLoginButton1.Name = "fbLoginButton1";
			this.fbLoginButton1.Click += new System.EventHandler(this.fbLoginButton1_Click);
			// 
			// loginInputBox1
			// 
			resources.ApplyResources(this.loginInputBox1, "loginInputBox1");
			this.loginInputBox1.EnableDropDown = false;
			this.loginInputBox1.Name = "loginInputBox1";
			this.loginInputBox1.InputDone += new System.EventHandler(this.loginInputBox1_InputDone);
			// 
			// LoginControl
			// 
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "LoginControl";
			resources.ApplyResources(this, "$this");
			this.tableLayoutPanel1.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Panel panel1;
		private FBLoginButton fbLoginButton1;
		private System.Windows.Forms.LinkLabel forgotPwdLabel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button loginButton;
		private LoginInputBox loginInputBox1;

	}
}
