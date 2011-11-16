namespace Waveface.PostUI
{
    partial class General_WebLink
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(General_WebLink));
            this.richTextBox = new System.Windows.Forms.RichTextBox();
            this.panel = new System.Windows.Forms.Panel();
            this.panelPreview = new System.Windows.Forms.Panel();
            this.richTextBoxDescription = new System.Windows.Forms.RichTextBox();
            this.labelTitle = new System.Windows.Forms.Label();
            this.pictureBoxPreview = new System.Windows.Forms.PictureBox();
            this.toolStripPreview = new System.Windows.Forms.ToolStrip();
            this.buttonRemovePreview = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.buttonPrev = new System.Windows.Forms.ToolStripButton();
            this.labelPictureIndex = new System.Windows.Forms.ToolStripLabel();
            this.buttonNext = new System.Windows.Forms.ToolStripButton();
            this.btnSend = new Waveface.Component.XPButton();
            this.contextMenuStripEdit = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel.SuspendLayout();
            this.panelPreview.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).BeginInit();
            this.toolStripPreview.SuspendLayout();
            this.contextMenuStripEdit.SuspendLayout();
            this.SuspendLayout();
            // 
            // richTextBox
            // 
            this.richTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox.ContextMenuStrip = this.contextMenuStripEdit;
            this.richTextBox.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.richTextBox.Location = new System.Drawing.Point(6, 6);
            this.richTextBox.Name = "richTextBox";
            this.richTextBox.Size = new System.Drawing.Size(609, 128);
            this.richTextBox.TabIndex = 3;
            this.richTextBox.Text = "";
            this.richTextBox.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.richTextBox_LinkClicked);
            // 
            // panel
            // 
            this.panel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel.Controls.Add(this.panelPreview);
            this.panel.Controls.Add(this.toolStripPreview);
            this.panel.Location = new System.Drawing.Point(6, 142);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(609, 215);
            this.panel.TabIndex = 5;
            // 
            // panelPreview
            // 
            this.panelPreview.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelPreview.Controls.Add(this.richTextBoxDescription);
            this.panelPreview.Controls.Add(this.labelTitle);
            this.panelPreview.Controls.Add(this.pictureBoxPreview);
            this.panelPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelPreview.Location = new System.Drawing.Point(0, 25);
            this.panelPreview.Name = "panelPreview";
            this.panelPreview.Size = new System.Drawing.Size(609, 190);
            this.panelPreview.TabIndex = 6;
            // 
            // richTextBoxDescription
            // 
            this.richTextBoxDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBoxDescription.BackColor = System.Drawing.SystemColors.Window;
            this.richTextBoxDescription.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBoxDescription.Location = new System.Drawing.Point(201, 42);
            this.richTextBoxDescription.Name = "richTextBoxDescription";
            this.richTextBoxDescription.ReadOnly = true;
            this.richTextBoxDescription.Size = new System.Drawing.Size(393, 101);
            this.richTextBoxDescription.TabIndex = 3;
            this.richTextBoxDescription.Text = "";
            // 
            // labelTitle
            // 
            this.labelTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTitle.AutoEllipsis = true;
            this.labelTitle.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.labelTitle.Location = new System.Drawing.Point(198, 12);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(398, 27);
            this.labelTitle.TabIndex = 2;
            // 
            // pictureBoxPreview
            // 
            this.pictureBoxPreview.Location = new System.Drawing.Point(12, 12);
            this.pictureBoxPreview.Name = "pictureBoxPreview";
            this.pictureBoxPreview.Size = new System.Drawing.Size(180, 120);
            this.pictureBoxPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxPreview.TabIndex = 0;
            this.pictureBoxPreview.TabStop = false;
            this.pictureBoxPreview.LoadCompleted += new System.ComponentModel.AsyncCompletedEventHandler(this.pictureBoxPreview_LoadCompleted);
            // 
            // toolStripPreview
            // 
            this.toolStripPreview.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonRemovePreview,
            this.toolStripSeparator4,
            this.toolStripSeparator2,
            this.buttonPrev,
            this.labelPictureIndex,
            this.buttonNext});
            this.toolStripPreview.Location = new System.Drawing.Point(0, 0);
            this.toolStripPreview.Name = "toolStripPreview";
            this.toolStripPreview.Size = new System.Drawing.Size(609, 25);
            this.toolStripPreview.TabIndex = 5;
            // 
            // buttonRemovePreview
            // 
            this.buttonRemovePreview.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonRemovePreview.Enabled = false;
            this.buttonRemovePreview.Image = ((System.Drawing.Image)(resources.GetObject("buttonRemovePreview.Image")));
            this.buttonRemovePreview.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonRemovePreview.Name = "buttonRemovePreview";
            this.buttonRemovePreview.Size = new System.Drawing.Size(23, 22);
            this.buttonRemovePreview.Text = "Remove Preview";
            this.buttonRemovePreview.Click += new System.EventHandler(this.buttonRemovePreview_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // buttonPrev
            // 
            this.buttonPrev.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonPrev.Enabled = false;
            this.buttonPrev.Image = global::Waveface.Properties.Resources.rewind;
            this.buttonPrev.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonPrev.Name = "buttonPrev";
            this.buttonPrev.Size = new System.Drawing.Size(23, 22);
            this.buttonPrev.ToolTipText = "Prev Picture";
            this.buttonPrev.Click += new System.EventHandler(this.buttonPrev_Click);
            // 
            // labelPictureIndex
            // 
            this.labelPictureIndex.Name = "labelPictureIndex";
            this.labelPictureIndex.Size = new System.Drawing.Size(13, 22);
            this.labelPictureIndex.Text = "-";
            // 
            // buttonNext
            // 
            this.buttonNext.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonNext.Enabled = false;
            this.buttonNext.Image = global::Waveface.Properties.Resources.fastforward;
            this.buttonNext.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonNext.Name = "buttonNext";
            this.buttonNext.Size = new System.Drawing.Size(23, 22);
            this.buttonNext.ToolTipText = "Next Picture";
            this.buttonNext.Click += new System.EventHandler(this.buttonNext_Click);
            // 
            // btnSend
            // 
            this.btnSend.AdjustImageLocation = new System.Drawing.Point(0, 0);
            this.btnSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSend.BtnShape = Waveface.Component.emunType.BtnShape.Rectangle;
            this.btnSend.BtnStyle = Waveface.Component.emunType.XPStyle.Silver;
            this.btnSend.Image = global::Waveface.Properties.Resources.Post;
            this.btnSend.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSend.Location = new System.Drawing.Point(621, 6);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(74, 32);
            this.btnSend.TabIndex = 4;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
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
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.cutToolStripMenuItem.Text = "Cut";
            this.cutToolStripMenuItem.Click += new System.EventHandler(this.cutToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.pasteToolStripMenuItem.Text = "Paste";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // General_WebLink
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel);
            this.Controls.Add(this.richTextBox);
            this.Controls.Add(this.btnSend);
            this.Font = new System.Drawing.Font("Tahoma", 9F);
            this.MinimumSize = new System.Drawing.Size(500, 130);
            this.Name = "General_WebLink";
            this.Size = new System.Drawing.Size(700, 360);
            this.panel.ResumeLayout(false);
            this.panel.PerformLayout();
            this.panelPreview.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).EndInit();
            this.toolStripPreview.ResumeLayout(false);
            this.toolStripPreview.PerformLayout();
            this.contextMenuStripEdit.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBox;
        private Component.XPButton btnSend;
        private System.Windows.Forms.Panel panel;
        private System.Windows.Forms.ToolStrip toolStripPreview;
        private System.Windows.Forms.ToolStripButton buttonRemovePreview;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton buttonPrev;
        private System.Windows.Forms.ToolStripLabel labelPictureIndex;
        private System.Windows.Forms.ToolStripButton buttonNext;
        private System.Windows.Forms.Panel panelPreview;
        private System.Windows.Forms.RichTextBox richTextBoxDescription;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.PictureBox pictureBoxPreview;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripEdit;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
    }
}
