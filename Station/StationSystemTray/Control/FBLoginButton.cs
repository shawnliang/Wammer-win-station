using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using StationSystemTray.Properties;

namespace StationSystemTray
{
	/// <summary>
	///
	/// </summary>
	public partial class FBLoginButton
	{
		private Bitmap img = Resources.fb_btn;

		#region Property

		/// <summary>
		/// </summary>
		/// <value></value>
		/// <returns>The text associated with this control.</returns>
		[Localizable(true)]
		public string DisplayText { get; set; }

		#endregion Property

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="FBLoginButton"/> class.
		/// </summary>
		public FBLoginButton()
		{
			InitializeComponent();
		}

		#endregion Constructor

		#region Protected Method

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			Graphics g = e.Graphics;
			Bitmap left = img.Clone(new Rectangle(0, 0, 44, img.Height), img.PixelFormat);
			Bitmap middle = img.Clone(new Rectangle(45, 0, 1, img.Height), img.PixelFormat);
			Bitmap right = img.Clone(new Rectangle(49, 0, img.Width - 49, img.Height), img.PixelFormat);

			var brush = new TextureBrush(middle) { WrapMode = WrapMode.Tile };

			g.FillRectangle(brush, new Rectangle(left.Width, 0, Width - right.Width - left.Width, middle.Height));
			g.DrawImage(left, 0, 0);
			g.DrawImage(right, Width - right.Width, 0);

			SizeF msgSize = g.MeasureString(DisplayText, Font);
			float x = 42 + ((Width - 42) - msgSize.Width) / 2;
			float y = (Height - msgSize.Height) / 2;
			g.DrawString(DisplayText, Font, Brushes.Black, x + 1, y + 1);
			g.DrawString(DisplayText, Font, Brushes.White, x, y);
		}

		#endregion Protected Method

		private void FBLoginButton_MouseDown(object sender, MouseEventArgs e)
		{
			img = Resources.fb_btn_press;
			Invalidate();
		}

		private void FBLoginButton_MouseUp(object sender, MouseEventArgs e)
		{
			img = Resources.fb_btn;
			Invalidate();
		}
	}
}