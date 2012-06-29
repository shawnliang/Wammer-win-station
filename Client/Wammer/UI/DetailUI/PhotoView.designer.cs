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
			this.imageBox = new Waveface.Component.ImageBox();
			this.thumbnailNavigator1 = new Waveface.ThumbnailNavigator();
			this.panelBottom = new System.Windows.Forms.Panel();
			this.pnlPhotoViewToolBar = new System.Windows.Forms.Panel();
			this.cultureManager = new Waveface.Localization.CultureManager(this.components);
			this.StatusLabelFileName = new System.Windows.Forms.ToolStripStatusLabel();
			this.StatusLabelCurrentSize = new System.Windows.Forms.ToolStripStatusLabel();
			this.imageSizeToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.positionToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.zoomToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.StatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.statusStrip = new System.Windows.Forms.StatusStrip();
			this.contextMenuStrip.SuspendLayout();
			this.panelBottom.SuspendLayout();
			this.pnlPhotoViewToolBar.SuspendLayout();
			this.statusStrip.SuspendLayout();
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
			this.imageButton1.Image = global::Waveface.Properties.Resources.gridview;
			this.imageButton1.ImageDisable = global::Waveface.Properties.Resources.gridview;
			this.imageButton1.ImageFront = null;
			this.imageButton1.ImageHover = global::Waveface.Properties.Resources.gridview_hl;
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
			// imageBox
			// 
			this.imageBox.AutoPan = false;
			resources.ApplyResources(this.imageBox, "imageBox");
			this.imageBox.BackColor = System.Drawing.Color.Black;
			this.imageBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.imageBox.Cursor = System.Windows.Forms.Cursors.Hand;
			this.imageBox.GridDisplayMode = Waveface.Component.ImageBoxGridDisplayMode.None;
			this.imageBox.Name = "imageBox";
			this.imageBox.SizeToFit = true;
			this.imageBox.ZoomIncrement = 10;
			this.imageBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.imageBox_MouseDown);
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
			// cultureManager
			// 
			this.cultureManager.ManagedControl = this;
			// 
			// StatusLabelFileName
			// 
			this.StatusLabelFileName.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
			this.StatusLabelFileName.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
			this.StatusLabelFileName.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.StatusLabelFileName.Name = "StatusLabelFileName";
			resources.ApplyResources(this.StatusLabelFileName, "StatusLabelFileName");
			// 
			// StatusLabelCurrentSize
			// 
			this.StatusLabelCurrentSize.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
			this.StatusLabelCurrentSize.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
			this.StatusLabelCurrentSize.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.StatusLabelCurrentSize.Name = "StatusLabelCurrentSize";
			resources.ApplyResources(this.StatusLabelCurrentSize, "StatusLabelCurrentSize");
			// 
			// imageSizeToolStripStatusLabel
			// 
			this.imageSizeToolStripStatusLabel.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
			this.imageSizeToolStripStatusLabel.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
			this.imageSizeToolStripStatusLabel.Image = global::Waveface.Properties.Resources.ObjectSize;
			this.imageSizeToolStripStatusLabel.Name = "imageSizeToolStripStatusLabel";
			resources.ApplyResources(this.imageSizeToolStripStatusLabel, "imageSizeToolStripStatusLabel");
			// 
			// positionToolStripStatusLabel
			// 
			this.positionToolStripStatusLabel.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
			this.positionToolStripStatusLabel.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
			this.positionToolStripStatusLabel.Image = global::Waveface.Properties.Resources.ObjectPosition;
			this.positionToolStripStatusLabel.Name = "positionToolStripStatusLabel";
			resources.ApplyResources(this.positionToolStripStatusLabel, "positionToolStripStatusLabel");
			// 
			// zoomToolStripStatusLabel
			// 
			this.zoomToolStripStatusLabel.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
			this.zoomToolStripStatusLabel.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
			this.zoomToolStripStatusLabel.Image = global::Waveface.Properties.Resources.Magnifier;
			this.zoomToolStripStatusLabel.Name = "zoomToolStripStatusLabel";
			resources.ApplyResources(this.zoomToolStripStatusLabel, "zoomToolStripStatusLabel");
			// 
			// StatusLabel
			// 
			this.StatusLabel.Name = "StatusLabel";
			resources.ApplyResources(this.StatusLabel, "StatusLabel");
			// 
			// statusStrip
			// 
			resources.ApplyResources(this.statusStrip, "statusStrip");
			this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusLabelFileName,
            this.StatusLabelCurrentSize,
            this.imageSizeToolStripStatusLabel,
            this.positionToolStripStatusLabel,
            this.zoomToolStripStatusLabel,
            this.StatusLabel});
			this.statusStrip.Name = "statusStrip";
			// 
			// PhotoView
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.panelBottom);
			this.Controls.Add(this.imageBox);
			this.Controls.Add(this.thumbnailNavigator1);
			this.Controls.Add(this.statusStrip);
			this.KeyPreview = true;
			this.MinimizeBox = false;
			this.Name = "PhotoView";
			this.ShowInTaskbar = false;
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PhotoView_KeyDown);
			this.Resize += new System.EventHandler(this.PhotoView_Resize);
			this.contextMenuStrip.ResumeLayout(false);
			this.panelBottom.ResumeLayout(false);
			this.panelBottom.PerformLayout();
			this.pnlPhotoViewToolBar.ResumeLayout(false);
			this.statusStrip.ResumeLayout(false);
			this.statusStrip.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem miSave;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private Localization.CultureManager cultureManager;
		private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.ToolStripMenuItem miSetAsCoverImage;
		private System.Windows.Forms.Panel panelBottom;
		private ImageBox imageBox;
		private ThumbnailNavigator thumbnailNavigator1;
		private System.Windows.Forms.Panel pnlPhotoViewToolBar;
		private ImageButton imageButton1;
		private ImageButton btnSave;
		private ImageButton btnCoverImage;
		private ImageButton btnSlideShow;
		private System.Windows.Forms.StatusStrip statusStrip;
		private System.Windows.Forms.ToolStripStatusLabel StatusLabelFileName;
		private System.Windows.Forms.ToolStripStatusLabel StatusLabelCurrentSize;
		private System.Windows.Forms.ToolStripStatusLabel imageSizeToolStripStatusLabel;
		private System.Windows.Forms.ToolStripStatusLabel positionToolStripStatusLabel;
		private System.Windows.Forms.ToolStripStatusLabel zoomToolStripStatusLabel;
		private System.Windows.Forms.ToolStripStatusLabel StatusLabel;
    }
}

