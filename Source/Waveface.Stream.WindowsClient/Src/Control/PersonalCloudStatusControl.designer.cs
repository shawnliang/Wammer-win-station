namespace Waveface.Stream.WindowsClient
{
	partial class PersonalCloudStatusControl2
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
			System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Station", System.Windows.Forms.HorizontalAlignment.Left);
			System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("Tablet", System.Windows.Forms.HorizontalAlignment.Left);
			System.Windows.Forms.ListViewGroup listViewGroup3 = new System.Windows.Forms.ListViewGroup("Phone", System.Windows.Forms.HorizontalAlignment.Left);
			this.label1 = new System.Windows.Forms.Label();
			this.listView1 = new System.Windows.Forms.ListView();
			this.nameColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.profileCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.btnGetApp = new System.Windows.Forms.Button();
			this.btnInstallChromeExtension = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(145, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Your Personal Cloud Devices";
			// 
			// listView1
			// 
			this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.nameColumn,
            this.profileCol});
			listViewGroup1.Header = "Station";
			listViewGroup1.Name = "station";
			listViewGroup2.Header = "Tablet";
			listViewGroup2.Name = "tablet";
			listViewGroup3.Header = "Phone";
			listViewGroup3.Name = "phone";
			this.listView1.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2,
            listViewGroup3});
			this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.listView1.Location = new System.Drawing.Point(5, 24);
			this.listView1.MultiSelect = false;
			this.listView1.Name = "listView1";
			this.listView1.Size = new System.Drawing.Size(510, 239);
			this.listView1.TabIndex = 4;
			this.listView1.UseCompatibleStateImageBehavior = false;
			this.listView1.View = System.Windows.Forms.View.Details;
			// 
			// nameColumn
			// 
			this.nameColumn.Text = "Name";
			this.nameColumn.Width = 173;
			// 
			// profileCol
			// 
			this.profileCol.Text = "Status";
			this.profileCol.Width = 324;
			// 
			// btnGetApp
			// 
			this.btnGetApp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnGetApp.Location = new System.Drawing.Point(6, 280);
			this.btnGetApp.Name = "btnGetApp";
			this.btnGetApp.Size = new System.Drawing.Size(149, 23);
			this.btnGetApp.TabIndex = 5;
			this.btnGetApp.Text = "Get the Apps...";
			this.btnGetApp.UseVisualStyleBackColor = true;
			// 
			// btnInstallChromeExtension
			// 
			this.btnInstallChromeExtension.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnInstallChromeExtension.Location = new System.Drawing.Point(6, 321);
			this.btnInstallChromeExtension.Name = "btnInstallChromeExtension";
			this.btnInstallChromeExtension.Size = new System.Drawing.Size(149, 23);
			this.btnInstallChromeExtension.TabIndex = 6;
			this.btnInstallChromeExtension.Text = "Install Chrome Extension...";
			this.btnInstallChromeExtension.UseVisualStyleBackColor = true;
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.Location = new System.Drawing.Point(161, 280);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(339, 36);
			this.label2.TabIndex = 7;
			this.label2.Text = "AOStream mobile apps will automatically sync with AOStream Windows Station under " +
    "the same WiFi network.";
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label3.Location = new System.Drawing.Point(161, 321);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(339, 36);
			this.label3.TabIndex = 8;
			this.label3.Text = "Install AOStream Google Chrome extension to collect web pages that you really rea" +
    "d.";
			this.label3.Click += new System.EventHandler(this.label3_Click);
			// 
			// PersonalCloudStatusControl2
			// 
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.btnInstallChromeExtension);
			this.Controls.Add(this.btnGetApp);
			this.Controls.Add(this.listView1);
			this.Controls.Add(this.label1);
			this.Name = "PersonalCloudStatusControl2";
			this.Size = new System.Drawing.Size(518, 363);
			this.Load += new System.EventHandler(this.PersonalCloudStatusControl2_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.ColumnHeader nameColumn;
		private System.Windows.Forms.ColumnHeader profileCol;
		private System.Windows.Forms.Button btnGetApp;
		private System.Windows.Forms.Button btnInstallChromeExtension;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
	}
}
