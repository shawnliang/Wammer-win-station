using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.ServiceProcess;
using System.Diagnostics;

using SharpSetup.Base;
using MongoDB.Driver;
using MongoDB.Bson;

namespace Gui
{
	class Migration
	{
		static log4net.ILog logger;

		static Migration()
		{
			InitLogger();
		}

		private static void InitLogger()
		{
			MemoryStream m = new MemoryStream();
			StreamWriter w = new StreamWriter(m, Encoding.UTF8);
			w.WriteLine(
			@"
<log4net>
  <appender name='RollingFile' type='log4net.Appender.RollingFileAppender'>
	<file value='${APPDATA}\WavefaceUpgrade.log' />
	<appendToFile value='true' />
	<maximumFileSize value='100KB' />
	<maxSizeRollBackups value='2' />
	<layout type='log4net.Layout.PatternLayout'>
	  <conversionPattern value='%level %thread %logger - %message%newline' />
	</layout>
  </appender>
  <root>
	<level value='DEBUG' />
	<appender-ref ref='RollingFile' />
  </root>
</log4net>");

			w.Flush();
			m.Position = 0;


			log4net.Config.XmlConfigurator.Configure(m);
			logger = log4net.LogManager.GetLogger("Upgrade");
		}

		public static void DoBackup()
		{
			try
			{
				if (HasFeaure("MainFeature"))
				{
					// stop WF service to prevent WinXP pops up "Waveface Installer
					// need to be stopped first".
					StopService("WavefaceStation");
					MongoDump();
					BackupRegistry();
				}

				BackupClientAppData();
			}
			catch (Exception e)
			{
				System.Windows.Forms.MessageBox.Show(e.Message, "Waveface", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
				throw;
			}
		}

		private static void StopService(string svcName)
		{
			logger.Info("Stop service: " + svcName);

			ServiceController svc = new ServiceController(svcName);
			int retry = 10;

			while (retry-- > 0)
			{
				try
				{
					if (svc.Status != ServiceControllerStatus.Stopped &&
						svc.Status != ServiceControllerStatus.StopPending)
					{
						logger.Info("stop " + svcName);
						svc.Stop();
						svc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10.0));
					}
				}
				catch(Exception e)
				{
					svc.Refresh();
					if (svc.Status == ServiceControllerStatus.Stopped)
						return;

					if (retry == 0)
					{
						logger.Error("stop service failed", e);
						System.Windows.Forms.MessageBox.Show(e.ToString());
						throw;
					}
					else
					{
						logger.Warn("stop service failed. Retry it. " + e.Message);
					}
				}
			}
		}



		public static bool DriverRegistered()
		{
			if (MongoServer.Create("mongodb://127.0.0.1:10319/?safe=true").GetDatabase("wammer").GetCollection("drivers").Count() > 0)
				return true;
			else
				return false;
		}

		private static void MongoDump()
		{
			StopService("MongoDbForWaveface");

			try
			{
				string appRoot = MsiConnection.Instance.GetPath("INSTALLLOCATION");
				string dumpFolder = Path.Combine(appRoot, @"MongoDB\Backup");
				string dbPath = Path.Combine(appRoot, @"MongoDB\Data\DB");

				if (Directory.Exists(dumpFolder))
				{
					logger.Info("deleting old backup folder: " + dumpFolder);
					Directory.Delete(dumpFolder, true);
				}

				logger.Info("dumping mongo db....");

				Process p = new Process();
				ProcessStartInfo info = new ProcessStartInfo(
					"mongodump.exe",
					string.Format("--dbpath \"{1}\" --forceTableScan --out \"{0}\" --db wammer --journal", dumpFolder, dbPath));

				info.CreateNoWindow = true;
				info.WindowStyle = ProcessWindowStyle.Hidden;
				p.StartInfo = info;
				p.Start();
				p.WaitForExit();

				if (p.ExitCode != 0)
					throw new DataBackupException("mongodump.exe returns failure: " + p.ExitCode);

				logger.Info("dump completed");


				// delete station collection to prevent previous version's uninstaller unregistering station. 
				StartMongoDB();
				MongoServer.Create("mongodb://127.0.0.1:10319/?safe=true").GetDatabase("wammer").GetCollection("station").RemoveAll();
			}
			catch (Exception e)
			{
				logger.Error("Dump mongo db error", e);
				throw new DataBackupException("Error using mongodump.exe to backup MongoDB", e);
			}
		}

		private static void StartMongoDB()
		{
			ServiceController svc = new ServiceController("MongoDBForWaveface");

			try
			{
				if (svc.Status != ServiceControllerStatus.Running &&
					svc.Status != ServiceControllerStatus.StartPending)
				{
					logger.Info("Starting Mongo db...");
					svc.Start();
					svc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromMinutes(1.0));
				}
			}
			catch (Exception e)
			{
				svc.Refresh();
				if (svc.Status != ServiceControllerStatus.Running)
				{
					logger.Error("Start mongo db failed", e);
					throw new DataBackupException("Unable to start MongoDB", e);
				}
			}

			logger.Info("Mongo db is running");

			int retry = 60;
			while (!Waveface.Common.MongoDbHelper.IsMongoDBReady("127.0.0.1", 10319) && retry > 0)
			{
				System.Threading.Thread.Sleep(3000);
				retry--;
			}

			if (!Waveface.Common.MongoDbHelper.IsMongoDBReady("127.0.0.1", 10319))
				throw new DataBackupException("MongoDB is not accessible");
		}


		private static void BackupClientAppData()
		{
			try
			{
				KillProcess("StationSystemTray");
				KillProcess("WavefaceWindowsClient");
				KillProcess("StationSetup");
				KillProcess("StationUI");

				System.Threading.Thread.Sleep(1000);

				MoveDir(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "waveface", "oldWaveface");
				MoveDir(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "waveface", "oldWaveface");
			}
			catch (Exception ex)
			{
				logger.Warn("Backup client app data unsuccessfully", ex);
			}
		}

		private static void MoveDir(string parentFolder, string orig, string backup)
		{
			string origName = Path.Combine(parentFolder, orig);
			string newName = Path.Combine(parentFolder, backup);

			if (Directory.Exists(newName))
				Directory.Delete(newName, true);

			if (Directory.Exists(origName))
				Directory.Move(origName, newName);
		}

		private static void KillProcess(string name)
		{
			try
			{
				Process[] procs = Process.GetProcessesByName(name);

				foreach (Process p in procs)
				{
					if (p.CloseMainWindow())
					{
						if (!p.WaitForExit(500))
							p.Kill();
					}
					else
						p.Kill();

					p.WaitForExit(200);
				}
			}
			catch (Exception ex)
			{
				logger.Warn("kill process failed: " + name, ex);
			}
		}

		private static void BackupRegistry()
		{
			try
			{
				string stationId = (string)Microsoft.Win32.Registry.GetValue(
								@"HKEY_LOCAL_MACHINE\Software\Wammer\WinStation",
								"stationId", null);

				if (stationId != null)
					Microsoft.Win32.Registry.SetValue(
						@"HKEY_LOCAL_MACHINE\Software\Wammer\WinStation",
						"oldStationId", stationId);

				string driver = (string)Microsoft.Win32.Registry.GetValue(
				@"HKEY_LOCAL_MACHINE\Software\Wammer\WinStation",
				"driver", null);

				if (driver != null)
					Microsoft.Win32.Registry.SetValue(
						@"HKEY_LOCAL_MACHINE\Software\Wammer\WinStation",
						"olddriver", driver);
			}
			catch (Exception e)
			{
				logger.Error("Unable to backup registry", e);
				throw new Exception("Unable to backup registry: " + e.Message);
			}
		}

		public static bool HasFeaure(string featureId)
		{
			foreach (Feature feature in MsiConnection.Instance.Features)
				if (feature.Id == featureId && feature.State == FeatureState.Installed)
				{
					return true;
				}

			return false;
		}

	}


	class DataBackupException: Exception
	{
		public DataBackupException(string msg, Exception inner)
			:base(msg, inner)
		{}

		public DataBackupException(string msg)
			: base(msg)
		{ }
	}
}