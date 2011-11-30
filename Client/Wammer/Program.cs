using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using Waveface.Diagnostics;
using Waveface.Localization;

namespace Waveface
{
    internal static class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            CultureManager.ApplicationUICulture = CultureInfo.CurrentCulture;
            //CultureManager.ApplicationUICulture = new CultureInfo("en-US");
            //CultureManager.ApplicationUICulture = new CultureInfo("zh-TW");

            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                string _email = string.Empty;
                string _password = string.Empty;

                if (args.Length == 2)
                {
                    _email = args[0];
                    _password = args[1];
                }

                Application.Run(new LoginForm(_email, _password));
            }
            catch (Exception _e)
            {
                CrashReporter _errorDlg = new CrashReporter(_e);
                _errorDlg.ShowDialog();
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            CrashReporter _errorDlg = new CrashReporter((Exception)e.ExceptionObject);
            _errorDlg.ShowDialog();
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            //CrashReporter _errorDlg = new CrashReporter(e.Exception);
            //_errorDlg.ShowDialog();
        }
    }
}