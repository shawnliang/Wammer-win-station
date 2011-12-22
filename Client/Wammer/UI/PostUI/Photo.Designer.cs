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
            this.panel.SuspendLayout();
            this.columnContextMenu.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.panelToolbar.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel
            // 
            this.panel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel.Controls.Add(this.imageListView);
            this.panel.Location = new System.Drawing.Point(6, 0);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(606, 233);
            this.panel.TabIndex = 8;
            // 
            // imageListView
            // 
            this.imageListView.AllowDrag = true;
            this.imageListView.AllowDrop = true;
            this.imageListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
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
            this.imageListView.Location = new System.Drawing.Point(2, 40);
            this.imageListView.Name = "imageListView";
            this.imageListView.Size = new System.Drawing.Size(603, 194);
            this.imageListView.TabIndex = 2;
            this.imageListView.ThumbnailSize = new System.Drawing.Size(120, 120);
            this.imageListView.ItemCollectionChanged += new Manina.Windows.Forms.ItemCollectionChangedEventHandler(this.imageListView_ItemCollectionChanged);
            // 
            // columnContextMenu
            // 
            this.columnContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sortByToolStripMenuItem});
            this.columnContextMenu.Name = "columnContextMenu";
            this.columnContextMenu.Size = new System.Drawing.Size(116, 26);
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
            this.sortByToolStripMenuItem.Size = new System.Drawing.Size(115, 22);
            this.sortByToolStripMenuItem.Text = "Sort By";
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(140, 6);
            // 
            // sortAscendingToolStripMenuItem
            // 
            this.sortAscendingToolStripMenuItem.Name = "sortAscendingToolStripMenuItem";
            this.sortAscendingToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.sortAscendingToolStripMenuItem.Text = "Ascending";
            this.sortAscendingToolStripMenuItem.Click += new System.EventHandler(this.sortAscendingToolStripMenuItem_Click);
            // 
            // sortDescendingToolStripMenuItem
            // 
            this.sortDescendingToolStripMenuItem.Name = "sortDescendingToolStripMenuItem";
            this.sortDescendingToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.sortDescendingToolStripMenuItem.Text = "Descending";
            this.sortDescendingToolStripMenuItem.Click += new System.EventHandler(this.sortDescendingToolStripMenuItem_Click);
            // 
            // toolStrip
            // 
            this.toolStrip.Dock = System.Windows.Forms.DockStyle.None;
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
            this.toolStrip.Location = new System.Drawing.Point(6, 242);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(357, 25);
            this.toolStrip.TabIndex = 1;
            this.toolStrip.Visible = false;
            // 
            // removeAllToolStripButton
            // 
            this.removeAllToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.removeAllToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("removeAllToolStripButton.Image")));
            this.removeAllToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.removeAllToolStripButton.Name = "removeAllToolStripButton";
            this.removeAllToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.removeAllToolStripButton.Text = "Remove All Files";
            this.removeAllToolStripButton.Click += new System.EventHandler(this.removeAllToolStripButton_Click);
            // 
            // addToolStripButton
            // 
            this.addToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.addToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("addToolStripButton.Image")));
            this.addToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.addToolStripButton.Name = "addToolStripButton";
            this.addToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.addToolStripButton.Text = "Add Files...";
            this.addToolStripButton.Click += new System.EventHandler(this.addToolStripButton_Click);
            // 
            // removeToolStripButton
            // 
            this.removeToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.removeToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("removeToolStripButton.Image")));
            this.removeToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.removeToolStripButton.Name = "removeToolStripButton";
            this.removeToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.removeToolStripButton.Text = "Remove Selected Files";
            this.removeToolStripButton.Click += new System.EventHandler(this.removeToolStripButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // rotateCCWToolStripButton
            // 
            this.rotateCCWToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.rotateCCWToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("rotateCCWToolStripButton.Image")));
            this.rotateCCWToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.rotateCCWToolStripButton.Name = "rotateCCWToolStripButton";
            this.rotateCCWToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.rotateCCWToolStripButton.Text = "Rotate Counter-clockwise";
            this.rotateCCWToolStripButton.Click += new System.EventHandler(this.rotateCCWToolStripButton_Click);
            // 
            // rotateCWToolStripButton
            // 
            this.rotateCWToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.rotateCWToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("rotateCWToolStripButton.Image")));
            this.rotateCWToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.rotateCWToolStripButton.Name = "rotateCWToolStripButton";
            this.rotateCWToolStripButton.Size = new System.Drawing.Size(23, 22);
            this.rotateCWToolStripButton.Text = "Rotate Clockwise";
            this.rotateCWToolStripButton.Click += new System.EventHandler(this.rotateCWToolStripButton_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonCamera
            // 
            this.toolStripButtonCamera.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonCamera.Image = global::Waveface.Properties.Resources.webcam;
            this.toolStripButtonCamera.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonCamera.Name = "toolStripButtonCamera";
            this.toolStripButtonCamera.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonCamera.Text = "Camera";
            this.toolStripButtonCamera.Click += new System.EventHandler(this.toolStripButtonCamera_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(47, 22);
            this.toolStripLabel1.Text = "Resize:";
            // 
            // toolStripComboBoxResize
            // 
            this.toolStripComboBoxResize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolStripComboBoxResize.Items.AddRange(new object[] {
            "100%",
            "1024",
            "1000",
            "960",
            "800",
            "640",
            "600",
            "512",
            "500",
            "480",
            "400",
            "360",
            "320",
            "300",
            "256",
            "200",
            "160",
            "128",
            "100",
            "96",
            "64",
            "32"});
            this.toolStripComboBoxResize.Name = "toolStripComboBoxResize";
            this.toolStripComboBoxResize.Size = new System.Drawing.Size(140, 25);
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = resources.GetString("openFileDialog.Filter");
            this.openFileDialog.Multiselect = true;
            this.openFileDialog.ShowReadOnly = true;
            // 
            // panelToolbar
            // 
            this.panelToolbar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelToolbar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(223)))), ((int)(((byte)(220)))), ((int)(((byte)(213)))));
            this.panelToolbar.Controls.Add(this.btnDeletePhoto);
            this.panelToolbar.Controls.Add(this.btnAddPhoto);
            this.panelToolbar.Location = new System.Drawing.Point(9, 0);
            this.panelToolbar.Name = "panelToolbar";
            this.panelToolbar.Size = new System.Drawing.Size(601, 31);
            this.panelToolbar.TabIndex = 15;
            // 
            // btnDeletePhoto
            // 
            this.btnDeletePhoto.AdjustImageLocation = new System.Drawing.Point(0, 0);
            this.btnDeletePhoto.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeletePhoto.BtnShape = Waveface.Component.emunType.BtnShape.Rectangle;
            this.btnDeletePhoto.BtnStyle = Waveface.Component.emunType.XPStyle.Silver;
            this.btnDeletePhoto.Image = global::Waveface.Properties.Resources.trash;
            this.btnDeletePhoto.Location = new System.Drawing.Point(572, 2);
            this.btnDeletePhoto.Name = "btnDeletePhoto";
            this.btnDeletePhoto.Size = new System.Drawing.Size(26, 26);
            this.btnDeletePhoto.TabIndex = 13;
            this.btnDeletePhoto.UseVisualStyleBackColor = true;
            this.btnDeletePhoto.Click += new System.EventHandler(this.btnDeletePhoto_Click);
            // 
            // btnAddPhoto
            // 
            this.btnAddPhoto.AdjustImageLocation = new System.Drawing.Point(0, 0);
            this.btnAddPhoto.BtnShape = Waveface.Component.emunType.BtnShape.Rectangle;
            this.btnAddPhoto.BtnStyle = Waveface.Component.emunType.XPStyle.Silver;
            this.btnAddPhoto.Image = global::Waveface.Properties.Resources.add_photo;
            this.btnAddPhoto.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAddPhoto.Location = new System.Drawing.Point(4, 2);
            this.btnAddPhoto.Name = "btnAddPhoto";
            this.btnAddPhoto.Size = new System.Drawing.Size(40, 26);
            this.btnAddPhoto.TabIndex = 12;
            this.btnAddPhoto.UseVisualStyleBackColor = true;
            this.btnAddPhoto.Click += new System.EventHandler(this.btnAddPhoto_Click);
            // 
            // btnSend
            // 
            this.btnSend.AdjustImageLocation = new System.Drawing.Point(0, 0);
            this.btnSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSend.BtnShape = Waveface.Component.emunType.BtnShape.Rectangle;
            this.btnSend.BtnStyle = Waveface.Component.emunType.XPStyle.Silver;
            this.btnSend.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSend.Location = new System.Drawing.Point(538, 242);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(74, 28);
            this.btnSend.TabIndex = 6;
            this.btnSend.Text = "Create";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // btnBatchPost
            // 
            this.btnBatchPost.AdjustImageLocation = new System.Drawing.Point(0, 0);
            this.btnBatchPost.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBatchPost.BtnShape = Waveface.Component.emunType.BtnShape.Rectangle;
            this.btnBatchPost.BtnStyle = Waveface.Component.emunType.XPStyle.Silver;
            this.btnBatchPost.Image = global::Waveface.Properties.Resources.arrow_divide;
            this.btnBatchPost.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBatchPost.Location = new System.Drawing.Point(458, 242);
            this.btnBatchPost.Name = "btnBatchPost";
            this.btnBatchPost.Size = new System.Drawing.Size(74, 28);
            this.btnBatchPost.TabIndex = 7;
            this.btnBatchPost.Text = "Post";
            this.btnBatchPost.UseVisualStyleBackColor = true;
            this.btnBatchPost.Visible = false;
            this.btnBatchPost.Click += new System.EventHandler(this.btnBatchPost_Click);
            // 
            // Photo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.panelToolbar);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.panel);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.btnBatchPost);
            this.Font = new System.Drawing.Font("Tahoma", 9F);
            this.Name = "Photo";
            this.Size = new System.Drawing.Size(618, 277);
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
    }
}
