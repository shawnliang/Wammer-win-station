namespace Waveface.Stream.WindowsClient
{
	partial class AddDeviceDialog
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddDeviceDialog));
			this.doneButton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.richTextBox1 = new System.Windows.Forms.RichTextBox();
			this.appStoreButton = new System.Windows.Forms.PictureBox();
			this.googlePlayButton = new System.Windows.Forms.PictureBox();
			this.pictureBox3 = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.appStoreButton)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.googlePlayButton)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
			this.SuspendLayout();
			// 
			// doneButton
			// 
			resources.ApplyResources(this.doneButton, "doneButton");
			this.doneButton.Name = "doneButton";
			this.doneButton.UseVisualStyleBackColor = true;
			this.doneButton.Click += new System.EventHandler(this.doneButton_Click);
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// richTextBox1
			// 
			resources.ApplyResources(this.richTextBox1, "richTextBox1");
			this.richTextBox1.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.richTextBox1.Cursor = System.Windows.Forms.Cursors.Default;
			this.richTextBox1.Name = "richTextBox1";
			// 
			// appStoreButton
			// 
			resources.ApplyResources(this.appStoreButton, "appStoreButton");
			this.appStoreButton.Cursor = System.Windows.Forms.Cursors.Hand;
			this.appStoreButton.Image = global::Waveface.Stream.WindowsClient.Properties.Resources.button_appstore;
			this.appStoreButton.Name = "appStoreButton";
			this.appStoreButton.TabStop = false;
			this.appStoreButton.Click += new System.EventHandler(this.appStoreButton_Click);
			// 
			// googlePlayButton
			// 
			resources.ApplyResources(this.googlePlayButton, "googlePlayButton");
			this.googlePlayButton.Cursor = System.Windows.Forms.Cursors.Hand;
			this.googlePlayButton.Image = global::Waveface.Stream.WindowsClient.Properties.Resources.button_googleplay;
			this.googlePlayButton.Name = "googlePlayButton";
			this.googlePlayButton.TabStop = false;
			this.googlePlayButton.Click += new System.EventHandler(this.googlePlayButton_Click);
			// 
			// pictureBox3
			// 
			resources.ApplyResources(this.pictureBox3, "pictureBox3");
			this.pictureBox3.Image = global::Waveface.Stream.WindowsClient.Properties.Resources.qr_waveface_site;
			this.pictureBox3.Name = "pictureBox3";
			this.pictureBox3.TabStop = false;
			// 
			// AddDeviceDialog
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.Controls.Add(this.pictureBox3);
			this.Controls.Add(this.googlePlayButton);
			this.Controls.Add(this.appStoreButton);
			this.Controls.Add(this.richTextBox1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.doneButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "AddDeviceDialog";
			this.Load += new System.EventHandler(this.AddDeviceDialog_Load);
			((System.ComponentModel.ISupportInitialize)(this.appStoreButton)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.googlePlayButton)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		protected System.Windows.Forms.Button doneButton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.RichTextBox richTextBox1;
		private System.Windows.Forms.PictureBox appStoreButton;
		private System.Windows.Forms.PictureBox googlePlayButton;
		private System.Windows.Forms.PictureBox pictureBox3;
	}
}