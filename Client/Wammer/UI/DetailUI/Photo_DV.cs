#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using Manina.Windows.Forms;
using NLog;
using Waveface.API.V2;
using Waveface.Component;

#endregion

namespace Waveface.DetailUI
{
    public class Photo_DV : UserControl, IDetailViewer
    {
        private static Logger s_logger = LogManager.GetCurrentClassLogger();

        #region Fields

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
        private Panel panelPictureInfo;
        private Label labelPictureInfo;
        private Dictionary<string, string> m_filesMapping;

        #endregion

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

            MyImageListViewRenderer _imageListViewRenderer = new MyImageListViewRenderer();
            _imageListViewRenderer.Clip = false;

            imageListView.SetRenderer(_imageListViewRenderer);

            imageListView.BackColor = Color.FromArgb(243, 242, 238);
            imageListView.Colors.BackColor = Color.FromArgb(243, 242, 238);
            imageListView.Colors.DisabledBackColor = Color.FromArgb(243, 242, 238);
            imageListView.ThumbnailSize = new Size(128, 128);
            imageListView.CacheMode = CacheMode.Continuous;

            m_filesMapping = new Dictionary<string, string>();
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
            this.panelRight = new System.Windows.Forms.Panel();
            this.PanelAddComment = new System.Windows.Forms.Panel();
            this.buttonAddComment = new Waveface.Component.XPButton();
            this.textBoxComment = new System.Windows.Forms.TextBox();
            this.webBrowserComment = new System.Windows.Forms.WebBrowser();
            this.PanelPictures = new System.Windows.Forms.Panel();
            this.panelPictureInfo = new System.Windows.Forms.Panel();
            this.labelPictureInfo = new System.Windows.Forms.Label();
            this.webBrowserTop = new System.Windows.Forms.WebBrowser();
            this.imageListView = new Manina.Windows.Forms.ImageListView();
            this.pictureBoxRemote = new System.Windows.Forms.PictureBox();
            this.panelMain.SuspendLayout();
            this.panelRight.SuspendLayout();
            this.PanelAddComment.SuspendLayout();
            this.PanelPictures.SuspendLayout();
            this.panelPictureInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRemote)).BeginInit();
            this.SuspendLayout();
            // 
            // panelMain
            // 
            resources.ApplyResources(this.panelMain, "panelMain");
            this.panelMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(242)))), ((int)(((byte)(238)))));
            this.panelMain.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelMain.Controls.Add(this.panelRight);
            this.panelMain.Name = "panelMain";
            // 
            // panelRight
            // 
            resources.ApplyResources(this.panelRight, "panelRight");
            this.panelRight.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(242)))), ((int)(((byte)(238)))));
            this.panelRight.Controls.Add(this.PanelAddComment);
            this.panelRight.Controls.Add(this.webBrowserComment);
            this.panelRight.Controls.Add(this.PanelPictures);
            this.panelRight.Controls.Add(this.panelPictureInfo);
            this.panelRight.Controls.Add(this.webBrowserTop);
            this.panelRight.Name = "panelRight";
            // 
            // PanelAddComment
            // 
            resources.ApplyResources(this.PanelAddComment, "PanelAddComment");
            this.PanelAddComment.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(242)))), ((int)(((byte)(238)))));
            this.PanelAddComment.Controls.Add(this.buttonAddComment);
            this.PanelAddComment.Controls.Add(this.textBoxComment);
            this.PanelAddComment.Name = "PanelAddComment";
            // 
            // buttonAddComment
            // 
            resources.ApplyResources(this.buttonAddComment, "buttonAddComment");
            this.buttonAddComment.AdjustImageLocation = new System.Drawing.Point(0, 0);
            this.buttonAddComment.BtnShape = Waveface.Component.emunType.BtnShape.Rectangle;
            this.buttonAddComment.BtnStyle = Waveface.Component.emunType.XPStyle.Silver;
            this.buttonAddComment.Name = "buttonAddComment";
            this.buttonAddComment.UseVisualStyleBackColor = true;
            this.buttonAddComment.Click += new System.EventHandler(this.buttonAddComment_Click);
            // 
            // textBoxComment
            // 
            resources.ApplyResources(this.textBoxComment, "textBoxComment");
            this.textBoxComment.Name = "textBoxComment";
            // 
            // webBrowserComment
            // 
            resources.ApplyResources(this.webBrowserComment, "webBrowserComment");
            this.webBrowserComment.AllowWebBrowserDrop = false;
            this.webBrowserComment.Name = "webBrowserComment";
            this.webBrowserComment.ScrollBarsEnabled = false;
            this.webBrowserComment.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowserComment_DocumentCompleted);
            // 
            // PanelPictures
            // 
            resources.ApplyResources(this.PanelPictures, "PanelPictures");
            this.PanelPictures.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(242)))), ((int)(((byte)(238)))));
            this.PanelPictures.Controls.Add(this.imageListView);
            this.PanelPictures.Controls.Add(this.pictureBoxRemote);
            this.PanelPictures.Name = "PanelPictures";
            // 
            // panelPictureInfo
            // 
            resources.ApplyResources(this.panelPictureInfo, "panelPictureInfo");
            this.panelPictureInfo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(208)))), ((int)(((byte)(170)))));
            this.panelPictureInfo.Controls.Add(this.labelPictureInfo);
            this.panelPictureInfo.Name = "panelPictureInfo";
            // 
            // labelPictureInfo
            // 
            resources.ApplyResources(this.labelPictureInfo, "labelPictureInfo");
            this.labelPictureInfo.ForeColor = System.Drawing.SystemColors.InfoText;
            this.labelPictureInfo.Name = "labelPictureInfo";
            // 
            // webBrowserTop
            // 
            resources.ApplyResources(this.webBrowserTop, "webBrowserTop");
            this.webBrowserTop.AllowWebBrowserDrop = false;
            this.webBrowserTop.Name = "webBrowserTop";
            this.webBrowserTop.ScrollBarsEnabled = false;
            this.webBrowserTop.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowserTop_DocumentCompleted);
            // 
            // imageListView
            // 
            resources.ApplyResources(this.imageListView, "imageListView");
            this.imageListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.imageListView.Colors = new Manina.Windows.Forms.ImageListViewColor(resources.GetString("imageListView.Colors"));
            this.imageListView.ColumnHeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.imageListView.DefaultImage = ((System.Drawing.Image)(resources.GetObject("imageListView.DefaultImage")));
            this.imageListView.ErrorImage = ((System.Drawing.Image)(resources.GetObject("imageListView.ErrorImage")));
            this.imageListView.GroupHeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.imageListView.Name = "imageListView";
            this.imageListView.ThumbnailSize = new System.Drawing.Size(120, 120);
            this.imageListView.ItemClick += new Manina.Windows.Forms.ItemClickEventHandler(this.imageListView_ItemClick);
            // 
            // pictureBoxRemote
            // 
            resources.ApplyResources(this.pictureBoxRemote, "pictureBoxRemote");
            this.pictureBoxRemote.Name = "pictureBoxRemote";
            this.pictureBoxRemote.TabStop = false;
            this.pictureBoxRemote.LoadCompleted += new System.ComponentModel.AsyncCompletedEventHandler(this.pictureBoxRemote_LoadCompleted);
            // 
            // Photo_DV
            // 
            resources.ApplyResources(this, "$this");
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(242)))), ((int)(((byte)(238)))));
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.panelMain);
            this.Name = "Photo_DV";
            this.Resize += new System.EventHandler(this.DetailView_Resize);
            this.panelMain.ResumeLayout(false);
            this.panelRight.ResumeLayout(false);
            this.panelRight.PerformLayout();
            this.PanelAddComment.ResumeLayout(false);
            this.PanelAddComment.PerformLayout();
            this.PanelPictures.ResumeLayout(false);
            this.panelPictureInfo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRemote)).EndInit();
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

            ReLayout();
        }

        private void ReLayout()
        {
            if (Post.attachment_count > 0)
            {
                PanelPictures.Height = imageListView.VScrollBar.Maximum + 16;
            }
            else
            {
                PanelPictures.Height = 0;
            }
        }

        private void Set_Comments_Part()
        {
            MyParent.SetComments(webBrowserComment, Post, true);
        }

        private void Set_MainContent_Part()
        {
            if(Post.content == string.Empty)
            {
                webBrowserTop.Visible = false;
                return;
            }

            StringBuilder _sb = new StringBuilder();

            _sb.Append("<font face='·L³n¥¿¶ÂÅé, Helvetica, Arial, Verdana, sans-serif'><p>[Text]</p></font>");

            string _html = _sb.ToString();
            _html = _html.Replace("[Text]", Post.content.Replace(Environment.NewLine, "<BR>"));

            webBrowserTop.DocumentText = "<body bgcolor=\"rgb(238,231,209)\">" + _html + "</body>";
        }

        private void webBrowserTop_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            Application.DoEvents();

            int _h = webBrowserTop.Document.Body.ScrollRectangle.Height;
            webBrowserTop.Height = _h;
        }

        private void webBrowserComment_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            Application.DoEvents();

            int _h = webBrowserComment.Document.Body.ScrollRectangle.Height;
            webBrowserComment.Height = _h;
        }

        private void Set_Pictures()
        {
            imageListView.Items.Clear();

            m_imageAttachments = new List<Attachment>();

            foreach (Attachment _a in Post.attachments)
            {
                if (_a.type == "image")
                    m_imageAttachments.Add(_a);
            }

            if (m_imageAttachments.Count == 0)
                return;

            imageFileIndex = 0;
            m_filesMapping.Clear();

            if (Post.attachment_count > 0)
            {
                panelPictureInfo.Visible = true;

                DownloadRemoteFile("origin");
            }
        }

        #region File Download [Picture]

        private void DownloadRemoteFile(string imageType)
        {
            labelPictureInfo.Text = "[" + imageFileIndex + "/" + m_imageAttachments.Count + "]";

            Attachment _attachment = m_imageAttachments[imageFileIndex];
            string _url = string.Empty;
            string _fileName = string.Empty;
            Main.Current.RT.REST.attachments_getRedirectURL_Image(_attachment, imageType, out _url, out _fileName); //origin medium

            string _localFile = Main.GCONST.CachePath + _fileName;

            if (!m_filesMapping.ContainsKey(_fileName))
            {
                if (_attachment.file_name != string.Empty)
                    m_filesMapping.Add(_fileName, _attachment.file_name);
            }

            if (System.IO.File.Exists(_localFile))
            {
                imageListView.Items.Add(_localFile);

                //Application.DoEvents();

                DownloadRemoteFileNext();
            }
            else
            {
                pictureBoxRemote.LoadAsync(_url);
                m_downloadFileName = _fileName;
            }
        }

        /*
        private void pictureBoxRemote_LoadProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            labelPictureInfo.Text = e.ProgressPercentage + "%" + " [" + imageFileIndex + "/" + m_imageAttachments.Count + "]";
        }
        */

        private void pictureBoxRemote_LoadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                //Application.DoEvents();

                DownloadRemoteFile("medium");
            }
            else
            {
                try
                {
                    string _localFile = Main.GCONST.CachePath + m_downloadFileName;

                    pictureBoxRemote.Image.Save(_localFile);

                    imageListView.Items.Add(_localFile);

                    //Application.DoEvents();

                    DownloadRemoteFileNext();

                    PanelPictures.Height = imageListView.VScrollBar.Maximum + 16;
                }
                catch (Exception _e)
                {
                    NLogUtility.Exception(s_logger, _e, "pictureBoxRemote_LoadCompleted");
                }
            }
        }

        private void DownloadRemoteFileNext()
        {
            imageFileIndex++;

            labelPictureInfo.Text = "[" + imageFileIndex + "/" + m_imageAttachments.Count + "]";

            Application.DoEvents();

            if (imageFileIndex < m_imageAttachments.Count)
            {
                DownloadRemoteFile("origin");
            }
            else
            {
                panelPictureInfo.Visible = false;

                ReLayout();
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

            m_photoView = new PhotoView(_files, m_filesMapping, e.Item.FileName);
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

        #region IDetailViewer

        public void ScrollToComment()
        {
            if (panelRight.VerticalScroll.Visible)
            {
                panelRight.VerticalScroll.Value = PanelAddComment.Top;
                textBoxComment.Focus();
            }
        }

        public bool WantToShowCommentButton()
        {
            if (panelRight.VerticalScroll.Visible)
            {
                return PanelAddComment.Bottom > panelRight.Height;
            }

            return false;
        }

        #endregion
    }
}