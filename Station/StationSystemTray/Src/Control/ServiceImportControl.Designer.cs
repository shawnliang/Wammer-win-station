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
			this.lblWelcome = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// lblWelcome
			// 
			this.lblWelcome.AutoSize = true;
			this.lblWelcome.Font = new System.Drawing.Font("Segoe UI", 15.75F);
			this.lblWelcome.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lblWelcome.Location = new System.Drawing.Point(20, 21);
			this.lblWelcome.Name = "lblWelcome";
			this.lblWelcome.Size = new System.Drawing.Size(268, 30);
			this.lblWelcome.TabIndex = 3;
			this.lblWelcome.Text = "Connect with other services";
			// 
			// label2
			// 
			this.label2.Font = new System.Drawing.Font("Segoe UI", 10F);
			this.label2.Location = new System.Drawing.Point(69, 91);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(536, 62);
			this.label2.TabIndex = 6;
			this.label2.Text = "Lorem ipsum sapien ultrices varius nec tempus gravida, etiam tempor id fusce maec" +
    "enas torquent, porttitor aliquam morbi taciti tellus suspendisse.";
			// 
			// ServiceImportControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Name = "ServiceImportControl";
			this.Size = new System.Drawing.Size(612, 393);
			this.Load += new System.EventHandler(this.ServiceImportControl_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lblWelcome;
		private System.Windows.Forms.Label label2;
		private Src.Control.ServiceItemControl serviceItemControl1;



	}
}
