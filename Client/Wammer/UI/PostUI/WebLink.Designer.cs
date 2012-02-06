namespace Waveface.PostUI
{
    partial class WebLink
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WebLink));
            this.panel = new System.Windows.Forms.Panel();
            this.panelToolbar = new System.Windows.Forms.Panel();
            this.buttonNext = new Waveface.Component.XPButton();
            this.buttonPrev = new Waveface.Component.XPButton();
            this.buttonRemovePreview = new Waveface.Component.XPButton();
            this.labelPictureIndex = new System.Windows.Forms.Label();
            this.label = new System.Windows.Forms.Label();
            this.panelPreview = new System.Windows.Forms.Panel();
            this.cbNoThumbnail = new System.Windows.Forms.CheckBox();
            this.richTextBoxDescription = new System.Windows.Forms.RichTextBox();
            this.labelTitle = new System.Windows.Forms.Label();
            this.pictureBoxPreview = new System.Windows.Forms.PictureBox();
            this.btnSend = new Waveface.Component.XPButton();
            this.cultureManager = new Waveface.Localization.CultureManager(this.components);
            this.panel.SuspendLayout();
            this.panelToolbar.SuspendLayout();
            this.panelPreview.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).BeginInit();
            this.SuspendLayout();
            // 
            // panel
            // 
            resources.ApplyResources(this.panel, "panel");
            this.panel.Controls.Add(this.panelToolbar);
            this.panel.Controls.Add(this.panelPreview);
            this.panel.Name = "panel";
            // 
            // panelToolbar
            // 
            resources.ApplyResources(this.panelToolbar, "panelToolbar");
            this.panelToolbar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(202)))), ((int)(((byte)(197)))), ((int)(((byte)(186)))));
            this.panelToolbar.Controls.Add(this.buttonNext);
            this.panelToolbar.Controls.Add(this.buttonPrev);
            this.panelToolbar.Controls.Add(this.buttonRemovePreview);
            this.panelToolbar.Controls.Add(this.labelPictureIndex);
            this.panelToolbar.Controls.Add(this.label);
            this.panelToolbar.Name = "panelToolbar";
            // 
            // buttonNext
            // 
            this.buttonNext.AdjustImageLocation = new System.Drawing.Point(0, 0);
            this.buttonNext.BtnShape = Waveface.Component.emunType.BtnShape.Rectangle;
            this.buttonNext.BtnStyle = Waveface.Component.emunType.XPStyle.Silver;
            resources.ApplyResources(this.buttonNext, "buttonNext");
            this.buttonNext.Name = "buttonNext";
            this.buttonNext.UseVisualStyleBackColor = true;
            this.buttonNext.Click += new System.EventHandler(this.buttonNext_Click);
            // 
            // buttonPrev
            // 
            this.buttonPrev.AdjustImageLocation = new System.Drawing.Point(0, 0);
            this.buttonPrev.BtnShape = Waveface.Component.emunType.BtnShape.Rectangle;
            this.buttonPrev.BtnStyle = Waveface.Component.emunType.XPStyle.Silver;
            resources.ApplyResources(this.buttonPrev, "buttonPrev");
            this.buttonPrev.Name = "buttonPrev";
            this.buttonPrev.UseVisualStyleBackColor = true;
            this.buttonPrev.Click += new System.EventHandler(this.buttonPrev_Click);
            // 
            // buttonRemovePreview
            // 
            this.buttonRemovePreview.AdjustImageLocation = new System.Drawing.Point(0, 0);
            resources.ApplyResources(this.buttonRemovePreview, "buttonRemovePreview");
            this.buttonRemovePreview.BtnShape = Waveface.Component.emunType.BtnShape.Rectangle;
            this.buttonRemovePreview.BtnStyle = Waveface.Component.emunType.XPStyle.Silver;
            this.buttonRemovePreview.Image = global::Waveface.Properties.Resources.trash;
            this.buttonRemovePreview.Name = "buttonRemovePreview";
            this.buttonRemovePreview.UseVisualStyleBackColor = true;
            this.buttonRemovePreview.Click += new System.EventHandler(this.buttonRemovePreview_Click);
            // 
            // labelPictureIndex
            // 
            resources.ApplyResources(this.labelPictureIndex, "labelPictureIndex");
            this.labelPictureIndex.Name = "labelPictureIndex";
            // 
            // label
            // 
            resources.ApplyResources(this.label, "label");
            this.label.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label.Name = "label";
            // 
            // panelPreview
            // 
            resources.ApplyResources(this.panelPreview, "panelPreview");
            this.panelPreview.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelPreview.Controls.Add(this.cbNoThumbnail);
            this.panelPreview.Controls.Add(this.richTextBoxDescription);
            this.panelPreview.Controls.Add(this.labelTitle);
            this.panelPreview.Controls.Add(this.pictureBoxPreview);
            this.panelPreview.Name = "panelPreview";
            // 
            // cbNoThumbnail
            // 
            resources.ApplyResources(this.cbNoThumbnail, "cbNoThumbnail");
            this.cbNoThumbnail.Name = "cbNoThumbnail";
            this.cbNoThumbnail.UseVisualStyleBackColor = true;
            this.cbNoThumbnail.CheckedChanged += new System.EventHandler(this.cbNoThumbnail_CheckedChanged);
            // 
            // richTextBoxDescription
            // 
            resources.ApplyResources(this.richTextBoxDescription, "richTextBoxDescription");
            this.richTextBoxDescription.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(242)))), ((int)(((byte)(238)))));
            this.richTextBoxDescription.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBoxDescription.Name = "richTextBoxDescription";
            this.richTextBoxDescription.ReadOnly = true;
            // 
            // labelTitle
            // 
            resources.ApplyResources(this.labelTitle, "labelTitle");
            this.labelTitle.AutoEllipsis = true;
            this.labelTitle.Name = "labelTitle";
            // 
            // pictureBoxPreview
            // 
            resources.ApplyResources(this.pictureBoxPreview, "pictureBoxPreview");
            this.pictureBoxPreview.Name = "pictureBoxPreview";
            this.pictureBoxPreview.TabStop = false;
            this.pictureBoxPreview.LoadCompleted += new System.ComponentModel.AsyncCompletedEventHandler(this.pictureBoxPreview_LoadCompleted);
            // 
            // btnSend
            // 
            this.btnSend.AdjustImageLocation = new System.Drawing.Point(0, 0);
            resources.ApplyResources(this.btnSend, "btnSend");
            this.btnSend.BtnShape = Waveface.Component.emunType.BtnShape.Rectangle;
            this.btnSend.BtnStyle = Waveface.Component.emunType.XPStyle.Silver;
            this.btnSend.Name = "btnSend";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // cultureManager
            // 
            this.cultureManager.ManagedControl = this;
            // 
            // WebLink
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(242)))), ((int)(((byte)(238)))));
            this.Controls.Add(this.panel);
            this.Controls.Add(this.btnSend);
            this.MinimumSize = new System.Drawing.Size(500, 130);
            this.Name = "WebLink";
            this.Resize += new System.EventHandler(this.General_WebLink_Resize);
            this.panel.ResumeLayout(false);
            this.panelToolbar.ResumeLayout(false);
            this.panelToolbar.PerformLayout();
            this.panelPreview.ResumeLayout(false);
            this.panelPreview.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Component.XPButton btnSend;
        private System.Windows.Forms.Panel panel;
        private System.Windows.Forms.Panel panelPreview;
        private System.Windows.Forms.RichTextBox richTextBoxDescription;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.PictureBox pictureBoxPreview;
        private System.Windows.Forms.CheckBox cbNoThumbnail;
        private System.Windows.Forms.Label labelPictureIndex;
        private System.Windows.Forms.Panel panelToolbar;
        private Component.XPButton buttonRemovePreview;
        private Component.XPButton buttonNext;
        private Component.XPButton buttonPrev;
        private Localization.CultureManager cultureManager;
        private System.Windows.Forms.Label label;
    }
}
