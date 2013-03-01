namespace Waveface.Stream.WindowsClient
{
	partial class SignupControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SignupControl));
			this.emailBox = new Waveface.Stream.WindowsClient.CueTextBox();
			this.passwordBox = new Waveface.Stream.WindowsClient.CueTextBox();
			this.nameBox = new Waveface.Stream.WindowsClient.CueTextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.nativeSignupButton = new System.Windows.Forms.Button();
			this.fbButton = new Waveface.Stream.WindowsClient.FBLoginButton();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// emailBox
			// 
			resources.ApplyResources(this.emailBox, "emailBox");
			this.emailBox.Name = "emailBox";
			this.emailBox.Leave += new System.EventHandler(this.emailBox_Leave);
			// 
			// passwordBox
			// 
			resources.ApplyResources(this.passwordBox, "passwordBox");
			this.passwordBox.Name = "passwordBox";
			// 
			// nameBox
			// 
			resources.ApplyResources(this.nameBox, "nameBox");
			this.nameBox.Name = "nameBox";
			this.nameBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.nameBox_KeyPress);
			// 
			// label3
			// 
			resources.ApplyResources(this.label3, "label3");
			this.label3.Name = "label3";
			// 
			// label4
			// 
			resources.ApplyResources(this.label4, "label4");
			this.label4.Name = "label4";
			// 
			// label5
			// 
			resources.ApplyResources(this.label5, "label5");
			this.label5.Name = "label5";
			// 
			// label6
			// 
			resources.ApplyResources(this.label6, "label6");
			this.label6.ForeColor = System.Drawing.SystemColors.ControlDark;
			this.label6.Name = "label6";
			// 
			// nativeSignupButton
			// 
			resources.ApplyResources(this.nativeSignupButton, "nativeSignupButton");
			this.nativeSignupButton.Name = "nativeSignupButton";
			this.nativeSignupButton.UseVisualStyleBackColor = true;
			this.nativeSignupButton.Click += new System.EventHandler(this.nativeSignupButton_Click);
			// 
			// fbButton
			// 
			resources.ApplyResources(this.fbButton, "fbButton");
			this.fbButton.Name = "fbButton";
			this.fbButton.Click += new System.EventHandler(this.fbButton_Click);
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// SignupControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.label1);
			this.Controls.Add(this.nativeSignupButton);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.nameBox);
			this.Controls.Add(this.passwordBox);
			this.Controls.Add(this.emailBox);
			this.Controls.Add(this.fbButton);
			this.Name = "SignupControl";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private FBLoginButton fbButton;
		private CueTextBox emailBox;
		private CueTextBox passwordBox;
		private CueTextBox nameBox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Button nativeSignupButton;
		private System.Windows.Forms.Label label1;
	}
}
