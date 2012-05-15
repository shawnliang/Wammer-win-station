namespace StationSystemTray
{
	partial class SettingDialog
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingDialog));
			this.btnUnlink = new System.Windows.Forms.Button();
			this.cmbStations = new System.Windows.Forms.ComboBox();
			this.lblUserEmail = new System.Windows.Forms.Label();
			this.button1 = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.lblStorageUsageValue = new System.Windows.Forms.Label();
			this.lblStorageUsage = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnUnlink
			// 
			resources.ApplyResources(this.btnUnlink, "btnUnlink");
			this.btnUnlink.Name = "btnUnlink";
			this.btnUnlink.UseVisualStyleBackColor = true;
			this.btnUnlink.Click += new System.EventHandler(this.btnUnlink_Click);
			// 
			// cmbStations
			// 
			resources.ApplyResources(this.cmbStations, "cmbStations");
			this.cmbStations.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbStations.FormattingEnabled = true;
			this.cmbStations.Name = "cmbStations";
			this.cmbStations.SelectedIndexChanged += new System.EventHandler(this.cmbStations_SelectedIndexChanged);
			this.cmbStations.TextChanged += new System.EventHandler(this.cmbStations_TextChanged);
			// 
			// lblUserEmail
			// 
			resources.ApplyResources(this.lblUserEmail, "lblUserEmail");
			this.lblUserEmail.Name = "lblUserEmail";
			// 
			// button1
			// 
			resources.ApplyResources(this.button1, "button1");
			this.button1.Name = "button1";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// groupBox1
			// 
			resources.ApplyResources(this.groupBox1, "groupBox1");
			this.groupBox1.Controls.Add(this.lblStorageUsageValue);
			this.groupBox1.Controls.Add(this.lblStorageUsage);
			this.groupBox1.Controls.Add(this.cmbStations);
			this.groupBox1.Controls.Add(this.btnUnlink);
			this.groupBox1.Controls.Add(this.lblUserEmail);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.TabStop = false;
			// 
			// lblStorageUsageValue
			// 
			resources.ApplyResources(this.lblStorageUsageValue, "lblStorageUsageValue");
			this.lblStorageUsageValue.Name = "lblStorageUsageValue";
			// 
			// lblStorageUsage
			// 
			resources.ApplyResources(this.lblStorageUsage, "lblStorageUsage");
			this.lblStorageUsage.Name = "lblStorageUsage";
			// 
			// SettingDialog
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.button1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SettingDialog";
			this.Load += new System.EventHandler(this.LocalSettingDialog_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btnUnlink;
		private System.Windows.Forms.ComboBox cmbStations;
		private System.Windows.Forms.Label lblUserEmail;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label lblStorageUsageValue;
		private System.Windows.Forms.Label lblStorageUsage;
	}
}