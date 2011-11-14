using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Security.Permissions;
using System.IO;
using System.Net;
using System.Xml;
using System.Xml.XPath;
using System.Diagnostics;
using System.ServiceProcess;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Wammer.Station.Service;
using Wammer.Station;
using log4net;
using MongoDB.Driver;
using MongoDB.Bson;

namespace InstallHelper
{
	[RunInstaller(true)]
	public partial class Installer : System.Configuration.Install.Installer
	{
		private const string WAMMER_SERVICE_INSTALLED = "wammer_service_installed";
		private const string MONGO_INSTALLED = "mongo_installed";

		private static log4net.ILog logger = null;

		public Installer()
		{
			InitializeComponent();

			log4net.Config.XmlConfigurator.Configure(
									System.Reflection.Assembly.GetExecutingAssembly().GetFile(
																"InstallHelper.log4net.config"));
			logger = log4net.LogManager.GetLogger("installHelper");
		}

		//[SecurityPermission(SecurityAction.Demand)]
		public override void Install(IDictionary stateSaver)
		{
			base.Install(stateSaver);
			string wammerDir = Path.GetDirectoryName(this.Context.Parameters["assemblypath"]);

			try
			{
				installMongoDbService(wammerDir);
				stateSaver.Add(MONGO_INSTALLED, 1);
				installWammerService(wammerDir);
				stateSaver.Add(WAMMER_SERVICE_INSTALLED, 1);
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message, "Error during installation");
				logger.Error("Error during installtion", e);
				throw;
			}
		}

		private static void installMongoDbService(string wammerDir)
		{
			string mongoDir = Path.Combine(wammerDir, "Mongo");
			string dataDir = Path.Combine(mongoDir, "Data");
			string dbDir = Path.Combine(dataDir, "DB");

			string filename = Path.Combine(mongoDir, "mongod.exe");
			string args = string.Format("--port 10319 --bind_ip 127.0.0.1 --logpath \"{0}\" " +
					"--dbpath \"{1}\" --journal --install --serviceName \"{2}\"",
					Path.Combine(dataDir, "mongo.log"), dbDir, StationService.MONGO_SERVICE_NAME);
			string purposeText = "Install MongoDB service";

			RunProgram(filename, args, purposeText);
		}

		private static void uninstallMongoDbService(string wammerDir)
		{
			string mongoDir = Path.Combine(wammerDir, "Mongo");
			string dataDir = Path.Combine(mongoDir, "Data");
			string dbDir = Path.Combine(dataDir, "DB");

			string filename = Path.Combine(mongoDir, "mongod.exe");
			string args = string.Format("--remove --serviceName \"{0}\"",
				StationService.MONGO_SERVICE_NAME);
			string purposeText = "Remove MongoDB service";

			RunProgram(filename, args, purposeText);
		}

		private static void RunProgram(string filename, string args, string purposeText)
		{
			using (Process proc = new Process())
			{
				ProcessStartInfo info = new ProcessStartInfo(filename, args);
				info.CreateNoWindow = true;
				info.WindowStyle = ProcessWindowStyle.Hidden;
				proc.StartInfo = info;
				if (!proc.Start())
					throw new InstallException("Unable to start " + filename);

				if (!proc.WaitForExit(60 * 1000))
				{
					proc.Kill();
					throw new InstallException(filename + "is running too long...");
				}

				if (proc.ExitCode != 0)
				{
					System.Windows.Forms.MessageBox.Show(purposeText + " not success:\r\n\r\n" +
						"cmd = " + filename + " " + args);
					throw new InstallException(filename + " exited with error code: " + proc.ExitCode);
				}
			}
		}
		private static void installWammerService(string wammerDir)
		{
			string argument = '"' + Path.Combine(wammerDir, "Wammer.Station.Service.exe") + '"';
			runInstallUtil(argument, "Install Wammer Station service");
		}

		private static void uninstallWammerService(string wammerDir)
		{
			string argument = "/u \"" + Path.Combine(wammerDir, "Wammer.Station.Service.exe") + 
				'"';
			runInstallUtil(argument, "Uninstall Wammer Station service");
		}

		private static void runInstallUtil(string argument, string purposeText)
		{
			string netFrameworkDir = RuntimeEnvironment.GetRuntimeDirectory();
			string installUtil = Path.Combine(netFrameworkDir, "installUtil.exe");

			RunProgram(installUtil, argument, purposeText);
		}

		private static void startService(string serviceName)
		{
			using (ServiceController ctrl = new ServiceController(serviceName))
			{
				if (ctrl.Status != ServiceControllerStatus.Running || 
					ctrl.Status != ServiceControllerStatus.StartPending)
				{
					ctrl.Start();
					ctrl.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(20));

					if (ctrl.Status != ServiceControllerStatus.Running)
						throw new InstallException("Unable to start service: " + serviceName);
				}
			}
		}

		private static void stopService(string serviceName)
		{
			using (ServiceController ctrl = new ServiceController(serviceName))
			{
				if (ctrl.Status != ServiceControllerStatus.Stopped)
				{
					ctrl.Stop();
					ctrl.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(20));

					if (ctrl.Status != ServiceControllerStatus.Stopped)
						throw new InstallException("Unable to stop service: " + serviceName);
				}
			}
		}


		//[SecurityPermission(SecurityAction.Demand)]
		public override void Commit(IDictionary savedState)
		{
			base.Commit(savedState);
			string wammerDir = Path.GetDirectoryName(this.Context.Parameters["assemblypath"]);

			try
			{
				startService(StationService.MONGO_SERVICE_NAME);
				startService(StationService.SERVICE_NAME);
				Process startUpApp = Process.Start(Path.Combine(wammerDir, 
																"Wammer.Station.AddUserApp.exe"));
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message, "Error during commit");
				logger.Error("Error during commit", e);
				throw;
			}
		}

		private static void RemoveStationAndMongoSvc(IDictionary savedState, string wammerDir)
		{
			if (savedState.Contains(WAMMER_SERVICE_INSTALLED))
			{
				stopService(StationService.SERVICE_NAME);
				uninstallWammerService(wammerDir);
			}

			if (savedState.Contains(MONGO_INSTALLED))
			{
				try
				{
					stopService(StationService.MONGO_SERVICE_NAME);
				}
				catch (Exception e)
				{

				}
				uninstallMongoDbService(wammerDir);
			}
		}

		public void SignOffStation()
		{
			try
			{
				MongoServer mongo = MongoServer.Create("mongodb://localhost:10319/?safe=true");
				MongoCollection sDocs = mongo.GetDatabase("wammer").GetCollection("station");
				if (sDocs == null)
					return;

				StationDBDoc station = sDocs.FindOneAs<StationDBDoc>();
				if (station == null || station.Id == null || station.SessionToken == null)
					return;

				logger.Info("Sign off station");
				logger.Info("id: " + station.Id);
				logger.Info("token: " + station.SessionToken);
				Wammer.Cloud.Station.SignOff(new WebClient(), station.Id, station.SessionToken);
			}
			catch (Exception e)
			{
				logger.Warn("unable to unregister station", e);
			}
		}

		//[SecurityPermission(SecurityAction.Demand)]
		public override void Rollback(IDictionary savedState)
		{
			base.Rollback(savedState);
			string wammerDir = Path.GetDirectoryName(this.Context.Parameters["assemblypath"]);

			try
			{
				RemoveStationAndMongoSvc(savedState, wammerDir);
			}
			catch (Exception e)
			{
				logger.Warn("Error during rollback", e);
				throw;
			}
		}

		//[SecurityPermission(SecurityAction.Demand)]
		public override void Uninstall(IDictionary savedState)
		{
			base.Uninstall(savedState);
			string wammerDir = Path.GetDirectoryName(this.Context.Parameters["assemblypath"]);

			try
			{
				SignOffStation();
				RemoveStationAndMongoSvc(savedState, wammerDir);
			}
			catch (Exception e)
			{
				//MessageBox.Show(e.Message, "Error during uninstall");
				logger.Warn("Error during uninstall", e);
				throw;
			}
		}
	}
}
