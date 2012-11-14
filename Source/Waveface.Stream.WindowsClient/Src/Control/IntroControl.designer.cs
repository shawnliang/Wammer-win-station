namespace Waveface.Stream.WindowsClient
{
	partial class IntroControl
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
			this.tutorialNavigator1 = new TutorialNavigator();
			this.SuspendLayout();
			// 
			// tutorialNavigator1
			// 
			this.tutorialNavigator1.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.tutorialNavigator1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tutorialNavigator1.EnableManualNavigate = true;
			this.tutorialNavigator1.Location = new System.Drawing.Point(0, 0);
			this.tutorialNavigator1.Name = "tutorialNavigator1";
			this.tutorialNavigator1.Size = new System.Drawing.Size(555, 400);
			this.tutorialNavigator1.TabIndex = 0;
			this.tutorialNavigator1.TutorialPhotos = null;
			// 
			// IntroControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ActiveBorder;
			this.Controls.Add(this.tutorialNavigator1);
			this.Name = "IntroControl";
			this.Size = new System.Drawing.Size(555, 400);
			this.ResumeLayout(false);

		}

		#endregion

		private TutorialNavigator tutorialNavigator1;

	}
}
