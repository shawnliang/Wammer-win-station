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
			this.lblStorageUsageValue = new System.Windows.Forms.Label();
			this.lblLastSyncValue = new System.Windows.Forms.Label();
			this.lblStorageUsage = new System.Windows.Forms.Label();
			this.lblLastSync = new System.Windows.Forms.Label();
			this.cmbStations = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// btnUnlink
			// 
			this.btnUnlink.Enabled = false;
			this.btnUnlink.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btnUnlink.Location = new System.Drawing.Point(303, 88);
			this.btnUnlink.Name = "btnUnlink";
			this.btnUnlink.Size = new System.Drawing.Size(99, 23);
			this.btnUnlink.TabIndex = 13;
			this.btnUnlink.Text = "Unlink this PC";
			this.btnUnlink.UseVisualStyleBackColor = true;
			this.btnUnlink.Visible = false;
			// 
			// lblStorageUsageValue
			// 
			this.lblStorageUsageValue.AutoSize = true;
			this.lblStorageUsageValue.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lblStorageUsageValue.Location = new System.Drawing.Point(96, 62);
			this.lblStorageUsageValue.Name = "lblStorageUsageValue";
			this.lblStorageUsageValue.Size = new System.Drawing.Size(41, 13);
			this.lblStorageUsageValue.TabIndex = 12;
			this.lblStorageUsageValue.Text = "400MB";
			this.lblStorageUsageValue.Visible = false;
			// 
			// lblLastSyncValue
			// 
			this.lblLastSyncValue.AutoSize = true;
			this.lblLastSyncValue.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lblLastSyncValue.Location = new System.Drawing.Point(106, 42);
			this.lblLastSyncValue.Name = "lblLastSyncValue";
			this.lblLastSyncValue.Size = new System.Drawing.Size(58, 13);
			this.lblLastSyncValue.TabIndex = 11;
			this.lblLastSyncValue.Text = "5 mins ago";
			this.lblLastSyncValue.Visible = false;
			// 
			// lblStorageUsage
			// 
			this.lblStorageUsage.AutoSize = true;
			this.lblStorageUsage.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lblStorageUsage.Location = new System.Drawing.Point(9, 62);
			this.lblStorageUsage.Name = "lblStorageUsage";
			this.lblStorageUsage.Size = new System.Drawing.Size(79, 13);
			this.lblStorageUsage.TabIndex = 10;
			this.lblStorageUsage.Text = "Storage usage:";
			this.lblStorageUsage.Visible = false;
			// 
			// lblLastSync
			// 
			this.lblLastSync.AutoSize = true;
			this.lblLastSync.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lblLastSync.Location = new System.Drawing.Point(9, 42);
			this.lblLastSync.Name = "lblLastSync";
			this.lblLastSync.Size = new System.Drawing.Size(89, 13);
			this.lblLastSync.TabIndex = 9;
			this.lblLastSync.Text = "Last synchronize:";
			this.lblLastSync.Visible = false;
			// 
			// cmbStations
			// 
			this.cmbStations.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbStations.FormattingEnabled = true;
			this.cmbStations.Location = new System.Drawing.Point(12, 12);
			this.cmbStations.Name = "cmbStations";
			this.cmbStations.Size = new System.Drawing.Size(390, 21);
			this.cmbStations.TabIndex = 8;
			this.cmbStations.Visible = false;
			// 
			// LocalSettingDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(426, 461);
			this.Controls.Add(this.btnUnlink);
			this.Controls.Add(this.lblStorageUsageValue);
			this.Controls.Add(this.lblLastSyncValue);
			this.Controls.Add(this.lblStorageUsage);
			this.Controls.Add(this.lblLastSync);
			this.Controls.Add(this.cmbStations);
			this.Name = "LocalSettingDialog";
			this.Text = "LocalSettingDialog";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnUnlink;
		private System.Windows.Forms.Label lblStorageUsageValue;
		private System.Windows.Forms.Label lblLastSyncValue;
		private System.Windows.Forms.Label lblStorageUsage;
		private System.Windows.Forms.Label lblLastSync;
		private System.Windows.Forms.ComboBox cmbStations;
	}
}