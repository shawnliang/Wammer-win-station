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
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
			this.changeButton = new System.Windows.Forms.Button();
			this.txtStoreLocation = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.button2 = new System.Windows.Forms.Button();
			this.button1 = new System.Windows.Forms.Button();
			this.dataGridView1 = new System.Windows.Forms.DataGridView();
			this.chkBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.folderColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.addButton = new System.Windows.Forms.Button();
			this.lblWelcome = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
			this.SuspendLayout();
			// 
			// folderBrowserDialog1
			// 
			this.folderBrowserDialog1.ShowNewFolderButton = false;
			// 
			// changeButton
			// 
			this.changeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.changeButton.Enabled = false;
			this.changeButton.Location = new System.Drawing.Point(421, 349);
			this.changeButton.Name = "changeButton";
			this.changeButton.Size = new System.Drawing.Size(118, 24);
			this.changeButton.TabIndex = 13;
			this.changeButton.Text = "Change";
			this.changeButton.UseVisualStyleBackColor = true;
			// 
			// txtStoreLocation
			// 
			this.txtStoreLocation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.txtStoreLocation.Enabled = false;
			this.txtStoreLocation.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
			this.txtStoreLocation.Location = new System.Drawing.Point(46, 352);
			this.txtStoreLocation.Name = "txtStoreLocation";
			this.txtStoreLocation.Size = new System.Drawing.Size(355, 21);
			this.txtStoreLocation.TabIndex = 12;
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(43, 332);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(220, 12);
			this.label2.TabIndex = 11;
			this.label2.Text = "All imported and synced files will be stored at:";
			// 
			// button2
			// 
			this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
			this.button2.Location = new System.Drawing.Point(421, 128);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(118, 24);
			this.button2.TabIndex = 10;
			this.button2.Text = "Add from Picasa";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// button1
			// 
			this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
			this.button1.Location = new System.Drawing.Point(421, 98);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(118, 24);
			this.button1.TabIndex = 9;
			this.button1.Text = "Add from Library";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click_1);
			// 
			// dataGridView1
			// 
			this.dataGridView1.AllowUserToAddRows = false;
			this.dataGridView1.AllowUserToDeleteRows = false;
			this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
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
			this.dataGridView1.Location = new System.Drawing.Point(45, 68);
			this.dataGridView1.Name = "dataGridView1";
			this.dataGridView1.RowHeadersVisible = false;
			this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dataGridView1.Size = new System.Drawing.Size(356, 224);
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
			// Column1
			// 
			this.Column1.HeaderText = "CollectionName";
			this.Column1.Name = "Column1";
			this.Column1.ReadOnly = true;
			// 
			// folderColumn
			// 
			this.folderColumn.HeaderText = "Location";
			this.folderColumn.Name = "folderColumn";
			this.folderColumn.Width = 313;
			// 
			// addButton
			// 
			this.addButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.addButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
			this.addButton.Location = new System.Drawing.Point(421, 68);
			this.addButton.Name = "addButton";
			this.addButton.Size = new System.Drawing.Size(118, 24);
			this.addButton.TabIndex = 6;
			this.addButton.Text = "Add ...";
			this.addButton.UseVisualStyleBackColor = true;
			this.addButton.Click += new System.EventHandler(this.button1_Click);
			// 
			// lblWelcome
			// 
			this.lblWelcome.AutoSize = true;
			this.lblWelcome.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F);
			this.lblWelcome.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lblWelcome.Location = new System.Drawing.Point(27, 18);
			this.lblWelcome.Name = "lblWelcome";
			this.lblWelcome.Size = new System.Drawing.Size(268, 18);
			this.lblWelcome.TabIndex = 4;
			this.lblWelcome.Text = "Choose existing photo folders to import";
			// 
			// FileImportControl
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.Controls.Add(this.changeButton);
			this.Controls.Add(this.txtStoreLocation);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.dataGridView1);
			this.Controls.Add(this.addButton);
			this.Controls.Add(this.lblWelcome);
			this.Name = "FileImportControl";
			this.Size = new System.Drawing.Size(555, 380);
			this.Load += new System.EventHandler(this.FileImportControl_Load);
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lblWelcome;
		private System.Windows.Forms.Button addButton;
		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
		private System.Windows.Forms.DataGridView dataGridView1;
		private System.Windows.Forms.DataGridViewCheckBoxColumn chkBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
		private System.Windows.Forms.DataGridViewTextBoxColumn folderColumn;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox txtStoreLocation;
		private System.Windows.Forms.Button changeButton;



	}
}
