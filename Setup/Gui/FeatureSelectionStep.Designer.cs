namespace Gui
{
	partial class FeatureSelectionStep
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FeatureSelectionStep));
			this.radioClientAndStation = new System.Windows.Forms.RadioButton();
			this.radioClientOnly = new System.Windows.Forms.RadioButton();
			this.labelClientAndStationDesc = new System.Windows.Forms.Label();
			this.labelClientOnlyDesc = new System.Windows.Forms.Label();
			this.lblNote = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// radioClientAndStation
			// 
			resources.ApplyResources(this.radioClientAndStation, "radioClientAndStation");
			this.radioClientAndStation.Checked = true;
			this.radioClientAndStation.Name = "radioClientAndStation";
			this.radioClientAndStation.TabStop = true;
			this.radioClientAndStation.UseVisualStyleBackColor = true;
			this.radioClientAndStation.CheckedChanged += new System.EventHandler(this.radioClientAndStation_CheckedChanged);
			// 
			// radioClientOnly
			// 
			resources.ApplyResources(this.radioClientOnly, "radioClientOnly");
			this.radioClientOnly.Name = "radioClientOnly";
			this.radioClientOnly.UseVisualStyleBackColor = true;
			this.radioClientOnly.CheckedChanged += new System.EventHandler(this.radioClientOnly_CheckedChanged);
			// 
			// labelClientAndStationDesc
			// 
			resources.ApplyResources(this.labelClientAndStationDesc, "labelClientAndStationDesc");
			this.labelClientAndStationDesc.Name = "labelClientAndStationDesc";
			// 
			// labelClientOnlyDesc
			// 
			resources.ApplyResources(this.labelClientOnlyDesc, "labelClientOnlyDesc");
			this.labelClientOnlyDesc.Name = "labelClientOnlyDesc";
			// 
			// lblNote
			// 
			resources.ApplyResources(this.lblNote, "lblNote");
			this.lblNote.Name = "lblNote";
			// 
			// FeatureSelectionStep
			// 
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.lblNote);
			this.Controls.Add(this.labelClientOnlyDesc);
			this.Controls.Add(this.labelClientAndStationDesc);
			this.Controls.Add(this.radioClientOnly);
			this.Controls.Add(this.radioClientAndStation);
			this.Name = "FeatureSelectionStep";
			this.Entering += new System.EventHandler<SharpSetup.Base.ChangeStepEventArgs>(this.FeatureSelectionStep_Entering);
			this.Controls.SetChildIndex(this.radioClientAndStation, 0);
			this.Controls.SetChildIndex(this.radioClientOnly, 0);
			this.Controls.SetChildIndex(this.labelClientAndStationDesc, 0);
			this.Controls.SetChildIndex(this.labelClientOnlyDesc, 0);
			this.Controls.SetChildIndex(this.lblNote, 0);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.RadioButton radioClientAndStation;
		private System.Windows.Forms.RadioButton radioClientOnly;
		private System.Windows.Forms.Label labelClientAndStationDesc;
		private System.Windows.Forms.Label labelClientOnlyDesc;
		private System.Windows.Forms.Label lblNote;
	}
}
