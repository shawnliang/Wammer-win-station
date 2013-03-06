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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PlanChangeDialog));
			this.choosePlanControl1 = new Waveface.Stream.WindowsClient.ChoosePlanControl();
			this.button1 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// choosePlanControl1
			// 
			resources.ApplyResources(this.choosePlanControl1, "choosePlanControl1");
			this.choosePlanControl1.Name = "choosePlanControl1";
			// 
			// button1
			// 
			resources.ApplyResources(this.button1, "button1");
			this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button1.Name = "button1";
			this.button1.UseVisualStyleBackColor = true;
			// 
			// PlanChangeDialog
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.button1);
			this.Controls.Add(this.choosePlanControl1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PlanChangeDialog";
			this.ResumeLayout(false);

		}

		#endregion

		private ChoosePlanControl choosePlanControl1;
		private System.Windows.Forms.Button button1;
	}
}