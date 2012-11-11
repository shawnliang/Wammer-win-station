namespace StationSystemTray
{
	partial class HidableProgressingDialog
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
			this.goBackgroundButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Size = new System.Drawing.Size(345, 23);
			// 
			// progressBar1
			// 
			this.progressBar1.Size = new System.Drawing.Size(345, 23);
			// 
			// goBackgroundButton
			// 
			this.goBackgroundButton.Location = new System.Drawing.Point(126, 80);
			this.goBackgroundButton.Name = "goBackgroundButton";
			this.goBackgroundButton.Size = new System.Drawing.Size(116, 34);
			this.goBackgroundButton.TabIndex = 4;
			this.goBackgroundButton.Text = "Hide";
			this.goBackgroundButton.UseVisualStyleBackColor = true;
			this.goBackgroundButton.Click += new System.EventHandler(this.goBackgroundButton_Click);
			// 
			// HidableProgressingDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(369, 136);
			this.Controls.Add(this.goBackgroundButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "HidableProgressingDialog";
			this.ShowProcessMessage = true;
			this.Text = "Progress";
			this.Shown += new System.EventHandler(this.HidableProgressingDialog_Shown);
			this.Controls.SetChildIndex(this.goBackgroundButton, 0);
			this.Controls.SetChildIndex(this.progressBar1, 0);
			this.Controls.SetChildIndex(this.label1, 0);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button goBackgroundButton;
	}
}