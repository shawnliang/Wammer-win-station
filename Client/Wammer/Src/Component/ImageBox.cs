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
        #region Const
		const int MIN_ZOOM = 10;
        const int MAX_ZOOM = 3500;
        #endregion

        #region  Var
        private bool _autoCenter;
        private Image _image;
        private bool _sizeToFit;
        private int _zoom;
        private int _zoomIncrement;
		private Rectangle? _imageViewPort;
		private Rectangle? _sourceImageRegion;
        #endregion



		#region Private Property
		/// <summary>
		/// Gets or sets the m_image view port.
		/// </summary>
		/// <value>The m_image view port.</value>
		private Rectangle? m_imageViewPort 
		{
			get
			{
 				return _imageViewPort??(_imageViewPort = GetImageViewPort());
			}
			set
			{
				_imageViewPort = value;
			}
		}

		/// <summary>
		/// Gets or sets the m_ source image region.
		/// </summary>
		/// <value>The m_ source image region.</value>
		private Rectangle? m_SourceImageRegion
		{
			get
			{
				return _sourceImageRegion??(_sourceImageRegion = GetSourceImageRegion());
			}
			set
			{
				_sourceImageRegion = value;
			}
		}
		#endregion



		#region Protected Property
		/// <summary>
		/// Gets the height of the scaled image.
		/// </summary>
		/// <value>The height of the scaled image.</value>
		protected virtual int m_ScaledImageHeight
		{
			get { return Image != null ? (int)(Image.Size.Height * m_ZoomFactor) : 0; }
		}

		/// <summary>
		/// Gets the width of the scaled image.
		/// </summary>
		/// <value>The width of the scaled image.</value>
		protected virtual int m_ScaledImageWidth
		{
			get { return Image != null ? (int)(Image.Size.Width * m_ZoomFactor) : 0; }
		}

		/// <summary>
		/// Gets the zoom factor.
		/// </summary>
		/// <value>The zoom factor.</value>
		protected virtual double m_ZoomFactor
		{
			get { return (double)Zoom / 100; }
		}
		#endregion 



		#region Public Property
		/// <summary>
		/// This property is not relevant for this class.
		/// </summary>
		/// <value></value>
		/// <returns>true if enabled; otherwise, false.</returns>
		[Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		 DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), DefaultValue(true)]
		public override bool AutoSize
		{
			get { return base.AutoSize; }
			set
			{
				if (base.AutoSize == value)
					return;

				base.AutoSize = value;
				OnAutoSizeChanged(EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether [auto center].
		/// </summary>
		/// <value><c>true</c> if [auto center]; otherwise, <c>false</c>.</value>
		[DefaultValue(true), Category("Appearance")]
		public bool AutoCenter
		{
			get { return _autoCenter; }
			set
			{
				if (_autoCenter == value)
					return;

				_autoCenter = value;
				OnAutoCenterChanged(EventArgs.Empty);
			}
		}

		[Category("Appearance"), DefaultValue(null)]
		public virtual Image Image
		{
			get { return _image; }
			set
			{
				if (_image == value)
					return;

				_image = value;

				ExifStuff.OrientImage(_image);
				OnImageChanged(EventArgs.Empty);
			}
		}

		[DefaultValue(false), Category("Appearance")]
		public bool SizeToFit
		{
			get { return _sizeToFit; }
			set
			{
				if (_sizeToFit != value)
				{
					_sizeToFit = value;
					OnSizeToFitChanged(EventArgs.Empty);
				}
			}
		}

		[DefaultValue(100), Category("Appearance")]
		public int Zoom
		{
			get { return _zoom; }
			set
			{
				if (value < MIN_ZOOM)
					value = MIN_ZOOM;
				else if (value > MAX_ZOOM)
					value = MAX_ZOOM;

				if (_zoom != value)
				{
					_zoom = value;

					OnZoomChanged(EventArgs.Empty);
				}
			}
		}

		[DefaultValue(20), Category("Behavior")]
		public int ZoomIncrement
		{
			get { return _zoomIncrement; }
			set
			{
				if (_zoomIncrement != value)
				{
					_zoomIncrement = value;
					OnZoomIncrementChanged(EventArgs.Empty);
				}
			}
		}
		#endregion



		#region Events
		public event EventHandler AutoSizeChanged;
		public event EventHandler AutoCenterChanged;
		public event EventHandler ImageChanged;
		public event EventHandler SizeToFitChanged;
		public event EventHandler ZoomChanged;
		public event EventHandler ZoomIncrementChanged;
		#endregion


        #region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="ImageBox"/> class.
		/// </summary>
        public ImageBox()
        {
            InitializeComponent();

			try
			{
				this.SuspendLayout();
				SetStyle(
					ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer |
					ControlStyles.ResizeRedraw, true);
				SetStyle(ControlStyles.StandardDoubleClick, false);

				UpdateStyles();

				AutoSize = true;
				Zoom = 100;
				ZoomIncrement = 10;
				AutoCenter = true;

				this.ZoomChanged += new EventHandler(ImageBox_ZoomChanged);
				this.AutoSizeChanged += new EventHandler(ImageBox_AutoSizeChanged);
				this.ImageChanged += new EventHandler(ImageBox_ImageChanged);
				this.AutoCenterChanged += new EventHandler(ImageBox_AutoCenterChanged);
				this.SizeToFitChanged += new EventHandler(ImageBox_SizeToFitChanged);
				this.BackColorChanged += new EventHandler(ImageBox_BackColorChanged);
			}
			finally
			{
				this.ResumeLayout();
			}
        }
        #endregion




		#region Protected Method
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
			}

			base.Dispose(disposing);
		}

		/// <summary>
		/// Determines whether the specified key is a regular input key or a special key that requires preprocessing.
		/// </summary>
		/// <param name="keyData">One of the <see cref="T:System.Windows.Forms.Keys"/> values.</param>
		/// <returns>
		/// true if the specified key is a regular input key; otherwise, false.
		/// </returns>
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

			m_imageViewPort = null;
			AdjustLayout();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			Rectangle innerRectangle;

			var g = e.Graphics;

			// draw the image
			if (Image != null)
			{
				g.DrawImage(Image, m_imageViewPort.Value, m_SourceImageRegion.Value, GraphicsUnit.Pixel);
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

		public virtual void AdjustLayout()
		{
			if (AutoSize)
				AdjustSize();
			else if (SizeToFit)
				ZoomToFit();
			else if (AutoScroll)
				AdjustViewPort();

			m_imageViewPort = null;
			m_SourceImageRegion = null;
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
				AutoScrollMinSize = new Size(m_ScaledImageWidth + Padding.Horizontal, m_ScaledImageHeight + Padding.Vertical);
		}

		protected virtual void OnAutoSizeChanged(EventArgs e)
		{
			if (AutoSizeChanged == null)
				return;

			AutoSizeChanged(this, e);
		}

		protected virtual void OnAutoCenterChanged(EventArgs e)
		{
			if (AutoCenterChanged != null)
				AutoCenterChanged(this, e);
		}

		protected virtual void OnImageChanged(EventArgs e)
		{
			if (ImageChanged != null)
				ImageChanged(this, e);
		}


		protected virtual void OnSizeToFitChanged(EventArgs e)
		{
			if (SizeToFitChanged != null)
				SizeToFitChanged(this, e);
		}

		protected virtual void OnZoomChanged(EventArgs e)
		{
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
		#endregion




        #region Public Method
		public override Size GetPreferredSize(Size proposedSize)
		{
			Size size;

			if (Image != null)
			{
				int width;
				int height;

				// get the size of the image
				width = m_ScaledImageWidth;
				height = m_ScaledImageHeight;

				// add an offset based on padding
				width += Padding.Horizontal;
				height += Padding.Vertical;

				size = new Size(width, height);
			}
			else
				size = base.GetPreferredSize(proposedSize);

			return size;
		}

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

				x = !HScroll ? (innerRectangle.Width - (m_ScaledImageWidth + Padding.Horizontal)) / 2 : 0;
				y = !VScroll ? (innerRectangle.Height - (m_ScaledImageHeight + Padding.Vertical)) / 2 : 0;

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
			sourceLeft = (int)(-AutoScrollPosition.X / m_ZoomFactor);
			sourceTop = (int)(-AutoScrollPosition.Y / m_ZoomFactor);
			sourceWidth = (int)(viewPort.Width / m_ZoomFactor);
			sourceHeight = (int)(viewPort.Height / m_ZoomFactor);

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
        #endregion






		#region Event Process
		/// <summary>
		/// Handles the ZoomChanged event of the ImageBox control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void ImageBox_ZoomChanged(object sender, EventArgs e)
		{
			AdjustLayout();
		}

		/// <summary>
		/// Handles the AutoSizeChanged event of the ImageBox control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void ImageBox_AutoSizeChanged(object sender, EventArgs e)
		{
			AdjustLayout();
		}

		/// <summary>
		/// Handles the ImageChanged event of the ImageBox control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void ImageBox_ImageChanged(object sender, EventArgs e)
		{
			AdjustLayout();
		}

		/// <summary>
		/// Handles the AutoCenterChanged event of the ImageBox control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void ImageBox_AutoCenterChanged(object sender, EventArgs e)
		{
			m_imageViewPort = null;
			m_SourceImageRegion = null;
			Invalidate();
		}

		/// <summary>
		/// Handles the SizeToFitChanged event of the ImageBox control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void ImageBox_SizeToFitChanged(object sender, EventArgs e)
		{
			AdjustLayout();
		}

		/// <summary>
		/// Handles the BackColorChanged event of the ImageBox control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void ImageBox_BackColorChanged(object sender, EventArgs e)
		{
			Invalidate();
		}
		#endregion
	}
}