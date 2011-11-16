#region

using System;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

#endregion

namespace Waveface.Component.DropableNotifyIcon
{
    public static class NotifyArea
    {
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        #region NotifyArea

        [DllImport("User32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("User32.dll")]
        private static extern IntPtr FindWindowEx(IntPtr hWnd1, IntPtr hWnd2, string lpsz1, string lpsz2);

        [DllImport("User32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

        public static Rectangle GetRectangle()
        {
            RECT _rect = new RECT();
            Rectangle _retval = Rectangle.Empty;

            IntPtr _hTaskbarHandle = FindWindow("Shell_TrayWnd", null);

            if (_hTaskbarHandle != IntPtr.Zero)
            {
                IntPtr _hSystemTray = FindWindowEx(_hTaskbarHandle, IntPtr.Zero, "TrayNotifyWnd", null);

                if (_hSystemTray != IntPtr.Zero)
                {
                    GetWindowRect(_hSystemTray, ref _rect);
                    _retval = Rectangle.FromLTRB(_rect.left, _rect.top, _rect.right, _rect.bottom);
                }
            }

            return _retval;
        }

        #endregion

        #region NotifyIcon

        [StructLayout(LayoutKind.Sequential)]
        private struct NOTIFYICONIDENTIFIER
        {
            public uint cbSize;
            public IntPtr hWnd;
            public uint uID;
            public Guid guidItem;
        }

        [DllImport("shell32.dll")]
        private static extern int Shell_NotifyIconGetRect([In] ref NOTIFYICONIDENTIFIER identifier,
                                                          out RECT iconLocation);

        private static IntPtr GetHandle(object obj)
        {
            FieldInfo _fieldInfo = obj.GetType().GetField("window",
                                                          BindingFlags.NonPublic | BindingFlags.Static |
                                                          BindingFlags.Instance);
            NativeWindow _nativeWindow = (NativeWindow) _fieldInfo.GetValue(obj);

            if (_nativeWindow == null || _nativeWindow.Handle == IntPtr.Zero)
                return IntPtr.Zero;

            return _nativeWindow.Handle;
        }

        private static int GetUID(object obj)
        {
            return (int) obj.GetType().GetField("id", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(obj);
        }

        public static Rectangle GetNotifyIconRectangle(NotifyIcon notifyIcon)
        {
            if (!OperatingSystem.GteWindows7)
                return new Rectangle(0, 0, 0, 0);

            NOTIFYICONIDENTIFIER _notifyIconIdentifier = new NOTIFYICONIDENTIFIER();
            _notifyIconIdentifier.cbSize = (uint) Marshal.SizeOf(_notifyIconIdentifier);
            _notifyIconIdentifier.hWnd = GetHandle(notifyIcon);
            _notifyIconIdentifier.uID = (uint) GetUID(notifyIcon);

            RECT _notifyIconLocation = new RECT();
            Shell_NotifyIconGetRect(ref _notifyIconIdentifier, out _notifyIconLocation);

            return Rectangle.FromLTRB(_notifyIconLocation.left, _notifyIconLocation.top, _notifyIconLocation.right,
                                      _notifyIconLocation.bottom);
        }

        #endregion
    }
}