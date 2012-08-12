#region

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.Win32;

#endregion

namespace Waveface
{
    public class ApplicationInformation
    {
        private FileVersionInfo m_version;

        public static bool CheckIfWinXP()
        {
            OperatingSystem _os = Environment.OSVersion;

            if ((_os.Platform == PlatformID.Win32NT) && (_os.Version.Major == 5) && (_os.Version.Minor >= 1))
                return true;
            else
                return false;
        }

        [Category("Process")]
        [Description("The command line and arguments used to start the application.")]
        public string CommandLine
        {
            get { return Environment.CommandLine; }
        }

        [Category("Application")]
        [Description("The name of the company who has developed this application.")]
        public string CompanyName
        {
            get { return m_version.CompanyName; }
        }

        [Category("Application")]
        [Description("A description of the application")]
        public string Description
        {
            get { return m_version.Comments; }
        }

        [Description("The version of Microsoft Internet Explorer installed on the machine.")]
        [Category("Operating System")]
        public string InternetExplorerVersion
        {
            get
            {
                try
                {
                    RegistryKey _registryKey = Registry.LocalMachine.OpenSubKey(
                        "Software\\Microsoft\\Internet Explorer", false);

                    if (_registryKey != null)
                    {
                        string _s = _registryKey.GetValue("Version").ToString();
                        _registryKey.Close();
                        return _s;
                    }
                }
                catch
                {
                }

                return String.Empty;
            }
        }

        [Category("Operating System")]
        [Description("The version of the operating system installed on the machine.")]
        public string OperatingSystemVersion
        {
            get { return Environment.OSVersion.ToString(); }
        }

        [Category("Application")]
        [Description("The name of the product which this application belongs to.")]
        public string ProductName
        {
            get { return m_version.ProductName; }
        }

        [Category("Application")]
        [Description("The version of product.")]
        public string ProductVersion
        {
            get { return m_version.ProductVersion; }
        }

        [Category("Process")]
        [Description("The path where this process was started from.")]
        public string StartupPath
        {
            get { return Application.StartupPath; }
        }

        [Category("Process")]
        [Description("The amount of physical memory mapped into this process.")]
        public long WorkingSet
        {
            get { return Environment.WorkingSet; }
        }

        public ApplicationInformation()
        {
            m_version = FileVersionInfo.GetVersionInfo(base.GetType().Module.FullyQualifiedName);
        }
    }
}