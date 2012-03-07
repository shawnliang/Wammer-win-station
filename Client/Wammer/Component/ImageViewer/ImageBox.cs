#region

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#endregion

namespace Waveface.Windows.Forms
{
    [DefaultProperty("Image")]
    public partial class ImageBox : ScrollableControl
    {
        #region  Private Member Declarations

        private bool _autoPan;
        private BorderStyle _borderStyle;
        private int _gridCellSize;
        private Color _gridColor;
        private Color _gridColorAlternate;
        private ImageBoxGridScale _gridScale;
        private Bitmap _gridTile;
        private Image _image;
        private bool _invertMouse;
        private bool _isPanning;
        private bool _showGrid;
        private Point _startMousePosition;
        private Point _startScrollPosition;
        private TextureBrush _texture;

        #endregion  Private Member Declarations

        #region  Public Constructors

        public ImageBox()
        {
            InitializeComponent();

            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw, true);
            UpdateStyles();

            BackColor = Color.White;
            AutoSize = true;
            GridScale = ImageBoxGridScale.Small;
            ShowGrid = true;
            GridColor = Color.Gainsboro;
            GridColorAlternate = Color.White;
            GridCellSize = 8;
            BorderStyle = BorderStyle.FixedSingle;
            AutoPan = true;
        }

        #endregion  Public Constructors

        #region  Events

        public event EventHandler AutoPanChanged;

        public event EventHandler BorderStyleChanged;

        public event EventHandler GridCellSizeChanged;

        public event EventHandler GridColorAlternateChanged;

        public event EventHandler GridColorChanged;

        public event EventHandler GridScaleChanged;

        public event EventHandler ImageChanged;

        public event EventHandler InvertMouseChanged;

        public event EventHandler PanEnd;

        public event EventHandler PanStart;

        public event EventHandler ShowGridChanged;

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
                // get the size of the image
                int width = Image.Size.Width;
                int height = Image.Size.Height;

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

                if (_texture != null)
                {
                    _texture.Dispose();
                    _texture = null;
                }

                if (_gridTile != null)
                {
                    _gridTile.Dispose();
                    _gridTile = null;
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

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            switch (e.KeyCode)
            {
                case Keys.Left:
                    AdjustScroll(
                        -(e.Modifiers == Keys.None ? HorizontalScroll.SmallChange : HorizontalScroll.LargeChange), 0);
                    break;
                case Keys.Right:
                    AdjustScroll(
                        e.Modifiers == Keys.None ? HorizontalScroll.SmallChange : HorizontalScroll.LargeChange, 0);
                    break;
                case Keys.Up:
                    AdjustScroll(0,
                                 -(e.Modifiers == Keys.None ? VerticalScroll.SmallChange : VerticalScroll.LargeChange));
                    break;
                case Keys.Down:
                    AdjustScroll(0, e.Modifiers == Keys.None ? VerticalScroll.SmallChange : VerticalScroll.LargeChange);
                    break;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.Button == MouseButtons.Left && AutoPan && Image != null)
            {
                if (!IsPanning)
                {
                    _startMousePosition = e.Location;
                    IsPanning = true;
                }

                if (IsPanning)
                {
                    int x;
                    int y;

                    if (!InvertMouse)
                    {
                        x = -_startScrollPosition.X + (_startMousePosition.X - e.Location.X);
                        y = -_startScrollPosition.Y + (_startMousePosition.Y - e.Location.Y);
                    }
                    else
                    {
                        x = -(_startScrollPosition.X + (_startMousePosition.X - e.Location.X));
                        y = -(_startScrollPosition.Y + (_startMousePosition.Y - e.Location.Y));
                    }

                    Point position = new Point(x, y);

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

        protected override void OnPaddingChanged(EventArgs e)
        {
            base.OnPaddingChanged(e);
            AdjustLayout();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Rectangle innerRectangle;

            int borderOffset = GetBorderOffset();

            if (borderOffset != 0)
            {
                // draw the borders
                switch (BorderStyle)
                {
                    case BorderStyle.FixedSingle:
                        ControlPaint.DrawBorder(e.Graphics, ClientRectangle, ForeColor, ButtonBorderStyle.Solid);
                        break;
                    case BorderStyle.Fixed3D:
                        ControlPaint.DrawBorder3D(e.Graphics, ClientRectangle, Border3DStyle.Sunken);
                        break;
                }

                // clip the background so we don't overwrite the border
                innerRectangle = Rectangle.Inflate(ClientRectangle, -borderOffset, -borderOffset);
                e.Graphics.SetClip(innerRectangle);
            }
            else
                innerRectangle = ClientRectangle;


            // draw the background
            if (_texture != null && ShowGrid)
                e.Graphics.FillRectangle(_texture, innerRectangle);
            else
            {
                using (SolidBrush brush = new SolidBrush(BackColor))
                    e.Graphics.FillRectangle(brush, innerRectangle);
            }

            // draw the image
            if (Image != null)
            {
                int left = Padding.Left + borderOffset;
                int top = Padding.Top + borderOffset;

                if (AutoScroll)
                {
                    left += AutoScrollPosition.X;
                    top += AutoScrollPosition.Y;
                }

                e.Graphics.DrawImageUnscaled(Image, new Point(left, top));
            }

            // reset the clipping
            if (borderOffset != 0)
                e.Graphics.ResetClip();
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

        #region  Public Properties

        [DefaultValue(true), Category("Behavior")]
        public bool AutoPan
        {
            get { return _autoPan; }
            set
            {
                if (_autoPan != value)
                {
                    _autoPan = value;
                    OnAutoPanChanged(EventArgs.Empty);
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
            get { return _borderStyle; }
            set
            {
                if (_borderStyle != value)
                {
                    _borderStyle = value;
                    OnBorderStyleChanged(EventArgs.Empty);
                }
            }
        }

        [Category("Appearance"), DefaultValue(8)]
        public int GridCellSize
        {
            get { return _gridCellSize; }
            set
            {
                if (_gridCellSize != value)
                {
                    _gridCellSize = value;
                    OnGridCellSizeChanged(EventArgs.Empty);
                }
            }
        }

        [Category("Appearance"), DefaultValue(typeof(Color), "Gainsboro")]
        public Color GridColor
        {
            get { return _gridColor; }
            set
            {
                if (_gridColor != value)
                {
                    _gridColor = value;
                    OnGridColorChanged(EventArgs.Empty);
                }
            }
        }

        [Category("Appearance"), DefaultValue(typeof(Color), "White")]
        public Color GridColorAlternate
        {
            get { return _gridColorAlternate; }
            set
            {
                if (_gridColorAlternate != value)
                {
                    _gridColorAlternate = value;
                    OnGridColorAlternateChanged(EventArgs.Empty);
                }
            }
        }

        [DefaultValue(typeof(ImageBoxGridScale), "Small"), Category("Appearance")]
        public ImageBoxGridScale GridScale
        {
            get { return _gridScale; }
            set
            {
                if (_gridScale != value)
                {
                    _gridScale = value;
                    OnGridScaleChanged(EventArgs.Empty);
                }
            }
        }

        [Category("Appearance"), DefaultValue(null)]
        public Image Image
        {
            get { return _image; }
            set
            {
                if (_image != value)
                {
                    _image = value;
                    OnImageChanged(EventArgs.Empty);
                }
            }
        }

        [DefaultValue(false), Category("Behavior")]
        public bool InvertMouse
        {
            get { return _invertMouse; }
            set
            {
                if (_invertMouse != value)
                {
                    _invertMouse = value;
                    OnInvertMouseChanged(EventArgs.Empty);
                }
            }
        }

        [DefaultValue(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public bool IsPanning
        {
            get { return _isPanning; }
            protected set
            {
                if (_isPanning != value)
                {
                    _isPanning = value;
                    _startScrollPosition = AutoScrollPosition;

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

        [DefaultValue(true), Category("Appearance")]
        public bool ShowGrid
        {
            get { return _showGrid; }
            set
            {
                if (_showGrid != value)
                {
                    _showGrid = value;
                    OnShowGridChanged(EventArgs.Empty);
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
            if (_texture != null)
                _texture.Dispose();

            if (_gridTile != null)
                _gridTile.Dispose();

            if (ShowGrid && GridCellSize != 0)
            {
                _gridTile = CreateGridTileImage(GridCellSize, GridColor, GridColorAlternate);
                _texture = new TextureBrush(_gridTile);
            }

            Invalidate();
        }

        #endregion  Private Methods

        #region  Protected Methods

        protected virtual void AdjustLayout()
        {
            if (AutoSize)
                AdjustSize();
            else if (AutoScroll)
                AdjustViewPort();
        }

        protected virtual void AdjustScroll(int x, int y)
        {
            Point scrollPosition = new Point(HorizontalScroll.Value + x, VerticalScroll.Value + y);

            UpdateScrollPosition(scrollPosition);
        }

        protected virtual void AdjustSize()
        {
            if (AutoSize && Dock == DockStyle.None)
                Size = PreferredSize;
        }

        protected virtual void AdjustViewPort()
        {
            if (AutoScroll && Image != null)
                AutoScrollMinSize = Image.Size;
        }

        protected virtual Bitmap CreateGridTileImage(int cellSize, Color firstColor, Color secondColor)
        {
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
            int width = cellSize * 2;
            int height = cellSize * 2;
            Bitmap result = new Bitmap(width, height);
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

        protected virtual void OnAutoPanChanged(EventArgs e)
        {
            if (AutoPanChanged != null)
                AutoPanChanged(this, e);
        }

        protected virtual void OnBorderStyleChanged(EventArgs e)
        {
            AdjustLayout();
            Invalidate();

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

        protected virtual void OnGridScaleChanged(EventArgs e)
        {
            InitializeGridTile();

            if (GridScaleChanged != null)
                GridScaleChanged(this, e);
        }

        protected virtual void OnImageChanged(EventArgs e)
        {
            AdjustLayout();
            Invalidate();

            if (ImageChanged != null)
                ImageChanged(this, e);
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

        protected virtual void OnShowGridChanged(EventArgs e)
        {
            InitializeGridTile();

            if (ShowGridChanged != null)
                ShowGridChanged(this, e);
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