#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.Win32;
using NLog;
using Waveface.API.V2;
using Waveface.Component;
using Timer = System.Windows.Forms.Timer;

#endregion

namespace Waveface
{
    public class PostsList : UserControl
    {
        private static Logger s_logger = LogManager.GetCurrentClassLogger();

        private const int PicHeight = 102; //115
        private const int PicWidth = 102; //115

        private IContainer components;
        private DataGridView dataGridView;
        private int m_clickIndex;
        private DetailView m_detailView;
        private Font m_font;
        private BindingSource m_postBS;
        private DataGridViewTextBoxColumn creatoridDataGridViewTextBoxColumn;
        private Timer timer;
        private List<Post> m_posts;
        private Timer timerDisplayedScrolling;
        private int m_lastRead;
        private Localization.CultureManager cultureManager;
        private bool m_manualRefresh;

        #region Properties

        public int SelectedRow
        {
            set { dataGridView.Rows[value].Selected = true; }
        }

        public DetailView DetailView
        {
            get { return m_detailView; }
            set { m_detailView = value; }
        }

        #endregion

        public PostsList()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);

            InitializeComponent();

            DoubleBufferedX(dataGridView, true);
        }

        public void DoubleBufferedX(DataGridView dgv, bool setting)
        {
            try
            {
                Type dgvType = dgv.GetType();
                PropertyInfo pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
                pi.SetValue(dgv, setting, null);
            }
            catch
            {
            }
        }

        private void PostsList_Load(object sender, EventArgs e)
        {
            SetFont();

            SystemEvents.UserPreferenceChanged += SystemEvents_UserPreferenceChanged;

            MouseWheelRedirector.Attach(dataGridView);

            if (!DesignMode) //Hack
            {
                Main.Current.PhotoDownloader.ThumbnailEvent += Thumbnail_EventHandler;
            }
        }

        public void Thumbnail_EventHandler(ImageItem item)
        {

            RefreshUI();
        }

        private void SystemEvents_UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            SetFont();
        }

        public void SetFilterPosts(List<Post> posts, bool refreshCurrentPost, bool isFirstTimeGetData)
        {
            dataGridView.Enabled = false;

            m_posts = posts;
            m_postBS.DataSource = posts;

            if (m_postBS.Position == m_clickIndex)
                NotifyDetailView();

            if (isFirstTimeGetData)
            {
                m_postBS.Position = 0;
            }
            else
            {
                if (m_clickIndex >= posts.Count)
                    m_postBS.Position = 0;
                else
                    m_postBS.Position = m_clickIndex;
            }

            try
            {
                dataGridView.DataSource = null;
                dataGridView.DataSource = m_postBS;
                dataGridView.Refresh();
            }
            catch (Exception _e)
            {
                Console.WriteLine(_e.Message);
            }

            dataGridView.Enabled = true;
        }

        public void SetPosts(List<Post> posts, int lastRead, bool manualRefresh)
        {
            try
            {
                m_manualRefresh = manualRefresh;

                dataGridView.Enabled = false;

                m_posts = posts;
                m_postBS.DataSource = posts;

                try
                {
                    dataGridView.DataSource = null;
                    dataGridView.DataSource = m_postBS;
                    dataGridView.Refresh();
                }
                catch (Exception _e)
                {
                    NLogUtility.Exception(s_logger, _e, "SetPosts-1");
                }

                dataGridView.Enabled = true;

                DoDisplayedScrolling(lastRead);

                NotifyDetailView();
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "SetPosts-2");
            }
        }

        #region DataGridView

        private Brush m_bgSelectedBrush = new SolidBrush(Color.FromArgb(224, 208, 170));
        private Brush m_bgReadBrush = new SolidBrush(Color.FromArgb(225, 225, 225));
        private Brush m_bgUnReadBrush = new SolidBrush(Color.FromArgb(217, 217, 217));
        private Color m_inforColor1 = Color.White;
        private Color m_inforColor2 = Color.FromArgb(63, 63, 63);
        private Font m_fontPostTime = new Font("Arial", 9);
        private Font m_fontPhotoInfo = new Font("Arial", 8, FontStyle.Bold);
        private Font m_fontText = new Font("Arial", 10);

        private void dataGridView_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            try
            {
                bool _isDrawThumbnail;

                Graphics _g = e.Graphics;

                Post _post = m_postBS[e.RowIndex] as Post;

                bool _selected = ((e.State & DataGridViewElementStates.Selected) == DataGridViewElementStates.Selected);

                //Color _fcolor = (_selected ? e.CellStyle.SelectionForeColor : e.CellStyle.ForeColor);
                //Color _bcolor = (_selected ? e.CellStyle.SelectionBackColor : e.CellStyle.BackColor);

                int _X = e.CellBounds.Left + e.CellStyle.Padding.Left;
                int _Y = e.CellBounds.Top + e.CellStyle.Padding.Top;
                int _W = e.CellBounds.Width - (e.CellStyle.Padding.Left + e.CellStyle.Padding.Right);
                int _H = e.CellBounds.Height - (e.CellStyle.Padding.Top + e.CellStyle.Padding.Bottom);

                Rectangle _cellRect = new Rectangle(_X, _Y, _W, _H);

                // Draw background

                if (_selected)
                {
                    _g.FillRectangle(m_bgSelectedBrush, e.CellBounds);
                }
                else
                {
                    if (Main.Current.RT.CurrentGroupHaveReadPosts.Contains(_post.post_id))
                    {
                        _g.FillRectangle(m_bgReadBrush, e.CellBounds);
                    }
                    else
                    {
                        _g.FillRectangle(m_bgUnReadBrush, e.CellBounds);
                    }
                }

                _g.DrawRectangle(Pens.White, e.CellBounds.X + 1, e.CellBounds.Y + 1, e.CellBounds.Width - 2, e.CellBounds.Height - 2);

                Rectangle _timeRect = DrawPostTime(_g, m_fontPostTime, _cellRect, _post);

                Rectangle _thumbnailRect = new Rectangle(_X + 4, _Y + 8, PicWidth, PicHeight);

                _isDrawThumbnail = DrawThumbnail(_g, _thumbnailRect, _post);

                int _offsetThumbnail_W = (_isDrawThumbnail ? _thumbnailRect.Width : 0);

                switch (_post.type)
                {
                    case "text":
                        Draw_Text_Post(_g, _post, _cellRect, _timeRect.Height, m_fontText);
                        break;

                    case "rtf":
                        Draw_RichText_Post(_g, _post, _cellRect, _timeRect.Height, m_fontText, _thumbnailRect.Width);
                        break;

                    case "image":
                    case "doc":
                        Draw_Photo_Doc_Post(_g, _post, _cellRect, _timeRect.Height, m_fontPhotoInfo, m_fontText, _thumbnailRect.Width, _selected);
                        break;

                    case "link":
                        Draw_Link(_g, _post, _cellRect, _timeRect.Height, m_fontPhotoInfo, _thumbnailRect.Width, _selected);
                        break;
                }
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "dataGridView_CellPainting");

                e.Handled = false;

                return;
            }

            // Let them know we handled it
            e.Handled = true;
        }

        private void Draw_Link(Graphics g, Post post, Rectangle rect, int timeRectHeight, Font fontPhotoInfo, int thumbnailRectWidth, bool selected)
        {
            if (post.preview.thumbnail_url == null)
                thumbnailRectWidth = 0;

            Rectangle _infoRect = DrawPostInfo(g, fontPhotoInfo, rect, post, thumbnailRectWidth, selected);

            Rectangle _rectAll = new Rectangle(rect.X + 8 + thumbnailRectWidth, rect.Y + _infoRect.Height + 12, rect.Width - thumbnailRectWidth - 8, rect.Height - timeRectHeight - _infoRect.Height - 20);

            TextRenderer.DrawText(g, "\"" + post.preview.title + "\"", new Font("Arial", 10, FontStyle.Bold), _rectAll, Color.FromArgb(23, 53, 93),
                 TextFormatFlags.WordBreak | TextFormatFlags.PreserveGraphicsClipping | TextFormatFlags.EndEllipsis);
        }

        private void Draw_Photo_Doc_Post(Graphics g, Post post, Rectangle rect, int timeRectHeight, Font fontPhotoInfo, Font fontText, int thumbnailRectWidth, bool selected)
        {
            Rectangle _infoRect = DrawPostInfo(g, fontPhotoInfo, rect, post, thumbnailRectWidth, selected);

            Rectangle _rectAll = new Rectangle(rect.X + 8 + thumbnailRectWidth, rect.Y + _infoRect.Height + 14, rect.Width - thumbnailRectWidth - 8, rect.Height - timeRectHeight - _infoRect.Height - 20);

            TextRenderer.DrawText(g, post.content, fontText, _rectAll, Color.Black,
                      TextFormatFlags.WordBreak | TextFormatFlags.PreserveGraphicsClipping | TextFormatFlags.EndEllipsis);
        }

        private void Draw_RichText_Post(Graphics g, Post post, Rectangle rect, int timeRectHeight, Font fontText, int thumbnailRectWidth)
        {
            Rectangle _rectAll = new Rectangle(rect.X + 8 + thumbnailRectWidth, rect.Y + 8, rect.Width - thumbnailRectWidth - 8, rect.Height - timeRectHeight - 16);

            TextRenderer.DrawText(g, post.content, fontText, _rectAll, Color.Black,
                      TextFormatFlags.WordBreak | TextFormatFlags.PreserveGraphicsClipping | TextFormatFlags.EndEllipsis);
        }

        private void Draw_Text_Post(Graphics g, Post post, Rectangle rect, int timeRectHeight, Font fontText)
        {
            Rectangle _rectAll = new Rectangle(rect.X + 8, rect.Y + 8, rect.Width - 8, rect.Height - timeRectHeight - 18);

            TextRenderer.DrawText(g, post.content, fontText, _rectAll, Color.Black,
                      TextFormatFlags.WordBreak | TextFormatFlags.PreserveGraphicsClipping | TextFormatFlags.EndEllipsis);
        }

        private Rectangle DrawPostInfo(Graphics g, Font font, Rectangle cellRect, Post post, int thumbnailOffset_X, bool selected)
        {
            string _info = string.Empty;

            switch (post.type)
            {
                case "image":
                    _info = post.attachment_count + " " + ((post.attachment_count > 1) ? I18n.L.T("photos") : I18n.L.T("photo"));
                    break;

                case "doc":
                    _info = post.attachments[0].file_name; //HttpUtility.UrlDecode(post.attachments[0].file_name)
                    break;

                case "link":
                    _info = StringUtility.ExtractDomainNameFromURL(post.preview.url);
                    break;
            }

            Size _sizeInfo = TextRenderer.MeasureText(g, _info, font);
            Rectangle _rect = new Rectangle(cellRect.X + thumbnailOffset_X + 8, cellRect.Y + 7, cellRect.Width - thumbnailOffset_X - 4, _sizeInfo.Height);

            TextRenderer.DrawText(g, _info, font, _rect, (selected ? m_inforColor1 : m_inforColor2),
                                  TextFormatFlags.PreserveGraphicsClipping | TextFormatFlags.EndEllipsis);

            return _rect;
        }

        private Rectangle DrawPostTime(Graphics g, Font font, Rectangle cellRect, Post post)
        {
            string _postTime = post.timestamp;
            _postTime = DateTimeHelp.ISO8601ToDotNet(_postTime, false);
            _postTime = DateTimeHelp.PrettyDate(_postTime);

            Size _sizeTime = TextRenderer.MeasureText(g, _postTime, font) + new Size(2, 2);
            Rectangle _timeRect = new Rectangle(cellRect.X + cellRect.Width - _sizeTime.Width - 2, cellRect.Y + cellRect.Height - _sizeTime.Height - 4, _sizeTime.Width, _sizeTime.Height);

            TextRenderer.DrawText(g, _postTime, font, _timeRect, Color.FromArgb(127, 127, 127),
                                  TextFormatFlags.PreserveGraphicsClipping | TextFormatFlags.EndEllipsis);

            return _timeRect;
        }

        #region Draw Thumbnail

        private bool DrawThumbnail(Graphics g, Rectangle thumbnailRect, Post post)
        {
            switch (post.type)
            {
                case "image":
                    return DrawPhotoThumbnail(g, thumbnailRect, post);

                case "link":
                    return DrawLinkThumbnail(g, thumbnailRect, post);

                case "rtf":
                    return DrawRtfThumbnail(thumbnailRect, g, post);

                case "doc":
                    return DrawDocThumbnail(thumbnailRect, g, post);
            }

            return false;
        }

        private bool DrawDocThumbnail(Rectangle thumbnailRect, Graphics g, Post post)
        {
            try
            {
                Attachment _a = post.attachments[0];

                if (_a.image == string.Empty)
                {
                    g.FillRectangle(new SolidBrush(SystemColors.Info), thumbnailRect);
                    g.DrawRectangle(new Pen(Color.Black), thumbnailRect);
                }
                else
                {
                    string _localPic = Main.GCONST.CachePath + _a.object_id + "_thumbnail" + ".jpg";

                    string _url = _a.image;

                    _url = Main.Current.RT.REST.attachments_getRedirectURL_PdfCoverPage(_url);

                    Bitmap _img = LoadThumbnail(_url, _localPic);

                    DrawResizedThumbnail(thumbnailRect, g, _img);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        private bool DrawRtfThumbnail(Rectangle thumbnailRect, Graphics g, Post post)
        {
            try
            {
                Attachment _a = null;

                foreach (Attachment _attachment in post.attachments)
                {
                    if (_attachment.type == "image")
                    {
                        _a = _attachment;
                        break;
                    }
                }

                DrawResizedThumbnail_2(thumbnailRect, g, _a);
            }
            catch
            {
                return false;
            }

            return true;
        }

        private bool DrawLinkThumbnail(Graphics g, Rectangle thumbnailRect, Post post)
        {
            try
            {
                if (post.preview.thumbnail_url == null)
                {
                    return false;
                }
                else
                {
                    string _url = post.preview.thumbnail_url;

                    string _localPic = Main.GCONST.CachePath + post.post_id + "_previewthumbnail_" + ".jpg";

                    Bitmap _img = LoadThumbnail(_url, _localPic);

                    DrawResizedThumbnail(thumbnailRect, g, _img);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        private bool DrawPhotoThumbnail(Graphics g, Rectangle thumbnailRect, Post post)
        {
            try
            {
                Attachment _a = post.attachments[0];

                DrawResizedThumbnail_2(thumbnailRect, g, _a);
            }
            catch
            {
                return false;
            }

            return true;
        }

        private Bitmap LoadThumbnail(string url, string localPicPath)
        {
            Bitmap _img;

            if (System.IO.File.Exists(localPicPath))
            {
                _img = new Bitmap(localPicPath);
            }
            else
            {
                ImageItem _item = new ImageItem();
                _item.PostItemType = PostItemType.Thumbnail;
                _item.ThumbnailPath = url;
                _item.LocalFilePath_Origin = localPicPath;

                Main.Current.PhotoDownloader.Add(_item, false);


                _img = new Bitmap(PicWidth, PicHeight);
                Graphics _g = Graphics.FromImage(_img);
                _g.FillRectangle(new SolidBrush(Color.WhiteSmoke), new Rectangle(0, 0, PicWidth, PicHeight));
                _g.DrawRectangle(new Pen(Color.Black), new Rectangle(0, 0, PicWidth - 1, PicHeight - 1));
            }

            return _img;
        }

        private void DrawResizedThumbnail_2(Rectangle thumbnailRect, Graphics g, Attachment a)
        {
            string _url = string.Empty;
            string _fileName = string.Empty;
            Main.Current.RT.REST.attachments_getRedirectURL_Image(a, "small", out _url, out _fileName, false);

            string _localPic = Main.GCONST.CachePath + _fileName;

            Bitmap _img = LoadThumbnail(_url, _localPic);

            DrawResizedThumbnail(thumbnailRect, g, _img);
        }

        private static void DrawResizedThumbnail(Rectangle thumbnailRect, Graphics g, Bitmap image)
        {
            int h = thumbnailRect.Height;
            int w = thumbnailRect.Width;
            int x = thumbnailRect.X;
            int y = thumbnailRect.Y;
            int mw = image.Width;
            int mh = image.Height;

            if (image.Width > image.Height)
            {
                int dh = w * mh / mw;

                Rectangle _r = new Rectangle(x, y, w, dh); // y + ((h - dh) / 2)

                g.DrawImage(image, _r, new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
            }
            else
            {
                int dw = h * mw / mh;

                Rectangle _r = new Rectangle(x + ((w - dw) / 2), y, dw, h);

                g.DrawImage(image, _r, new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
            }
        }

        #endregion

        private void dataGridView_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            //if (e.RowIndex == m_postBS.Position)
            //{
            //    e.DrawFocus(e.RowBounds, true);
            //}
            //else
            {
                using (Pen _p = new Pen(Color.LightGray))
                {
                    e.Graphics.DrawRectangle(_p, e.RowBounds);
                }
            }
        }

        private void postBS_PositionChanged(object sender, EventArgs e)
        {
            //if (!Main.Current.CheckNetworkStatus())
            //    return;

            m_clickIndex = m_postBS.Position;

            if (m_clickIndex < 0)
                return;

            Main.Current.PostListClick(m_clickIndex, m_postBS[m_postBS.Position] as Post);

            if (m_clickIndex > -1)
            {
                NotifyDetailView();

                setCalendarDay();
            }

            if (m_clickIndex == (m_postBS.Count - 1))
            {
                Main.Current.FilterReadMorePost();
            }
        }

        private void setCalendarDay()
        {
            Post _post = m_postBS[m_postBS.Position] as Post;

            Main.Current.setCalendarDay(DateTimeHelp.ISO8601ToDateTime(_post.timestamp).Date);
        }

        private void NotifyDetailView()
        {
            Post _post = m_postBS.Current as Post;

            foreach (User _user in Main.Current.RT.AllUsers)
            {
                if (_user.user_id == _post.creator_id)
                {
                    m_detailView.User = _user;
                }
            }

            m_detailView.Post = _post;
        }

        #endregion

        #region Misc

        private void SetFont()
        {
            m_font = SystemFonts.IconTitleFont;

            if (Font != m_font)
            {
                Font = m_font;

                dataGridView.RowTemplate.Height = 120; //140
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (Main.Current != null) //VS.NET_Bug
            {
                if (!Main.Current.RT.REST.IsNetworkAvailable)
                    return;

                RefreshUI();
            }
        }

        public void RefreshUI()
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(
                           delegate
                           {
                               RefreshUI();
                           }
                           ));
            }
            else
            {
                if (dataGridView != null)
                {
                    dataGridView.SuspendLayout();
                    dataGridView.Refresh();
                    dataGridView.ResumeLayout();
                }
            }
        }

        public void ScrollToDay(DateTime date)
        {
            int k = -1;

            for (int i = 0; i < m_posts.Count; i++)
            {
                if (DateTimeHelp.ISO8601ToDateTime(m_posts[i].timestamp).Date == date)
                {
                    k = i;
                    break;
                }
            }

            if (k != -1)
            {
                dataGridView.FirstDisplayedScrollingRowIndex = k;
                m_postBS.Position = k;
            }
        }

        #endregion

        public void DoDisplayedScrolling(int lastRead)
        {
            m_lastRead = lastRead;
            m_postBS.Position = m_lastRead;

            timerDisplayedScrolling.Enabled = true;
        }

        private void timerDisplayedScrolling_Tick(object sender, EventArgs e)
        {
            timerDisplayedScrolling.Enabled = false;

            try
            {
                dataGridView.FirstDisplayedScrollingRowIndex = m_lastRead;

                if (m_manualRefresh)
                {
                    dataGridView.FirstDisplayedScrollingRowIndex = 0;
                }
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "timerDisplayedScrolling_Tick");
            }
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.creatoridDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.m_postBS = new System.Windows.Forms.BindingSource(this.components);
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.timerDisplayedScrolling = new System.Windows.Forms.Timer(this.components);
            this.cultureManager = new Waveface.Localization.CultureManager(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_postBS)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.AllowUserToResizeRows = false;
            this.dataGridView.AutoGenerateColumns = false;
            this.dataGridView.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridView.ColumnHeadersVisible = false;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.creatoridDataGridViewTextBoxColumn});
            this.dataGridView.DataSource = this.m_postBS;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.FormatProvider = new System.Globalization.CultureInfo("en-US");
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.EnableHeadersVisualStyles = false;
            this.dataGridView.Location = new System.Drawing.Point(0, 0);
            this.dataGridView.Margin = new System.Windows.Forms.Padding(0);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.ReadOnly = true;
            this.dataGridView.RowHeadersVisible = false;
            this.dataGridView.RowTemplate.DefaultCellStyle.SelectionBackColor = System.Drawing.SystemColors.InactiveCaption;
            this.dataGridView.RowTemplate.DefaultCellStyle.SelectionForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.dataGridView.RowTemplate.Height = 64;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.Size = new System.Drawing.Size(385, 274);
            this.dataGridView.TabIndex = 0;
            this.dataGridView.VirtualMode = true;
            this.dataGridView.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dataGridView_CellPainting);
            this.dataGridView.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.dataGridView_RowPostPaint);
            // 
            // creatoridDataGridViewTextBoxColumn
            // 
            this.creatoridDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.creatoridDataGridViewTextBoxColumn.DataPropertyName = "creator_id";
            dataGridViewCellStyle1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 0);
            this.creatoridDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle1;
            this.creatoridDataGridViewTextBoxColumn.HeaderText = "";
            this.creatoridDataGridViewTextBoxColumn.Name = "creatoridDataGridViewTextBoxColumn";
            this.creatoridDataGridViewTextBoxColumn.ReadOnly = true;
            this.creatoridDataGridViewTextBoxColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // m_postBS
            // 
            this.m_postBS.DataSource = typeof(Waveface.API.V2.Post);
            this.m_postBS.PositionChanged += new System.EventHandler(this.postBS_PositionChanged);
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Interval = 30000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // timerDisplayedScrolling
            // 
            this.timerDisplayedScrolling.Interval = 500;
            this.timerDisplayedScrolling.Tick += new System.EventHandler(this.timerDisplayedScrolling_Tick);
            // 
            // cultureManager
            // 
            this.cultureManager.ManagedControl = this;
            // 
            // PostsList
            // 
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.dataGridView);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "PostsList";
            this.Size = new System.Drawing.Size(385, 274);
            this.Load += new System.EventHandler(this.PostsList_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_postBS)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
    }
}