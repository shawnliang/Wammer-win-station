namespace StationSystemTray
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
			this.lblWelcome = new System.Windows.Forms.Label();
			this.button1 = new System.Windows.Forms.Button();
			this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
			this.dataGridView1 = new System.Windows.Forms.DataGridView();
			this.chkBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.folderColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.photoCountColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
			this.SuspendLayout();
			// 
			// lblWelcome
			// 
			this.lblWelcome.AutoSize = true;
			this.lblWelcome.Font = new System.Drawing.Font("Segoe UI", 15.75F);
			this.lblWelcome.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lblWelcome.Location = new System.Drawing.Point(6, 9);
			this.lblWelcome.Name = "lblWelcome";
			this.lblWelcome.Size = new System.Drawing.Size(165, 30);
			this.lblWelcome.TabIndex = 4;
			this.lblWelcome.Text = "Import your files";
			// 
			// button1
			// 
			this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button1.Location = new System.Drawing.Point(365, 26);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(31, 23);
			this.button1.TabIndex = 6;
			this.button1.Text = "＋";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// folderBrowserDialog1
			// 
			this.folderBrowserDialog1.ShowNewFolderButton = false;
			// 
			// dataGridView1
			// 
			this.dataGridView1.AllowUserToAddRows = false;
			this.dataGridView1.AllowUserToDeleteRows = false;
			this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
			this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.chkBoxColumn,
            this.folderColumn,
            this.photoCountColumn});
			this.dataGridView1.Location = new System.Drawing.Point(11, 60);
			this.dataGridView1.Name = "dataGridView1";
			this.dataGridView1.RowHeadersVisible = false;
			this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dataGridView1.Size = new System.Drawing.Size(385, 167);
			this.dataGridView1.TabIndex = 8;
			this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
			// 
			// chkBoxColumn
			// 
			this.chkBoxColumn.HeaderText = "";
			this.chkBoxColumn.Name = "chkBoxColumn";
			this.chkBoxColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.chkBoxColumn.Width = 50;
			// 
			// folderColumn
			// 
			this.folderColumn.HeaderText = "Folder";
			this.folderColumn.Name = "folderColumn";
			this.folderColumn.Width = 200;
			// 
			// photoCountColumn
			// 
			this.photoCountColumn.HeaderText = "Photo Count";
			this.photoCountColumn.Name = "photoCountColumn";
			// 
			// FileImportControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.Controls.Add(this.dataGridView1);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.lblWelcome);
			this.Name = "FileImportControl";
			this.Size = new System.Drawing.Size(410, 271);
			this.Load += new System.EventHandler(this.FileImportControl_Load);
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lblWelcome;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
		private System.Windows.Forms.DataGridView dataGridView1;
		private System.Windows.Forms.DataGridViewCheckBoxColumn chkBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn folderColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn photoCountColumn;



	}
}
