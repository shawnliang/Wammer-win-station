/**
 * MouseHook.cs
 * Class written by CodeSummoner
 * http://www.codeproject.com/KB/system/globalmousekeyboardlib.aspx
 */

#region

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

#endregion

namespace Waveface.Component.DropableNotifyIcon
{
    public class MouseHook : GlobalHook
    {
        #region MouseEventType Enum

        private enum MouseEventType
        {
            None,
            MouseDown,
            MouseUp,
            DoubleClick,
            MouseWheel,
            MouseMove
        }

        #endregion

        #region Events

        public event MouseEventHandler MouseDown;
        public event MouseEventHandler MouseUp;
        public event MouseEventHandler MouseMove;
        public event MouseEventHandler MouseWheel;

        public event EventHandler Click;
        public event EventHandler DoubleClick;

        #endregion

        #region Constructor

        public MouseHook()
        {
            m_hookType = WH_MOUSE_LL;
        }

        #endregion

        #region Methods

        protected override int HookCallbackProcedure(int nCode, int wParam, IntPtr lParam)
        {
            if ((nCode > -1) && ((MouseDown != null) || (MouseUp != null) || (MouseMove != null)))
            {
                MouseLLHookStruct _mouseHookStruct =
                    (MouseLLHookStruct) Marshal.PtrToStructure(lParam, typeof (MouseLLHookStruct));

                MouseButtons _button = GetButton(wParam);
                MouseEventType _eventType = GetEventType(wParam);

                MouseEventArgs _e = new MouseEventArgs(
                    _button,
                    (_eventType == MouseEventType.DoubleClick ? 2 : 1),
                    _mouseHookStruct.pt.x,
                    _mouseHookStruct.pt.y,
                    (_eventType == MouseEventType.MouseWheel ? (short) ((_mouseHookStruct.mouseData >> 16) & 0xffff) : 0));

                // Prevent multiple Right Click events (this probably happens for popup menus)
                if ((_button == MouseButtons.Right) && (_mouseHookStruct.flags != 0))
                {
                    _eventType = MouseEventType.None;
                }

                switch (_eventType)
                {
                    case MouseEventType.MouseDown:
                        if (MouseDown != null)
                        {
                            MouseDown(this, _e);
                        }

                        break;
                    case MouseEventType.MouseUp:
                        if (Click != null)
                        {
                            Click(this, new EventArgs());
                        }

                        if (MouseUp != null)
                        {
                            MouseUp(this, _e);
                        }

                        break;
                    case MouseEventType.DoubleClick:
                        if (DoubleClick != null)
                        {
                            DoubleClick(this, new EventArgs());
                        }

                        break;
                    case MouseEventType.MouseWheel:
                        if (MouseWheel != null)
                        {
                            MouseWheel(this, _e);
                        }

                        break;
                    case MouseEventType.MouseMove:
                        if (MouseMove != null)
                        {
                            MouseMove(this, _e);
                        }

                        break;
                    default:
                        break;
                }
            }

            return CallNextHookEx(m_handleToHook, nCode, wParam, lParam);
        }

        private MouseButtons GetButton(Int32 wParam)
        {
            switch (wParam)
            {
                case WM_LBUTTONDOWN:
                case WM_LBUTTONUP:
                case WM_LBUTTONDBLCLK:
                    return MouseButtons.Left;
                case WM_RBUTTONDOWN:
                case WM_RBUTTONUP:
                case WM_RBUTTONDBLCLK:
                    return MouseButtons.Right;
                case WM_MBUTTONDOWN:
                case WM_MBUTTONUP:
                case WM_MBUTTONDBLCLK:
                    return MouseButtons.Middle;
                default:
                    return MouseButtons.None;
            }
        }

        private MouseEventType GetEventType(Int32 wParam)
        {
            switch (wParam)
            {
                case WM_LBUTTONDOWN:
                case WM_RBUTTONDOWN:
                case WM_MBUTTONDOWN:
                    return MouseEventType.MouseDown;
                case WM_LBUTTONUP:
                case WM_RBUTTONUP:
                case WM_MBUTTONUP:
                    return MouseEventType.MouseUp;
                case WM_LBUTTONDBLCLK:
                case WM_RBUTTONDBLCLK:
                case WM_MBUTTONDBLCLK:
                    return MouseEventType.DoubleClick;
                case WM_MOUSEWHEEL:
                    return MouseEventType.MouseWheel;
                case WM_MOUSEMOVE:
                    return MouseEventType.MouseMove;
                default:
                    return MouseEventType.None;
            }
        }

        #endregion
    }
}