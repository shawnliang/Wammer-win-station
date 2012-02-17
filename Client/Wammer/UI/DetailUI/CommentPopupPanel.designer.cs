using Waveface.Component;
using Waveface.Component.RichEdit;

namespace Waveface
{
    partial class CommentPopupPanel
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CommentPopupPanel));
            this.buttonAddComment = new Waveface.Component.XPButton();
            this.textBoxComment = new WaterMarkRichTextBox();
            this.SuspendLayout();
            // 
            // buttonAddComment
            // 
            this.buttonAddComment.AdjustImageLocation = new System.Drawing.Point(0, 0);
            resources.ApplyResources(this.buttonAddComment, "buttonAddComment");
            this.buttonAddComment.BtnShape = Waveface.Component.emunType.BtnShape.Rectangle;
            this.buttonAddComment.BtnStyle = Waveface.Component.emunType.XPStyle.Silver;
            this.buttonAddComment.Name = "buttonAddComment";
            this.buttonAddComment.UseVisualStyleBackColor = true;
            // 
            // textBoxComment
            // 
            resources.ApplyResources(this.textBoxComment, "textBoxComment");
            this.textBoxComment.Name = "textBoxComment";
            this.textBoxComment.WaterMarkColor = System.Drawing.Color.Silver;
            this.textBoxComment.WaterMarkText = "Water Mark";
            // 
            // CommentPopupPanel
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(242)))), ((int)(((byte)(238)))));
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.buttonAddComment);
            this.Controls.Add(this.textBoxComment);
            resources.ApplyResources(this, "$this");
            this.Name = "CommentPopupPanel";
            this.ResumeLayout(false);

        }

        #endregion

        public Component.XPButton buttonAddComment;
        private WaterMarkRichTextBox textBoxComment;

    }
}
