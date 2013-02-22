namespace Waveface.Stream.WindowsClient
{
	partial class NativeSignupControl
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
			this.emailBox = new System.Windows.Forms.TextBox();
			this.passwordBox = new System.Windows.Forms.TextBox();
			this.nameBox = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.nativeSignupButton = new System.Windows.Forms.Button();
			this.fbButton = new Waveface.Stream.WindowsClient.FBLoginButton();
			this.SuspendLayout();
			// 
			// emailBox
			// 
			this.emailBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
			this.emailBox.Location = new System.Drawing.Point(186, 219);
			this.emailBox.Name = "emailBox";
			this.emailBox.Size = new System.Drawing.Size(335, 24);
			this.emailBox.TabIndex = 1;
			// 
			// passwordBox
			// 
			this.passwordBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
			this.passwordBox.Location = new System.Drawing.Point(186, 275);
			this.passwordBox.Name = "passwordBox";
			this.passwordBox.PasswordChar = '*';
			this.passwordBox.Size = new System.Drawing.Size(335, 24);
			this.passwordBox.TabIndex = 2;
			// 
			// nameBox
			// 
			this.nameBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
			this.nameBox.Location = new System.Drawing.Point(186, 364);
			this.nameBox.Name = "nameBox";
			this.nameBox.Size = new System.Drawing.Size(335, 24);
			this.nameBox.TabIndex = 3;
			this.nameBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.nameBox_KeyPress);
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
			this.label1.Location = new System.Drawing.Point(186, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(335, 34);
			this.label1.TabIndex = 4;
			this.label1.Text = "Connect AOStream with your Facebook account ";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label2
			// 
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
			this.label2.Location = new System.Drawing.Point(186, 161);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(335, 37);
			this.label2.TabIndex = 5;
			this.label2.Text = "Sign up with your email";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
			this.label3.Location = new System.Drawing.Point(183, 198);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(49, 18);
			this.label3.TabIndex = 6;
			this.label3.Text = "Email:";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
			this.label4.Location = new System.Drawing.Point(183, 254);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(79, 18);
			this.label4.TabIndex = 7;
			this.label4.Text = "Password:";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
			this.label5.Location = new System.Drawing.Point(183, 343);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(84, 18);
			this.label5.TabIndex = 8;
			this.label5.Text = "Your name:";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
			this.label6.ForeColor = System.Drawing.SystemColors.ControlDark;
			this.label6.Location = new System.Drawing.Point(186, 302);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(309, 15);
			this.label6.TabIndex = 9;
			this.label6.Text = "Password must contain 6-16 characters with no spaces.";
			// 
			// nativeSignupButton
			// 
			this.nativeSignupButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
			this.nativeSignupButton.Location = new System.Drawing.Point(420, 399);
			this.nativeSignupButton.Name = "nativeSignupButton";
			this.nativeSignupButton.Size = new System.Drawing.Size(101, 27);
			this.nativeSignupButton.TabIndex = 10;
			this.nativeSignupButton.Text = "Sign Up";
			this.nativeSignupButton.UseVisualStyleBackColor = true;
			this.nativeSignupButton.Click += new System.EventHandler(this.nativeSignupButton_Click);
			// 
			// fbButton
			// 
			this.fbButton.AutoSize = true;
			this.fbButton.DisplayText = "Connect with Facebook";
			this.fbButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.25F);
			this.fbButton.Location = new System.Drawing.Point(186, 55);
			this.fbButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.fbButton.Name = "fbButton";
			this.fbButton.Size = new System.Drawing.Size(335, 48);
			this.fbButton.TabIndex = 0;
			this.fbButton.Click += new System.EventHandler(this.fbButton_Click);
			// 
			// NativeSignupControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.nativeSignupButton);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.nameBox);
			this.Controls.Add(this.passwordBox);
			this.Controls.Add(this.emailBox);
			this.Controls.Add(this.fbButton);
			this.Name = "NativeSignupControl";
			this.Size = new System.Drawing.Size(706, 433);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private FBLoginButton fbButton;
		private System.Windows.Forms.TextBox emailBox;
		private System.Windows.Forms.TextBox passwordBox;
		private System.Windows.Forms.TextBox nameBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Button nativeSignupButton;
	}
}
