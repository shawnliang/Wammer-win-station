#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Net;
using System.Windows.Forms;
using Microsoft.Win32;
using Waveface.API.V2;
using Waveface.Component;
using Waveface.Properties;

#endregion

namespace Waveface
{
    public class PostsList : UserControl
    {
        private IContainer components;
        private DataGridView dataGridView;
        private int m_clickIndex;
        private DetailView m_detailView;
        private Bitmap m_emptyImage;
        private Font m_font;
        private BindingSource m_postBS;
        private DataGridViewTextBoxColumn creatoridDataGridViewTextBoxColumn;
        private Timer timer;
        private List<Post> m_post;

        #region Properties

        public List<Post> Posts
        {
            set
            {
                m_post = value;
                m_postBS.DataSource = value;
                //m_postBS.Position = m_clickIndex;

                dataGridView.DataSource = null;
                dataGridView.DataSource = m_postBS;
                dataGridView.Refresh();
            }
        }

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

            MouseWheelRedirector.Attach(dataGridView);
        }

        private void SetFont()
        {
            m_font = SystemFonts.IconTitleFont;

            if (Font != m_font)
            {
                Font = m_font;

                dataGridView.RowTemplate.Height = 104;
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            dataGridView.SuspendLayout();

            dataGridView.Refresh();

            dataGridView.ResumeLayout();
        }

        public void ScrollToDay(DateTime date)
        {
            int k = -1;

            for (int i =0; i< m_post.Count; i++)
            {
                if(DateTimeHelp.ISO8601ToDateTime(m_post[i].timestamp).Date == date)
                {
                    k = i;
                    break;
                }
            }

            if(k != -1)
            {
                dataGridView.FirstDisplayedScrollingRowIndex = k;
                m_postBS.Position = k;
            }
        }

        #region Event Handlers

        private void PostsList_Load(object sender, EventArgs e)
        {
            m_emptyImage = Resources.Error;

            SetFont();

            // Add UPChanged
            SystemEvents.UserPreferenceChanged += SystemEvents_UserPreferenceChanged;
        }

        private void SystemEvents_UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            SetFont();
        }

        private void dataGridView_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            const int picHeight = 75;
            const int picWidth = 75;

            bool _isDrawThumbnail = false;

            Graphics _g = e.Graphics;

            Post _post = m_postBS[e.RowIndex] as Post;

            bool _selected = ((e.State & DataGridViewElementStates.Selected) == DataGridViewElementStates.Selected);

            Color _fcolor = (_selected ? e.CellStyle.SelectionForeColor : e.CellStyle.ForeColor);
            Color _bcolor = (_selected ? e.CellStyle.SelectionBackColor : e.CellStyle.BackColor);

            Font _fontLarge = new Font(e.CellStyle.Font.FontFamily, e.CellStyle.Font.Size + 1);
            Font _fontSmall = new Font(e.CellStyle.Font.FontFamily, e.CellStyle.Font.Size - 1);

            int _X = e.CellBounds.Left + e.CellStyle.Padding.Left;
            int _Y = e.CellBounds.Top + e.CellStyle.Padding.Top;
            int _W = e.CellBounds.Width - (e.CellStyle.Padding.Left + e.CellStyle.Padding.Right);
            int _H = e.CellBounds.Height - (e.CellStyle.Padding.Top + e.CellStyle.Padding.Bottom);

            Rectangle _cellRect = new Rectangle(_X, _Y, _W, _H);

            // Draw background
            _g.FillRectangle(new SolidBrush(_bcolor), e.CellBounds);

            Rectangle _timeRect = DrawPostTime(_g, _fontSmall, _cellRect, _post);

            Rectangle _thumbnailRect = new Rectangle(_X + 2, _Y + 2, picWidth, picHeight);

            _isDrawThumbnail = DrawThumbnail(_g, _thumbnailRect, _post);

            int _offectThumbnail_W = (_isDrawThumbnail ? _thumbnailRect.Width : 0);

            Rectangle _infoRect = DrawPostInfo(_g, _fontSmall, _cellRect, _post, _offectThumbnail_W);

            DrawSubject(_g, _fontLarge, _cellRect, _post, _offectThumbnail_W, _infoRect.Height, _timeRect.Height);

            // Draw Comment
            if (_post.comment_count > 0)
            {
                int _rx = _cellRect.X + (_thumbnailRect.Width / 2) - 8;
                int _ry = _cellRect.Y + _thumbnailRect.Height + 12;

                Rectangle _rect = new Rectangle(_rx, _ry, 24, 16);

                GraphicsPath _gPath = new GraphicsPath(new[]
                                                               {
                                                                   new Point(_rx + 6, _ry + 16),
                                                                   new Point(_rx + 6 + 8, _ry + 16),
                                                                   new Point(_rx + 6, _ry + 16 + 8)
                                                               }, new[]
                                                                      {
                                                                          (byte) PathPointType.Line,
                                                                          (byte) PathPointType.Line,
                                                                          (byte) PathPointType.Line
                                                                      });

                Region _r = new Region(_rect);
                _r.Union(_gPath);

                //_g.FillRegion(Brushes.LightGray, _r);
                Font _fontC = new Font(e.CellStyle.Font.FontFamily, e.CellStyle.Font.Size, FontStyle.Bold);

                //TextRenderer.DrawText(_g, _post.comment_count.ToString(), _fontC, new Rectangle(_rx + 2, _ry, 20, 16), Color.White,
                //TextFormatFlags.HorizontalCenter | TextFormatFlags.HorizontalCenter);
            }

            // Let them know we handled it
            e.Handled = true;
        }

        private void DrawSubject(Graphics g, Font font, Rectangle rect, Post post, int thumbnailW, int infoH, int timeH)
        {
            int dt = 4;
            int dt2 = dt * 2;

            Rectangle _r = new Rectangle();
            Rectangle _rectAll = new Rectangle(rect.X + (dt / 2), rect.Y + (dt / 2), rect.Width - dt, rect.Height - timeH - dt);
            Rectangle _rectNoInfo = new Rectangle(rect.X + thumbnailW + dt, rect.Y + dt, rect.Width - thumbnailW - dt2, rect.Height - timeH - dt2);
            Rectangle _rectSmall = new Rectangle(rect.X + thumbnailW + dt, rect.Y + infoH + dt, rect.Width - thumbnailW - dt2, rect.Height - infoH - timeH - dt2);

            switch (post.type)
            {
                case "image":
                case "link":
                case "doc":
                    _r = _rectSmall;
                    break;

                case "rtf":
                    _r = _rectNoInfo;
                    break;

                case "text":
                    _r = _rectAll;
                    break;
            }

            TextRenderer.DrawText(g, post.content, font, _r, SystemColors.WindowText,
                                  TextFormatFlags.WordBreak | TextFormatFlags.PreserveGraphicsClipping | TextFormatFlags.EndEllipsis);
        }

        private Rectangle DrawPostInfo(Graphics g, Font font, Rectangle cellRect, Post post, int thumbnailOffset_X)
        {
            string _info = string.Empty;

            switch (post.type)
            {
                case "image":
                    _info = post.attachments_count + ((post.attachments_count > 1) ? " photos" : " photo");
                    break;

                case "link":
                    _info = post.preview.url;
                    break;

                case "doc":
                    _info = post.attachments[0].file_name;
                    break;
            }

            Size _sizeInfo = TextRenderer.MeasureText(g, _info, font);
            Rectangle _rect = new Rectangle(cellRect.X + thumbnailOffset_X + 4, cellRect.Y + 2, cellRect.Width - thumbnailOffset_X - 6, _sizeInfo.Height);

            TextRenderer.DrawText(g, _info, font, _rect, Color.DodgerBlue,
                                  TextFormatFlags.PreserveGraphicsClipping | TextFormatFlags.EndEllipsis);

            return _rect;
        }

        private Rectangle DrawPostTime(Graphics g, Font font, Rectangle cellRect, Post post)
        {
            string _postTime = post.timestamp;
            _postTime = DateTimeHelp.ISO8601ToDotNet(_postTime);
            _postTime = DateTimeHelp.PrettyDate(_postTime);

            Size _sizeTime = TextRenderer.MeasureText(g, _postTime, font) + new Size(2, 2);
            Rectangle _timeRect = new Rectangle(cellRect.X + cellRect.Width - _sizeTime.Width, cellRect.Y + cellRect.Height - _sizeTime.Height, _sizeTime.Width, _sizeTime.Height);

            TextRenderer.DrawText(g, _postTime, font, _timeRect, Color.DimGray,
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
                /*
                Attachment _a = post.attachments[0];

                string _url = MainForm.THIS.attachments_getRedirectURL(_a.image);

                string _localPic = MainForm.GCONST.ImageCachePath + _a.object_id + "_small_" + _a.file_name;

                Bitmap _img = LoadThumbnail(_url, _localPic);

                g.DrawImage(_img, thumbnailRect, new Rectangle(0, 0, _img.Width, _img.Height), GraphicsUnit.Pixel);
                */

                g.FillRectangle(new SolidBrush(Color.DarkRed), thumbnailRect);
                g.DrawRectangle(new Pen(Color.Black), thumbnailRect);
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

                string _url = MainForm.THIS.attachments_getRedirectURL(_a.url + "&image_meta=small");

                string _localPic = MainForm.GCONST.CachePath + _a.object_id + "_small_" + _a.file_name;

                Bitmap _img = LoadThumbnail(_url, _localPic);

                g.DrawImage(_img, thumbnailRect, new Rectangle(0, 0, _img.Width, _img.Height),
                             GraphicsUnit.Pixel);
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
                string _url = post.preview.thumbnail_url;

                string _localPic = MainForm.GCONST.CachePath + post.post_id + "_previewthumbnail_" + ".jpg";

                Bitmap _img = LoadThumbnail(_url, _localPic);

                g.DrawImage(_img, thumbnailRect, new Rectangle(0, 0, _img.Width, _img.Height),
                             GraphicsUnit.Pixel);
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

                string _url = MainForm.THIS.attachments_getRedirectURL(_a.url + "&image_meta=small");

                string _localPic = MainForm.GCONST.CachePath + _a.object_id + "_small_" + _a.file_name;

                Bitmap _img = LoadThumbnail(_url, _localPic);

                g.DrawImage(_img, thumbnailRect, new Rectangle(0, 0, _img.Width, _img.Height), GraphicsUnit.Pixel);
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
                WebRequest _wReq = WebRequest.Create(url);
                WebResponse _wRep = _wReq.GetResponse();
                _img = (Bitmap)Image.FromStream(_wRep.GetResponseStream());
                _img.Save(localPicPath);
                _img = null;

                Shadow.CreateShadowImage(localPicPath, 3);

                _img = new Bitmap(localPicPath);
            }

            return _img;
        }

        #endregion

        private void dataGridView_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            if (e.RowIndex == m_postBS.Position)
            {
                e.DrawFocus(e.RowBounds, true);
            }
            else
            {
                using (Pen _p = new Pen(Color.LightGray))
                {
                    e.Graphics.DrawRectangle(_p, e.RowBounds);
                }
            }
        }

        private void postBS_PositionChanged(object sender, EventArgs e)
        {
            m_clickIndex = m_postBS.Position;

            if (m_clickIndex > -1)
            {
                NotifyDetailView();

                setCalendarDay();
            }

            if (m_clickIndex == (m_postBS.Count - 1))
            {
                MainForm.THIS.ReadMore();
            }
        }

        private void setCalendarDay()
        {
            Post _post = m_postBS[m_postBS.Position] as Post;

            MainForm.THIS.setCalendarDay(DateTimeHelp.ISO8601ToDateTime(_post.timestamp).Date);
        }

        private void NotifyDetailView()
        {
            Post _post = m_postBS.Current as Post;

            foreach (User _user in MainForm.THIS.RT.AllUsers)
            {
                if (_user.user_id == _post.creator_id)
                {
                    m_detailView.User = _user;
                }
            }

            m_detailView.Post = _post;
        }

        #endregion

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.m_postBS = new System.Windows.Forms.BindingSource(this.components);
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.creatoridDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
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
            this.dataGridView.BackgroundColor = System.Drawing.SystemColors.Window;
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