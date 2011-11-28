namespace Waveface.PostUI
{
    partial class Document
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Document));
            this.removeAllFilesButton = new System.Windows.Forms.ToolStripButton();
            this.removeFileButton = new System.Windows.Forms.ToolStripButton();
            this.addFileButton = new System.Windows.Forms.ToolStripButton();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.panel = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panelPreview = new System.Windows.Forms.Panel();
            this.splitter = new System.Windows.Forms.Splitter();
            this.panelLeft = new System.Windows.Forms.Panel();
            this.listViewFiles = new System.Windows.Forms.ListView();
            this.panelFileInfo = new System.Windows.Forms.Panel();
            this.labelFileTime = new System.Windows.Forms.Label();
            this.labeFilelSize = new System.Windows.Forms.Label();
            this.labelFTime = new System.Windows.Forms.Label();
            this.labelFSize = new System.Windows.Forms.Label();
            this.contextMenuStripEdit = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.btnBatchPost = new Waveface.Component.XPButton();
            this.btnSend = new Waveface.Component.XPButton();
            this.previewHandlerHost = new Waveface.Component.PreviewHandlerHost();
            this.toolStrip.SuspendLayout();
            this.panel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panelPreview.SuspendLayout();
            this.panelLeft.SuspendLayout();
            this.panelFileInfo.SuspendLayout();
            this.contextMenuStripEdit.SuspendLayout();
            this.SuspendLayout();
            // 
            // removeAllFilesButton
            // 
            this.removeAllFilesButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.removeAllFilesButton.Image = ((System.Drawing.Image)(resources.GetObject("removeAllFilesButton.Image")));
            this.removeAllFilesButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.removeAllFilesButton.Name = "removeAllFilesButton";
            this.removeAllFilesButton.Size = new System.Drawing.Size(23, 22);
            this.removeAllFilesButton.Text = "Remove All Files";
            this.removeAllFilesButton.Click += new System.EventHandler(this.removeAllFilesButton_Click);
            // 
            // removeFileButton
            // 
            this.removeFileButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.removeFileButton.Image = ((System.Drawing.Image)(resources.GetObject("removeFileButton.Image")));
            this.removeFileButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.removeFileButton.Name = "removeFileButton";
            this.removeFileButton.Size = new System.Drawing.Size(23, 22);
            this.removeFileButton.Text = "Remove Selected Files";
            this.removeFileButton.Click += new System.EventHandler(this.removeFileButton_Click);
            // 
            // addFileButton
            // 
            this.addFileButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.addFileButton.Image = ((System.Drawing.Image)(resources.GetObject("addFileButton.Image")));
            this.addFileButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.addFileButton.Name = "addFileButton";
            this.addFileButton.Size = new System.Drawing.Size(23, 22);
            this.addFileButton.Text = "Add Files...";
            this.addFileButton.Click += new System.EventHandler(this.addFileButton_Click);
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removeAllFilesButton,
            this.addFileButton,
            this.removeFileButton,
            this.toolStripSeparator1});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(607, 25);
            this.toolStrip.TabIndex = 1;
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // panel
            // 
            this.panel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel.Controls.Add(this.panel1);
            this.panel.Controls.Add(this.toolStrip);
            this.panel.Location = new System.Drawing.Point(6, 6);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(607, 215);
            this.panel.TabIndex = 12;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.panelPreview);
            this.panel1.Controls.Add(this.splitter);
            this.panel1.Controls.Add(this.panelLeft);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 25);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(607, 190);
            this.panel1.TabIndex = 2;
            // 
            // panelPreview
            // 
            this.panelPreview.Controls.Add(this.previewHandlerHost);
            this.panelPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelPreview.Location = new System.Drawing.Point(259, 0);
            this.panelPreview.Name = "panelPreview";
            this.panelPreview.Size = new System.Drawing.Size(344, 186);
            this.panelPreview.TabIndex = 2;
            // 
            // splitter
            // 
            this.splitter.Location = new System.Drawing.Point(255, 0);
            this.splitter.Name = "splitter";
            this.splitter.Size = new System.Drawing.Size(4, 186);
            this.splitter.TabIndex = 1;
            this.splitter.TabStop = false;
            // 
            // panelLeft
            // 
            this.panelLeft.Controls.Add(this.listViewFiles);
            this.panelLeft.Controls.Add(this.panelFileInfo);
            this.panelLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelLeft.Location = new System.Drawing.Point(0, 0);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Size = new System.Drawing.Size(255, 186);
            this.panelLeft.TabIndex = 0;
            // 
            // listViewFiles
            // 
            this.listViewFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewFiles.FullRowSelect = true;
            this.listViewFiles.GridLines = true;
            this.listViewFiles.Location = new System.Drawing.Point(0, 0);
            this.listViewFiles.Name = "listViewFiles";
            this.listViewFiles.Size = new System.Drawing.Size(255, 122);
            this.listViewFiles.TabIndex = 2;
            this.listViewFiles.UseCompatibleStateImageBehavior = false;
            this.listViewFiles.View = System.Windows.Forms.View.Tile;
            this.listViewFiles.Click += new System.EventHandler(this.listViewFiles_Click);
            this.listViewFiles.Resize += new System.EventHandler(this.listViewFiles_Resize);
            // 
            // panelFileInfo
            // 
            this.panelFileInfo.BackColor = System.Drawing.SystemColors.Info;
            this.panelFileInfo.Controls.Add(this.labelFileTime);
            this.panelFileInfo.Controls.Add(this.labeFilelSize);
            this.panelFileInfo.Controls.Add(this.labelFTime);
            this.panelFileInfo.Controls.Add(this.labelFSize);
            this.panelFileInfo.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelFileInfo.Location = new System.Drawing.Point(0, 122);
            this.panelFileInfo.Name = "panelFileInfo";
            this.panelFileInfo.Size = new System.Drawing.Size(255, 64);
            this.panelFileInfo.TabIndex = 0;
            // 
            // labelFileTime
            // 
            this.labelFileTime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelFileTime.AutoEllipsis = true;
            this.labelFileTime.Location = new System.Drawing.Point(94, 37);
            this.labelFileTime.Name = "labelFileTime";
            this.labelFileTime.Size = new System.Drawing.Size(159, 18);
            this.labelFileTime.TabIndex = 3;
            // 
            // labeFilelSize
            // 
            this.labeFilelSize.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labeFilelSize.AutoEllipsis = true;
            this.labeFilelSize.Location = new System.Drawing.Point(94, 11);
            this.labeFilelSize.Name = "labeFilelSize";
            this.labeFilelSize.Size = new System.Drawing.Size(159, 18);
            this.labeFilelSize.TabIndex = 2;
            // 
            // labelFTime
            // 
            this.labelFTime.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelFTime.Location = new System.Drawing.Point(4, 34);
            this.labelFTime.Name = "labelFTime";
            this.labelFTime.Size = new System.Drawing.Size(94, 21);
            this.labelFTime.TabIndex = 1;
            this.labelFTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // labelFSize
            // 
            this.labelFSize.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelFSize.Location = new System.Drawing.Point(4, 8);
            this.labelFSize.Name = "labelFSize";
            this.labelFSize.Size = new System.Drawing.Size(94, 21);
            this.labelFSize.TabIndex = 0;
            this.labelFSize.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // contextMenuStripEdit
            // 
            this.contextMenuStripEdit.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.contextMenuStripEdit.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem});
            this.contextMenuStripEdit.Name = "contextMenuStrip1";
            this.contextMenuStripEdit.Size = new System.Drawing.Size(105, 70);
            this.contextMenuStripEdit.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripEdit_Opening);
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(104, 22);
            this.cutToolStripMenuItem.Text = "Cut";
            this.cutToolStripMenuItem.Click += new System.EventHandler(this.cutToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(104, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(104, 22);
            this.pasteToolStripMenuItem.Text = "Paste";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = resources.GetString("openFileDialog.Filter");
            this.openFileDialog.Multiselect = true;
            this.openFileDialog.ShowReadOnly = true;
            // 
            // btnBatchPost
            // 
            this.btnBatchPost.AdjustImageLocation = new System.Drawing.Point(0, 0);
            this.btnBatchPost.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBatchPost.BtnShape = Waveface.Component.emunType.BtnShape.Rectangle;
            this.btnBatchPost.BtnStyle = Waveface.Component.emunType.XPStyle.Silver;
            this.btnBatchPost.Image = global::Waveface.Properties.Resources.arrow_divide;
            this.btnBatchPost.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBatchPost.Location = new System.Drawing.Point(457, 230);
            this.btnBatchPost.Name = "btnBatchPost";
            this.btnBatchPost.Size = new System.Drawing.Size(74, 28);
            this.btnBatchPost.TabIndex = 11;
            this.btnBatchPost.Text = "Post";
            this.btnBatchPost.UseVisualStyleBackColor = true;
            this.btnBatchPost.Visible = false;
            // 
            // btnSend
            // 
            this.btnSend.AdjustImageLocation = new System.Drawing.Point(0, 0);
            this.btnSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSend.BtnShape = Waveface.Component.emunType.BtnShape.Rectangle;
            this.btnSend.BtnStyle = Waveface.Component.emunType.XPStyle.Silver;
            this.btnSend.Image = global::Waveface.Properties.Resources.Post;
            this.btnSend.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSend.Location = new System.Drawing.Point(537, 230);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(74, 28);
            this.btnSend.TabIndex = 10;
            this.btnSend.Text = "Create";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // previewHandlerHost
            // 
            this.previewHandlerHost.BackColor = System.Drawing.Color.White;
            this.previewHandlerHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.previewHandlerHost.Location = new System.Drawing.Point(0, 0);
            this.previewHandlerHost.Name = "previewHandlerHost";
            this.previewHandlerHost.Size = new System.Drawing.Size(344, 186);
            this.previewHandlerHost.TabIndex = 0;
            this.previewHandlerHost.Text = "previewHandlerHost";
            // 
            // Document
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel);
            this.Controls.Add(this.btnBatchPost);
            this.Controls.Add(this.btnSend);
            this.Font = new System.Drawing.Font("Tahoma", 9F);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Document";
            this.Size = new System.Drawing.Size(619, 264);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.panel.ResumeLayout(false);
            this.panel.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panelPreview.ResumeLayout(false);
            this.panelLeft.ResumeLayout(false);
            this.panelFileInfo.ResumeLayout(false);
            this.contextMenuStripEdit.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStripButton removeAllFilesButton;
        private Component.XPButton btnSend;
        private System.Windows.Forms.ToolStripButton removeFileButton;
        private Component.XPButton btnBatchPost;
        private System.Windows.Forms.ToolStripButton addFileButton;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.Panel panel;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panelLeft;
        private System.Windows.Forms.Panel panelPreview;
        private System.Windows.Forms.Splitter splitter;
        private System.Windows.Forms.ListView listViewFiles;
        private System.Windows.Forms.Panel panelFileInfo;
        private Component.PreviewHandlerHost previewHandlerHost;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Label labelFileTime;
        private System.Windows.Forms.Label labeFilelSize;
        private System.Windows.Forms.Label labelFTime;
        private System.Windows.Forms.Label labelFSize;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripEdit;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
    }
}
