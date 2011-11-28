namespace Gui
{
	partial class InstallationLocationStep
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InstallationLocationStep));
			this.dpInstallLocation = new SharpSetup.UI.Controls.DestinationPath();
			this.lblDescription = new System.Windows.Forms.Label();
			this.sdcbDiskSpace = new SharpSetup.UI.Controls.SimpleDiskCostBox();
			this.SuspendLayout();
			// 
			// dpInstallLocation
			// 
			resources.ApplyResources(this.dpInstallLocation, "dpInstallLocation");
			this.dpInstallLocation.DefaultPathSuffix = null;
			this.dpInstallLocation.Name = "dpInstallLocation";
			// 
			// lblDescription
			// 
			resources.ApplyResources(this.lblDescription, "lblDescription");
			this.lblDescription.Name = "lblDescription";
			// 
			// sdcbDiskSpace
			// 
			resources.ApplyResources(this.sdcbDiskSpace, "sdcbDiskSpace");
			this.sdcbDiskSpace.InfoTypes.Add(SharpSetup.UI.Controls.DiskSpaceInfoType.Total);
			this.sdcbDiskSpace.InfoTypes.Add(SharpSetup.UI.Controls.DiskSpaceInfoType.Free);
			this.sdcbDiskSpace.Name = "sdcbDiskSpace";
			// 
			// InstallationLocationStep
			// 
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.sdcbDiskSpace);
			this.Controls.Add(this.lblDescription);
			this.Controls.Add(this.dpInstallLocation);
			this.Name = "InstallationLocationStep";
			this.Entering += new System.EventHandler<SharpSetup.Base.ChangeStepEventArgs>(this.InstallationLocationStep_Entering);
			this.Controls.SetChildIndex(this.dpInstallLocation, 0);
			this.Controls.SetChildIndex(this.lblDescription, 0);
			this.Controls.SetChildIndex(this.sdcbDiskSpace, 0);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private SharpSetup.UI.Controls.DestinationPath dpInstallLocation;
		private System.Windows.Forms.Label lblDescription;
		private SharpSetup.UI.Controls.SimpleDiskCostBox sdcbDiskSpace;
	}
}
