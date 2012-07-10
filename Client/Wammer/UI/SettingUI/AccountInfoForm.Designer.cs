namespace Waveface
{
    partial class AccountInfoForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AccountInfoForm));
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.tbxName = new System.Windows.Forms.TextBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.rbtnUnSubscribed = new System.Windows.Forms.RadioButton();
			this.rbtnSubscribed = new System.Windows.Forms.RadioButton();
			this.button2 = new System.Windows.Forms.Button();
			this.label6 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.lblEmail = new System.Windows.Forms.Label();
			this.btnLogout = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.btnFacebookImport = new System.Windows.Forms.Button();
			this.lblUploadedPhotoCount = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.lblIsFacebookImportEnabled = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.lblSince = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.tabPage3 = new System.Windows.Forms.TabPage();
			this.dataGridView1 = new System.Windows.Forms.DataGridView();
			this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.panel1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.tabPage3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
			this.SuspendLayout();
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Controls.Add(this.tabPage3);
			resources.ApplyResources(this.tabControl1, "tabControl1");
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.tbxName);
			this.tabPage1.Controls.Add(this.panel1);
			this.tabPage1.Controls.Add(this.button2);
			this.tabPage1.Controls.Add(this.label6);
			this.tabPage1.Controls.Add(this.label4);
			this.tabPage1.Controls.Add(this.lblEmail);
			this.tabPage1.Controls.Add(this.btnLogout);
			this.tabPage1.Controls.Add(this.label1);
			resources.ApplyResources(this.tabPage1, "tabPage1");
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// tbxName
			// 
			resources.ApplyResources(this.tbxName, "tbxName");
			this.tbxName.BackColor = System.Drawing.SystemColors.Window;
			this.tbxName.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.tbxName.Name = "tbxName";
			this.tbxName.ReadOnly = true;
			this.tbxName.TabStop = false;
			this.tbxName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbxName_KeyDown);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.rbtnUnSubscribed);
			this.panel1.Controls.Add(this.rbtnSubscribed);
			resources.ApplyResources(this.panel1, "panel1");
			this.panel1.Name = "panel1";
			// 
			// rbtnUnSubscribed
			// 
			resources.ApplyResources(this.rbtnUnSubscribed, "rbtnUnSubscribed");
			this.rbtnUnSubscribed.Name = "rbtnUnSubscribed";
			this.rbtnUnSubscribed.TabStop = true;
			this.rbtnUnSubscribed.UseVisualStyleBackColor = true;
			this.rbtnUnSubscribed.CheckedChanged += new System.EventHandler(this.rbtnSubscribed_CheckedChanged);
			// 
			// rbtnSubscribed
			// 
			resources.ApplyResources(this.rbtnSubscribed, "rbtnSubscribed");
			this.rbtnSubscribed.Name = "rbtnSubscribed";
			this.rbtnSubscribed.TabStop = true;
			this.rbtnSubscribed.UseVisualStyleBackColor = true;
			this.rbtnSubscribed.CheckedChanged += new System.EventHandler(this.rbtnSubscribed_CheckedChanged);
			// 
			// button2
			// 
			resources.ApplyResources(this.button2, "button2");
			this.button2.Name = "button2";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// label6
			// 
			resources.ApplyResources(this.label6, "label6");
			this.label6.Name = "label6";
			// 
			// label4
			// 
			resources.ApplyResources(this.label4, "label4");
			this.label4.Name = "label4";
			// 
			// lblEmail
			// 
			resources.ApplyResources(this.lblEmail, "lblEmail");
			this.lblEmail.Name = "lblEmail";
			// 
			// btnLogout
			// 
			resources.ApplyResources(this.btnLogout, "btnLogout");
			this.btnLogout.Name = "btnLogout";
			this.btnLogout.UseVisualStyleBackColor = true;
			this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.btnFacebookImport);
			this.tabPage2.Controls.Add(this.lblUploadedPhotoCount);
			this.tabPage2.Controls.Add(this.label12);
			this.tabPage2.Controls.Add(this.lblIsFacebookImportEnabled);
			this.tabPage2.Controls.Add(this.label10);
			this.tabPage2.Controls.Add(this.lblSince);
			this.tabPage2.Controls.Add(this.label8);
			resources.ApplyResources(this.tabPage2, "tabPage2");
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// btnFacebookImport
			// 
			resources.ApplyResources(this.btnFacebookImport, "btnFacebookImport");
			this.btnFacebookImport.Name = "btnFacebookImport";
			this.btnFacebookImport.UseVisualStyleBackColor = true;
			this.btnFacebookImport.Click += new System.EventHandler(this.btnFacebookImport_Click);
			// 
			// lblUploadedPhotoCount
			// 
			resources.ApplyResources(this.lblUploadedPhotoCount, "lblUploadedPhotoCount");
			this.lblUploadedPhotoCount.Name = "lblUploadedPhotoCount";
			// 
			// label12
			// 
			resources.ApplyResources(this.label12, "label12");
			this.label12.Name = "label12";
			// 
			// lblIsFacebookImportEnabled
			// 
			resources.ApplyResources(this.lblIsFacebookImportEnabled, "lblIsFacebookImportEnabled");
			this.lblIsFacebookImportEnabled.Name = "lblIsFacebookImportEnabled";
			// 
			// label10
			// 
			resources.ApplyResources(this.label10, "label10");
			this.label10.Name = "label10";
			// 
			// lblSince
			// 
			resources.ApplyResources(this.lblSince, "lblSince");
			this.lblSince.Name = "lblSince";
			// 
			// label8
			// 
			resources.ApplyResources(this.label8, "label8");
			this.label8.Name = "label8";
			// 
			// tabPage3
			// 
			this.tabPage3.Controls.Add(this.dataGridView1);
			resources.ApplyResources(this.tabPage3, "tabPage3");
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.UseVisualStyleBackColor = true;
			// 
			// dataGridView1
			// 
			this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.dataGridView1.BackgroundColor = System.Drawing.Color.White;
			this.dataGridView1.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
			this.dataGridView1.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
			this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
			this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column3,
            this.Column2});
			resources.ApplyResources(this.dataGridView1, "dataGridView1");
			this.dataGridView1.MultiSelect = false;
			this.dataGridView1.Name = "dataGridView1";
			this.dataGridView1.RowHeadersVisible = false;
			this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			// 
			// Column1
			// 
			resources.ApplyResources(this.Column1, "Column1");
			this.Column1.Name = "Column1";
			// 
			// Column3
			// 
			resources.ApplyResources(this.Column3, "Column3");
			this.Column3.Name = "Column3";
			// 
			// Column2
			// 
			resources.ApplyResources(this.Column2, "Column2");
			this.Column2.Name = "Column2";
			// 
			// AccountInfoForm
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.Controls.Add(this.tabControl1);
			this.Name = "AccountInfoForm";
			this.ShowInTaskbar = false;
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage1.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.tabPage2.ResumeLayout(false);
			this.tabPage2.PerformLayout();
			this.tabPage3.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label lblEmail;
		private System.Windows.Forms.Button btnLogout;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.Button btnFacebookImport;
		private System.Windows.Forms.Label lblUploadedPhotoCount;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label lblIsFacebookImportEnabled;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label lblSince;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TabPage tabPage3;
		private System.Windows.Forms.DataGridView dataGridView1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.RadioButton rbtnUnSubscribed;
		private System.Windows.Forms.RadioButton rbtnSubscribed;
		private System.Windows.Forms.TextBox tbxName;
		private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
		private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
		private System.Windows.Forms.DataGridViewTextBoxColumn Column2;

	}
}