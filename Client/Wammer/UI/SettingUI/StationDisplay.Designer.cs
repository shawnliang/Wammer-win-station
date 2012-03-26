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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StationDisplay));
			this.lblComputerName = new System.Windows.Forms.Label();
			this.lblLastSeen = new System.Windows.Forms.Label();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// lblComputerName
			// 
			resources.ApplyResources(this.lblComputerName, "lblComputerName");
			this.lblComputerName.Name = "lblComputerName";
			// 
			// lblLastSeen
			// 
			resources.ApplyResources(this.lblLastSeen, "lblLastSeen");
			this.lblLastSeen.Name = "lblLastSeen";
			// 
			// tableLayoutPanel1
			// 
			resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
			this.tableLayoutPanel1.Controls.Add(this.lblLastSeen, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.lblComputerName, 0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			// 
			// StationDisplay
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "StationDisplay";
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
