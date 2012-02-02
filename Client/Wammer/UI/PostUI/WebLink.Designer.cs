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
            this.panel = new System.Windows.Forms.Panel();
            this.panelPreview = new System.Windows.Forms.Panel();
            this.cbNoThumbnail = new System.Windows.Forms.CheckBox();
            this.richTextBoxDescription = new System.Windows.Forms.RichTextBox();
            this.labelTitle = new System.Windows.Forms.Label();
            this.pictureBoxPreview = new System.Windows.Forms.PictureBox();
            this.labelPictureIndex = new System.Windows.Forms.Label();
            this.panelToolbar = new System.Windows.Forms.Panel();
            this.buttonNext = new Waveface.Component.XPButton();
            this.buttonPrev = new Waveface.Component.XPButton();
            this.buttonRemovePreview = new Waveface.Component.XPButton();
            this.btnSend = new Waveface.Component.XPButton();
            this.panel.SuspendLayout();
            this.panelPreview.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).BeginInit();
            this.panelToolbar.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel
            // 
            this.panel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel.Controls.Add(this.panelToolbar);
            this.panel.Controls.Add(this.panelPreview);
            this.panel.Location = new System.Drawing.Point(6, 0);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(608, 219);
            this.panel.TabIndex = 5;
            // 
            // panelPreview
            // 
            this.panelPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelPreview.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelPreview.Controls.Add(this.cbNoThumbnail);
            this.panelPreview.Controls.Add(this.richTextBoxDescription);
            this.panelPreview.Controls.Add(this.labelTitle);
            this.panelPreview.Controls.Add(this.pictureBoxPreview);
            this.panelPreview.Location = new System.Drawing.Point(2, 40);
            this.panelPreview.Name = "panelPreview";
            this.panelPreview.Size = new System.Drawing.Size(605, 176);
            this.panelPreview.TabIndex = 6;
            // 
            // cbNoThumbnail
            // 
            this.cbNoThumbnail.AutoSize = true;
            this.cbNoThumbnail.Location = new System.Drawing.Point(46, 138);
            this.cbNoThumbnail.Name = "cbNoThumbnail";
            this.cbNoThumbnail.Size = new System.Drawing.Size(101, 18);
            this.cbNoThumbnail.TabIndex = 4;
            this.cbNoThumbnail.Text = "No Thumbnail";
            this.cbNoThumbnail.UseVisualStyleBackColor = true;
            this.cbNoThumbnail.CheckedChanged += new System.EventHandler(this.cbNoThumbnail_CheckedChanged);
            // 
            // richTextBoxDescription
            // 
            this.richTextBoxDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBoxDescription.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(242)))), ((int)(((byte)(238)))));
            this.richTextBoxDescription.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBoxDescription.Location = new System.Drawing.Point(201, 42);
            this.richTextBoxDescription.Name = "richTextBoxDescription";
            this.richTextBoxDescription.ReadOnly = true;
            this.richTextBoxDescription.Size = new System.Drawing.Size(389, 101);
            this.richTextBoxDescription.TabIndex = 3;
            this.richTextBoxDescription.Text = "";
            // 
            // labelTitle
            // 
            this.labelTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTitle.AutoEllipsis = true;
            this.labelTitle.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.labelTitle.Location = new System.Drawing.Point(198, 12);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(394, 27);
            this.labelTitle.TabIndex = 2;
            // 
            // pictureBoxPreview
            // 
            this.pictureBoxPreview.Location = new System.Drawing.Point(12, 12);
            this.pictureBoxPreview.Name = "pictureBoxPreview";
            this.pictureBoxPreview.Size = new System.Drawing.Size(180, 120);
            this.pictureBoxPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxPreview.TabIndex = 0;
            this.pictureBoxPreview.TabStop = false;
            this.pictureBoxPreview.LoadCompleted += new System.ComponentModel.AsyncCompletedEventHandler(this.pictureBoxPreview_LoadCompleted);
            // 
            // labelPictureIndex
            // 
            this.labelPictureIndex.AutoSize = true;
            this.labelPictureIndex.Location = new System.Drawing.Point(64, 8);
            this.labelPictureIndex.Name = "labelPictureIndex";
            this.labelPictureIndex.Size = new System.Drawing.Size(17, 14);
            this.labelPictureIndex.TabIndex = 6;
            this.labelPictureIndex.Text = "[]";
            // 
            // panelToolbar
            // 
            this.panelToolbar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelToolbar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(202)))), ((int)(((byte)(197)))), ((int)(((byte)(186)))));
            this.panelToolbar.Controls.Add(this.buttonNext);
            this.panelToolbar.Controls.Add(this.buttonPrev);
            this.panelToolbar.Controls.Add(this.buttonRemovePreview);
            this.panelToolbar.Controls.Add(this.labelPictureIndex);
            this.panelToolbar.Location = new System.Drawing.Point(2, 0);
            this.panelToolbar.Name = "panelToolbar";
            this.panelToolbar.Size = new System.Drawing.Size(604, 31);
            this.panelToolbar.TabIndex = 16;
            // 
            // buttonNext
            // 
            this.buttonNext.AdjustImageLocation = new System.Drawing.Point(0, 0);
            this.buttonNext.BtnShape = Waveface.Component.emunType.BtnShape.Rectangle;
            this.buttonNext.BtnStyle = Waveface.Component.emunType.XPStyle.Silver;
            this.buttonNext.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonNext.Location = new System.Drawing.Point(32, 2);
            this.buttonNext.Name = "buttonNext";
            this.buttonNext.Size = new System.Drawing.Size(26, 26);
            this.buttonNext.TabIndex = 15;
            this.buttonNext.Text = ">";
            this.buttonNext.UseVisualStyleBackColor = true;
            this.buttonNext.Click += new System.EventHandler(this.buttonNext_Click);
            // 
            // buttonPrev
            // 
            this.buttonPrev.AdjustImageLocation = new System.Drawing.Point(0, 0);
            this.buttonPrev.BtnShape = Waveface.Component.emunType.BtnShape.Rectangle;
            this.buttonPrev.BtnStyle = Waveface.Component.emunType.XPStyle.Silver;
            this.buttonPrev.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonPrev.Location = new System.Drawing.Point(4, 2);
            this.buttonPrev.Name = "buttonPrev";
            this.buttonPrev.Size = new System.Drawing.Size(26, 26);
            this.buttonPrev.TabIndex = 14;
            this.buttonPrev.Text = "<";
            this.buttonPrev.UseVisualStyleBackColor = true;
            this.buttonPrev.Click += new System.EventHandler(this.buttonPrev_Click);
            // 
            // buttonRemovePreview
            // 
            this.buttonRemovePreview.AdjustImageLocation = new System.Drawing.Point(0, 0);
            this.buttonRemovePreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonRemovePreview.BtnShape = Waveface.Component.emunType.BtnShape.Rectangle;
            this.buttonRemovePreview.BtnStyle = Waveface.Component.emunType.XPStyle.Silver;
            this.buttonRemovePreview.Image = global::Waveface.Properties.Resources.trash;
            this.buttonRemovePreview.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonRemovePreview.Location = new System.Drawing.Point(575, 2);
            this.buttonRemovePreview.Name = "buttonRemovePreview";
            this.buttonRemovePreview.Size = new System.Drawing.Size(26, 26);
            this.buttonRemovePreview.TabIndex = 13;
            this.buttonRemovePreview.UseVisualStyleBackColor = true;
            this.buttonRemovePreview.Click += new System.EventHandler(this.buttonRemovePreview_Click);
            // 
            // btnSend
            // 
            this.btnSend.AdjustImageLocation = new System.Drawing.Point(0, 0);
            this.btnSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSend.BtnShape = Waveface.Component.emunType.BtnShape.Rectangle;
            this.btnSend.BtnStyle = Waveface.Component.emunType.XPStyle.Silver;
            this.btnSend.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSend.Location = new System.Drawing.Point(540, 233);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(74, 28);
            this.btnSend.TabIndex = 4;
            this.btnSend.Text = "Create";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // WebLink
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(242)))), ((int)(((byte)(238)))));
            this.Controls.Add(this.panel);
            this.Controls.Add(this.btnSend);
            this.Font = new System.Drawing.Font("Tahoma", 9F);
            this.MinimumSize = new System.Drawing.Size(500, 130);
            this.Name = "WebLink";
            this.Size = new System.Drawing.Size(620, 269);
            this.Resize += new System.EventHandler(this.General_WebLink_Resize);
            this.panel.ResumeLayout(false);
            this.panelPreview.ResumeLayout(false);
            this.panelPreview.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).EndInit();
            this.panelToolbar.ResumeLayout(false);
            this.panelToolbar.PerformLayout();
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
    }
}
