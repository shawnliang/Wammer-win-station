namespace Waveface.Stream.WindowsClient.Src.Dialog
{
	partial class PlanChangeDialog
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
			this.choosePlanControl1 = new Waveface.Stream.WindowsClient.ChoosePlanControl();
			this.SuspendLayout();
			// 
			// choosePlanControl1
			// 
			this.choosePlanControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.choosePlanControl1.Location = new System.Drawing.Point(0, 0);
			this.choosePlanControl1.Name = "choosePlanControl1";
			this.choosePlanControl1.Size = new System.Drawing.Size(693, 338);
			this.choosePlanControl1.TabIndex = 0;
			// 
			// PlanChangeDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(693, 338);
			this.Controls.Add(this.choosePlanControl1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PlanChangeDialog";
			this.Text = "PlanChangeDialog";
			this.ResumeLayout(false);

		}

		#endregion

		private ChoosePlanControl choosePlanControl1;
	}
}