namespace Waveface.Stream.WindowsClient.Src.Control
{
	partial class DeviceNameControl
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
			this.deviceNameLabel = new System.Windows.Forms.Label();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// deviceNameLabel
			// 
			this.deviceNameLabel.AutoSize = true;
			this.deviceNameLabel.Location = new System.Drawing.Point(36, 11);
			this.deviceNameLabel.Name = "deviceNameLabel";
			this.deviceNameLabel.Size = new System.Drawing.Size(72, 12);
			this.deviceNameLabel.TabIndex = 0;
			this.deviceNameLabel.Text = "[DeviceName]";
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = global::Waveface.Stream.WindowsClient.Properties.Resources.winControl2;
			this.pictureBox1.Location = new System.Drawing.Point(7, 4);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(23, 27);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.pictureBox1.TabIndex = 1;
			this.pictureBox1.TabStop = false;
			// 
			// DeviceNameControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.deviceNameLabel);
			this.Name = "DeviceNameControl";
			this.Size = new System.Drawing.Size(119, 36);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label deviceNameLabel;
		private System.Windows.Forms.PictureBox pictureBox1;


	}
}
