namespace Waveface.Stream.WindowsClient
{
	partial class FileImportControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FileImportControl));
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			this.changeButton = new System.Windows.Forms.Button();
			this.txtStoreLocation = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.button2 = new System.Windows.Forms.Button();
			this.button1 = new System.Windows.Forms.Button();
			this.dataGridView1 = new System.Windows.Forms.DataGridView();
			this.addButton = new System.Windows.Forms.Button();
			this.lblWelcome = new System.Windows.Forms.Label();
			this.radioIndexOnly = new System.Windows.Forms.RadioButton();
			this.radioCopy = new System.Windows.Forms.RadioButton();
			this.chkBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.folderColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
			this.SuspendLayout();
			// 
			// changeButton
			// 
			resources.ApplyResources(this.changeButton, "changeButton");
			this.changeButton.Name = "changeButton";
			this.changeButton.UseVisualStyleBackColor = true;
			this.changeButton.Click += new System.EventHandler(this.changeButton_Click);
			// 
			// txtStoreLocation
			// 
			resources.ApplyResources(this.txtStoreLocation, "txtStoreLocation");
			this.txtStoreLocation.Name = "txtStoreLocation";
			this.txtStoreLocation.ReadOnly = true;
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			// 
			// button2
			// 
			resources.ApplyResources(this.button2, "button2");
			this.button2.Name = "button2";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// button1
			// 
			resources.ApplyResources(this.button1, "button1");
			this.button1.Name = "button1";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click_1);
			// 
			// dataGridView1
			// 
			resources.ApplyResources(this.dataGridView1, "dataGridView1");
			this.dataGridView1.AllowUserToAddRows = false;
			this.dataGridView1.AllowUserToDeleteRows = false;
			this.dataGridView1.AllowUserToResizeRows = false;
			this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.dataGridView1.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.chkBoxColumn,
            this.Column1,
            this.folderColumn});
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
			dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
			dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle2;
			this.dataGridView1.Name = "dataGridView1";
			this.dataGridView1.RowHeadersVisible = false;
			this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
			// 
			// addButton
			// 
			resources.ApplyResources(this.addButton, "addButton");
			this.addButton.Name = "addButton";
			this.addButton.UseVisualStyleBackColor = true;
			this.addButton.Click += new System.EventHandler(this.button1_Click);
			// 
			// lblWelcome
			// 
			resources.ApplyResources(this.lblWelcome, "lblWelcome");
			this.lblWelcome.Name = "lblWelcome";
			// 
			// radioIndexOnly
			// 
			resources.ApplyResources(this.radioIndexOnly, "radioIndexOnly");
			this.radioIndexOnly.Name = "radioIndexOnly";
			this.radioIndexOnly.UseVisualStyleBackColor = true;
			// 
			// radioCopy
			// 
			resources.ApplyResources(this.radioCopy, "radioCopy");
			this.radioCopy.Checked = true;
			this.radioCopy.Name = "radioCopy";
			this.radioCopy.TabStop = true;
			this.radioCopy.UseVisualStyleBackColor = true;
			// 
			// chkBoxColumn
			// 
			this.chkBoxColumn.FillWeight = 22.16398F;
			resources.ApplyResources(this.chkBoxColumn, "chkBoxColumn");
			this.chkBoxColumn.Name = "chkBoxColumn";
			this.chkBoxColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			// 
			// Column1
			// 
			this.Column1.FillWeight = 70F;
			resources.ApplyResources(this.Column1, "Column1");
			this.Column1.Name = "Column1";
			this.Column1.ReadOnly = true;
			// 
			// folderColumn
			// 
			this.folderColumn.FillWeight = 149.3538F;
			resources.ApplyResources(this.folderColumn, "folderColumn");
			this.folderColumn.Name = "folderColumn";
			this.folderColumn.ReadOnly = true;
			// 
			// FileImportControl
			// 
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.radioCopy);
			this.Controls.Add(this.radioIndexOnly);
			this.Controls.Add(this.changeButton);
			this.Controls.Add(this.txtStoreLocation);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.dataGridView1);
			this.Controls.Add(this.addButton);
			this.Controls.Add(this.lblWelcome);
			this.Name = "FileImportControl";
			this.Load += new System.EventHandler(this.FileImportControl_Load);
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lblWelcome;
		private System.Windows.Forms.Button addButton;
		private System.Windows.Forms.DataGridView dataGridView1;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox txtStoreLocation;
		private System.Windows.Forms.Button changeButton;
		private System.Windows.Forms.RadioButton radioIndexOnly;
		private System.Windows.Forms.RadioButton radioCopy;
		private System.Windows.Forms.DataGridViewCheckBoxColumn chkBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
		private System.Windows.Forms.DataGridViewTextBoxColumn folderColumn;



	}
}
