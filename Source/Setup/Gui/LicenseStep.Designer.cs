namespace Gui
{
	partial class LicenseStep
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LicenseStep));
			this.rtbLicense = new System.Windows.Forms.RichTextBox();
			this.cbAccept = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// rtbLicense
			// 
			resources.ApplyResources(this.rtbLicense, "rtbLicense");
			this.rtbLicense.BackColor = System.Drawing.SystemColors.Window;
			this.rtbLicense.Name = "rtbLicense";
			this.rtbLicense.ReadOnly = true;
			// 
			// cbAccept
			// 
			resources.ApplyResources(this.cbAccept, "cbAccept");
			this.cbAccept.Name = "cbAccept";
			this.cbAccept.CheckedChanged += new System.EventHandler(this.cbAccept_CheckedChanged);
			// 
			// LicenseStep
			// 
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.rtbLicense);
			this.Controls.Add(this.cbAccept);
			this.Name = "LicenseStep";
			this.Entered += new System.EventHandler<System.EventArgs>(this.cbAccept_CheckedChanged);
			this.Load += new System.EventHandler(this.LicenseStep_Load);
			this.Controls.SetChildIndex(this.cbAccept, 0);
			this.Controls.SetChildIndex(this.rtbLicense, 0);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.RichTextBox rtbLicense;
		private System.Windows.Forms.CheckBox cbAccept;
	}
}
