#region

using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

#endregion

namespace Waveface.Component
{

    /*
    *	SingeInstance
    *
    *	This is where the magic happens.
    *
    *	Start() tries to create a mutex.
    *	If it detects that another instance is already using the mutex, then it returns FALSE.
    *	Otherwise it returns TRUE.
    *	(Notice that a GUID is used for the mutex name, which is a little better than using the application name.)
    *
    *	If another instance is detected, then you can use ShowFirstInstance() to show it
    *	(which will work as long as you override WndProc as shown above).
    *
    *	ShowFirstInstance() broadcasts a message to all windows.
    *	The message is WM_SHOWFIRSTINSTANCE.
    *	(Notice that a GUID is used for WM_SHOWFIRSTINSTANCE.
    *	That allows you to reuse this code in multiple applications without getting
    *	strange results when you run them all at the same time.)
    *
    */

    public static class SingleInstance
    {
        #region WinApi

        public static class WinApi
        {
            public const int HWND_BROADCAST = 0xffff;
            public const int SW_SHOWNORMAL = 1;

            [DllImport("user32")]
            public static extern int RegisterWindowMessage(string message);

            public static int RegisterWindowMessage(string format, params object[] args)
            {
                string _message = String.Format(format, args);
                return RegisterWindowMessage(_message);
            }

            [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
            public static extern bool SendNotifyMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

            [DllImport("user32")]
            public static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);

            [DllImportAttribute("user32.dll")]
            public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

            [DllImportAttribute("user32.dll")]
            public static extern bool SetForegroundWindow(IntPtr hWnd);

            public static void ShowToFront(IntPtr window)
            {
                ShowWindow(window, SW_SHOWNORMAL);
                SetForegroundWindow(window);
            }
        }

        #endregion

        #region ProgramInfo

        public static class ProgramInfo
        {
            public static string AssemblyGuid
            {
                get
                {
                    object[] _attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(GuidAttribute), false);

                    if (_attributes.Length == 0)
                    {
                        return String.Empty;
                    }

                    return ((GuidAttribute)_attributes[0]).Value;
                }
            }

            public static string AssemblyTitle
            {
                get
                {
                    object[] _attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);

                    if (_attributes.Length > 0)
                    {
                        AssemblyTitleAttribute _titleAttribute = (AssemblyTitleAttribute)_attributes[0];

                        if (_titleAttribute.Title != "")
                        {
                            return _titleAttribute.Title;
                        }
                    }

                    return Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().CodeBase);
                }
            }
        }

        #endregion

        public static readonly int WM_SHOWFIRSTINSTANCE =
            WinApi.RegisterWindowMessage("WM_SHOWFIRSTINSTANCE|{0}", ProgramInfo.AssemblyGuid);

        private static Mutex s_mutex;

        public static bool Start()
        {
            bool _onlyInstance;
            string _mutexName = String.Format("Local\\{0}", ProgramInfo.AssemblyGuid);

            // if you want your app to be limited to a single instance
            // across ALL SESSIONS (multiple users & terminal services), then use the following line instead:
            // string mutexName = String.Format("Global\\{0}", ProgramInfo.AssemblyGuid);

            s_mutex = new Mutex(true, _mutexName, out _onlyInstance);
            return _onlyInstance;
        }

        public static void ShowFirstInstance()
        {
            WinApi.SendNotifyMessage(
                (IntPtr)WinApi.HWND_BROADCAST,
                WM_SHOWFIRSTINSTANCE,
                IntPtr.Zero,
                IntPtr.Zero);
        }

        public static void Stop()
        {
            s_mutex.ReleaseMutex();
        }
    }
}