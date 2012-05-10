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
			this.lblUserEmail = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.button1 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// btnUnlink
			// 
			this.btnUnlink.Enabled = false;
			this.btnUnlink.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.btnUnlink.Location = new System.Drawing.Point(279, 112);
			this.btnUnlink.Name = "btnUnlink";
			this.btnUnlink.Size = new System.Drawing.Size(123, 23);
			this.btnUnlink.TabIndex = 13;
			this.btnUnlink.Text = "Remove this account";
			this.btnUnlink.UseVisualStyleBackColor = true;
			this.btnUnlink.Click += new System.EventHandler(this.btnUnlink_Click);
			// 
			// cmbStations
			// 
			this.cmbStations.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbStations.FormattingEnabled = true;
			this.cmbStations.Location = new System.Drawing.Point(12, 12);
			this.cmbStations.Name = "cmbStations";
			this.cmbStations.Size = new System.Drawing.Size(390, 21);
			this.cmbStations.TabIndex = 8;
			this.cmbStations.SelectedIndexChanged += new System.EventHandler(this.cmbStations_SelectedIndexChanged);
			// 
			// lblUserEmail
			// 
			this.lblUserEmail.AutoSize = true;
			this.lblUserEmail.Location = new System.Drawing.Point(12, 51);
			this.lblUserEmail.Name = "lblUserEmail";
			this.lblUserEmail.Size = new System.Drawing.Size(62, 13);
			this.lblUserEmail.TabIndex = 14;
			this.lblUserEmail.Text = "[User email]";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 78);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(70, 13);
			this.label2.TabIndex = 15;
			this.label2.Text = "[Used space]";
			// 
			// button1
			// 
			this.button1.Enabled = false;
			this.button1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.button1.Location = new System.Drawing.Point(338, 144);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(64, 23);
			this.button1.TabIndex = 16;
			this.button1.Text = "Close";
			this.button1.UseVisualStyleBackColor = true;
			// 
			// LocalSettingDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(426, 179);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.lblUserEmail);
			this.Controls.Add(this.btnUnlink);
			this.Controls.Add(this.cmbStations);
			this.Name = "LocalSettingDialog";
			this.Text = "Local Setting";
			this.Load += new System.EventHandler(this.LocalSettingDialog_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnUnlink;
		private System.Windows.Forms.ComboBox cmbStations;
		private System.Windows.Forms.Label lblUserEmail;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button button1;
	}
}