namespace Waveface.Stream.WindowsClient
{
	partial class PlanControl
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
			this.btnPlanHeaderIcon = new System.Windows.Forms.Button();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.panel1 = new System.Windows.Forms.Panel();
			this.rtbxPlanDescription = new System.Windows.Forms.RichTextBox();
			this.lblPlanHeader = new System.Windows.Forms.Label();
			this.tableLayoutPanel1.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnPlanHeaderIcon
			// 
			this.btnPlanHeaderIcon.BackColor = System.Drawing.Color.DodgerBlue;
			this.btnPlanHeaderIcon.FlatAppearance.MouseDownBackColor = System.Drawing.Color.DodgerBlue;
			this.btnPlanHeaderIcon.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DodgerBlue;
			this.btnPlanHeaderIcon.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.btnPlanHeaderIcon.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnPlanHeaderIcon.ForeColor = System.Drawing.SystemColors.ControlLightLight;
			this.btnPlanHeaderIcon.Location = new System.Drawing.Point(3, 3);
			this.btnPlanHeaderIcon.Name = "btnPlanHeaderIcon";
			this.btnPlanHeaderIcon.Size = new System.Drawing.Size(84, 42);
			this.btnPlanHeaderIcon.TabIndex = 9;
			this.btnPlanHeaderIcon.Text = "[Plan Header Icon]";
			this.btnPlanHeaderIcon.UseVisualStyleBackColor = false;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this.btnPlanHeaderIcon, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(266, 66);
			this.tableLayoutPanel1.TabIndex = 12;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.rtbxPlanDescription);
			this.panel1.Controls.Add(this.lblPlanHeader);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(93, 3);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(170, 60);
			this.panel1.TabIndex = 10;
			// 
			// rtbxPlanDescription
			// 
			this.rtbxPlanDescription.BackColor = System.Drawing.SystemColors.Control;
			this.rtbxPlanDescription.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.rtbxPlanDescription.Dock = System.Windows.Forms.DockStyle.Fill;
			this.rtbxPlanDescription.Location = new System.Drawing.Point(0, 16);
			this.rtbxPlanDescription.Name = "rtbxPlanDescription";
			this.rtbxPlanDescription.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
			this.rtbxPlanDescription.ShortcutsEnabled = false;
			this.rtbxPlanDescription.Size = new System.Drawing.Size(170, 44);
			this.rtbxPlanDescription.TabIndex = 17;
			this.rtbxPlanDescription.Text = "[Plan Description]";
			// 
			// lblPlanHeader
			// 
			this.lblPlanHeader.Dock = System.Windows.Forms.DockStyle.Top;
			this.lblPlanHeader.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblPlanHeader.Location = new System.Drawing.Point(0, 0);
			this.lblPlanHeader.Name = "lblPlanHeader";
			this.lblPlanHeader.Size = new System.Drawing.Size(170, 16);
			this.lblPlanHeader.TabIndex = 16;
			this.lblPlanHeader.Text = "[Plan Header]";
			// 
			// PlanBox
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "PlanBox";
			this.Size = new System.Drawing.Size(266, 66);
			this.BackColorChanged += new System.EventHandler(this.PlanBox_BackColorChanged);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btnPlanHeaderIcon;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.RichTextBox rtbxPlanDescription;
		private System.Windows.Forms.Label lblPlanHeader;


	}
}
