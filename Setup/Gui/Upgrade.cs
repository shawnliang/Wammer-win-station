using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.ServiceProcess;
using SharpSetup.Base;
using MongoDB.Driver;
using MongoDB.Bson;

namespace Gui
{
	class Migration
	{
		static MongoDatabase db;
		
		static Migration()
		{
			// make sure MongoDB is started
			ServiceController svc = new ServiceController("MongoDBForWaveface");
			if (svc.Status != ServiceControllerStatus.Running &&
				svc.Status != ServiceControllerStatus.StartPending)
				svc.Start();

			svc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromMinutes(2));


			// init static vairables
			db = MongoServer.Create("mongodb://localhost:10319/?safe=true").GetDatabase("wammer");
		}


		public static void DoBackup()
		{
			try
			{
				if (HasFeaure("MainFeature"))
				{
					System.Windows.Forms.MessageBox.Show("MainFeature is installed");
					MoveCollection("drivers", "oldDrivers");
					MoveCollection("station", "oldStation");
					MoveCollection("service", "oldService");
					MoveCollection("cloudstorage", "oldCloudstorage");

					BackupRegistry();
				}
				else
					System.Windows.Forms.MessageBox.Show("MainFeature is not installed");


				BackupClientAppData();
			}
			catch (Exception e)
			{
				System.Windows.Forms.MessageBox.Show(e.Message, "Waveface", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
				throw;
			}
		}

		public static void DoRestore()
		{
			try
			{
				RestoreClientAppData();
			}
			catch (Exception e)
			{
				System.Windows.Forms.MessageBox.Show(e.Message, "Waveface", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
			}
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

		private static void RestoreClientAppData()
		{
			try
			{
				string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
				string wavefaceData = Path.Combine(appData, "Waveface");
				string BackupData = Path.Combine(appData, "oldWaveface");

				if (Directory.Exists(BackupData))
					Directory.Move(BackupData, appData);
			}
			catch (Exception e)
			{
				throw new Exception("Unable to restore Waveface Client application data: " + e.Message);
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

		private static void MoveCollection(string collectionName, string backupCollectionName)
		{
			try
			{
				if (db.CollectionExists(collectionName))
					db.RenameCollection(collectionName, backupCollectionName, true);
			}
			catch (Exception e)
			{
				throw new Exception("Unable to rename " + collectionName + " collection: " + e.Message);
			}
		}
	}
}