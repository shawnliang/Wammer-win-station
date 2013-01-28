namespace Waveface.Stream.WindowsClient
{
	partial class ChoosePlanControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChoosePlanControl));
			this.label1 = new System.Windows.Forms.Label();
			this.radioFree = new System.Windows.Forms.RadioButton();
			this.radio250 = new System.Windows.Forms.RadioButton();
			this.radio500 = new System.Windows.Forms.RadioButton();
			this.linkLabel1 = new System.Windows.Forms.LinkLabel();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.planBox3 = new Waveface.Stream.WindowsClient.PlanControl();
			this.planBox2 = new Waveface.Stream.WindowsClient.PlanControl();
			this.planBox1 = new Waveface.Stream.WindowsClient.PlanControl();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold);
			this.label1.Location = new System.Drawing.Point(18, 21);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(583, 48);
			this.label1.TabIndex = 0;
			this.label1.Text = "Choose an upgrade plan to get cloud storage and premium features";
			// 
			// radioFree
			// 
			this.radioFree.AutoSize = true;
			this.radioFree.Checked = true;
			this.radioFree.Location = new System.Drawing.Point(21, 82);
			this.radioFree.Name = "radioFree";
			this.radioFree.Size = new System.Drawing.Size(14, 13);
			this.radioFree.TabIndex = 1;
			this.radioFree.TabStop = true;
			this.radioFree.UseVisualStyleBackColor = true;
			// 
			// radio250
			// 
			this.radio250.AutoSize = true;
			this.radio250.Enabled = false;
			this.radio250.Location = new System.Drawing.Point(21, 162);
			this.radio250.Name = "radio250";
			this.radio250.Size = new System.Drawing.Size(14, 13);
			this.radio250.TabIndex = 2;
			this.radio250.UseVisualStyleBackColor = true;
			// 
			// radio500
			// 
			this.radio500.AutoSize = true;
			this.radio500.Enabled = false;
			this.radio500.Location = new System.Drawing.Point(21, 244);
			this.radio500.Name = "radio500";
			this.radio500.Size = new System.Drawing.Size(14, 13);
			this.radio500.TabIndex = 3;
			this.radio500.UseVisualStyleBackColor = true;
			// 
			// linkLabel1
			// 
			this.linkLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.linkLabel1.AutoSize = true;
			this.linkLabel1.Font = new System.Drawing.Font("Arial", 9F);
			this.linkLabel1.LinkArea = new System.Windows.Forms.LinkArea(0, 10);
			this.linkLabel1.Location = new System.Drawing.Point(18, 292);
			this.linkLabel1.Name = "linkLabel1";
			this.linkLabel1.Size = new System.Drawing.Size(156, 19);
			this.linkLabel1.TabIndex = 14;
			this.linkLabel1.TabStop = true;
			this.linkLabel1.Text = "Learn more about our plans";
			this.linkLabel1.UseCompatibleTextRendering = true;
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = global::Waveface.Stream.WindowsClient.Properties.Resources.PersonalCloud;
			this.pictureBox1.Location = new System.Drawing.Point(387, 66);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(283, 212);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBox1.TabIndex = 13;
			this.pictureBox1.TabStop = false;
			// 
			// planBox3
			// 
			this.planBox3.Description = "250GB cloud storage, plus multiple PC storage management";
			this.planBox3.DescriptionColor = System.Drawing.SystemColors.ControlDarkDark;
			this.planBox3.DescriptionFont = new System.Drawing.Font("Arial", 9.75F);
			this.planBox3.HeaderIconText = "500";
			this.planBox3.HeaderText = "$7.99 USD/month";
			this.planBox3.HeaderVisible = true;
			this.planBox3.Location = new System.Drawing.Point(41, 223);
			this.planBox3.Name = "planBox3";
			this.planBox3.RTFDescription = resources.GetString("planBox3.RTFDescription");
			this.planBox3.Size = new System.Drawing.Size(340, 65);
			this.planBox3.TabIndex = 17;
			// 
			// planBox2
			// 
			this.planBox2.Description = "250GB cloud storage, plus multiple PC storage management";
			this.planBox2.DescriptionColor = System.Drawing.SystemColors.ControlDarkDark;
			this.planBox2.DescriptionFont = new System.Drawing.Font("Arial", 9.75F);
			this.planBox2.HeaderIconText = "250";
			this.planBox2.HeaderText = "$4.99 USD/month";
			this.planBox2.HeaderVisible = true;
			this.planBox2.Location = new System.Drawing.Point(41, 147);
			this.planBox2.Name = "planBox2";
			this.planBox2.RTFDescription = resources.GetString("planBox2.RTFDescription");
			this.planBox2.Size = new System.Drawing.Size(340, 65);
			this.planBox2.TabIndex = 16;
			// 
			// planBox1
			// 
			this.planBox1.Description = "Save all original files to one PC";
			this.planBox1.DescriptionColor = System.Drawing.SystemColors.ControlDarkDark;
			this.planBox1.DescriptionFont = new System.Drawing.Font("Arial", 9.75F);
			this.planBox1.HeaderIconText = "Free";
			this.planBox1.HeaderText = "Free";
			this.planBox1.HeaderVisible = true;
			this.planBox1.Location = new System.Drawing.Point(41, 66);
			this.planBox1.Name = "planBox1";
			this.planBox1.RTFDescription = resources.GetString("planBox1.RTFDescription");
			this.planBox1.Size = new System.Drawing.Size(340, 48);
			this.planBox1.TabIndex = 15;
			// 
			// ChoosePlanControl
			// 
			this.Controls.Add(this.planBox3);
			this.Controls.Add(this.planBox2);
			this.Controls.Add(this.planBox1);
			this.Controls.Add(this.linkLabel1);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.radio500);
			this.Controls.Add(this.radio250);
			this.Controls.Add(this.radioFree);
			this.Controls.Add(this.label1);
			this.Name = "ChoosePlanControl";
			this.Size = new System.Drawing.Size(704, 330);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.RadioButton radioFree;
		private System.Windows.Forms.RadioButton radio250;
		private System.Windows.Forms.RadioButton radio500;
		private System.Windows.Forms.LinkLabel linkLabel1;
		private System.Windows.Forms.PictureBox pictureBox1;
		private PlanControl planBox1;
		private PlanControl planBox2;
		private PlanControl planBox3;
	}
}
