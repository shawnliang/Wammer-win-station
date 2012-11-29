
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using Waveface.Stream.WindowsClient.Properties;

namespace Waveface.Stream.WindowsClient
{
	public class TitlePanel : UserControl
	{
		#region Var
		private TextureBrush _backgroundBrush;
		private System.ComponentModel.IContainer components;
		private Bitmap _bmpOffscreen;
		#endregion



		#region Private Property
		/// <summary>
		/// Gets or sets the m_ BMP offscreen.
		/// </summary>
		/// <value>The m_ BMP offscreen.</value>
		private Bitmap m_BmpOffscreen
		{
			get
			{
				if (_bmpOffscreen == null)
				{
					_bmpOffscreen = new Bitmap(ClientSize.Width, ClientSize.Height);
					using (Graphics g = Graphics.FromImage(m_BmpOffscreen))
					{
						g.TextRenderingHint = TextRenderingHint.AntiAlias;
						g.InterpolationMode = InterpolationMode.HighQualityBilinear;
						g.PixelOffsetMode = PixelOffsetMode.HighQuality;
						g.SmoothingMode = SmoothingMode.HighQuality;
						g.FillRectangle(m_BackgroundBrush, 0, 0, Width, Height);
						g.DrawImage(Resources.desktop_logo, 4, -2);
					}
				}
				return _bmpOffscreen;
			}
			set
			{
				if (_bmpOffscreen == value)
					return;

				if (_bmpOffscreen != null)
				{
					_bmpOffscreen.Dispose();
					_bmpOffscreen = null;
				}

				_bmpOffscreen = value;
			}
		}

		/// <summary>
		/// Gets the m_ background brush.
		/// </summary>
		/// <value>The m_ background brush.</value>
		private Brush m_BackgroundBrush
		{
			get
			{
				return _backgroundBrush ?? (_backgroundBrush = new TextureBrush(Resources.titlebar_1, WrapMode.Tile));
			}
		}
		#endregion



		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="TitlePanel"/> class.
		/// </summary>
		public TitlePanel()
		{
			InitializeComponent();

			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.DoubleBuffer, true);
			SetStyle(ControlStyles.UserPaint, true);
		}
		#endregion




		#region Windows Form Designer generated code

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TitlePanel));
			this.SuspendLayout();
			// 
			// TitlePanel
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(161)))), ((int)(((byte)(185)))));
			resources.ApplyResources(this, "$this");
			this.Name = "TitlePanel";
			this.Load += new System.EventHandler(this.TitlePanel_Load);
			this.ResumeLayout(false);

		}

		#endregion



		#region Protected Method
		protected override void OnPaint(PaintEventArgs e)
		{
			e.Graphics.DrawImage(m_BmpOffscreen, 0, 0);
		}

		protected override void Dispose(bool disposing)
		{
			if (IsDisposed)
				return;

			m_BmpOffscreen = null;

			if (_backgroundBrush != null)
			{
				_backgroundBrush.Dispose();
				_backgroundBrush = null;
			}
			base.Dispose(disposing);
		}
		#endregion


		#region Event Process
		/// <summary>
		/// Handles the Load event of the TitlePanel control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void TitlePanel_Load(object sender, EventArgs e)
		{
			this.SizeChanged += new EventHandler(TitlePanel_SizeChanged);
		}

		/// <summary>
		/// Handles the SizeChanged event of the TitlePanel control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void TitlePanel_SizeChanged(object sender, EventArgs e)
		{
			m_BmpOffscreen = null;
		}
		#endregion
	}
}
