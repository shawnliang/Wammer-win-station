namespace Waveface.Stream.WindowsClient
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
			this.svcPanel = new System.Windows.Forms.FlowLayoutPanel();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// svcPanel
			// 
			this.svcPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.svcPanel.AutoScroll = true;
			this.svcPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.svcPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this.svcPanel.Location = new System.Drawing.Point(3, 45);
			this.svcPanel.Name = "svcPanel";
			this.svcPanel.Size = new System.Drawing.Size(547, 295);
			this.svcPanel.TabIndex = 4;
			this.svcPanel.WrapContents = false;
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(5, 352);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(244, 12);
			this.label1.TabIndex = 3;
			this.label1.Text = "You can change theses settings whenever you want.";
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.Location = new System.Drawing.Point(3, 9);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(547, 33);
			this.label2.TabIndex = 2;
			this.label2.Text = "A better way to fill your journal with memories from the past. Connect o all your" +
    " favorite web services and let Stream generate real life events for you.";
			// 
			// ServiceImportControl
			// 
			this.Controls.Add(this.svcPanel);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.label2);
			this.Name = "ServiceImportControl";
			this.Size = new System.Drawing.Size(555, 369);
			this.Load += new System.EventHandler(this.ServiceImportControl_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.FlowLayoutPanel svcPanel;
	}
}
