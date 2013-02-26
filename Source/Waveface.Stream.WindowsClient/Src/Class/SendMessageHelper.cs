using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Waveface.Stream.WindowsClient
{
	public class SendMessageHelper
	{
		#region Public Method
		/// <summary>
		/// Sends the message.
		/// </summary>
		/// <param name="title">The title.</param>
		/// <param name="message">The message.</param>
		/// <param name="wParam">The w param.</param>
		/// <param name="lParam">The l param.</param>
		/// <History>
		/// Larry 2011/6/9 上午 10:05 Created
		/// </History>
		public void SendMessage(string title, int message, IntPtr wParam, IntPtr lParam)
		{
			SendMessage(null, title, message, wParam, lParam);
		}

		/// <summary>
		/// Sends the message.
		/// </summary>
		/// <param name="className">Name of the class.</param>
		/// <param name="title">The title.</param>
		/// <param name="message">The message.</param>
		/// <param name="wParam">The w param.</param>
		/// <param name="lParam">The l param.</param>
		/// <History>
		/// Larry 2011/6/9 上午 10:05 Created
		/// </History>
		public void SendMessage(string className, string title, int message, IntPtr wParam, IntPtr lParam)
		{
			if (title.Length == 0)
				return;

			var handle = NativeMethods.FindWindow(className, title);

			if (handle == IntPtr.Zero)
				return;

			NativeMethods.SendMessage(handle, message, wParam, lParam);
		}

		public void SendMessage(string title, int wParam, string lParam, Boolean useUnicode)
		{
			SendMessage(null, title, wParam, lParam, useUnicode);
		}

		public void SendMessage(string title, int wParam, string lParam)
		{
			SendMessage(null, title, wParam, lParam);
		}

		public void SendMessage(string className, string title, int wParam, string lParam)
		{
			SendMessage(className, title, wParam, lParam, false);
		}

		public void SendMessage(string className, string title, int wParam, string lParam, Boolean useUnicode)
		{
			var encoder = (useUnicode) ? Encoding.Unicode : Encoding.Default;

			byte[] buffer = encoder.GetBytes(lParam);

			var cds = new CopyDataStruct();
			cds.cbData = buffer.Length;
			cds.lpData = Marshal.AllocHGlobal(buffer.Length);
			Marshal.Copy(buffer, 0, cds.lpData, buffer.Length);

			var handle = NativeMethods.FindWindow(className, title);

			if (handle == IntPtr.Zero)
				return;

			NativeMethods.SendMessage(handle, NativeMethods.WM_COPYDATA, wParam, ref cds);
		}
		#endregion
	}
}
