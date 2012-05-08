using System.Windows.Forms;
using Manina.Windows.Forms;
using Waveface.Component;
using Waveface.Component.RichEdit;
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
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.btnAddPhoto = new Waveface.Component.ImageButton();
            this.multiPanel = new Waveface.Component.MultiPage.MultiPanel();
            this.Page_RichText = new Waveface.Component.MultiPage.MultiPanelPage();
            this.panelRichTextPanel = new System.Windows.Forms.Panel();
            this.panelRichText_Main = new System.Windows.Forms.Panel();
            this.richText_UI = new Waveface.PostUI.RichText();
            this.panelRichText_Top = new System.Windows.Forms.Panel();
            this.btnPureText = new Waveface.Component.ImageButton();
            this.Page_P_D_W = new Waveface.Component.MultiPage.MultiPanelPage();
            this.panelPWD_Main = new System.Windows.Forms.Panel();
            this.multiPanel_P_D_W = new Waveface.Component.MultiPage.MultiPanel();
            this.Page__Link = new Waveface.Component.MultiPage.MultiPanelPage();
            this.weblink_UI = new Waveface.PostUI.WebLink();
            this.Page__Photo = new Waveface.Component.MultiPage.MultiPanelPage();
            this.photo_UI = new Waveface.PostUI.Photo();
            this.Page__DOC = new Waveface.Component.MultiPage.MultiPanelPage();
            this.document_UI = new Waveface.PostUI.Document();
            this.panelMiddleBar = new System.Windows.Forms.Panel();
            this.pictureBoxWaiting = new System.Windows.Forms.PictureBox();
            this.panelToolbar = new System.Windows.Forms.Panel();
            this.btnAddDoc = new Waveface.Component.ImageButton();
            this.btnSend = new Waveface.Component.ImageButton();
            this.labelPreviewMsg = new System.Windows.Forms.Label();
            this.panelText = new System.Windows.Forms.Panel();
            this.buttonRichText = new Waveface.Component.ImageButton();
            this.pureTextBox = new Waveface.Component.RichEdit.RichTextEditor();
            this.cultureManager = new Waveface.Localization.CultureManager(this.components);
            this.backgroundWorker_GetPreview = new System.ComponentModel.BackgroundWorker();
            this.timerNoPreviewMsg = new System.Windows.Forms.Timer(this.components);
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
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWaiting)).BeginInit();
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
            // btnAddPhoto
            // 
            resources.ApplyResources(this.btnAddPhoto, "btnAddPhoto");
            this.btnAddPhoto.CenterAlignImage = false;
            this.btnAddPhoto.ForeColor = System.Drawing.Color.White;
            this.btnAddPhoto.Image = global::Waveface.Properties.Resources.FB_blue_btn;
            this.btnAddPhoto.ImageDisable = global::Waveface.Properties.Resources.FB_blue_btn_hl;
            this.btnAddPhoto.ImageFront = global::Waveface.Properties.Resources.FB_edit_add_photo;
            this.btnAddPhoto.ImageHover = global::Waveface.Properties.Resources.FB_blue_btn_hl;
            this.btnAddPhoto.Name = "btnAddPhoto";
            this.btnAddPhoto.TabStop = false;
            this.toolTip.SetToolTip(this.btnAddPhoto, resources.GetString("btnAddPhoto.ToolTip"));
            this.btnAddPhoto.Click += new System.EventHandler(this.btnAddPhoto_Click);
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
            this.panelRichTextPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(223)))), ((int)(((byte)(220)))), ((int)(((byte)(213)))));
            this.panelRichTextPanel.Controls.Add(this.panelRichText_Main);
            this.panelRichTextPanel.Controls.Add(this.panelRichText_Top);
            resources.ApplyResources(this.panelRichTextPanel, "panelRichTextPanel");
            this.panelRichTextPanel.Name = "panelRichTextPanel";
            // 
            // panelRichText_Main
            // 
            this.panelRichText_Main.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(223)))), ((int)(((byte)(220)))), ((int)(((byte)(213)))));
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
            this.panelRichText_Top.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(223)))), ((int)(((byte)(220)))), ((int)(((byte)(213)))));
            this.panelRichText_Top.Controls.Add(this.btnPureText);
            resources.ApplyResources(this.panelRichText_Top, "panelRichText_Top");
            this.panelRichText_Top.Name = "panelRichText_Top";
            // 
            // btnPureText
            // 
            resources.ApplyResources(this.btnPureText, "btnPureText");
            this.btnPureText.CenterAlignImage = false;
            this.btnPureText.Image = global::Waveface.Properties.Resources.white_edit;
            this.btnPureText.ImageDisable = null;
            this.btnPureText.ImageFront = null;
            this.btnPureText.ImageHover = null;
            this.btnPureText.Name = "btnPureText";
            this.btnPureText.TabStop = false;
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
            this.panelPWD_Main.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(223)))), ((int)(((byte)(220)))), ((int)(((byte)(213)))));
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
            this.Page__Link.Controls.Add(this.weblink_UI);
            resources.ApplyResources(this.Page__Link, "Page__Link");
            this.Page__Link.Name = "Page__Link";
            // 
            // weblink_UI
            // 
            this.weblink_UI.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            resources.ApplyResources(this.weblink_UI, "weblink_UI");
            this.weblink_UI.MinimumSize = new System.Drawing.Size(500, 130);
            this.weblink_UI.MyParent = null;
            this.weblink_UI.Name = "weblink_UI";
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
            this.photo_UI.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.photo_UI.FileNameMapping = ((System.Collections.Generic.Dictionary<string, string>)(resources.GetObject("photo_UI.FileNameMapping")));
            this.photo_UI.MinimumSize = new System.Drawing.Size(500, 78);
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
            this.panelMiddleBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.panelMiddleBar.Controls.Add(this.pictureBoxWaiting);
            this.panelMiddleBar.Controls.Add(this.panelToolbar);
            this.panelMiddleBar.Controls.Add(this.btnSend);
            this.panelMiddleBar.Controls.Add(this.labelPreviewMsg);
            resources.ApplyResources(this.panelMiddleBar, "panelMiddleBar");
            this.panelMiddleBar.Name = "panelMiddleBar";
            // 
            // pictureBoxWaiting
            // 
            this.pictureBoxWaiting.Image = global::Waveface.Properties.Resources.loader;
            resources.ApplyResources(this.pictureBoxWaiting, "pictureBoxWaiting");
            this.pictureBoxWaiting.Name = "pictureBoxWaiting";
            this.pictureBoxWaiting.TabStop = false;
            // 
            // panelToolbar
            // 
            resources.ApplyResources(this.panelToolbar, "panelToolbar");
            this.panelToolbar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(212)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.panelToolbar.Controls.Add(this.btnAddPhoto);
            this.panelToolbar.Controls.Add(this.btnAddDoc);
            this.panelToolbar.Name = "panelToolbar";
            // 
            // btnAddDoc
            // 
            this.btnAddDoc.CenterAlignImage = false;
            this.btnAddDoc.Image = global::Waveface.Properties.Resources.page;
            this.btnAddDoc.ImageDisable = null;
            this.btnAddDoc.ImageFront = null;
            this.btnAddDoc.ImageHover = null;
            resources.ApplyResources(this.btnAddDoc, "btnAddDoc");
            this.btnAddDoc.Name = "btnAddDoc";
            this.btnAddDoc.TabStop = false;
            this.btnAddDoc.Click += new System.EventHandler(this.btnAddDoc_Click);
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
            this.btnSend.TabStop = false;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // labelPreviewMsg
            // 
            resources.ApplyResources(this.labelPreviewMsg, "labelPreviewMsg");
            this.labelPreviewMsg.AutoEllipsis = true;
            this.labelPreviewMsg.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(97)))), ((int)(((byte)(101)))));
            this.labelPreviewMsg.Name = "labelPreviewMsg";
            // 
            // panelText
            // 
            this.panelText.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.panelText.Controls.Add(this.buttonRichText);
            this.panelText.Controls.Add(this.pureTextBox);
            resources.ApplyResources(this.panelText, "panelText");
            this.panelText.Name = "panelText";
            // 
            // buttonRichText
            // 
            resources.ApplyResources(this.buttonRichText, "buttonRichText");
            this.buttonRichText.CenterAlignImage = false;
            this.buttonRichText.Image = global::Waveface.Properties.Resources.content;
            this.buttonRichText.ImageDisable = null;
            this.buttonRichText.ImageFront = null;
            this.buttonRichText.ImageHover = null;
            this.buttonRichText.Name = "buttonRichText";
            this.buttonRichText.TabStop = false;
            this.buttonRichText.Click += new System.EventHandler(this.buttonRichText_Click);
            // 
            // pureTextBox
            // 
            resources.ApplyResources(this.pureTextBox, "pureTextBox");
            this.pureTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.pureTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pureTextBox.ContextMenuStrip = this.contextMenuStripEdit;
            this.pureTextBox.DetectUrls = false;
            this.pureTextBox.Name = "pureTextBox";
            this.pureTextBox.UndoLength = 100;
            this.pureTextBox.WaterMarkColor = System.Drawing.Color.Silver;
            this.pureTextBox.WaterMarkText = "";
            this.pureTextBox.TextChanged2 += new Waveface.Component.RichEdit.TextChanged2EventHandler(this.pureTextBox_TextChanged2);
            this.pureTextBox.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.richTextBox_LinkClicked);
            this.pureTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.pureTextBox_KeyDown);
            // 
            // cultureManager
            // 
            this.cultureManager.ManagedControl = this;
            // 
            // timerNoPreviewMsg
            // 
            this.timerNoPreviewMsg.Interval = 5000;
            this.timerNoPreviewMsg.Tick += new System.EventHandler(this.timerNoPreviewMsg_Tick);
            // 
            // PostForm
            // 
            this.AllowDrop = true;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.multiPanel);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PostForm";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PostForm_FormClosing);
            this.SizeChanged += new System.EventHandler(this.PostForm_SizeChanged);
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
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWaiting)).EndInit();
            this.panelToolbar.ResumeLayout(false);
            this.panelText.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private WebLink weblink_UI;
        private Photo photo_UI;
        private Document document_UI;
        private RichText richText_UI;
        public RichTextEditor pureTextBox;
        private Component.MultiPage.MultiPanel multiPanel;
        private Component.MultiPage.MultiPanelPage Page_RichText;
        private Component.MultiPage.MultiPanelPage Page_P_D_W;
        private Component.MultiPage.MultiPanel multiPanel_P_D_W;
        private Component.MultiPage.MultiPanelPage Page__Link;
        private Component.MultiPage.MultiPanelPage Page__Photo;
        private Component.MultiPage.MultiPanelPage Page__DOC;
        private Component.ImageButton buttonRichText;
        private Component.ImageButton btnSend;
        private Panel panelText;
        private Panel panelPWD_Main;
        private Panel panelMiddleBar;
        private Component.ImageButton btnAddDoc;
        private Component.ImageButton btnAddPhoto;
        private Panel panelRichTextPanel;
        private Panel panelRichText_Main;
        private Panel panelRichText_Top;
        private Component.ImageButton btnPureText;
        private ContextMenuStrip contextMenuStripEdit;
        private ToolStripMenuItem cutToolStripMenuItem;
        private ToolStripMenuItem copyToolStripMenuItem;
        private ToolStripMenuItem pasteToolStripMenuItem;
        private Panel panelToolbar;
        private Localization.CultureManager cultureManager;
        private ToolTip toolTip;
        private System.ComponentModel.BackgroundWorker backgroundWorker_GetPreview;
        private Label labelPreviewMsg;
        private Timer timerNoPreviewMsg;
        private PictureBox pictureBoxWaiting;
    }
}

