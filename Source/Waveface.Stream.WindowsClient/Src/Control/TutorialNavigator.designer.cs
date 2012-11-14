namespace Waveface.Stream.WindowsClient
{
	partial class TutorialNavigator
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
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.photoGalleryViewer1 = new PhotoGalleryViewer();
			this.stepNavigator1 = new StepNavigator();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.IsSplitterFixed = true;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.photoGalleryViewer1);
			this.splitContainer1.Panel1.Padding = new System.Windows.Forms.Padding(0, 30, 0, 10);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.stepNavigator1);
			this.splitContainer1.Size = new System.Drawing.Size(555, 400);
			this.splitContainer1.SplitterDistance = 350;
			this.splitContainer1.TabIndex = 0;
			// 
			// photoGalleryViewer1
			// 
			this.photoGalleryViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.photoGalleryViewer1.Images = null;
			this.photoGalleryViewer1.Location = new System.Drawing.Point(0, 30);
			this.photoGalleryViewer1.Name = "photoGalleryViewer1";
			this.photoGalleryViewer1.PhotoIndex = 0;
			this.photoGalleryViewer1.Size = new System.Drawing.Size(555, 310);
			this.photoGalleryViewer1.TabIndex = 0;
			this.photoGalleryViewer1.Text = "photoGalleryViewer1";
			this.photoGalleryViewer1.PhotosChanged += new System.EventHandler(this.photoGalleryViewer1_PhotosChanged);
			// 
			// stepNavigator1
			// 
			this.stepNavigator1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.stepNavigator1.EnableManualNavigate = false;
			this.stepNavigator1.Location = new System.Drawing.Point(0, 0);
			this.stepNavigator1.Name = "stepNavigator1";
			this.stepNavigator1.Size = new System.Drawing.Size(555, 46);
			this.stepNavigator1.StepCount = 0;
			this.stepNavigator1.TabIndex = 0;
			this.stepNavigator1.StepIndexChanged += new System.EventHandler(this.stepNavigator1_StepIndexChanged);
			// 
			// TutorialNavigator
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.splitContainer1);
			this.Name = "TutorialNavigator";
			this.Size = new System.Drawing.Size(555, 400);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private StepNavigator stepNavigator1;
		private PhotoGalleryViewer photoGalleryViewer1;
	}
}
