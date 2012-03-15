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
            this.miSaveAll = new System.Windows.Forms.ToolStripMenuItem();
            this.miSetAsCoverImage = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.cultureManager = new Waveface.Localization.CultureManager(this.components);
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.btnSave = new Waveface.Component.XPButton();
            this.btnSaveAll = new Waveface.Component.XPButton();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.StatusLabelFileName = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusLabelCurrentSize = new System.Windows.Forms.ToolStripStatusLabel();
            this.StatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.panelMain = new System.Windows.Forms.Panel();
            this.btnCoverImage = new Waveface.Component.XPButton();
            this.btnSlideShow = new Waveface.Component.XPButton();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.contextMenuStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.panelMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // imageListView
            // 
            this.imageListView.AllowDuplicateFileNames = true;
            this.imageListView.ColumnHeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.imageListView.ContextMenuStrip = this.contextMenuStrip;
            resources.ApplyResources(this.imageListView, "imageListView");
            this.imageListView.GroupHeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.imageListView.Name = "imageListView";
            this.imageListView.SelectionChanged += new System.EventHandler(this.imageListView_SelectionChanged);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miSave,
            this.miSaveAll,
            this.miSetAsCoverImage});
            this.contextMenuStrip.Name = "contextMenuStrip";
            resources.ApplyResources(this.contextMenuStrip, "contextMenuStrip");
            // 
            // miSave
            // 
            this.miSave.Image = global::Waveface.Properties.Resources.Save;
            this.miSave.Name = "miSave";
            resources.ApplyResources(this.miSave, "miSave");
            this.miSave.Click += new System.EventHandler(this.miSave_Click);
            // 
            // miSaveAll
            // 
            this.miSaveAll.Image = global::Waveface.Properties.Resources.SaveAll;
            this.miSaveAll.Name = "miSaveAll";
            resources.ApplyResources(this.miSaveAll, "miSaveAll");
            this.miSaveAll.Click += new System.EventHandler(this.miSaveAll_Click);
            // 
            // miSetAsCoverImage
            // 
            this.miSetAsCoverImage.Image = global::Waveface.Properties.Resources.CoverImage;
            this.miSetAsCoverImage.Name = "miSetAsCoverImage";
            resources.ApplyResources(this.miSetAsCoverImage, "miSetAsCoverImage");
            this.miSetAsCoverImage.Click += new System.EventHandler(this.miSetAsCoverImage_Click);
            // 
            // cultureManager
            // 
            this.cultureManager.ManagedControl = this;
            // 
            // btnSave
            // 
            this.btnSave.AdjustImageLocation = new System.Drawing.Point(0, 0);
            this.btnSave.BtnShape = Waveface.Component.emunType.BtnShape.Rectangle;
            this.btnSave.BtnStyle = Waveface.Component.emunType.XPStyle.Silver;
            this.btnSave.Image = global::Waveface.Properties.Resources.Save;
            resources.ApplyResources(this.btnSave, "btnSave");
            this.btnSave.Name = "btnSave";
            this.toolTip.SetToolTip(this.btnSave, resources.GetString("btnSave.ToolTip"));
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnSaveAll
            // 
            this.btnSaveAll.AdjustImageLocation = new System.Drawing.Point(0, 0);
            this.btnSaveAll.BtnShape = Waveface.Component.emunType.BtnShape.Rectangle;
            this.btnSaveAll.BtnStyle = Waveface.Component.emunType.XPStyle.Silver;
            this.btnSaveAll.Image = global::Waveface.Properties.Resources.SaveAll;
            resources.ApplyResources(this.btnSaveAll, "btnSaveAll");
            this.btnSaveAll.Name = "btnSaveAll";
            this.toolTip.SetToolTip(this.btnSaveAll, resources.GetString("btnSaveAll.ToolTip"));
            this.btnSaveAll.UseVisualStyleBackColor = true;
            this.btnSaveAll.Click += new System.EventHandler(this.btnSaveAll_Click);
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
            this.panelMain.Controls.Add(this.btnSaveAll);
            this.panelMain.Controls.Add(this.imageListView);
            this.panelMain.Name = "panelMain";
            // 
            // btnCoverImage
            // 
            this.btnCoverImage.AdjustImageLocation = new System.Drawing.Point(0, 0);
            resources.ApplyResources(this.btnCoverImage, "btnCoverImage");
            this.btnCoverImage.BtnShape = Waveface.Component.emunType.BtnShape.Rectangle;
            this.btnCoverImage.BtnStyle = Waveface.Component.emunType.XPStyle.Silver;
            this.btnCoverImage.Image = global::Waveface.Properties.Resources.CoverImage;
            this.btnCoverImage.Name = "btnCoverImage";
            this.btnCoverImage.UseVisualStyleBackColor = true;
            this.btnCoverImage.Click += new System.EventHandler(this.btnCoverImage_Click);
            // 
            // btnSlideShow
            // 
            this.btnSlideShow.AdjustImageLocation = new System.Drawing.Point(0, 0);
            resources.ApplyResources(this.btnSlideShow, "btnSlideShow");
            this.btnSlideShow.BtnShape = Waveface.Component.emunType.BtnShape.Rectangle;
            this.btnSlideShow.BtnStyle = Waveface.Component.emunType.XPStyle.Silver;
            this.btnSlideShow.Image = global::Waveface.Properties.Resources.SlideShow;
            this.btnSlideShow.Name = "btnSlideShow";
            this.btnSlideShow.UseVisualStyleBackColor = true;
            this.btnSlideShow.Click += new System.EventHandler(this.btnSlideShow_Click);
            // 
            // timer
            // 
            this.timer.Interval = 3000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
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
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
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
        private System.Windows.Forms.ToolStripMenuItem miSaveAll;
        private XPButton btnSave;
        private XPButton btnSaveAll;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private Localization.CultureManager cultureManager;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabelFileName;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabelCurrentSize;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.ToolStripStatusLabel StatusLabel;
        private XPButton btnSlideShow;
        private XPButton btnCoverImage;
        private System.Windows.Forms.ToolStripMenuItem miSetAsCoverImage;
    }
}

