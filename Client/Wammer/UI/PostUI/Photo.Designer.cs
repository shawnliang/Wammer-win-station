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
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.removeAllToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.addToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.removeToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.rotateCCWToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.rotateCWToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonCamera = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripComboBoxResize = new System.Windows.Forms.ToolStripComboBox();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.panelToolbar = new System.Windows.Forms.Panel();
            this.btnDeletePhoto = new Waveface.Component.XPButton();
            this.btnAddPhoto = new Waveface.Component.XPButton();
            this.btnSend = new Waveface.Component.XPButton();
            this.btnBatchPost = new Waveface.Component.XPButton();
            this.cultureManager = new Waveface.Localization.CultureManager(this.components);
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.panel.SuspendLayout();
            this.columnContextMenu.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.panelToolbar.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel
            // 
            resources.ApplyResources(this.panel, "panel");
            this.panel.Controls.Add(this.imageListView);
            this.panel.Name = "panel";
            // 
            // imageListView
            // 
            this.imageListView.AllowDrag = true;
            this.imageListView.AllowDrop = true;
            resources.ApplyResources(this.imageListView, "imageListView");
            this.imageListView.ColumnHeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.imageListView.Columns.AddRange(new Manina.Windows.Forms.ImageListView.ImageListViewColumnHeader[] {
            new Manina.Windows.Forms.ImageListView.ImageListViewColumnHeader(Manina.Windows.Forms.ColumnType.Name, "", 100, 0, true),
            new Manina.Windows.Forms.ImageListView.ImageListViewColumnHeader(Manina.Windows.Forms.ColumnType.DateTaken, "", 100, 1, true),
            new Manina.Windows.Forms.ImageListView.ImageListViewColumnHeader(Manina.Windows.Forms.ColumnType.Dimensions, "", 100, 2, true),
            new Manina.Windows.Forms.ImageListView.ImageListViewColumnHeader(Manina.Windows.Forms.ColumnType.FileSize, "", 100, 3, true),
            new Manina.Windows.Forms.ImageListView.ImageListViewColumnHeader(Manina.Windows.Forms.ColumnType.DateCreated, "", 100, 4, true),
            new Manina.Windows.Forms.ImageListView.ImageListViewColumnHeader(Manina.Windows.Forms.ColumnType.DateModified, "", 100, 5, true)});
            this.imageListView.ContextMenuStrip = this.columnContextMenu;
            this.imageListView.DefaultImage = ((System.Drawing.Image)(resources.GetObject("imageListView.DefaultImage")));
            this.imageListView.ErrorImage = ((System.Drawing.Image)(resources.GetObject("imageListView.ErrorImage")));
            this.imageListView.GroupHeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.imageListView.Name = "imageListView";
            this.imageListView.ThumbnailSize = new System.Drawing.Size(120, 120);
            this.imageListView.ItemCollectionChanged += new Manina.Windows.Forms.ItemCollectionChangedEventHandler(this.imageListView_ItemCollectionChanged);
            // 
            // columnContextMenu
            // 
            this.columnContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sortByToolStripMenuItem});
            this.columnContextMenu.Name = "columnContextMenu";
            resources.ApplyResources(this.columnContextMenu, "columnContextMenu");
            this.columnContextMenu.Closing += new System.Windows.Forms.ToolStripDropDownClosingEventHandler(this.columnContextMenu_Closing);
            this.columnContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.columnContextMenu_Opening);
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
            // toolStrip
            // 
            resources.ApplyResources(this.toolStrip, "toolStrip");
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removeAllToolStripButton,
            this.addToolStripButton,
            this.removeToolStripButton,
            this.toolStripSeparator1,
            this.rotateCCWToolStripButton,
            this.rotateCWToolStripButton,
            this.toolStripSeparator5,
            this.toolStripButtonCamera,
            this.toolStripSeparator6,
            this.toolStripLabel1,
            this.toolStripComboBoxResize});
            this.toolStrip.Name = "toolStrip";
            // 
            // removeAllToolStripButton
            // 
            this.removeAllToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.removeAllToolStripButton, "removeAllToolStripButton");
            this.removeAllToolStripButton.Name = "removeAllToolStripButton";
            this.removeAllToolStripButton.Click += new System.EventHandler(this.removeAllToolStripButton_Click);
            // 
            // addToolStripButton
            // 
            this.addToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.addToolStripButton, "addToolStripButton");
            this.addToolStripButton.Name = "addToolStripButton";
            this.addToolStripButton.Click += new System.EventHandler(this.addToolStripButton_Click);
            // 
            // removeToolStripButton
            // 
            this.removeToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.removeToolStripButton, "removeToolStripButton");
            this.removeToolStripButton.Name = "removeToolStripButton";
            this.removeToolStripButton.Click += new System.EventHandler(this.removeToolStripButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // rotateCCWToolStripButton
            // 
            this.rotateCCWToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.rotateCCWToolStripButton, "rotateCCWToolStripButton");
            this.rotateCCWToolStripButton.Name = "rotateCCWToolStripButton";
            this.rotateCCWToolStripButton.Click += new System.EventHandler(this.rotateCCWToolStripButton_Click);
            // 
            // rotateCWToolStripButton
            // 
            this.rotateCWToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.rotateCWToolStripButton, "rotateCWToolStripButton");
            this.rotateCWToolStripButton.Name = "rotateCWToolStripButton";
            this.rotateCWToolStripButton.Click += new System.EventHandler(this.rotateCWToolStripButton_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            resources.ApplyResources(this.toolStripSeparator5, "toolStripSeparator5");
            // 
            // toolStripButtonCamera
            // 
            this.toolStripButtonCamera.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonCamera.Image = global::Waveface.Properties.Resources.webcam;
            resources.ApplyResources(this.toolStripButtonCamera, "toolStripButtonCamera");
            this.toolStripButtonCamera.Name = "toolStripButtonCamera";
            this.toolStripButtonCamera.Click += new System.EventHandler(this.toolStripButtonCamera_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            resources.ApplyResources(this.toolStripSeparator6, "toolStripSeparator6");
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            resources.ApplyResources(this.toolStripLabel1, "toolStripLabel1");
            // 
            // toolStripComboBoxResize
            // 
            this.toolStripComboBoxResize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolStripComboBoxResize.Items.AddRange(new object[] {
            resources.GetString("toolStripComboBoxResize.Items"),
            resources.GetString("toolStripComboBoxResize.Items1"),
            resources.GetString("toolStripComboBoxResize.Items2"),
            resources.GetString("toolStripComboBoxResize.Items3"),
            resources.GetString("toolStripComboBoxResize.Items4"),
            resources.GetString("toolStripComboBoxResize.Items5"),
            resources.GetString("toolStripComboBoxResize.Items6"),
            resources.GetString("toolStripComboBoxResize.Items7"),
            resources.GetString("toolStripComboBoxResize.Items8"),
            resources.GetString("toolStripComboBoxResize.Items9"),
            resources.GetString("toolStripComboBoxResize.Items10"),
            resources.GetString("toolStripComboBoxResize.Items11"),
            resources.GetString("toolStripComboBoxResize.Items12"),
            resources.GetString("toolStripComboBoxResize.Items13"),
            resources.GetString("toolStripComboBoxResize.Items14"),
            resources.GetString("toolStripComboBoxResize.Items15"),
            resources.GetString("toolStripComboBoxResize.Items16"),
            resources.GetString("toolStripComboBoxResize.Items17"),
            resources.GetString("toolStripComboBoxResize.Items18"),
            resources.GetString("toolStripComboBoxResize.Items19"),
            resources.GetString("toolStripComboBoxResize.Items20"),
            resources.GetString("toolStripComboBoxResize.Items21")});
            this.toolStripComboBoxResize.Name = "toolStripComboBoxResize";
            resources.ApplyResources(this.toolStripComboBoxResize, "toolStripComboBoxResize");
            // 
            // openFileDialog
            // 
            resources.ApplyResources(this.openFileDialog, "openFileDialog");
            this.openFileDialog.Multiselect = true;
            this.openFileDialog.ShowReadOnly = true;
            // 
            // panelToolbar
            // 
            resources.ApplyResources(this.panelToolbar, "panelToolbar");
            this.panelToolbar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(202)))), ((int)(((byte)(197)))), ((int)(((byte)(186)))));
            this.panelToolbar.Controls.Add(this.btnDeletePhoto);
            this.panelToolbar.Controls.Add(this.btnAddPhoto);
            this.panelToolbar.Name = "panelToolbar";
            // 
            // btnDeletePhoto
            // 
            this.btnDeletePhoto.AdjustImageLocation = new System.Drawing.Point(0, 0);
            resources.ApplyResources(this.btnDeletePhoto, "btnDeletePhoto");
            this.btnDeletePhoto.BtnShape = Waveface.Component.emunType.BtnShape.Rectangle;
            this.btnDeletePhoto.BtnStyle = Waveface.Component.emunType.XPStyle.Silver;
            this.btnDeletePhoto.Image = global::Waveface.Properties.Resources.trash;
            this.btnDeletePhoto.Name = "btnDeletePhoto";
            this.toolTip.SetToolTip(this.btnDeletePhoto, resources.GetString("btnDeletePhoto.ToolTip"));
            this.btnDeletePhoto.UseVisualStyleBackColor = true;
            this.btnDeletePhoto.Click += new System.EventHandler(this.btnDeletePhoto_Click);
            // 
            // btnAddPhoto
            // 
            this.btnAddPhoto.AdjustImageLocation = new System.Drawing.Point(0, 0);
            this.btnAddPhoto.BtnShape = Waveface.Component.emunType.BtnShape.Rectangle;
            this.btnAddPhoto.BtnStyle = Waveface.Component.emunType.XPStyle.Silver;
            this.btnAddPhoto.Image = global::Waveface.Properties.Resources.add_photo;
            resources.ApplyResources(this.btnAddPhoto, "btnAddPhoto");
            this.btnAddPhoto.Name = "btnAddPhoto";
            this.toolTip.SetToolTip(this.btnAddPhoto, resources.GetString("btnAddPhoto.ToolTip"));
            this.btnAddPhoto.UseVisualStyleBackColor = true;
            this.btnAddPhoto.Click += new System.EventHandler(this.btnAddPhoto_Click);
            // 
            // btnSend
            // 
            this.btnSend.AdjustImageLocation = new System.Drawing.Point(0, 0);
            resources.ApplyResources(this.btnSend, "btnSend");
            this.btnSend.BtnShape = Waveface.Component.emunType.BtnShape.Rectangle;
            this.btnSend.BtnStyle = Waveface.Component.emunType.XPStyle.Silver;
            this.btnSend.Name = "btnSend";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // btnBatchPost
            // 
            this.btnBatchPost.AdjustImageLocation = new System.Drawing.Point(0, 0);
            resources.ApplyResources(this.btnBatchPost, "btnBatchPost");
            this.btnBatchPost.BtnShape = Waveface.Component.emunType.BtnShape.Rectangle;
            this.btnBatchPost.BtnStyle = Waveface.Component.emunType.XPStyle.Silver;
            this.btnBatchPost.Image = global::Waveface.Properties.Resources.arrow_divide;
            this.btnBatchPost.Name = "btnBatchPost";
            this.btnBatchPost.UseVisualStyleBackColor = true;
            this.btnBatchPost.Click += new System.EventHandler(this.btnBatchPost_Click);
            // 
            // cultureManager
            // 
            this.cultureManager.ManagedControl = this;
            // 
            // Photo
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(242)))), ((int)(((byte)(238)))));
            this.Controls.Add(this.panelToolbar);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.panel);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.btnBatchPost);
            this.Name = "Photo";
            this.Resize += new System.EventHandler(this.Photo_Resize);
            this.panel.ResumeLayout(false);
            this.columnContextMenu.ResumeLayout(false);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.panelToolbar.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Component.XPButton btnBatchPost;
        private Component.XPButton btnSend;
        private System.Windows.Forms.Panel panel;
        private Manina.Windows.Forms.ImageListView imageListView;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton addToolStripButton;
        private System.Windows.Forms.ToolStripButton removeToolStripButton;
        private System.Windows.Forms.ToolStripButton removeAllToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton rotateCCWToolStripButton;
        private System.Windows.Forms.ToolStripButton rotateCWToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripButton toolStripButtonCamera;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBoxResize;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Panel panelToolbar;
        private Component.XPButton btnAddPhoto;
        private Component.XPButton btnDeletePhoto;
        private System.Windows.Forms.ContextMenuStrip columnContextMenu;
        private System.Windows.Forms.ToolStripMenuItem sortByToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem6;
        private System.Windows.Forms.ToolStripMenuItem sortAscendingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sortDescendingToolStripMenuItem;
        private Localization.CultureManager cultureManager;
        private System.Windows.Forms.ToolTip toolTip;
    }
}
