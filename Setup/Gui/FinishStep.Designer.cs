namespace Gui
{
	partial class FinishStep
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FinishStep));
            this.lblInstruction = new System.Windows.Forms.Label();
            this.lblDetails = new System.Windows.Forms.Label();
            this.lblFinished = new System.Windows.Forms.Label();
            this.label_Hint = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pbLeft)).BeginInit();
            this.SuspendLayout();
            // 
            // pbLeft
            // 
            resources.ApplyResources(this.pbLeft, "pbLeft");
            // 
            // lblInstruction
            // 
            resources.ApplyResources(this.lblInstruction, "lblInstruction");
            this.lblInstruction.Name = "lblInstruction";
            // 
            // lblDetails
            // 
            resources.ApplyResources(this.lblDetails, "lblDetails");
            this.lblDetails.Name = "lblDetails";
            // 
            // lblFinished
            // 
            resources.ApplyResources(this.lblFinished, "lblFinished");
            this.lblFinished.Name = "lblFinished";
            // 
            // label_Hint
            // 
            resources.ApplyResources(this.label_Hint, "label_Hint");
            this.label_Hint.Name = "label_Hint";
            // 
            // FinishStep
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.label_Hint);
            this.Controls.Add(this.lblInstruction);
            this.Controls.Add(this.lblDetails);
            this.Controls.Add(this.lblFinished);
            this.Name = "FinishStep";
            this.StepType = SharpSetup.UI.Forms.Modern.ModernStepType.Last;
            this.Finish += new System.EventHandler<SharpSetup.Base.ChangeStepEventArgs>(this.FinishStep_Finish);
            this.Entering += new System.EventHandler<SharpSetup.Base.ChangeStepEventArgs>(this.FinishStep_Entering);
            this.Entered += new System.EventHandler<System.EventArgs>(this.FinishStep_Entered);
            this.Controls.SetChildIndex(this.lblFinished, 0);
            this.Controls.SetChildIndex(this.lblDetails, 0);
            this.Controls.SetChildIndex(this.lblInstruction, 0);
            this.Controls.SetChildIndex(this.label_Hint, 0);
            this.Controls.SetChildIndex(this.pbLeft, 0);
            ((System.ComponentModel.ISupportInitialize)(this.pbLeft)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label lblInstruction;
		private System.Windows.Forms.Label lblDetails;
        private System.Windows.Forms.Label lblFinished;
        private System.Windows.Forms.Label label_Hint;


	}
}
