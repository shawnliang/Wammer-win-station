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
			this.radioIndexOnly = new System.Windows.Forms.RadioButton();
			this.radioCopy = new System.Windows.Forms.RadioButton();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
			this.SuspendLayout();
			// 
			// changeButton
			// 
			this.changeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.changeButton.Location = new System.Drawing.Point(407, 356);
			this.changeButton.Name = "changeButton";
			this.changeButton.Size = new System.Drawing.Size(67, 24);
			this.changeButton.TabIndex = 13;
			this.changeButton.Text = "Change";
			this.changeButton.UseVisualStyleBackColor = true;
			this.changeButton.Click += new System.EventHandler(this.changeButton_Click);
			// 
			// txtStoreLocation
			// 
			this.txtStoreLocation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtStoreLocation.Enabled = false;
			this.txtStoreLocation.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
			this.txtStoreLocation.Location = new System.Drawing.Point(30, 357);
			this.txtStoreLocation.Name = "txtStoreLocation";
			this.txtStoreLocation.ReadOnly = true;
			this.txtStoreLocation.Size = new System.Drawing.Size(371, 21);
			this.txtStoreLocation.TabIndex = 12;
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(41, 318);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(0, 13);
			this.label2.TabIndex = 11;
			// 
			// button2
			// 
			this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
			this.button2.Location = new System.Drawing.Point(468, 92);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(145, 24);
			this.button2.TabIndex = 10;
			this.button2.Text = "Add from Picasa";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// button1
			// 
			this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
			this.button1.Location = new System.Drawing.Point(468, 62);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(145, 24);
			this.button1.TabIndex = 9;
			this.button1.Text = "Add from Library";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click_1);
			// 
			// dataGridView1
			// 
			this.dataGridView1.AllowUserToAddRows = false;
			this.dataGridView1.AllowUserToDeleteRows = false;
			this.dataGridView1.AllowUserToResizeRows = false;
			this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
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
			this.dataGridView1.Location = new System.Drawing.Point(15, 32);
			this.dataGridView1.Name = "dataGridView1";
			this.dataGridView1.RowHeadersVisible = false;
			this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dataGridView1.Size = new System.Drawing.Size(444, 261);
			this.dataGridView1.TabIndex = 8;
			this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
			// 
			// chkBoxColumn
			// 
			this.chkBoxColumn.FillWeight = 22.16398F;
			this.chkBoxColumn.HeaderText = "";
			this.chkBoxColumn.MinimumWidth = 14;
			this.chkBoxColumn.Name = "chkBoxColumn";
			this.chkBoxColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			// 
			// Column1
			// 
			this.Column1.FillWeight = 70F;
			this.Column1.HeaderText = "CollectionName";
			this.Column1.Name = "Column1";
			this.Column1.ReadOnly = true;
			// 
			// folderColumn
			// 
			this.folderColumn.FillWeight = 149.3538F;
			this.folderColumn.HeaderText = "Location";
			this.folderColumn.Name = "folderColumn";
			this.folderColumn.ReadOnly = true;
			// 
			// addButton
			// 
			this.addButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.addButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
			this.addButton.Location = new System.Drawing.Point(468, 32);
			this.addButton.Name = "addButton";
			this.addButton.Size = new System.Drawing.Size(145, 24);
			this.addButton.TabIndex = 6;
			this.addButton.Text = "Add ...";
			this.addButton.UseVisualStyleBackColor = true;
			this.addButton.Click += new System.EventHandler(this.button1_Click);
			// 
			// lblWelcome
			// 
			this.lblWelcome.AutoSize = true;
			this.lblWelcome.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.lblWelcome.Location = new System.Drawing.Point(13, 9);
			this.lblWelcome.Name = "lblWelcome";
			this.lblWelcome.Size = new System.Drawing.Size(188, 13);
			this.lblWelcome.TabIndex = 4;
			this.lblWelcome.Text = "Choose existing photo folders to import";
			// 
			// radioIndexOnly
			// 
			this.radioIndexOnly.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.radioIndexOnly.AutoSize = true;
			this.radioIndexOnly.Location = new System.Drawing.Point(15, 313);
			this.radioIndexOnly.Name = "radioIndexOnly";
			this.radioIndexOnly.Size = new System.Drawing.Size(184, 17);
			this.radioIndexOnly.TabIndex = 14;
			this.radioIndexOnly.Text = "Index and generate previews only";
			this.radioIndexOnly.UseVisualStyleBackColor = true;
			// 
			// radioCopy
			// 
			this.radioCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.radioCopy.AutoSize = true;
			this.radioCopy.Checked = true;
			this.radioCopy.Location = new System.Drawing.Point(15, 336);
			this.radioCopy.Name = "radioCopy";
			this.radioCopy.Size = new System.Drawing.Size(187, 17);
			this.radioCopy.TabIndex = 15;
			this.radioCopy.TabStop = true;
			this.radioCopy.Text = "Copy imported files to this location:";
			this.radioCopy.UseVisualStyleBackColor = true;
			// 
			// FileImportControl
			// 
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
			this.Size = new System.Drawing.Size(625, 387);
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
		private System.Windows.Forms.DataGridViewCheckBoxColumn chkBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
		private System.Windows.Forms.DataGridViewTextBoxColumn folderColumn;
		private System.Windows.Forms.RadioButton radioIndexOnly;
		private System.Windows.Forms.RadioButton radioCopy;



	}
}
