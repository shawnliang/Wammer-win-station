namespace Waveface
{
    partial class VirtualFolderForm
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
            this.SuspendLayout();
            // 
            // VirtualFolderForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(116, 40);
            this.Font = new System.Drawing.Font("Tahoma", 9F);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "VirtualFolderForm";
            this.Opacity = 0.5D;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.Load += new System.EventHandler(this.VirtualFolderForm_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.VirtualFolderForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.VirtualFolderForm_DragEnter);
            this.DragOver += new System.Windows.Forms.DragEventHandler(this.VirtualFolderForm_DragOver);
            this.DragLeave += new System.EventHandler(this.VirtualFolderForm_DragLeave);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.frmBounds_Paint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.frmBounds_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.frmBounds_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.frmBounds_MouseUp);
            this.Resize += new System.EventHandler(this.frmBounds_Resize);
            this.ResumeLayout(false);

        }

        #endregion


    }
}

