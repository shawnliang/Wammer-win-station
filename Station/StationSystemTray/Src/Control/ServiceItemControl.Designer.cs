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
			this.serviceName = new System.Windows.Forms.Label();
			this.serviceIcon = new System.Windows.Forms.PictureBox();
			this.connectCheckbox = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this.serviceIcon)).BeginInit();
			this.SuspendLayout();
			// 
			// serviceName
			// 
			this.serviceName.AutoSize = true;
			this.serviceName.Font = new System.Drawing.Font("Segoe UI", 15.75F);
			this.serviceName.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.serviceName.Location = new System.Drawing.Point(69, 24);
			this.serviceName.Name = "serviceName";
			this.serviceName.Size = new System.Drawing.Size(134, 30);
			this.serviceName.TabIndex = 9;
			this.serviceName.Text = "service name";
			// 
			// serviceIcon
			// 
			this.serviceIcon.Location = new System.Drawing.Point(3, 10);
			this.serviceIcon.Name = "serviceIcon";
			this.serviceIcon.Size = new System.Drawing.Size(60, 60);
			this.serviceIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.serviceIcon.TabIndex = 8;
			this.serviceIcon.TabStop = false;
			// 
			// connectCheckbox
			// 
			this.connectCheckbox.Appearance = System.Windows.Forms.Appearance.Button;
			this.connectCheckbox.Checked = true;
			this.connectCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.connectCheckbox.Font = new System.Drawing.Font("Segoe UI", 10F);
			this.connectCheckbox.Location = new System.Drawing.Point(324, 24);
			this.connectCheckbox.Name = "connectCheckbox";
			this.connectCheckbox.Size = new System.Drawing.Size(147, 36);
			this.connectCheckbox.TabIndex = 11;
			this.connectCheckbox.Text = "Connect";
			this.connectCheckbox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.connectCheckbox.UseVisualStyleBackColor = true;
			this.connectCheckbox.CheckedChanged += new System.EventHandler(this.connectCheckbox_CheckedChanged);
			// 
			// ServiceItemControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.connectCheckbox);
			this.Controls.Add(this.serviceName);
			this.Controls.Add(this.serviceIcon);
			this.Name = "ServiceItemControl";
			this.Size = new System.Drawing.Size(474, 75);
			((System.ComponentModel.ISupportInitialize)(this.serviceIcon)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label serviceName;
		private System.Windows.Forms.PictureBox serviceIcon;
		private System.Windows.Forms.CheckBox connectCheckbox;
	}
}
