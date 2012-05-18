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
using ICSharpCode.SharpZipLib.Tar;
using ICSharpCode.SharpZipLib.GZip;
using System.Runtime.InteropServices;
using MongoDB.Driver.Builders;


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
				foreach (var user in DriverCollection.Instance.FindAll())
				{
					if (string.IsNullOrEmpty(user.session_token))
						continue;

					try
					{
						Wammer.Cloud.StationApi.SignOff(new WebClient(), StationRegistry.StationId, user.session_token);
						Logger.Info("Signoff station is successful");
						break; // use anyone's token to signoff this station once is enough
					}
					catch (Exception e)
					{
						Logger.WarnFormat("Signoff station with user {0}'s token not success: {1}", user.email, e.Message);
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

			KillProcess("WavefaceWindowsClient");
			KillProcess("StationUI");
			KillProcess("StationSystemTray");

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
					p.Kill();
					if (name == "StationSystemTray")
					{
						ClearGhostTrayIcons();
					}
				}
			}
			catch (Exception e)
			{
				Logger.Warn("Cannot kill process " + name, e);
			}
		}

		[DllImport("user32.dll")]
		private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string className, IntPtr title);

		private struct RECT
		{
			public int left;
			public int top;
			public int right;
			public int bottom;
		}

		[DllImport("user32.dll")]
		private static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

		[DllImport("user32.dll")]
		private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 msg, Int32 wparam, Int32 lparam);
		private static readonly UInt32 WM_MOUSEMOVE = 0x0200;

		private static void ClearGhostTrayIcons()
		{
			IntPtr hwndTray = FindWindowEx(IntPtr.Zero, IntPtr.Zero, "Shell_TrayWnd", IntPtr.Zero);
			IntPtr hwndNotify = FindWindowEx(hwndTray, IntPtr.Zero, "TrayNotifyWnd", IntPtr.Zero);
			IntPtr hwndPager = FindWindowEx(hwndNotify, IntPtr.Zero, "SysPager", IntPtr.Zero);
			IntPtr hwndToolbar = FindWindowEx(hwndPager, IntPtr.Zero, "ToolbarWindow32", IntPtr.Zero);

			RECT toolbarRect;
			GetClientRect(hwndToolbar, out toolbarRect);
			for (int x = 0; x <= toolbarRect.right; x += 5)
			{
				SendMessage(hwndToolbar, WM_MOUSEMOVE, 0, toolbarRect.bottom / 2 << 16 + x);
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
				Logger.Info(dumpFolder + " exists. Restoring DB....");
				RunProgram(Path.Combine(session["INSTALLLOCATION"], @"MongoDB\mongorestore.exe"),
					"--port 10319 \"" + dumpFolder + "\"");

				string dumpBackup = Path.Combine(Path.GetDirectoryName(dumpFolder), "Backup.old");
				if (Directory.Exists(dumpBackup))
					Directory.Delete(dumpBackup, true);

				Directory.Move(dumpFolder, dumpBackup);
			}
			else
				Logger.Info(dumpFolder + " does not exist. Skip restoring");
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
				Logger.Debug("Restoring Client AppData");
				RestoreWavefaceFolder(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
				RestoreWavefaceFolder(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));

				Logger.Debug("Restore Client AppData complete");
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

				Logger.Debug("Starting DB...");
				StartService(svcName);
				Logger.Debug("DB is started");

				int retry = 180;
				while (!IsMongoDBReady() && 0 < retry--)
				{
					System.Threading.Thread.Sleep(TimeSpan.FromSeconds(2.0));
					Logger.Debug("Testing MongoDB again...");
					StartService(svcName);
				}

				if (!IsMongoDBReady())
					throw new System.TimeoutException("MongoDB is not ready in 360 seconds");

				Logger.Debug("MongoDB is ready");
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
				Logger.Debug("Stopping DB...");
				StopService(StationService.MONGO_SERVICE_NAME);
				Logger.Debug("DB is stopped");

				string journalDir = Path.Combine(installDir, @"MongoDB\Data\DB\journal");
				if (Directory.Exists(journalDir))
				{
					string[] preallocFiles = Directory.GetFiles(journalDir, "prealloc.*");

					if (preallocFiles != null && preallocFiles.Length > 0)
						return;
				}

				Logger.Debug("Extracting prealloc DB files...");
				string preallocZip = Path.Combine(installDir, @"MongoDB\Data\DB\mongoPrealloc.tar.gz");
				GZipInputStream gzipIn = new GZipInputStream(File.OpenRead(preallocZip));
				using (TarArchive tar = TarArchive.CreateInputTarArchive(gzipIn))
				{
					tar.ExtractContents(Path.Combine(installDir, @"MongoDB\Data\DB"));
				}

				Logger.Info("Extract prealloc DB files successfully");
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
				MongoServer mongo = MongoServer.Create("mongodb://127.0.0.1:10319/?connectTimeoutMS=10000");
				mongo.GetDatabaseNames();
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

		private static void StopService(string svcName)
		{
			ServiceController mongoSvc = new ServiceController(svcName);

			int retry = 3;

			while (0 < retry--)
			{
				try
				{
					if (mongoSvc.Status != ServiceControllerStatus.Stopped &&
						mongoSvc.Status != ServiceControllerStatus.StopPending)
					{
						mongoSvc.Stop();
						mongoSvc.WaitForStatus(ServiceControllerStatus.Stopped);
					}

					return;
				}
				catch (Exception e)
				{
					if (retry > 0)
						System.Threading.Thread.Sleep(3000);
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
