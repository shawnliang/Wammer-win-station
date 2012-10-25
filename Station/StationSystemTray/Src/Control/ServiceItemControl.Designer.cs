namespace StationSystemTray.Src.Control
{
	partial class ServiceItemControl
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
			this.onOffSwitch = new System.Windows.Forms.TrackBar();
			this.serviceName = new System.Windows.Forms.Label();
			this.serviceIcon = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.onOffSwitch)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.serviceIcon)).BeginInit();
			this.SuspendLayout();
			// 
			// onOffSwitch
			// 
			this.onOffSwitch.Location = new System.Drawing.Point(401, 32);
			this.onOffSwitch.Maximum = 1;
			this.onOffSwitch.Name = "onOffSwitch";
			this.onOffSwitch.Size = new System.Drawing.Size(70, 45);
			this.onOffSwitch.TabIndex = 10;
			this.onOffSwitch.TickStyle = System.Windows.Forms.TickStyle.Both;
			this.onOffSwitch.Scroll += new System.EventHandler(this.onOffSwitch_Scroll);
			// 
			// serviceName
			// 
			this.serviceName.AutoSize = true;
			this.serviceName.Font = new System.Drawing.Font("Segoe UI", 15.75F);
			this.serviceName.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.serviceName.Location = new System.Drawing.Point(77, 34);
			this.serviceName.Name = "serviceName";
			this.serviceName.Size = new System.Drawing.Size(134, 30);
			this.serviceName.TabIndex = 9;
			this.serviceName.Text = "service name";
			// 
			// serviceIcon
			// 
			this.serviceIcon.Location = new System.Drawing.Point(2, 17);
			this.serviceIcon.Name = "serviceIcon";
			this.serviceIcon.Size = new System.Drawing.Size(60, 60);
			this.serviceIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.serviceIcon.TabIndex = 8;
			this.serviceIcon.TabStop = false;
			// 
			// ServiceItemControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.onOffSwitch);
			this.Controls.Add(this.serviceName);
			this.Controls.Add(this.serviceIcon);
			this.Name = "ServiceItemControl";
			this.Size = new System.Drawing.Size(474, 95);
			((System.ComponentModel.ISupportInitialize)(this.onOffSwitch)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.serviceIcon)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TrackBar onOffSwitch;
		private System.Windows.Forms.Label serviceName;
		private System.Windows.Forms.PictureBox serviceIcon;
	}
}
