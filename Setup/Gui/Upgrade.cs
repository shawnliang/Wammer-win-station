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
		static Migration()
		{
			// make sure MongoDB is started
			ServiceController svc = new ServiceController("MongoDBForWaveface");
			if (svc.Status != ServiceControllerStatus.Running &&
				svc.Status != ServiceControllerStatus.StartPending)
				svc.Start();

			svc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromMinutes(2));
		}


		public static void DoBackup()
		{
			try
			{
				if (HasFeaure("MainFeature"))
				{
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

		private static void MongoDump()
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
				throw new Exception("Backing up mongo db failed: " + p.ExitCode);

			MongoServer.Create("mongodb://localhost:10319/?safe=true").GetDatabase("wammer").GetCollection("station").RemoveAll();
		}

		private static void BackupClientAppData()
		{
			try
			{
				string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
				string wavefaceData = Path.Combine(appData, "Waveface");
				string BackupData = Path.Combine(appData, "oldWaveface");

				if (Directory.Exists(BackupData))
					Directory.Delete(BackupData, true);

				if (Directory.Exists(wavefaceData))
					Directory.Move(wavefaceData, BackupData);
			}
			catch (Exception e)
			{
				throw new Exception("Unable to backup Waveface Client application data: " + e.Message);
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
			}
			catch (Exception e)
			{
				throw new Exception("Unable to backup station id in registry: " + e.Message);
			}
		}

		private static bool HasFeaure(string featureId)
		{
			foreach (Feature feature in MsiConnection.Instance.Features)
				if (feature.Id == featureId)
				{
					return true;
				}

			return false;
		}

	}
}