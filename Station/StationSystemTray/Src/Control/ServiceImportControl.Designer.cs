namespace StationSystemTray
{
	partial class ServiceImportControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServiceImportControl));
			this.svcItem = new StationSystemTray.Src.Control.ServiceItemControl();
			this.lbTitle = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// svcItem
			// 
			this.svcItem.Location = new System.Drawing.Point(45, 154);
			this.svcItem.Name = "svcItem";
			this.svcItem.ServiceEnabled = false;
			this.svcItem.ServiceIcon = null;
			this.svcItem.ServiceName = "service name";
			this.svcItem.Size = new System.Drawing.Size(474, 95);
			this.svcItem.TabIndex = 0;
			// 
			// lbTitle
			// 
			this.lbTitle.AutoSize = true;
			this.lbTitle.Font = new System.Drawing.Font("Arial", 19.5F);
			this.lbTitle.Location = new System.Drawing.Point(21, 31);
			this.lbTitle.Name = "lbTitle";
			this.lbTitle.Size = new System.Drawing.Size(268, 31);
			this.lbTitle.TabIndex = 1;
			this.lbTitle.Text = "Connect with Services";
			// 
			// label2
			// 
			this.label2.Font = new System.Drawing.Font("Arial", 10F);
			this.label2.Location = new System.Drawing.Point(42, 80);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(477, 57);
			this.label2.TabIndex = 2;
			this.label2.Text = resources.GetString("label2.Text");
			// 
			// ServiceImportControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.label2);
			this.Controls.Add(this.lbTitle);
			this.Controls.Add(this.svcItem);
			this.Name = "ServiceImportControl";
			this.Size = new System.Drawing.Size(552, 343);
			this.Load += new System.EventHandler(this.ServiceImportControl_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private Src.Control.ServiceItemControl svcItem;
		private System.Windows.Forms.Label lbTitle;
		private System.Windows.Forms.Label label2;
	}
}
