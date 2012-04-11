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

#endregion

namespace Waveface.DetailUI
{
    public class Photo_DV : UserControl, IDetailView
    {
        private static Logger s_logger = LogManager.GetCurrentClassLogger();

        private Color BG_COLOR = Color.FromArgb(240, 240, 240);

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
        private ContextMenuStrip contextMenuStripImageList;
        private ToolStripMenuItem miSetCoverImage;

        private bool m_canEdit;
        private ImageListViewItem m_selectedItem;

        #endregion

        #region Properties

        public Post Post
        {
            get { return m_post; }
            set
            {
                m_post = value;

                if (m_post != null)
                {
                    m_canEdit = false;

                    RefreshUI();
                }
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

            MyImageListViewRenderer _imageListViewRenderer = new MyImageListViewRenderer
                                                                 {
                                                                     Clip = false,
                                                                     ItemBorderless = true,
                                                                     ShowHovered = false
                                                                 };

            imageListView.SetRenderer(_imageListViewRenderer);

            imageListView.BackColor = BG_COLOR;
            imageListView.Colors.BackColor = BG_COLOR;
            imageListView.Colors.DisabledBackColor = BG_COLOR;
            imageListView.ThumbnailSize = new Size(128, 128);
            imageListView.CacheMode = CacheMode.Continuous;

            //imageListView.AutoRotateThumbnails = false;
            imageListView.UseEmbeddedThumbnails = UseEmbeddedThumbnails.Never;

            m_filesMapping = new Dictionary<string, string>();
            m_clickableURL = new List<string>();

            Visible = false;
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
            this.panelRight = new Waveface.Component.AutoScrollPanel();
            this.imageListView = new Manina.Windows.Forms.ImageListView();
            this.contextMenuStripImageList = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miSetCoverImage = new System.Windows.Forms.ToolStripMenuItem();
            this.panelPictureInfo = new System.Windows.Forms.Panel();
            this.labelPictureInfo = new System.Windows.Forms.Label();
            this.webBrowserTop = new System.Windows.Forms.WebBrowser();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.cultureManager = new Waveface.Localization.CultureManager(this.components);
            this.contextMenuStripTop = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miCopyTop = new System.Windows.Forms.ToolStripMenuItem();
            this.panelMain.SuspendLayout();
            this.panelRight.SuspendLayout();
            this.contextMenuStripImageList.SuspendLayout();
            this.panelPictureInfo.SuspendLayout();
            this.contextMenuStripTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelMain
            // 
            resources.ApplyResources(this.panelMain, "panelMain");
            this.panelMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.panelMain.Controls.Add(this.panelRight);
            this.panelMain.Name = "panelMain";
            // 
            // panelRight
            // 
            resources.ApplyResources(this.panelRight, "panelRight");
            this.panelRight.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.panelRight.Controls.Add(this.imageListView);
            this.panelRight.Controls.Add(this.panelPictureInfo);
            this.panelRight.Controls.Add(this.webBrowserTop);
            this.panelRight.Name = "panelRight";
            // 
            // imageListView
            // 
            resources.ApplyResources(this.imageListView, "imageListView");
            this.imageListView.AllowDuplicateFileNames = true;
            this.imageListView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.imageListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.imageListView.CacheLimit = "0";
            this.imageListView.Colors = new Manina.Windows.Forms.ImageListViewColor(resources.GetString("imageListView.Colors"));
            this.imageListView.ColumnHeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.imageListView.ContextMenuStrip = this.contextMenuStripImageList;
            this.imageListView.DefaultImage = ((System.Drawing.Image)(resources.GetObject("imageListView.DefaultImage")));
            this.imageListView.ErrorImage = ((System.Drawing.Image)(resources.GetObject("imageListView.ErrorImage")));
            this.imageListView.GroupHeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.imageListView.Name = "imageListView";
            this.imageListView.ThumbnailSize = new System.Drawing.Size(128, 128);
            this.imageListView.ItemClick += new Manina.Windows.Forms.ItemClickEventHandler(this.imageListView_ItemClick);
            this.imageListView.ItemDoubleClick += new Manina.Windows.Forms.ItemDoubleClickEventHandler(this.imageListView_ItemDoubleClick);
            this.imageListView.MouseMove += new System.Windows.Forms.MouseEventHandler(this.imageListView_MouseMove);
            this.imageListView.Resize += new System.EventHandler(this.imageListView_Resize);
            // 
            // contextMenuStripImageList
            // 
            resources.ApplyResources(this.contextMenuStripImageList, "contextMenuStripImageList");
            this.contextMenuStripImageList.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miSetCoverImage});
            this.contextMenuStripImageList.Name = "contextMenuStripImageList";
            this.contextMenuStripImageList.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripImageList_Opening);
            // 
            // miSetCoverImage
            // 
            resources.ApplyResources(this.miSetCoverImage, "miSetCoverImage");
            this.miSetCoverImage.Name = "miSetCoverImage";
            this.miSetCoverImage.Click += new System.EventHandler(this.miSetCoverImage_Click);
            // 
            // panelPictureInfo
            // 
            resources.ApplyResources(this.panelPictureInfo, "panelPictureInfo");
            this.panelPictureInfo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(242)))), ((int)(((byte)(238)))));
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
            resources.ApplyResources(this.contextMenuStripTop, "contextMenuStripTop");
            this.contextMenuStripTop.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miCopyTop});
            this.contextMenuStripTop.Name = "contextMenuStripTop";
            // 
            // miCopyTop
            // 
            resources.ApplyResources(this.miCopyTop, "miCopyTop");
            this.miCopyTop.Name = "miCopyTop";
            // 
            // Photo_DV
            // 
            resources.ApplyResources(this, "$this");
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.panelMain);
            this.Name = "Photo_DV";
            this.Resize += new System.EventHandler(this.DetailView_Resize);
            this.panelMain.ResumeLayout(false);
            this.panelRight.ResumeLayout(false);
            this.contextMenuStripImageList.ResumeLayout(false);
            this.panelPictureInfo.ResumeLayout(false);
            this.contextMenuStripTop.ResumeLayout(false);
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

        public bool CanEdit()
        {
            return m_canEdit;
        }

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
            StringBuilder _sb = new StringBuilder();

            _sb.Append("<font face='·L³n¥¿¶ÂÅé, Helvetica, Arial, Verdana, sans-serif'><p>[Text]</p></font>");

            string _html = _sb.ToString();

            string _content = HttpUtility.HtmlEncode(Post.content);
            _content = _content.Replace(Environment.NewLine, "<BR>");
            _content = _content.Replace("\n", "<BR>");
            _content = _content.Replace("\r", "<BR>");

            _html = _html.Replace("[Text]", _content);

            _html += MyParent.GenCommentHTML(Post, false);

            _html = HtmlUtility.MakeLink(_html, m_clickableURL);

            _html = "<body bgcolor=\"rgb(240,240, 240)\">" + _html + "</body>";

            webBrowserTop.DocumentText = HtmlUtility.TrimScript(_html);
        }

        private void webBrowserTop_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            Visible = true;

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

                string _localFileO = Path.Combine( Main.GCONST.CachePath , _fileNameO);

                m_filePathOrigins.Add(_localFileO);

                if (!m_filesMapping.ContainsKey(_fileNameO))
                {
                    if ((_attachment.file_name != string.Empty) && (!_attachment.file_name.Contains("?")))
                        m_filesMapping.Add(_fileNameO, _attachment.file_name);
                }

                string _urlM = string.Empty;
                string _fileNameM = string.Empty;
                Main.Current.RT.REST.attachments_getRedirectURL_Image(_attachment, "medium", out _urlM, out _fileNameM, false);

                string _localFileM = Path.Combine(Main.GCONST.CachePath ,_fileNameM);

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

            string _cover_attach = m_post.cover_attach;

            if(string.IsNullOrEmpty(m_post.cover_attach))
                _cover_attach = m_imageAttachments[0].object_id;

            for (int i = 0; i < m_imageAttachments.Count; i++)
            {
                DetailViewImageListViewItemTag _tag = new DetailViewImageListViewItemTag();
                _tag.Index = i.ToString();
                _tag.IsCoverImage = (_cover_attach == m_imageAttachments[i].object_id);

                if (File.Exists(m_filePathMediums[i]))
                {
                    if (firstTime)
                        imageListView.Items.Add(m_filePathMediums[i]);
                    else
                        imageListView.Items[i].FileName = m_filePathMediums[i];

                    imageListView.Items[i].Tag = _tag;

                    k++;

                    continue;
                }

                if (firstTime)
                    imageListView.Items.Add(Main.Current.LoadingImagePath);
                else
                    imageListView.Items[i].FileName = Main.Current.LoadingImagePath;

                imageListView.Items[i].Tag = _tag;
            }

            imageListView.ResumeLayout();

            labelPictureInfo.Text = "[" + k + "/" + m_imageAttachments.Count + "]";

            if (k == m_imageAttachments.Count)
                panelPictureInfo.Visible = false;

            ReLayout();

            bool _flag = (k == m_imageAttachments.Count);

            if (_flag)
                m_canEdit = true;

            return _flag;
        }

        private void ShowPhotoView(ItemClickEventArgs e)
        {
            PhotoDownloader.PreloadPictures(m_post, true);

            List<string> _files = new List<string>();

            foreach (var _file in imageListView.Items)
            {
                _files.Add(_file.FileName);
            }

            using (
                PhotoView _photoView = new PhotoView(m_post, m_imageAttachments, m_filePathOrigins, m_filePathMediums,
                                                     m_filesMapping, int.Parse(((DetailViewImageListViewItemTag)(e.Item.Tag)).Index)))
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

            try
            {
                if ((webBrowserTop.Document != null) && (webBrowserTop.Document.Body != null))
                    webBrowserTop.Height = webBrowserTop.Document.Body.ScrollRectangle.Height;
            }
            catch
            {
            }

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

            if (_hitInfo.ItemIndex != -1)
            {
                //MessageBox.Show(_hitInfo.ItemIndex.ToString());
            }
        }

        private void imageListView_ItemDoubleClick(object sender, ItemClickEventArgs e)
        {
            ShowPhotoView(e);
        }

        private void miSetCoverImage_Click(object sender, EventArgs e)
        {
            if (m_selectedItem != null)
            {
                Cursor = Cursors.WaitCursor;

                try
                {
                    string _cover_attach = Post.attachments[int.Parse(((DetailViewImageListViewItemTag)(m_selectedItem.Tag)).Index)].object_id;

                    if (_cover_attach != m_post.cover_attach)
                    {
                        Dictionary<string, string> _params = new Dictionary<string, string>();
                        _params.Add("cover_attach", _cover_attach);

                        Post _retPost = Main.Current.PostUpdate(m_post, _params, true);

                        if (_retPost == null)
                        {
                            return;
                        }

                        m_post = _retPost;
                    }

                    MessageBox.Show(I18n.L.T("ChangedCoverImageOK"), "Waveface", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch
                {
                }

                Cursor = Cursors.Default;
            }
        }

        private void contextMenuStripImageList_Opening(object sender, CancelEventArgs e)
        {
            e.Cancel = !m_canEdit;
        }

        private void imageListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            m_selectedItem = e.Item;
        }

        public List<ToolStripMenuItem> GetMoreMenuItems()
        {
            return null;
        }
    }
}