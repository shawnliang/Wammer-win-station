using System.Drawing;
using System.Windows.Forms;
using StationSystemTray.Properties;

namespace StationSystemTray
{
	public class LoginButton : Button
	{
		private Bitmap m_Image = Resources.login_btn;

		public LoginButton()
		{
			Width = m_Image.Width;
			Height = m_Image.Height;
			MaximumSize = Size;

			MouseHover += LoginButton_MouseHover;
			MouseLeave += LoginButton_MouseLeave;
			MouseUp += LoginButton_MouseUp;
			MouseDown += LoginButton_MouseDown;
		}

		void LoginButton_MouseLeave(object sender, System.EventArgs e)
		{
			m_Image = Resources.login_btn;
			Invalidate();
		}

		void LoginButton_MouseHover(object sender, System.EventArgs e)
		{
			m_Image = Resources.login_btn_press;
			Invalidate();
		}

		private void LoginButton_MouseDown(object sender, MouseEventArgs e)
		{
			m_Image = Resources.login_btn_press;
			Invalidate();
		}

		private void LoginButton_MouseUp(object sender, MouseEventArgs e)
		{
			m_Image = Resources.login_btn;
			Invalidate();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics g = e.Graphics;

			g.DrawImage(m_Image, 0, 0);

			SizeF msgSize = g.MeasureString(Text, Font);
			float x = (Width - msgSize.Width)/2;
			float y = (Height - msgSize.Height)/2;

			g.DrawString(Text, Font, Brushes.Black, x + 1, y + 1);
			g.DrawString(Text, Font, Brushes.White, x, y);
		}
	}
}