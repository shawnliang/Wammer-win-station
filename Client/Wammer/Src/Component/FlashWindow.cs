#region

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

#endregion

namespace Waveface.Component
{
    public static class FlashWindow
    {
        public const uint FLASHW_STOP = 0;
        public const uint FLASHW_CAPTION = 1;
        public const uint FLASHW_TRAY = 2;
        public const uint FLASHW_ALL = 3;
        public const uint FLASHW_TIMER = 4;

        public const uint FLASHW_TIMERNOFG = 12;

        private static bool Win2000OrLater
        {
            get { return Environment.OSVersion.Version.Major >= 5; }
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FlashWindowEx(ref FLASHWINFO pwfi);

        public static bool Flash(Form form)
        {
            if (Win2000OrLater)
            {
                FLASHWINFO _fi = Create_FLASHWINFO(form.Handle, FLASHW_ALL | FLASHW_TIMERNOFG, uint.MaxValue, 0);
                return FlashWindowEx(ref _fi);
            }

            return false;
        }

        private static FLASHWINFO Create_FLASHWINFO(IntPtr handle, uint flags, uint count, uint timeout)
        {
            FLASHWINFO _fi = new FLASHWINFO();
            _fi.cbSize = Convert.ToUInt32(Marshal.SizeOf(_fi));
            _fi.hwnd = handle;
            _fi.dwFlags = flags;
            _fi.uCount = count;
            _fi.dwTimeout = timeout;
            return _fi;
        }

        public static bool Flash(Form form, uint count)
        {
            if (Win2000OrLater)
            {
                FLASHWINFO _fi = Create_FLASHWINFO(form.Handle, FLASHW_ALL, count, 0);
                return FlashWindowEx(ref _fi);
            }

            return false;
        }

        public static bool Start(Form form)
        {
            if (Win2000OrLater)
            {
                FLASHWINFO _fi = Create_FLASHWINFO(form.Handle, FLASHW_ALL, uint.MaxValue, 0);
                return FlashWindowEx(ref _fi);
            }

            return false;
        }

        public static bool Stop(Form form)
        {
            if (Win2000OrLater)
            {
                FLASHWINFO _fi = Create_FLASHWINFO(form.Handle, FLASHW_STOP, uint.MaxValue, 0);
                return FlashWindowEx(ref _fi);
            }

            return false;
        }

        #region Nested type: FLASHWINFO

        [StructLayout(LayoutKind.Sequential)]
        private struct FLASHWINFO
        {
            public uint cbSize;
            public IntPtr hwnd;
            public uint dwFlags;
            public uint uCount;
            public uint dwTimeout;
        }

        #endregion
    }
}