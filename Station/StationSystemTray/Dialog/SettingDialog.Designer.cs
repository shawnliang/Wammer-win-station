namespace StationSystemTray
{
	partial class SettingDialog
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingDialog));
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.btnUpdate = new System.Windows.Forms.Button();
			this.lblVersion = new System.Windows.Forms.Label();
			this.btnMove = new System.Windows.Forms.Button();
			this.lblCopyRight = new System.Windows.Forms.Label();
			this.linkLegalNotice = new System.Windows.Forms.LinkLabel();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.lblResorcePath = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.dgvAccountList = new System.Windows.Forms.DataGridView();
			this.colAccount = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colUsage = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colAction = new System.Windows.Forms.DataGridViewButtonColumn();
			this.bgworkerUpdate = new System.ComponentModel.BackgroundWorker();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvAccountList)).BeginInit();
			this.SuspendLayout();
			// 
			// groupBox2
			// 
			resources.ApplyResources(this.groupBox2, "groupBox2");
			this.groupBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(226)))));
			this.groupBox2.Controls.Add(this.btnUpdate);
			this.groupBox2.Controls.Add(this.lblVersion);
			this.groupBox2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(85)))), ((int)(((byte)(154)))), ((int)(((byte)(174)))));
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.TabStop = false;
			// 
			// btnUpdate
			// 
			resources.ApplyResources(this.btnUpdate, "btnUpdate");
			this.btnUpdate.ForeColor = System.Drawing.Color.Black;
			this.btnUpdate.Name = "btnUpdate";
			this.btnUpdate.UseVisualStyleBackColor = true;
			this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
			// 
			// lblVersion
			// 
			resources.ApplyResources(this.lblVersion, "lblVersion");
			this.lblVersion.ForeColor = System.Drawing.Color.Black;
			this.lblVersion.Name = "lblVersion";
			// 
			// btnMove
			// 
			resources.ApplyResources(this.btnMove, "btnMove");
			this.btnMove.ForeColor = System.Drawing.Color.Black;
			this.btnMove.Name = "btnMove";
			this.btnMove.UseVisualStyleBackColor = true;
			this.btnMove.Click += new System.EventHandler(this.btnMove_Click);
			// 
			// lblCopyRight
			// 
			resources.ApplyResources(this.lblCopyRight, "lblCopyRight");
			this.lblCopyRight.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(134)))), ((int)(((byte)(134)))), ((int)(((byte)(134)))));
			this.lblCopyRight.Name = "lblCopyRight";
			// 
			// linkLegalNotice
			// 
			resources.ApplyResources(this.linkLegalNotice, "linkLegalNotice");
			this.linkLegalNotice.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(188)))), ((int)(((byte)(211)))));
			this.linkLegalNotice.Name = "linkLegalNotice";
			this.linkLegalNotice.TabStop = true;
			// 
			// groupBox3
			// 
			resources.ApplyResources(this.groupBox3, "groupBox3");
			this.groupBox3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(226)))));
			this.groupBox3.Controls.Add(this.lblResorcePath);
			this.groupBox3.Controls.Add(this.btnMove);
			this.groupBox3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(85)))), ((int)(((byte)(154)))), ((int)(((byte)(174)))));
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.TabStop = false;
			// 
			// lblResorcePath
			// 
			this.lblResorcePath.AutoEllipsis = true;
			resources.ApplyResources(this.lblResorcePath, "lblResorcePath");
			this.lblResorcePath.ForeColor = System.Drawing.Color.Black;
			this.lblResorcePath.Name = "lblResorcePath";
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(85)))), ((int)(((byte)(154)))), ((int)(((byte)(174)))));
			this.label1.Name = "label1";
			// 
			// dgvAccountList
			// 
			this.dgvAccountList.AllowUserToAddRows = false;
			this.dgvAccountList.AllowUserToDeleteRows = false;
			this.dgvAccountList.AllowUserToResizeColumns = false;
			this.dgvAccountList.AllowUserToResizeRows = false;
			this.dgvAccountList.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.dgvAccountList.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(226)))));
			this.dgvAccountList.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleVertical;
			this.dgvAccountList.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
			dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(85)))), ((int)(((byte)(154)))), ((int)(((byte)(174)))));
			dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.dgvAccountList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			resources.ApplyResources(this.dgvAccountList, "dgvAccountList");
			this.dgvAccountList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
			this.dgvAccountList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colAccount,
            this.colUsage,
            this.colAction});
			this.dgvAccountList.EnableHeadersVisualStyles = false;
			this.dgvAccountList.MultiSelect = false;
			this.dgvAccountList.Name = "dgvAccountList";
			this.dgvAccountList.ReadOnly = true;
			this.dgvAccountList.RowHeadersVisible = false;
			this.dgvAccountList.RowTemplate.DefaultCellStyle.Padding = new System.Windows.Forms.Padding(5, 0, 5, 0);
			this.dgvAccountList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dgvAccountList.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvAccountList_CellContentClick);
			this.dgvAccountList.Paint += new System.Windows.Forms.PaintEventHandler(this.dgvAccountList_Paint);
			// 
			// colAccount
			// 
			resources.ApplyResources(this.colAccount, "colAccount");
			this.colAccount.Name = "colAccount";
			this.colAccount.ReadOnly = true;
			this.colAccount.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// colUsage
			// 
			resources.ApplyResources(this.colUsage, "colUsage");
			this.colUsage.Name = "colUsage";
			this.colUsage.ReadOnly = true;
			this.colUsage.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// colAction
			// 
			resources.ApplyResources(this.colAction, "colAction");
			this.colAction.Name = "colAction";
			this.colAction.ReadOnly = true;
			// 
			// bgworkerUpdate
			// 
			this.bgworkerUpdate.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgworkerUpdate_DoWork);
			this.bgworkerUpdate.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgworkerUpdate_RunWorkerCompleted);
			// 
			// SettingDialog
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(225)))), ((int)(((byte)(225)))));
			this.Controls.Add(this.dgvAccountList);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.lblCopyRight);
			this.Controls.Add(this.linkLegalNotice);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SettingDialog";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SettingDialog_FormClosing);
			this.Load += new System.EventHandler(this.LocalSettingDialog_Load);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgvAccountList)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Button btnMove;
		private System.Windows.Forms.Button btnUpdate;
		private System.Windows.Forms.Label lblVersion;
		private System.Windows.Forms.Label lblCopyRight;
		private System.Windows.Forms.LinkLabel linkLegalNotice;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.DataGridView dgvAccountList;
		private System.Windows.Forms.Label lblResorcePath;
		private System.ComponentModel.BackgroundWorker bgworkerUpdate;
		private System.Windows.Forms.DataGridViewTextBoxColumn colAccount;
		private System.Windows.Forms.DataGridViewTextBoxColumn colUsage;
		private System.Windows.Forms.DataGridViewButtonColumn colAction;
	}
}
