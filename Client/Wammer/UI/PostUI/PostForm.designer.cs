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
            this.multiPanel = new Waveface.Component.MultiPage.MultiPanel();
            this.Page_RichText = new Waveface.Component.MultiPage.MultiPanelPage();
            this.panelRichTextPanel = new System.Windows.Forms.Panel();
            this.panelRichText_Main = new System.Windows.Forms.Panel();
            this.richText_UI = new Waveface.PostUI.RichText();
            this.panelRichText_Top = new System.Windows.Forms.Panel();
            this.btnPureText = new Waveface.Component.XPButton();
            this.Page_P_D_W = new Waveface.Component.MultiPage.MultiPanelPage();
            this.panelPWD_Main = new System.Windows.Forms.Panel();
            this.multiPanel_P_D_W = new Waveface.Component.MultiPage.MultiPanel();
            this.Page__Link = new Waveface.Component.MultiPage.MultiPanelPage();
            this.general_weblink_UI = new Waveface.PostUI.General_WebLink();
            this.Page__Photo = new Waveface.Component.MultiPage.MultiPanelPage();
            this.photo_UI = new Waveface.PostUI.Photo();
            this.Page__DOC = new Waveface.Component.MultiPage.MultiPanelPage();
            this.document_UI = new Waveface.PostUI.Document();
            this.panelMiddleBar = new System.Windows.Forms.Panel();
            this.btnAddDoc = new Waveface.Component.XPButton();
            this.btnAddPhoto = new Waveface.Component.XPButton();
            this.btnSend = new Waveface.Component.XPButton();
            this.panelText = new System.Windows.Forms.Panel();
            this.buttonRichText = new Waveface.Component.XPButton();
            this.richTextBox = new System.Windows.Forms.RichTextBox();
            this.multiPanel.SuspendLayout();
            this.Page_RichText.SuspendLayout();
            this.panelRichTextPanel.SuspendLayout();
            this.panelRichText_Main.SuspendLayout();
            this.panelRichText_Top.SuspendLayout();
            this.Page_P_D_W.SuspendLayout();
            this.panelPWD_Main.SuspendLayout();
            this.multiPanel_P_D_W.SuspendLayout();
            this.Page__Link.SuspendLayout();
            this.Page__Photo.SuspendLayout();
            this.Page__DOC.SuspendLayout();
            this.panelMiddleBar.SuspendLayout();
            this.panelText.SuspendLayout();
            this.SuspendLayout();
            // 
            // multiPanel
            // 
            this.multiPanel.Controls.Add(this.Page_RichText);
            this.multiPanel.Controls.Add(this.Page_P_D_W);
            this.multiPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.multiPanel.Location = new System.Drawing.Point(0, 0);
            this.multiPanel.Name = "multiPanel";
            this.multiPanel.SelectedPage = this.Page_P_D_W;
            this.multiPanel.Size = new System.Drawing.Size(624, 208);
            this.multiPanel.TabIndex = 7;
            // 
            // Page_RichText
            // 
            this.Page_RichText.Controls.Add(this.panelRichTextPanel);
            this.Page_RichText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Page_RichText.Location = new System.Drawing.Point(0, 0);
            this.Page_RichText.Name = "Page_RichText";
            this.Page_RichText.Size = new System.Drawing.Size(624, 208);
            this.Page_RichText.TabIndex = 0;
            this.Page_RichText.Text = "Page_RichText";
            // 
            // panelRichTextPanel
            // 
            this.panelRichTextPanel.Controls.Add(this.panelRichText_Main);
            this.panelRichTextPanel.Controls.Add(this.panelRichText_Top);
            this.panelRichTextPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelRichTextPanel.Location = new System.Drawing.Point(0, 0);
            this.panelRichTextPanel.Name = "panelRichTextPanel";
            this.panelRichTextPanel.Size = new System.Drawing.Size(624, 208);
            this.panelRichTextPanel.TabIndex = 6;
            // 
            // panelRichText_Main
            // 
            this.panelRichText_Main.Controls.Add(this.richText_UI);
            this.panelRichText_Main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelRichText_Main.Location = new System.Drawing.Point(0, 30);
            this.panelRichText_Main.Name = "panelRichText_Main";
            this.panelRichText_Main.Size = new System.Drawing.Size(624, 178);
            this.panelRichText_Main.TabIndex = 7;
            // 
            // richText_UI
            // 
            this.richText_UI.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richText_UI.Font = new System.Drawing.Font("Tahoma", 9F);
            this.richText_UI.Location = new System.Drawing.Point(0, 0);
            this.richText_UI.MinimumSize = new System.Drawing.Size(500, 130);
            this.richText_UI.MyParent = null;
            this.richText_UI.Name = "richText_UI";
            this.richText_UI.Size = new System.Drawing.Size(624, 178);
            this.richText_UI.TabIndex = 5;
            // 
            // panelRichText_Top
            // 
            this.panelRichText_Top.Controls.Add(this.btnPureText);
            this.panelRichText_Top.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelRichText_Top.Location = new System.Drawing.Point(0, 0);
            this.panelRichText_Top.Name = "panelRichText_Top";
            this.panelRichText_Top.Size = new System.Drawing.Size(624, 30);
            this.panelRichText_Top.TabIndex = 6;
            // 
            // btnPureText
            // 
            this.btnPureText.AdjustImageLocation = new System.Drawing.Point(0, 0);
            this.btnPureText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPureText.BtnShape = Waveface.Component.emunType.BtnShape.Rectangle;
            this.btnPureText.BtnStyle = Waveface.Component.emunType.XPStyle.Silver;
            this.btnPureText.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnPureText.Image = global::Waveface.Properties.Resources.white_edit;
            this.btnPureText.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPureText.Location = new System.Drawing.Point(591, 4);
            this.btnPureText.Name = "btnPureText";
            this.btnPureText.Size = new System.Drawing.Size(26, 26);
            this.btnPureText.TabIndex = 9;
            this.btnPureText.UseVisualStyleBackColor = true;
            this.btnPureText.Click += new System.EventHandler(this.btnPureText_Click);
            // 
            // Page_P_D_W
            // 
            this.Page_P_D_W.Controls.Add(this.panelPWD_Main);
            this.Page_P_D_W.Controls.Add(this.panelMiddleBar);
            this.Page_P_D_W.Controls.Add(this.panelText);
            this.Page_P_D_W.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Page_P_D_W.Location = new System.Drawing.Point(0, 0);
            this.Page_P_D_W.Name = "Page_P_D_W";
            this.Page_P_D_W.Size = new System.Drawing.Size(624, 208);
            this.Page_P_D_W.TabIndex = 1;
            this.Page_P_D_W.Text = "Page_P_D_W";
            // 
            // panelPWD_Main
            // 
            this.panelPWD_Main.Controls.Add(this.multiPanel_P_D_W);
            this.panelPWD_Main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelPWD_Main.Location = new System.Drawing.Point(0, 208);
            this.panelPWD_Main.Name = "panelPWD_Main";
            this.panelPWD_Main.Size = new System.Drawing.Size(624, 0);
            this.panelPWD_Main.TabIndex = 14;
            // 
            // multiPanel_P_D_W
            // 
            this.multiPanel_P_D_W.Controls.Add(this.Page__Link);
            this.multiPanel_P_D_W.Controls.Add(this.Page__Photo);
            this.multiPanel_P_D_W.Controls.Add(this.Page__DOC);
            this.multiPanel_P_D_W.Dock = System.Windows.Forms.DockStyle.Fill;
            this.multiPanel_P_D_W.Location = new System.Drawing.Point(0, 0);
            this.multiPanel_P_D_W.Name = "multiPanel_P_D_W";
            this.multiPanel_P_D_W.SelectedPage = this.Page__Link;
            this.multiPanel_P_D_W.Size = new System.Drawing.Size(624, 0);
            this.multiPanel_P_D_W.TabIndex = 7;
            // 
            // Page__Link
            // 
            this.Page__Link.Controls.Add(this.general_weblink_UI);
            this.Page__Link.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Page__Link.Location = new System.Drawing.Point(0, 0);
            this.Page__Link.Name = "Page__Link";
            this.Page__Link.Size = new System.Drawing.Size(624, 0);
            this.Page__Link.TabIndex = 0;
            this.Page__Link.Text = "Page_Link";
            // 
            // general_weblink_UI
            // 
            this.general_weblink_UI.Dock = System.Windows.Forms.DockStyle.Fill;
            this.general_weblink_UI.Font = new System.Drawing.Font("Tahoma", 9F);
            this.general_weblink_UI.Location = new System.Drawing.Point(0, 0);
            this.general_weblink_UI.MinimumSize = new System.Drawing.Size(500, 130);
            this.general_weblink_UI.MyParent = null;
            this.general_weblink_UI.Name = "general_weblink_UI";
            this.general_weblink_UI.Size = new System.Drawing.Size(624, 130);
            this.general_weblink_UI.TabIndex = 0;
            // 
            // Page__Photo
            // 
            this.Page__Photo.Controls.Add(this.photo_UI);
            this.Page__Photo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Page__Photo.Location = new System.Drawing.Point(0, 0);
            this.Page__Photo.Name = "Page__Photo";
            this.Page__Photo.Size = new System.Drawing.Size(617, 245);
            this.Page__Photo.TabIndex = 1;
            this.Page__Photo.Text = "Page_Photo";
            // 
            // photo_UI
            // 
            this.photo_UI.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.photo_UI.Font = new System.Drawing.Font("Tahoma", 9F);
            this.photo_UI.Location = new System.Drawing.Point(0, 0);
            this.photo_UI.MinimumSize = new System.Drawing.Size(500, 130);
            this.photo_UI.MyParent = null;
            this.photo_UI.Name = "photo_UI";
            this.photo_UI.Size = new System.Drawing.Size(617, 245);
            this.photo_UI.TabIndex = 1;
            // 
            // Page__DOC
            // 
            this.Page__DOC.Controls.Add(this.document_UI);
            this.Page__DOC.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Page__DOC.Location = new System.Drawing.Point(0, 0);
            this.Page__DOC.Name = "Page__DOC";
            this.Page__DOC.Size = new System.Drawing.Size(617, 245);
            this.Page__DOC.TabIndex = 2;
            this.Page__DOC.Text = "Page_2";
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
            this.document_UI.Size = new System.Drawing.Size(617, 245);
            this.document_UI.TabIndex = 1;
            // 
            // panelMiddleBar
            // 
            this.panelMiddleBar.Controls.Add(this.btnAddDoc);
            this.panelMiddleBar.Controls.Add(this.btnAddPhoto);
            this.panelMiddleBar.Controls.Add(this.btnSend);
            this.panelMiddleBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelMiddleBar.Location = new System.Drawing.Point(0, 166);
            this.panelMiddleBar.Name = "panelMiddleBar";
            this.panelMiddleBar.Size = new System.Drawing.Size(624, 42);
            this.panelMiddleBar.TabIndex = 13;
            // 
            // btnAddDoc
            // 
            this.btnAddDoc.AdjustImageLocation = new System.Drawing.Point(0, 0);
            this.btnAddDoc.BtnShape = Waveface.Component.emunType.BtnShape.Rectangle;
            this.btnAddDoc.BtnStyle = Waveface.Component.emunType.XPStyle.Silver;
            this.btnAddDoc.Image = global::Waveface.Properties.Resources.page;
            this.btnAddDoc.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAddDoc.Location = new System.Drawing.Point(113, 6);
            this.btnAddDoc.Name = "btnAddDoc";
            this.btnAddDoc.Size = new System.Drawing.Size(115, 26);
            this.btnAddDoc.TabIndex = 13;
            this.btnAddDoc.Text = "Add Document";
            this.btnAddDoc.UseVisualStyleBackColor = true;
            this.btnAddDoc.Click += new System.EventHandler(this.btnAddDoc_Click);
            // 
            // btnAddPhoto
            // 
            this.btnAddPhoto.AdjustImageLocation = new System.Drawing.Point(0, 0);
            this.btnAddPhoto.BtnShape = Waveface.Component.emunType.BtnShape.Rectangle;
            this.btnAddPhoto.BtnStyle = Waveface.Component.emunType.XPStyle.Silver;
            this.btnAddPhoto.Image = global::Waveface.Properties.Resources.AllTime;
            this.btnAddPhoto.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAddPhoto.Location = new System.Drawing.Point(8, 6);
            this.btnAddPhoto.Name = "btnAddPhoto";
            this.btnAddPhoto.Size = new System.Drawing.Size(99, 26);
            this.btnAddPhoto.TabIndex = 12;
            this.btnAddPhoto.Text = "Add Photo";
            this.btnAddPhoto.UseVisualStyleBackColor = true;
            this.btnAddPhoto.Click += new System.EventHandler(this.btnAddPhoto_Click);
            // 
            // btnSend
            // 
            this.btnSend.AdjustImageLocation = new System.Drawing.Point(0, 0);
            this.btnSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSend.BtnShape = Waveface.Component.emunType.BtnShape.Rectangle;
            this.btnSend.BtnStyle = Waveface.Component.emunType.XPStyle.Silver;
            this.btnSend.Image = global::Waveface.Properties.Resources.Post;
            this.btnSend.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSend.Location = new System.Drawing.Point(543, 6);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(74, 26);
            this.btnSend.TabIndex = 11;
            this.btnSend.Text = "Create";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // panelText
            // 
            this.panelText.Controls.Add(this.buttonRichText);
            this.panelText.Controls.Add(this.richTextBox);
            this.panelText.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelText.Location = new System.Drawing.Point(0, 0);
            this.panelText.Name = "panelText";
            this.panelText.Size = new System.Drawing.Size(624, 166);
            this.panelText.TabIndex = 12;
            // 
            // buttonRichText
            // 
            this.buttonRichText.AdjustImageLocation = new System.Drawing.Point(0, 0);
            this.buttonRichText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonRichText.BtnShape = Waveface.Component.emunType.BtnShape.Rectangle;
            this.buttonRichText.BtnStyle = Waveface.Component.emunType.XPStyle.Silver;
            this.buttonRichText.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.buttonRichText.Image = global::Waveface.Properties.Resources.content;
            this.buttonRichText.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonRichText.Location = new System.Drawing.Point(590, 6);
            this.buttonRichText.Name = "buttonRichText";
            this.buttonRichText.Size = new System.Drawing.Size(26, 26);
            this.buttonRichText.TabIndex = 8;
            this.buttonRichText.UseVisualStyleBackColor = true;
            this.buttonRichText.Click += new System.EventHandler(this.buttonRichText_Click);
            // 
            // richTextBox
            // 
            this.richTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.richTextBox.Location = new System.Drawing.Point(8, 36);
            this.richTextBox.Name = "richTextBox";
            this.richTextBox.Size = new System.Drawing.Size(609, 124);
            this.richTextBox.TabIndex = 6;
            this.richTextBox.Text = "";
            this.richTextBox.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.richTextBox_LinkClicked);
            this.richTextBox.TextChanged += new System.EventHandler(this.richTextBox_TextChanged);
            // 
            // PostForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 208);
            this.Controls.Add(this.multiPanel);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(540, 236);
            this.Name = "PostForm";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "New Post";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PostForm_FormClosing);
            this.Resize += new System.EventHandler(this.PostForm_Resize);
            this.multiPanel.ResumeLayout(false);
            this.Page_RichText.ResumeLayout(false);
            this.panelRichTextPanel.ResumeLayout(false);
            this.panelRichText_Main.ResumeLayout(false);
            this.panelRichText_Top.ResumeLayout(false);
            this.Page_P_D_W.ResumeLayout(false);
            this.panelPWD_Main.ResumeLayout(false);
            this.multiPanel_P_D_W.ResumeLayout(false);
            this.Page__Link.ResumeLayout(false);
            this.Page__Photo.ResumeLayout(false);
            this.Page__DOC.ResumeLayout(false);
            this.panelMiddleBar.ResumeLayout(false);
            this.panelText.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private General_WebLink general_weblink_UI;
        private Photo photo_UI;
        private Document document_UI;
        private RichText richText_UI;
        public RichTextBox richTextBox;
        private Component.MultiPage.MultiPanel multiPanel;
        private Component.MultiPage.MultiPanelPage Page_RichText;
        private Component.MultiPage.MultiPanelPage Page_P_D_W;
        private Component.MultiPage.MultiPanel multiPanel_P_D_W;
        private Component.MultiPage.MultiPanelPage Page__Link;
        private Component.MultiPage.MultiPanelPage Page__Photo;
        private Component.MultiPage.MultiPanelPage Page__DOC;
        private XPButton buttonRichText;
        private XPButton btnSend;
        private Panel panelText;
        private Panel panelPWD_Main;
        private Panel panelMiddleBar;
        private XPButton btnAddDoc;
        private XPButton btnAddPhoto;
        private Panel panelRichTextPanel;
        private Panel panelRichText_Main;
        private Panel panelRichText_Top;
        private XPButton btnPureText;
    }
}

