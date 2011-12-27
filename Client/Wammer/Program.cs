using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using NLog;
using Waveface.Compoment;
using Waveface.Diagnostics;
using Waveface.Localization;

namespace Waveface
{
    internal static class Program
    {
        private static Logger s_logger = LogManager.GetCurrentClassLogger();

        [STAThread]
        private static void Main(string[] args)
        {
            s_logger.Trace("==================== Windows Client Start ====================");

            if (!SingleInstance.Start())
            {
                SingleInstance.ShowFirstInstance();
                return;
            }

            string culture = (string)StationRegHelper.GetValue("Culture", null);
            if (culture==null)
                CultureManager.ApplicationUICulture = CultureInfo.CurrentCulture;
            else
                CultureManager.ApplicationUICulture = new CultureInfo(culture);


            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                ProgramSetting settings = new ProgramSetting();

                LoginForm loginForm;

                if (args.Length == 3)
                {
                    string _email = args[0];
                    string _password = args[1];
                    string _token = args[2];

                    settings.Email = _email;
                    settings.Password = _password;
                    settings.StationToken = _token;
                    settings.Save();

                    loginForm = new LoginForm(_email, _password, true);
                }
                else if (settings.IsLoggedIn)
                {
                    loginForm = new LoginForm(settings.Email, settings.Password, true);
                }
                else
                {
                    loginForm = new LoginForm(settings.Email, settings.Password, false);
                }

                Application.Run(loginForm);
            }
            catch (Exception _e)
            {
                CrashReporter _errorDlg = new CrashReporter(_e);
                _errorDlg.ShowDialog();
            }

            SingleInstance.Stop();

            s_logger.Trace("==================== Windows Client Exit ====================");
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

            //NLogUtility.Exception(s_logger, e.Exception, "Application_ThreadException");
        }
    }
}