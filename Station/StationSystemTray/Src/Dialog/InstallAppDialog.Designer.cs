namespace StationSystemTray.Src.Dialog
{
	partial class InstallAppDialog
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InstallAppDialog));
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.qrCodePicture = new System.Windows.Forms.PictureBox();
			this.storePicture = new System.Windows.Forms.PictureBox();
			this.progress = new System.Windows.Forms.ProgressBar();
			this.label3 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.qrCodePicture)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.storePicture)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Arial", 10F);
			this.label1.Location = new System.Drawing.Point(38, 28);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(238, 63);
			this.label1.TabIndex = 5;
			this.label1.Text = resources.GetString("label1.Text");
			// 
			// label2
			// 
			this.label2.Font = new System.Drawing.Font("Arial", 10F);
			this.label2.Location = new System.Drawing.Point(331, 28);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(238, 63);
			this.label2.TabIndex = 6;
			this.label2.Text = resources.GetString("label2.Text");
			// 
			// qrCodePicture
			// 
			this.qrCodePicture.Location = new System.Drawing.Point(363, 112);
			this.qrCodePicture.Name = "qrCodePicture";
			this.qrCodePicture.Size = new System.Drawing.Size(128, 128);
			this.qrCodePicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.qrCodePicture.TabIndex = 7;
			this.qrCodePicture.TabStop = false;
			// 
			// storePicture
			// 
			this.storePicture.Cursor = System.Windows.Forms.Cursors.Hand;
			this.storePicture.Location = new System.Drawing.Point(59, 153);
			this.storePicture.Name = "storePicture";
			this.storePicture.Size = new System.Drawing.Size(100, 50);
			this.storePicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.storePicture.TabIndex = 8;
			this.storePicture.TabStop = false;
			this.storePicture.Click += new System.EventHandler(this.storePicture_Click);
			// 
			// progress
			// 
			this.progress.Location = new System.Drawing.Point(59, 314);
			this.progress.Name = "progress";
			this.progress.Size = new System.Drawing.Size(432, 23);
			this.progress.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
			this.progress.TabIndex = 9;
			// 
			// label3
			// 
			this.label3.Font = new System.Drawing.Font("Arial", 10F);
			this.label3.Location = new System.Drawing.Point(137, 291);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(312, 20);
			this.label3.TabIndex = 10;
			this.label3.Text = resources.GetString("label3.Text");
			// 
			// InstallApp
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(605, 373);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.progress);
			this.Controls.Add(this.storePicture);
			this.Controls.Add(this.qrCodePicture);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Name = "InstallApp";
			this.Text = "Install Stream";
			((System.ComponentModel.ISupportInitialize)(this.qrCodePicture)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.storePicture)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.PictureBox qrCodePicture;
		private System.Windows.Forms.PictureBox storePicture;
		private System.Windows.Forms.ProgressBar progress;
		private System.Windows.Forms.Label label3;
	}
}