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
            this.imageListView = new Manina.Windows.Forms.ImageListView();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miSave = new System.Windows.Forms.ToolStripMenuItem();
            this.miSetAsCoverImage = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.cultureManager = new Waveface.Localization.CultureManager(this.components);
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.StatusLabelFileName = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusLabelCurrentSize = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.panelMain = new System.Windows.Forms.Panel();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.btnCoverImage = new Waveface.Component.ImageButton();
            this.btnSlideShow = new Waveface.Component.ImageButton();
            this.btnSave = new Waveface.Component.ImageButton();
            this.contextMenuStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.panelMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // imageListView
            // 
            this.imageListView.AllowDuplicateFileNames = true;
            this.imageListView.BackColor = System.Drawing.Color.White;
            this.imageListView.Colors = new Manina.Windows.Forms.ImageListViewColor(resources.GetString("imageListView.Colors"));
            this.imageListView.ColumnHeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.imageListView.ContextMenuStrip = this.contextMenuStrip;
            this.imageListView.GroupHeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            resources.ApplyResources(this.imageListView, "imageListView");
            this.imageListView.Name = "imageListView";
            this.imageListView.SelectionChanged += new System.EventHandler(this.imageListView_SelectionChanged);
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
            // cultureManager
            // 
            this.cultureManager.ManagedControl = this;
            // 
            // statusStrip
            // 
            resources.ApplyResources(this.statusStrip, "statusStrip");
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusLabelFileName,
            this.StatusLabelCurrentSize,
            this.StatusLabel});
            this.statusStrip.Name = "statusStrip";
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
            // StatusLabel
            // 
            this.StatusLabel.Name = "StatusLabel";
            resources.ApplyResources(this.StatusLabel, "StatusLabel");
            // 
            // panelMain
            // 
            resources.ApplyResources(this.panelMain, "panelMain");
            this.panelMain.Controls.Add(this.btnCoverImage);
            this.panelMain.Controls.Add(this.btnSlideShow);
            this.panelMain.Controls.Add(this.btnSave);
            this.panelMain.Controls.Add(this.imageListView);
            this.panelMain.Name = "panelMain";
            // 
            // timer
            // 
            this.timer.Interval = 3000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // btnCoverImage
            // 
            resources.ApplyResources(this.btnCoverImage, "btnCoverImage");
            this.btnCoverImage.BackColor = System.Drawing.Color.White;
            this.btnCoverImage.CenterAlignImage = false;
            this.btnCoverImage.Image = global::Waveface.Properties.Resources.FB_cover;
            this.btnCoverImage.ImageDisable = global::Waveface.Properties.Resources.FB_cover_hl;
            this.btnCoverImage.ImageFront = null;
            this.btnCoverImage.ImageHover = global::Waveface.Properties.Resources.FB_cover_hl;
            this.btnCoverImage.Name = "btnCoverImage";
            this.toolTip.SetToolTip(this.btnCoverImage, resources.GetString("btnCoverImage.ToolTip"));
            this.btnCoverImage.UseVisualStyleBackColor = false;
            this.btnCoverImage.Click += new System.EventHandler(this.btnCoverImage_Click);
            // 
            // btnSlideShow
            // 
            resources.ApplyResources(this.btnSlideShow, "btnSlideShow");
            this.btnSlideShow.BackColor = System.Drawing.Color.White;
            this.btnSlideShow.CenterAlignImage = false;
            this.btnSlideShow.Image = global::Waveface.Properties.Resources.FB_slide1;
            this.btnSlideShow.ImageDisable = global::Waveface.Properties.Resources.FB_slide1_hl;
            this.btnSlideShow.ImageFront = null;
            this.btnSlideShow.ImageHover = global::Waveface.Properties.Resources.FB_slide1_hl;
            this.btnSlideShow.Name = "btnSlideShow";
            this.toolTip.SetToolTip(this.btnSlideShow, resources.GetString("btnSlideShow.ToolTip"));
            this.btnSlideShow.UseVisualStyleBackColor = false;
            this.btnSlideShow.Click += new System.EventHandler(this.btnSlideShow_Click);
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.White;
            this.btnSave.CenterAlignImage = false;
            this.btnSave.Image = global::Waveface.Properties.Resources.FB_save;
            this.btnSave.ImageDisable = global::Waveface.Properties.Resources.FB_save_hl;
            this.btnSave.ImageFront = null;
            this.btnSave.ImageHover = global::Waveface.Properties.Resources.FB_save_hl;
            resources.ApplyResources(this.btnSave, "btnSave");
            this.btnSave.Name = "btnSave";
            this.toolTip.SetToolTip(this.btnSave, resources.GetString("btnSave.ToolTip"));
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // PhotoView
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.panelMain);
            this.MinimizeBox = false;
            this.Name = "PhotoView";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.PhotoView_Load);
            this.Resize += new System.EventHandler(this.PhotoView_Resize);
            this.contextMenuStrip.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.panelMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ImageListView imageListView;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem miSave;
        private Component.ImageButton btnSave;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private Localization.CultureManager cultureManager;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabelFileName;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabelCurrentSize;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabel;
        private Component.ImageButton btnSlideShow;
        private Component.ImageButton btnCoverImage;
        private System.Windows.Forms.ToolStripMenuItem miSetAsCoverImage;
    }
}

