namespace Waveface.PostUI
{
    partial class Photo
    {
        /// <summary> 
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();

				//if (m_imageListViewRenderer != null)
				//{
				//    m_imageListViewRenderer.Dispose();
				//    m_imageListViewRenderer = null;
				//}
            }
            base.Dispose(disposing);
        }

        #region 元件設計工具產生的程式碼

        /// <summary> 
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Photo));
			this.panel = new System.Windows.Forms.Panel();
			this.imageListView = new Manina.Windows.Forms.ImageListView();
			this.columnContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.sortByToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripSeparator();
			this.sortAscendingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.sortDescendingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.miSetCoverImage = new System.Windows.Forms.ToolStripMenuItem();
			this.panelToolbar = new System.Windows.Forms.Panel();
			this.labelSummary = new System.Windows.Forms.Label();
			this.btnDeletePhoto = new Waveface.Component.ImageButton();
			this.btnAddPhoto = new Waveface.Component.ImageButton();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.btnSend = new Waveface.Component.ImageButton();
			this.panel.SuspendLayout();
			this.columnContextMenu.SuspendLayout();
			this.panelToolbar.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel
			// 
			resources.ApplyResources(this.panel, "panel");
			this.panel.Controls.Add(this.imageListView);
			this.panel.Controls.Add(this.panelToolbar);
			this.panel.Name = "panel";
			// 
			// imageListView
			// 
			this.imageListView.AllowDrag = true;
			this.imageListView.AllowDrop = true;
			resources.ApplyResources(this.imageListView, "imageListView");
			this.imageListView.ColumnHeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
			this.imageListView.ContextMenuStrip = this.columnContextMenu;
			this.imageListView.DefaultImage = ((System.Drawing.Image)(resources.GetObject("imageListView.DefaultImage")));
			this.imageListView.ErrorImage = ((System.Drawing.Image)(resources.GetObject("imageListView.ErrorImage")));
			this.imageListView.GroupHeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
			this.imageListView.IsWaveface = true;
			this.imageListView.Name = "imageListView";
			this.imageListView.ThumbnailSize = new System.Drawing.Size(120, 120);
			this.imageListView.DropFiles += new Manina.Windows.Forms.DropFilesEventHandler(this.imageListView_DropFiles);
			this.imageListView.ItemClick += new Manina.Windows.Forms.ItemClickEventHandler(this.imageListView_ItemClick);
			this.imageListView.ItemHover += new Manina.Windows.Forms.ItemHoverEventHandler(this.imageListView_ItemHover);
			this.imageListView.ItemCollectionChanged += new Manina.Windows.Forms.ItemCollectionChangedEventHandler(this.imageListView_ItemCollectionChanged);
			this.imageListView.DragDrop += new System.Windows.Forms.DragEventHandler(this.imageListView_DragDrop);
			this.imageListView.DragEnter += new System.Windows.Forms.DragEventHandler(this.imageListView_DragEnter);
			this.imageListView.DragOver += new System.Windows.Forms.DragEventHandler(this.imageListView_DragOver);
			this.imageListView.DragLeave += new System.EventHandler(this.imageListView_DragLeave);
			// 
			// columnContextMenu
			// 
			this.columnContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sortByToolStripMenuItem,
            this.miSetCoverImage});
			this.columnContextMenu.Name = "columnContextMenu";
			resources.ApplyResources(this.columnContextMenu, "columnContextMenu");
			// 
			// sortByToolStripMenuItem
			// 
			this.sortByToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem6,
            this.sortAscendingToolStripMenuItem,
            this.sortDescendingToolStripMenuItem});
			this.sortByToolStripMenuItem.Name = "sortByToolStripMenuItem";
			resources.ApplyResources(this.sortByToolStripMenuItem, "sortByToolStripMenuItem");
			// 
			// toolStripMenuItem6
			// 
			this.toolStripMenuItem6.Name = "toolStripMenuItem6";
			resources.ApplyResources(this.toolStripMenuItem6, "toolStripMenuItem6");
			// 
			// sortAscendingToolStripMenuItem
			// 
			this.sortAscendingToolStripMenuItem.Name = "sortAscendingToolStripMenuItem";
			resources.ApplyResources(this.sortAscendingToolStripMenuItem, "sortAscendingToolStripMenuItem");
			this.sortAscendingToolStripMenuItem.Click += new System.EventHandler(this.sortAscendingToolStripMenuItem_Click);
			// 
			// sortDescendingToolStripMenuItem
			// 
			this.sortDescendingToolStripMenuItem.Name = "sortDescendingToolStripMenuItem";
			resources.ApplyResources(this.sortDescendingToolStripMenuItem, "sortDescendingToolStripMenuItem");
			this.sortDescendingToolStripMenuItem.Click += new System.EventHandler(this.sortDescendingToolStripMenuItem_Click);
			// 
			// miSetCoverImage
			// 
			this.miSetCoverImage.Name = "miSetCoverImage";
			resources.ApplyResources(this.miSetCoverImage, "miSetCoverImage");
			this.miSetCoverImage.Click += new System.EventHandler(this.miSetCoverImage_Click);
			// 
			// panelToolbar
			// 
			resources.ApplyResources(this.panelToolbar, "panelToolbar");
			this.panelToolbar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
			this.panelToolbar.Controls.Add(this.labelSummary);
			this.panelToolbar.Controls.Add(this.btnDeletePhoto);
			this.panelToolbar.Controls.Add(this.btnAddPhoto);
			this.panelToolbar.Name = "panelToolbar";
			// 
			// labelSummary
			// 
			resources.ApplyResources(this.labelSummary, "labelSummary");
			this.labelSummary.AutoEllipsis = true;
			this.labelSummary.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(97)))), ((int)(((byte)(101)))));
			this.labelSummary.Name = "labelSummary";
			// 
			// btnDeletePhoto
			// 
			this.btnDeletePhoto.CenterAlignImage = false;
			resources.ApplyResources(this.btnDeletePhoto, "btnDeletePhoto");
			this.btnDeletePhoto.ForeColor = System.Drawing.Color.White;
			this.btnDeletePhoto.Image = global::Waveface.Properties.Resources.FB_blue_btn;
			this.btnDeletePhoto.ImageDisable = global::Waveface.Properties.Resources.FB_blue_btn_hl;
			this.btnDeletePhoto.ImageFront = global::Waveface.Properties.Resources.FB_edit_delete;
			this.btnDeletePhoto.ImageHover = global::Waveface.Properties.Resources.FB_blue_btn_hl;
			this.btnDeletePhoto.Name = "btnDeletePhoto";
			this.btnDeletePhoto.TextShadow = true;
			this.toolTip.SetToolTip(this.btnDeletePhoto, resources.GetString("btnDeletePhoto.ToolTip"));
			this.btnDeletePhoto.Click += new System.EventHandler(this.btnDeletePhoto_Click);
			// 
			// btnAddPhoto
			// 
			this.btnAddPhoto.CenterAlignImage = false;
			resources.ApplyResources(this.btnAddPhoto, "btnAddPhoto");
			this.btnAddPhoto.ForeColor = System.Drawing.Color.White;
			this.btnAddPhoto.Image = global::Waveface.Properties.Resources.FB_blue_btn;
			this.btnAddPhoto.ImageDisable = global::Waveface.Properties.Resources.FB_blue_btn_hl;
			this.btnAddPhoto.ImageFront = global::Waveface.Properties.Resources.FB_edit_add_photo;
			this.btnAddPhoto.ImageHover = global::Waveface.Properties.Resources.FB_blue_btn_hl;
			this.btnAddPhoto.Name = "btnAddPhoto";
			this.btnAddPhoto.TextShadow = true;
			this.toolTip.SetToolTip(this.btnAddPhoto, resources.GetString("btnAddPhoto.ToolTip"));
			this.btnAddPhoto.Click += new System.EventHandler(this.btnAddPhoto_Click);
			// 
			// openFileDialog
			// 
			resources.ApplyResources(this.openFileDialog, "openFileDialog");
			this.openFileDialog.Multiselect = true;
			this.openFileDialog.ShowReadOnly = true;
			// 
			// btnSend
			// 
			resources.ApplyResources(this.btnSend, "btnSend");
			this.btnSend.CenterAlignImage = false;
			this.btnSend.ForeColor = System.Drawing.Color.White;
			this.btnSend.Image = global::Waveface.Properties.Resources.FB_creat_btn;
			this.btnSend.ImageDisable = global::Waveface.Properties.Resources.FB_creat_btn_hl;
			this.btnSend.ImageFront = null;
			this.btnSend.ImageHover = global::Waveface.Properties.Resources.FB_creat_btn_hl;
			this.btnSend.Name = "btnSend";
			this.btnSend.TextShadow = true;
			this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
			// 
			// Photo
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
			this.Controls.Add(this.panel);
			this.Controls.Add(this.btnSend);
			resources.ApplyResources(this, "$this");
			this.Name = "Photo";
			this.Resize += new System.EventHandler(this.Photo_Resize);
			this.panel.ResumeLayout(false);
			this.columnContextMenu.ResumeLayout(false);
			this.panelToolbar.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private Component.ImageButton btnSend;
        private System.Windows.Forms.Panel panel;
		private Manina.Windows.Forms.ImageListView imageListView;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Panel panelToolbar;
        private Component.ImageButton btnAddPhoto;
        private Component.ImageButton btnDeletePhoto;
        private System.Windows.Forms.ContextMenuStrip columnContextMenu;
        private System.Windows.Forms.ToolStripMenuItem sortByToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem6;
        private System.Windows.Forms.ToolStripMenuItem sortAscendingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sortDescendingToolStripMenuItem;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Label labelSummary;
        private System.Windows.Forms.ToolStripMenuItem miSetCoverImage;
    }
}