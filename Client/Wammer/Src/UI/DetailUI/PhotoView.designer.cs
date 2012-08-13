using Manina.Windows.Forms;
using Waveface.Component;

namespace Waveface.DetailUI
{
    partial class PhotoView
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PhotoView));
			this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.miSave = new System.Windows.Forms.ToolStripMenuItem();
			this.miSetAsCoverImage = new System.Windows.Forms.ToolStripMenuItem();
			this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.imageButton1 = new Waveface.Component.ImageButton();
			this.btnSave = new Waveface.Component.ImageButton();
			this.btnCoverImage = new Waveface.Component.ImageButton();
			this.btnSlideShow = new Waveface.Component.ImageButton();
			this.thumbnailNavigator1 = new Waveface.PhotoNavigator();
			this.panelBottom = new System.Windows.Forms.Panel();
			this.pnlPhotoViewToolBar = new System.Windows.Forms.Panel();
			this.imageBox = new Waveface.Component.ImageBox();
			this.contextMenuStrip.SuspendLayout();
			this.panelBottom.SuspendLayout();
			this.pnlPhotoViewToolBar.SuspendLayout();
			this.SuspendLayout();
			// 
			// contextMenuStrip
			// 
			this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miSave,
            this.miSetAsCoverImage});
			this.contextMenuStrip.Name = "contextMenuStrip";
			resources.ApplyResources(this.contextMenuStrip, "contextMenuStrip");
			// 
			// miSave
			// 
			this.miSave.Image = global::Waveface.Properties.Resources.FB_save;
			this.miSave.Name = "miSave";
			resources.ApplyResources(this.miSave, "miSave");
			this.miSave.Click += new System.EventHandler(this.miSave_Click);
			// 
			// miSetAsCoverImage
			// 
			this.miSetAsCoverImage.Image = global::Waveface.Properties.Resources.FB_cover;
			this.miSetAsCoverImage.Name = "miSetAsCoverImage";
			resources.ApplyResources(this.miSetAsCoverImage, "miSetAsCoverImage");
			this.miSetAsCoverImage.Click += new System.EventHandler(this.miSetAsCoverImage_Click);
			// 
			// imageButton1
			// 
			this.imageButton1.CenterAlignImage = false;
			this.imageButton1.Image = global::Waveface.Properties.Resources.back;
			this.imageButton1.ImageDisable = global::Waveface.Properties.Resources.back;
			this.imageButton1.ImageFront = null;
			this.imageButton1.ImageHover = global::Waveface.Properties.Resources.back_hl;
			resources.ApplyResources(this.imageButton1, "imageButton1");
			this.imageButton1.Name = "imageButton1";
			this.imageButton1.TextShadow = true;
			this.toolTip.SetToolTip(this.imageButton1, resources.GetString("imageButton1.ToolTip"));
			this.imageButton1.Click += new System.EventHandler(this.imageButton1_Click);
			// 
			// btnSave
			// 
			this.btnSave.CenterAlignImage = false;
			this.btnSave.Image = global::Waveface.Properties.Resources.Save;
			this.btnSave.ImageDisable = global::Waveface.Properties.Resources.FB_save_hl;
			this.btnSave.ImageFront = null;
			this.btnSave.ImageHover = global::Waveface.Properties.Resources.FB_save_hl;
			resources.ApplyResources(this.btnSave, "btnSave");
			this.btnSave.Name = "btnSave";
			this.btnSave.TextShadow = true;
			this.toolTip.SetToolTip(this.btnSave, resources.GetString("btnSave.ToolTip"));
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// btnCoverImage
			// 
			this.btnCoverImage.CenterAlignImage = false;
			this.btnCoverImage.Image = global::Waveface.Properties.Resources.cover;
			this.btnCoverImage.ImageDisable = global::Waveface.Properties.Resources.cover;
			this.btnCoverImage.ImageFront = null;
			this.btnCoverImage.ImageHover = global::Waveface.Properties.Resources.cover_hl;
			resources.ApplyResources(this.btnCoverImage, "btnCoverImage");
			this.btnCoverImage.Name = "btnCoverImage";
			this.btnCoverImage.TextShadow = true;
			this.toolTip.SetToolTip(this.btnCoverImage, resources.GetString("btnCoverImage.ToolTip"));
			this.btnCoverImage.Click += new System.EventHandler(this.btnCoverImage_Click);
			// 
			// btnSlideShow
			// 
			this.btnSlideShow.CenterAlignImage = false;
			this.btnSlideShow.Image = global::Waveface.Properties.Resources.slide;
			this.btnSlideShow.ImageDisable = global::Waveface.Properties.Resources.slide;
			this.btnSlideShow.ImageFront = null;
			this.btnSlideShow.ImageHover = global::Waveface.Properties.Resources.slide_hl;
			resources.ApplyResources(this.btnSlideShow, "btnSlideShow");
			this.btnSlideShow.Name = "btnSlideShow";
			this.btnSlideShow.TextShadow = true;
			this.toolTip.SetToolTip(this.btnSlideShow, resources.GetString("btnSlideShow.ToolTip"));
			this.btnSlideShow.Click += new System.EventHandler(this.btnSlideShow_Click);
			// 
			// thumbnailNavigator1
			// 
			this.thumbnailNavigator1.BackColor = System.Drawing.Color.Black;
			this.thumbnailNavigator1.DefaultThumbnail = ((System.Drawing.Image)(resources.GetObject("thumbnailNavigator1.DefaultThumbnail")));
			resources.ApplyResources(this.thumbnailNavigator1, "thumbnailNavigator1");
			this.thumbnailNavigator1.Name = "thumbnailNavigator1";
			this.thumbnailNavigator1.SelectedIndex = -1;
			this.thumbnailNavigator1.ThumbnailPadding = new System.Windows.Forms.Padding(0);
			this.thumbnailNavigator1.ThumbnailWidth = 128;
			// 
			// panelBottom
			// 
			this.panelBottom.BackColor = System.Drawing.Color.Black;
			this.panelBottom.Controls.Add(this.pnlPhotoViewToolBar);
			resources.ApplyResources(this.panelBottom, "panelBottom");
			this.panelBottom.Name = "panelBottom";
			// 
			// pnlPhotoViewToolBar
			// 
			resources.ApplyResources(this.pnlPhotoViewToolBar, "pnlPhotoViewToolBar");
			this.pnlPhotoViewToolBar.Controls.Add(this.imageButton1);
			this.pnlPhotoViewToolBar.Controls.Add(this.btnSave);
			this.pnlPhotoViewToolBar.Controls.Add(this.btnCoverImage);
			this.pnlPhotoViewToolBar.Controls.Add(this.btnSlideShow);
			this.pnlPhotoViewToolBar.Name = "pnlPhotoViewToolBar";
			// 
			// imageBox
			// 
			resources.ApplyResources(this.imageBox, "imageBox");
			this.imageBox.BackColor = System.Drawing.Color.Black;
			this.imageBox.Name = "imageBox";
			this.imageBox.ZoomIncrement = 10;
			this.imageBox.Click += new System.EventHandler(this.imageBox_Click);
			// 
			// PhotoView
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.imageBox);
			this.Controls.Add(this.thumbnailNavigator1);
			this.Controls.Add(this.panelBottom);
			this.KeyPreview = true;
			this.MinimizeBox = false;
			this.Name = "PhotoView";
			this.ShowInTaskbar = false;
			this.Load += new System.EventHandler(this.PhotoView_Load);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PhotoView_KeyDown);
			this.Resize += new System.EventHandler(this.PhotoView_Resize);
			this.contextMenuStrip.ResumeLayout(false);
			this.panelBottom.ResumeLayout(false);
			this.panelBottom.PerformLayout();
			this.pnlPhotoViewToolBar.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem miSave;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
		private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.ToolStripMenuItem miSetAsCoverImage;
		private System.Windows.Forms.Panel panelBottom;
		private PhotoNavigator thumbnailNavigator1;
		private System.Windows.Forms.Panel pnlPhotoViewToolBar;
		private ImageButton imageButton1;
		private ImageButton btnSave;
		private ImageButton btnCoverImage;
		private ImageButton btnSlideShow;
		private ImageBox imageBox;
    }
}

