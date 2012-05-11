using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace StationSystemTray
{
	/// <summary>
	/// 
	/// </summary>
	public partial class FBLoginButton : UserControl
	{
		Bitmap img = Properties.Resources.fb_btn;

		#region Property
		/// <summary>
		/// </summary>
		/// <value></value>
		/// <returns>The text associated with this control.</returns>
		[Localizable(true)]
		public string DisplayText
		{
			get; set;
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="FBLoginButton"/> class.
		/// </summary>
		public FBLoginButton()
		{
			InitializeComponent();
		} 
		#endregion

		#region Protected Method
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			var g = e.Graphics;
			var left = img.Clone(new Rectangle(0, 0, 44, img.Height), img.PixelFormat);
			var middle = img.Clone(new Rectangle(45, 0, 1, img.Height), img.PixelFormat);
			var right = img.Clone(new Rectangle(49, 0, img.Width - 49, img.Height), img.PixelFormat);

			TextureBrush brush = new TextureBrush(middle);
			brush.WrapMode = WrapMode.Tile;

			g.FillRectangle(brush, new Rectangle(left.Width, 0, this.Width - right.Width - left.Width, middle.Height));
			g.DrawImage(left, 0, 0);
			g.DrawImage(right, this.Width - right.Width, 0);

			var msgSize = g.MeasureString(DisplayText, this.Font);
			var x = 42 + ((this.Width -42) - msgSize.Width) / 2;
			var y = (this.Height - msgSize.Height)/2;
			g.DrawString(DisplayText, this.Font, Brushes.Black, x + 1, y + 1);
			g.DrawString(DisplayText, this.Font, Brushes.White, x, y);
		} 
		#endregion

		private void FBLoginButton_MouseDown(object sender, MouseEventArgs e)
		{
			img = Properties.Resources.fb_btn_press;
			this.Invalidate();
		}

		private void FBLoginButton_MouseUp(object sender, MouseEventArgs e)
		{
			img = Properties.Resources.fb_btn;
			this.Invalidate();
		}
	}
}
