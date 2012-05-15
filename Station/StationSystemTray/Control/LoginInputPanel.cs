using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using StationSystemTray.Properties;

namespace StationSystemTray
{
	public class LoginInputPanel : System.Windows.Forms.Panel
	{
		private readonly Bitmap m_Image = Resources.input_box;

		public LoginInputPanel()
		{
			Height = m_Image.Height;
			SizeChanged += LoginInputPanel_SizeChanged;
		}


		private void LoginInputPanel_SizeChanged(object sender, EventArgs e)
		{
			Height = m_Image.Height;
		}

		#region Protected Method

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			var g = e.Graphics;
			var left = m_Image.Clone(new Rectangle(0, 0, 10, m_Image.Height), m_Image.PixelFormat);
			var middle = m_Image.Clone(new Rectangle(20, 0, 1, m_Image.Height), m_Image.PixelFormat);
			var right = m_Image.Clone(new Rectangle(40, 0, m_Image.Width - 40, m_Image.Height), m_Image.PixelFormat);

			var brush = new TextureBrush(middle) { WrapMode = WrapMode.Tile };

			g.FillRectangle(brush, new Rectangle(left.Width, 0, Width - right.Width - left.Width, middle.Height));
			g.DrawImage(left, 0, 0);
			g.DrawImage(right, Width - right.Width, 0);
		}

		#endregion Protected Method
	}
}