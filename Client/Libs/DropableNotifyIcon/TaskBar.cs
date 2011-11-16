/**
 * TaskBar.cs
 * http://www.crowsprogramming.com/archives/88
 */

#region

using System;
using System.Drawing;
using System.Runtime.InteropServices;

#endregion

namespace Waveface.Component.DropableNotifyIcon
{
    internal partial class TaskBar
    {
        #region Native API

        // The native API functions/enums/structs needed to access taskbar info
        // consult MSDN for more detailed information about them
        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct APPBARDATA
        {
            public Int32 cbSize;
            public IntPtr hWnd;
            public Int32 uCallbackMessage;
            public ABEdge uEdge;
            public RECT rc;
            public IntPtr lParam;
        }

        private enum ABMsg
        {
            ABM_NEW = 0,
            ABM_REMOVE = 1,
            ABM_QUERYPOS = 2,
            ABM_SETPOS = 3,
            ABM_GETSTATE = 4,
            ABM_GETTASKBARPOS = 5,
            ABM_ACTIVATE = 6,
            ABM_GETAUTOHIDEBAR = 7,
            ABM_SETAUTOHIDEBAR = 8,
            ABM_WINDOWPOSCHANGED = 9,
            ABM_SETSTATE = 10
        }

        private enum ABEdge
        {
            ABE_LEFT = 0,
            ABE_TOP = 1,
            ABE_RIGHT = 2,
            ABE_BOTTOM = 3,
        }

        private enum ABState
        {
            ABS_MANUAL = 0,
            ABS_AUTOHIDE = 1,
            ABS_ALWAYSONTOP = 2,
            ABS_AUTOHIDEANDONTOP = 3
        }

        [DllImport("shell32.dll")]
        private static extern IntPtr SHAppBarMessage(ABMsg dwMessage, [In, Out] ref APPBARDATA pData);

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        #endregion

        /// <summary>
        /// Represents the edge of the screen the task bar is docked to.
        /// </summary>
        public enum TaskBarEdge
        {
            Left = ABEdge.ABE_LEFT,
            Top = ABEdge.ABE_TOP,
            Right = ABEdge.ABE_RIGHT,
            Bottom = ABEdge.ABE_BOTTOM
        }

        /// <summary>
        /// The states the task bar can be in.
        /// </summary>
        [Flags]
        public enum TaskBarState
        {
            /// <summary>
            /// No autohide, not always top
            /// </summary>
            None = ABState.ABS_MANUAL,

            /// <summary>
            /// Hides task bar when mouse exits task bar region
            /// </summary>
            AutoHide = ABState.ABS_AUTOHIDE,

            /// <summary>
            /// Taskbar is always on top of other windows
            /// </summary>
            AlwaysTop = ABState.ABS_ALWAYSONTOP
        }

        /// <summary>
        /// Gets the location, in screen coordinates of the task bar.
        /// </summary>
        /// <returns>The taskbar location.</returns>
        public static Point GetTaskBarLocation()
        {
            APPBARDATA _appBar = GetTaskBarData();

            return new Point(_appBar.rc.left, _appBar.rc.top);
        }

        /// <summary>
        /// Gets the size, in pixels of the task bar.
        /// </summary>
        /// <returns>The taskbar size.</returns>
        public static Size GetTaskBarSize()
        {
            APPBARDATA _appBar = GetTaskBarData();

            return new Size(_appBar.rc.right - _appBar.rc.left, _appBar.rc.bottom - _appBar.rc.top);
        }

        /// <summary>
        /// Gets the edge of the screen that the task bar is docked to.
        /// </summary>
        /// <returns></returns>
        public static TaskBarEdge GetTaskBarEdge()
        {
            APPBARDATA _appBar = GetTaskBarData();

            return (TaskBarEdge)_appBar.uEdge;
        }

        /// <summary>
        /// Gets the current state of the taskbar.
        /// </summary>
        /// <returns></returns>
        public static TaskBarState GetTaskBarState()
        {
            APPBARDATA _appBar = CreateAppBarData();

            return (TaskBarState)SHAppBarMessage(ABMsg.ABM_GETSTATE, ref _appBar);
        }

        /// <summary>
        /// Sets the state of the task bar.
        /// </summary>
        /// <param name="state">The new state.</param>
        public static void SetTaskBarState(TaskBarState state)
        {
            APPBARDATA _appBar = CreateAppBarData();

            _appBar.lParam = (IntPtr)state;

            SHAppBarMessage(ABMsg.ABM_SETSTATE, ref _appBar);
        }

        /// <summary>
        /// Gets an APPBARDATA struct with valid location,size,and edge of the taskbar.
        /// </summary>
        /// <returns></returns>
        private static APPBARDATA GetTaskBarData()
        {
            APPBARDATA _appBar = CreateAppBarData();

            IntPtr _ret = SHAppBarMessage(ABMsg.ABM_GETTASKBARPOS, ref _appBar);

            return _appBar;
        }

        /// <summary>
        /// Creats an APPBARDATA struct with its hWnd member set to the task bar window.
        /// </summary>
        /// <returns></returns>
        private static APPBARDATA CreateAppBarData()
        {
            APPBARDATA _appBar = new APPBARDATA();

            _appBar.hWnd = FindWindow("Shell_TrayWnd", "");
            _appBar.cbSize = Marshal.SizeOf(_appBar);

            return _appBar;
        }
    }
}