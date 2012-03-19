#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;
using System.Web;
using System.Windows.Forms;
using Manina.Windows.Forms;
using NLog;
using Waveface.API.V2;
using Waveface.Component;
using Waveface.Component;

#endregion

namespace Waveface.DetailUI
{
    public class Photo_DV : UserControl
    {
        private static Logger s_logger = LogManager.GetCurrentClassLogger();

        #region Fields

        private Panel panelMain;
        private AutoScrollPanel panelRight;
        private WebBrowser webBrowserTop;
        private ImageListView imageListView;
        private Post m_post;
        private Panel panelPictureInfo;
        private Label labelPictureInfo;
        private Dictionary<string, string> m_filesMapping;
        private IContainer components;
        private Timer timer;

        private List<string> m_filePathOrigins;
        private List<string> m_filePathMediums;

        private List<Attachment> m_imageAttachments;
        private Localization.CultureManager cultureManager;
        private ContextMenuStrip contextMenuStripTop;
        private ToolStripMenuItem miCopyTop;

        private int m_loadingPhotosCount;

        private WebBrowserContextMenuHandler m_topBrowserContextMenuHandler;

        private List<string> m_clickableURL;

        #endregion

        #region Properties

        public Post Post
        {
            get { return m_post; }
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
            imageListView.CacheMode = CacheMode.Continuous;

            //imageListView.AutoRotateThumbnails = false;
            imageListView.UseEmbeddedThumbnails = UseEmbeddedThumbnails.Never;

            m_filesMapping = new Dictionary<string, string>();
            m_clickableURL = new List<string>();
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Photo_DV));
            this.panelMain = new System.Windows.Forms.Panel();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.cultureManager = new Waveface.Localization.CultureManager(this.components);
            this.contextMenuStripTop = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miCopyTop = new System.Windows.Forms.ToolStripMenuItem();
            this.panelRight = new Waveface.Component.AutoScrollPanel();
            this.imageListView = new Manina.Windows.Forms.ImageListView();
            this.panelPictureInfo = new System.Windows.Forms.Panel();
            this.labelPictureInfo = new System.Windows.Forms.Label();
            this.webBrowserTop = new System.Windows.Forms.WebBrowser();
            this.panelMain.SuspendLayout();
            this.contextMenuStripTop.SuspendLayout();
            this.panelRight.SuspendLayout();
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
            // timer
            // 
            this.timer.Interval = 3000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // cultureManager
            // 
            this.cultureManager.ManagedControl = this;
            // 
            // contextMenuStripTop
            // 
            this.contextMenuStripTop.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miCopyTop});
            this.contextMenuStripTop.Name = "contextMenuStripTop";
            resources.ApplyResources(this.contextMenuStripTop, "contextMenuStripTop");
            // 
            // miCopyTop
            // 
            this.miCopyTop.Name = "miCopyTop";
            resources.ApplyResources(this.miCopyTop, "miCopyTop");
            // 
            // panelRight
            // 
            resources.ApplyResources(this.panelRight, "panelRight");
            this.panelRight.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(242)))), ((int)(((byte)(238)))));
            this.panelRight.Controls.Add(this.imageListView);
            this.panelRight.Controls.Add(this.panelPictureInfo);
            this.panelRight.Controls.Add(this.webBrowserTop);
            this.panelRight.Name = "panelRight";
            // 
            // imageListView
            // 
            this.imageListView.AllowDuplicateFileNames = true;
            this.imageListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.imageListView.CacheLimit = "0";
            this.imageListView.ColumnHeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.imageListView.DefaultImage = global::Waveface.Properties.Resources.LoadingImage;
            resources.ApplyResources(this.imageListView, "imageListView");
            this.imageListView.ErrorImage = ((System.Drawing.Image)(resources.GetObject("imageListView.ErrorImage")));
            this.imageListView.GroupHeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.imageListView.Name = "imageListView";
            this.imageListView.ThumbnailSize = new System.Drawing.Size(128, 128);
            this.imageListView.ItemClick += new Manina.Windows.Forms.ItemClickEventHandler(this.imageListView_ItemClick);
            this.imageListView.MouseMove += new System.Windows.Forms.MouseEventHandler(this.imageListView_MouseMove);
            this.imageListView.Resize += new System.EventHandler(this.imageListView_Resize);
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
            // Photo_DV
            // 
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.panelMain);
            resources.ApplyResources(this, "$this");
            this.Name = "Photo_DV";
            this.Resize += new System.EventHandler(this.DetailView_Resize);
            this.panelMain.ResumeLayout(false);
            this.contextMenuStripTop.ResumeLayout(false);
            this.panelRight.ResumeLayout(false);
            this.panelPictureInfo.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        #region Event Handlers

        protected override void OnPaint(PaintEventArgs e)
        {
            LinearGradientBrush _brush = new LinearGradientBrush(ClientRectangle, Color.FromArgb(106, 112, 128),
                                                                 Color.FromArgb(138, 146, 166),
                                                                 LinearGradientMode.ForwardDiagonal);

            e.Graphics.FillRectangle(_brush, ClientRectangle);

            base.OnPaint(e);
        }

        #endregion

        private void RefreshUI()
        {
            Set_MainContent_Part();
            Set_Pictures();

            ReLayout();
        }

        private void ReLayout()
        {
            if (Post.attachment_count > 0)
            {
                imageListView.Height = imageListView.VScrollBar.Maximum + 16;
            }
            else
            {
                imageListView.Height = 0;
            }
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

            string _content = HttpUtility.HtmlEncode(Post.content);
            _content = _content.Replace(Environment.NewLine, "<BR>");
            _content = _content.Replace("\n", "<BR>");
            _content = _content.Replace("\r", "<BR>");

            _html = _html.Replace("[Text]", _content);

            _html += MyParent.GenCommentHTML(Post);

            _html = HtmlUtility.MakeLink(_html, m_clickableURL);

            _html = "<body bgcolor=\"rgb(243, 242, 238)\">" + _html + "</body>";

            webBrowserTop.DocumentText = HtmlUtility.TrimScript(_html);
        }

        private void webBrowserTop_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            int _h = webBrowserTop.Document.Body.ScrollRectangle.Height;
            webBrowserTop.Height = _h;

            m_topBrowserContextMenuHandler = new WebBrowserContextMenuHandler(webBrowserTop, miCopyTop);
            contextMenuStripTop.Opening += contextMenuStripTop_Opening;
            miCopyTop.Click += m_topBrowserContextMenuHandler.CopyCtxMenuClickHandler;
            webBrowserTop.Document.ContextMenuShowing += webBrowserTop_ContextMenuShowing;
        }

        private void Set_Pictures()
        {
            PhotoDownloader.PreloadPictures(m_post, true);

            imageListView.Items.Clear();

            m_filePathOrigins = new List<string>();
            m_filePathMediums = new List<string>();

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
                Main.Current.RT.REST.attachments_getRedirectURL_Image(_attachment, "origin", out _urlO, out _fileNameO, false);

                string _localFileO = Main.GCONST.CachePath + _fileNameO;

                m_filePathOrigins.Add(_localFileO);

                if (!m_filesMapping.ContainsKey(_fileNameO))
                {
                    if ((_attachment.file_name != string.Empty) && (!_attachment.file_name.Contains("?")))
                        m_filesMapping.Add(_fileNameO, _attachment.file_name);
                }

                string _urlM = string.Empty;
                string _fileNameM = string.Empty;
                Main.Current.RT.REST.attachments_getRedirectURL_Image(_attachment, "medium", out _urlM, out _fileNameM, false);

                string _localFileM = Main.GCONST.CachePath + _fileNameM;

                m_filePathMediums.Add(_localFileM);

                if (!m_filesMapping.ContainsKey(_fileNameM))
                {
                    if ((_attachment.file_name != string.Empty) && (!_attachment.file_name.Contains("?")))
                        m_filesMapping.Add(_fileNameM, _attachment.file_name);
                }
            }

            timer.Interval = ((m_imageAttachments.Count / 200) + 3) * 1000;

            if (!FillImageListView(true))
                timer.Enabled = true;
        }


        private void timer_Tick(object sender, EventArgs e)
        {
            FillImageListView(false);
        }

        private bool FillImageListView(bool firstTime)
        {
            int _count = 0;

            for (int i = 0; i < m_imageAttachments.Count; i++)
            {
                if (File.Exists(m_filePathMediums[i]))
                {
                    _count++;
                }
            }

            bool _show = (m_loadingPhotosCount != m_imageAttachments.Count);

            m_loadingPhotosCount = _count;

            if (firstTime || _show)
            {
                ShowImageListView(firstTime);
                return false;
            }

            if (_count == m_imageAttachments.Count)
            {
                timer.Enabled = false;

                ShowImageListView(firstTime);
                return true;
            }

            return false;
        }

        private bool ShowImageListView(bool firstTime)
        {
            int k = 0;

            panelPictureInfo.Visible = true;

            imageListView.SuspendLayout();

            for (int i = 0; i < m_imageAttachments.Count; i++)
            {
                if (File.Exists(m_filePathMediums[i]))
                {
                    if (firstTime)
                        imageListView.Items.Add(m_filePathMediums[i]);
                    else
                        imageListView.Items[i].FileName = m_filePathMediums[i];

                    imageListView.Items[i].Tag = i.ToString();

                    k++;

                    continue;
                }

                if (firstTime)
                    imageListView.Items.Add(Main.Current.LoadingImagePath);
                else
                    imageListView.Items[i].FileName = Main.Current.LoadingImagePath;

                imageListView.Items[i].Tag = i.ToString();
            }

            imageListView.ResumeLayout();

            labelPictureInfo.Text = "[" + k + "/" + m_imageAttachments.Count + "]";

            if (k == m_imageAttachments.Count)
                panelPictureInfo.Visible = false;

            ReLayout();

            bool _flag = (k == m_imageAttachments.Count);

            if (_flag)
                MyParent.CanEdit = true;

            return _flag;
        }

        private void imageListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            PhotoDownloader.PreloadPictures(m_post, true);

            List<string> _files = new List<string>();

            foreach (var _file in imageListView.Items)
            {
                _files.Add(_file.FileName);
            }

            using (PhotoView _photoView = new PhotoView(m_post, m_imageAttachments, m_filePathOrigins, m_filePathMediums, m_filesMapping, int.Parse(e.Item.Tag.ToString())))
            {
                _photoView.ShowDialog();
            }
        }

        private void DetailView_Resize(object sender, EventArgs e)
        {
            imageListView.Height = imageListView.VScrollBar.Maximum + 16;
        }

        #region ContextMenu

        void contextMenuStripTop_Opening(object sender, CancelEventArgs e)
        {
            m_topBrowserContextMenuHandler.UpdateButtons();
        }

        void webBrowserTop_ContextMenuShowing(object sender, HtmlElementEventArgs e)
        {
            contextMenuStripTop.Show(webBrowserTop.PointToScreen(e.MousePosition));
            e.ReturnValue = false;
        }

        #endregion

        private void imageListView_Resize(object sender, EventArgs e)
        {
            if (Post != null)
                ReLayout();

            if ((webBrowserTop.Document != null) && (webBrowserTop.Document.Body != null))
                webBrowserTop.Height = webBrowserTop.Document.Body.ScrollRectangle.Height;

            if (imageListView.Width > 768)
            {
                int _w = (int)(imageListView.Width / 6.5);
                imageListView.ThumbnailSize = new Size(_w, _w);
            }
            else
            {
                imageListView.ThumbnailSize = new Size(128, 128);
            }
        }

        private void imageListView_MouseMove(object sender, MouseEventArgs e)
        {
            ImageListView.HitInfo _hitInfo;

            imageListView.HitTest(e.Location, out _hitInfo);

            if(_hitInfo.ItemIndex != -1)
            {
                //MessageBox.Show(_hitInfo.ItemIndex.ToString());
            }
        }
    }
}