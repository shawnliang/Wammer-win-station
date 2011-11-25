using System;
using System.Threading;
using System.Windows.Forms;
using Waveface.Diagnostics;

namespace Waveface
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
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