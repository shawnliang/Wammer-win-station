using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Waveface
{
	public partial class PhotoBox : UserControl
	{
		#region Var
		private Image _image;
		private Rectangle _displayArea;
		private Image _displayImage;
		#endregion


		#region Private Property
		/// <summary>
		/// Gets or sets the m_ click X.
		/// </summary>
		/// <value>The m_ click X.</value>
		private int m_ClickX { get; set; }

		/// <summary>
		/// Gets or sets the m_ click Y.
		/// </summary>
		/// <value>The m_ click Y.</value>
		private int m_ClickY { get; set; }

		/// <summary>
		/// Gets or sets the display image.
		/// </summary>
		/// <value>The display image.</value>
		private Image m_DisplayImage
		{
			get
			{
				if (_displayImage == null)
				{
					if (Image == null)
						return null;

					if (DisplayArea.Size.IsEmpty)
						return null;

					try
					{
						var image = new Bitmap(DisplayRectangle.Width, DisplayRectangle.Height);

						var displayRectangle = this.DisplayRectangle;
						if (Image.Width > Image.Height)
						{
							//橫的圖
							displayRectangle.Width = image.Width;
							displayRectangle.Height = Image.Height * displayRectangle.Width / Image.Width;
						}
						else
						{
							displayRectangle.Width = Image.Width * displayRectangle.Height / Image.Height;
							displayRectangle.Height = image.Height;
						}

						using (var g = Graphics.FromImage(image))
						{
							g.DrawImage(Image, displayRectangle, DisplayArea, GraphicsUnit.Pixel);
						}
						_displayImage = image;
					}
					catch
					{
					}
				}
				return _displayImage;
			}
			set
			{
				_displayImage = value;
			}
		}
		#endregion


		#region Public Property
		/// <summary>
		/// Gets or sets the image.
		/// </summary>
		/// <value>The image.</value>
		public Image Image
		{
			get
			{
				return _image;
			}
			set
			{
				if (_image == value)
					return;

				_image = value;


				RestoreSize();

				Invalidate();
			}
		}

		/// <summary>
		/// Gets or sets the display area.
		/// </summary>
		/// <value>The display area.</value>
		public Rectangle DisplayArea
		{
			get
			{
				return _displayArea;
			}
			set
			{
				_displayArea = value;
				m_DisplayImage = null;
				Invalidate();
			}
		}
		#endregion


		#region Constructor
		public PhotoBox()
		{
			this.SizeChanged += new EventHandler(PhotoBox_SizeChanged);
			this.MouseDown += new MouseEventHandler(PhotoBox_MouseDown);
			this.MouseMove += new MouseEventHandler(PhotoBox_MouseMove);
			this.MouseWheel += new MouseEventHandler(PhotoBox_MouseWheel);

			SetStyle(ControlStyles.DoubleBuffer, true);
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
		}
		#endregion


		#region Protected Method
		protected override void OnPaint(PaintEventArgs e)
		{
			if (Image == null)
				return;

			if (m_DisplayImage == null)
				return;

			var g = e.Graphics;

			//var displayRectangle = this.DisplayRectangle;
			//if (m_DisplayImage.Width > m_DisplayImage.Height)
			//{
			//    //橫的圖
			//    displayRectangle.Width = m_DisplayImage.Width;
			//    displayRectangle.Height = m_DisplayImage.Height * (displayRectangle.Width / m_DisplayImage.Width);
			//}
			//else
			//{	
			//    displayRectangle.Width = m_DisplayImage.Width * (displayRectangle.Height / m_DisplayImage.Height);
			//    displayRectangle.Height = m_DisplayImage.Height;
			//}

			g.DrawImage(this.m_DisplayImage, DisplayRectangle);
		}

		void PhotoBox_SizeChanged(object sender, EventArgs e)
		{
			m_DisplayImage = null;
			Invalidate();
		}
		#endregion


		#region Public Method
		/// <summary>
		/// Zooms the in.
		/// </summary>
		public void ZoomIn()
		{
			ZoomIn(2);
		}

		/// <summary>
		/// Zooms the in.
		/// </summary>
		/// <param name="ratio">The ratio.</param>
		public void ZoomIn(float ratio)
		{
			try
			{
				var width = (int)(DisplayArea.Width / ratio);
				var height = (int)(DisplayArea.Height / ratio);

				if (width <= 0 || height <= 0)
					return;

				DisplayArea = new Rectangle(
					(int)(DisplayArea.Left + DisplayArea.Width / ratio / 2),
					(int)(DisplayArea.Top + DisplayArea.Height / ratio / 2),
					width,
					height);
			}
			catch
			{
			}
		}

		/// <summary>
		/// Zooms the out.
		/// </summary>
		public void ZoomOut()
		{
			ZoomOut(2);
		}

		/// <summary>
		/// Zooms the out.
		/// </summary>
		/// <param name="ratio">The ratio.</param>
		public void ZoomOut(float ratio)
		{
			try
			{
				var width = (int)(DisplayArea.Width * ratio);
				var height = (int)(DisplayArea.Height * ratio);

				if (width <= 0 || height <= 0)
					return;

				DisplayArea = new Rectangle(
					(int)(DisplayArea.Left - DisplayArea.Width / 2),
					(int)(DisplayArea.Top - DisplayArea.Height / 2),
					width,
					height);
			}
			catch
			{
			}
		}


		/// <summary>
		/// Restores the size.
		/// </summary>
		public void RestoreSize()
		{
			if (_image == null)
				return;
			DisplayArea = new Rectangle(0, 0, Image.Width, Image.Height);
		}
		#endregion


		#region Event Process
		/// <summary>
		/// Handles the MouseDown event of the PhotoBox control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
		void PhotoBox_MouseDown(object sender, MouseEventArgs e)
		{
			if (this.Capture && e.Button == MouseButtons.Left)
			{
				if (Image == null)
					return;

				if (DisplayArea == new Rectangle(0, 0, Image.Width, Image.Height))
					return;

				m_ClickX = e.X;
				m_ClickY = e.Y;
			}
		}

		/// <summary>
		/// Handles the MouseMove event of the PhotoBox control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
		void PhotoBox_MouseMove(object sender, MouseEventArgs e)
		{
			if (this.Capture && e.Button == MouseButtons.Left)
			{
				if (Image == null)
					return;

				if (DisplayArea == new Rectangle(0, 0, Image.Width, Image.Height))
					return;

				Cursor.Current = Cursors.SizeAll;
				DisplayArea = new Rectangle(DisplayArea.Left - (e.X - m_ClickX), DisplayArea.Top - (e.Y - m_ClickY), DisplayArea.Width, DisplayArea.Height);
				m_ClickX = e.X;
				m_ClickY = e.Y;
				Invalidate();
			}
		}

		/// <summary>
		/// Handles the MouseWheel event of the PhotoBox control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
		void PhotoBox_MouseWheel(object sender, MouseEventArgs e)
		{
			if (e.Delta > 0)
				ZoomIn();
			else
				ZoomOut();
		}
		#endregion
	}
}
