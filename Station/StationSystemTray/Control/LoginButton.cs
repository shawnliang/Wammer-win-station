using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;

namespace StationSystemTray
{
	public partial class LoginButton : System.Windows.Forms.Control
	{
		private Bitmap m_Image = Properties.Resources.login_btn;

		public LoginButton()
		{
			this.Width = m_Image.Width;
			this.Height = m_Image.Height;
			this.MaximumSize = this.Size;

			this.MouseUp += new System.Windows.Forms.MouseEventHandler(LoginButton_MouseUp);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(LoginButton_MouseDown);
		}

		void LoginButton_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			m_Image = Properties.Resources.login_btn_press;
			this.Invalidate();
		}

		void LoginButton_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			m_Image = Properties.Resources.login_btn;
			this.Invalidate();
		}

		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
		{
			var g = e.Graphics;

			g.DrawImage(m_Image, 0, 0);

			var msgSize = g.MeasureString(this.Text, this.Font);
			var x = (this.Width - msgSize.Width) / 2;
			var y = (this.Height - msgSize.Height) / 2;

			g.DrawString(this.Text, this.Font, Brushes.Black, x + 1, y + 1);
			g.DrawString(this.Text, this.Font, Brushes.White, x, y);
		}
	}
}
