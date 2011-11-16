namespace Waveface.PostUI
{
    partial class RichTextPostForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RichTextPostForm));
            this.imageListView = new Manina.Windows.Forms.ImageListView();
            this.label1 = new System.Windows.Forms.Label();
            this.richTextBox = new System.Windows.Forms.RichTextBox();
            this.contextMenuStripEdit = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label2 = new System.Windows.Forms.Label();
            this.checkBoxBackUpPics = new System.Windows.Forms.CheckBox();
            this.labelImageCount = new System.Windows.Forms.Label();
            this.btnClearText = new Waveface.Component.XPButton();
            this.btnAutoGenTitle = new Waveface.Component.XPButton();
            this.btnCancal = new Waveface.Component.XPButton();
            this.btnSend = new Waveface.Component.XPButton();
            this.contextMenuStripEdit.SuspendLayout();
            this.SuspendLayout();
            // 
            // imageListView
            // 
            this.imageListView.ColumnHeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.imageListView.DefaultImage = ((System.Drawing.Image)(resources.GetObject("imageListView.DefaultImage")));
            this.imageListView.ErrorImage = ((System.Drawing.Image)(resources.GetObject("imageListView.ErrorImage")));
            this.imageListView.GroupHeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.imageListView.Location = new System.Drawing.Point(11, 194);
            this.imageListView.Name = "imageListView";
            this.imageListView.Size = new System.Drawing.Size(669, 136);
            this.imageListView.TabIndex = 11;
            this.imageListView.ThumbnailSize = new System.Drawing.Size(120, 120);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 14);
            this.label1.TabIndex = 12;
            this.label1.Text = "Title:";
            // 
            // richTextBox
            // 
            this.richTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox.ContextMenuStrip = this.contextMenuStripEdit;
            this.richTextBox.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.richTextBox.Location = new System.Drawing.Point(12, 26);
            this.richTextBox.Name = "richTextBox";
            this.richTextBox.Size = new System.Drawing.Size(668, 136);
            this.richTextBox.TabIndex = 13;
            this.richTextBox.Text = "";
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
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 177);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 14);
            this.label2.TabIndex = 14;
            this.label2.Text = "Icon:";
            // 
            // checkBoxBackUpPics
            // 
            this.checkBoxBackUpPics.AutoSize = true;
            this.checkBoxBackUpPics.Location = new System.Drawing.Point(11, 347);
            this.checkBoxBackUpPics.Name = "checkBoxBackUpPics";
            this.checkBoxBackUpPics.Size = new System.Drawing.Size(155, 18);
            this.checkBoxBackUpPics.TabIndex = 15;
            this.checkBoxBackUpPics.Text = "Backup Remote Images";
            this.checkBoxBackUpPics.UseVisualStyleBackColor = true;
            this.checkBoxBackUpPics.Visible = false;
            // 
            // labelImageCount
            // 
            this.labelImageCount.Location = new System.Drawing.Point(623, 177);
            this.labelImageCount.Name = "labelImageCount";
            this.labelImageCount.Size = new System.Drawing.Size(57, 14);
            this.labelImageCount.TabIndex = 19;
            this.labelImageCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnClearText
            // 
            this.btnClearText.AdjustImageLocation = new System.Drawing.Point(0, 0);
            this.btnClearText.BtnShape = Waveface.Component.emunType.BtnShape.Rectangle;
            this.btnClearText.BtnStyle = Waveface.Component.emunType.XPStyle.Silver;
            this.btnClearText.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnClearText.Location = new System.Drawing.Point(681, 59);
            this.btnClearText.Name = "btnClearText";
            this.btnClearText.Size = new System.Drawing.Size(28, 28);
            this.btnClearText.TabIndex = 18;
            this.btnClearText.UseVisualStyleBackColor = true;
            this.btnClearText.Click += new System.EventHandler(this.btnClearText_Click);
            // 
            // btnAutoGenTitle
            // 
            this.btnAutoGenTitle.AdjustImageLocation = new System.Drawing.Point(0, 0);
            this.btnAutoGenTitle.BtnShape = Waveface.Component.emunType.BtnShape.Rectangle;
            this.btnAutoGenTitle.BtnStyle = Waveface.Component.emunType.XPStyle.Silver;
            this.btnAutoGenTitle.Image = global::Waveface.Properties.Resources.lightbulb;
            this.btnAutoGenTitle.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAutoGenTitle.Location = new System.Drawing.Point(681, 25);
            this.btnAutoGenTitle.Name = "btnAutoGenTitle";
            this.btnAutoGenTitle.Size = new System.Drawing.Size(28, 28);
            this.btnAutoGenTitle.TabIndex = 16;
            this.btnAutoGenTitle.UseVisualStyleBackColor = true;
            this.btnAutoGenTitle.Click += new System.EventHandler(this.btnAutoGenTitle_Click);
            // 
            // btnCancal
            // 
            this.btnCancal.AdjustImageLocation = new System.Drawing.Point(0, 0);
            this.btnCancal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancal.BtnShape = Waveface.Component.emunType.BtnShape.Rectangle;
            this.btnCancal.BtnStyle = Waveface.Component.emunType.XPStyle.Silver;
            this.btnCancal.Image = global::Waveface.Properties.Resources.postItem_delete;
            this.btnCancal.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancal.Location = new System.Drawing.Point(277, 368);
            this.btnCancal.Name = "btnCancal";
            this.btnCancal.Size = new System.Drawing.Size(74, 32);
            this.btnCancal.TabIndex = 10;
            this.btnCancal.Text = "Cancel";
            this.btnCancal.UseVisualStyleBackColor = true;
            // 
            // btnSend
            // 
            this.btnSend.AdjustImageLocation = new System.Drawing.Point(0, 0);
            this.btnSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSend.BtnShape = Waveface.Component.emunType.BtnShape.Rectangle;
            this.btnSend.BtnStyle = Waveface.Component.emunType.XPStyle.Silver;
            this.btnSend.Image = global::Waveface.Properties.Resources.Post;
            this.btnSend.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSend.Location = new System.Drawing.Point(357, 368);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(74, 32);
            this.btnSend.TabIndex = 9;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // RichTextPostForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(715, 412);
            this.Controls.Add(this.labelImageCount);
            this.Controls.Add(this.richTextBox);
            this.Controls.Add(this.btnClearText);
            this.Controls.Add(this.btnAutoGenTitle);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.checkBoxBackUpPics);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.imageListView);
            this.Controls.Add(this.btnCancal);
            this.Controls.Add(this.btnSend);
            this.Font = new System.Drawing.Font("Tahoma", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RichTextPostForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Rich Text Post";
            this.Load += new System.EventHandler(this.RichTextPostForm_Load);
            this.contextMenuStripEdit.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Component.XPButton btnSend;
        private Component.XPButton btnCancal;
        private Manina.Windows.Forms.ImageListView imageListView;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox richTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkBoxBackUpPics;
        private Component.XPButton btnAutoGenTitle;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripEdit;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private Component.XPButton btnClearText;
        private System.Windows.Forms.Label labelImageCount;
    }
}