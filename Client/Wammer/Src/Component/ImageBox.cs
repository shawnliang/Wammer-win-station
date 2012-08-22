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

		/// <summary>
		/// Gets or sets the image.
		/// </summary>
		/// <value>The image.</value>
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

		/// <summary>
		/// Gets or sets a value indicating whether [size to fit].
		/// </summary>
		/// <value><c>true</c> if [size to fit]; otherwise, <c>false</c>.</value>
		[DefaultValue(false), Category("Appearance")]
		public bool SizeToFit
		{
			get { return _sizeToFit; }
			set
			{
				if (_sizeToFit == value)
					return;

				_sizeToFit = value;
				OnSizeToFitChanged(EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets or sets the zoom.
		/// </summary>
		/// <value>The zoom.</value>
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

				if (_zoom == value)
					return;
				_zoom = value;
				OnZoomChanged(EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets or sets the zoom increment.
		/// </summary>
		/// <value>The zoom increment.</value>
		[DefaultValue(20), Category("Behavior")]
		public int ZoomIncrement
		{
			get { return _zoomIncrement; }
			set
			{
				if (_zoomIncrement == value)
					return;
				_zoomIncrement = value;
				OnZoomIncrementChanged(EventArgs.Empty);
			}
		}
		#endregion



		#region Events
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

				Zoom = 100;
				ZoomIncrement = 10;
				AutoCenter = true;

				this.ZoomChanged += new EventHandler(ImageBox_ZoomChanged);
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

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.MouseDown"/> event.
		/// </summary>
		/// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the event data.</param>
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);

			if (!Focused)
				Focus();
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.MouseWheel"/> event.
		/// </summary>
		/// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the event data.</param>
		protected override void OnMouseWheel(MouseEventArgs e)
		{
			if (!SizeToFit)
			{
				int increment = ZoomIncrement;

				if (e.Delta < 0)
					increment = -increment;

				Zoom += increment;
			}
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.PaddingChanged"/> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
		protected override void OnPaddingChanged(EventArgs e)
		{
			base.OnPaddingChanged(e);
			AdjustLayout();
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.Paint"/> event.
		/// </summary>
		/// <param name="e">A <see cref="T:System.Windows.Forms.PaintEventArgs"/> that contains the event data.</param>
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


		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.Resize"/> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
		protected override void OnResize(EventArgs e)
		{
			AdjustLayout();

			base.OnResize(e);
		}

		/// <summary>
		/// Adjusts the layout.
		/// </summary>
		public virtual void AdjustLayout()
		{
			if (SizeToFit)
				ZoomToFit();
			else if (AutoScroll)
				AdjustViewPort();

			m_imageViewPort = null;
			m_SourceImageRegion = null;
			Invalidate();
		}

		/// <summary>
		/// Adjusts the view port.
		/// </summary>
		protected virtual void AdjustViewPort()
		{
			if (AutoScroll && Image != null)
				AutoScrollMinSize = new Size(m_ScaledImageWidth + Padding.Horizontal, m_ScaledImageHeight + Padding.Vertical);
		}


		/// <summary>
		/// Raises the <see cref="E:AutoCenterChanged"/> event.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected virtual void OnAutoCenterChanged(EventArgs e)
		{
			if (AutoCenterChanged != null)
				AutoCenterChanged(this, e);
		}

		/// <summary>
		/// Raises the <see cref="E:ImageChanged"/> event.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected virtual void OnImageChanged(EventArgs e)
		{
			if (ImageChanged != null)
				ImageChanged(this, e);
		}


		/// <summary>
		/// Raises the <see cref="E:SizeToFitChanged"/> event.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected virtual void OnSizeToFitChanged(EventArgs e)
		{
			if (SizeToFitChanged != null)
				SizeToFitChanged(this, e);
		}

		/// <summary>
		/// Raises the <see cref="E:ZoomChanged"/> event.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected virtual void OnZoomChanged(EventArgs e)
		{
			if (ZoomChanged != null)
				ZoomChanged(this, e);
		}

		/// <summary>
		/// Raises the <see cref="E:ZoomIncrementChanged"/> event.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected virtual void OnZoomIncrementChanged(EventArgs e)
		{
			if (ZoomIncrementChanged != null)
				ZoomIncrementChanged(this, e);
		}
		#endregion




        #region Public Method
		/// <summary>
		/// Gets the image view port.
		/// </summary>
		/// <returns></returns>
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


		/// <summary>
		/// Gets the inside view port.
		/// </summary>
		/// <param name="includePadding">if set to <c>true</c> [include padding].</param>
		/// <returns></returns>
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

		/// <summary>
		/// Gets the source image region.
		/// </summary>
		/// <returns></returns>
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

		/// <summary>
		/// Zooms to fit.
		/// </summary>
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