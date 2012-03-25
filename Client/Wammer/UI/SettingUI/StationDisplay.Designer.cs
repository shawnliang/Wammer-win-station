namespace Waveface.SettingUI
{
	partial class StationDisplay
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
			this.lblComputerName = new System.Windows.Forms.Label();
			this.lblLastSeen = new System.Windows.Forms.Label();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// lblComputerName
			// 
			this.lblComputerName.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.lblComputerName.AutoSize = true;
			this.lblComputerName.Location = new System.Drawing.Point(3, 7);
			this.lblComputerName.Name = "lblComputerName";
			this.lblComputerName.Size = new System.Drawing.Size(68, 13);
			this.lblComputerName.TabIndex = 0;
			this.lblComputerName.Text = "Jonathan-PC";
			// 
			// lblLastSeen
			// 
			this.lblLastSeen.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.lblLastSeen.AutoSize = true;
			this.lblLastSeen.Location = new System.Drawing.Point(227, 7);
			this.lblLastSeen.Name = "lblLastSeen";
			this.lblLastSeen.Size = new System.Drawing.Size(56, 13);
			this.lblLastSeen.TabIndex = 1;
			this.lblLastSeen.Text = "Last seen:";
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 47.82609F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 52.17391F));
			this.tableLayoutPanel1.Controls.Add(this.lblLastSeen, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.lblComputerName, 0, 0);
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(345, 27);
			this.tableLayoutPanel1.TabIndex = 2;
			// 
			// StationDisplay
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Margin = new System.Windows.Forms.Padding(0);
			this.Name = "StationDisplay";
			this.Size = new System.Drawing.Size(345, 27);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label lblComputerName;
		private System.Windows.Forms.Label lblLastSeen;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
	}
}
