using Manina.Windows.Forms;
using Waveface.Component;

namespace Waveface
{
    partial class PhotoView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PhotoView));
            this.imageListView = new Manina.Windows.Forms.ImageListView();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miSave = new System.Windows.Forms.ToolStripMenuItem();
            this.miSaveAll = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.btnSave = new Waveface.Component.XPButton();
            this.btnSaveAll = new Waveface.Component.XPButton();
            this.cultureManager = new Waveface.Localization.CultureManager(this.components);
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // imageListView
            // 
            resources.ApplyResources(this.imageListView, "imageListView");
            this.imageListView.ColumnHeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.imageListView.ContextMenuStrip = this.contextMenuStrip;
            this.imageListView.GroupHeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.imageListView.Name = "imageListView";
            this.toolTip.SetToolTip(this.imageListView, resources.GetString("imageListView.ToolTip"));
            this.imageListView.SelectionChanged += new System.EventHandler(this.imageListView_SelectionChanged);
            // 
            // contextMenuStrip
            // 
            resources.ApplyResources(this.contextMenuStrip, "contextMenuStrip");
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miSave,
            this.miSaveAll});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.toolTip.SetToolTip(this.contextMenuStrip, resources.GetString("contextMenuStrip.ToolTip"));
            this.contextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Opening);
            // 
            // miSave
            // 
            resources.ApplyResources(this.miSave, "miSave");
            this.miSave.Name = "miSave";
            this.miSave.Click += new System.EventHandler(this.miSave_Click);
            // 
            // miSaveAll
            // 
            resources.ApplyResources(this.miSaveAll, "miSaveAll");
            this.miSaveAll.Name = "miSaveAll";
            this.miSaveAll.Click += new System.EventHandler(this.miSaveAll_Click);
            // 
            // saveFileDialog
            // 
            resources.ApplyResources(this.saveFileDialog, "saveFileDialog");
            // 
            // btnSave
            // 
            resources.ApplyResources(this.btnSave, "btnSave");
            this.btnSave.AdjustImageLocation = new System.Drawing.Point(0, 0);
            this.btnSave.BtnShape = Waveface.Component.emunType.BtnShape.Rectangle;
            this.btnSave.BtnStyle = Waveface.Component.emunType.XPStyle.Silver;
            this.btnSave.Image = global::Waveface.Properties.Resources.Save;
            this.btnSave.Name = "btnSave";
            this.toolTip.SetToolTip(this.btnSave, resources.GetString("btnSave.ToolTip"));
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnSaveAll
            // 
            resources.ApplyResources(this.btnSaveAll, "btnSaveAll");
            this.btnSaveAll.AdjustImageLocation = new System.Drawing.Point(0, 0);
            this.btnSaveAll.BtnShape = Waveface.Component.emunType.BtnShape.Rectangle;
            this.btnSaveAll.BtnStyle = Waveface.Component.emunType.XPStyle.Silver;
            this.btnSaveAll.Image = global::Waveface.Properties.Resources.SaveAll;
            this.btnSaveAll.Name = "btnSaveAll";
            this.toolTip.SetToolTip(this.btnSaveAll, resources.GetString("btnSaveAll.ToolTip"));
            this.btnSaveAll.UseVisualStyleBackColor = true;
            this.btnSaveAll.Click += new System.EventHandler(this.btnSaveAll_Click);
            // 
            // cultureManager
            // 
            this.cultureManager.ManagedControl = this;
            // 
            // PhotoView
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnSaveAll);
            this.Controls.Add(this.imageListView);
            this.MinimizeBox = false;
            this.Name = "PhotoView";
            this.toolTip.SetToolTip(this, resources.GetString("$this.ToolTip"));
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ImageListView imageListView;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem miSave;
        private System.Windows.Forms.ToolStripMenuItem miSaveAll;
        private XPButton btnSave;
        private XPButton btnSaveAll;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private Localization.CultureManager cultureManager;
        private System.Windows.Forms.ToolTip toolTip;
    }
}

