#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using NLog;
using Newtonsoft.Json;
using Waveface.API.V2;
using Waveface.Component;
using System.Diagnostics;
using Waveface.Properties;
using System.Threading;
using System.Linq;

#endregion

namespace Waveface
{
    public class PostsList : UserControl
	{

		#region Var
		private Brush _brush;
		private Brush _textBrush;
		private Brush m_bgSelectedBrush = new SolidBrush(Color.FromArgb(255, 255, 255));
		private Brush m_bgUnReadBrush = new SolidBrush(Color.FromArgb(234, 234, 234)); // 217, 217, 217

		private Color m_inforColor = Color.FromArgb(95, 121, 143);
		private Color m_textColor = Color.FromArgb(57, 80, 85);
		private Color m_selectedTextColor = Color.FromArgb(57, 80, 85); //Color.FromArgb(89, 154, 174);
		private Color m_linkURLColor = Color.FromArgb(95, 121, 143);

		private Font m_fontInfo = new Font("Arial", 8, FontStyle.Bold);
		private Font m_fontLinkURL = new Font("Arial", 8, FontStyle.Italic | FontStyle.Bold);
		private Font m_fontLinkTitle = new Font("Arial", 10, FontStyle.Bold);
		private Font m_fontText = new Font("Arial", 10);

		private Dictionary<string, Image> _photoPool;
		#endregion

		#region Private Property
		/// <summary>
		/// Gets or sets the m_ brush.
		/// </summary>
		/// <value>The m_ brush.</value>
		private Brush m_Brush
		{
			get
			{
				return _brush ?? (_brush = new SolidBrush(Color.FromArgb(127, 0, 0, 0)));
			}
			set
			{
				if (_brush == value)
					return;

				if (_brush != null)
					_brush.Dispose();

				_brush = value;
			}
		}

		/// <summary>
		/// Gets or sets the m_ text brush.
		/// </summary>
		/// <value>The m_ text brush.</value>
		private Brush m_TextBrush
		{
			get
			{
				return _textBrush ?? (_textBrush = new SolidBrush(m_textColor));
			}
			set
			{
				if (_textBrush == value)
					return;

				if (_textBrush != null)
					_textBrush.Dispose();

				_textBrush = value;
			}
		}

		/// <summary>
		/// Gets the m_ photo pool.
		/// </summary>
		/// <value>The m_ photo pool.</value>
		private Dictionary<string, Image> m_PhotoPool
		{
			get
			{
				return _photoPool ?? (_photoPool = new Dictionary<string, Image>());
			}
		}
		#endregion

		private static Logger s_logger = LogManager.GetCurrentClassLogger();

        private int PicHeight = 78;
        private int PicWidth = 78;

        private IContainer components;
        private CustomDataGridView dataGridView;
        private int m_clickedIndex;
        private string m_clickedPostID;
        private DetailView m_detailView;
        private BindingSource m_postBS;
        private DataGridViewTextBoxColumn creatoridDataGridViewTextBoxColumn;
        private System.Windows.Forms.Timer timer;
        private List<Post> m_posts;
        private ContextMenuStrip contextMenuStrip;
        private ToolStripMenuItem miRemovePost;

        private int m_oldFirstDisplayedIndex;
        private string m_oldFirstDisplayedPostID;

        private string m_defaultFont;
        private ToolStripMenuItem displayToolStripMenuItem;
        private ToolStripMenuItem miDisplayAll;
        private ToolStripMenuItem miDisplayText;
        private ToolStripMenuItem miDisplayPhoto;
        private ToolStripMenuItem miDisplayWebLink;

        private DragDrop_Clipboard_Helper m_dragDropClipboardHelper;

        private int m_cellHeight;
        private int m_cellLinkHeight;
        private int m_timeBarHeight = 20;
        private int m_cellLinkHackValue;

        private Dictionary<DateTime, string> m_firstPostInADay;

        #region Properties

        public PostArea MyParent { get; set; }

        public int SelectedRow
        {
            set { dataGridView.Rows[value].Selected = true; }
        }

        public DetailView DetailView
        {
            get { return m_detailView; }
            set
            {
                m_detailView = value;

                if (value != null)
                    Main.Current.PhotoDownloader.ThumbnailEvent += Thumbnail_EventHandler;
            }
        }

        #endregion

        public PostsList()
        {
			DebugInfo.ShowMethod();
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);

            InitializeComponent();

            m_dragDropClipboardHelper = new DragDrop_Clipboard_Helper(false);

            DoubleBufferedX(dataGridView, true);

			//dataGridView.MouseWheel += (s, e) =>
			//{
			//    if (m_postBS.Count <= 0)
			//        return;

			//    SetDateText();
			//};
        }

        public void DoubleBufferedX(DataGridView dgv, bool setting)
        {
			DebugInfo.ShowMethod();

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
			DebugInfo.ShowMethod();

            InitFont();

            MouseWheelRedirector.Attach(dataGridView);
        }

        public void RefreshTimelineUI()
        {
			DebugInfo.ShowMethod();

            dataGridView.Refresh();
        }

        private void InitFont()
        {
			DebugInfo.ShowMethod();

            float _d = 0;

			m_defaultFont = Resources.DEFAULT_FONT;

            Font _font = new Font(m_defaultFont, 10, GraphicsUnit.Point);

            if (_font.Height > 17)
                _d = 0.5f;

            m_fontInfo = new Font(m_defaultFont, 8, FontStyle.Bold, GraphicsUnit.Point);
            m_fontLinkURL = new Font(m_defaultFont, 8, FontStyle.Italic | FontStyle.Bold, GraphicsUnit.Point);
            m_fontLinkTitle = new Font(m_defaultFont, 10 - _d, FontStyle.Bold, GraphicsUnit.Point);
            m_fontText = new Font(m_defaultFont, 10 - _d, GraphicsUnit.Point);

            PicHeight = (int)(m_fontText.Height * 4.8);
            PicWidth = (int)(m_fontText.Height * 4.8);

            float _dpi;

            using (Graphics _g = CreateGraphics())
            {
                _dpi = _g.DpiX;
            }

            m_cellHeight = (m_fontText.Height * 7) + 6;
            m_cellLinkHeight = (m_fontText.Height * 7) + 6;
            m_cellLinkHackValue = 0;

            m_timeBarHeight = m_fontText.Height;

            if (_dpi == 120)
            {
                m_cellLinkHackValue = 2;
                m_cellHeight = (int)(m_fontText.Height * 6.9);
                m_cellLinkHeight = m_fontText.Height * 7;

				if (Thread.CurrentThread.CurrentUICulture.Name == "zh-TW")
                {
                    m_cellLinkHeight = (int)(m_fontText.Height * 6.7);
                    m_cellHeight = (int)(m_fontText.Height * 7.3);
                }
            }

            dataGridView.RowTemplate.Height = m_cellHeight;
        }

		//private Boolean NeedUpdatePosts(List<Post> posts)
		//{
		//    DebugInfo.ShowMethod();

		//    if (m_posts == null)
		//        return true;

		//    if (m_posts.Count != posts.Count)
		//        return true;

		//    for (var index = 0; index < m_posts.Count; ++index)
		//    {
		//        if (m_posts[index].post_id != posts[index].post_id)
		//        {
		//            return true;
		//        }
		//    }
		//    return false;
		//}

        public void SetPosts(List<Post> posts, Dictionary<DateTime, string> firstPostInADay)
        {
			DebugInfo.ShowMethod();

			try
			{
				//if (!NeedUpdatePosts(posts))
				//    return;

				m_PhotoPool.Clear(); 
				dataGridView.SuspendLayout();
				GetFirstDisplayed(posts);

				m_firstPostInADay = firstPostInADay;
				m_posts = posts;
				m_postBS.DataSource = posts;

				try
				{
					dataGridView.DataSource = null;
					dataGridView.DataSource = m_postBS;
				}
				catch
				{
				}

				SetDateText();

				if (m_posts.Count == 0)
				{
					m_detailView.ResetUI();
				}
				else
				{
					SetFirstDisplayed(posts);

					NotifyDetailView();
				}
			}
			catch (Exception _e)
			{
			}
			finally
			{
				dataGridView.ResumeLayout();
			}
        }

        private bool IsSamePostContent(Post p1, Post p2)
        {
			DebugInfo.ShowMethod();

            if ((p1 == null) || (p2 == null))
                return false;

            if (p1.post_id != p2.post_id)
                return false;

            if (p1.content != p2.content)
                return false;

            if (p1.favorite != p2.favorite)
                return false;

            if (p1.comment_count != p2.comment_count)
                return false;

            if (p1.type != p2.type)
                return false;

            switch (p1.type)
            {
                case "text":
                    break;

                case "link":
                    string _pv1 = JsonConvert.SerializeObject(p1.preview);
                    string _pv2 = JsonConvert.SerializeObject(p2.preview);

                    if (_pv1 != _pv2)
                        return false;

                    break;

                case "image":
                    if (p1.cover_attach != p2.cover_attach)
                        return false;

                    if (p1.attachment_id_array.Count != p2.attachment_id_array.Count)
                        return false;

                    for (int i = 0; i < p1.attachment_id_array.Count; i++)
                    {
                        if (p1.attachment_id_array[i] != p2.attachment_id_array[i])
                            return false;
                    }

                    break;
            }

            return true;
        }

        private void SetFirstDisplayed(List<Post> posts)
        {
			DebugInfo.ShowMethod();

            int _idxFirst = -1;
            int _idxClicked = -1;

            for (int i = 0; i < posts.Count; i++)
            {
                if (posts[i].post_id == m_oldFirstDisplayedPostID)
                {
                    _idxFirst = i;
                    break;
                }
            }

            for (int i = 0; i < posts.Count; i++)
            {
                if (posts[i].post_id == m_clickedPostID)
                {
                    _idxClicked = i;
                    break;
                }
            }

            if (_idxClicked != -1) // Old Clicked Item Exist
            {
                if (_idxFirst != -1) // Old First Displayed Item Exist
                {
                    dataGridView.FirstDisplayedScrollingRowIndex = _idxFirst;
                }
                else
                {
                    int _d = m_clickedIndex - m_oldFirstDisplayedIndex;

                    if (_d < 0)
                    {
                        dataGridView.FirstDisplayedScrollingRowIndex = _idxClicked;
                    }
                    else
                    {
                        dataGridView.FirstDisplayedScrollingRowIndex = _idxClicked - _d;
                    }
                }
            }
            else
            {
                if (_idxFirst == -1)
                {
                    dataGridView.FirstDisplayedScrollingRowIndex = 0;
                }
                else
                {
                    dataGridView.FirstDisplayedScrollingRowIndex = _idxFirst;
                }
            }
        }

        private void GetFirstDisplayed(List<Post> posts)
        {
			DebugInfo.ShowMethod();

            m_oldFirstDisplayedIndex = dataGridView.FirstDisplayedScrollingRowIndex;

            m_oldFirstDisplayedPostID = "";

            if (m_oldFirstDisplayedIndex >= 0)
            {
                if (m_oldFirstDisplayedIndex < posts.Count)
                {
                    m_oldFirstDisplayedPostID = posts[m_oldFirstDisplayedIndex].post_id;
                }
            }
        }

        #region DataGridView



        private void dataGridView_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
			DebugInfo.ShowMethod();

            try
            {
                bool isDrawThumbnail = false;
                bool isFirstPostInADay = false;
				bool isFirstDisplayed = false;

                Graphics g = e.Graphics;


                Post post = m_postBS[e.RowIndex] as Post;

                DateTime dt = DateTimeHelp.ISO8601ToDateTime(post.timestamp).Date;

                if (m_firstPostInADay.ContainsValue(post.post_id))
                {
                    isFirstPostInADay = true;

					if (dataGridView.FirstDisplayedScrollingRowIndex == e.RowIndex)
					{
						isFirstDisplayed = true;
					}
                }

                bool selected = ((e.State & DataGridViewElementStates.Selected) == DataGridViewElementStates.Selected);


                int _X = e.CellBounds.Left + e.CellStyle.Padding.Left;
                int _Y = e.CellBounds.Top + e.CellStyle.Padding.Top;
                int _W = e.CellBounds.Width - (e.CellStyle.Padding.Left + e.CellStyle.Padding.Right);
                int _H = e.CellBounds.Height - (e.CellStyle.Padding.Top + e.CellStyle.Padding.Bottom);

				if (isFirstPostInADay && !isFirstDisplayed && e.RowIndex > 0)
                {
                    _Y += m_timeBarHeight;
                    _H -= m_timeBarHeight;
                }

                // Draw background
                if (selected)
                {
                    g.FillRectangle(m_bgSelectedBrush, e.CellBounds);
                }
                else
                {
                    g.FillRectangle(m_bgUnReadBrush, e.CellBounds);
                }

                if (isFirstPostInADay && !isFirstDisplayed)
                {
                    DrawTimeBar(e, dt);
                }

                g.DrawRectangle(Pens.LightGray, e.CellBounds.X, e.CellBounds.Y, e.CellBounds.Width, e.CellBounds.Height);

                if (post.type == "link")
                {
                    DrawLink(e, post, selected, _X, _Y, _W, _H);
                }
                else
                {
                    Rectangle _cellRect = new Rectangle(_X, _Y, _W, _H);

					int _underThumbnailHeight = 17;
					int _picWH = _H - _underThumbnailHeight;

                    Rectangle _thumbnailRect = new Rectangle(e.CellBounds.Width - _picWH - 10, _Y + 8, _picWH, _picWH);

                    isDrawThumbnail = DrawThumbnail(g, _thumbnailRect, post);

                    switch (post.type)
                    {
                        case "text":
                            Draw_Text_Post(g, post, _cellRect, _underThumbnailHeight, m_fontText, selected);
                            break;

                        case "rtf":
                            Draw_RichText_Post(g, post, _cellRect, _underThumbnailHeight, m_fontText,
                                               _thumbnailRect.Width);
                            break;

                        case "image":
                        case "doc":
                            Draw_Photo_Doc_Post(g, post, _cellRect, _underThumbnailHeight, _thumbnailRect.Width,
                                                selected, _thumbnailRect.Height);
                            break;
                    }
                }
            }
            catch (Exception _e)
            {
                NLogUtility.Exception(s_logger, _e, "dataGridView_CellPainting");

                e.Handled = false;

                return;
            }

            e.Handled = true;

            //Trace.WriteLine(String.Format("dataGridView_CellPainting elapsed {0} ms", sw.ElapsedMilliseconds.ToString()));
        }

        private void DrawLink(DataGridViewCellPaintingEventArgs e, Post post, bool selected, int X, int Y, int W, int H)
        {
			DebugInfo.ShowMethod();

            Graphics _g = e.Graphics;

            if (!string.IsNullOrEmpty(post.content))
            {
                Rectangle _rTContent = new Rectangle(X + 4, Y + 6, e.CellBounds.Width - 8,
                                                     m_fontText.Height * 2 - m_cellLinkHackValue);

                TextRenderer.DrawText(_g, post.content.Trim(), m_fontText, _rTContent, m_textColor,
									  TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix | TextFormatFlags.WordEllipsis);

                Y += m_fontText.Height * 2;
                H -= m_fontText.Height * 2;
            }

            Rectangle _cellRect = new Rectangle(X, Y, W, H);

            Rectangle _thumbnailRect = new Rectangle(18, Y + 10, PicWidth - 12, PicHeight - 12);

            bool _isDrawThumbnail = DrawLinkThumbnail(_g, _thumbnailRect, post);

            _g.FillRectangle(Brushes.LightGray, 10, Y + 10, 2, PicHeight);

            int _offsetThumbnail_W = (_isDrawThumbnail ? _thumbnailRect.Width + 12 : 12);

            Draw_LinkContent(_g, post, _cellRect, _offsetThumbnail_W, selected);
        }

        private void DrawTimeBar(DataGridViewCellPaintingEventArgs e, DateTime dt)
        {
			DebugInfo.ShowMethod();

            e.Graphics.DrawImage(Properties.Resources.timebar, e.CellBounds.Left, e.CellBounds.Top);

			e.Graphics.DrawString(dt.Date.ToString("yyyy-MM-dd (ddd)"), m_fontText, m_TextBrush,
								  e.CellBounds.Left + 4, e.CellBounds.Top + 2);
        }

        private void Draw_LinkContent(Graphics g, Post post, Rectangle rect, int offsetX, bool selected)
        {
			DebugInfo.ShowMethod();

            Size _sizeTitle = TextRenderer.MeasureText(g, "-- Tg --", m_fontLinkTitle);
            Rectangle _rTitle = new Rectangle(rect.X + offsetX + 4, rect.Y + 8, rect.Width - offsetX - 8,
                                              _sizeTitle.Height);

            if (!string.IsNullOrEmpty(post.preview.title))
            {
                TextRenderer.DrawText(g, post.preview.title.Trim(), m_fontLinkTitle, _rTitle, Color.FromArgb(89, 154, 174),
                                      TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix);
            }

            string _url = StringUtility.ExtractDomainNameFromURL(post.preview.url);
            Size _sizeURL = TextRenderer.MeasureText(g, _url, m_fontLinkURL);
            Rectangle _rURL = new Rectangle(rect.X + offsetX + 4, _rTitle.Bottom + 2, rect.Width - offsetX - 8,
                                            _sizeURL.Height);

            TextRenderer.DrawText(g, _url, m_fontLinkURL, _rURL, m_textColor, TextFormatFlags.EndEllipsis);

            Rectangle _rText = new Rectangle(rect.X + offsetX + 4, _rURL.Bottom + 2, rect.Width - offsetX - 8,
                                             rect.Height - _rTitle.Height - _rURL.Height - 20);

            if (!string.IsNullOrEmpty(post.preview.description))
            {
                TextRenderer.DrawText(g, post.preview.description.Trim(), m_fontText, _rText, m_linkURLColor,
									  TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix | TextFormatFlags.WordEllipsis);
            }
        }

        private void Draw_Photo_Doc_Post(Graphics g, Post post, Rectangle rect, int underThumbnailHeight, int thumbnailRectWidth, bool selected, int thumbnailRectHeight)
        {
			DebugInfo.ShowMethod();

            string _info = post.attachment_id_array.Count + " " +
						   ((post.attachment_id_array.Count > 1) ? Resources.PHOTOS : Resources.PHOTO);

            Size _sizeInfo = TextRenderer.MeasureText(g, _info, m_fontInfo);

            Rectangle _rect = new Rectangle(rect.X + rect.Width - thumbnailRectWidth - 6,
                                            rect.Y + rect.Height - underThumbnailHeight - 6,
                                            thumbnailRectWidth,
                                            _sizeInfo.Height);

			g.FillRectangle(m_Brush, _rect);

            TextRenderer.DrawText(g, _info, m_fontInfo, _rect, Color.WhiteSmoke);

            Rectangle _rectAll = new Rectangle(rect.X + 4, rect.Y + 8, rect.Width - thumbnailRectWidth - 8,
                                               rect.Height - underThumbnailHeight - 18);

            TextRenderer.DrawText(g, post.content, m_fontText, _rectAll, selected ? m_selectedTextColor : m_textColor,
								  TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix | TextFormatFlags.WordEllipsis);
        }

        private void Draw_RichText_Post(Graphics g, Post post, Rectangle rect, int underThumbnailHeight, Font fontText,
                                        int thumbnailRectWidth)
        {
			DebugInfo.ShowMethod();

            Rectangle _rectAll = new Rectangle(rect.X + 4, rect.Y + 8, rect.Width - thumbnailRectWidth - 8,
                                               rect.Height - underThumbnailHeight - 16);

            TextRenderer.DrawText(g, post.content, fontText, _rectAll, m_textColor,
								  TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix | TextFormatFlags.WordEllipsis);
        }

        private void Draw_Text_Post(Graphics g, Post post, Rectangle rect, int underThumbnailHeight, Font fontText,
                                    bool selected)
        {
			DebugInfo.ShowMethod();

            Rectangle _rectAll = new Rectangle(rect.X + 4, rect.Y + 8, rect.Width - 8,
                                               rect.Height - 18);

            TextRenderer.DrawText(g, post.content, fontText, _rectAll, selected ? m_selectedTextColor : m_textColor,
								  TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix | TextFormatFlags.WordEllipsis);
        }

        #region Draw Thumbnail

        private bool DrawThumbnail(Graphics g, Rectangle thumbnailRect, Post post)
        {
			DebugInfo.ShowMethod();

            switch (post.type)
            {
                case "image":
                    return DrawPhotoThumbnail(g, thumbnailRect, post);

                case "rtf":
                    return DrawRtfThumbnail(thumbnailRect, g, post);

                case "doc":
                    return DrawDocThumbnail(thumbnailRect, g, post);
            }

            return false;
        }

        private bool DrawDocThumbnail(Rectangle thumbnailRect, Graphics g, Post post)
        {
			DebugInfo.ShowMethod();

            try
            {
                // -------------------------------------------------------------------------
                // Need to revise later:
                // Temporary remove this because post.attachments is not garuanteed to exist
                // -------------------------------------------------------------------------

                //Attachment _a = post.attachments[0];

                //if (_a.image == string.Empty)
                //{
                //    g.FillRectangle(m_bgSelectedBrush, thumbnailRect);
                //    g.DrawRectangle(new Pen(Color.Black), thumbnailRect);
                //}
                //else
                //{
                //    string _localPic = Path.Combine(Main.GCONST.ImageCachePath, _a.object_id + "_thumbnail" + ".jpg");

                //    string _url = _a.image;

                //    _url = Main.Current.RT.REST.attachments_getRedirectURL_PdfCoverPage(_url);

                //    Bitmap _img = LoadThumbnail(_url, _localPic, false);

                //    DrawResizedThumbnail(thumbnailRect, g, _img);
                //}
            }
            catch
            {
                return false;
            }

            return true;
        }

        private bool DrawRtfThumbnail(Rectangle thumbnailRect, Graphics g, Post post)
        {
			DebugInfo.ShowMethod();

            try
            {
                // -------------------------------------------------------------------------
                // Need to revise later:
                // Temporary remove this because post.attachments is not garuanteed to exist
                // -------------------------------------------------------------------------


                //Attachment _a = null;

                //foreach (Attachment _attachment in post.attachments)
                //{
                //    if (_attachment.type == "image")
                //    {
                //        _a = _attachment;
                //        break;
                //    }
                //}

                //DrawResizedThumbnail_2(thumbnailRect, g, _a);
            }
            catch
            {
                return false;
            }

            return true;
        }

        private bool DrawLinkThumbnail(Graphics g, Rectangle thumbnailRect, Post post)
        {
			DebugInfo.ShowMethod();

            try
            {
                if (string.IsNullOrEmpty(post.preview.thumbnail_url))
                {
                    return false;
                }
                else
                {
                    string _url = post.preview.thumbnail_url;
					var postID = post.post_id;

                    int _hashCode = Math.Abs(post.preview.thumbnail_url.GetHashCode());

                    string _localPic = Path.Combine(Main.GCONST.AppDataPath,
													postID + "_previewthumbnail_" + _hashCode + ".jpg");

                    Image img = null;

					if (m_PhotoPool.ContainsKey(postID))
					{
						img = m_PhotoPool[postID];
					}
					else
					{
						var hasThumbnail = File.Exists(_localPic);
						img = LoadThumbnail(_url, _localPic, false);

						if (hasThumbnail)
							m_PhotoPool.Add(postID, img);
					}


                    DrawResizedThumbnail(thumbnailRect, g, img);
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
			DebugInfo.ShowMethod();

            try
            {
                var coverId = post.getCoverImageId();

                DrawResizedThumbnail_2(thumbnailRect, g, coverId);
            }
            catch
            {
                return false;
            }

            return true;
        }

        private Bitmap LoadThumbnail(string url, string localPicPath, bool forceNull)
        {
			DebugInfo.ShowMethod();

            Bitmap _img;

            if (File.Exists(localPicPath))
            {
                _img = new Bitmap(localPicPath);
            }
            else
            {
                if (forceNull)
                    return null;

                ImageItem _item = new ImageItem();
                _item.PostItemType = PostItemType.Thumbnail;
                _item.ThumbnailPath = url;
                _item.LocalFilePath_Origin = localPicPath;

                Main.Current.PhotoDownloader.Add(_item, false);

                _img = new Bitmap(PicWidth, PicHeight);
                Graphics _g = Graphics.FromImage(_img);

                int _w = PicWidth / 2;
                int _w2 = PicWidth / 4;

                _g.FillRectangle(Brushes.White, 0, 0, PicWidth, PicHeight);
                _g.DrawImage(Properties.Resources.photo_spinner, _w2, _w2, _w, _w);

                using (Pen _pen = new Pen(Color.FromArgb(226, 226, 226), 2))
                {
                    _pen.DashStyle = DashStyle.Dot;

                    _g.DrawRectangle(_pen, new Rectangle(1, 1, PicWidth - 2, PicHeight - 2));
                }
            }
            return _img;
        }

        private void DrawResizedThumbnail_2(Rectangle thumbnailRect, Graphics g, string object_id)
        {
			DebugInfo.ShowMethod();

            var _img = LoadAttachmentThumbnail(object_id, false);

            DrawResizedThumbnail(thumbnailRect, g, _img);
        }

        private Image LoadAttachmentThumbnail(string object_id, bool forceNull)
        {
			DebugInfo.ShowMethod();

			if (m_PhotoPool.ContainsKey(object_id))
			{
				return m_PhotoPool[object_id];
			}

			string localPicPath = Main.Current.RT.REST.attachments_getThumbnailFilePath(object_id, "small");

			if (File.Exists(localPicPath))
			{
				m_PhotoPool.Add(object_id, new Bitmap(localPicPath));
				return m_PhotoPool[object_id];
			}

			string _url = Main.Current.RT.REST.attachments_getImageURL(object_id, "small");

			return LoadThumbnail(_url, localPicPath, forceNull);
        }

        private static void DrawResizedThumbnail(Rectangle thumbnailRect, Graphics g, Image image)
        {
			DebugInfo.ShowMethod();

            if (image.Width > image.Height)
            {
                image = ImageUtility.GenerateSquareImage(image, image.Width);
            }
            else
            {
                image = ImageUtility.GenerateSquareImage(image, image.Height);
            }

            g.DrawImage(image, thumbnailRect, new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
        }

        #endregion

        private void dataGridView_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
			DebugInfo.ShowMethod();

            using (Pen _p = new Pen(Color.LightGray))
            {
                e.Graphics.DrawRectangle(_p, e.RowBounds);
            }
        }

        private void postBS_PositionChanged(object sender, EventArgs e)
        {
			DebugInfo.ShowMethod();

            m_clickedIndex = m_postBS.Position;

            if (m_clickedIndex < 0)
                return;

            Post _p = m_postBS[m_postBS.Position] as Post;

            m_clickedPostID = _p.post_id;

            if (m_clickedIndex > -1)
            {
                NotifyDetailView();

                setCalendarDay();
            }

            if (m_clickedIndex == (m_postBS.Count - 1))
            {
                // Main.Current.FilterReadMorePost(); 
            }
        }

        private void setCalendarDay()
        {
			DebugInfo.ShowMethod();

            Post _post = m_postBS[m_postBS.Position] as Post;

            Main.Current.setCalendarDay(DateTimeHelp.ISO8601ToDateTime(_post.timestamp).Date);
        }

        private void NotifyDetailView()
        {
			DebugInfo.ShowMethod();

            Post _post = m_postBS.Current as Post;

            if (IsSamePostContent(_post, m_detailView.Post))
                return;

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

        public void Thumbnail_EventHandler(ImageItem item)
        {
			DebugInfo.ShowMethod();

            RefreshUI();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
			DebugInfo.ShowMethod();

            if (Main.Current != null) //VS.NET_Bug
            {
                RefreshUI();
            }
        }

        public void RefreshUI()
        {
			DebugInfo.ShowMethod();

            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(
                           delegate { RefreshUI(); }
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

        public void ScrollTo(int index)
        {
			DebugInfo.ShowMethod();

            dataGridView.FirstDisplayedScrollingRowIndex = index;
            m_postBS.Position = index;
        }

        public void ScrollToDay(DateTime date)
        {
			DebugInfo.ShowMethod();

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

        #region Component Designer generated code

        private void InitializeComponent()
        {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PostsList));
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.miRemovePost = new System.Windows.Forms.ToolStripMenuItem();
			this.displayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.miDisplayAll = new System.Windows.Forms.ToolStripMenuItem();
			this.miDisplayText = new System.Windows.Forms.ToolStripMenuItem();
			this.miDisplayPhoto = new System.Windows.Forms.ToolStripMenuItem();
			this.miDisplayWebLink = new System.Windows.Forms.ToolStripMenuItem();
			this.timer = new System.Windows.Forms.Timer(this.components);
			this.dataGridView = new Waveface.Component.CustomDataGridView();
			this.creatoridDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.m_postBS = new System.Windows.Forms.BindingSource(this.components);
			this.contextMenuStrip.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.m_postBS)).BeginInit();
			this.SuspendLayout();
			// 
			// contextMenuStrip
			// 
			this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miRemovePost,
            this.displayToolStripMenuItem});
			this.contextMenuStrip.Name = "contextMenuStripImageList";
			resources.ApplyResources(this.contextMenuStrip, "contextMenuStrip");
			// 
			// miRemovePost
			// 
			this.miRemovePost.Image = global::Waveface.Properties.Resources.FB_remove;
			this.miRemovePost.Name = "miRemovePost";
			resources.ApplyResources(this.miRemovePost, "miRemovePost");
			this.miRemovePost.Click += new System.EventHandler(this.miRemovePost_Click);
			// 
			// displayToolStripMenuItem
			// 
			this.displayToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miDisplayAll,
            this.miDisplayText,
            this.miDisplayPhoto,
            this.miDisplayWebLink});
			this.displayToolStripMenuItem.Name = "displayToolStripMenuItem";
			resources.ApplyResources(this.displayToolStripMenuItem, "displayToolStripMenuItem");
			// 
			// miDisplayAll
			// 
			this.miDisplayAll.Checked = true;
			this.miDisplayAll.CheckState = System.Windows.Forms.CheckState.Checked;
			this.miDisplayAll.Name = "miDisplayAll";
			resources.ApplyResources(this.miDisplayAll, "miDisplayAll");
			this.miDisplayAll.Click += new System.EventHandler(this.miDisplayAll_Click);
			// 
			// miDisplayText
			// 
			this.miDisplayText.Name = "miDisplayText";
			resources.ApplyResources(this.miDisplayText, "miDisplayText");
			this.miDisplayText.Click += new System.EventHandler(this.miDisplayText_Click);
			// 
			// miDisplayPhoto
			// 
			this.miDisplayPhoto.Name = "miDisplayPhoto";
			resources.ApplyResources(this.miDisplayPhoto, "miDisplayPhoto");
			this.miDisplayPhoto.Click += new System.EventHandler(this.miDisplayPhoto_Click);
			// 
			// miDisplayWebLink
			// 
			this.miDisplayWebLink.Name = "miDisplayWebLink";
			resources.ApplyResources(this.miDisplayWebLink, "miDisplayWebLink");
			this.miDisplayWebLink.Click += new System.EventHandler(this.miDisplayWebLink_Click);
			// 
			// timer
			// 
			this.timer.Enabled = true;
			this.timer.Interval = 30000;
			this.timer.Tick += new System.EventHandler(this.timer_Tick);
			// 
			// dataGridView
			// 
			this.dataGridView.AllowDrop = true;
			this.dataGridView.AllowUserToAddRows = false;
			this.dataGridView.AllowUserToDeleteRows = false;
			this.dataGridView.AllowUserToResizeRows = false;
			this.dataGridView.AutoGenerateColumns = false;
			this.dataGridView.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(234)))), ((int)(((byte)(234)))));
			this.dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.dataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Raised;
			this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
			this.dataGridView.ColumnHeadersVisible = false;
			this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.creatoridDataGridViewTextBoxColumn});
			this.dataGridView.ContextMenuStrip = this.contextMenuStrip;
			this.dataGridView.DataSource = this.m_postBS;
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
			dataGridViewCellStyle2.Font = new System.Drawing.Font("Tahoma", 9F);
			dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
			dataGridViewCellStyle2.FormatProvider = new System.Globalization.CultureInfo("en-US");
			dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.dataGridView.DefaultCellStyle = dataGridViewCellStyle2;
			resources.ApplyResources(this.dataGridView, "dataGridView");
			this.dataGridView.EnableHeadersVisualStyles = false;
			this.dataGridView.GridColor = System.Drawing.Color.LightGray;
			this.dataGridView.MultiSelect = false;
			this.dataGridView.Name = "dataGridView";
			this.dataGridView.ReadOnly = true;
			this.dataGridView.RowHeadersVisible = false;
			this.dataGridView.RowTemplate.DefaultCellStyle.SelectionBackColor = System.Drawing.SystemColors.InactiveCaption;
			this.dataGridView.RowTemplate.DefaultCellStyle.SelectionForeColor = System.Drawing.SystemColors.InactiveCaptionText;
			this.dataGridView.RowTemplate.Height = 64;
			this.dataGridView.RowTemplate.ReadOnly = true;
			this.dataGridView.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dataGridView.VirtualMode = true;
			this.dataGridView.ContextMenuStripNeeded += new System.EventHandler<System.Windows.Forms.DataGridViewCellContextMenuStripNeededEventArgs>(this.dataGridView_ContextMenuStripNeeded);
			this.dataGridView.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dataGridView_CellPainting);
			this.dataGridView.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.dataGridView_RowPostPaint);
			this.dataGridView.Scroll += new System.Windows.Forms.ScrollEventHandler(this.dataGridView_Scroll);
			this.dataGridView.DragDrop += new System.Windows.Forms.DragEventHandler(this.dataGridView_DragDrop);
			this.dataGridView.DragEnter += new System.Windows.Forms.DragEventHandler(this.dataGridView_DragEnter);
			this.dataGridView.DragOver += new System.Windows.Forms.DragEventHandler(this.dataGridView_DragOver);
			this.dataGridView.DragLeave += new System.EventHandler(this.dataGridView_DragLeave);
			this.dataGridView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridView_KeyDown);
			this.dataGridView.KeyUp += new System.Windows.Forms.KeyEventHandler(this.dataGridView_KeyUp);
			// 
			// creatoridDataGridViewTextBoxColumn
			// 
			this.creatoridDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.creatoridDataGridViewTextBoxColumn.DataPropertyName = "creator_id";
			dataGridViewCellStyle1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 0);
			this.creatoridDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle1;
			resources.ApplyResources(this.creatoridDataGridViewTextBoxColumn, "creatoridDataGridViewTextBoxColumn");
			this.creatoridDataGridViewTextBoxColumn.Name = "creatoridDataGridViewTextBoxColumn";
			this.creatoridDataGridViewTextBoxColumn.ReadOnly = true;
			this.creatoridDataGridViewTextBoxColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			// 
			// m_postBS
			// 
			this.m_postBS.DataSource = typeof(Waveface.API.V2.Post);
			this.m_postBS.PositionChanged += new System.EventHandler(this.postBS_PositionChanged);
			// 
			// PostsList
			// 
			this.BackColor = System.Drawing.SystemColors.Window;
			this.Controls.Add(this.dataGridView);
			resources.ApplyResources(this, "$this");
			this.Name = "PostsList";
			this.Load += new System.EventHandler(this.PostsList_Load);
			this.contextMenuStrip.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.m_postBS)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion


        private void dataGridView_ContextMenuStripNeeded(object sender,
                                                         DataGridViewCellContextMenuStripNeededEventArgs e)
        {
			DebugInfo.ShowMethod();

            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            dataGridView[e.ColumnIndex, e.RowIndex].Selected = true;
            dataGridView.CurrentCell = dataGridView[e.ColumnIndex, e.RowIndex];
        }

        private void miRemovePost_Click(object sender, EventArgs e)
        {
			DebugInfo.ShowMethod();

            RemovePost();
        }

        public void RemovePost()
        {
			DebugInfo.ShowMethod();

            Post _post = m_postBS[m_postBS.Position] as Post;

            DialogResult _dr = MessageBox.Show(Resources.ASK_REMOVE_POST, "Stream", MessageBoxButtons.YesNo,
                                               MessageBoxIcon.Question);

            if (_dr != DialogResult.Yes)
                return;

            Main.Current.HidePost(_post.post_id);
        }

		Boolean m_IsSettingDataText = false;
        private void dataGridView_Scroll(object sender, ScrollEventArgs e)
        {
			DebugInfo.ShowMethod();

			if (m_postBS.Count <= 0)
				return;

			if (m_IsSettingDataText)
			{
				m_IsSettingDataText = false;
				return;
			}
			m_IsSettingDataText = true;
			SetDateText();
			m_IsSettingDataText = false;
        }

        private void SetDateText()
        {
			DebugInfo.ShowMethod();

            ResizeCell();

            MyParent.SetDateTextFont(m_fontText);

            if (m_posts.Count > 0)
            {
                Post _post = m_postBS[dataGridView.FirstDisplayedScrollingRowIndex] as Post;
                DateTime _dt = DateTimeHelp.ISO8601ToDateTime(_post.timestamp).Date;

                MyParent.SetDateText(_dt.Date.ToString("yyyy-MM-dd (ddd)"));
            }
            else
            {
                MyParent.SetDateText("");
            }
        }

        private void ResizeCell()
        {
			DebugInfo.ShowMethod();

			try
			{
				SuspendLayout();
				int _s = dataGridView.FirstDisplayedScrollingRowIndex;
				int _k = 0;
				int _cn = (dataGridView.Height / m_cellHeight);

				for (int i = _s; (i < m_posts.Count) && (_k < _cn); i++)
				{
					var post = m_posts[i];

					bool _isLinkPost = post.type == "link";

					var row = dataGridView.Rows[i];
					if (m_firstPostInADay.ContainsValue(post.post_id) && (i != dataGridView.FirstDisplayedScrollingRowIndex))
					{
						row.Height = (_isLinkPost ? m_cellLinkHeight : m_cellHeight) + m_timeBarHeight;
					}
					else
					{
						row.Height = (_isLinkPost ? m_cellLinkHeight : m_cellHeight);
					}

					if (_isLinkPost)
					{
						if (!string.IsNullOrEmpty(post.content))
						{
							row.Height += m_fontText.Height;
						}
						else
						{
							row.Height -= m_fontText.Height;
						}
					}

					_k++;
				}
			}
			finally
			{
				ResumeLayout();
			}
        }

        #region Drag & Drop

        private Post GetCurrentPost()
        {
			DebugInfo.ShowMethod();

            Point _cursorPosition = dataGridView.PointToClient(Cursor.Position);
            DataGridView.HitTestInfo _info = dataGridView.HitTest(_cursorPosition.X, _cursorPosition.Y);

            if (_info.RowIndex >= 0)
            {
                dataGridView[_info.ColumnIndex, _info.RowIndex].Selected = true;
                dataGridView.CurrentCell = dataGridView[_info.ColumnIndex, _info.RowIndex];
                return m_postBS[_info.RowIndex] as Post;
            }
            else
            {
                return null;
            }
        }

        private void dataGridView_DragDrop(object sender, DragEventArgs e)
        {
			DebugInfo.ShowMethod();

            Post _p = GetCurrentPost();

            if (_p != null)
            {
                List<string> _pics = m_dragDropClipboardHelper.Drag_Drop(e);

                if (_pics != null)
                {
                    m_detailView.ExistPostAddPhotos(_pics, 0);
                }
            }

            FlashWindow.Stop(Main.Current);
        }

        private void dataGridView_DragEnter(object sender, DragEventArgs e)
        {
			DebugInfo.ShowMethod();

            FlashWindow.Start(Main.Current);

            Post _p = GetCurrentPost();

            if (_p != null)
            {
                m_dragDropClipboardHelper.Drag_Enter(e, (_p.type == "link"));
            }
        }

        private void dataGridView_DragOver(object sender, DragEventArgs e)
        {
			DebugInfo.ShowMethod();

            Post _p = GetCurrentPost();

            if (_p != null)
            {
                m_dragDropClipboardHelper.Drag_Over(e, (_p.type == "link"));
            }
        }

        private void dataGridView_DragLeave(object sender, EventArgs e)
        {
			DebugInfo.ShowMethod();

            m_dragDropClipboardHelper.Drag_Leave();

            FlashWindow.Stop(Main.Current);
        }

        #endregion

        #region Key

        private bool m_isKeyPressed;

        private void dataGridView_KeyUp(object sender, KeyEventArgs e)
        {
			DebugInfo.ShowMethod();

            m_isKeyPressed = false;
        }

        private void dataGridView_KeyDown(object sender, KeyEventArgs e)
        {
			DebugInfo.ShowMethod();

            e.SuppressKeyPress = m_isKeyPressed;

            m_isKeyPressed = true;
        }

        #endregion

        #region Display

        private void miDisplayAll_Click(object sender, EventArgs e)
        {
			DebugInfo.ShowMethod();

            doDisplay("");

            miDisplayAll.Checked = true;
        }

        private void miDisplayText_Click(object sender, EventArgs e)
        {
			DebugInfo.ShowMethod();

            doDisplay("text");

            miDisplayText.Checked = true;
        }

        private void miDisplayPhoto_Click(object sender, EventArgs e)
        {
			DebugInfo.ShowMethod();

            doDisplay("image");

            miDisplayPhoto.Checked = true;
        }

        private void miDisplayWebLink_Click(object sender, EventArgs e)
        {
			DebugInfo.ShowMethod();

            doDisplay("link");

            miDisplayWebLink.Checked = true;
        }

        private void doDisplay(string type)
        {
			DebugInfo.ShowMethod();

            miDisplayAll.Checked = false;
            miDisplayText.Checked = false;
            miDisplayPhoto.Checked = false;
            miDisplayWebLink.Checked = false;

            Main.Current.DisplayFilter(type);
        }

        #endregion
    }
}