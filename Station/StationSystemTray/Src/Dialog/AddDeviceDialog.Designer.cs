namespace StationSystemTray
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
			this.doneButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.doneButton.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.doneButton.Location = new System.Drawing.Point(422, 399);
			this.doneButton.Name = "doneButton";
			this.doneButton.Size = new System.Drawing.Size(110, 32);
			this.doneButton.TabIndex = 2;
			this.doneButton.Text = "Done";
			this.doneButton.UseVisualStyleBackColor = true;
			this.doneButton.Click += new System.EventHandler(this.doneButton_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(12, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(263, 19);
			this.label1.TabIndex = 3;
			this.label1.Text = "Link your device to your Personal Cloud";
			// 
			// richTextBox1
			// 
			this.richTextBox1.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.richTextBox1.Cursor = System.Windows.Forms.Cursors.Default;
			this.richTextBox1.Location = new System.Drawing.Point(35, 41);
			this.richTextBox1.Name = "richTextBox1";
			this.richTextBox1.Size = new System.Drawing.Size(473, 165);
			this.richTextBox1.TabIndex = 4;
			this.richTextBox1.Text = "";
			// 
			// appStoreButton
			// 
			this.appStoreButton.Cursor = System.Windows.Forms.Cursors.Hand;
			this.appStoreButton.Image = global::StationSystemTray.Properties.Resources.button_appstore;
			this.appStoreButton.Location = new System.Drawing.Point(60, 233);
			this.appStoreButton.Name = "appStoreButton";
			this.appStoreButton.Size = new System.Drawing.Size(172, 60);
			this.appStoreButton.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.appStoreButton.TabIndex = 5;
			this.appStoreButton.TabStop = false;
			this.appStoreButton.Click += new System.EventHandler(this.appStoreButton_Click);
			// 
			// googlePlayButton
			// 
			this.googlePlayButton.Cursor = System.Windows.Forms.Cursors.Hand;
			this.googlePlayButton.Image = global::StationSystemTray.Properties.Resources.button_googleplay;
			this.googlePlayButton.Location = new System.Drawing.Point(60, 312);
			this.googlePlayButton.Name = "googlePlayButton";
			this.googlePlayButton.Size = new System.Drawing.Size(172, 60);
			this.googlePlayButton.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.googlePlayButton.TabIndex = 6;
			this.googlePlayButton.TabStop = false;
			this.googlePlayButton.Click += new System.EventHandler(this.googlePlayButton_Click);
			// 
			// pictureBox3
			// 
			this.pictureBox3.Image = global::StationSystemTray.Properties.Resources.qr_waveface_site;
			this.pictureBox3.Location = new System.Drawing.Point(307, 226);
			this.pictureBox3.Name = "pictureBox3";
			this.pictureBox3.Size = new System.Drawing.Size(150, 150);
			this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBox3.TabIndex = 7;
			this.pictureBox3.TabStop = false;
			// 
			// AddDeviceDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.ClientSize = new System.Drawing.Size(544, 443);
			this.Controls.Add(this.pictureBox3);
			this.Controls.Add(this.googlePlayButton);
			this.Controls.Add(this.appStoreButton);
			this.Controls.Add(this.richTextBox1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.doneButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "AddDeviceDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Add a device";
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