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
            this.panelPreview = new System.Windows.Forms.Panel();
            this.panelContent = new System.Windows.Forms.Panel();
            this.labelProvider = new System.Windows.Forms.Label();
            this.labelTitle = new System.Windows.Forms.Label();
            this.richTextBoxDescription = new System.Windows.Forms.RichTextBox();
            this.panelSelectPicture = new System.Windows.Forms.Panel();
            this.pictureBoxPreview = new System.Windows.Forms.PictureBox();
            this.buttonNext = new Waveface.Component.XPButton();
            this.labelPictureIndex = new System.Windows.Forms.Label();
            this.cbNoThumbnail = new System.Windows.Forms.CheckBox();
            this.buttonPrev = new Waveface.Component.XPButton();
            this.panelToolbar = new System.Windows.Forms.Panel();
            this.labelSummary = new System.Windows.Forms.Label();
            this.buttonRemovePreview = new Waveface.Component.ImageButton();
            this.cultureManager = new Waveface.Localization.CultureManager(this.components);
            this.btnSend = new Waveface.Component.ImageButton();
            this.panel.SuspendLayout();
            this.panelPreview.SuspendLayout();
            this.panelContent.SuspendLayout();
            this.panelSelectPicture.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).BeginInit();
            this.panelToolbar.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel
            // 
            resources.ApplyResources(this.panel, "panel");
            this.panel.Controls.Add(this.panelPreview);
            this.panel.Name = "panel";
            // 
            // panelPreview
            // 
            resources.ApplyResources(this.panelPreview, "panelPreview");
            this.panelPreview.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.panelPreview.Controls.Add(this.panelContent);
            this.panelPreview.Controls.Add(this.panelSelectPicture);
            this.panelPreview.Name = "panelPreview";
            // 
            // panelContent
            // 
            resources.ApplyResources(this.panelContent, "panelContent");
            this.panelContent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.panelContent.Controls.Add(this.labelProvider);
            this.panelContent.Controls.Add(this.labelTitle);
            this.panelContent.Controls.Add(this.richTextBoxDescription);
            this.panelContent.Name = "panelContent";
            // 
            // labelProvider
            // 
            resources.ApplyResources(this.labelProvider, "labelProvider");
            this.labelProvider.AutoEllipsis = true;
            this.labelProvider.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.labelProvider.ForeColor = System.Drawing.Color.Gray;
            this.labelProvider.Name = "labelProvider";
            // 
            // labelTitle
            // 
            resources.ApplyResources(this.labelTitle, "labelTitle");
            this.labelTitle.AutoEllipsis = true;
            this.labelTitle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.labelTitle.Name = "labelTitle";
            // 
            // richTextBoxDescription
            // 
            resources.ApplyResources(this.richTextBoxDescription, "richTextBoxDescription");
            this.richTextBoxDescription.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.richTextBoxDescription.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBoxDescription.Name = "richTextBoxDescription";
            this.richTextBoxDescription.ReadOnly = true;
            // 
            // panelSelectPicture
            // 
            this.panelSelectPicture.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.panelSelectPicture.Controls.Add(this.pictureBoxPreview);
            this.panelSelectPicture.Controls.Add(this.buttonNext);
            this.panelSelectPicture.Controls.Add(this.labelPictureIndex);
            this.panelSelectPicture.Controls.Add(this.cbNoThumbnail);
            this.panelSelectPicture.Controls.Add(this.buttonPrev);
            resources.ApplyResources(this.panelSelectPicture, "panelSelectPicture");
            this.panelSelectPicture.Name = "panelSelectPicture";
            // 
            // pictureBoxPreview
            // 
            resources.ApplyResources(this.pictureBoxPreview, "pictureBoxPreview");
            this.pictureBoxPreview.Name = "pictureBoxPreview";
            this.pictureBoxPreview.TabStop = false;
            this.pictureBoxPreview.LoadCompleted += new System.ComponentModel.AsyncCompletedEventHandler(this.pictureBoxPreview_LoadCompleted);
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
            // labelPictureIndex
            // 
            resources.ApplyResources(this.labelPictureIndex, "labelPictureIndex");
            this.labelPictureIndex.Name = "labelPictureIndex";
            // 
            // cbNoThumbnail
            // 
            resources.ApplyResources(this.cbNoThumbnail, "cbNoThumbnail");
            this.cbNoThumbnail.Name = "cbNoThumbnail";
            this.cbNoThumbnail.UseVisualStyleBackColor = true;
            this.cbNoThumbnail.CheckedChanged += new System.EventHandler(this.cbNoThumbnail_CheckedChanged);
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
            // panelToolbar
            // 
            resources.ApplyResources(this.panelToolbar, "panelToolbar");
            this.panelToolbar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.panelToolbar.Controls.Add(this.labelSummary);
            this.panelToolbar.Controls.Add(this.buttonRemovePreview);
            this.panelToolbar.Name = "panelToolbar";
            // 
            // labelSummary
            // 
            resources.ApplyResources(this.labelSummary, "labelSummary");
            this.labelSummary.AutoEllipsis = true;
            this.labelSummary.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(97)))), ((int)(((byte)(101)))));
            this.labelSummary.Name = "labelSummary";
            // 
            // buttonRemovePreview
            // 
            resources.ApplyResources(this.buttonRemovePreview, "buttonRemovePreview");
            this.buttonRemovePreview.CenterAlignImage = false;
            this.buttonRemovePreview.ForeColor = System.Drawing.Color.White;
            this.buttonRemovePreview.Image = global::Waveface.Properties.Resources.FB_blue_btn;
            this.buttonRemovePreview.ImageDisable = global::Waveface.Properties.Resources.FB_blue_btn_hl;
            this.buttonRemovePreview.ImageFront = global::Waveface.Properties.Resources.FB_edit_delete;
            this.buttonRemovePreview.ImageHover = global::Waveface.Properties.Resources.FB_blue_btn_hl;
            this.buttonRemovePreview.Name = "buttonRemovePreview";
            this.buttonRemovePreview.Click += new System.EventHandler(this.buttonRemovePreview_Click);
            // 
            // cultureManager
            // 
            this.cultureManager.ManagedControl = this;
            // 
            // btnSend
            // 
            resources.ApplyResources(this.btnSend, "btnSend");
            this.btnSend.CenterAlignImage = false;
            this.btnSend.ForeColor = System.Drawing.Color.White;
            this.btnSend.Image = global::Waveface.Properties.Resources.FB_creat_btn;
            this.btnSend.ImageDisable = global::Waveface.Properties.Resources.FB_creat_btn_hl;
            this.btnSend.ImageFront = null;
            this.btnSend.ImageHover = global::Waveface.Properties.Resources.FB_creat_btn_hl;
            this.btnSend.Name = "btnSend";
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // WebLink
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.Controls.Add(this.panelToolbar);
            this.Controls.Add(this.panel);
            this.Controls.Add(this.btnSend);
            this.MinimumSize = new System.Drawing.Size(500, 130);
            this.Name = "WebLink";
            this.Resize += new System.EventHandler(this.General_WebLink_Resize);
            this.panel.ResumeLayout(false);
            this.panelPreview.ResumeLayout(false);
            this.panelContent.ResumeLayout(false);
            this.panelSelectPicture.ResumeLayout(false);
            this.panelSelectPicture.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).EndInit();
            this.panelToolbar.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Component.ImageButton btnSend;
        private System.Windows.Forms.Panel panel;
        private System.Windows.Forms.Panel panelPreview;
        private System.Windows.Forms.RichTextBox richTextBoxDescription;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.PictureBox pictureBoxPreview;
        private System.Windows.Forms.CheckBox cbNoThumbnail;
        private System.Windows.Forms.Label labelPictureIndex;
        private System.Windows.Forms.Panel panelToolbar;
        private Component.ImageButton buttonRemovePreview;
        private Component.XPButton buttonNext;
        private Component.XPButton buttonPrev;
        private Localization.CultureManager cultureManager;
        private System.Windows.Forms.Panel panelContent;
        private System.Windows.Forms.Label labelProvider;
        private System.Windows.Forms.Panel panelSelectPicture;
        private System.Windows.Forms.Label labelSummary;
    }
}
