
#region

using System;
using System.Windows;

#endregion

namespace Waveface.Configuration
{
    public class WindowsApplicationSettings : ApplicationSettings
    {
        private readonly Application m_application;
        private bool m_saveOnClose = true;

        public Application Application
        {
            get { return m_application; }
        }

        public bool SaveOnClose
        {
            get { return m_saveOnClose; }
            set { m_saveOnClose = value; }
        }

        public WindowsApplicationSettings(Application m_application) :
            this(m_application, m_application.GetType())
        {
        }

        public WindowsApplicationSettings(Application m_application, Type type) :
            this(m_application, type.Name)
        {
        }

        public WindowsApplicationSettings(Application application, string settingsKey) :
            base(settingsKey)
        {
            if (application == null)
            {
                throw new ArgumentNullException("application");
            }

            m_application = application;
            application.Startup += ApplicationStartup;
            application.Exit += ApplicationExit;
        }

        private void ApplicationStartup(object sender, StartupEventArgs e)
        {
            Load();
        }

        private void ApplicationExit(object sender, ExitEventArgs e)
        {
            if (m_saveOnClose == false)
            {
                return;
            }

            Save();
        }
    }
}