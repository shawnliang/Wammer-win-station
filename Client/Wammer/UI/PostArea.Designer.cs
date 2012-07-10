using Waveface.Component;

namespace Waveface
{
    partial class PostArea
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PostArea));
            this.panelTimeBar = new System.Windows.Forms.Panel();
            this.panelMain = new System.Windows.Forms.Panel();
            this.cultureManager = new Waveface.Localization.CultureManager(this.components);
            this.postList = new Waveface.PostsList();
            this.panelMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTimeBar
            // 
            resources.ApplyResources(this.panelTimeBar, "panelTimeBar");
            this.panelTimeBar.Name = "panelTimeBar";
            this.panelTimeBar.Paint += new System.Windows.Forms.PaintEventHandler(this.panelTimeBar_Paint);
            // 
            // panelMain
            // 
            this.panelMain.Controls.Add(this.postList);
            this.panelMain.Controls.Add(this.panelTimeBar);
            resources.ApplyResources(this.panelMain, "panelMain");
            this.panelMain.Name = "panelMain";
            // 
            // cultureManager
            // 
            this.cultureManager.ManagedControl = this;
            // 
            // postList
            // 
            this.postList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(225)))), ((int)(((byte)(225)))));
            this.postList.DetailView = null;
            resources.ApplyResources(this.postList, "postList");
            this.postList.MyParent = null;
            this.postList.Name = "postList";
            // 
            // PostArea
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.panelMain);
            resources.ApplyResources(this, "$this");
            this.MinimumSize = new System.Drawing.Size(336, 2);
            this.Name = "PostArea";
            this.panelMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private PostsList postList;
        private System.Windows.Forms.Panel panelMain;
        private Localization.CultureManager cultureManager;
        private System.Windows.Forms.Panel panelTimeBar;
    }
}
