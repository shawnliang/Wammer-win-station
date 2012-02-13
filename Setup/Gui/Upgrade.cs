using System;
using System.Collections.Generic;
using System.IO;
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
		public static void DoBackup()
		{
			try
			{
				if (HasFeaure("MainFeature"))
				{
					// make sure MongoDB is started
					ServiceController svc = new ServiceController("MongoDBForWaveface");
					if (svc.Status != ServiceControllerStatus.Running &&
						svc.Status != ServiceControllerStatus.StartPending)
						svc.Start();

					svc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromMinutes(2));

					MongoDump();
					BackupRegistry();

					// stop WF service to prevent WinXP pops up "Waveface Installer
					// need to be stopped first".
					StopService("WavefaceStation");
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
			ServiceController svc = new ServiceController(svcName);
			int retry = 10;

			while (retry-- > 0)
			{
				try
				{
					if (svc.Status != ServiceControllerStatus.Stopped &&
						svc.Status != ServiceControllerStatus.StopPending)
						svc.Stop();

					svc.WaitForStatus(ServiceControllerStatus.Stopped);
				}
				catch(Exception e)
				{
					System.Threading.Thread.Sleep(500);
					if (svc.Status == ServiceControllerStatus.Stopped)
						return;

					if (retry == 0)
					{
						System.Windows.Forms.MessageBox.Show(e.ToString());
						throw;

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
			StartMongoDB();

			try
			{
				string appRoot = MsiConnection.Instance.GetPath("INSTALLLOCATION");
				string dumpFolder = Path.Combine(appRoot, @"MongoDB\Backup");

				Process p = new Process();
				ProcessStartInfo info = new ProcessStartInfo(
					"mongodump.exe",
					string.Format("--port 10319 --forceTableScan --out \"{0}\" --db wammer", dumpFolder));

				info.CreateNoWindow = true;
				info.WindowStyle = ProcessWindowStyle.Hidden;
				p.StartInfo = info;
				p.Start();
				p.WaitForExit();

				if (p.ExitCode != 0)
					throw new DataBackupException("mongodump.exe returns failure: " + p.ExitCode);

				// delete station collection to prevent previous version's uninstaller unregistering station. 
				MongoServer.Create("mongodb://127.0.0.1:10319/?safe=true").GetDatabase("wammer").GetCollection("station").RemoveAll();
			}
			catch (Exception e)
			{
				throw new DataBackupException("Error using mongodump.exe to backup MongoDB", e);
			}
		}

		private static void StartMongoDB()
		{
			try
			{
				ServiceController svc = new ServiceController("MongoDBForWaveface");
				if (svc.Status != ServiceControllerStatus.Running &&
					svc.Status != ServiceControllerStatus.StartPending)
					svc.Start();

				svc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromMinutes(1.0));
			}
			catch (Exception e)
			{
				throw new DataBackupException("Unable to start MongoDB", e);
			}


			DateTime start = DateTime.Now;
			bool mongoDBOK = false;
			do
			{
				try
				{
					MongoServer sv = MongoServer.Create("mongodb://127.0.0.1:10319/?safe=true");
					mongoDBOK = true;
				}
				catch
				{
					mongoDBOK = false;
				}
			}
			while (!mongoDBOK && DateTime.Now - start < TimeSpan.FromMinutes(5.0));

			if (!mongoDBOK)
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
			catch (Exception)
			{
				//System.Windows.Forms.MessageBox.Show("Unable to backup client AppData: " + e.ToString());
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
					p.Kill();
				}
			}
			catch (Exception)
			{
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