#region

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

#endregion

namespace Waveface.Component
{
    public class TaskbarNotifier : Form
    {
        #region TaskbarNotifier Enums

        // List of the different popup animation status
        public enum TaskbarStates
        {
            hidden = 0,
            appearing = 1,
            visible = 2,
            disappearing = 3
        }

        #endregion

        public event EventHandler CloseClick = null;
        public event EventHandler TitleClick = null;
        public event EventHandler ContentClick = null;

        #region Fields

        public bool CloseClickable = true;
        public bool ContentClickable = true;
        public Rectangle ContentRectangle;
        public bool EnableSelectionRectangle = true;
        public bool TitleClickable;
        public Rectangle TitleRectangle;

        protected Bitmap BackgroundBitmap;
        protected Bitmap CloseBitmap;
        protected Point CloseBitmapLocation;
        protected Size CloseBitmapSize;
        protected Rectangle RealContentRectangle;
        protected Rectangle RealTitleRectangle;
        protected Rectangle WorkAreaRectangle;
        protected bool m_isMouseDown;
        protected bool m_isMouseOverClose;
        protected bool m_isMouseOverContent;
        protected bool m_isMouseOverPopup;
        protected bool m_isMouseOverTitle;
        protected bool m_keepVisibleOnMouseOver = true; 
        protected bool m_reShowOnMouseOver; 
        protected string m_contentText;
        protected Color m_hoverContentColor = Color.FromArgb(0, 0, 0x66);
        protected Font m_hoverContentFont = new Font("Arial", 11, FontStyle.Regular, GraphicsUnit.Pixel);
        protected Color m_hoverTitleColor = Color.FromArgb(255, 0, 0);
        protected Font m_hoverTitleFont = new Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Pixel);
        protected int m_hideEvents;
        protected int m_incrementHide;
        protected int m_incrementShow;
        protected int m_showEvents;
        protected int m_visibleEvents;
        protected Color m_normalContentColor = Color.FromArgb(0, 0, 0);
        protected Font m_normalContentFont = new Font("Arial", 11, FontStyle.Regular, GraphicsUnit.Pixel);
        protected Color m_normalTitleColor = Color.FromArgb(255, 0, 0);
        protected Font m_normalTitleFont = new Font("Arial", 12, FontStyle.Regular, GraphicsUnit.Pixel);
        protected TaskbarStates m_taskbarState = TaskbarStates.hidden;
        protected Timer m_timer = new Timer();
        protected string m_titleText;

        #endregion

        #region Properties

        public TaskbarStates TaskbarState
        {
            get { return m_taskbarState; }
        }

        public string TitleText
        {
            get { return m_titleText; }
            set
            {
                m_titleText = value;
                Refresh();
            }
        }

        public string ContentText
        {
            get { return m_contentText; }
            set
            {
                m_contentText = value;
                Refresh();
            }
        }

        public Color NormalTitleColor
        {
            get { return m_normalTitleColor; }
            set
            {
                m_normalTitleColor = value;
                Refresh();
            }
        }

        public Color HoverTitleColor
        {
            get { return m_hoverTitleColor; }
            set
            {
                m_hoverTitleColor = value;
                Refresh();
            }
        }

        public Color NormalContentColor
        {
            get { return m_normalContentColor; }
            set
            {
                m_normalContentColor = value;
                Refresh();
            }
        }

        public Color HoverContentColor
        {
            get { return m_hoverContentColor; }
            set
            {
                m_hoverContentColor = value;
                Refresh();
            }
        }

        public Font NormalTitleFont
        {
            get { return m_normalTitleFont; }
            set
            {
                m_normalTitleFont = value;
                Refresh();
            }
        }

        public Font HoverTitleFont
        {
            get { return m_hoverTitleFont; }
            set
            {
                m_hoverTitleFont = value;
                Refresh();
            }
        }

        public Font NormalContentFont
        {
            get { return m_normalContentFont; }
            set
            {
                m_normalContentFont = value;
                Refresh();
            }
        }

        public Font HoverContentFont
        {
            get { return m_hoverContentFont; }
            set
            {
                m_hoverContentFont = value;
                Refresh();
            }
        }

        public bool KeepVisibleOnMousOver
        {
            get { return m_keepVisibleOnMouseOver; }
            set { m_keepVisibleOnMouseOver = value; }
        }

        public bool ReShowOnMouseOver
        {
            get { return m_reShowOnMouseOver; }
            set { m_reShowOnMouseOver = value; }
        }

        #endregion

        public TaskbarNotifier()
        {
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Minimized;
            
            Show();
            
            base.Hide();
           
            WindowState = FormWindowState.Normal;
            ShowInTaskbar = false;
            TopMost = true;
            MaximizeBox = false;
            MinimizeBox = false;
            ControlBox = false;

            m_timer.Enabled = true;
            m_timer.Tick += OnTimer;
        }

        #region TaskbarNotifier Public Methods

        [DllImport("user32.dll")]
        private static extern Boolean ShowWindow(IntPtr hWnd, Int32 nCmdShow);

        /// <summary>
        /// Displays the popup for a certain amount of time
        /// </summary>
        /// <param name="strTitle">The string which will be shown as the title of the popup</param>
        /// <param name="strContent">The string which will be shown as the content of the popup</param>
        /// <param name="timeToShow">Duration of the showing animation (in milliseconds)</param>
        /// <param name="timeToStay">Duration of the visible state before collapsing (in milliseconds)</param>
        /// <param name="timeToHide">Duration of the hiding animation (in milliseconds)</param>
        /// <returns>Nothing</returns>
        public void Show(string strTitle, string strContent, int timeToShow, int timeToStay, int timeToHide)
        {
            WorkAreaRectangle = Screen.GetWorkingArea(WorkAreaRectangle);
            m_titleText = strTitle;
            m_contentText = strContent;
            m_visibleEvents = timeToStay;
            CalculateMouseRectangles();

            // We calculate the pixel increment and the timer value for the showing animation
            int _events;

            if (timeToShow > 10)
            {
                _events = Math.Min((timeToShow / 10), BackgroundBitmap.Height);
                m_showEvents = timeToShow / _events;
                m_incrementShow = BackgroundBitmap.Height / _events;
            }
            else
            {
                m_showEvents = 10;
                m_incrementShow = BackgroundBitmap.Height;
            }

            // We calculate the pixel increment and the timer value for the hiding animation
            if (timeToHide > 10)
            {
                _events = Math.Min((timeToHide / 10), BackgroundBitmap.Height);
                m_hideEvents = timeToHide / _events;
                m_incrementHide = BackgroundBitmap.Height / _events;
            }
            else
            {
                m_hideEvents = 10;
                m_incrementHide = BackgroundBitmap.Height;
            }

            switch (m_taskbarState)
            {
                case TaskbarStates.hidden:
                    m_taskbarState = TaskbarStates.appearing;
                    SetBounds(WorkAreaRectangle.Right - BackgroundBitmap.Width - 17, WorkAreaRectangle.Bottom - 1, BackgroundBitmap.Width, 0);
                    m_timer.Interval = m_showEvents;
                    m_timer.Start();

                    // We Show the popup without stealing focus
                    ShowWindow(Handle, 4);
                    break;

                case TaskbarStates.appearing:
                    Refresh();
                    break;

                case TaskbarStates.visible:
                    m_timer.Stop();
                    m_timer.Interval = m_visibleEvents;
                    m_timer.Start();
                    Refresh();
                    break;

                case TaskbarStates.disappearing:
                    m_timer.Stop();
                    m_taskbarState = TaskbarStates.visible;
                    SetBounds(WorkAreaRectangle.Right - BackgroundBitmap.Width - 17,
                              WorkAreaRectangle.Bottom - BackgroundBitmap.Height - 1, BackgroundBitmap.Width,
                              BackgroundBitmap.Height);
                    m_timer.Interval = m_visibleEvents;
                    m_timer.Start();
                    Refresh();
                    break;
            }
        }

        public new void Hide()
        {
            if (m_taskbarState != TaskbarStates.hidden)
            {
                m_timer.Stop();
                m_taskbarState = TaskbarStates.hidden;
                base.Hide();
            }
        }

        public void SetBackgroundBitmap(string strFilename, Color transparencyColor)
        {
            BackgroundBitmap = new Bitmap(strFilename);
            Width = BackgroundBitmap.Width;
            Height = BackgroundBitmap.Height;
            Region = BitmapToRegion(BackgroundBitmap, transparencyColor);
        }

        public void SetBackgroundBitmap(Image image, Color transparencyColor)
        {
            BackgroundBitmap = new Bitmap(image);
            Width = BackgroundBitmap.Width;
            Height = BackgroundBitmap.Height;
            Region = BitmapToRegion(BackgroundBitmap, transparencyColor);
        }

        public void SetCloseBitmap(string strFilename, Color transparencyColor, Point position)
        {
            CloseBitmap = new Bitmap(strFilename);
            CloseBitmap.MakeTransparent(transparencyColor);
            CloseBitmapSize = new Size(CloseBitmap.Width / 3, CloseBitmap.Height);
            CloseBitmapLocation = position;
        }

        public void SetCloseBitmap(Image image, Color transparencyColor, Point position)
        {
            CloseBitmap = new Bitmap(image);
            CloseBitmap.MakeTransparent(transparencyColor);
            CloseBitmapSize = new Size(CloseBitmap.Width / 3, CloseBitmap.Height);
            CloseBitmapLocation = position;
        }

        #endregion

        #region TaskbarNotifier Protected Methods

        protected void DrawCloseButton(Graphics grfx)
        {
            if (CloseBitmap != null)
            {
                Rectangle _rectDest = new Rectangle(CloseBitmapLocation, CloseBitmapSize);
                Rectangle _rectSrc;

                if (m_isMouseOverClose)
                {
                    if (m_isMouseDown)
                        _rectSrc = new Rectangle(new Point(CloseBitmapSize.Width * 2, 0), CloseBitmapSize);
                    else
                        _rectSrc = new Rectangle(new Point(CloseBitmapSize.Width, 0), CloseBitmapSize);
                }
                else
                    _rectSrc = new Rectangle(new Point(0, 0), CloseBitmapSize);


                grfx.DrawImage(CloseBitmap, _rectDest, _rectSrc, GraphicsUnit.Pixel);
            }
        }

        protected void DrawText(Graphics grfx)
        {
            if (!string.IsNullOrEmpty(m_titleText))
            {
                StringFormat _sf = new StringFormat();
                _sf.Alignment = StringAlignment.Near;
                _sf.LineAlignment = StringAlignment.Center;
                _sf.FormatFlags = StringFormatFlags.NoWrap;
                _sf.Trimming = StringTrimming.EllipsisCharacter;

                if (m_isMouseOverTitle)
                    grfx.DrawString(m_titleText, m_hoverTitleFont, new SolidBrush(m_hoverTitleColor), TitleRectangle, _sf);
                else
                    grfx.DrawString(m_titleText, m_normalTitleFont, new SolidBrush(m_normalTitleColor), TitleRectangle, _sf);
            }

            if (!string.IsNullOrEmpty(m_contentText))
            {
                StringFormat _sf = new StringFormat();
                _sf.Alignment = StringAlignment.Center;
                _sf.LineAlignment = StringAlignment.Center;
                _sf.FormatFlags = StringFormatFlags.MeasureTrailingSpaces;
                _sf.Trimming = StringTrimming.Word;

                if (m_isMouseOverContent)
                {
                    grfx.DrawString(m_contentText, m_hoverContentFont, new SolidBrush(m_hoverContentColor), ContentRectangle, _sf);
                   
                    if (EnableSelectionRectangle)
                        System.Windows.Forms.ControlPaint.DrawBorder3D(grfx, RealContentRectangle, Border3DStyle.Etched,
                                                  Border3DSide.Top | Border3DSide.Bottom | Border3DSide.Left | Border3DSide.Right);
                }
                else
                    grfx.DrawString(m_contentText, m_normalContentFont, new SolidBrush(m_normalContentColor), ContentRectangle, _sf);
            }
        }

        protected void CalculateMouseRectangles()
        {
            Graphics _grfx = CreateGraphics();
            StringFormat _sf = new StringFormat();
            _sf.Alignment = StringAlignment.Center;
            _sf.LineAlignment = StringAlignment.Center;
            _sf.FormatFlags = StringFormatFlags.MeasureTrailingSpaces;
           
            SizeF _sizefTitle = _grfx.MeasureString(m_titleText, m_hoverTitleFont, TitleRectangle.Width, _sf);
            SizeF _sizefContent = _grfx.MeasureString(m_contentText, m_hoverContentFont, ContentRectangle.Width, _sf);
            _grfx.Dispose();

            //We should check if the title size really fits inside the pre-defined title rectangle
            if (_sizefTitle.Height > TitleRectangle.Height)
            {
                RealTitleRectangle = new Rectangle(TitleRectangle.Left, TitleRectangle.Top, TitleRectangle.Width,
                                                   TitleRectangle.Height);
            }
            else
            {
                RealTitleRectangle = new Rectangle(TitleRectangle.Left, TitleRectangle.Top, (int)_sizefTitle.Width,
                                                   (int)_sizefTitle.Height);
            }
            RealTitleRectangle.Inflate(0, 2);

            //We should check if the Content size really fits inside the pre-defined Content rectangle
            if (_sizefContent.Height > ContentRectangle.Height)
            {
                RealContentRectangle =
                    new Rectangle((ContentRectangle.Width - (int)_sizefContent.Width) / 2 + ContentRectangle.Left,
                                  ContentRectangle.Top, (int)_sizefContent.Width, ContentRectangle.Height);
            }
            else
            {
                RealContentRectangle =
                    new Rectangle((ContentRectangle.Width - (int)_sizefContent.Width) / 2 + ContentRectangle.Left,
                                  (ContentRectangle.Height - (int)_sizefContent.Height) / 2 + ContentRectangle.Top,
                                  (int)_sizefContent.Width, (int)_sizefContent.Height);
            }

            RealContentRectangle.Inflate(0, 2);
        }

        protected Region BitmapToRegion(Bitmap bitmap, Color transparencyColor)
        {
            if (bitmap == null)
                throw new ArgumentNullException("Bitmap", "Bitmap cannot be null!");

            int _height = bitmap.Height;
            int _width = bitmap.Width;

            GraphicsPath _path = new GraphicsPath();

            for (int j = 0; j < _height; j++)
                for (int i = 0; i < _width; i++)
                {
                    if (bitmap.GetPixel(i, j) == transparencyColor)
                        continue;

                    int x0 = i;

                    while ((i < _width) && (bitmap.GetPixel(i, j) != transparencyColor))
                        i++;

                    _path.AddRectangle(new Rectangle(x0, j, i - x0, 1));
                }

            Region region = new Region(_path);
            _path.Dispose();
            return region;
        }

        #endregion

        #region TaskbarNotifier Events Overrides

        protected void OnTimer(Object obj, EventArgs ea)
        {
            switch (m_taskbarState)
            {
                case TaskbarStates.appearing:
                    if (Height < BackgroundBitmap.Height)
                    {
                        Top -= m_incrementShow;
                        Height += m_incrementShow;                                                                       
                    }
                    else
                    {
                        m_timer.Stop();
                        Height = BackgroundBitmap.Height;
                        m_timer.Interval = m_visibleEvents;
                        m_taskbarState = TaskbarStates.visible;
                        m_timer.Start();
                    }

                    break;

                case TaskbarStates.visible:
                    m_timer.Stop();
                    m_timer.Interval = m_hideEvents;
                   
                    if ((m_keepVisibleOnMouseOver && !m_isMouseOverPopup) || (!m_keepVisibleOnMouseOver))
                    {
                        m_taskbarState = TaskbarStates.disappearing;
                    }
                    
                    m_timer.Start();
                    break;

                case TaskbarStates.disappearing:
                    if (m_reShowOnMouseOver && m_isMouseOverPopup)
                    {
                        m_taskbarState = TaskbarStates.appearing;
                    }
                    else
                    {
                        if (Top < WorkAreaRectangle.Bottom)
                        {
                            Top += m_incrementHide;
                            Height -= m_incrementHide;
                        }
                        else
                            Hide();
                    }

                    break;
            }
        }

        protected override void OnMouseEnter(EventArgs ea)
        {
            base.OnMouseEnter(ea);

            m_isMouseOverPopup = true;
            Refresh();
        }

        protected override void OnMouseLeave(EventArgs ea)
        {
            base.OnMouseLeave(ea);

            m_isMouseOverPopup = false;
            m_isMouseOverClose = false;
            m_isMouseOverTitle = false;
            m_isMouseOverContent = false;
            Refresh();
        }

        protected override void OnMouseMove(MouseEventArgs mea)
        {
            base.OnMouseMove(mea);

            bool _contentModified = false;

            if ((mea.X > CloseBitmapLocation.X) && (mea.X < CloseBitmapLocation.X + CloseBitmapSize.Width) &&
                (mea.Y > CloseBitmapLocation.Y) && (mea.Y < CloseBitmapLocation.Y + CloseBitmapSize.Height) &&
                CloseClickable)
            {
                if (!m_isMouseOverClose)
                {
                    m_isMouseOverClose = true;
                    m_isMouseOverTitle = false;
                    m_isMouseOverContent = false;
                    Cursor = Cursors.Hand;
                    _contentModified = true;
                }
            }
            else if (RealContentRectangle.Contains(new Point(mea.X, mea.Y)) && ContentClickable)
            {
                if (!m_isMouseOverContent)
                {
                    m_isMouseOverClose = false;
                    m_isMouseOverTitle = false;
                    m_isMouseOverContent = true;
                    Cursor = Cursors.Hand;
                    _contentModified = true;
                }
            }
            else if (RealTitleRectangle.Contains(new Point(mea.X, mea.Y)) && TitleClickable)
            {
                if (!m_isMouseOverTitle)
                {
                    m_isMouseOverClose = false;
                    m_isMouseOverTitle = true;
                    m_isMouseOverContent = false;
                    Cursor = Cursors.Hand;
                    _contentModified = true;
                }
            }
            else
            {
                if (m_isMouseOverClose || m_isMouseOverTitle || m_isMouseOverContent)
                    _contentModified = true;

                m_isMouseOverClose = false;
                m_isMouseOverTitle = false;
                m_isMouseOverContent = false;
                Cursor = Cursors.Default;
            }

            if (_contentModified)
                Refresh();
        }

        protected override void OnMouseDown(MouseEventArgs mea)
        {
            base.OnMouseDown(mea);

            m_isMouseDown = true;

            if (m_isMouseOverClose)
                Refresh();
        }

        protected override void OnMouseUp(MouseEventArgs mea)
        {
            base.OnMouseUp(mea);

            m_isMouseDown = false;

            if (m_isMouseOverClose)
            {
                Hide();

                if (CloseClick != null)
                    CloseClick(this, new EventArgs());
            }
            else if (m_isMouseOverTitle)
            {
                if (TitleClick != null)
                    TitleClick(this, new EventArgs());
            }
            else if (m_isMouseOverContent)
            {
                if (ContentClick != null)
                    ContentClick(this, new EventArgs());
            }
        }

        protected override void OnPaintBackground(PaintEventArgs pea)
        {
            if (DesignMode)
            {
                pea.Graphics.Clear(SystemColors.Window);
            }
            else
            {
                Graphics _grfx = pea.Graphics;
                _grfx.PageUnit = GraphicsUnit.Pixel;

                Bitmap _offscreenBitmap = new Bitmap(BackgroundBitmap.Width, BackgroundBitmap.Height);
                Graphics _offScreenGraphics = Graphics.FromImage(_offscreenBitmap);

                if (BackgroundBitmap != null)
                {
                    _offScreenGraphics.DrawImage(BackgroundBitmap, 0, 0, BackgroundBitmap.Width, BackgroundBitmap.Height);
                }

                DrawCloseButton(_offScreenGraphics);
                DrawText(_offScreenGraphics);

                _grfx.DrawImage(_offscreenBitmap, 0, 0);
            }
        }

        #endregion
    }
}