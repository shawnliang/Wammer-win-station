using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Waveface
{
	public class Toast : Control
	{
		#region Const
		public const int LENGTH_LONG = 3500;
		public const int LENGTH_SHORT = 2000;
		#endregion

		#region Var
		private Timer _toastTimer;
		#endregion


		#region Private Property
		private Timer m_ToastTimer
		{
			get
			{
				if (_toastTimer == null)
				{
					_toastTimer = new Timer();
					_toastTimer.Tick += new EventHandler(_toastTimer_Tick);
				}
				return _toastTimer;
			}
			set
			{
				_toastTimer = null;
			}
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="Toast"/> class.
		/// </summary>
		/// <param name="parent">The parent.</param>
		/// <param name="text">The text.</param>
		/// <param name="displayDuration">The display duration.</param>
		private Toast(Control parent, string text, int displayDuration)
		{
			this.Parent = parent;
			this.Text = text;
			m_ToastTimer.Interval = displayDuration;

			InitToast();
		}
		#endregion


		#region Private Method
		/// <summary>
		/// Inits the toast.
		/// </summary>
		private void InitToast()
		{
			this.Visible = false;

			this.VisibleChanged += new EventHandler(Toast_VisibleChanged);
			this.Parent.SizeChanged += new EventHandler(Parent_SizeChanged);
			this.Parent.Disposed += new EventHandler(Parent_Disposed);

			AdjustToastMsg();
		}

		/// <summary>
		/// Adjusts the toast MSG location.
		/// </summary>
		private void AdjustToastMsg()
		{
			using (var g = this.CreateGraphics())
			{
				var messageSize = g.MeasureString(this.Text, this.Font);
				this.Size = new Size((int)messageSize.Width + 10, (int)messageSize.Height + 10);

				var top = (int)(this.Parent.Height * 0.9);

				if ((top + this.Height) > this.Parent.DisplayRectangle.Height)
					top = (top > this.Height) ? (this.Parent.DisplayRectangle.Height - this.Height) : 0;

				this.Top = top;
				this.Left = (this.Parent.DisplayRectangle.Width - this.Width) / 2;
			}
		}
		#endregion


		#region Protected Method
		/// <summary>
		/// Releases the unmanaged resources used by the <see cref="T:System.Windows.Forms.Control"/> and its child controls and optionally releases the managed resources.
		/// </summary>
		/// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
		protected override void Dispose(bool disposing)
		{
			if (_toastTimer != null)
			{
				_toastTimer.Dispose();
				m_ToastTimer = null;
			}

			this.VisibleChanged -= new EventHandler(Toast_VisibleChanged);

			if (this.Parent != null)
			{
				this.Parent.SizeChanged -= new EventHandler(Parent_SizeChanged);
				this.Parent.Disposed -= new EventHandler(Parent_Disposed);
			}
			base.Dispose(disposing);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			var g = e.Graphics;
			g.FillRectangle(new SolidBrush(ColorTranslator.FromHtml("#434343")), DisplayRectangle);
			g.DrawRectangle(new Pen(ColorTranslator.FromHtml("#505050")), DisplayRectangle);
			g.DrawString(this.Text, this.Font, new SolidBrush(Color.White), 5, 5);
		}
		#endregion


		#region Public Static Method
		/// <summary>
		/// Makes the text.
		/// </summary>
		/// <param name="parent">The parent.</param>
		/// <param name="message">The message.</param>
		/// <param name="displayDuration">The display duration.</param>
		/// <returns></returns>
		public static Toast MakeText(Control parent, string message, int displayDuration)
		{
			return new Toast(parent, message, displayDuration);
		}
		#endregion


		#region Event Process
		/// <summary>
		/// Handles the VisibleChanged event of the Toast control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void Toast_VisibleChanged(object sender, EventArgs e)
		{
			if (this.Visible)
			{
				AdjustToastMsg();
				BringToFront();
			}

			m_ToastTimer.Enabled = this.Visible;
		}

		/// <summary>
		/// Handles the Tick event of the _toastTimer control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void _toastTimer_Tick(object sender, EventArgs e)
		{
			_toastTimer.Stop();
			this.Dispose();
		}

		/// <summary>
		/// Handles the SizeChanged event of the Parent control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void Parent_SizeChanged(object sender, EventArgs e)
		{
			AdjustToastMsg();
		}

		void Parent_Disposed(object sender, EventArgs e)
		{
			_toastTimer.Stop();
			this.Dispose();
		}
		#endregion
	}
}
