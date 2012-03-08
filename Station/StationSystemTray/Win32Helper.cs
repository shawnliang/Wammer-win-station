using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace StationSystemTray
{
	public class Win32Helper
	{
        #region Const
        public const int WM_USER = 0x400;
        public const int WM_COPYDATA = 0x004A; 
        #endregion


        #region Structure
        [StructLayout(LayoutKind.Sequential)]
        public struct CopyDataStruct
        {
            public int dwData;  // 附加參數
            public int cbData;  // 數據大小
            public IntPtr lpData;  // 數據內容
        } 
        #endregion



		[DllImport("user32.dll")]
		public static extern bool SetForegroundWindow(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

		[DllImport("user32.dll")]
		public static extern bool BringWindowToTop(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll")]
		public static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);

		[DllImport("kernel32.dll")]
		public static extern uint GetCurrentThreadId();

		[DllImport("user32.dll")]
		public static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

	}
}
