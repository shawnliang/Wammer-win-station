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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FeatureSelectionStep));
			this.ftMain = new SharpSetup.UI.Controls.FeatureTree();
			this.gbDescription = new System.Windows.Forms.GroupBox();
			this.lblDescription = new System.Windows.Forms.Label();
			this.sdcbDiskSpace = new SharpSetup.UI.Controls.SimpleDiskCostBox();
			this.lblInstruction = new System.Windows.Forms.Label();
			this.gbDescription.SuspendLayout();
			this.SuspendLayout();
			// 
			// ftMain
			// 
			resources.ApplyResources(this.ftMain, "ftMain");
			this.ftMain.Name = "ftMain";
			this.ftMain.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.ftMain_AfterSelect);
			// 
			// gbDescription
			// 
			resources.ApplyResources(this.gbDescription, "gbDescription");
			this.gbDescription.Controls.Add(this.lblDescription);
			this.gbDescription.Name = "gbDescription";
			this.gbDescription.TabStop = false;
			// 
			// lblDescription
			// 
			resources.ApplyResources(this.lblDescription, "lblDescription");
			this.lblDescription.Name = "lblDescription";
			// 
			// sdcbDiskSpace
			// 
			resources.ApplyResources(this.sdcbDiskSpace, "sdcbDiskSpace");
			this.sdcbDiskSpace.InfoTypes.Add(SharpSetup.UI.Controls.DiskSpaceInfoType.Required);
			this.sdcbDiskSpace.InfoTypes.Add(SharpSetup.UI.Controls.DiskSpaceInfoType.Remains);
			this.sdcbDiskSpace.Name = "sdcbDiskSpace";
			// 
			// lblInstruction
			// 
			resources.ApplyResources(this.lblInstruction, "lblInstruction");
			this.lblInstruction.Name = "lblInstruction";
			// 
			// FeatureSelectionStep
			// 
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.lblInstruction);
			this.Controls.Add(this.sdcbDiskSpace);
			this.Controls.Add(this.gbDescription);
			this.Controls.Add(this.ftMain);
			this.Name = "FeatureSelectionStep";
			this.Entering += new System.EventHandler<SharpSetup.Base.ChangeStepEventArgs>(this.FeatureSelectionStep_Entering);
			this.Controls.SetChildIndex(this.ftMain, 0);
			this.Controls.SetChildIndex(this.gbDescription, 0);
			this.Controls.SetChildIndex(this.sdcbDiskSpace, 0);
			this.Controls.SetChildIndex(this.lblInstruction, 0);
			this.gbDescription.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private SharpSetup.UI.Controls.FeatureTree ftMain;
		private System.Windows.Forms.GroupBox gbDescription;
		private System.Windows.Forms.Label lblDescription;
		private SharpSetup.UI.Controls.SimpleDiskCostBox sdcbDiskSpace;
		private System.Windows.Forms.Label lblInstruction;
	}
}
