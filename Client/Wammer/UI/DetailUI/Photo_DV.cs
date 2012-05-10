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
        private Color BG_COLOR = Color.FromArgb(255, 255, 255);

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
        private ToolStripMenuItem miOpen;
        private ImageButton btnSaveAllPhotos;
        private SaveFileDialog saveFileDialog;
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
            imageListView.ThumbnailSize = new Size(144, 144);
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
            this.btnSaveAllPhotos = new Waveface.Component.ImageButton();
            this.imageListView = new Manina.Windows.Forms.ImageListView();
            this.contextMenuStripImageList = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miSetCoverImage = new System.Windows.Forms.ToolStripMenuItem();
            this.miOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.panelPictureInfo = new System.Windows.Forms.Panel();
            this.labelPictureInfo = new System.Windows.Forms.Label();
            this.webBrowserTop = new System.Windows.Forms.WebBrowser();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.cultureManager = new Waveface.Localization.CultureManager(this.components);
            this.contextMenuStripTop = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miCopyTop = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.panelMain.SuspendLayout();
            this.panelRight.SuspendLayout();
            this.contextMenuStripImageList.SuspendLayout();
            this.panelPictureInfo.SuspendLayout();
            this.contextMenuStripTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelMain
            // 
            this.panelMain.BackColor = System.Drawing.Color.White;
            this.panelMain.Controls.Add(this.panelRight);
            resources.ApplyResources(this.panelMain, "panelMain");
            this.panelMain.Name = "panelMain";
            // 
            // panelRight
            // 
            resources.ApplyResources(this.panelRight, "panelRight");
            this.panelRight.BackColor = System.Drawing.Color.White;
            this.panelRight.Controls.Add(this.btnSaveAllPhotos);
            this.panelRight.Controls.Add(this.imageListView);
            this.panelRight.Controls.Add(this.panelPictureInfo);
            this.panelRight.Controls.Add(this.webBrowserTop);
            this.panelRight.Name = "panelRight";
            // 
            // btnSaveAllPhotos
            // 
            this.btnSaveAllPhotos.CenterAlignImage = false;
            this.btnSaveAllPhotos.Image = global::Waveface.Properties.Resources.FB_saveall;
            this.btnSaveAllPhotos.ImageDisable = global::Waveface.Properties.Resources.FB_saveall_hl;
            this.btnSaveAllPhotos.ImageFront = null;
            this.btnSaveAllPhotos.ImageHover = global::Waveface.Properties.Resources.FB_saveall_hl;
            resources.ApplyResources(this.btnSaveAllPhotos, "btnSaveAllPhotos");
            this.btnSaveAllPhotos.Name = "btnSaveAllPhotos";
            // 
            // imageListView
            // 
            this.imageListView.AllowDuplicateFileNames = true;
            this.imageListView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.imageListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.imageListView.CacheLimit = "0";
            this.imageListView.Colors = new Manina.Windows.Forms.ImageListViewColor(resources.GetString("imageListView.Colors"));
            this.imageListView.ColumnHeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.imageListView.ContextMenuStrip = this.contextMenuStripImageList;
            this.imageListView.DefaultImage = ((System.Drawing.Image)(resources.GetObject("imageListView.DefaultImage")));
            resources.ApplyResources(this.imageListView, "imageListView");
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
            this.contextMenuStripImageList.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miSetCoverImage,
            this.miOpen});
            this.contextMenuStripImageList.Name = "contextMenuStripImageList";
            resources.ApplyResources(this.contextMenuStripImageList, "contextMenuStripImageList");
            this.contextMenuStripImageList.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripImageList_Opening);
            // 
            // miSetCoverImage
            // 
            this.miSetCoverImage.Image = global::Waveface.Properties.Resources.FB_cover;
            this.miSetCoverImage.Name = "miSetCoverImage";
            resources.ApplyResources(this.miSetCoverImage, "miSetCoverImage");
            this.miSetCoverImage.Click += new System.EventHandler(this.miSetCoverImage_Click);
            // 
            // miOpen
            // 
            this.miOpen.Image = global::Waveface.Properties.Resources.FB_openin;
            this.miOpen.Name = "miOpen";
            resources.ApplyResources(this.miOpen, "miOpen");
            this.miOpen.Click += new System.EventHandler(this.miOpen_Click);
            // 
            // panelPictureInfo
            // 
            this.panelPictureInfo.BackColor = System.Drawing.Color.WhiteSmoke;
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
            // Photo_DV
            // 
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.panelMain);
            resources.ApplyResources(this, "$this");
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
            PhotoDownloader.PreloadPictures(m_post, true);

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

            _sb.Append("<font face='微軟正黑體, Helvetica, Arial, Verdana, sans-serif'><p>[Text]</p></font>");

            string _html = _sb.ToString();

            string _content = HttpUtility.HtmlEncode(Post.content);
            _content = _content.Replace(Environment.NewLine, "<BR>");
            _content = _content.Replace("\n", "<BR>");
            _content = _content.Replace("\r", "<BR>");

            _html = _html.Replace("[Text]", _content);

            _html += MyParent.GenCommentHTML(Post, false);

            _html = HtmlUtility.MakeLink(_html, m_clickableURL);

            _html = "<body bgcolor=\"rgb(255, 255, 255)\">" + _html + "</body>";

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

                string _localFileO = Path.Combine(Main.GCONST.ImageCachePath, _fileNameO);

                m_filePathOrigins.Add(_localFileO);

                if (!m_filesMapping.ContainsKey(_fileNameO))
                {
                    if ((!String.IsNullOrEmpty(_attachment.file_name)) && (!_attachment.file_name.Contains("?")))
                        m_filesMapping.Add(_fileNameO, _attachment.file_name);
                }

                string _urlM = string.Empty;
                string _fileNameM = string.Empty;
                Main.Current.RT.REST.attachments_getRedirectURL_Image(_attachment, "medium", out _urlM, out _fileNameM, false);

                string _localFileM = Path.Combine(Main.GCONST.ImageCachePath, _fileNameM);

                m_filePathMediums.Add(_localFileM);

                if (!m_filesMapping.ContainsKey(_fileNameM))
                {
                    if ((!string.IsNullOrEmpty(_attachment.file_name)) && (!_attachment.file_name.Contains("?")))
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

            if (string.IsNullOrEmpty(m_post.cover_attach))
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

        private void ShowPhotoView(int index)
        {
            PhotoDownloader.PreloadPictures(m_post, true);

            List<string> _files = new List<string>();

            foreach (var _file in imageListView.Items)
            {
                _files.Add(_file.FileName);
            }

            using (PhotoView _photoView = new PhotoView(m_post, m_imageAttachments, m_filePathOrigins, m_filePathMediums, m_filesMapping, index))
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

            int W = 144;

            if (Main.Current.Width > 1280)
            {
                int _w = (int)(imageListView.Width / 7.55);

                if (_w < W)
                    _w = W;

                imageListView.ThumbnailSize = new Size(_w, _w);
            }
            else
            {
                imageListView.ThumbnailSize = new Size(W, W);
            }
        }

        private void imageListView_MouseMove(object sender, MouseEventArgs e)
        {
            /*
            ImageListView.HitInfo _hitInfo;

            imageListView.HitTest(e.Location, out _hitInfo);

            if (_hitInfo.ItemIndex != -1)
            {
                //MessageBox.Show(_hitInfo.ItemIndex.ToString());
            }
            */
        }

        private void imageListView_ItemDoubleClick(object sender, ItemClickEventArgs e)
        {
            ShowPhotoView(int.Parse(((DetailViewImageListViewItemTag)(e.Item.Tag)).Index));
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

                    Main.Current.ShowStatuMessage(I18n.L.T("ChangedCoverImageOK"), true);
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

        public ImageButton GetMoreFonction1()
        {
            return btnSaveAllPhotos;
        }

        public void MoreFonction1()
        {
            SaveAllPics();
        }

        private void miOpen_Click(object sender, EventArgs e)
        {
            ShowPhotoView(int.Parse(((DetailViewImageListViewItemTag)(m_selectedItem.Tag)).Index));
        }

        private bool CheckIfLoadingImage(ImageListViewItem imageListViewItem)
        {
            string _trueName = new FileInfo(imageListViewItem.FileName).Name;

            if (m_filesMapping.ContainsKey(_trueName))
                _trueName = m_filesMapping[_trueName];

            return (_trueName == "LoadingImage.jpg");
        }

        public void SaveAllPics()
        {
            string _fileName = string.Empty;

            using (FolderBrowserDialog _dialog = new FolderBrowserDialog())
            {
                _dialog.Description = I18n.L.T("PhotoView.SelectLoc");
                _dialog.ShowNewFolderButton = true;
                _dialog.RootFolder = Environment.SpecialFolder.Desktop;

                if (_dialog.ShowDialog() == DialogResult.OK)
                {
                    string _folder = _dialog.SelectedPath + "\\";

                    foreach (ImageListViewItem _item in imageListView.Items)
                    {
                        if (CheckIfLoadingImage(_item))
                            continue;

                        _fileName = new FileInfo(_item.FileName).Name;

                        if (m_filesMapping.ContainsKey(_fileName))
                            _fileName = m_filesMapping[_fileName]; // 取出真實名稱

                        _fileName = FileUtility.saveFileWithoutOverwrite(_fileName, _folder);

                        try
                        {
                            File.Copy(_item.FileName, _fileName);
                        }
                        catch
                        {
                        }
                    }

                    MessageBox.Show(I18n.L.T("PhotoView.SaveAllOK"), "Waveface Stream", MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                }
            }
        }
    }
}
