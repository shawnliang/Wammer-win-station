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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PersonalCloudStatusControl2));
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
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// listView1
			// 
			resources.ApplyResources(this.listView1, "listView1");
			this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.nameColumn,
            this.profileCol});
			this.listView1.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            ((System.Windows.Forms.ListViewGroup)(resources.GetObject("listView1.Groups"))),
            ((System.Windows.Forms.ListViewGroup)(resources.GetObject("listView1.Groups1")))});
			this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.listView1.MultiSelect = false;
			this.listView1.Name = "listView1";
			this.listView1.UseCompatibleStateImageBehavior = false;
			this.listView1.View = System.Windows.Forms.View.Details;
			// 
			// nameColumn
			// 
			resources.ApplyResources(this.nameColumn, "nameColumn");
			// 
			// profileCol
			// 
			resources.ApplyResources(this.profileCol, "profileCol");
			// 
			// btnGetApp
			// 
			resources.ApplyResources(this.btnGetApp, "btnGetApp");
			this.btnGetApp.Name = "btnGetApp";
			this.btnGetApp.UseVisualStyleBackColor = true;
			this.btnGetApp.Click += new System.EventHandler(this.btnGetApp_Click);
			// 
			// btnInstallChromeExtension
			// 
			resources.ApplyResources(this.btnInstallChromeExtension, "btnInstallChromeExtension");
			this.btnInstallChromeExtension.Name = "btnInstallChromeExtension";
			this.btnInstallChromeExtension.UseVisualStyleBackColor = true;
			this.btnInstallChromeExtension.Click += new System.EventHandler(this.btnInstallChromeExtension_Click);
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			// 
			// label3
			// 
			resources.ApplyResources(this.label3, "label3");
			this.label3.Name = "label3";
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
			resources.ApplyResources(this, "$this");
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
