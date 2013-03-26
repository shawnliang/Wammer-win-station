using System;
using System.Collections.Generic;
using System.IO;
using Waveface.Stream.ClientFramework;
using Waveface.Stream.Core;
using Waveface.Stream.Model;
using Waveface.Stream.WindowsClient.Properties;

namespace Waveface.Stream.WindowsClient
{
	class PortableMediaService : IPortableMediaService
	{
		public PortableMediaService()
		{
		}

		public IEnumerable<PortableDevice> GetPortableDevices()
		{
			foreach (var drive in DriveInfo.GetDrives())
			{
				if ((drive.DriveType == DriveType.CDRom || drive.DriveType == DriveType.Removable) &&
					drive.IsReady)
				{
					yield return new PortableDevice
					{
						DrivePath = drive.Name,
						Name = string.Format("({0}) {1}", drive.Name, drive.VolumeLabel)
					};
				}
			}

		}


		public string ImportAsync(string drive_path)
		{
			var user = StreamClient.Instance.LoginedUser;
			return StationAPI.ImportPhoto(user.SessionToken, user.GroupID, new string[] { drive_path }, true, true).task_id;
		}

		public ImportTaskStaus QueryTaskStatus(string taskId)
		{
			return TaskStatusCollection.Instance.FindOneById(new Guid(taskId));
		}

		public bool GetAlwaysAutoImport(string driveName)
		{
			string devId = getDevIdFromHiddenFile(driveName);

			return
				!string.IsNullOrEmpty(devId) &&
				Settings.Default.AutoImportDevices != null &&
				Settings.Default.AutoImportDevices.Contains(devId);
		}

		public void SetAlwaysAutoImport(string driveName, bool autoImport)
		{
			var devId = getDevIdFromHiddenFile(driveName);

			if (string.IsNullOrEmpty(devId))
			{
				devId = generateDevIdToDevice(driveName);
			}

			if (Settings.Default.AutoImportDevices == null)
				Settings.Default.AutoImportDevices = new System.Collections.Specialized.StringCollection();

			if (!Settings.Default.AutoImportDevices.Contains(devId))
			{
				Settings.Default.AutoImportDevices.Add(devId);
				Settings.Default.Save();
			}
		}

		private static string getDevIdFromHiddenFile(string driveName)
		{
			try
			{
				DriveInfo d = new DriveInfo(driveName);
				var file = Path.Combine(d.RootDirectory.FullName, ".streamDrive");


				using (var readder = File.OpenText(file))
				{
					return readder.ReadToEnd();
				}
			}
			catch
			{
				return string.Empty;
			}
		}

		private string generateDevIdToDevice(string driveName)
		{
			var id = Guid.NewGuid().ToString();
			var file = Path.Combine(driveName, ".streamDrive");
			using (StreamWriter w = new StreamWriter(File.OpenWrite(file)))
			{
				w.Write(id);
			}

			return id;
		}
	}
}
