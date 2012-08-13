#region

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using System.Diagnostics;

#endregion

namespace Waveface.Component
{
    public enum ImageBoxGridDisplayMode
    {
        None,
        Client,
        Image
    }

    public enum ImageBoxGridScale
    {
        Small,
        Medium,
        Large
    }

    public partial class ImageBox : ScrollableControl
    {
        #region  Private Class Member Declarations

        private static readonly int MinZoom = 10;
        private static readonly int MaxZoom = 3500;

        #endregion  Private Class Member Declarations

        #region  Private Member Declarations

        private bool m_autoCenter;
        private int m_gridCellSize;
        private Color m_gridColor;
        private Color m_gridColorAlternate;
        [Category("Property Changed")]
        private ImageBoxGridDisplayMode m_gridDisplayMode;
        private ImageBoxGridScale m_gridScale;
        private Bitmap m_gridTile;
        private Image m_image;
        private bool m_invertMouse;
        private bool m_sizeToFit;
        private TextureBrush m_texture;
        private int m_zoom;
        private int m_zoomIncrement;

        #endregion  Private Member Declarations

        #region Constructor
        public ImageBox()
        {
            InitializeComponent();

            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.StandardDoubleClick, false);

            UpdateStyles();

            AutoSize = true;
            Zoom = 100;
            ZoomIncrement = 10;
            AutoCenter = true;
        }
        #endregion


        #region  Events

        [Category("Property Changed")]
        public event EventHandler AutoCenterChanged;



        [Category("Property Changed")]
        public event EventHandler ImageChanged;


        [Category("Property Changed")]
        public event EventHandler InvertMouseChanged;


        [Category("Property Changed")]
        public event EventHandler SizeToFitChanged;

        [Category("Property Changed")]
        public event EventHandler ZoomChanged;

        [Category("Property Changed")]
        public event EventHandler ZoomIncrementChanged;

        #endregion  Events

        #region  Overriden Properties

        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), DefaultValue(true)]
        public override bool AutoSize
        {
            get { return base.AutoSize; }
            set
            {
                if (base.AutoSize != value)
                {
                    base.AutoSize = value;

                    AdjustLayout();
                }
            }
        }

        [DefaultValue(typeof(Color), "White")]
        public override Color BackColor
        {
            get { return base.BackColor; }
            set { base.BackColor = value; }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Image BackgroundImage
        {
            get { return base.BackgroundImage; }
            set { base.BackgroundImage = value; }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override ImageLayout BackgroundImageLayout
        {
            get { return base.BackgroundImageLayout; }
            set { base.BackgroundImageLayout = value; }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Font Font
        {
            get { return base.Font; }
            set { base.Font = value; }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        #endregion  Overriden Properties

        #region  Public Overridden Methods

        public override Size GetPreferredSize(Size proposedSize)
        {
            Size size;

            if (Image != null)
            {
                int width;
                int height;

                // get the size of the image
                width = ScaledImageWidth;
                height = ScaledImageHeight;

                // add an offset based on padding
                width += Padding.Horizontal;
                height += Padding.Vertical;

				//// add an offset based on the border style
				//width += GetBorderOffset();
				//height += GetBorderOffset();

                size = new Size(width, height);
            }
            else
                size = base.GetPreferredSize(proposedSize);

            return size;
        }

        #endregion  Public Overridden Methods

        #region  Protected Overridden Methods

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                    components.Dispose();

                if (m_texture != null)
                {
                    m_texture.Dispose();
                    m_texture = null;
                }

                if (m_gridTile != null)
                {
                    m_gridTile.Dispose();
                    m_gridTile = null;
                }
            }

            base.Dispose(disposing);
        }

        protected override bool IsInputKey(Keys keyData)
        {
            bool result;

            if ((keyData & Keys.Right) == Keys.Right | (keyData & Keys.Left) == Keys.Left |
                (keyData & Keys.Up) == Keys.Up | (keyData & Keys.Down) == Keys.Down)
                result = true;
            else
                result = base.IsInputKey(keyData);

            return result;
        }

        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);

            Invalidate();
        }

        protected override void OnDockChanged(EventArgs e)
        {
            base.OnDockChanged(e);

            if (Dock != DockStyle.None)
                AutoSize = false;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (!Focused)
                Focus();
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (!SizeToFit)
            {
                int increment;

                if (ModifierKeys == Keys.None)
                    increment = ZoomIncrement;
                else
                    increment = ZoomIncrement * 5;

                if (e.Delta < 0)
                    increment = -increment;

                Zoom += increment;
            }
        }

        protected override void OnPaddingChanged(EventArgs e)
        {
            base.OnPaddingChanged(e);
			AdjustLayout();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Rectangle innerRectangle;

			var g = e.Graphics;
			g.Clear(BackColor);

			// draw the image
			if (Image != null)
			{
				g.DrawImage(Image, GetImageViewPort(), GetSourceImageRegion(), GraphicsUnit.Pixel);
			}
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);

            AdjustLayout();
        }

        protected override void OnResize(EventArgs e)
        {
            AdjustLayout();

            base.OnResize(e);
        }

        protected override void OnScroll(ScrollEventArgs se)
        {
            Invalidate();

            base.OnScroll(se);
        }

        #endregion  Protected Overridden Methods

        #region  Public Methods

		public virtual Rectangle GetImageViewPort()
		{
			if (Image == null)
				return Rectangle.Empty;

			Rectangle viewPort;
			Rectangle innerRectangle;
			Point offset;

			innerRectangle = GetInsideViewPort();

			if (AutoCenter)
			{
				int x;
				int y;

				x = !HScroll ? (innerRectangle.Width - (ScaledImageWidth + Padding.Horizontal)) / 2 : 0;
				y = !VScroll ? (innerRectangle.Height - (ScaledImageHeight + Padding.Vertical)) / 2 : 0;

				offset = new Point(x, y);
			}
			else
				offset = Point.Empty;

			viewPort = new Rectangle(offset.X + innerRectangle.Left + Padding.Left,
									 offset.Y + innerRectangle.Top + Padding.Top,
									 innerRectangle.Width - (Padding.Horizontal + (offset.X * 2)),
									 innerRectangle.Height - (Padding.Vertical + (offset.Y * 2)));

			return viewPort;
		}


        public virtual Rectangle GetInsideViewPort(bool includePadding = false)
        {
            int left = 0;
            int top = 0;
			int width = ClientSize.Width;
			int height = ClientSize.Height;

            if (includePadding)
            {
                left += Padding.Left;
                top += Padding.Top;
                width -= Padding.Horizontal;
                height -= Padding.Vertical;
            }

            return new Rectangle(left, top, width, height);
        }

		public virtual Rectangle GetSourceImageRegion()
		{
			if (Image == null)
				return Rectangle.Empty;

			int sourceLeft;
			int sourceTop;
			int sourceWidth;
			int sourceHeight;
			Rectangle viewPort;
			Rectangle region;

			viewPort = GetImageViewPort();
			sourceLeft = (int)(-AutoScrollPosition.X / ZoomFactor);
			sourceTop = (int)(-AutoScrollPosition.Y / ZoomFactor);
			sourceWidth = (int)(viewPort.Width / ZoomFactor);
			sourceHeight = (int)(viewPort.Height / ZoomFactor);

			region = new Rectangle(sourceLeft, sourceTop, sourceWidth, sourceHeight);

			return region;
		}

        public virtual void ZoomToFit()
        {
            if (Image != null)
            {
                Rectangle innerRectangle;
                double zoom;
                double aspectRatio;

                AutoScrollMinSize = Size.Empty;

                innerRectangle = GetInsideViewPort(true);

                if (Image.Width > Image.Height)
                {
                    aspectRatio = (innerRectangle.Width) / ((double)Image.Width);
                    zoom = aspectRatio * 100.0;

                    if (innerRectangle.Height < ((Image.Height * zoom) / 100.0))
                    {
                        aspectRatio = (innerRectangle.Height) / ((double)Image.Height);
                        zoom = aspectRatio * 100.0;
                    }
                }
                else
                {
                    aspectRatio = (innerRectangle.Height) / ((double)Image.Height);
                    zoom = aspectRatio * 100.0;

                    if (innerRectangle.Width < ((Image.Width * zoom) / 100.0))
                    {
                        aspectRatio = (innerRectangle.Width) / ((double)Image.Width);
                        zoom = aspectRatio * 100.0;
                    }
                }

                Zoom = (int)Math.Round(Math.Floor(zoom));
            }
        }

        #endregion  Public Methods

        #region  Public Properties

        [DefaultValue(true), Category("Appearance")]
        public bool AutoCenter
        {
            get { return m_autoCenter; }
            set
            {
                if (m_autoCenter != value)
                {
                    m_autoCenter = value;
                    OnAutoCenterChanged(EventArgs.Empty);
                }
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new Size AutoScrollMinSize
        {
            get { return base.AutoScrollMinSize; }
            set { base.AutoScrollMinSize = value; }
        }
        [Category("Appearance"), DefaultValue(null)]
        public virtual Image Image
        {
            get { return m_image; }
            set
            {
                if (m_image != value)
                {
                    m_image = value;

                    ExifStuff.OrientImage(m_image);

                    OnImageChanged(EventArgs.Empty);
                }
            }
        }

        [DefaultValue(false), Category("Behavior")]
        public bool InvertMouse
        {
            get { return m_invertMouse; }
            set
            {
                if (m_invertMouse != value)
                {
                    m_invertMouse = value;
                    OnInvertMouseChanged(EventArgs.Empty);
                }
            }
        }

        [DefaultValue(false), Category("Appearance")]
        public bool SizeToFit
        {
            get { return m_sizeToFit; }
            set
            {
                if (m_sizeToFit != value)
                {
                    m_sizeToFit = value;
                    OnSizeToFitChanged(EventArgs.Empty);
                }
            }
        }

        [DefaultValue(100), Category("Appearance")]
        public int Zoom
        {
            get { return m_zoom; }
            set
            {
                if (value < MinZoom)
                    value = MinZoom;
                else if (value > MaxZoom)
                    value = MaxZoom;

                if (m_zoom != value)
                {
                    m_zoom = value;

                    OnZoomChanged(EventArgs.Empty);
                }
            }
        }

        [DefaultValue(20), Category("Behavior")]
        public int ZoomIncrement
        {
            get { return m_zoomIncrement; }
            set
            {
                if (m_zoomIncrement != value)
                {
                    m_zoomIncrement = value;
                    OnZoomIncrementChanged(EventArgs.Empty);
                }
            }
        }

        #endregion  Public Properties


        #region  Protected Properties

        protected virtual int ScaledImageHeight
        {
            get { return Image != null ? (int)(Image.Size.Height * ZoomFactor) : 0; }
        }

        protected virtual int ScaledImageWidth
        {
            get { return Image != null ? (int)(Image.Size.Width * ZoomFactor) : 0; }
        }

        protected virtual double ZoomFactor
        {
            get { return (double)Zoom / 100; }
        }

        #endregion  Protected Properties

        #region  Protected Methods

        public virtual void AdjustLayout()
        {
            if (AutoSize)
                AdjustSize();
            else if (SizeToFit)
                ZoomToFit();
            else if (AutoScroll)
                AdjustViewPort();

            Invalidate();
        }

        protected virtual void AdjustScroll(int x, int y)
        {
            Point scrollPosition;

            scrollPosition = new Point(HorizontalScroll.Value + x, VerticalScroll.Value + y);

            UpdateScrollPosition(scrollPosition);
        }

        protected virtual void AdjustSize()
        {
            if (AutoSize && Dock == DockStyle.None)
                base.Size = base.PreferredSize;
        }

        protected virtual void AdjustViewPort()
        {
            if (AutoScroll && Image != null)
                AutoScrollMinSize = new Size(ScaledImageWidth + Padding.Horizontal, ScaledImageHeight + Padding.Vertical);
        }


        protected virtual void OnAutoCenterChanged(EventArgs e)
        {
            Invalidate();

            if (AutoCenterChanged != null)
                AutoCenterChanged(this, e);
        }

        protected virtual void OnImageChanged(EventArgs e)
        {
            AdjustLayout();

            if (ImageChanged != null)
                ImageChanged(this, e);
        }

        protected virtual void OnInvertMouseChanged(EventArgs e)
        {
            if (InvertMouseChanged != null)
                InvertMouseChanged(this, e);
        }


        protected virtual void OnSizeToFitChanged(EventArgs e)
        {
            AdjustLayout();

            if (SizeToFitChanged != null)
                SizeToFitChanged(this, e);
        }

        protected virtual void OnZoomChanged(EventArgs e)
        {
            AdjustLayout();

            if (ZoomChanged != null)
                ZoomChanged(this, e);
        }

        protected virtual void OnZoomIncrementChanged(EventArgs e)
        {
            if (ZoomIncrementChanged != null)
                ZoomIncrementChanged(this, e);
        }

		protected virtual void UpdateScrollPosition(Point position)
		{
			AutoScrollPosition = position;
			Invalidate();
			OnScroll(new ScrollEventArgs(ScrollEventType.ThumbPosition, 0));
		}

        #endregion  Protected Methods
    }
}