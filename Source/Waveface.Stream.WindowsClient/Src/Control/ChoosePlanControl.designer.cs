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
			this.linkLabel1 = new System.Windows.Forms.LinkLabel();
			this.radioButton1 = new System.Windows.Forms.RadioButton();
			this.planBox2 = new Waveface.Stream.WindowsClient.PlanBox();
			this.planBox1 = new Waveface.Stream.WindowsClient.PlanBox();
			this.planBox4 = new Waveface.Stream.WindowsClient.PlanBox();
			this.SuspendLayout();
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// radioFree
			// 
			resources.ApplyResources(this.radioFree, "radioFree");
			this.radioFree.Checked = true;
			this.radioFree.Name = "radioFree";
			this.radioFree.TabStop = true;
			this.radioFree.UseVisualStyleBackColor = true;
			// 
			// radio250
			// 
			resources.ApplyResources(this.radio250, "radio250");
			this.radio250.Name = "radio250";
			this.radio250.UseVisualStyleBackColor = true;
			// 
			// linkLabel1
			// 
			resources.ApplyResources(this.linkLabel1, "linkLabel1");
			this.linkLabel1.Name = "linkLabel1";
			this.linkLabel1.TabStop = true;
			this.linkLabel1.UseCompatibleTextRendering = true;
			this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
			// 
			// radioButton1
			// 
			resources.ApplyResources(this.radioButton1, "radioButton1");
			this.radioButton1.Name = "radioButton1";
			this.radioButton1.UseVisualStyleBackColor = true;
			// 
			// planBox2
			// 
			this.planBox2.HeaderVisibile = true;
			resources.ApplyResources(this.planBox2, "planBox2");
			this.planBox2.Name = "planBox2";
			this.planBox2.Type = Waveface.Stream.WindowsClient.PlanBox.PlanType.Plan2;
			// 
			// planBox1
			// 
			this.planBox1.HeaderVisibile = true;
			resources.ApplyResources(this.planBox1, "planBox1");
			this.planBox1.Name = "planBox1";
			this.planBox1.Type = Waveface.Stream.WindowsClient.PlanBox.PlanType.Plan1;
			// 
			// planBox4
			// 
			this.planBox4.HeaderVisibile = true;
			resources.ApplyResources(this.planBox4, "planBox4");
			this.planBox4.Name = "planBox4";
			this.planBox4.Type = Waveface.Stream.WindowsClient.PlanBox.PlanType.Free;
			// 
			// ChoosePlanControl
			// 
			this.Controls.Add(this.radioButton1);
			this.Controls.Add(this.planBox2);
			this.Controls.Add(this.planBox1);
			this.Controls.Add(this.planBox4);
			this.Controls.Add(this.linkLabel1);
			this.Controls.Add(this.radio250);
			this.Controls.Add(this.radioFree);
			this.Controls.Add(this.label1);
			this.Name = "ChoosePlanControl";
			resources.ApplyResources(this, "$this");
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.RadioButton radioFree;
		private System.Windows.Forms.RadioButton radio250;
		private System.Windows.Forms.LinkLabel linkLabel1;
		private System.Windows.Forms.RadioButton radioButton1;
		private PlanBox planBox2;
		private PlanBox planBox1;
		private PlanBox planBox4;
	}
}
