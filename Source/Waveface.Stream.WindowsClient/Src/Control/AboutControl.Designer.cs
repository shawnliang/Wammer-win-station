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
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.checkUpdateButton = new System.Windows.Forms.Button();
			this.lblVersion = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.checkUpdateButton);
			this.groupBox1.Controls.Add(this.lblVersion);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Location = new System.Drawing.Point(3, 3);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(493, 59);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			// 
			// checkUpdateButton
			// 
			this.checkUpdateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkUpdateButton.AutoSize = true;
			this.checkUpdateButton.Location = new System.Drawing.Point(386, 21);
			this.checkUpdateButton.Name = "checkUpdateButton";
			this.checkUpdateButton.Size = new System.Drawing.Size(101, 23);
			this.checkUpdateButton.TabIndex = 2;
			this.checkUpdateButton.Text = "Check for Update";
			this.checkUpdateButton.UseVisualStyleBackColor = true;
			this.checkUpdateButton.Click += new System.EventHandler(this.CheckUpdateButton_Click);
			// 
			// lblVersion
			// 
			this.lblVersion.AutoSize = true;
			this.lblVersion.Location = new System.Drawing.Point(57, 26);
			this.lblVersion.Name = "lblVersion";
			this.lblVersion.Size = new System.Drawing.Size(46, 13);
			this.lblVersion.TabIndex = 1;
			this.lblVersion.Text = "[0.0.0.0]";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(6, 26);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(45, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Version:";
			// 
			// AboutControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.groupBox1);
			this.Name = "AboutControl";
			this.Size = new System.Drawing.Size(499, 320);
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
