namespace StationSystemTray
{
	partial class ImportFromPotableMediaControl
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
			this.label1 = new System.Windows.Forms.Label();
			this.deviceCombobox = new System.Windows.Forms.ComboBox();
			this.importButton = new System.Windows.Forms.Button();
			this.progressBar = new System.Windows.Forms.ProgressBar();
			this.progressText = new System.Windows.Forms.Label();
			this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
			this.label2 = new System.Windows.Forms.Label();
			this.dataGridView1 = new System.Windows.Forms.DataGridView();
			this.nameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.photoColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Arial", 11F);
			this.label1.Location = new System.Drawing.Point(30, 17);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(165, 17);
			this.label1.TabIndex = 0;
			this.label1.Text = "Import removable media";
			// 
			// deviceCombobox
			// 
			this.deviceCombobox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.deviceCombobox.Font = new System.Drawing.Font("Arial", 9F);
			this.deviceCombobox.FormattingEnabled = true;
			this.deviceCombobox.Location = new System.Drawing.Point(48, 73);
			this.deviceCombobox.Name = "deviceCombobox";
			this.deviceCombobox.Size = new System.Drawing.Size(338, 23);
			this.deviceCombobox.TabIndex = 1;
			this.deviceCombobox.DropDown += new System.EventHandler(this.deviceCombobox_DropDown);
			// 
			// importButton
			// 
			this.importButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.importButton.Font = new System.Drawing.Font("Arial", 9F);
			this.importButton.Location = new System.Drawing.Point(401, 73);
			this.importButton.Name = "importButton";
			this.importButton.Size = new System.Drawing.Size(112, 23);
			this.importButton.TabIndex = 2;
			this.importButton.Text = "Import";
			this.importButton.UseVisualStyleBackColor = true;
			this.importButton.Click += new System.EventHandler(this.importButton_Click);
			// 
			// progressBar
			// 
			this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.progressBar.Location = new System.Drawing.Point(48, 122);
			this.progressBar.Name = "progressBar";
			this.progressBar.Size = new System.Drawing.Size(465, 23);
			this.progressBar.Step = 1;
			this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			this.progressBar.TabIndex = 3;
			this.progressBar.Value = 30;
			this.progressBar.Visible = false;
			// 
			// progressText
			// 
			this.progressText.AutoSize = true;
			this.progressText.Font = new System.Drawing.Font("Arial", 9F);
			this.progressText.Location = new System.Drawing.Point(48, 103);
			this.progressText.Name = "progressText";
			this.progressText.Size = new System.Drawing.Size(79, 15);
			this.progressText.TabIndex = 4;
			this.progressText.Text = "Processing...";
			this.progressText.Visible = false;
			// 
			// backgroundWorker
			// 
			this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
			this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Arial", 9F);
			this.label2.Location = new System.Drawing.Point(48, 167);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(94, 15);
			this.label2.TabIndex = 5;
			this.label2.Text = "Imported media";
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
			this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.nameColumn,
            this.photoColumn});
			this.dataGridView1.Location = new System.Drawing.Point(48, 195);
			this.dataGridView1.Name = "dataGridView1";
			this.dataGridView1.ReadOnly = true;
			this.dataGridView1.RowHeadersVisible = false;
			this.dataGridView1.Size = new System.Drawing.Size(462, 170);
			this.dataGridView1.TabIndex = 6;
			// 
			// nameColumn
			// 
			this.nameColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.nameColumn.HeaderText = "Name";
			this.nameColumn.MinimumWidth = 50;
			this.nameColumn.Name = "nameColumn";
			this.nameColumn.ReadOnly = true;
			// 
			// photoColumn
			// 
			this.photoColumn.HeaderText = "Photo";
			this.photoColumn.Name = "photoColumn";
			this.photoColumn.ReadOnly = true;
			// 
			// ImportFromPotableMediaControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.Controls.Add(this.dataGridView1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.progressText);
			this.Controls.Add(this.progressBar);
			this.Controls.Add(this.importButton);
			this.Controls.Add(this.deviceCombobox);
			this.Controls.Add(this.label1);
			this.Name = "ImportFromPotableMediaControl";
			this.Size = new System.Drawing.Size(555, 400);
			this.Load += new System.EventHandler(this.ImportFromPotableMediaControl_Load);
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox deviceCombobox;
		private System.Windows.Forms.Button importButton;
		private System.Windows.Forms.ProgressBar progressBar;
		private System.Windows.Forms.Label progressText;
		private System.ComponentModel.BackgroundWorker backgroundWorker;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.DataGridView dataGridView1;
		private System.Windows.Forms.DataGridViewTextBoxColumn nameColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn photoColumn;
	}
}
