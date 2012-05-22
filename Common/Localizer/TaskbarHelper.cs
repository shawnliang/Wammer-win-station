using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Waveface.Common
{
	public class TaskbarHelper
	{
		[DllImport("shell32.dll")]
		internal static extern void SetCurrentProcessExplicitAppUserModelID([MarshalAs(UnmanagedType.LPWStr)] string AppID);

		private static Version VersionWin7 = new Version(6, 1);

		public static void SetAppId(string AppId)
		{
			if (Environment.OSVersion.Version < VersionWin7)
				return; // taskbar is only supported after windows 7 (v6.1)

			SetCurrentProcessExplicitAppUserModelID(AppId);
		}
	}
}
