namespace Waveface.Stream.WindowsClient
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServiceItemControl));
            this.serviceName = new System.Windows.Forms.Label();
            this.serviceIcon = new System.Windows.Forms.PictureBox();
            this.connectCheckbox = new System.Windows.Forms.CheckBox();
            this.description = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.serviceIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // serviceName
            // 
            resources.ApplyResources(this.serviceName, "serviceName");
            this.serviceName.Name = "serviceName";
            // 
            // serviceIcon
            // 
            resources.ApplyResources(this.serviceIcon, "serviceIcon");
            this.serviceIcon.Name = "serviceIcon";
            this.serviceIcon.TabStop = false;
            // 
            // connectCheckbox
            // 
            resources.ApplyResources(this.connectCheckbox, "connectCheckbox");
            this.connectCheckbox.Name = "connectCheckbox";
            this.connectCheckbox.UseVisualStyleBackColor = true;
            this.connectCheckbox.CheckedChanged += new System.EventHandler(this.connectCheckbox_CheckedChanged);
            this.connectCheckbox.Click += new System.EventHandler(this.connectCheckbox_Click);
            // 
            // description
            // 
            resources.ApplyResources(this.description, "description");
            this.description.Name = "description";
            // 
            // ServiceItemControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.description);
            this.Controls.Add(this.connectCheckbox);
            this.Controls.Add(this.serviceName);
            this.Controls.Add(this.serviceIcon);
            this.Name = "ServiceItemControl";
            ((System.ComponentModel.ISupportInitialize)(this.serviceIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label serviceName;
		private System.Windows.Forms.PictureBox serviceIcon;
		private System.Windows.Forms.CheckBox connectCheckbox;
		private System.Windows.Forms.Label description;
	}
}
