using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Diagnostics;
using Microsoft.Deployment.WindowsInstaller;
using System.Configuration;
using Wammer.Cloud;
using Wammer.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using log4net;
using System.Reflection;
using System.ServiceProcess;
using Wammer.Station.Service;

namespace Wammer.Station
{
	public class InstallHelper
	{
		private static ILog Logger;

		static InstallHelper()
		{
			Stream stream =
				System.Reflection.Assembly.GetExecutingAssembly().
				GetManifestResourceStream("InstallHelper.log4net.config");

			if (stream != null)
				log4net.Config.XmlConfigurator.Configure(stream);

			Logger = log4net.LogManager.GetLogger("InstallHelper");
		}

		[CustomAction]
		public static ActionResult SignoffStation(Session session)
		{
			string wavefaceDir = session["INSTALLLOCATION"];
			if (wavefaceDir == null)
				return ActionResult.Failure;

			try
			{
				StationInfo station = StationCollection.Instance.FindOne();
				if (station == null || station.Id == null || station.SessionToken == null)
				{
					Logger.Info("No station Id or token exist. Skip sign off station.");
					return ActionResult.Success;
				}

				Wammer.Cloud.StationApi.SignOff(new WebClient(), station.Id, station.SessionToken);
				Logger.Info("Sign off station success");
				return ActionResult.Success;
			}
			catch (Exception e)
			{
				Logger.Warn("Sign off station not success. Continue as if without error.", e);
				return ActionResult.Success;
			}
		}

		[CustomAction]
		public static ActionResult KillClientProcess(Session session)
		{
			string wavefaceDir = session["INSTALLLOCATION"];
			if (wavefaceDir == null)
				return ActionResult.Failure;

			KillProcess("WavefaceWindowsClient");
			KillProcess("StationSetup");

			return ActionResult.Success;
		}


		private static void KillProcess(string name)
		{
			try
			{
				Process[] procs = Process.GetProcessesByName(name);
				if (procs == null)
					return;

				foreach (Process p in procs)
					p.Kill();
			}
			catch (Exception e)
			{
				Logger.Warn("Cannot kill process " + name, e);
			}
		}

		[CustomAction]
		public static ActionResult RestoreBackupData(Session session)
		{
			try
			{
				string dumpFolder = Path.Combine(session["INSTALLLOCATION"], @"MongoDB\Backup");

				if (Directory.Exists(dumpFolder))
				{
					Logger.Info(dumpFolder + " exists. Restoring DB....");
					RunProgram(Path.Combine(session["INSTALLLOCATION"], @"MongoDB\mongorestore.exe"), 
						"--port 10319 \"" + dumpFolder + "\"");
					Directory.Delete(dumpFolder, true);
				}
				else
					Logger.Info(dumpFolder + " does not exist. Skip restoring");


				RestoreClientAppData();

				return ActionResult.Success;
			}
			catch (Exception e)
			{
				Logger.Error("Failed to restore saved data", e);
				return ActionResult.Failure;
			}
		}

		private static void RestoreClientAppData()
		{
			try
			{
				string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
				string backupData = Path.Combine(appData, "oldWaveface");
				string restoreData = Path.Combine(appData, "waveface");

				if (Directory.Exists(backupData) && !Directory.Exists(restoreData))
					Directory.Move(backupData, restoreData);
			}
			catch (Exception e)
			{
				Logger.Warn("Unable to restore client app data", e);
			}
		}

		private static void RunProgram(string file, string args)
		{
			using (Process p = new Process())
			{
				p.StartInfo = new ProcessStartInfo(file, args);
				p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
				p.StartInfo.CreateNoWindow = true;
				p.Start();

				p.WaitForExit();

				if (p.ExitCode != 0)
					throw new InstallerException(file + " returned " + p.ExitCode);
			}
		}


		[CustomAction]
		public static ActionResult SetRegistry(Session session)
		{
			try
			{
				string myPath = Assembly.GetExecutingAssembly().Location;
				Configuration config = ConfigurationManager.OpenExeConfiguration(myPath);

				StationRegistry.SetValue("cloudBaseURL", config.AppSettings.Settings["cloudBaseURL"].Value);
			}
			catch (Exception e)
			{
				Logger.Warn("Unable to set cloudBaseURL to registry", e);
			}

			try
			{
				int maxWorker;
				int maxIO;
				int minWorker;
				int minIO;

				System.Threading.ThreadPool.GetMaxThreads(out maxWorker, out maxIO);
				System.Threading.ThreadPool.GetMinThreads(out minWorker, out minIO);

				maxWorker = minWorker * 3 / 2;
				if (maxWorker < 3)
					maxWorker = 3;

				StationRegistry.SetValue("MaxWorkerThreads", maxWorker);
				StationRegistry.SetValue("MaxIOThreads", maxIO);
			}
			catch (Exception e)
			{
				Logger.Warn("Unable to set thread numbers", e);
			}

			return ActionResult.Success;
		}

		[CustomAction]
		public static ActionResult AddPerfCounters(Session session)
		{
			try
			{
				Wammer.PerfMonitor.PerfCounterInstaller.Install();
			}
			catch (Exception e)
			{
				Logger.Warn("Unable to add performance counters", e);

				// rollback
				RemovePerfCounters(session);
			}

			return ActionResult.Success;
		}

		[CustomAction]
		public static ActionResult RemovePerfCounters(Session session)
		{
			try
			{
				if (PerformanceCounterCategory.Exists(Wammer.PerfMonitor.PerfCounter.CATEGORY_NAME))
					PerformanceCounterCategory.Delete(Wammer.PerfMonitor.PerfCounter.CATEGORY_NAME);
			}
			catch (Exception e)
			{
				Logger.Warn("Unable to remove performance counters", e);
			}

			return ActionResult.Success;
		}

		[CustomAction]
		public static ActionResult CleanStationInfo(Session session)
		{
			string wavefaceDir = session["INSTALLLOCATION"];

			if (wavefaceDir == null)
				return ActionResult.Failure;

			try
			{
				StationRegistry.DeleteValue("stationId");
			}
			catch (Exception e)
			{
				Logger.Warn("Unable to delete station id in registry", e);
			}

			try
			{
				DriverCollection.Instance.RemoveAll();
			}
			catch (Exception e)
			{
				Logger.Warn("Unable to delete station driver from MongoDB", e);
			}

			try
			{
				StationCollection.Instance.RemoveAll();
			}
			catch (Exception e)
			{
				Logger.Warn("Unable to delete station info from MongoDB", e);
			}

			try
			{
				CloudStorageCollection.Instance.RemoveAll();
			}
			catch (Exception e)
			{
				Logger.Warn("Unable to delete cloud storage from MongoDB", e);
			}

			try
			{
				ServiceCollection.Instance.RemoveAll();
			}
			catch (Exception e)
			{
				Logger.Warn("Unable to delete service collection from MongoDB", e);
			}

			string userDataFolder = "";
			try
			{
				string appPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
				userDataFolder = Path.Combine(appPath, "waveface");

				Logger.Info("Deleting " + userDataFolder);
				Directory.Delete(userDataFolder, true);
			}
			catch (Exception e)
			{
				Logger.Warn("Unable to delete " + userDataFolder, e);
			}

			string userDataFolder2 = "";
			try
			{
				string appPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
				userDataFolder2 = Path.Combine(appPath, "waveface");

				Logger.Info("Deleting " + userDataFolder2);
				Directory.Delete(userDataFolder2, true);
			}
			catch (Exception e)
			{
				Logger.Warn("Unable to delete " + userDataFolder2, e);
			}

			return ActionResult.Success;
		}

		[CustomAction]
		public static ActionResult StartAndWaitMongoDbReady(Session session)
		{
			try
			{
				string svcName = StationService.MONGO_SERVICE_NAME;

				StartService(svcName);

				int retry = 180;
				while (!IsMongoDBReady() && 0 < retry--)
				{
					System.Threading.Thread.Sleep(TimeSpan.FromSeconds(2.0));
				}

				if (!IsMongoDBReady())
					throw new System.TimeoutException("MongoDB is not ready in 360 seconds");

				Model.Database.RestoreCollection("station", "oldStation");
				Model.Database.RestoreCollection("drivers", "oldDrivers");

				return ActionResult.Success;

			}
			catch (Exception e)
			{
				Logger.Warn(e);
				return ActionResult.Failure;
			}
		}

		[CustomAction]
		public static ActionResult StartWavefaceService(Session session)
		{
			try
			{
				int exitCode;
				using (var process = new Process())
				{
					var startInfo = process.StartInfo;
					startInfo.FileName = "sc";
					startInfo.WindowStyle = ProcessWindowStyle.Hidden;

					// tell Windows that the service should restart if it fails
					startInfo.Arguments = string.Format("failure WavefaceStation reset= 0 actions= restart/10000");

					process.Start();
					if (!process.WaitForExit(60 * 1000))
						throw new System.TimeoutException("Wait SC timeout");

					exitCode = process.ExitCode;

					process.Close();
				}

				if (exitCode != 0)
					throw new InvalidOperationException("sc retuens error: " + exitCode);
			}
			catch (Exception e)
			{
				Logger.Warn("Unable to set WavefaceStation service failure action", e);
			}

			try
			{
				StartService("WavefaceStation");
				return ActionResult.Success;
			}
			catch (Exception e)
			{
				Logger.Error("Unable to start Waveface service", e);
				return ActionResult.Failure;
			}
		}

		private static bool IsMongoDBReady()
		{
			try
			{
				// use mongo db to test if it is ready
				Model.StationCollection.Instance.FindOne();
				return true;
			}
			catch
			{
				return false;
			}
		}

		private static void StartService(string svcName)
		{
			int retry = 3;

			while (0 < retry--)
			{
				try
				{
					ServiceController mongoSvc = new ServiceController(svcName);
					if (mongoSvc.Status != ServiceControllerStatus.Running &&
						mongoSvc.Status != ServiceControllerStatus.StartPending)
					{
						mongoSvc.Start();
						mongoSvc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(60));
					}

					return;
				}
				catch (Exception e)
				{
					if (retry > 0)
						System.Threading.Thread.Sleep(3000);
					else
						throw new InstallerException("Unable to start " + svcName, e);
				}
			}
		}
	}
}
