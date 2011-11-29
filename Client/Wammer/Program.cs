using System;
using System.Threading;
using System.Windows.Forms;
using Waveface.Diagnostics;

namespace Waveface
{
    internal static class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                string _email;
                string _password;

                if (args.Length == 2)
                {
                    _email = args[0];
                    _password = args[1];
                }
                else
                {
                    LoginForm _loginForm = new LoginForm();
                    DialogResult _dr = _loginForm.ShowDialog();

                    if (_dr != DialogResult.OK)
                        return;

                    _email = _loginForm.User;
                    _password = _loginForm.Password;
                }

                MainForm _mailForm = new MainForm();

                if (!_mailForm.Login(_email, _password))
                {
                    MessageBox.Show("Login Error!");
                }
                else
                {
                    _mailForm.ShowDialog();
                }
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