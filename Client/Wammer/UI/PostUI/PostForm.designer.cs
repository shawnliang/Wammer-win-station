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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PostForm));
            this.contextMenuStripEdit = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.panelToolbar = new System.Windows.Forms.Panel();
            this.btnAddPhoto = new Waveface.Component.XPButton();
            this.btnAddDoc = new Waveface.Component.XPButton();
            this.btnSend = new Waveface.Component.XPButton();
            this.panelText = new System.Windows.Forms.Panel();
            this.buttonRichText = new Waveface.Component.XPButton();
            this.richTextBox = new System.Windows.Forms.RichTextBox();
            this.cultureManager = new Waveface.Localization.CultureManager(this.components);
            this.contextMenuStripEdit.SuspendLayout();
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
            this.panelToolbar.SuspendLayout();
            this.panelText.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStripEdit
            // 
            resources.ApplyResources(this.contextMenuStripEdit, "contextMenuStripEdit");
            this.contextMenuStripEdit.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem});
            this.contextMenuStripEdit.Name = "contextMenuStrip1";
            this.contextMenuStripEdit.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripEdit_Opening);
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            resources.ApplyResources(this.cutToolStripMenuItem, "cutToolStripMenuItem");
            this.cutToolStripMenuItem.Click += new System.EventHandler(this.cutToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            resources.ApplyResources(this.copyToolStripMenuItem, "copyToolStripMenuItem");
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            resources.ApplyResources(this.pasteToolStripMenuItem, "pasteToolStripMenuItem");
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // multiPanel
            // 
            this.multiPanel.Controls.Add(this.Page_RichText);
            this.multiPanel.Controls.Add(this.Page_P_D_W);
            resources.ApplyResources(this.multiPanel, "multiPanel");
            this.multiPanel.Name = "multiPanel";
            this.multiPanel.SelectedPage = this.Page_P_D_W;
            // 
            // Page_RichText
            // 
            this.Page_RichText.Controls.Add(this.panelRichTextPanel);
            resources.ApplyResources(this.Page_RichText, "Page_RichText");
            this.Page_RichText.Name = "Page_RichText";
            // 
            // panelRichTextPanel
            // 
            this.panelRichTextPanel.Controls.Add(this.panelRichText_Main);
            this.panelRichTextPanel.Controls.Add(this.panelRichText_Top);
            resources.ApplyResources(this.panelRichTextPanel, "panelRichTextPanel");
            this.panelRichTextPanel.Name = "panelRichTextPanel";
            // 
            // panelRichText_Main
            // 
            this.panelRichText_Main.Controls.Add(this.richText_UI);
            resources.ApplyResources(this.panelRichText_Main, "panelRichText_Main");
            this.panelRichText_Main.Name = "panelRichText_Main";
            // 
            // richText_UI
            // 
            resources.ApplyResources(this.richText_UI, "richText_UI");
            this.richText_UI.MinimumSize = new System.Drawing.Size(500, 130);
            this.richText_UI.MyParent = null;
            this.richText_UI.Name = "richText_UI";
            // 
            // panelRichText_Top
            // 
            this.panelRichText_Top.Controls.Add(this.btnPureText);
            resources.ApplyResources(this.panelRichText_Top, "panelRichText_Top");
            this.panelRichText_Top.Name = "panelRichText_Top";
            // 
            // btnPureText
            // 
            this.btnPureText.AdjustImageLocation = new System.Drawing.Point(0, 0);
            resources.ApplyResources(this.btnPureText, "btnPureText");
            this.btnPureText.BtnShape = Waveface.Component.emunType.BtnShape.Rectangle;
            this.btnPureText.BtnStyle = Waveface.Component.emunType.XPStyle.Silver;
            this.btnPureText.Image = global::Waveface.Properties.Resources.white_edit;
            this.btnPureText.Name = "btnPureText";
            this.btnPureText.UseVisualStyleBackColor = true;
            this.btnPureText.Click += new System.EventHandler(this.btnPureText_Click);
            // 
            // Page_P_D_W
            // 
            this.Page_P_D_W.Controls.Add(this.panelPWD_Main);
            this.Page_P_D_W.Controls.Add(this.panelMiddleBar);
            this.Page_P_D_W.Controls.Add(this.panelText);
            resources.ApplyResources(this.Page_P_D_W, "Page_P_D_W");
            this.Page_P_D_W.Name = "Page_P_D_W";
            // 
            // panelPWD_Main
            // 
            this.panelPWD_Main.Controls.Add(this.multiPanel_P_D_W);
            resources.ApplyResources(this.panelPWD_Main, "panelPWD_Main");
            this.panelPWD_Main.Name = "panelPWD_Main";
            // 
            // multiPanel_P_D_W
            // 
            this.multiPanel_P_D_W.Controls.Add(this.Page__Link);
            this.multiPanel_P_D_W.Controls.Add(this.Page__Photo);
            this.multiPanel_P_D_W.Controls.Add(this.Page__DOC);
            resources.ApplyResources(this.multiPanel_P_D_W, "multiPanel_P_D_W");
            this.multiPanel_P_D_W.Name = "multiPanel_P_D_W";
            this.multiPanel_P_D_W.SelectedPage = this.Page__Link;
            // 
            // Page__Link
            // 
            this.Page__Link.Controls.Add(this.general_weblink_UI);
            resources.ApplyResources(this.Page__Link, "Page__Link");
            this.Page__Link.Name = "Page__Link";
            // 
            // general_weblink_UI
            // 
            resources.ApplyResources(this.general_weblink_UI, "general_weblink_UI");
            this.general_weblink_UI.MinimumSize = new System.Drawing.Size(500, 130);
            this.general_weblink_UI.MyParent = null;
            this.general_weblink_UI.Name = "general_weblink_UI";
            // 
            // Page__Photo
            // 
            this.Page__Photo.Controls.Add(this.photo_UI);
            resources.ApplyResources(this.Page__Photo, "Page__Photo");
            this.Page__Photo.Name = "Page__Photo";
            // 
            // photo_UI
            // 
            resources.ApplyResources(this.photo_UI, "photo_UI");
            this.photo_UI.BackColor = System.Drawing.Color.White;
            this.photo_UI.MinimumSize = new System.Drawing.Size(500, 130);
            this.photo_UI.MyParent = null;
            this.photo_UI.Name = "photo_UI";
            // 
            // Page__DOC
            // 
            this.Page__DOC.Controls.Add(this.document_UI);
            resources.ApplyResources(this.Page__DOC, "Page__DOC");
            this.Page__DOC.Name = "Page__DOC";
            // 
            // document_UI
            // 
            resources.ApplyResources(this.document_UI, "document_UI");
            this.document_UI.MinimumSize = new System.Drawing.Size(500, 130);
            this.document_UI.MyParent = null;
            this.document_UI.Name = "document_UI";
            // 
            // panelMiddleBar
            // 
            this.panelMiddleBar.BackColor = System.Drawing.Color.White;
            this.panelMiddleBar.Controls.Add(this.panelToolbar);
            this.panelMiddleBar.Controls.Add(this.btnSend);
            resources.ApplyResources(this.panelMiddleBar, "panelMiddleBar");
            this.panelMiddleBar.Name = "panelMiddleBar";
            // 
            // panelToolbar
            // 
            resources.ApplyResources(this.panelToolbar, "panelToolbar");
            this.panelToolbar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(223)))), ((int)(((byte)(220)))), ((int)(((byte)(213)))));
            this.panelToolbar.Controls.Add(this.btnAddPhoto);
            this.panelToolbar.Controls.Add(this.btnAddDoc);
            this.panelToolbar.Name = "panelToolbar";
            // 
            // btnAddPhoto
            // 
            this.btnAddPhoto.AdjustImageLocation = new System.Drawing.Point(0, 0);
            this.btnAddPhoto.BtnShape = Waveface.Component.emunType.BtnShape.Rectangle;
            this.btnAddPhoto.BtnStyle = Waveface.Component.emunType.XPStyle.Silver;
            this.btnAddPhoto.Image = global::Waveface.Properties.Resources.add_photo;
            resources.ApplyResources(this.btnAddPhoto, "btnAddPhoto");
            this.btnAddPhoto.Name = "btnAddPhoto";
            this.btnAddPhoto.UseVisualStyleBackColor = true;
            this.btnAddPhoto.Click += new System.EventHandler(this.btnAddPhoto_Click);
            // 
            // btnAddDoc
            // 
            this.btnAddDoc.AdjustImageLocation = new System.Drawing.Point(0, 0);
            this.btnAddDoc.BtnShape = Waveface.Component.emunType.BtnShape.Rectangle;
            this.btnAddDoc.BtnStyle = Waveface.Component.emunType.XPStyle.Silver;
            this.btnAddDoc.Image = global::Waveface.Properties.Resources.page;
            resources.ApplyResources(this.btnAddDoc, "btnAddDoc");
            this.btnAddDoc.Name = "btnAddDoc";
            this.btnAddDoc.UseVisualStyleBackColor = true;
            this.btnAddDoc.Click += new System.EventHandler(this.btnAddDoc_Click);
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
            // panelText
            // 
            this.panelText.BackColor = System.Drawing.Color.White;
            this.panelText.Controls.Add(this.buttonRichText);
            this.panelText.Controls.Add(this.richTextBox);
            resources.ApplyResources(this.panelText, "panelText");
            this.panelText.Name = "panelText";
            // 
            // buttonRichText
            // 
            this.buttonRichText.AdjustImageLocation = new System.Drawing.Point(0, 0);
            resources.ApplyResources(this.buttonRichText, "buttonRichText");
            this.buttonRichText.BtnShape = Waveface.Component.emunType.BtnShape.Rectangle;
            this.buttonRichText.BtnStyle = Waveface.Component.emunType.XPStyle.Silver;
            this.buttonRichText.Image = global::Waveface.Properties.Resources.content;
            this.buttonRichText.Name = "buttonRichText";
            this.buttonRichText.UseVisualStyleBackColor = true;
            this.buttonRichText.Click += new System.EventHandler(this.buttonRichText_Click);
            // 
            // richTextBox
            // 
            resources.ApplyResources(this.richTextBox, "richTextBox");
            this.richTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(242)))), ((int)(((byte)(238)))));
            this.richTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox.ContextMenuStrip = this.contextMenuStripEdit;
            this.richTextBox.DetectUrls = false;
            this.richTextBox.Name = "richTextBox";
            this.richTextBox.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.richTextBox_LinkClicked);
            this.richTextBox.TextChanged += new System.EventHandler(this.richTextBox_TextChanged);
            // 
            // cultureManager
            // 
            this.cultureManager.ManagedControl = this;
            // 
            // PostForm
            // 
            this.AllowDrop = true;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.multiPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PostForm";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PostForm_FormClosing);
            this.Resize += new System.EventHandler(this.PostForm_Resize);
            this.contextMenuStripEdit.ResumeLayout(false);
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
            this.panelToolbar.ResumeLayout(false);
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
        private ContextMenuStrip contextMenuStripEdit;
        private ToolStripMenuItem cutToolStripMenuItem;
        private ToolStripMenuItem copyToolStripMenuItem;
        private ToolStripMenuItem pasteToolStripMenuItem;
        private Panel panelToolbar;
        private Localization.CultureManager cultureManager;
    }
}

