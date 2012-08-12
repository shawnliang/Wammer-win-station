using System.Drawing;
using System.Windows.Forms;
using Waveface.Properties;

namespace StationSystemTray
{
	public class LogoutButton : Button
	{
		private Bitmap m_Image = Resources.FB_creat_btn;
		private Brush m_ShadowBrush = new SolidBrush(ColorTranslator.FromHtml("#5e332b"));

		public LogoutButton()
		{
			Width = m_Image.Width;
			Height = m_Image.Height;
			MaximumSize = Size;

			MouseHover += LogoutButton_MouseHover;
			MouseLeave += LogoutButton_MouseLeave;
			MouseUp += LogoutButton_MouseUp;
			MouseDown += LogoutButton_MouseDown;
		}

		void LogoutButton_MouseLeave(object sender, System.EventArgs e)
		{
			m_Image = Resources.FB_creat_btn;
			Invalidate();
		}

		void LogoutButton_MouseHover(object sender, System.EventArgs e)
		{
			m_Image = Resources.FB_creat_btn_hl;
			Invalidate();
		}

		private void LogoutButton_MouseDown(object sender, MouseEventArgs e)
		{
			m_Image = Resources.FB_creat_btn_hl;
			Invalidate();
		}

		private void LogoutButton_MouseUp(object sender, MouseEventArgs e)
		{
			m_Image = Resources.FB_creat_btn;
			Invalidate();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics g = e.Graphics;

			g.DrawImage(m_Image, 0, 0);

			SizeF msgSize = g.MeasureString(Text, Font);
			float x = (Width - msgSize.Width)/2;
			float y = (Height - msgSize.Height)/2;
			
			g.DrawString(Text, Font, m_ShadowBrush, x + 1, y + 1);
			g.DrawString(Text, Font, Brushes.White, x, y);
		}
	}
}