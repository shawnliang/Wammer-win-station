namespace Waveface.Stream.WindowsClient
{
	partial class CleanResourceDialog
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CleanResourceDialog));
			this.btnYes = new System.Windows.Forms.Button();
			this.btnNo = new System.Windows.Forms.Button();
			this.lblConfirm = new System.Windows.Forms.Label();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// btnYes
			// 
			resources.ApplyResources(this.btnYes, "btnYes");
			this.btnYes.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.btnYes.DialogResult = System.Windows.Forms.DialogResult.Yes;
			this.btnYes.Name = "btnYes";
			this.btnYes.UseVisualStyleBackColor = false;
			this.btnYes.Click += new System.EventHandler(this.btnYes_Click);
			// 
			// btnNo
			// 
			resources.ApplyResources(this.btnNo, "btnNo");
			this.btnNo.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.btnNo.DialogResult = System.Windows.Forms.DialogResult.No;
			this.btnNo.Name = "btnNo";
			this.btnNo.UseVisualStyleBackColor = false;
			// 
			// lblConfirm
			// 
			resources.ApplyResources(this.lblConfirm, "lblConfirm");
			this.lblConfirm.Name = "lblConfirm";
			// 
			// pictureBox1
			// 
			resources.ApplyResources(this.pictureBox1, "pictureBox1");
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.TabStop = false;
			// 
			// checkBox1
			// 
			resources.ApplyResources(this.checkBox1, "checkBox1");
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.UseVisualStyleBackColor = true;
			// 
			// CleanResourceDialog
			// 
			this.AcceptButton = this.btnYes;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(225)))), ((int)(((byte)(225)))));
			this.CancelButton = this.btnNo;
			this.Controls.Add(this.checkBox1);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.lblConfirm);
			this.Controls.Add(this.btnNo);
			this.Controls.Add(this.btnYes);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "CleanResourceDialog";
			this.ShowIcon = false;
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnYes;
		private System.Windows.Forms.Button btnNo;
		private System.Windows.Forms.Label lblConfirm;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.CheckBox checkBox1;
	}
}