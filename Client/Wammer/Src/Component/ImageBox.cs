#region

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

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
        private bool m_autoPan;
        private BorderStyle m_borderStyle;
        private int m_gridCellSize;
        private Color m_gridColor;
        private Color m_gridColorAlternate;
        [Category("Property Changed")]
        private ImageBoxGridDisplayMode m_gridDisplayMode;
        private ImageBoxGridScale m_gridScale;
        private Bitmap m_gridTile;
        private Image m_image;
        private InterpolationMode m_interpolationMode;
        private bool m_invertMouse;
        private bool m_isPanning;
        private bool m_sizeToFit;
        private Point m_startMousePosition;
        private Point m_startScrollPosition;
        private TextureBrush m_texture;
        private int m_zoom;
        private int m_zoomIncrement;

        #endregion  Private Member Declarations

        #region  Public Constructors

        public ImageBox()
        {
            InitializeComponent();

            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.StandardDoubleClick, false);

            UpdateStyles();

            BackColor = Color.White;
            AutoSize = true;
            GridScale = ImageBoxGridScale.Small;
            GridDisplayMode = ImageBoxGridDisplayMode.Client;
            GridColor = Color.Gainsboro;
            GridColorAlternate = Color.White;
            GridCellSize = 8;
            BorderStyle = BorderStyle.FixedSingle;
            AutoPan = true;
            Zoom = 100;
            ZoomIncrement = 10;
            InterpolationMode = InterpolationMode.Default;
            AutoCenter = true;
        }

        #endregion  Public Constructors

        #region  Events

        [Category("Property Changed")]
        public event EventHandler AutoCenterChanged;

        [Category("Property Changed")]
        public event EventHandler AutoPanChanged;

        [Category("Property Changed")]
        public event EventHandler BorderStyleChanged;

        [Category("Property Changed")]
        public event EventHandler GridCellSizeChanged;

        [Category("Property Changed")]
        public event EventHandler GridColorAlternateChanged;

        [Category("Property Changed")]
        public event EventHandler GridColorChanged;

        [Category("Property Changed")]
        public event EventHandler GridDisplayModeChanged;

        [Category("Property Changed")]
        public event EventHandler GridScaleChanged;

        [Category("Property Changed")]
        public event EventHandler ImageChanged;

        [Category("Property Changed")]
        public event EventHandler InterpolationModeChanged;

        [Category("Property Changed")]
        public event EventHandler InvertMouseChanged;

        [Category("Property Changed")]
        public event EventHandler PanEnd;

        [Category("Property Changed")]
        public event EventHandler PanStart;

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

                // add an offset based on the border style
                width += GetBorderOffset();
                height += GetBorderOffset();

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

		//protected override void OnKeyDown(KeyEventArgs e)
		//{
		//    base.OnKeyDown(e);

		//    switch (e.KeyCode)
		//    {
		//        case Keys.Left:
		//            AdjustScroll(
		//                -(e.Modifiers == Keys.None ? HorizontalScroll.SmallChange : HorizontalScroll.LargeChange), 0);
		//            break;
		//        case Keys.Right:
		//            AdjustScroll(
		//                e.Modifiers == Keys.None ? HorizontalScroll.SmallChange : HorizontalScroll.LargeChange, 0);
		//            break;
		//        case Keys.Up:
		//            AdjustScroll(0,
		//                         -(e.Modifiers == Keys.None ? VerticalScroll.SmallChange : VerticalScroll.LargeChange));
		//            break;
		//        case Keys.Down:
		//            AdjustScroll(0, e.Modifiers == Keys.None ? VerticalScroll.SmallChange : VerticalScroll.LargeChange);
		//            break;
		//    }
		//}

		//protected override void OnMouseClick(MouseEventArgs e)
		//{
		//    if (!IsPanning && !SizeToFit)
		//    {
		//        if (e.Button == MouseButtons.Left && ModifierKeys == Keys.None)
		//        {
		//            if (Zoom >= 100)
		//                Zoom = (int)Math.Round((double)(Zoom + 100) / 100) * 100;
		//            else if (Zoom >= 75)
		//                Zoom = 100;
		//            else
		//                Zoom = (int)(Zoom / 0.75F);
		//        }
		//        else if (e.Button == MouseButtons.Right || (e.Button == MouseButtons.Left && ModifierKeys != Keys.None))
		//        {
		//            if (Zoom > 100 && Zoom <= 125)
		//                Zoom = 100;
		//            else if (Zoom > 100)
		//                Zoom = (int)Math.Round((double)(Zoom - 100) / 100) * 100;
		//            else
		//                Zoom = (int)(Zoom * 0.75F);
		//        }
		//    }

		//    base.OnMouseClick(e);
		//}

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (!Focused)
                Focus();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.Button == MouseButtons.Left && AutoPan && Image != null)
            {
                if (!IsPanning)
                {
                    m_startMousePosition = e.Location;
                    IsPanning = true;
                }

                if (IsPanning)
                {
                    int x;
                    int y;
                    Point position;

                    if (!InvertMouse)
                    {
                        x = -m_startScrollPosition.X + (m_startMousePosition.X - e.Location.X);
                        y = -m_startScrollPosition.Y + (m_startMousePosition.Y - e.Location.Y);
                    }
                    else
                    {
                        x = -(m_startScrollPosition.X + (m_startMousePosition.X - e.Location.X));
                        y = -(m_startScrollPosition.Y + (m_startMousePosition.Y - e.Location.Y));
                    }

                    position = new Point(x, y);

                    UpdateScrollPosition(position);
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (IsPanning)
                IsPanning = false;
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

            e.Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;

            // draw the borders
            switch (BorderStyle)
            {
                case BorderStyle.FixedSingle:
                    System.Windows.Forms.ControlPaint.DrawBorder(e.Graphics, ClientRectangle, ForeColor, ButtonBorderStyle.Solid);
                    break;
                case BorderStyle.Fixed3D:
                    System.Windows.Forms.ControlPaint.DrawBorder3D(e.Graphics, ClientRectangle, Border3DStyle.Sunken);
                    break;
            }

            innerRectangle = GetInsideViewPort();

            // draw the background
            using (SolidBrush brush = new SolidBrush(BackColor))
                e.Graphics.FillRectangle(brush, innerRectangle);

            if (m_texture != null && GridDisplayMode != ImageBoxGridDisplayMode.None)
            {
                switch (GridDisplayMode)
                {
                    case ImageBoxGridDisplayMode.Image:
                        Rectangle fillRectangle;

                        fillRectangle = GetImageViewPort();
                        e.Graphics.FillRectangle(m_texture, fillRectangle);

                        if (!fillRectangle.Equals(innerRectangle))
                        {
                            fillRectangle.Inflate(1, 1);
                            System.Windows.Forms.ControlPaint.DrawBorder(e.Graphics, fillRectangle, ForeColor, ButtonBorderStyle.Solid);
                        }
                        break;
                    case ImageBoxGridDisplayMode.Client:
                        e.Graphics.FillRectangle(m_texture, innerRectangle);
                        break;
                }
            }

            // draw the image
            if (Image != null)
                DrawImage(e.Graphics);

            base.OnPaint(e);
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
            Rectangle viewPort;

            if (Image != null)
            {
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
            }
            else
                viewPort = Rectangle.Empty;

            return viewPort;
        }

        public Rectangle GetInsideViewPort()
        {
            return GetInsideViewPort(false);
        }

        public virtual Rectangle GetInsideViewPort(bool includePadding)
        {
            int left;
            int top;
            int width;
            int height;
            int borderOffset;

            borderOffset = GetBorderOffset();
            left = borderOffset;
            top = borderOffset;
            width = ClientSize.Width - (borderOffset * 2);
            height = ClientSize.Height - (borderOffset * 2);

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
            int sourceLeft;
            int sourceTop;
            int sourceWidth;
            int sourceHeight;
            Rectangle viewPort;
            Rectangle region;

            if (Image != null)
            {
                viewPort = GetImageViewPort();
                sourceLeft = (int)(-AutoScrollPosition.X / ZoomFactor);
                sourceTop = (int)(-AutoScrollPosition.Y / ZoomFactor);
                sourceWidth = (int)(viewPort.Width / ZoomFactor);
                sourceHeight = (int)(viewPort.Height / ZoomFactor);

                region = new Rectangle(sourceLeft, sourceTop, sourceWidth, sourceHeight);
            }
            else
                region = Rectangle.Empty;

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

        [DefaultValue(true), Category("Behavior")]
        public bool AutoPan
        {
            get { return m_autoPan; }
            set
            {
                if (m_autoPan != value)
                {
                    m_autoPan = value;
                    OnAutoPanChanged(EventArgs.Empty);

                    if (value)
                        SizeToFit = false;
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

        [Category("Appearance"), DefaultValue(typeof(BorderStyle), "FixedSingle")]
        public BorderStyle BorderStyle
        {
            get { return m_borderStyle; }
            set
            {
                if (m_borderStyle != value)
                {
                    m_borderStyle = value;
                    OnBorderStyleChanged(EventArgs.Empty);
                }
            }
        }

        [Category("Appearance"), DefaultValue(8)]
        public int GridCellSize
        {
            get { return m_gridCellSize; }
            set
            {
                if (m_gridCellSize != value)
                {
                    m_gridCellSize = value;
                    OnGridCellSizeChanged(EventArgs.Empty);
                }
            }
        }

        [Category("Appearance"), DefaultValue(typeof(Color), "Gainsboro")]
        public Color GridColor
        {
            get { return m_gridColor; }
            set
            {
                if (m_gridColor != value)
                {
                    m_gridColor = value;
                    OnGridColorChanged(EventArgs.Empty);
                }
            }
        }

        [Category("Appearance"), DefaultValue(typeof(Color), "White")]
        public Color GridColorAlternate
        {
            get { return m_gridColorAlternate; }
            set
            {
                if (m_gridColorAlternate != value)
                {
                    m_gridColorAlternate = value;
                    OnGridColorAlternateChanged(EventArgs.Empty);
                }
            }
        }

        [DefaultValue(ImageBoxGridDisplayMode.Client), Category("Appearance")]
        public ImageBoxGridDisplayMode GridDisplayMode
        {
            get { return m_gridDisplayMode; }
            set
            {
                if (m_gridDisplayMode != value)
                {
                    m_gridDisplayMode = value;
                    OnGridDisplayModeChanged(EventArgs.Empty);
                }
            }
        }

        [DefaultValue(typeof(ImageBoxGridScale), "Small"), Category("Appearance")]
        public ImageBoxGridScale GridScale
        {
            get { return m_gridScale; }
            set
            {
                if (m_gridScale != value)
                {
                    m_gridScale = value;
                    OnGridScaleChanged(EventArgs.Empty);
                }
            }
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

        [DefaultValue(InterpolationMode.Default), Category("Appearance")]
        public InterpolationMode InterpolationMode
        {
            get { return m_interpolationMode; }
            set
            {
                if (value == InterpolationMode.Invalid)
                    value = InterpolationMode.Default;

                if (m_interpolationMode != value)
                {
                    m_interpolationMode = value;
                    OnInterpolationModeChanged(EventArgs.Empty);
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

        [DefaultValue(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public bool IsPanning
        {
            get { return m_isPanning; }
            protected set
            {
                if (m_isPanning != value)
                {
                    m_isPanning = value;
                    m_startScrollPosition = AutoScrollPosition;

                    if (value)
                    {
                        Cursor = Cursors.SizeAll;
                        OnPanStart(EventArgs.Empty);
                    }
                    else
                    {
                        Cursor = Cursors.Default;
                        OnPanEnd(EventArgs.Empty);
                    }
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

                    if (value)
                        AutoPan = false;
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

        #region  Private Methods

        private int GetBorderOffset()
        {
            int offset;

            switch (BorderStyle)
            {
                case BorderStyle.Fixed3D:
                    offset = 2;
                    break;
                case BorderStyle.FixedSingle:
                    offset = 1;
                    break;
                default:
                    offset = 0;
                    break;
            }

            return offset;
        }

        private void InitializeGridTile()
        {
            if (m_texture != null)
                m_texture.Dispose();

            if (m_gridTile != null)
                m_gridTile.Dispose();

            if (GridDisplayMode != ImageBoxGridDisplayMode.None && GridCellSize != 0)
            {
                m_gridTile = CreateGridTileImage(GridCellSize, GridColor, GridColorAlternate);
                m_texture = new TextureBrush(m_gridTile);
            }

            Invalidate();
        }

        #endregion  Private Methods

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

        protected virtual Bitmap CreateGridTileImage(int cellSize, Color firstColor, Color secondColor)
        {
            Bitmap result;
            int width;
            int height;
            float scale;

            // rescale the cell size
            switch (GridScale)
            {
                case ImageBoxGridScale.Medium:
                    scale = 1.5F;
                    break;
                case ImageBoxGridScale.Large:
                    scale = 2;
                    break;
                default:
                    scale = 1;
                    break;
            }

            cellSize = (int)(cellSize * scale);

            // draw the tile
            width = cellSize * 2;
            height = cellSize * 2;
            result = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(result))
            {
                using (SolidBrush brush = new SolidBrush(firstColor))
                    g.FillRectangle(brush, new Rectangle(0, 0, width, height));

                using (SolidBrush brush = new SolidBrush(secondColor))
                {
                    g.FillRectangle(brush, new Rectangle(0, 0, cellSize, cellSize));
                    g.FillRectangle(brush, new Rectangle(cellSize, cellSize, cellSize, cellSize));
                }
            }

            return result;
        }

        protected virtual void DrawImage(Graphics g)
        {
            g.InterpolationMode = InterpolationMode;
            g.DrawImage(Image, GetImageViewPort(), GetSourceImageRegion(), GraphicsUnit.Pixel);
        }

        protected virtual void OnAutoCenterChanged(EventArgs e)
        {
            Invalidate();

            if (AutoCenterChanged != null)
                AutoCenterChanged(this, e);
        }

        protected virtual void OnAutoPanChanged(EventArgs e)
        {
            if (AutoPanChanged != null)
                AutoPanChanged(this, e);
        }

        protected virtual void OnBorderStyleChanged(EventArgs e)
        {
            AdjustLayout();

            if (BorderStyleChanged != null)
                BorderStyleChanged(this, e);
        }

        protected virtual void OnGridCellSizeChanged(EventArgs e)
        {
            InitializeGridTile();

            if (GridCellSizeChanged != null)
                GridCellSizeChanged(this, e);
        }

        protected virtual void OnGridColorAlternateChanged(EventArgs e)
        {
            InitializeGridTile();

            if (GridColorAlternateChanged != null)
                GridColorAlternateChanged(this, e);
        }

        protected virtual void OnGridColorChanged(EventArgs e)
        {
            InitializeGridTile();

            if (GridColorChanged != null)
                GridColorChanged(this, e);
        }

        protected virtual void OnGridDisplayModeChanged(EventArgs e)
        {
            InitializeGridTile();
            Invalidate();

            if (GridDisplayModeChanged != null)
                GridDisplayModeChanged(this, e);
        }

        protected virtual void OnGridScaleChanged(EventArgs e)
        {
            InitializeGridTile();

            if (GridScaleChanged != null)
                GridScaleChanged(this, e);
        }

        protected virtual void OnImageChanged(EventArgs e)
        {
            AdjustLayout();

            if (ImageChanged != null)
                ImageChanged(this, e);
        }

        protected virtual void OnInterpolationModeChanged(EventArgs e)
        {
            Invalidate();

            if (InterpolationModeChanged != null)
                InterpolationModeChanged(this, e);
        }

        protected virtual void OnInvertMouseChanged(EventArgs e)
        {
            if (InvertMouseChanged != null)
                InvertMouseChanged(this, e);
        }

        protected virtual void OnPanEnd(EventArgs e)
        {
            if (PanEnd != null)
                PanEnd(this, e);
        }

        protected virtual void OnPanStart(EventArgs e)
        {
            if (PanStart != null)
                PanStart(this, e);
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