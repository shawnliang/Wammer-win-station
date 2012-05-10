namespace StationSystemTray
{
	partial class LocalSettingDialog
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
			this.btnUnlink = new System.Windows.Forms.Button();
			this.cmbStations = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// btnUnlink
			// 
			this.btnUnlink.Enabled = false;
			this.btnUnlink.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btnUnlink.Location = new System.Drawing.Point(303, 54);
			this.btnUnlink.Name = "btnUnlink";
			this.btnUnlink.Size = new System.Drawing.Size(99, 23);
			this.btnUnlink.TabIndex = 13;
			this.btnUnlink.Text = "Remove account";
			this.btnUnlink.UseVisualStyleBackColor = true;
			this.btnUnlink.Visible = false;
			// 
			// cmbStations
			// 
			this.cmbStations.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbStations.FormattingEnabled = true;
			this.cmbStations.Location = new System.Drawing.Point(12, 12);
			this.cmbStations.Name = "cmbStations";
			this.cmbStations.Size = new System.Drawing.Size(390, 21);
			this.cmbStations.TabIndex = 8;
			// 
			// LocalSettingDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(426, 461);
			this.Controls.Add(this.btnUnlink);
			this.Controls.Add(this.cmbStations);
			this.Name = "LocalSettingDialog";
			this.Text = "LocalSettingDialog";
			this.Load += new System.EventHandler(this.LocalSettingDialog_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btnUnlink;
		private System.Windows.Forms.ComboBox cmbStations;
	}
}