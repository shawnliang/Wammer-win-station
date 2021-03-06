
#region

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Windows.Forms;

#endregion

namespace Waveface.Component.PopupControl
{
    internal static class NativeMethods
    {
        internal const int WM_NCHITTEST = 0x0084,
                           WM_NCACTIVATE = 0x0086,
                           WS_EX_TRANSPARENT = 0x00000020,
                           WS_EX_TOOLWINDOW = 0x00000080,
                           WS_EX_LAYERED = 0x00080000,
                           WS_EX_NOACTIVATE = 0x08000000,
                           HTTRANSPARENT = -1,
                           HTLEFT = 10,
                           HTRIGHT = 11,
                           HTTOP = 12,
                           HTTOPLEFT = 13,
                           HTTOPRIGHT = 14,
                           HTBOTTOM = 15,
                           HTBOTTOMLEFT = 16,
                           HTBOTTOMRIGHT = 17,
                           WM_PRINT = 0x0317,
                           WM_USER = 0x0400,
                           WM_REFLECT = WM_USER + 0x1C00,
                           WM_COMMAND = 0x0111,
                           CBN_DROPDOWN = 7,
                           WM_GETMINMAXINFO = 0x0024;

        private static HandleRef HWND_TOPMOST = new HandleRef(null, new IntPtr(-1));
        private static bool? s_isRunningOnMono;

        public static bool IsRunningOnMono
        {
            get
            {
                if (!s_isRunningOnMono.HasValue)
                    s_isRunningOnMono = Type.GetType("Mono.Runtime") != null;

                return s_isRunningOnMono.Value;
            }
        }

        [SuppressUnmanagedCodeSecurity]
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int AnimateWindow(HandleRef windowHandle, int time, AnimationFlags flags);

        internal static void AnimateWindow(Control control, int time, AnimationFlags flags)
        {
            try
            {
                SecurityPermission _sp = new SecurityPermission(SecurityPermissionFlag.UnmanagedCode);
                _sp.Demand();
                AnimateWindow(new HandleRef(control, control.Handle), time, flags);
            }
            catch (SecurityException)
            {
            }
        }

        [SuppressUnmanagedCodeSecurity]
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern bool SetWindowPos(HandleRef hWnd, HandleRef hWndInsertAfter, int x, int y, int cx, int cy, int flags);

        internal static void SetTopMost(Control control)
        {
            try
            {
                SecurityPermission _sp = new SecurityPermission(SecurityPermissionFlag.UnmanagedCode);
                _sp.Demand();
                SetWindowPos(new HandleRef(control, control.Handle), HWND_TOPMOST, 0, 0, 0, 0, 0x13);
            }
            catch (SecurityException)
            {
            }
        }

        internal static int HIWORD(int n)
        {
            return (short) ((n >> 16) & 0xffff);
        }

        internal static int HIWORD(IntPtr n)
        {
            return HIWORD(unchecked((int) (long) n));
        }

        internal static int LOWORD(int n)
        {
            return (short) (n & 0xffff);
        }

        internal static int LOWORD(IntPtr n)
        {
            return LOWORD(unchecked((int) (long) n));
        }

        #region Nested type: AnimationFlags

        [Flags]
        internal enum AnimationFlags
        {
            Roll = 0x0000, // Uses a roll animation.
            HorizontalPositive = 0x00001, // Animates the window from left to right. This flag can be used with roll or slide animation.
            HorizontalNegative = 0x00002, // Animates the window from right to left. This flag can be used with roll or slide animation.
            VerticalPositive = 0x00004, // Animates the window from top to bottom. This flag can be used with roll or slide animation.
            VerticalNegative = 0x00008, // Animates the window from bottom to top. This flag can be used with roll or slide animation.
            Center = 0x00010, // Makes the window appear to collapse inward if Hide is used or expand outward if the Hide is not used.
            Hide = 0x10000, // Hides the window. By default, the window is shown.
            Activate = 0x20000, // Activates the window.
            Slide = 0x40000, // Uses a slide animation. By default, roll animation is used.
            Blend = 0x80000, // Uses a fade effect. This flag can be used only with a top-level window.
            Mask = 0xfffff,
        }

        #endregion

        #region Nested type: MINMAXINFO

        [StructLayout(LayoutKind.Sequential)]
        internal struct MINMAXINFO
        {
            public Point reserved;
            public Size maxSize;
            public Point maxPosition;
            public Size minTrackSize;
            public Size maxTrackSize;
        }

        #endregion
    }
}