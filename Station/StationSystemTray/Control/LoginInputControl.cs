using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using StationSystemTray.Properties;

namespace StationSystemTray
{
	public class LoginInputControl : System.Windows.Forms.Control
	{
		private readonly Bitmap m_Image = Resources.input_box;
		private CueTextBox _id;
		private CueTextBox _password;

		public LoginInputControl()
		{
			Height = m_Image.Height;
			SizeChanged += LoginInputControl_SizeChanged;

			Controls.Add(m_ID);
			Controls.Add(m_Password);
		}

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

		private CueTextBox m_Password
		{
			get
			{
				Size msgSize = TextRenderer.MeasureText(PasswordHintMessage, Font);
				return _password ?? (_password = new CueTextBox
													{
														//BorderStyle = BorderStyle.None,
														Left = 10,
														Top = (Height - msgSize.Height) / 2 / 2 + (Height - msgSize.Height) / 2 + 10,
														Width = Width - 10 - m_Password.Left,
														PasswordChar = '*'
													});
			}
		}

		private CueTextBox m_ID
		{
			get
			{
				Size msgSize = TextRenderer.MeasureText(IDHintMessage, Font);
				if (_id == null)
				{
					_id = new CueTextBox
							{
								//BorderStyle = BorderStyle.None,
								Left = 10,
								Top = (Height - msgSize.Height) / 2 / 2,
								Width = Width - 10 - m_ID.Left
							};
				}
				return _password;
			}
		}

		private void LoginInputControl_SizeChanged(object sender, EventArgs e)
		{
			Height = m_Image.Height;
		}

		#region Protected Method

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			Graphics g = e.Graphics;
			Bitmap left = m_Image.Clone(new Rectangle(0, 0, 10, m_Image.Height), m_Image.PixelFormat);
			Bitmap middle = m_Image.Clone(new Rectangle(20, 0, 1, m_Image.Height), m_Image.PixelFormat);
			Bitmap right = m_Image.Clone(new Rectangle(40, 0, m_Image.Width - 40, m_Image.Height), m_Image.PixelFormat);

			var brush = new TextureBrush(middle) { WrapMode = WrapMode.Tile };

			g.FillRectangle(brush, new Rectangle(left.Width, 0, Width - right.Width - left.Width, middle.Height));
			g.DrawImage(left, 0, 0);
			g.DrawImage(right, Width - right.Width, 0);

			//var msgSize = TextRenderer.MeasureText(IDHintMessage, this.Font);
			//m_ID.Left = 10;
			//m_ID.Top = (this.Height - msgSize.Height) / 2 / 2;
			//m_ID.Width = this.Width - 10 - m_ID.Left;

			//m_Password.Left = 10;
			//m_Password.Top = (this.Height - msgSize.Height) / 2 / 2 + (this.Height - msgSize.Height) / 2 + 10;
			//m_Password.Width = this.Width - 10 - m_Password.Left;
		}

		#endregion Protected Method

		#region Nested type: CueTextBox

		private class CueTextBox : TextBox
		{
			private const int EM_SETCUEBANNER = 0x1501;

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

			[DllImport("user32.dll", CharSet = CharSet.Auto)]
			private static extern Int32 SendMessage(IntPtr hWnd, int msg, int wParam,
													[MarshalAs(UnmanagedType.LPWStr)] string lParam);

			private void updateCue()
			{
				SendMessage(Handle, EM_SETCUEBANNER, 0, CueText);
			}
		}

		#endregion Nested type: CueTextBox
	}
}