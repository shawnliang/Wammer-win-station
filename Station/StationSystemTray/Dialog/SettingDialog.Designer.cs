using Waveface;
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
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle15 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle16 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
			this.lblCopyRight = new System.Windows.Forms.Label();
			this.linkLegalNotice = new System.Windows.Forms.LinkLabel();
			this.label1 = new System.Windows.Forms.Label();
			this.bgworkerUpdate = new System.ComponentModel.BackgroundWorker();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.lblResorcePath = new System.Windows.Forms.Label();
			this.btnMove = new System.Windows.Forms.Button();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.btnUpdate = new System.Windows.Forms.Button();
			this.lblVersion = new System.Windows.Forms.Label();
			this.dgvAccountList = new System.Windows.Forms.DataGridView();
			this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Column3 = new System.Windows.Forms.DataGridViewButtonColumn();
			this.groupBox3.SuspendLayout();
			this.groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvAccountList)).BeginInit();
			this.SuspendLayout();
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
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// bgworkerUpdate
			// 
			this.bgworkerUpdate.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgworkerUpdate_DoWork);
			this.bgworkerUpdate.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgworkerUpdate_RunWorkerCompleted);
			// 
			// groupBox3
			// 
			resources.ApplyResources(this.groupBox3, "groupBox3");
			this.groupBox3.Controls.Add(this.lblResorcePath);
			this.groupBox3.Controls.Add(this.btnMove);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.TabStop = false;
			// 
			// lblResorcePath
			// 
			resources.ApplyResources(this.lblResorcePath, "lblResorcePath");
			this.lblResorcePath.AutoEllipsis = true;
			this.lblResorcePath.Name = "lblResorcePath";
			// 
			// btnMove
			// 
			resources.ApplyResources(this.btnMove, "btnMove");
			this.btnMove.ForeColor = System.Drawing.Color.Black;
			this.btnMove.Name = "btnMove";
			this.btnMove.UseVisualStyleBackColor = true;
			this.btnMove.Click += new System.EventHandler(this.btnMove_Click);
			// 
			// groupBox2
			// 
			resources.ApplyResources(this.groupBox2, "groupBox2");
			this.groupBox2.Controls.Add(this.btnUpdate);
			this.groupBox2.Controls.Add(this.lblVersion);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.TabStop = false;
			this.groupBox2.Enter += new System.EventHandler(this.groupBox2_Enter);
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
			this.lblVersion.Name = "lblVersion";
			// 
			// dgvAccountList
			// 
			this.dgvAccountList.AllowUserToAddRows = false;
			this.dgvAccountList.AllowUserToDeleteRows = false;
			this.dgvAccountList.AllowUserToResizeColumns = false;
			this.dgvAccountList.AllowUserToResizeRows = false;
			dataGridViewCellStyle9.SelectionBackColor = System.Drawing.Color.White;
			dataGridViewCellStyle9.SelectionForeColor = System.Drawing.Color.Black;
			this.dgvAccountList.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle9;
			this.dgvAccountList.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.dgvAccountList.BackgroundColor = System.Drawing.Color.White;
			this.dgvAccountList.CausesValidation = false;
			this.dgvAccountList.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
			this.dgvAccountList.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
			this.dgvAccountList.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
			dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle10.BackColor = System.Drawing.Color.White;
			dataGridViewCellStyle10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			dataGridViewCellStyle10.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle10.SelectionBackColor = System.Drawing.Color.White;
			dataGridViewCellStyle10.SelectionForeColor = System.Drawing.Color.Black;
			dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.dgvAccountList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle10;
			resources.ApplyResources(this.dgvAccountList, "dgvAccountList");
			this.dgvAccountList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
			this.dgvAccountList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3});
			dataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle14.BackColor = System.Drawing.SystemColors.Window;
			dataGridViewCellStyle14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			dataGridViewCellStyle14.ForeColor = System.Drawing.SystemColors.ControlText;
			dataGridViewCellStyle14.SelectionBackColor = System.Drawing.Color.White;
			dataGridViewCellStyle14.SelectionForeColor = System.Drawing.Color.Black;
			dataGridViewCellStyle14.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.dgvAccountList.DefaultCellStyle = dataGridViewCellStyle14;
			this.dgvAccountList.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
			this.dgvAccountList.MultiSelect = false;
			this.dgvAccountList.Name = "dgvAccountList";
			dataGridViewCellStyle15.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle15.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
			dataGridViewCellStyle15.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle15.SelectionBackColor = System.Drawing.Color.White;
			dataGridViewCellStyle15.SelectionForeColor = System.Drawing.Color.Black;
			dataGridViewCellStyle15.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.dgvAccountList.RowHeadersDefaultCellStyle = dataGridViewCellStyle15;
			this.dgvAccountList.RowHeadersVisible = false;
			dataGridViewCellStyle16.SelectionBackColor = System.Drawing.Color.White;
			dataGridViewCellStyle16.SelectionForeColor = System.Drawing.Color.Black;
			this.dgvAccountList.RowsDefaultCellStyle = dataGridViewCellStyle16;
			this.dgvAccountList.RowTemplate.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.White;
			this.dgvAccountList.RowTemplate.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.Black;
			this.dgvAccountList.RowTemplate.Height = 32;
			this.dgvAccountList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dgvAccountList.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvAccountList_CellContentClick);
			this.dgvAccountList.Paint += new System.Windows.Forms.PaintEventHandler(this.dgvAccountList_Paint);
			// 
			// Column1
			// 
			dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle11.Padding = new System.Windows.Forms.Padding(16, 0, 0, 0);
			dataGridViewCellStyle11.SelectionBackColor = System.Drawing.Color.White;
			dataGridViewCellStyle11.SelectionForeColor = System.Drawing.Color.Black;
			this.Column1.DefaultCellStyle = dataGridViewCellStyle11;
			this.Column1.FillWeight = 285F;
			resources.ApplyResources(this.Column1, "Column1");
			this.Column1.Name = "Column1";
			this.Column1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// Column2
			// 
			dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle12.SelectionBackColor = System.Drawing.Color.White;
			dataGridViewCellStyle12.SelectionForeColor = System.Drawing.Color.Black;
			this.Column2.DefaultCellStyle = dataGridViewCellStyle12;
			this.Column2.FillWeight = 128F;
			resources.ApplyResources(this.Column2, "Column2");
			this.Column2.Name = "Column2";
			this.Column2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// Column3
			// 
			dataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle13.Padding = new System.Windows.Forms.Padding(24, 3, 24, 3);
			dataGridViewCellStyle13.SelectionBackColor = System.Drawing.Color.White;
			dataGridViewCellStyle13.SelectionForeColor = System.Drawing.Color.Black;
			this.Column3.DefaultCellStyle = dataGridViewCellStyle13;
			this.Column3.FillWeight = 156F;
			resources.ApplyResources(this.Column3, "Column3");
			this.Column3.Name = "Column3";
			this.Column3.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			// 
			// SettingDialog
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
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
			this.groupBox3.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
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
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label lblResorcePath;
		private System.ComponentModel.BackgroundWorker bgworkerUpdate;
		private System.Windows.Forms.DataGridView dgvAccountList;
		private System.Windows.Forms.LinkLabel linkLegalNotice;
		private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
		private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
		private System.Windows.Forms.DataGridViewButtonColumn Column3;
	}
}
