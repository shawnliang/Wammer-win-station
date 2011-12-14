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
            bool _createdNew;
            Mutex _mutex = new Mutex(true, "Waveface Windows Client", out _createdNew);

            if (!_createdNew)
            {
                MessageBox.Show("Another instance is already running.");
                return;
            }

            CultureManager.ApplicationUICulture = CultureInfo.CurrentCulture;
            //CultureManager.ApplicationUICulture = new CultureInfo("en-US");
            //CultureManager.ApplicationUICulture = new CultureInfo("zh-TW");

            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                ProgramSetting settings = new ProgramSetting();

                LoginForm loginForm = null;
                if (args.Length == 3)
                {
                    string _email = args[0];
                    string _password = args[1];
                    string _token = args[2];
                    
                    settings.Email = _email;
                    settings.Password = _password;
                    settings.StationToken = _token;
                    settings.Save();

                    loginForm = new LoginForm(_email, _password);
                }
                else if (settings.IsLoggedIn)
                {
                    loginForm = new LoginForm(settings.Email, settings.Password);
                }
                else
                {
                    loginForm = new LoginForm("", "");
                }

                Application.Run(loginForm);
                
            }
            catch (Exception _e)
            {
                CrashReporter _errorDlg = new CrashReporter(_e);
                _errorDlg.ShowDialog();
            }

            GC.KeepAlive(_mutex);
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