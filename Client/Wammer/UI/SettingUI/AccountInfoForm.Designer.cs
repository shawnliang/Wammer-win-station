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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AccountInfoForm));
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.tbxName = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.lblEmail = new System.Windows.Forms.Label();
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
			this.btnLogout = new System.Windows.Forms.Button();
			this.button1 = new System.Windows.Forms.Button();
			this.button3 = new System.Windows.Forms.Button();
			this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.tabPage3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
			this.SuspendLayout();
			// 
			// tabControl1
			// 
			resources.ApplyResources(this.tabControl1, "tabControl1");
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Controls.Add(this.tabPage3);
			this.errorProvider1.SetError(this.tabControl1, resources.GetString("tabControl1.Error"));
			this.errorProvider1.SetIconAlignment(this.tabControl1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("tabControl1.IconAlignment"))));
			this.errorProvider1.SetIconPadding(this.tabControl1, ((int)(resources.GetObject("tabControl1.IconPadding"))));
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			// 
			// tabPage1
			// 
			resources.ApplyResources(this.tabPage1, "tabPage1");
			this.tabPage1.Controls.Add(this.checkBox1);
			this.tabPage1.Controls.Add(this.tbxName);
			this.tabPage1.Controls.Add(this.label6);
			this.tabPage1.Controls.Add(this.label4);
			this.tabPage1.Controls.Add(this.lblEmail);
			this.tabPage1.Controls.Add(this.label1);
			this.errorProvider1.SetError(this.tabPage1, resources.GetString("tabPage1.Error"));
			this.errorProvider1.SetIconAlignment(this.tabPage1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("tabPage1.IconAlignment"))));
			this.errorProvider1.SetIconPadding(this.tabPage1, ((int)(resources.GetObject("tabPage1.IconPadding"))));
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// checkBox1
			// 
			resources.ApplyResources(this.checkBox1, "checkBox1");
			this.errorProvider1.SetError(this.checkBox1, resources.GetString("checkBox1.Error"));
			this.errorProvider1.SetIconAlignment(this.checkBox1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("checkBox1.IconAlignment"))));
			this.errorProvider1.SetIconPadding(this.checkBox1, ((int)(resources.GetObject("checkBox1.IconPadding"))));
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.UseVisualStyleBackColor = true;
			// 
			// tbxName
			// 
			resources.ApplyResources(this.tbxName, "tbxName");
			this.tbxName.BackColor = System.Drawing.SystemColors.Window;
			this.errorProvider1.SetError(this.tbxName, resources.GetString("tbxName.Error"));
			this.errorProvider1.SetIconAlignment(this.tbxName, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("tbxName.IconAlignment"))));
			this.errorProvider1.SetIconPadding(this.tbxName, ((int)(resources.GetObject("tbxName.IconPadding"))));
			this.tbxName.Name = "tbxName";
			this.tbxName.TabStop = false;
			this.tbxName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbxName_KeyDown);
			this.tbxName.Validated += new System.EventHandler(this.tbxName_Validated);
			// 
			// label6
			// 
			resources.ApplyResources(this.label6, "label6");
			this.errorProvider1.SetError(this.label6, resources.GetString("label6.Error"));
			this.errorProvider1.SetIconAlignment(this.label6, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label6.IconAlignment"))));
			this.errorProvider1.SetIconPadding(this.label6, ((int)(resources.GetObject("label6.IconPadding"))));
			this.label6.Name = "label6";
			// 
			// label4
			// 
			resources.ApplyResources(this.label4, "label4");
			this.errorProvider1.SetError(this.label4, resources.GetString("label4.Error"));
			this.errorProvider1.SetIconAlignment(this.label4, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label4.IconAlignment"))));
			this.errorProvider1.SetIconPadding(this.label4, ((int)(resources.GetObject("label4.IconPadding"))));
			this.label4.Name = "label4";
			// 
			// lblEmail
			// 
			resources.ApplyResources(this.lblEmail, "lblEmail");
			this.errorProvider1.SetError(this.lblEmail, resources.GetString("lblEmail.Error"));
			this.errorProvider1.SetIconAlignment(this.lblEmail, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("lblEmail.IconAlignment"))));
			this.errorProvider1.SetIconPadding(this.lblEmail, ((int)(resources.GetObject("lblEmail.IconPadding"))));
			this.lblEmail.Name = "lblEmail";
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.errorProvider1.SetError(this.label1, resources.GetString("label1.Error"));
			this.errorProvider1.SetIconAlignment(this.label1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label1.IconAlignment"))));
			this.errorProvider1.SetIconPadding(this.label1, ((int)(resources.GetObject("label1.IconPadding"))));
			this.label1.Name = "label1";
			// 
			// tabPage2
			// 
			resources.ApplyResources(this.tabPage2, "tabPage2");
			this.tabPage2.Controls.Add(this.btnFacebookImport);
			this.tabPage2.Controls.Add(this.lblUploadedPhotoCount);
			this.tabPage2.Controls.Add(this.label12);
			this.tabPage2.Controls.Add(this.lblIsFacebookImportEnabled);
			this.tabPage2.Controls.Add(this.label10);
			this.tabPage2.Controls.Add(this.lblSince);
			this.tabPage2.Controls.Add(this.label8);
			this.errorProvider1.SetError(this.tabPage2, resources.GetString("tabPage2.Error"));
			this.errorProvider1.SetIconAlignment(this.tabPage2, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("tabPage2.IconAlignment"))));
			this.errorProvider1.SetIconPadding(this.tabPage2, ((int)(resources.GetObject("tabPage2.IconPadding"))));
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// btnFacebookImport
			// 
			resources.ApplyResources(this.btnFacebookImport, "btnFacebookImport");
			this.errorProvider1.SetError(this.btnFacebookImport, resources.GetString("btnFacebookImport.Error"));
			this.errorProvider1.SetIconAlignment(this.btnFacebookImport, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("btnFacebookImport.IconAlignment"))));
			this.errorProvider1.SetIconPadding(this.btnFacebookImport, ((int)(resources.GetObject("btnFacebookImport.IconPadding"))));
			this.btnFacebookImport.Name = "btnFacebookImport";
			this.btnFacebookImport.UseVisualStyleBackColor = true;
			this.btnFacebookImport.Click += new System.EventHandler(this.btnFacebookImport_Click);
			// 
			// lblUploadedPhotoCount
			// 
			resources.ApplyResources(this.lblUploadedPhotoCount, "lblUploadedPhotoCount");
			this.errorProvider1.SetError(this.lblUploadedPhotoCount, resources.GetString("lblUploadedPhotoCount.Error"));
			this.errorProvider1.SetIconAlignment(this.lblUploadedPhotoCount, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("lblUploadedPhotoCount.IconAlignment"))));
			this.errorProvider1.SetIconPadding(this.lblUploadedPhotoCount, ((int)(resources.GetObject("lblUploadedPhotoCount.IconPadding"))));
			this.lblUploadedPhotoCount.Name = "lblUploadedPhotoCount";
			// 
			// label12
			// 
			resources.ApplyResources(this.label12, "label12");
			this.errorProvider1.SetError(this.label12, resources.GetString("label12.Error"));
			this.errorProvider1.SetIconAlignment(this.label12, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label12.IconAlignment"))));
			this.errorProvider1.SetIconPadding(this.label12, ((int)(resources.GetObject("label12.IconPadding"))));
			this.label12.Name = "label12";
			// 
			// lblIsFacebookImportEnabled
			// 
			resources.ApplyResources(this.lblIsFacebookImportEnabled, "lblIsFacebookImportEnabled");
			this.errorProvider1.SetError(this.lblIsFacebookImportEnabled, resources.GetString("lblIsFacebookImportEnabled.Error"));
			this.errorProvider1.SetIconAlignment(this.lblIsFacebookImportEnabled, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("lblIsFacebookImportEnabled.IconAlignment"))));
			this.errorProvider1.SetIconPadding(this.lblIsFacebookImportEnabled, ((int)(resources.GetObject("lblIsFacebookImportEnabled.IconPadding"))));
			this.lblIsFacebookImportEnabled.Name = "lblIsFacebookImportEnabled";
			// 
			// label10
			// 
			resources.ApplyResources(this.label10, "label10");
			this.errorProvider1.SetError(this.label10, resources.GetString("label10.Error"));
			this.errorProvider1.SetIconAlignment(this.label10, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label10.IconAlignment"))));
			this.errorProvider1.SetIconPadding(this.label10, ((int)(resources.GetObject("label10.IconPadding"))));
			this.label10.Name = "label10";
			// 
			// lblSince
			// 
			resources.ApplyResources(this.lblSince, "lblSince");
			this.errorProvider1.SetError(this.lblSince, resources.GetString("lblSince.Error"));
			this.errorProvider1.SetIconAlignment(this.lblSince, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("lblSince.IconAlignment"))));
			this.errorProvider1.SetIconPadding(this.lblSince, ((int)(resources.GetObject("lblSince.IconPadding"))));
			this.lblSince.Name = "lblSince";
			// 
			// label8
			// 
			resources.ApplyResources(this.label8, "label8");
			this.errorProvider1.SetError(this.label8, resources.GetString("label8.Error"));
			this.errorProvider1.SetIconAlignment(this.label8, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label8.IconAlignment"))));
			this.errorProvider1.SetIconPadding(this.label8, ((int)(resources.GetObject("label8.IconPadding"))));
			this.label8.Name = "label8";
			// 
			// tabPage3
			// 
			resources.ApplyResources(this.tabPage3, "tabPage3");
			this.tabPage3.Controls.Add(this.dataGridView1);
			this.errorProvider1.SetError(this.tabPage3, resources.GetString("tabPage3.Error"));
			this.errorProvider1.SetIconAlignment(this.tabPage3, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("tabPage3.IconAlignment"))));
			this.errorProvider1.SetIconPadding(this.tabPage3, ((int)(resources.GetObject("tabPage3.IconPadding"))));
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.UseVisualStyleBackColor = true;
			// 
			// dataGridView1
			// 
			resources.ApplyResources(this.dataGridView1, "dataGridView1");
			this.dataGridView1.AllowUserToAddRows = false;
			this.dataGridView1.AllowUserToDeleteRows = false;
			this.dataGridView1.AllowUserToResizeColumns = false;
			this.dataGridView1.AllowUserToResizeRows = false;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.White;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
			this.dataGridView1.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
			this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.dataGridView1.BackgroundColor = System.Drawing.Color.White;
			this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.dataGridView1.CausesValidation = false;
			this.dataGridView1.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
			this.dataGridView1.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle2.Font = new System.Drawing.Font("Tahoma", 9F);
			dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.White;
			dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Black;
			dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
			this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
			this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column3,
            this.Column2});
			dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Window;
			dataGridViewCellStyle6.Font = new System.Drawing.Font("Tahoma", 9F);
			dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.ControlText;
			dataGridViewCellStyle6.SelectionBackColor = System.Drawing.Color.White;
			dataGridViewCellStyle6.SelectionForeColor = System.Drawing.Color.Black;
			dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle6;
			this.dataGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
			this.errorProvider1.SetError(this.dataGridView1, resources.GetString("dataGridView1.Error"));
			this.errorProvider1.SetIconAlignment(this.dataGridView1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("dataGridView1.IconAlignment"))));
			this.errorProvider1.SetIconPadding(this.dataGridView1, ((int)(resources.GetObject("dataGridView1.IconPadding"))));
			this.dataGridView1.MultiSelect = false;
			this.dataGridView1.Name = "dataGridView1";
			dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle7.Font = new System.Drawing.Font("Tahoma", 9F);
			dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle7.SelectionBackColor = System.Drawing.Color.White;
			dataGridViewCellStyle7.SelectionForeColor = System.Drawing.Color.Black;
			dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.dataGridView1.RowHeadersDefaultCellStyle = dataGridViewCellStyle7;
			this.dataGridView1.RowHeadersVisible = false;
			dataGridViewCellStyle8.SelectionBackColor = System.Drawing.Color.White;
			dataGridViewCellStyle8.SelectionForeColor = System.Drawing.Color.Black;
			this.dataGridView1.RowsDefaultCellStyle = dataGridViewCellStyle8;
			this.dataGridView1.RowTemplate.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.White;
			this.dataGridView1.RowTemplate.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.Black;
			this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			// 
			// Column1
			// 
			dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.White;
			dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Black;
			this.Column1.DefaultCellStyle = dataGridViewCellStyle3;
			resources.ApplyResources(this.Column1, "Column1");
			this.Column1.Name = "Column1";
			this.Column1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
			// 
			// Column3
			// 
			dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.White;
			dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.Black;
			this.Column3.DefaultCellStyle = dataGridViewCellStyle4;
			resources.ApplyResources(this.Column3, "Column3");
			this.Column3.Name = "Column3";
			this.Column3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
			// 
			// Column2
			// 
			dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.White;
			dataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.Black;
			this.Column2.DefaultCellStyle = dataGridViewCellStyle5;
			resources.ApplyResources(this.Column2, "Column2");
			this.Column2.Name = "Column2";
			this.Column2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
			// 
			// btnLogout
			// 
			resources.ApplyResources(this.btnLogout, "btnLogout");
			this.errorProvider1.SetError(this.btnLogout, resources.GetString("btnLogout.Error"));
			this.errorProvider1.SetIconAlignment(this.btnLogout, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("btnLogout.IconAlignment"))));
			this.errorProvider1.SetIconPadding(this.btnLogout, ((int)(resources.GetObject("btnLogout.IconPadding"))));
			this.btnLogout.Name = "btnLogout";
			this.btnLogout.UseVisualStyleBackColor = true;
			this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
			// 
			// button1
			// 
			resources.ApplyResources(this.button1, "button1");
			this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.errorProvider1.SetError(this.button1, resources.GetString("button1.Error"));
			this.errorProvider1.SetIconAlignment(this.button1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("button1.IconAlignment"))));
			this.errorProvider1.SetIconPadding(this.button1, ((int)(resources.GetObject("button1.IconPadding"))));
			this.button1.Name = "button1";
			this.button1.UseVisualStyleBackColor = true;
			// 
			// button3
			// 
			resources.ApplyResources(this.button3, "button3");
			this.errorProvider1.SetError(this.button3, resources.GetString("button3.Error"));
			this.errorProvider1.SetIconAlignment(this.button3, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("button3.IconAlignment"))));
			this.errorProvider1.SetIconPadding(this.button3, ((int)(resources.GetObject("button3.IconPadding"))));
			this.button3.Name = "button3";
			this.button3.UseVisualStyleBackColor = true;
			this.button3.Click += new System.EventHandler(this.button3_Click);
			// 
			// errorProvider1
			// 
			this.errorProvider1.ContainerControl = this;
			resources.ApplyResources(this.errorProvider1, "errorProvider1");
			// 
			// AccountInfoForm
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.Controls.Add(this.button3);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.btnLogout);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AccountInfoForm";
			this.ShowInTaskbar = false;
			this.Load += new System.EventHandler(this.AccountInfoForm_Load);
			this.Shown += new System.EventHandler(this.AccountInfoForm_Shown);
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage1.PerformLayout();
			this.tabPage2.ResumeLayout(false);
			this.tabPage2.PerformLayout();
			this.tabPage3.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
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
		private System.Windows.Forms.TextBox tbxName;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.ErrorProvider errorProvider1;
		private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
		private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
		private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
		private System.Windows.Forms.CheckBox checkBox1;

	}
}