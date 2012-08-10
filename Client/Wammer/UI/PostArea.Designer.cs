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

				if (_font != null)
				{
					_font.Dispose();
					_font = null;
				}
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
			this.postList = new Waveface.PostsList();
			this.SuspendLayout();
			// 
			// panelTimeBar
			// 
			resources.ApplyResources(this.panelTimeBar, "panelTimeBar");
			this.panelTimeBar.Name = "panelTimeBar";
			this.panelTimeBar.Paint += new System.Windows.Forms.PaintEventHandler(this.panelTimeBar_Paint);
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
			this.Controls.Add(this.postList);
			this.Controls.Add(this.panelTimeBar);
			resources.ApplyResources(this, "$this");
			this.MinimumSize = new System.Drawing.Size(336, 2);
			this.Name = "PostArea";
			this.ResumeLayout(false);

        }

        #endregion

		private PostsList postList;
        private System.Windows.Forms.Panel panelTimeBar;
    }
}
