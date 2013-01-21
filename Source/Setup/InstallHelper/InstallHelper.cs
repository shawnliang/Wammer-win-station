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

			CloseStream();
			KillProcess("WavefaceWindowsClient");
			KillProcess("StationUI");
			KillProcess("StationSystemTray");
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
				if (HasFeature(session, "MainFeature"))
				{
					RestoreStationDB(session, dumpFolder);
					RestoreStationId();
				}

				if (HasFeature(session, "ClientFeature"))
				{
					RestoreClientAppData();
				}

				MigrateUserFolder(session);
				return ActionResult.Success;
			}
			catch (Exception e)
			{
				Logger.Error("Failed to restore saved data", e);
				return ActionResult.Failure;
			}
			finally
			{
				RemoveBackupData(dumpFolder);
			}
		}

		private static void MigrateUserFolder(Session session)
		{
			if (StationRegistry.GetValue("ResourceFolder", null) == null)
			{
				StationRegistry.SetValue("ResourceFolder", Path.Combine(session["INSTALLLOCATION"], "resource"));

				var allIds = DriverCollection.Instance.FindAll().Select(x => new { id = x.user_id, email = x.email });

				foreach (var id in allIds)
				{
					DriverCollection.Instance.Update(Query.EQ("_id", id.id),
						Update.Set("folder", "user_" + id.id));

					Logger.InfoFormat("Update {0} : {1}", id.email, "user_" + id.id);
				}
			}
		}

		private static void RemoveBackupData(string dumpFolder)
		{
			try
			{
				RemoveDirectory(dumpFolder);

				if (StationRegistry.GetValue("oldStationId", null) != null)
					StationRegistry.DeleteValue("oldStattionId");
				if (StationRegistry.GetValue("olddriver", null) != null)
					StationRegistry.DeleteValue("olddriver");
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
					startInfo.Arguments = string.Format("failure WavefaceStation reset= 86400 actions= restart/60000");

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

		[CustomAction]
		public static ActionResult MigrateOriginalPhotoLocation(Session session)
		{
			try
			{
				ProductInfo product = ProductInfoCollection.Instance.FindOne();
				if (product != null && product.PlaceAttachmentsByDate)
					return ActionResult.Success;


				// copy all caches from {res}\user_{guid}\*.dat to {installlocation}\cache\{guid}

				var oldResBase = FileStorage.ResourceFolder;
				var newCacheBase = Path.Combine(session["INSTALLLOCATION"], "cache");

				if (!Directory.Exists(newCacheBase))
					Directory.CreateDirectory(newCacheBase);

				foreach (var user in DriverCollection.Instance.FindAll())
				{
					var oldUserResFolder = Path.Combine(oldResBase, "user_" + user.user_id);
					var newUserCacheFolder = Path.Combine(newCacheBase, user.user_id);

					if (!Directory.Exists(newUserCacheFolder))
						Directory.CreateDirectory(newUserCacheFolder);

					var datFiles = Directory.GetFiles(oldUserResFolder, "*.dat");
					
					Array.ForEach(datFiles, (datFile) => {
						var name = Path.GetFileName(datFile);
						
						File.Copy(datFile, Path.Combine(newUserCacheFolder, name), true);
					});
				}



				// copy all resources to resource folder by date
				
				/// key => object id,   value => new save file name
				var newPaths = new Dictionary<string, string>();
				foreach (var user in DriverCollection.Instance.FindAll())
				{
					var userResFolder = Path.Combine(oldResBase, "user_" + user.user_id);
					var allFiles = Directory.GetFiles(userResFolder, "*.*");
					
					foreach (var file in allFiles)
					{
						if (file.EndsWith(".dat"))
							continue;

						var object_id = Path.GetFileNameWithoutExtension(file);
						
						var dbDoc = AttachmentCollection.Instance.FindOneById(object_id);
						if (dbDoc == null)
							continue;

						var rawData = File.ReadAllBytes(file);
						var fileInfo = new FileInfo(file);
						var properties = new AttachmentInfo
						{
							object_id = object_id,
							group_id = dbDoc.group_id,
							file_name = dbDoc.file_name,
							file_create_time = fileInfo.CreationTime.ToUTCISO8601ShortString()
						};

						var saveResult = Wammer.Station.Timeline.ResourceDownloadTask.SaveAttachmentToDisk(ImageMeta.Origin, properties, rawData);
						newPaths.Add(object_id, saveResult.RelativePath);
					}
				}


				// update db saved_file_path
				foreach (var user in DriverCollection.Instance.FindAll())
				{
					var group_id = user.groups[0].group_id;

					foreach (var dbDoc in AttachmentCollection.Instance.Find(Query.EQ("group_id", group_id)))
					{
						if (newPaths.ContainsKey(dbDoc.object_id))
							dbDoc.saved_file_name = newPaths[dbDoc.object_id];

						if (dbDoc.image_meta != null)
						{
							if (dbDoc.image_meta.small != null && !string.IsNullOrEmpty(dbDoc.image_meta.small.saved_file_name))
								dbDoc.image_meta.small.saved_file_name = string.Format(@"cache\{0}\{1}_small.dat", user.user_id, dbDoc.object_id);

							if (dbDoc.image_meta.medium != null && !string.IsNullOrEmpty(dbDoc.image_meta.medium.saved_file_name))
								dbDoc.image_meta.medium.saved_file_name = string.Format(@"cache\{0}\{1}_medium.dat", user.user_id, dbDoc.object_id);

							if (dbDoc.image_meta.large != null && !string.IsNullOrEmpty(dbDoc.image_meta.large.saved_file_name))
								dbDoc.image_meta.large.saved_file_name = string.Format(@"cache\{0}\{1}_large.dat", user.user_id, dbDoc.object_id);

							if (dbDoc.image_meta.square != null && !string.IsNullOrEmpty(dbDoc.image_meta.square.saved_file_name))
								dbDoc.image_meta.square.saved_file_name = string.Format(@"cache\{0}\{1}_square.dat", user.user_id, dbDoc.object_id);
						}

						AttachmentCollection.Instance.Save(dbDoc);
					}
				}


				// delete old resources
				foreach (var user in DriverCollection.Instance.FindAll())
				{
					var userResFolder = Path.Combine(oldResBase, "user_" + user.user_id);
					var allFiles = Directory.GetFiles(userResFolder, "*.*");

					Array.ForEach(allFiles, (filepath) => {
						try
						{
							File.Delete(filepath);
						}
						catch (Exception e)
						{
							Logger.Warn("Unable to delete file: " + filepath, e);
						}
					});
				}

			}
			catch (Exception e)
			{
				Logger.Warn("MigrateOriginalPhotoLocation failed", e);
				throw;
			}

			return ActionResult.Success;
		}
		
		[CustomAction]
		// Since v1.0.3, thumbnail extension always uses ".dat" regardless its image format.
		// This change is because windows client is using thumbnails saved in station resource folder
		// and windows client is not able to know the exact extension of a thumbnail file based on
		// current timeline meta data.
		//
		// Saved thumbnails in previous waveface station need to be migrated to ".dat".
		public static ActionResult MigrateThumbnailsExtension(Session session)
		{
			try
			{
				ProductInfo product = ProductInfoCollection.Instance.FindOne();
				if (product != null && product.ThumbnailExtensionIsDat)
					return ActionResult.Success;


				MongoCursor<Attachment> attachments = AttachmentCollection.Instance.FindAll();
				foreach (Attachment attachment in attachments)
				{
					if (attachment.image_meta == null)
						continue;

					try
					{
						string resourceFolder = GetResourceDir(session, attachment);
						bool modified = RenameThumbnailExtensions(attachment, resourceFolder);

						if (modified)
							AttachmentCollection.Instance.Save(attachment);
					}
					catch (Exception e)
					{
						Logger.Warn("Unable to update migrated file name of attachment: " + attachment.object_id, e);
						// delete this db record to force re-downloading this attachments
						AttachmentCollection.Instance.Remove(Query.EQ("_id", attachment.object_id));
					}
				}

				return ActionResult.Success;
			}
			catch (Exception e)
			{
				Logger.Error("MigrateThumbnailsExtension failed", e);
				return ActionResult.Failure;
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
