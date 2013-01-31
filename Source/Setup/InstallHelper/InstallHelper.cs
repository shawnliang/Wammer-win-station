using System;
using System.Collections.Generic;
using System.Linq;
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
using ICSharpCode.SharpZipLib.Tar;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Zip;
using System.Runtime.InteropServices;
using MongoDB.Driver.Builders;
using Wammer.Utility;
using Waveface.Stream.Model;


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
				if (station != null)
				{
					foreach (var user in DriverCollection.Instance.FindAll())
					{
						if (string.IsNullOrEmpty(user.session_token))
							continue;

						try
						{
							StationApi.SignOff(station.Id, user.session_token);
							Logger.Warn("Signoff station is successful");
							break; // use anyone's token to signoff this station once is enough
						}
						catch (Exception e)
						{
							Logger.WarnFormat("Signoff station with user {0}'s token not success: {1}", user.email, e.Message);
						}
					}
				}
			}
			catch (Exception e)
			{
				Logger.Warn("Sign off station not success. Continue as if without error.", e);
			}

			return ActionResult.Success;
		}

		[CustomAction]
		public static ActionResult KillClientProcess(Session session)
		{
			string wavefaceDir = session["INSTALLLOCATION"];
			if (wavefaceDir == null)
				return ActionResult.Failure;

			//CloseStream();
			KillProcess("WindowsClient");

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
				{
					Logger.Info("Kill process " + name);
					p.Kill();
				}
			}
			catch (Exception e)
			{
				Logger.Warn("Cannot kill process " + name, e);
			}
		}

		[DllImport("user32.dll")]
		public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

		[DllImport("user32.dll")]
		public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

		private static void CloseStream()
		{
			Logger.Info("Close Stream gracefully");
			try
			{
				string mainformTitle = "Login - Stream";

				var culture = (string)StationRegistry.GetValue("Culture", null);
				if (culture != null)
				{
					if (culture == "zh-TW")
						mainformTitle = "登入 - Stream";
					else
						mainformTitle = "Login - Stream";
				}

				var handle = FindWindow(null, mainformTitle);

				if (handle == IntPtr.Zero)
					return;

				SendMessage(handle, 0x402, IntPtr.Zero, IntPtr.Zero);

				for (int retry = 0; retry < 10; retry++)
				{
					System.Threading.Thread.Sleep(500);
					if (FindWindow(null, mainformTitle) == IntPtr.Zero)
					{
						Logger.Info("Successfully close Stream");
						break;
					}
				}
			}
			catch (Exception e)
			{
				Logger.Warn("Cannot close Stream", e);
			}
		}

		[CustomAction]
		public static ActionResult RestoreBackupData(Session session)
		{
			string dumpFolder = Path.Combine(session["INSTALLLOCATION"], @"MongoDB\Backup");

			try
			{
				RestoreStationDB(session, dumpFolder);
				RestoreStationId();
				RestoreClientAppData();
				return ActionResult.Success;
			}
			catch (Exception e)
			{
				Logger.Error("Failed to restore saved data", e);
				return ActionResult.Failure;
			}
			finally
			{
				RemoveBackupData(dumpFolder, session["INSTALLLOCATION"]);
			}
		}

		private static void RemoveBackupData(string dumpFolder, string appRoot)
		{
			try
			{
				RemoveDirectory(dumpFolder);

				if (StationRegistry.GetValue("oldStationId", null) != null)
					StationRegistry.DeleteValue("oldStattionId");
			}
			catch (Exception e)
			{
				Logger.Warn("Unable to clean up backup data", e);
			}
		}

		private static void RestoreStationDB(Session session, string dumpFolder)
		{
			if (Directory.Exists(dumpFolder))
			{
				Logger.Warn(dumpFolder + " exists. Restoring DB....");
				RunProgram(Path.Combine(session["INSTALLLOCATION"], @"MongoDB\mongorestore.exe"),
					"--port 10319 \"" + dumpFolder + "\"");

				moveDumpFolder(dumpFolder);
			}
			else
				Logger.Warn(dumpFolder + " does not exist. Skip restoring");
		}

		private static void moveDumpFolder(string dumpFolder)
		{
			int retry = 0;

			while (retry++ < 5)
			{
				try
				{
					string dumpBackup = Path.Combine(Path.GetDirectoryName(dumpFolder), "Backup.old");
					if (Directory.Exists(dumpBackup))
						Directory.Delete(dumpBackup, true);

					Directory.Move(dumpFolder, dumpBackup);

					break;
				}
				catch (Exception e)
				{
					Logger.Warn("Failed to move dump folder, retry?", e);
					System.Threading.Thread.Sleep(500);
				}
			}

			if (Directory.Exists(dumpFolder))
				Directory.Delete(dumpFolder, true);
		}

		private static void RestoreStationId()
		{
			string oldStationId = (string)StationRegistry.GetValue("oldStationId", null);
			if (oldStationId != null)
				StationRegistry.SetValue("stationId", oldStationId);
			StationRegistry.DeleteValue("oldStationId");
		}

		private static void RestoreDriverAccount()
		{
			string oldDriver = (string)StationRegistry.GetValue("olddriver", null);
			if (oldDriver != null)
				StationRegistry.SetValue("driver", oldDriver);
			StationRegistry.DeleteValue("olddriver");
		}

		private static bool HasFeature(Session session, string featureId)
		{
			foreach (FeatureInfo feature in session.Features)
			{
				if (feature.Name == featureId)
					return feature.RequestState == InstallState.Local;
			}

			return false;
		}

		private static void RestoreClientAppData()
		{
			try
			{
				Logger.Info("Restoring Client AppData");
				RestoreWavefaceFolder(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
				RestoreWavefaceFolder(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));

				Logger.Info("Restore Client AppData complete");
			}
			catch (Exception e)
			{
				Logger.Warn("Unable to restore client app data", e);
			}
		}

		private static void RestoreWavefaceFolder(string parentFolder)
		{
			string backupData = Path.Combine(parentFolder, "oldWaveface");
			string restoreData = Path.Combine(parentFolder, "waveface");

			if (Directory.Exists(backupData) && !Directory.Exists(restoreData))
				Directory.Move(backupData, restoreData);
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

		private static void RemoveDirectory(string dir)
		{
			try
			{
				if (Directory.Exists(dir))
					Directory.Delete(dir, true);
			}
			catch (Exception e)
			{
				Logger.Warn("Unable to delete " + dir, e);
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
				if (PerformanceCounterCategory.Exists(PerfCounter.CATEGORY_NAME))
					PerformanceCounterCategory.Delete(PerfCounter.CATEGORY_NAME);
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
				StationRegistry.DeleteValue("driver");
			}
			catch (Exception e)
			{
				Logger.Warn("Unable to delete driver account in registry", e);
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

			try
			{
				LoginedSessionCollection.Instance.RemoveAll();
			}
			catch (Exception e)
			{
				Logger.Warn("Unable to delete LoginedSession collection from mongoDB", e);
			}

			try
			{
				QueuedTaskCollection.Instance.RemoveAll();
			}
			catch (Exception e)
			{
				Logger.Warn("Unable to delete QueuedTaskCollection from mongoDB", e);
			}

			try
			{
				PostUploadTasksCollection.Instance.RemoveAll();
			}
			catch (Exception e)
			{
				Logger.Warn("Unable to delete PostUploadTasksCollection from mongoDB", e);
			}

			try
			{
				PostDBDataCollection.Instance.RemoveAll();
			}
			catch (Exception e)
			{
				Logger.Warn("Unable to delete PostCollection from mongoDB", e);
			}


			RemoveDirectory(Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
				"waveface"));

			RemoveDirectory(Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
				"waveface"));

			return ActionResult.Success;
		}

		[CustomAction]
		public static ActionResult CleanAppData(Session session)
		{
			RemoveDirectory(Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
				"waveface"));

			RemoveDirectory(Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
				"waveface"));

			return ActionResult.Success;
		}


		[CustomAction]
		public static ActionResult StartAndWaitMongoDbReady(Session session)
		{
			string installDir = session["INSTALLLOCATION"];

			try
			{
				string svcName = StationService.MONGO_SERVICE_NAME;

				PreallocDBFiles(installDir);

				Logger.Info("Starting DB...");
				StartService(svcName);
				Logger.Info("DB is started");

				int retry = 60;
				while (!Waveface.Common.MongoDbHelper.IsMongoDBReady("127.0.0.1", 10319) && 0 < retry--)
				{
					Logger.Info("Testing MongoDB again...");
					System.Threading.Thread.Sleep(3000);
					StartService(svcName);
				}

				if (!Waveface.Common.MongoDbHelper.IsMongoDBReady("127.0.0.1", 10319))
					throw new System.TimeoutException("MongoDB is not ready after 20 retries");

				Logger.Info("MongoDB is ready");
				return ActionResult.Success;

			}
			catch (Exception e)
			{
				Logger.Warn(e);
				return ActionResult.Failure;
			}
		}

		private static void PreallocDBFiles(string installDir)
		{
			try
			{
				Logger.Info("Stopping DB...");
				StopService(StationService.MONGO_SERVICE_NAME);
				Logger.Info("DB is stopped");

				string journalDir = Path.Combine(installDir, @"MongoDB\Data\DB\journal");
				if (Directory.Exists(journalDir))
				{
					string[] preallocFiles = Directory.GetFiles(journalDir, "prealloc.*");

					if (preallocFiles != null && preallocFiles.Length > 0)
						return;
				}

				Logger.Info("Extracting prealloc DB files...");
				string preallocZip = Path.Combine(installDir, @"MongoDB\Data\DB\mongoPrealloc.tar.gz");
				GZipInputStream gzipIn = new GZipInputStream(File.OpenRead(preallocZip));
				using (TarArchive tar = TarArchive.CreateInputTarArchive(gzipIn))
				{
					tar.ExtractContents(Path.Combine(installDir, @"MongoDB\Data\DB"));
				}

				Logger.Warn("Extract prealloc DB files successfully");
			}
			catch (Exception e)
			{
				Logger.Warn("Error during extracting prealloc DB files", e);
			}
		}


		private static void StartService(string svcName)
		{
			int retry = 3;

			while (0 < retry--)
			{
				ServiceController svc = new ServiceController(svcName);
				try
				{
					
					if (svc.Status != ServiceControllerStatus.Running &&
						svc.Status != ServiceControllerStatus.StartPending)
					{
						svc.Start();
						svc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(60));
					}

					return;
				}
				catch (Exception e)
				{
					if (retry > 0)
					{
						System.Threading.Thread.Sleep(3000);
						svc.Refresh();
					}
					else
						throw new InstallerException("Unable to start " + svcName, e);
				}
			}
		}

		private static void StopService(string svcName)
		{
			ServiceController svc = new ServiceController(svcName);

			int retry = 3;

			while (0 < retry--)
			{
				try
				{
					if (svc.Status != ServiceControllerStatus.Stopped &&
						svc.Status != ServiceControllerStatus.StopPending)
					{
						svc.Stop();
						svc.WaitForStatus(ServiceControllerStatus.Stopped);
					}

					return;
				}
				catch (Exception e)
				{
					if (retry > 0)
					{
						System.Threading.Thread.Sleep(3000);
						svc.Refresh();
					}
					else
						throw new InstallerException("Unable to stop " + svcName, e);
				}
			}
		}

		[CustomAction]
		public static ActionResult WriteProductInfo(Session session)
		{
			ProductInfo oldProduct = ProductInfoCollection.Instance.FindOne();
			ProductInfo newProduct = ProductInfo.GetCurrentVersion();

			if (oldProduct != null)
				newProduct.id = oldProduct.id;

			ProductInfoCollection.Instance.Save(newProduct);


			string installDir = session["INSTALLLOCATION"];
			//using (var zip = new ZipFile(Path.Combine(installDir, "WebFiles.zip")))
			//{
			//    foreach(ZipEntry entry in zip)
			//    {
			//        if (entry.IsFile)
			//            extractFile(zip.GetInputStream(entry), entry, installDir);
			//    }
			//}

			return ActionResult.Success;
		}

		[CustomAction]
		public static ActionResult CleanRemainingFiles(Session session)
		{
			string installDir = session["INSTALLLOCATION"];
			var dir = new DirectoryInfo(installDir);
			foreach (var subdir in dir.GetDirectories())
			{
				try
				{
					// Skip mongodb backup folder
					if (subdir.Name.Equals("MongoDB", StringComparison.InvariantCultureIgnoreCase))
					{
						var mongodbDir = subdir;
						var subsInMongodbDir = mongodbDir.GetDirectories();

						foreach (var subInMongo in subsInMongodbDir)
						{
							if (subInMongo.Name.Equals("Backup", StringComparison.InvariantCultureIgnoreCase))
								continue;
							else
								subInMongo.Delete(true);
						}
					}
					else
					{
						subdir.Delete(true);
					}
				}
				catch (Exception e)
				{
					Logger.Warn("Unable to delete: " + subdir.FullName, e);
				}
			}

			return ActionResult.Success;
		}

		private static void extractFile(Stream inputStream, ZipEntry theEntry, string targetDir)
		{
			// try and sort out the correct place to save this entry
			string entryFileName;

			if (Path.IsPathRooted(theEntry.Name))
			{
				string workName = Path.GetPathRoot(theEntry.Name);
				workName = theEntry.Name.Substring(workName.Length);
				entryFileName = Path.Combine(Path.GetDirectoryName(workName), Path.GetFileName(theEntry.Name));
			}
			else
			{
				entryFileName = theEntry.Name;
			}

			string targetName = Path.Combine(targetDir, entryFileName);
			string fullPath = Path.GetDirectoryName(Path.GetFullPath(targetName));

			// Could be an option or parameter to allow failure or try creation
			if (!Directory.Exists(fullPath))
			{
				Directory.CreateDirectory(fullPath);
			}


			if (entryFileName.Length > 0)
			{				
				using (FileStream outputStream = File.Create(targetName))
				{
					StreamHelper.Copy(inputStream, outputStream);
				}

				File.SetLastWriteTime(targetName, theEntry.DateTime);
			}
		}


		private static bool RenameThumbnailExtensions(Attachment attachment, string resourceFolder)
		{
			bool modified = false;

			foreach (ImageMeta thumbnail in Enum.GetValues(typeof(ImageMeta)))
			{
				if (thumbnail == ImageMeta.Origin || thumbnail == ImageMeta.None)
					continue;

				ThumbnailInfo thumbnailInfo = attachment.image_meta.GetThumbnailInfo(thumbnail);
				if (thumbnailInfo != null && thumbnailInfo.saved_file_name != null &&
					Path.GetExtension(thumbnailInfo.saved_file_name) != ".dat")
				{
					string newFileName = Path.GetFileNameWithoutExtension(thumbnailInfo.saved_file_name) + ".dat";
					File.Move(
						Path.Combine(resourceFolder, thumbnailInfo.saved_file_name),
						Path.Combine(resourceFolder, newFileName));

					thumbnailInfo.saved_file_name = newFileName;
					modified = true;
				}
			}

			return modified;
		}

		private static string GetResourceDir(Session session, Attachment attachment)
		{
			Driver user = DriverCollection.Instance.FindDriverByGroupId(attachment.group_id);
			if (user == null)
				throw new Exception("user has been removed.");

			if (Path.IsPathRooted(user.folder))
				return user.folder;
			else
				return Path.Combine(session["INSTALLLOCATION"], user.folder);
		}
	}
}
