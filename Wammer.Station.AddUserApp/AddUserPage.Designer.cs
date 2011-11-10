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
			this.label4 = new System.Windows.Forms.Label();
			this.textFolder = new System.Windows.Forms.TextBox();
			this.btnFolderSelect = new System.Windows.Forms.Button();
			this.linkLabel1 = new System.Windows.Forms.LinkLabel();
			this.openFolderDialog = new System.Windows.Forms.FolderBrowserDialog();
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
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(47, 165);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(63, 13);
			this.label4.TabIndex = 7;
			this.label4.Text = "Folder path:";
			// 
			// textFolder
			// 
			this.textFolder.Location = new System.Drawing.Point(201, 162);
			this.textFolder.Name = "textFolder";
			this.textFolder.Size = new System.Drawing.Size(299, 20);
			this.textFolder.TabIndex = 4;
			// 
			// btnFolderSelect
			// 
			this.btnFolderSelect.Location = new System.Drawing.Point(506, 160);
			this.btnFolderSelect.Name = "btnFolderSelect";
			this.btnFolderSelect.Size = new System.Drawing.Size(39, 23);
			this.btnFolderSelect.TabIndex = 7;
			this.btnFolderSelect.Text = "...";
			this.btnFolderSelect.UseVisualStyleBackColor = true;
			this.btnFolderSelect.Click += new System.EventHandler(this.btnFolderSelect_Click);
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
			// AddUserPage
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(588, 294);
			this.Controls.Add(this.linkLabel1);
			this.Controls.Add(this.btnFolderSelect);
			this.Controls.Add(this.textFolder);
			this.Controls.Add(this.label4);
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
			this.Load += new System.EventHandler(this.AddUserPage_Load);
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
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textFolder;
		private System.Windows.Forms.Button btnFolderSelect;
		private System.Windows.Forms.LinkLabel linkLabel1;
		private System.Windows.Forms.FolderBrowserDialog openFolderDialog;
	}
}

