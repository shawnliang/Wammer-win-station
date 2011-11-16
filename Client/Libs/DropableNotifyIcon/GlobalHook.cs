/**
 * GlobalHook.cs
 * Class written by CodeSummoner
 * http://www.codeproject.com/KB/system/globalmousekeyboardlib.aspx
 */

#region

using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

#endregion

namespace Waveface.Component.DropableNotifyIcon
{
    // Abstract base class for Mouse and Keyboard hooks
    public abstract class GlobalHook
    {
        #region Windows API Code

        protected const int WH_MOUSE_LL = 14;
        protected const int WH_KEYBOARD_LL = 13;

        protected const int WH_MOUSE = 7;
        protected const int WH_KEYBOARD = 2;
        protected const int WM_MOUSEMOVE = 0x200;
        protected const int WM_LBUTTONDOWN = 0x201;
        protected const int WM_RBUTTONDOWN = 0x204;
        protected const int WM_MBUTTONDOWN = 0x207;
        protected const int WM_LBUTTONUP = 0x202;
        protected const int WM_RBUTTONUP = 0x205;
        protected const int WM_MBUTTONUP = 0x208;
        protected const int WM_LBUTTONDBLCLK = 0x203;
        protected const int WM_RBUTTONDBLCLK = 0x206;
        protected const int WM_MBUTTONDBLCLK = 0x209;
        protected const int WM_MOUSEWHEEL = 0x020A;
        protected const int WM_KEYDOWN = 0x100;
        protected const int WM_KEYUP = 0x101;
        protected const int WM_SYSKEYDOWN = 0x104;
        protected const int WM_SYSKEYUP = 0x105;

        protected const byte VK_SHIFT = 0x10;
        protected const byte VK_CAPITAL = 0x14;
        protected const byte VK_NUMLOCK = 0x90;

        protected const byte VK_LSHIFT = 0xA0;
        protected const byte VK_RSHIFT = 0xA1;
        protected const byte VK_LCONTROL = 0xA2;
        protected const byte VK_RCONTROL = 0x3;
        protected const byte VK_LALT = 0xA4;
        protected const byte VK_RALT = 0xA5;

        protected const byte LLKHF_ALTDOWN = 0x20;

        [DllImport("user32.dll", CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        protected static extern int SetWindowsHookEx(
            int idHook,
            HookProc lpfn,
            IntPtr hMod,
            int dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        protected static extern int UnhookWindowsHookEx(int idHook);


        [DllImport("user32.dll", CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        protected static extern int CallNextHookEx(
            int idHook,
            int nCode,
            int wParam,
            IntPtr lParam);

        [DllImport("user32")]
        protected static extern int ToAscii(
            int uVirtKey,
            int uScanCode,
            byte[] lpbKeyState,
            byte[] lpwTransKey,
            int fuState);

        [DllImport("user32")]
        protected static extern int GetKeyboardState(byte[] pbKeyState);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        protected static extern short GetKeyState(int vKey);

        #region Nested type: HookProc

        protected delegate int HookProc(int nCode, int wParam, IntPtr lParam);

        #endregion

        #region Nested type: KeyboardHookStruct

        [StructLayout(LayoutKind.Sequential)]
        protected class KeyboardHookStruct
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }

        #endregion

        #region Nested type: MouseHookStruct

        [StructLayout(LayoutKind.Sequential)]
        protected class MouseHookStruct
        {
            public POINT pt;
            public int hwnd;
            public int wHitTestCode;
            public int dwExtraInfo;
        }

        #endregion

        #region Nested type: MouseLLHookStruct

        [StructLayout(LayoutKind.Sequential)]
        protected class MouseLLHookStruct
        {
            public POINT pt;
            public int mouseData;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }

        #endregion

        #region Nested type: POINT

        [StructLayout(LayoutKind.Sequential)]
        protected class POINT
        {
            public int x;
            public int y;
        }

        #endregion

        #endregion

        #region Private Variables

        protected int m_handleToHook;
        protected HookProc m_hookCallback;
        protected int m_hookType;
        protected bool m_isStarted;

        #endregion

        #region Properties

        public bool IsStarted
        {
            get { return m_isStarted; }
        }

        #endregion

        #region Constructor

        public GlobalHook()
        {
            Application.ApplicationExit += Application_ApplicationExit;
        }

        #endregion

        #region Methods

        public void Start()
        {
            if (!m_isStarted && (m_hookType != 0))
            {
                // Make sure we keep a reference to this delegate!
                // If not, GC randomly collects it, and a NullReference exception is thrown
                m_hookCallback = HookCallbackProcedure;

                m_handleToHook = SetWindowsHookEx(
                    m_hookType,
                    m_hookCallback,
                    Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]),
                    0);

                // Were we able to sucessfully start hook?
                if (m_handleToHook != 0)
                {
                    m_isStarted = true;
                }
            }
        }

        public void Stop()
        {
            if (m_isStarted)
            {
                UnhookWindowsHookEx(m_handleToHook);

                m_isStarted = false;
            }
        }

        protected virtual int HookCallbackProcedure(int nCode, Int32 wParam, IntPtr lParam)
        {
            // This method must be overriden by each extending hook
            return 0;
        }

        protected void Application_ApplicationExit(object sender, EventArgs e)
        {
            if (m_isStarted)
            {
                Stop();
            }
        }

        #endregion
    }
}