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
            this.multiPanel = new Waveface.Component.MultiPage.MultiPanel();
            this.Page_RichText = new Waveface.Component.MultiPage.MultiPanelPage();
            this.panelRichTextPanel = new System.Windows.Forms.Panel();
            this.panelRichText_Main = new System.Windows.Forms.Panel();
            this.panelRichText_Top = new System.Windows.Forms.Panel();
            this.Page_P_D_W = new Waveface.Component.MultiPage.MultiPanelPage();
            this.panelPWD_Main = new System.Windows.Forms.Panel();
            this.multiPanel_P_D_W = new Waveface.Component.MultiPage.MultiPanel();
            this.Page__Link = new Waveface.Component.MultiPage.MultiPanelPage();
            this.Page__Photo = new Waveface.Component.MultiPage.MultiPanelPage();
            this.Page__DOC = new Waveface.Component.MultiPage.MultiPanelPage();
            this.panelMiddleBar = new System.Windows.Forms.Panel();
            this.cbGenerateWebPreview = new System.Windows.Forms.CheckBox();
            this.panelToolbar = new System.Windows.Forms.Panel();
            this.panelText = new System.Windows.Forms.Panel();
            this.cultureManager = new Waveface.Localization.CultureManager(this.components);
            this.backgroundWorker_GetPreview = new System.ComponentModel.BackgroundWorker();
            this.labelNoPreviewMsg = new System.Windows.Forms.Label();
            this.timerNoPreviewMsg = new System.Windows.Forms.Timer(this.components);
            this.richText_UI = new Waveface.PostUI.RichText();
            this.btnPureText = new Waveface.Component.XPButton();
            this.weblink_UI = new Waveface.PostUI.WebLink();
            this.photo_UI = new Waveface.PostUI.Photo();
            this.document_UI = new Waveface.PostUI.Document();
            this.btnAddPhoto = new Waveface.Component.XPButton();
            this.btnAddDoc = new Waveface.Component.XPButton();
            this.btnSend = new Waveface.Component.XPButton();
            this.buttonRichText = new Waveface.Component.XPButton();
            this.pureTextBox = new Waveface.Component.RichEdit.RichTextEditor();
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
            // panelRichText_Top
            // 
            this.panelRichText_Top.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(223)))), ((int)(((byte)(220)))), ((int)(((byte)(213)))));
            this.panelRichText_Top.Controls.Add(this.btnPureText);
            resources.ApplyResources(this.panelRichText_Top, "panelRichText_Top");
            this.panelRichText_Top.Name = "panelRichText_Top";
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
            // Page__Photo
            // 
            this.Page__Photo.Controls.Add(this.photo_UI);
            resources.ApplyResources(this.Page__Photo, "Page__Photo");
            this.Page__Photo.Name = "Page__Photo";
            // 
            // Page__DOC
            // 
            this.Page__DOC.Controls.Add(this.document_UI);
            resources.ApplyResources(this.Page__DOC, "Page__DOC");
            this.Page__DOC.Name = "Page__DOC";
            // 
            // panelMiddleBar
            // 
            this.panelMiddleBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(242)))), ((int)(((byte)(238)))));
            this.panelMiddleBar.Controls.Add(this.labelNoPreviewMsg);
            this.panelMiddleBar.Controls.Add(this.cbGenerateWebPreview);
            this.panelMiddleBar.Controls.Add(this.panelToolbar);
            this.panelMiddleBar.Controls.Add(this.btnSend);
            resources.ApplyResources(this.panelMiddleBar, "panelMiddleBar");
            this.panelMiddleBar.Name = "panelMiddleBar";
            // 
            // cbGenerateWebPreview
            // 
            resources.ApplyResources(this.cbGenerateWebPreview, "cbGenerateWebPreview");
            this.cbGenerateWebPreview.Checked = true;
            this.cbGenerateWebPreview.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbGenerateWebPreview.Name = "cbGenerateWebPreview";
            this.cbGenerateWebPreview.UseVisualStyleBackColor = true;
            this.cbGenerateWebPreview.CheckedChanged += new System.EventHandler(this.cbGenerateWebPreview_CheckedChanged);
            // 
            // panelToolbar
            // 
            resources.ApplyResources(this.panelToolbar, "panelToolbar");
            this.panelToolbar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(202)))), ((int)(((byte)(197)))), ((int)(((byte)(186)))));
            this.panelToolbar.Controls.Add(this.btnAddPhoto);
            this.panelToolbar.Controls.Add(this.btnAddDoc);
            this.panelToolbar.Name = "panelToolbar";
            // 
            // panelText
            // 
            this.panelText.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(242)))), ((int)(((byte)(238)))));
            this.panelText.Controls.Add(this.buttonRichText);
            this.panelText.Controls.Add(this.pureTextBox);
            resources.ApplyResources(this.panelText, "panelText");
            this.panelText.Name = "panelText";
            // 
            // cultureManager
            // 
            this.cultureManager.ManagedControl = this;
            // 
            // labelNoPreviewMsg
            // 
            resources.ApplyResources(this.labelNoPreviewMsg, "labelNoPreviewMsg");
            this.labelNoPreviewMsg.ForeColor = System.Drawing.Color.Red;
            this.labelNoPreviewMsg.Name = "labelNoPreviewMsg";
            // 
            // timerNoPreviewMsg
            // 
            this.timerNoPreviewMsg.Interval = 5000;
            this.timerNoPreviewMsg.Tick += new System.EventHandler(this.timerNoPreviewMsg_Tick);
            // 
            // richText_UI
            // 
            resources.ApplyResources(this.richText_UI, "richText_UI");
            this.richText_UI.MinimumSize = new System.Drawing.Size(500, 130);
            this.richText_UI.MyParent = null;
            this.richText_UI.Name = "richText_UI";
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
            // weblink_UI
            // 
            this.weblink_UI.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(242)))), ((int)(((byte)(238)))));
            resources.ApplyResources(this.weblink_UI, "weblink_UI");
            this.weblink_UI.MinimumSize = new System.Drawing.Size(500, 130);
            this.weblink_UI.MyParent = null;
            this.weblink_UI.Name = "weblink_UI";
            // 
            // photo_UI
            // 
            resources.ApplyResources(this.photo_UI, "photo_UI");
            this.photo_UI.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(242)))), ((int)(((byte)(238)))));
            this.photo_UI.MinimumSize = new System.Drawing.Size(500, 130);
            this.photo_UI.MyParent = null;
            this.photo_UI.Name = "photo_UI";
            // 
            // document_UI
            // 
            resources.ApplyResources(this.document_UI, "document_UI");
            this.document_UI.MinimumSize = new System.Drawing.Size(500, 130);
            this.document_UI.MyParent = null;
            this.document_UI.Name = "document_UI";
            // 
            // btnAddPhoto
            // 
            this.btnAddPhoto.AdjustImageLocation = new System.Drawing.Point(0, 0);
            resources.ApplyResources(this.btnAddPhoto, "btnAddPhoto");
            this.btnAddPhoto.BtnShape = Waveface.Component.emunType.BtnShape.Rectangle;
            this.btnAddPhoto.BtnStyle = Waveface.Component.emunType.XPStyle.Silver;
            this.btnAddPhoto.Image = global::Waveface.Properties.Resources.add_photo;
            this.btnAddPhoto.Name = "btnAddPhoto";
            this.toolTip.SetToolTip(this.btnAddPhoto, resources.GetString("btnAddPhoto.ToolTip"));
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
            this.pureTextBox.TextChanged += new System.EventHandler(this.pureTextBox_TextChanged);
            // 
            // PostForm
            // 
            this.AllowDrop = true;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            resources.ApplyResources(this, "$this");
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
            this.panelMiddleBar.PerformLayout();
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
        private ToolTip toolTip;
        private System.ComponentModel.BackgroundWorker backgroundWorker_GetPreview;
        private CheckBox cbGenerateWebPreview;
        private Label labelNoPreviewMsg;
        private Timer timerNoPreviewMsg;
    }
}

