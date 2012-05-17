namespace Waveface
{
    partial class CommentForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CommentForm));
            this.contextMenuStripEdit = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textBoxComment = new Waveface.Component.RichEdit.RichTextEditor();
            this.buttonAddComment = new Waveface.Component.ImageButton();
            this.contextMenuStripEdit.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStripEdit
            // 
            resources.ApplyResources(this.contextMenuStripEdit, "contextMenuStripEdit");
            this.contextMenuStripEdit.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem});
            this.contextMenuStripEdit.Name = "contextMenuStrip1";
            this.contextMenuStripEdit.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripEdit_Opening);
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            resources.ApplyResources(this.cutToolStripMenuItem, "cutToolStripMenuItem");
            this.cutToolStripMenuItem.Click += new System.EventHandler(this.cutToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            resources.ApplyResources(this.copyToolStripMenuItem, "copyToolStripMenuItem");
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            resources.ApplyResources(this.pasteToolStripMenuItem, "pasteToolStripMenuItem");
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // textBoxComment
            // 
            resources.ApplyResources(this.textBoxComment, "textBoxComment");
            this.textBoxComment.ContextMenuStrip = this.contextMenuStripEdit;
            this.textBoxComment.Name = "textBoxComment";
            this.textBoxComment.UndoLength = 100;
            this.textBoxComment.WaterMarkColor = System.Drawing.Color.Silver;
            this.textBoxComment.WaterMarkText = "Water Mark";
            this.textBoxComment.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxComment_KeyDown);
            // 
            // buttonAddComment
            // 
            resources.ApplyResources(this.buttonAddComment, "buttonAddComment");
            this.buttonAddComment.CenterAlignImage = false;
            this.buttonAddComment.ForeColor = System.Drawing.Color.White;
            this.buttonAddComment.Image = global::Waveface.Properties.Resources.FB_creat_btn;
            this.buttonAddComment.ImageDisable = global::Waveface.Properties.Resources.FB_creat_btn_hl;
            this.buttonAddComment.ImageFront = null;
            this.buttonAddComment.ImageHover = global::Waveface.Properties.Resources.FB_creat_btn_hl;
            this.buttonAddComment.Name = "buttonAddComment";
            this.buttonAddComment.TextShadow = true;
            this.buttonAddComment.Click += new System.EventHandler(this.buttonAddComment_Click);
            // 
            // CommentForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.textBoxComment);
            this.Controls.Add(this.buttonAddComment);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CommentForm";
            this.ShowInTaskbar = false;
            this.contextMenuStripEdit.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStripEdit;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private Waveface.Component.RichEdit.RichTextEditor textBoxComment;
        public Component.ImageButton buttonAddComment;
    }
}