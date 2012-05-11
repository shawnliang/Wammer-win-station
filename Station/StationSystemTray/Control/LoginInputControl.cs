using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace StationSystemTray
{
	public class LoginInputControl : System.Windows.Forms.Control
	{
		private class CueTextBox : TextBox
		{
			[DllImport("user32.dll", CharSet = CharSet.Auto)]
			private static extern Int32 SendMessage(IntPtr hWnd, int msg, int wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);

			const int EM_SETCUEBANNER = 0x1501;

			private string _cueText;

			[Localizable(true)]
			public string CueText
			{
				get
				{
					if (_cueText == null)
						return string.Empty;
					return _cueText;
				}
				set
				{
					_cueText = value;
					updateCue();
				}
			}

			private void updateCue()
			{
				SendMessage(this.Handle, EM_SETCUEBANNER, 0, CueText);
			}
		}

		Bitmap m_Image = Properties.Resources.input_box;

		[Localizable(true)]
		public String IDHintMessage
		{
			get { return m_ID.CueText; }
			set { m_ID.CueText = value; }
		}

		[Localizable(true)]
		public String PasswordHintMessage
		{
			get { return m_Password.CueText; }
			set { m_Password.CueText = value; }
		}

		public string ID
		{
			get { return m_ID.Text; }
			set { m_ID.Text = value; }
		}

		public string Password
		{
			get { return m_Password.Text; }
			set { m_Password.Text = value; }
		}

		private CueTextBox _password;
		private CueTextBox m_Password
		{
			get
			{
				var msgSize = TextRenderer.MeasureText(PasswordHintMessage, this.Font);
				if (_password == null)
				{
					_password = new CueTextBox()
					            	{
					            		//BorderStyle = BorderStyle.None,
										Left = 10,
										Top = (this.Height - msgSize.Height) / 2 / 2 + (this.Height - msgSize.Height) / 2 + 10,
										Width = this.Width - 10 - m_Password.Left,
										PasswordChar = '*'
					            	};
				}
				return _password;
			}
		}

		private CueTextBox _id;
		private CueTextBox m_ID
		{
			get
			{
				var msgSize = TextRenderer.MeasureText(IDHintMessage, this.Font);
				if (_id == null)
				{
					_id = new CueTextBox()
					      	{
					      		//BorderStyle = BorderStyle.None,
					      		Left = 10,
					      		Top = (this.Height - msgSize.Height)/2/2,
					      		Width = this.Width - 10 - m_ID.Left
					      	};
				}
				return _password;
			}
		}

		public LoginInputControl()
		{
			this.Height = m_Image.Height;
			this.SizeChanged += new EventHandler(LoginInputControl_SizeChanged);

			this.Controls.Add(m_ID);
			this.Controls.Add(m_Password);
		}

		void LoginInputControl_SizeChanged(object sender, EventArgs e)
		{
			this.Height = m_Image.Height;
		}

		#region Protected Method
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			var g = e.Graphics;
			var left = m_Image.Clone(new Rectangle(0, 0, 10, m_Image.Height), m_Image.PixelFormat);
			var middle = m_Image.Clone(new Rectangle(20, 0, 1, m_Image.Height), m_Image.PixelFormat);
			var right = m_Image.Clone(new Rectangle(40, 0, m_Image.Width - 40, m_Image.Height), m_Image.PixelFormat);

			TextureBrush brush = new TextureBrush(middle);
			brush.WrapMode = WrapMode.Tile;

			g.FillRectangle(brush, new Rectangle(left.Width, 0, this.Width - right.Width - left.Width, middle.Height));
			g.DrawImage(left, 0, 0);
			g.DrawImage(right, this.Width - right.Width, 0);

			//var msgSize = TextRenderer.MeasureText(IDHintMessage, this.Font);
			//m_ID.Left = 10;
			//m_ID.Top = (this.Height - msgSize.Height) / 2 / 2;
			//m_ID.Width = this.Width - 10 - m_ID.Left;

			//m_Password.Left = 10;
			//m_Password.Top = (this.Height - msgSize.Height) / 2 / 2 + (this.Height - msgSize.Height) / 2 + 10;
			//m_Password.Width = this.Width - 10 - m_Password.Left;
		}
		#endregion
	}
}
