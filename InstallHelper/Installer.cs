using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Security.Permissions;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Diagnostics;
using System.ServiceProcess;
using System.Runtime.InteropServices;

namespace InstallHelper
{
    [RunInstaller(true)]
    public partial class Installer : System.Configuration.Install.Installer
    {
        private const string WAMMER_SERVICE_INSTALLED = "wammer_service_installed";

        public Installer()
        {
            InitializeComponent();
        }

        [SecurityPermission(SecurityAction.Demand)]
        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);           
            string wammerDir = Path.GetDirectoryName(this.Context.Parameters["assemblypath"]);

            installWammerService(wammerDir);
            stateSaver.Add(WAMMER_SERVICE_INSTALLED, 1);
        }

        private static void installWammerService(string wammerDir)
        {
            string argument = '"' + Path.Combine(wammerDir, "Wammer.Station.Service.exe") + '"';
            runInstallUtil(argument);
        }

        private static void uninstallWammerService(string wammerDir)
        {
            string argument = "/u \"" + Path.Combine(wammerDir, "Wammer.Station.Service.exe") + 
                '"';
            runInstallUtil(argument);
        }

        private static void runInstallUtil(string argument)
        {
            string netFrameworkDir = RuntimeEnvironment.GetRuntimeDirectory();
            string installUtil = Path.Combine(netFrameworkDir, "installUtil.exe");
            using (Process proc = new Process())
            {
                ProcessStartInfo info = new ProcessStartInfo(installUtil, argument);
                info.CreateNoWindow = true;
                info.WindowStyle = ProcessWindowStyle.Hidden;
                proc.StartInfo = info;
                if (!proc.Start())
                    throw new InstallException("Unable to start installUtil.exe");

                if (!proc.WaitForExit(20 * 1000))
                    throw new InstallException("installUtil.exe does not exit in 20 seconds");

                if (proc.ExitCode != 0)
                {
                    System.Windows.Forms.MessageBox.Show(installUtil + " " + argument);
                    throw new InstallException("installUtil.exe failed: " + proc.ExitCode);
                }
            }
        }

        [SecurityPermission(SecurityAction.Demand)]
        public override void Commit(IDictionary savedState)
        {
            base.Commit(savedState);

            string wammerDir = Path.GetDirectoryName(this.Context.Parameters["assemblypath"]);
            Process startUpApp = Process.Start(Path.Combine(wammerDir, "Wammer.Station.StartUp.exe"));
        }

        [SecurityPermission(SecurityAction.Demand)]
        public override void Rollback(IDictionary savedState)
        {
            base.Rollback(savedState);
            string wammerDir = Path.GetDirectoryName(this.Context.Parameters["assemblypath"]);

            if (savedState.Contains(WAMMER_SERVICE_INSTALLED))
                uninstallWammerService(wammerDir);
        }

        [SecurityPermission(SecurityAction.Demand)]
        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);

            string wammerDir = Path.GetDirectoryName(this.Context.Parameters["assemblypath"]);

            if (savedState.Contains(WAMMER_SERVICE_INSTALLED))
                uninstallWammerService(wammerDir);
        }

    }
}
