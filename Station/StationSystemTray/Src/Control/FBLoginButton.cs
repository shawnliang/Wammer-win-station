﻿using System.ComponentModel;
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
		private Bitmap img = Resources.fb_btn_press;

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

			var g = e.Graphics;
			var left = img.Clone(new Rectangle(0, 0, 44, img.Height), img.PixelFormat);
			var middle = img.Clone(new Rectangle(45, 0, 1, img.Height), img.PixelFormat);
			var right = img.Clone(new Rectangle(49, 0, img.Width - 49, img.Height), img.PixelFormat);

			var brush = new TextureBrush(middle) { WrapMode = WrapMode.Tile };

			g.FillRectangle(brush, new Rectangle(left.Width, 0, Width - right.Width - left.Width, middle.Height));
			g.DrawImage(left, 0, 0);
			g.DrawImage(right, Width - right.Width, 0);
			g.DrawImage(Resources.fb_logo, 5, (Height - Resources.fb_logo.Height)/2);

			var msgSize = g.MeasureString(DisplayText, Font);
			var x = 5 + Resources.fb_logo.Width + ((Width - (5 + Resources.fb_logo.Width)) - msgSize.Width) / 2;
			var y = (Height - msgSize.Height) / 2;
			g.DrawString(DisplayText, Font, Brushes.Black, x + 1, y + 1);
			g.DrawString(DisplayText, Font, Brushes.White, x, y);
		}
		#endregion Protected Method



		/// <summary>
		/// Handles the MouseDown event of the FBLoginButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
		private void FBLoginButton_MouseDown(object sender, MouseEventArgs e)
		{
			img = Resources.fb_btn;
			Invalidate();
		}

		/// <summary>
		/// Handles the MouseUp event of the FBLoginButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
		private void FBLoginButton_MouseUp(object sender, MouseEventArgs e)
		{
			img = Resources.fb_btn_press;
			Invalidate();
		}

		/// <summary>
		/// Handles the MouseHover event of the FBLoginButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void FBLoginButton_MouseHover(object sender, System.EventArgs e)
		{
			img = Resources.fb_btn;
			Invalidate();
		}

		/// <summary>
		/// Handles the MouseLeave event of the FBLoginButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void FBLoginButton_MouseLeave(object sender, System.EventArgs e)
		{
			img = Resources.fb_btn_press;
			Invalidate();
		}
	}
}