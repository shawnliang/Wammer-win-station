namespace Waveface.Stream.WindowsClient
{
	partial class UsbImportDialog
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
			Waveface.Stream.WindowsClient.NullPortableMediaService nullPortableMediaService1 = new Waveface.Stream.WindowsClient.NullPortableMediaService();
			this.importControl = new Waveface.Stream.WindowsClient.ImportFromPotableMediaControl();
			this.SuspendLayout();
			// 
			// importControl
			// 
			this.importControl.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.importControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.importControl.Location = new System.Drawing.Point(0, 0);
			this.importControl.Name = "importControl";
			this.importControl.Service = nullPortableMediaService1;
			this.importControl.Size = new System.Drawing.Size(536, 431);
			this.importControl.TabIndex = 0;
			// 
			// UsbImportDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(536, 431);
			this.Controls.Add(this.importControl);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "UsbImportDialog";
			this.Text = "Import to aostream";
			this.Load += new System.EventHandler(this.UsbImportDialog_Load);
			this.Shown += new System.EventHandler(this.UsbImportDialog_Shown);
			this.ResumeLayout(false);

		}

		#endregion

		private ImportFromPotableMediaControl importControl;

	}
}