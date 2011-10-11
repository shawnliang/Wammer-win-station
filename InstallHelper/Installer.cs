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

namespace InstallHelper
{
    [RunInstaller(true)]
    public partial class Installer : System.Configuration.Install.Installer
    {
        const string NGINX_SVC_INSTALLED = "nginx_svc_installed";

        public Installer()
        {
            InitializeComponent();
        }

        [SecurityPermission(SecurityAction.Demand)]
        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);
           
            string wammerDir = Path.GetDirectoryName(this.Context.Parameters["assemblypath"]);
            string nginxDir = Path.Combine(wammerDir, "nginx");
            string nginxSvcName = "nginx for Wammer";
            try
            {
                // nginx
                createNginxDirs(nginxDir);
                writeNginxPathToSvcConfig(nginxDir, nginxSvcName);
                // TODO: config nginx port/fastcgi port/...
                installNginxService(nginxDir);
                stateSaver[NGINX_SVC_INSTALLED] = "true";
            }
            catch (Exception e)
            {
                using (FileStream f = File.OpenWrite("C:\\wammer-install.log"))
                using (StreamWriter sw = new StreamWriter(f))
                {
                    sw.WriteLine(e.ToString());
                }

                throw;
            }
            
        }

        private static void nginxService(string nginxDir, string cmd)
        {
            Process proc = Process.Start(
                Path.Combine(nginxDir, "ng_srv.exe"), cmd);
            if (!proc.WaitForExit(20 * 1000))
            {
                proc.Kill();
                throw new System.TimeoutException("ng_srv.exe timeout");
            }

            if (proc.ExitCode != 0)
                throw new InstallException(
                    string.Format("ng_srv.exe exit with {0}", proc.ExitCode));
        }

        private static void installNginxService(string nginxDir)
        {
            nginxService(nginxDir, "install");
        }

        private static void removeNginxService(string nginxDir)
        {
            nginxService(nginxDir, "uninstall");
        }

        private static void writeNginxPathToSvcConfig(string nginxDir,
            string nginxSvcName)
        {
            try
            {
                string svcConfigFile = Path.Combine(nginxDir, "ng_srv.xml");
                XmlDocument doc = new XmlDocument();
                doc.Load(svcConfigFile);
                XmlNode nameNode = doc.SelectSingleNode("/service/name");
                nameNode.InnerText = nginxSvcName;
                XmlNode exeNode = doc.SelectSingleNode("/service/executable");
                exeNode.InnerText = Path.Combine(nginxDir, "nginx.exe");
                XmlNode logNode = doc.SelectSingleNode("/service/logpath");
                logNode.InnerText = Path.Combine(nginxDir, "logs");
                XmlNode startNode = doc.SelectSingleNode("/service/startargument");
                startNode.InnerText = "-p \"" + nginxDir + "\"";
                XmlNode stopNode = doc.SelectSingleNode("/service/stopargument");
                stopNode.InnerText = "-p \"" + nginxDir + "\" -s stop";
                doc.Save(svcConfigFile);
            }
            catch (Exception e)
            {
                throw new InstallException("error writing ng_srv.xml", e);
            }
        }

        private static void createNginxDirs(string nginxDir)
        {
            string logDir = Path.Combine(nginxDir, "logs");
            string tmpDir = Path.Combine(nginxDir, "temp");
            if (!Directory.Exists(logDir))
                Directory.CreateDirectory(logDir);
            if (!Directory.Exists(tmpDir))
                Directory.CreateDirectory(tmpDir);
        }

        [SecurityPermission(SecurityAction.Demand)]
        public override void Commit(IDictionary savedState)
        {
            base.Commit(savedState);

            string nginxSvcName = "nginx for Wammer";
            ServiceController svc = new ServiceController(nginxSvcName);
            if (svc.Status != ServiceControllerStatus.Running)
            {
                svc.Start();
                svc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
            }

            string wammerDir = Path.GetDirectoryName(this.Context.Parameters["assemblypath"]);
            Process startUpApp = Process.Start(Path.Combine(wammerDir, "Wammer.Station.StartUp.exe"));

        }

        [SecurityPermission(SecurityAction.Demand)]
        public override void Rollback(IDictionary savedState)
        {
            base.Rollback(savedState);

            string wammerDir = Path.GetDirectoryName(this.Context.Parameters["assemblypath"]);
            string nginxDir = Path.Combine(wammerDir, "nginx");

            uninstallNginx(savedState, nginxDir);
        }

        [SecurityPermission(SecurityAction.Demand)]
        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);

            string wammerDir = Path.GetDirectoryName(this.Context.Parameters["assemblypath"]);
            string nginxDir = Path.Combine(wammerDir, "nginx");

            uninstallNginx(savedState, nginxDir);

        }

        private static void uninstallNginx(IDictionary savedState, string nginxDir)
        {
            try
            {
                string nginxSvcName = "nginx for Wammer";
                ServiceController svc = new ServiceController(nginxSvcName);
                if (svc.Status != ServiceControllerStatus.Stopped)
                {
                    svc.Stop();
                    svc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(20));
                }

                if (savedState[NGINX_SVC_INSTALLED] != null)
                    removeNginxService(nginxDir);
            }
            catch (Exception e)
            {
                using (FileStream f = File.OpenWrite("C:\\wammer-uninstall.log"))
                using (StreamWriter sw = new StreamWriter(f))
                {
                    sw.WriteLine(e.ToString());
                }
            }
        }

    }
}
