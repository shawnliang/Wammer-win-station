namespace Waveface.Stream.WindowsClient.Src.Control
{
	partial class AboutControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutControl));
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.checkUpdateButton = new System.Windows.Forms.Button();
			this.lblVersion = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			resources.ApplyResources(this.groupBox1, "groupBox1");
			this.groupBox1.Controls.Add(this.checkUpdateButton);
			this.groupBox1.Controls.Add(this.lblVersion);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.TabStop = false;
			// 
			// checkUpdateButton
			// 
			resources.ApplyResources(this.checkUpdateButton, "checkUpdateButton");
			this.checkUpdateButton.Name = "checkUpdateButton";
			this.checkUpdateButton.UseVisualStyleBackColor = true;
			this.checkUpdateButton.Click += new System.EventHandler(this.CheckUpdateButton_Click);
			// 
			// lblVersion
			// 
			resources.ApplyResources(this.lblVersion, "lblVersion");
			this.lblVersion.Name = "lblVersion";
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// AboutControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.groupBox1);
			this.Name = "AboutControl";
			this.Load += new System.EventHandler(this.AboutControl_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button checkUpdateButton;
		private System.Windows.Forms.Label lblVersion;
		private System.Windows.Forms.Label label1;
	}
}
