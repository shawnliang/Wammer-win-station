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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PostArea));
			this.panelTimeBar = new System.Windows.Forms.Panel();
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.postList = new Waveface.PostsList();
			this.panelTimeBar.SuspendLayout();
			this.SuspendLayout();
			// 
			// panelTimeBar
			// 
			this.panelTimeBar.Controls.Add(this.comboBox1);
			resources.ApplyResources(this.panelTimeBar, "panelTimeBar");
			this.panelTimeBar.Name = "panelTimeBar";
			this.panelTimeBar.Paint += new System.Windows.Forms.PaintEventHandler(this.panelTimeBar_Paint);
			// 
			// comboBox1
			// 
			resources.ApplyResources(this.comboBox1, "comboBox1");
			this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox1.FormattingEnabled = true;
			this.comboBox1.Items.AddRange(new object[] {
            resources.GetString("comboBox1.Items"),
            resources.GetString("comboBox1.Items1"),
            resources.GetString("comboBox1.Items2"),
            resources.GetString("comboBox1.Items3"),
            resources.GetString("comboBox1.Items4")});
			this.comboBox1.Name = "comboBox1";
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
			this.Load += new System.EventHandler(this.PostArea_Load);
			this.panelTimeBar.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

		private PostsList postList;
        private System.Windows.Forms.Panel panelTimeBar;
		private System.Windows.Forms.ComboBox comboBox1;
    }
}
