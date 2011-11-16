using System.Windows.Forms;
using Manina.Windows.Forms;
using Waveface.Component;
using Waveface.PostUI;
using Waveface.Windows.Forms;

namespace Waveface
{
    partial class PostForm
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
            this.tabControl = new Waveface.Windows.Forms.FlatTabControl();
            this.tabPageWebLink = new System.Windows.Forms.TabPage();
            this.general_weblink_UI = new Waveface.PostUI.General_WebLink();
            this.tabPagePhoto = new System.Windows.Forms.TabPage();
            this.photo_UI = new Waveface.PostUI.Photo();
            this.tabPageRichText = new System.Windows.Forms.TabPage();
            this.richText_UI = new Waveface.PostUI.RichText();
            this.tabPageDocument = new System.Windows.Forms.TabPage();
            this.document_UI = new Waveface.PostUI.Document();
            this.tabControl.SuspendLayout();
            this.tabPageWebLink.SuspendLayout();
            this.tabPagePhoto.SuspendLayout();
            this.tabPageRichText.SuspendLayout();
            this.tabPageDocument.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.BorderDarkColor = System.Drawing.SystemColors.ControlDarkDark;
            this.tabControl.BorderLightColor = System.Drawing.SystemColors.ControlDark;
            this.tabControl.ControlBackColor = System.Drawing.SystemColors.Control;
            this.tabControl.Controls.Add(this.tabPageWebLink);
            this.tabControl.Controls.Add(this.tabPagePhoto);
            this.tabControl.Controls.Add(this.tabPageRichText);
            this.tabControl.Controls.Add(this.tabPageDocument);
            this.tabControl.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.tabControl.Location = new System.Drawing.Point(6, 6);
            this.tabControl.Multiline = true;
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.ShowTabsHeader = true;
            this.tabControl.Size = new System.Drawing.Size(762, 390);
            this.tabControl.TabAlignment = Waveface.Windows.Forms.FlatTabControl.FlatTabAlignment.Top;
            this.tabControl.TabIndex = 4;
            this.tabControl.TextColor = System.Drawing.SystemColors.ControlText;
            // 
            // tabPageWebLink
            // 
            this.tabPageWebLink.Controls.Add(this.general_weblink_UI);
            this.tabPageWebLink.Location = new System.Drawing.Point(4, 26);
            this.tabPageWebLink.Name = "tabPageWebLink";
            this.tabPageWebLink.Size = new System.Drawing.Size(754, 360);
            this.tabPageWebLink.TabIndex = 1;
            this.tabPageWebLink.Text = "Web Link";
            this.tabPageWebLink.UseVisualStyleBackColor = true;
            // 
            // general_weblink_UI
            // 
            this.general_weblink_UI.Dock = System.Windows.Forms.DockStyle.Fill;
            this.general_weblink_UI.Font = new System.Drawing.Font("Tahoma", 9F);
            this.general_weblink_UI.Location = new System.Drawing.Point(0, 0);
            this.general_weblink_UI.MinimumSize = new System.Drawing.Size(500, 130);
            this.general_weblink_UI.MyParent = null;
            this.general_weblink_UI.Name = "general_weblink_UI";
            this.general_weblink_UI.Size = new System.Drawing.Size(754, 360);
            this.general_weblink_UI.TabIndex = 0;
            // 
            // tabPagePhoto
            // 
            this.tabPagePhoto.Controls.Add(this.photo_UI);
            this.tabPagePhoto.Location = new System.Drawing.Point(4, 26);
            this.tabPagePhoto.Name = "tabPagePhoto";
            this.tabPagePhoto.Size = new System.Drawing.Size(754, 360);
            this.tabPagePhoto.TabIndex = 0;
            this.tabPagePhoto.Text = "Photo";
            this.tabPagePhoto.UseVisualStyleBackColor = true;
            // 
            // photo_UI
            // 
            this.photo_UI.Dock = System.Windows.Forms.DockStyle.Fill;
            this.photo_UI.Font = new System.Drawing.Font("Tahoma", 9F);
            this.photo_UI.Location = new System.Drawing.Point(0, 0);
            this.photo_UI.MinimumSize = new System.Drawing.Size(500, 130);
            this.photo_UI.MyParent = null;
            this.photo_UI.Name = "photo_UI";
            this.photo_UI.Size = new System.Drawing.Size(754, 360);
            this.photo_UI.TabIndex = 1;
            // 
            // tabPageRichText
            // 
            this.tabPageRichText.Controls.Add(this.richText_UI);
            this.tabPageRichText.Location = new System.Drawing.Point(4, 26);
            this.tabPageRichText.Name = "tabPageRichText";
            this.tabPageRichText.Size = new System.Drawing.Size(754, 360);
            this.tabPageRichText.TabIndex = 2;
            this.tabPageRichText.Text = "Rich Text";
            this.tabPageRichText.UseVisualStyleBackColor = true;
            // 
            // richText_UI
            // 
            this.richText_UI.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richText_UI.Font = new System.Drawing.Font("Tahoma", 9F);
            this.richText_UI.Location = new System.Drawing.Point(0, 0);
            this.richText_UI.MinimumSize = new System.Drawing.Size(500, 130);
            this.richText_UI.MyParent = null;
            this.richText_UI.Name = "richText_UI";
            this.richText_UI.Size = new System.Drawing.Size(754, 360);
            this.richText_UI.TabIndex = 1;
            // 
            // tabPageDocument
            // 
            this.tabPageDocument.Controls.Add(this.document_UI);
            this.tabPageDocument.Location = new System.Drawing.Point(4, 26);
            this.tabPageDocument.Name = "tabPageDocument";
            this.tabPageDocument.Size = new System.Drawing.Size(754, 360);
            this.tabPageDocument.TabIndex = 3;
            this.tabPageDocument.Text = "Document";
            this.tabPageDocument.UseVisualStyleBackColor = true;
            // 
            // document_UI
            // 
            this.document_UI.Dock = System.Windows.Forms.DockStyle.Fill;
            this.document_UI.Font = new System.Drawing.Font("Tahoma", 9F);
            this.document_UI.Location = new System.Drawing.Point(0, 0);
            this.document_UI.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.document_UI.MinimumSize = new System.Drawing.Size(500, 130);
            this.document_UI.MyParent = null;
            this.document_UI.Name = "document_UI";
            this.document_UI.Size = new System.Drawing.Size(754, 360);
            this.document_UI.TabIndex = 1;
            // 
            // PostForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(774, 402);
            this.Controls.Add(this.tabControl);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(744, 169);
            this.Name = "PostForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "New Post";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PostForm_FormClosing);
            this.tabControl.ResumeLayout(false);
            this.tabPageWebLink.ResumeLayout(false);
            this.tabPagePhoto.ResumeLayout(false);
            this.tabPageRichText.ResumeLayout(false);
            this.tabPageDocument.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private TabPage tabPageWebLink;
        private TabPage tabPagePhoto;
        private FlatTabControl tabControl;
        private TabPage tabPageRichText;
        private TabPage tabPageDocument;
        private General_WebLink general_weblink_UI;
        private Photo photo_UI;
        private RichText richText_UI;
        private Document document_UI;
    }
}

