namespace Wammer.Station.StartUp
{
	partial class AddUserPage
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
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.textEmail = new System.Windows.Forms.TextBox();
			this.textPassword = new System.Windows.Forms.TextBox();
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.linkLabel1 = new System.Windows.Forms.LinkLabel();
			this.openFolderDialog = new System.Windows.Forms.FolderBrowserDialog();
			this.cloudStorageListTextBox = new System.Windows.Forms.TextBox();
			this.dropboxOAuthUrlTextBox = new System.Windows.Forms.TextBox();
			this.button3 = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.quotaText = new System.Windows.Forms.TextBox();
			this.button4 = new System.Windows.Forms.Button();
			this.button5 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(45, 92);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(149, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Your wammer account (email):";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(47, 130);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(56, 13);
			this.label2.TabIndex = 1;
			this.label2.Text = "Password:";
			// 
			// textEmail
			// 
			this.textEmail.Location = new System.Drawing.Point(201, 89);
			this.textEmail.Name = "textEmail";
			this.textEmail.Size = new System.Drawing.Size(299, 20);
			this.textEmail.TabIndex = 2;
			// 
			// textPassword
			// 
			this.textPassword.Location = new System.Drawing.Point(201, 127);
			this.textPassword.Name = "textPassword";
			this.textPassword.Size = new System.Drawing.Size(299, 20);
			this.textPassword.TabIndex = 3;
			this.textPassword.UseSystemPasswordChar = true;
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(327, 217);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 25);
			this.button1.TabIndex = 6;
			this.button1.Text = "Cancel";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.cancelButton_Click);
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(425, 217);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(75, 25);
			this.button2.TabIndex = 5;
			this.button2.Text = "Ok";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.okButton_Click);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(47, 30);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(512, 13);
			this.label3.TabIndex = 9;
			this.label3.Text = "Welcome to Wammer. Please type your wammer account and password to start Wammer W" +
				"indows Station.";
			// 
			// linkLabel1
			// 
			this.linkLabel1.AutoSize = true;
			this.linkLabel1.LinkArea = new System.Windows.Forms.LinkArea(28, 12);
			this.linkLabel1.Location = new System.Drawing.Point(47, 43);
			this.linkLabel1.Name = "linkLabel1";
			this.linkLabel1.Size = new System.Drawing.Size(232, 17);
			this.linkLabel1.TabIndex = 8;
			this.linkLabel1.TabStop = true;
			this.linkLabel1.Text = "Have no Wammer account yet? Sign up here!";
			this.linkLabel1.UseCompatibleTextRendering = true;
			this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
			// 
			// cloudStorageListTextBox
			// 
			this.cloudStorageListTextBox.Location = new System.Drawing.Point(31, 308);
			this.cloudStorageListTextBox.Multiline = true;
			this.cloudStorageListTextBox.Name = "cloudStorageListTextBox";
			this.cloudStorageListTextBox.Size = new System.Drawing.Size(371, 34);
			this.cloudStorageListTextBox.TabIndex = 10;
			// 
			// dropboxOAuthUrlTextBox
			// 
			this.dropboxOAuthUrlTextBox.Location = new System.Drawing.Point(31, 376);
			this.dropboxOAuthUrlTextBox.Multiline = true;
			this.dropboxOAuthUrlTextBox.Name = "dropboxOAuthUrlTextBox";
			this.dropboxOAuthUrlTextBox.Size = new System.Drawing.Size(371, 34);
			this.dropboxOAuthUrlTextBox.TabIndex = 11;
			// 
			// button3
			// 
			this.button3.Location = new System.Drawing.Point(201, 450);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(115, 23);
			this.button3.TabIndex = 14;
			this.button3.Text = "ConnectDropbox";
			this.button3.UseVisualStyleBackColor = true;
			this.button3.Click += new System.EventHandler(this.button3_Click);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(28, 458);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(34, 13);
			this.label4.TabIndex = 15;
			this.label4.Text = "quota";
			// 
			// quotaText
			// 
			this.quotaText.Location = new System.Drawing.Point(69, 453);
			this.quotaText.Name = "quotaText";
			this.quotaText.Size = new System.Drawing.Size(115, 20);
			this.quotaText.TabIndex = 16;
			// 
			// button4
			// 
			this.button4.Location = new System.Drawing.Point(420, 306);
			this.button4.Name = "button4";
			this.button4.Size = new System.Drawing.Size(113, 23);
			this.button4.TabIndex = 17;
			this.button4.Text = "List Cloud Storages";
			this.button4.UseVisualStyleBackColor = true;
			this.button4.Click += new System.EventHandler(this.button4_Click);
			// 
			// button5
			// 
			this.button5.Location = new System.Drawing.Point(420, 380);
			this.button5.Name = "button5";
			this.button5.Size = new System.Drawing.Size(139, 23);
			this.button5.TabIndex = 18;
			this.button5.Text = "Get Dropbox OAuth URL";
			this.button5.UseVisualStyleBackColor = true;
			this.button5.Click += new System.EventHandler(this.button5_Click);
			// 
			// AddUserPage
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(597, 651);
			this.Controls.Add(this.button5);
			this.Controls.Add(this.button4);
			this.Controls.Add(this.quotaText);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.button3);
			this.Controls.Add(this.dropboxOAuthUrlTextBox);
			this.Controls.Add(this.cloudStorageListTextBox);
			this.Controls.Add(this.linkLabel1);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.textPassword);
			this.Controls.Add(this.textEmail);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Name = "AddUserPage";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Welcome to Wammer";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textEmail;
		private System.Windows.Forms.TextBox textPassword;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.LinkLabel linkLabel1;
		private System.Windows.Forms.FolderBrowserDialog openFolderDialog;
		private System.Windows.Forms.TextBox cloudStorageListTextBox;
		private System.Windows.Forms.TextBox dropboxOAuthUrlTextBox;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox quotaText;
		private System.Windows.Forms.Button button4;
		private System.Windows.Forms.Button button5;
	}
}

