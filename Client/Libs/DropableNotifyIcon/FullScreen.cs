#region

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

#endregion

namespace Waveface.Component.DropableNotifyIcon
{
    internal static class FullScreen
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern bool
            GetWindowRect(IntPtr hWnd, out RECT lpRect);

        public static bool Detect()
        {
            IntPtr _foreWindow = GetForegroundWindow();

            RECT _foreRect;
            GetWindowRect(_foreWindow, out _foreRect);

            Size _screenSize = SystemInformation.PrimaryMonitorSize;

            if ((_foreRect.left <= 0) && (_foreRect.top <= 0) &&
                (_foreRect.right >= _screenSize.Width) && (_foreRect.bottom >= _screenSize.Height))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #region Nested type: RECT

        private struct RECT
        {
            public int bottom;
            public int left;
            public int right;
            public int top;
        }

        #endregion
    }
}