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
        private XPButton buttonAddComment;
        private TextBox textBoxComment;
        private Panel panelPictureInfo;
        private Label labelPictureInfo;
        private Dictionary<string, string> m_filesMapping;
        private IContainer components;
        private Timer timer;

        private List<string> m_filePathOrigins;
        private List<string> m_filePathMediums;
        private List<string> m_urlOrigins;
        private List<string> m_urlMediums;
        private int m_displayCount;

        private List<Attachment> m_imageAttachments;

        #endregion

        #region Properties

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

        public ImageListView ImageListView
        {
            get { return imageListView; }
            set { imageListView = value; }
        }

        #endregion

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
            imageListView.CacheMode = CacheMode.OnDemand;

            m_filesMapping = new Dictionary<string, string>();

            m_filePathOrigins = new List<string>();
            m_filePathMediums = new List<string>();
            m_urlOrigins = new List<string>();
            m_urlMediums = new List<string>();
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Photo_DV));
            this.panelMain = new System.Windows.Forms.Panel();
            this.panelRight = new System.Windows.Forms.Panel();
            this.PanelAddComment = new System.Windows.Forms.Panel();
            this.textBoxComment = new System.Windows.Forms.TextBox();
            this.webBrowserComment = new System.Windows.Forms.WebBrowser();
            this.PanelPictures = new System.Windows.Forms.Panel();
            this.imageListView = new Manina.Windows.Forms.ImageListView();
            this.pictureBoxRemote = new System.Windows.Forms.PictureBox();
            this.panelPictureInfo = new System.Windows.Forms.Panel();
            this.labelPictureInfo = new System.Windows.Forms.Label();
            this.webBrowserTop = new System.Windows.Forms.WebBrowser();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.buttonAddComment = new Waveface.Component.XPButton();
            this.panelMain.SuspendLayout();
            this.panelRight.SuspendLayout();
            this.PanelAddComment.SuspendLayout();
            this.PanelPictures.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRemote)).BeginInit();
            this.panelPictureInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelMain
            // 
            this.panelMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(242)))), ((int)(((byte)(238)))));
            this.panelMain.Controls.Add(this.panelRight);
            resources.ApplyResources(this.panelMain, "panelMain");
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
            // textBoxComment
            // 
            resources.ApplyResources(this.textBoxComment, "textBoxComment");
            this.textBoxComment.Name = "textBoxComment";
            // 
            // webBrowserComment
            // 
            this.webBrowserComment.AllowWebBrowserDrop = false;
            resources.ApplyResources(this.webBrowserComment, "webBrowserComment");
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
            // imageListView
            // 
            this.imageListView.AllowDuplicateFileNames = true;
            this.imageListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.imageListView.CacheLimit = "0";
            this.imageListView.Colors = new Manina.Windows.Forms.ImageListViewColor(resources.GetString("imageListView.Colors"));
            this.imageListView.ColumnHeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.imageListView.DefaultImage = global::Waveface.Properties.Resources.LoadingImage;
            resources.ApplyResources(this.imageListView, "imageListView");
            this.imageListView.ErrorImage = ((System.Drawing.Image)(resources.GetObject("imageListView.ErrorImage")));
            this.imageListView.GroupHeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.imageListView.Name = "imageListView";
            this.imageListView.ThumbnailSize = new System.Drawing.Size(128, 128);
            this.imageListView.ItemClick += new Manina.Windows.Forms.ItemClickEventHandler(this.imageListView_ItemClick);
            // 
            // pictureBoxRemote
            // 
            resources.ApplyResources(this.pictureBoxRemote, "pictureBoxRemote");
            this.pictureBoxRemote.Name = "pictureBoxRemote";
            this.pictureBoxRemote.TabStop = false;
            // 
            // panelPictureInfo
            // 
            this.panelPictureInfo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(208)))), ((int)(((byte)(170)))));
            this.panelPictureInfo.Controls.Add(this.labelPictureInfo);
            resources.ApplyResources(this.panelPictureInfo, "panelPictureInfo");
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
            this.webBrowserTop.AllowWebBrowserDrop = false;
            resources.ApplyResources(this.webBrowserTop, "webBrowserTop");
            this.webBrowserTop.Name = "webBrowserTop";
            this.webBrowserTop.ScrollBarsEnabled = false;
            this.webBrowserTop.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowserTop_DocumentCompleted);
            // 
            // timer
            // 
            this.timer.Interval = 2000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // buttonAddComment
            // 
            this.buttonAddComment.AdjustImageLocation = new System.Drawing.Point(0, 0);
            resources.ApplyResources(this.buttonAddComment, "buttonAddComment");
            this.buttonAddComment.BtnShape = Waveface.Component.emunType.BtnShape.Rectangle;
            this.buttonAddComment.BtnStyle = Waveface.Component.emunType.XPStyle.Silver;
            this.buttonAddComment.Name = "buttonAddComment";
            this.buttonAddComment.UseVisualStyleBackColor = true;
            this.buttonAddComment.Click += new System.EventHandler(this.buttonAddComment_Click);
            // 
            // Photo_DV
            // 
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.panelMain);
            resources.ApplyResources(this, "$this");
            this.Name = "Photo_DV";
            this.Resize += new System.EventHandler(this.DetailView_Resize);
            this.panelMain.ResumeLayout(false);
            this.panelRight.ResumeLayout(false);
            this.panelRight.PerformLayout();
            this.PanelAddComment.ResumeLayout(false);
            this.PanelAddComment.PerformLayout();
            this.PanelPictures.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRemote)).EndInit();
            this.panelPictureInfo.ResumeLayout(false);
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
            if (Post.content == string.Empty)
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
                if (_a.type == "image")
                    m_imageAttachments.Add(_a);
            }

            if (m_imageAttachments.Count == 0)
                return;

            m_filesMapping.Clear();

            foreach (Attachment _attachment in m_imageAttachments)
            {
                string _urlO = string.Empty;
                string _fileNameO = string.Empty;
                Main.Current.RT.REST.attachments_getRedirectURL_Image(_attachment, "origin", out _urlO, out _fileNameO);

                string _localFileO = Main.GCONST.CachePath + _fileNameO;

                m_filePathOrigins.Add(_localFileO);
                m_urlOrigins.Add(_urlO);

                if (!m_filesMapping.ContainsKey(_fileNameO))
                {
                    if (_attachment.file_name != string.Empty)
                        m_filesMapping.Add(_fileNameO, _attachment.file_name);
                }

                string _urlM = string.Empty;
                string _fileNameM = string.Empty;
                Main.Current.RT.REST.attachments_getRedirectURL_Image(_attachment, "medium", out _urlM, out _fileNameM);

                string _localFileM = Main.GCONST.CachePath + _fileNameM;

                m_filePathMediums.Add(_localFileM);
                m_urlMediums.Add(_urlM);

                if (!m_filesMapping.ContainsKey(_fileNameM))
                {
                    if (_attachment.file_name != string.Empty)
                        m_filesMapping.Add(_fileNameM, _attachment.file_name);
                }
            }

            for (int i = m_imageAttachments.Count - 1; i >= 0; i--)
            {
                if (!System.IO.File.Exists(m_filePathOrigins[i]) || !System.IO.File.Exists(m_filePathMediums[i]))
                {
                    ImageItem _item = new ImageItem();
                    _item.PostItemType = PostItemType.Origin;
                    _item.OriginPath = m_urlOrigins[i];
                    _item.MediumPath = m_urlMediums[i];
                    _item.LocalFilePath = m_filePathOrigins[i];
                    _item.LocalFilePath2 = m_filePathMediums[i];

                    PhotoDownloader.Current.Add(_item);
                }
            }

            if (!FillImageListView(true))
                timer.Enabled = true;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            FillImageListView(false);
        }

        private bool FillImageListView(bool firstTime)
        {
            int k = 0;

            for (int i = 0; i < m_imageAttachments.Count; i++)
            {
                if (System.IO.File.Exists(m_filePathOrigins[i]))
                {
                    k++;
                    continue;
                }

                if (System.IO.File.Exists(m_filePathMediums[i]))
                {
                    k++;
                    continue;
                }
            }

            if (!firstTime)
            {
                if (k == m_displayCount)
                    return false;
            }

            panelPictureInfo.Visible = true;

            imageListView.SuspendLayout();

            k = 0;

            for (int i = 0; i < m_imageAttachments.Count; i++)
            {
                if (System.IO.File.Exists(m_filePathOrigins[i]))
                {
                    if (firstTime)
                        imageListView.Items.Add(m_filePathOrigins[i]);
                    else
                        imageListView.Items[i].FileName = m_filePathOrigins[i];
                    
                    k++;
                    continue;
                }

                if (System.IO.File.Exists(m_filePathMediums[i]))
                {
                    if (firstTime)
                        imageListView.Items.Add(m_filePathMediums[i]);
                    else
                        imageListView.Items[i].FileName = m_filePathMediums[i];
                    
                    k++;
                    continue;
                }

                if (firstTime)
                    imageListView.Items.Add(Main.GCONST.CachePath + "LoadingImage" + ".jpg");
                else
                    imageListView.Items[i].FileName = Main.GCONST.CachePath + "LoadingImage" + ".jpg";
            }

            m_displayCount = k;

            imageListView.ResumeLayout();

            labelPictureInfo.Text = "[" + m_displayCount + "/" + m_imageAttachments.Count + "]";

            ReLayout();

            if (k == m_imageAttachments.Count)
            {
                timer.Enabled = false;
                panelPictureInfo.Visible = false;
                return true;
            }

            return false;
        }

        private void imageListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            List<string> _files = new List<string>();

            foreach (var _file in imageListView.Items)
            {
                _files.Add(_file.FileName);
            }

            using (PhotoView _photoView =  new PhotoView(_files, m_filesMapping, e.Item.FileName))
            {
                _photoView.ShowDialog();
            }
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