namespace StationSystemTray
{
	partial class PersonalCloudStatusControl
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
			this.components = new System.ComponentModel.Container();
			this.title = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.deviceGridView = new System.Windows.Forms.DataGridView();
			this.nameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.typeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.statusColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.photoCount = new System.Windows.Forms.Label();
			this.eventCount = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.linkLabel1 = new System.Windows.Forms.LinkLabel();
			this.label7 = new System.Windows.Forms.Label();
			this.linkLabel2 = new System.Windows.Forms.LinkLabel();
			this.label8 = new System.Windows.Forms.Label();
			this.timer = new System.Windows.Forms.Timer(this.components);
			((System.ComponentModel.ISupportInitialize)(this.deviceGridView)).BeginInit();
			this.SuspendLayout();
			// 
			// title
			// 
			this.title.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.title.Font = new System.Drawing.Font("Arial", 18F);
			this.title.Location = new System.Drawing.Point(36, 17);
			this.title.Name = "title";
			this.title.Size = new System.Drawing.Size(454, 63);
			this.title.TabIndex = 0;
			this.title.Text = "Lorem ipsum dolor sit amet, consectetur adipisci";
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.Font = new System.Drawing.Font("Arial", 10F);
			this.label1.Location = new System.Drawing.Point(38, 80);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(452, 32);
			this.label1.TabIndex = 1;
			this.label1.Text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus eget porttitor a" +
    "nte. Donec eget arcu nisl. Praesent mattis auctor dui, ac iaculis ipsum tempus";
			// 
			// deviceGridView
			// 
			this.deviceGridView.AllowUserToAddRows = false;
			this.deviceGridView.AllowUserToDeleteRows = false;
			this.deviceGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.deviceGridView.BackgroundColor = System.Drawing.SystemColors.Control;
			this.deviceGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.deviceGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.nameColumn,
            this.typeColumn,
            this.statusColumn});
			this.deviceGridView.Location = new System.Drawing.Point(41, 130);
			this.deviceGridView.MultiSelect = false;
			this.deviceGridView.Name = "deviceGridView";
			this.deviceGridView.ReadOnly = true;
			this.deviceGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.deviceGridView.Size = new System.Drawing.Size(449, 135);
			this.deviceGridView.TabIndex = 2;
			// 
			// nameColumn
			// 
			this.nameColumn.HeaderText = "Device";
			this.nameColumn.Name = "nameColumn";
			this.nameColumn.ReadOnly = true;
			this.nameColumn.Width = 200;
			// 
			// typeColumn
			// 
			this.typeColumn.HeaderText = "Type";
			this.typeColumn.Name = "typeColumn";
			this.typeColumn.ReadOnly = true;
			// 
			// statusColumn
			// 
			this.statusColumn.HeaderText = "Status";
			this.statusColumn.Name = "statusColumn";
			this.statusColumn.ReadOnly = true;
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.Font = new System.Drawing.Font("Arial", 20F);
			this.label2.Location = new System.Drawing.Point(41, 286);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(180, 32);
			this.label2.TabIndex = 3;
			this.label2.Text = "Events";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label3.Font = new System.Drawing.Font("Arial", 20F);
			this.label3.Location = new System.Drawing.Point(310, 286);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(180, 32);
			this.label3.TabIndex = 4;
			this.label3.Text = "Photos";
			this.label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// photoCount
			// 
			this.photoCount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.photoCount.Font = new System.Drawing.Font("Arial", 15F);
			this.photoCount.Location = new System.Drawing.Point(310, 324);
			this.photoCount.Name = "photoCount";
			this.photoCount.Size = new System.Drawing.Size(180, 32);
			this.photoCount.TabIndex = 6;
			this.photoCount.Text = "33";
			this.photoCount.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// eventCount
			// 
			this.eventCount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.eventCount.Font = new System.Drawing.Font("Arial", 15F);
			this.eventCount.Location = new System.Drawing.Point(41, 324);
			this.eventCount.Name = "eventCount";
			this.eventCount.Size = new System.Drawing.Size(180, 32);
			this.eventCount.TabIndex = 5;
			this.eventCount.Text = "50";
			this.eventCount.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// label6
			// 
			this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label6.AutoSize = true;
			this.label6.Font = new System.Drawing.Font("Arial", 10F);
			this.label6.Location = new System.Drawing.Point(38, 376);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(228, 16);
			this.label6.TabIndex = 7;
			this.label6.Text = "Extend your personal cloud to your";
			// 
			// linkLabel1
			// 
			this.linkLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.linkLabel1.AutoSize = true;
			this.linkLabel1.Font = new System.Drawing.Font("Arial", 10F);
			this.linkLabel1.Location = new System.Drawing.Point(263, 376);
			this.linkLabel1.Name = "linkLabel1";
			this.linkLabel1.Size = new System.Drawing.Size(57, 16);
			this.linkLabel1.TabIndex = 8;
			this.linkLabel1.TabStop = true;
			this.linkLabel1.Text = "Android";
			// 
			// label7
			// 
			this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label7.AutoSize = true;
			this.label7.Font = new System.Drawing.Font("Arial", 10F);
			this.label7.Location = new System.Drawing.Point(317, 376);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(36, 16);
			this.label7.TabIndex = 9;
			this.label7.Text = "and ";
			// 
			// linkLabel2
			// 
			this.linkLabel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.linkLabel2.AutoSize = true;
			this.linkLabel2.Font = new System.Drawing.Font("Arial", 10F);
			this.linkLabel2.Location = new System.Drawing.Point(347, 376);
			this.linkLabel2.Name = "linkLabel2";
			this.linkLabel2.Size = new System.Drawing.Size(31, 16);
			this.linkLabel2.TabIndex = 10;
			this.linkLabel2.TabStop = true;
			this.linkLabel2.Text = "iOS";
			// 
			// label8
			// 
			this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label8.AutoSize = true;
			this.label8.Font = new System.Drawing.Font("Arial", 10F);
			this.label8.Location = new System.Drawing.Point(379, 376);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(60, 16);
			this.label8.TabIndex = 11;
			this.label8.Text = "devices.";
			// 
			// timer
			// 
			this.timer.Interval = 1000;
			// 
			// PersonalCloudStatusControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.Controls.Add(this.label8);
			this.Controls.Add(this.linkLabel2);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.linkLabel1);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.photoCount);
			this.Controls.Add(this.eventCount);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.deviceGridView);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.title);
			this.Name = "PersonalCloudStatusControl";
			this.Size = new System.Drawing.Size(524, 424);
			((System.ComponentModel.ISupportInitialize)(this.deviceGridView)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label title;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.DataGridView deviceGridView;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label photoCount;
		private System.Windows.Forms.Label eventCount;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.LinkLabel linkLabel1;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.LinkLabel linkLabel2;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.DataGridViewTextBoxColumn nameColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn typeColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn statusColumn;
		private System.Windows.Forms.Timer timer;
	}
}
