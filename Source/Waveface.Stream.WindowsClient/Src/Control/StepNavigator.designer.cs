namespace Waveface.Stream.WindowsClient
{
	partial class StepNavigator
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
			this.centralLayoutPanel1 = new CentralLayoutPanel();
			this.SuspendLayout();
			// 
			// centralLayoutPanel1
			// 
			this.centralLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.centralLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.centralLayoutPanel1.Name = "centralLayoutPanel1";
			this.centralLayoutPanel1.Size = new System.Drawing.Size(344, 50);
			this.centralLayoutPanel1.TabIndex = 0;
			this.centralLayoutPanel1.Text = "centralLayoutPanel1";
			// 
			// StepNavigator
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.centralLayoutPanel1);
			this.Name = "StepNavigator";
			this.Size = new System.Drawing.Size(344, 50);
			this.ResumeLayout(false);

		}

		#endregion

		private CentralLayoutPanel centralLayoutPanel1;
	}
}
