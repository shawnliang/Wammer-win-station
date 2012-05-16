#region

using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using NLog;
using Waveface.Component;
using Waveface.Diagnostics;
using Waveface.Localization;

#endregion

namespace Waveface
{
    internal static class Program
    {
        private static Logger s_logger = LogManager.GetCurrentClassLogger();

        #region DllImport

        [DllImport("user32.dll")]
        private static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

        [DllImport("user32.dll")]
        private static extern bool BringWindowToTop(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);

        [DllImport("kernel32.dll")]
        private static extern uint GetCurrentThreadId();

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);

        #endregion

        public static bool ShowCrashReporter { get; set;}

        [STAThread]
        private static void Main(string[] args)
        {
            s_logger.Trace("==================== Windows Client Start ====================");

            if (!SingleInstance.Start())
            {
                SingleInstance.ShowFirstInstance();
                return;
            }

            string _culture = (string)StationRegHelper.GetValue("Culture", null);

            if (_culture == null)
                CultureManager.ApplicationUICulture = CultureInfo.CurrentCulture;
            else
                CultureManager.ApplicationUICulture = new CultureInfo(_culture);


            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                LoginForm _loginForm;

                if (args.Length == 2)
                {
                    string _email = args[0];
                    string _password = args[1];

                    _loginForm = new LoginForm(_email, _password);
                }
                else if (args.Length == 1)
                {
                    Main mainUI = new Main(args[0]);
                    Application.Run(mainUI);

                    if (mainUI.QuitOption == QuitOption.Unlink)
                        Environment.Exit(-3);
                    else if (mainUI.QuitOption == QuitOption.Logout)
                        Environment.Exit(-2);

                    return;
                }
                else
                {
#if DEBUG
                    _loginForm = new LoginForm();
#else
                    var path = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
                    var systemTrayFile = System.IO.Path.Combine(path, "StationSystemTray.exe");
                    Process.Start(systemTrayFile);

                    return;
#endif
                }

                #region force window to have focus

                // please refer http://stackoverflow.com/questions/278237/keep-window-on-top-and-steal-focus-in-winforms
                uint _foreThread = GetWindowThreadProcessId(GetForegroundWindow(), IntPtr.Zero);
                uint _appThread = GetCurrentThreadId();

                if (_foreThread != _appThread)
                {
                    AttachThreadInput(_foreThread, _appThread, true);
                    BringWindowToTop(_loginForm.Handle);

                    if (args.Length != 3)
                        ShowWindow(_loginForm.Handle, 5); //SW_SHOW

                    AttachThreadInput(_foreThread, _appThread, false);
                }
                else
                {
                    BringWindowToTop(_loginForm.Handle);

                    if (args.Length != 3)
                        ShowWindow(_loginForm.Handle, 5); //SW_SHOW
                }

                _loginForm.Activate();

                #endregion

                Application.Run(_loginForm);
            }
            catch (Exception _e)
            {
                CrashReporter _errorDlg = new CrashReporter(_e);
                _errorDlg.ShowDialog();
            }

            SingleInstance.Stop();

            s_logger.Trace("==================== Windows Client Exit ====================");
            return;
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            NLogUtility.Exception(s_logger, (Exception)e.ExceptionObject, "CurrentDomain_UnhandledException");

            if (ShowCrashReporter)
            {
                CrashReporter _errorDlg = new CrashReporter((Exception) e.ExceptionObject);
                _errorDlg.ShowDialog();
            }
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            //CrashReporter _errorDlg = new CrashReporter(e.Exception);
            //_errorDlg.ShowDialog();

            //NLogUtility.Exception(s_logger, e.Exception, "Application_ThreadException");
        }
    }
}