using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Waveface.Stream.WindowsClient
{
	public class SendMessageHelper
	{
		#region Const
		const int WM_COPYDATA = 0x4A;
		#endregion

		#region DllImport
		[DllImport("user32.dll", SetLastError = true)]
		static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		static extern IntPtr SendMessage(IntPtr hWnd, Int32 Msg, Int32 wParam, Int32 lParam);

		[DllImport("User32.dll", EntryPoint = "SendMessage")]
		private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, ref CopyDataStruct lParam);

		[return: MarshalAs(UnmanagedType.Bool)]
		[DllImport("user32.dll", SetLastError = true)]
		static extern bool PostMessage(IntPtr hWnd, Int32 Msg, Int32 wParam, Int32 lParam);
		#endregion

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
		public void SendMessage(string title, int message, int wParam, int lParam)
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
		public void SendMessage(string className,string title,int message,int wParam, int lParam)
		{
			if (title.Length == 0)
				return;

			var handle = FindWindow(className, title);

			if (handle == IntPtr.Zero)
				return;

			SendMessage(handle, message, wParam, lParam);
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

		public void SendMessage(string className,string title,int wParam, string lParam,Boolean useUnicode)
		{
			var encoder = (useUnicode) ? Encoding.Unicode : Encoding.Default;

			byte[] buffer = encoder.GetBytes(lParam);

			var cds = new CopyDataStruct();
			cds.cbData = buffer.Length;
			cds.lpData = Marshal.AllocHGlobal(buffer.Length);
			Marshal.Copy(buffer, 0, cds.lpData, buffer.Length);

			var handle = FindWindow(className, title);

			if (handle == IntPtr.Zero)
				return;

			SendMessage(handle, WM_COPYDATA, wParam, ref cds);
		}
		#endregion
	}
}
