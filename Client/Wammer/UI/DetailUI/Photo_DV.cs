#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using Manina.Windows.Forms;
using Waveface.API.V2;
using Waveface.Component;

#endregion

namespace Waveface.DetailUI
{
    public class Photo_DV : UserControl
    {
        private IContainer components = null;
        private Panel panelMain;
        private Panel panelRight;
        private WebBrowser webBrowserTop;
        private Panel PanelAddComment;
        private WebBrowser webBrowserComment;
        private Panel PanelPictures;
        private ImageListView imageListView;
        private PictureBox pictureBoxRemote;
        private Post m_post;
        private int imageFileIndex;
        private string m_downloadFileName;
        private XPButton buttonAddComment;
        private TextBox textBoxComment;
        private PhotoView m_photoView;

        private List<Attachment> m_imageAttachments;

        public Post Post
        {
            get
            {
                return m_post;
            }
            set
            {
                m_post = value;

                if (m_post != null)
                    RefreshUI();
            }
        }

        public User User { get; set; }

        public DetailView MyParent { get; set; }

        public Photo_DV()
        {
            InitializeComponent();

            imageListView.SetRenderer(new MyImageListViewRenderer());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        // private void InitializeComponent()
        // {
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Photo_DV));
            this.panelMain = new System.Windows.Forms.Panel();
            this.pictureBoxRemote = new System.Windows.Forms.PictureBox();
            this.panelRight = new System.Windows.Forms.Panel();
            this.PanelAddComment = new System.Windows.Forms.Panel();
            this.buttonAddComment = new Waveface.Component.XPButton();
            this.textBoxComment = new System.Windows.Forms.TextBox();
            this.webBrowserComment = new System.Windows.Forms.WebBrowser();
            this.PanelPictures = new System.Windows.Forms.Panel();
            this.imageListView = new Manina.Windows.Forms.ImageListView();
            this.webBrowserTop = new System.Windows.Forms.WebBrowser();
            this.panelMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRemote)).BeginInit();
            this.panelRight.SuspendLayout();
            this.PanelAddComment.SuspendLayout();
            this.PanelPictures.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelMain
            // 
            this.panelMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelMain.BackColor = System.Drawing.SystemColors.Window;
            this.panelMain.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelMain.Controls.Add(this.pictureBoxRemote);
            this.panelMain.Controls.Add(this.panelRight);
            this.panelMain.Location = new System.Drawing.Point(3, 3);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(529, 487);
            this.panelMain.TabIndex = 0;
            // 
            // pictureBoxRemote
            // 
            this.pictureBoxRemote.Location = new System.Drawing.Point(17, 162);
            this.pictureBoxRemote.Name = "pictureBoxRemote";
            this.pictureBoxRemote.Size = new System.Drawing.Size(54, 53);
            this.pictureBoxRemote.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxRemote.TabIndex = 3;
            this.pictureBoxRemote.TabStop = false;
            this.pictureBoxRemote.Visible = false;
            this.pictureBoxRemote.LoadCompleted += new System.ComponentModel.AsyncCompletedEventHandler(this.pictureBoxRemote_LoadCompleted);
            // 
            // panelRight
            // 
            this.panelRight.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelRight.AutoScroll = true;
            this.panelRight.Controls.Add(this.PanelAddComment);
            this.panelRight.Controls.Add(this.webBrowserComment);
            this.panelRight.Controls.Add(this.PanelPictures);
            this.panelRight.Controls.Add(this.webBrowserTop);
            this.panelRight.Location = new System.Drawing.Point(16, 0);
            this.panelRight.Name = "panelRight";
            this.panelRight.Size = new System.Drawing.Size(510, 482);
            this.panelRight.TabIndex = 2;
            // 
            // PanelAddComment
            // 
            this.PanelAddComment.AutoScroll = true;
            this.PanelAddComment.AutoScrollMinSize = new System.Drawing.Size(345, 0);
            this.PanelAddComment.AutoSize = true;
            this.PanelAddComment.BackColor = System.Drawing.SystemColors.Window;
            this.PanelAddComment.Controls.Add(this.buttonAddComment);
            this.PanelAddComment.Controls.Add(this.textBoxComment);
            this.PanelAddComment.Dock = System.Windows.Forms.DockStyle.Top;
            this.PanelAddComment.Location = new System.Drawing.Point(0, 355);
            this.PanelAddComment.Name = "PanelAddComment";
            this.PanelAddComment.Size = new System.Drawing.Size(510, 67);
            this.PanelAddComment.TabIndex = 3;
            this.PanelAddComment.Visible = false;
            // 
            // buttonAddComment
            // 
            this.buttonAddComment.AdjustImageLocation = new System.Drawing.Point(0, 0);
            this.buttonAddComment.BtnShape = Waveface.Component.emunType.BtnShape.Rectangle;
            this.buttonAddComment.BtnStyle = Waveface.Component.emunType.XPStyle.Silver;
            this.buttonAddComment.Location = new System.Drawing.Point(32, 35);
            this.buttonAddComment.Name = "buttonAddComment";
            this.buttonAddComment.Size = new System.Drawing.Size(102, 29);
            this.buttonAddComment.TabIndex = 1;
            this.buttonAddComment.Text = "Add Comment";
            this.buttonAddComment.UseVisualStyleBackColor = true;
            this.buttonAddComment.Click += new System.EventHandler(this.buttonAddComment_Click);
            // 
            // textBoxComment
            // 
            this.textBoxComment.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxComment.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxComment.Location = new System.Drawing.Point(32, 3);
            this.textBoxComment.Name = "textBoxComment";
            this.textBoxComment.Size = new System.Drawing.Size(467, 26);
            this.textBoxComment.TabIndex = 0;
            // 
            // webBrowserComment
            // 
            this.webBrowserComment.AllowWebBrowserDrop = false;
            this.webBrowserComment.Dock = System.Windows.Forms.DockStyle.Top;
            this.webBrowserComment.Location = new System.Drawing.Point(0, 258);
            this.webBrowserComment.Name = "webBrowserComment";
            this.webBrowserComment.ScrollBarsEnabled = false;
            this.webBrowserComment.Size = new System.Drawing.Size(510, 97);
            this.webBrowserComment.TabIndex = 2;
            this.webBrowserComment.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowserComment_DocumentCompleted);
            // 
            // PanelPictures
            // 
            this.PanelPictures.AutoScroll = true;
            this.PanelPictures.AutoScrollMinSize = new System.Drawing.Size(345, 0);
            this.PanelPictures.BackColor = System.Drawing.SystemColors.Window;
            this.PanelPictures.Controls.Add(this.imageListView);
            this.PanelPictures.Dock = System.Windows.Forms.DockStyle.Top;
            this.PanelPictures.Location = new System.Drawing.Point(0, 97);
            this.PanelPictures.Name = "PanelPictures";
            this.PanelPictures.Size = new System.Drawing.Size(510, 161);
            this.PanelPictures.TabIndex = 1;
            // 
            // imageListView
            // 
            this.imageListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.imageListView.ColumnHeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.imageListView.DefaultImage = ((System.Drawing.Image)(resources.GetObject("imageListView.DefaultImage")));
            this.imageListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imageListView.ErrorImage = ((System.Drawing.Image)(resources.GetObject("imageListView.ErrorImage")));
            this.imageListView.GroupHeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.imageListView.Location = new System.Drawing.Point(0, 0);
            this.imageListView.Name = "imageListView";
            this.imageListView.Size = new System.Drawing.Size(510, 161);
            this.imageListView.TabIndex = 1;
            this.imageListView.ThumbnailSize = new System.Drawing.Size(120, 120);
            this.imageListView.ItemClick += new Manina.Windows.Forms.ItemClickEventHandler(this.imageListView_ItemClick);
            // 
            // webBrowserTop
            // 
            this.webBrowserTop.AllowWebBrowserDrop = false;
            this.webBrowserTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.webBrowserTop.Location = new System.Drawing.Point(0, 0);
            this.webBrowserTop.Name = "webBrowserTop";
            this.webBrowserTop.ScrollBarsEnabled = false;
            this.webBrowserTop.Size = new System.Drawing.Size(510, 97);
            this.webBrowserTop.TabIndex = 0;
            this.webBrowserTop.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowserTop_DocumentCompleted);
            // 
            // Photo_DV
            // 
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.panelMain);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "Photo_DV";
            this.Size = new System.Drawing.Size(535, 493);
            this.Resize += new System.EventHandler(this.DetailView_Resize);
            this.panelMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRemote)).EndInit();
            this.panelRight.ResumeLayout(false);
            this.panelRight.PerformLayout();
            this.PanelAddComment.ResumeLayout(false);
            this.PanelAddComment.PerformLayout();
            this.PanelPictures.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        #region Event Handlers

        protected override void OnPaint(PaintEventArgs e)
        {
            LinearGradientBrush _brush = new LinearGradientBrush(ClientRectangle, Color.FromArgb(106, 112, 128), Color.FromArgb(138, 146, 166), LinearGradientMode.ForwardDiagonal);

            e.Graphics.FillRectangle(_brush, ClientRectangle);

            base.OnPaint(e);
        }

        #endregion

        private void RefreshUI()
        {
            Set_MainContent_Part();
            Set_Comments_Part();
            Set_Pictures();

            PanelAddComment.Visible = true;

            ReLayout();
        }

        private void ReLayout()
        {
            if (Post.attachments_count > 0)
            {
                PanelPictures.Height = 160;
            }
            else
            {
                PanelPictures.Height = 0;
            }

            PanelPictures.Height = imageListView.VScrollBar.Maximum + 16;
        }


        private void Set_Comments_Part()
        {
            MyParent.SetComments(webBrowserComment, Post);
        }

        private void Set_MainContent_Part()
        {
            StringBuilder _sb = new StringBuilder();

            _sb.Append("<p><strong>[Text]</strong></p>");

            string _html = _sb.ToString();
            _html = _html.Replace("[Text]", Post.content);

            webBrowserTop.DocumentText = _html;
        }

        private void webBrowserTop_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            int _h = webBrowserTop.Document.Body.ScrollRectangle.Height;
            webBrowserTop.Height = _h;
        }

        private void webBrowserComment_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            int _h = webBrowserComment.Document.Body.ScrollRectangle.Height;
            webBrowserComment.Height = _h;
        }

        private void Set_Pictures()
        {    
            imageListView.Items.Clear();

            m_imageAttachments = new List<Attachment>();

            foreach (Attachment _a in Post.attachments)
            {
                if(_a.type == "image")
                    m_imageAttachments.Add(_a);
            }

            if (m_imageAttachments.Count == 0)
                return;
            
            imageFileIndex = 0;

            if (Post.attachments_count > 0)
            {
                m_downloadFileName = m_imageAttachments[imageFileIndex].file_name;
                DownloadRemoteFile();
            }
        }

        #region File Download [Picture]

        private void DownloadRemoteFile()
        {
            string _localFile = MainForm.GCONST.CachePath + m_downloadFileName;

            if (System.IO.File.Exists(_localFile))
            {
                imageListView.Items.Add(_localFile);
                DownloadRemoteFileNext();
            }
            else
            {
                string _url = MainForm.THIS.attachments_getRedirectURL(m_imageAttachments[imageFileIndex].url);
                
                pictureBoxRemote.LoadAsync(_url);
                m_downloadFileName = m_imageAttachments[imageFileIndex].file_name;
            }
        }

        private void pictureBoxRemote_LoadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            try
            {
                string _localFile = MainForm.GCONST.CachePath + m_downloadFileName;

                pictureBoxRemote.Image.Save(_localFile);

                imageListView.Items.Add(_localFile);

                DownloadRemoteFileNext();

                PanelPictures.Height = imageListView.VScrollBar.Maximum + 16;
            }
            catch (Exception _e)
            {
                Console.WriteLine(_e.Message);
            }
        }

        private void DownloadRemoteFileNext()
        {
            imageFileIndex++;

            if (imageFileIndex < m_imageAttachments.Count)
            {
               m_downloadFileName = m_imageAttachments[imageFileIndex].file_name;
               DownloadRemoteFile();
            }
        }

        #endregion

        private void imageListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            List<string> _files = new List<string>();

            foreach (var _file in imageListView.Items)
            {
                _files.Add(_file.FileName);
            }

            m_photoView = new PhotoView(_files, e.Item.FileName);
            m_photoView.ShowDialog();
        }

        private void DetailView_Resize(object sender, EventArgs e)
        {
            PanelPictures.Height = imageListView.VScrollBar.Maximum + 16;
        }

        private void buttonAddComment_Click(object sender, EventArgs e)
        {
            MyParent.PostComment(textBoxComment, Post);
        }
    }
}