using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

using Wammer.Model;
using Wammer.Cloud;
using MongoDB.Driver.Builders;

namespace Wammer.Station
{
	class CloudStorageSync
	{
		private log4net.ILog logger = log4net.LogManager.GetLogger(typeof(StationManagementService));
		private object cs = new object();
		private string resourceFolder = "resource";

		public void HandleAttachmentSaved(object sender, AttachmentEventArgs evt)
		{
			if (evt.Attachment.saved_file_name == null) // this attachment is a thumbnail
				return;

			// just for avoiding race condition, might cause bad performance
			lock (cs)
			{
				string fileName = evt.Attachment.saved_file_name;
				string objectId = evt.Attachment.object_id;
				long fileSize = evt.Attachment.file_size;
				string origFolder = evt.FolderPath;
				string driverId = evt.DriverId;

				CloudStorage storage = CloudStorage.collection.FindOne();
				if (storage != null)
				{
					if (fileSize > storage.Quota)
					{
						logger.WarnFormat("File size is too large, do not backup. file = {0}, size = {1}, quota = {2}", fileName, fileSize, storage.Quota);
					}
					else
					{
						try
						{
							BackupAttachment(fileName, fileSize, origFolder, storage, driverId, objectId);
						}
						finally
						{
							// storage.Used might be changed after BackupAttachment
							CloudStorage.collection.Save(storage);
						}
					}
				}
			}
		}

		private void BackupAttachment(string fileName, long fileSize, string origFolder, CloudStorage storage, string driverId, string objectId)
		{
			if (!Directory.Exists(storage.Folder))
			{
				logger.DebugFormat("Folder does not exist, create folder {0}", storage.Folder);
				Directory.CreateDirectory(storage.Folder);
			}

			string origFilePath = Path.Combine(origFolder, fileName);
			string backupFileRelativePath = Path.Combine(resourceFolder, fileName);
			string backupFilePath = Path.Combine(storage.Folder, backupFileRelativePath);

			Attachment attachment = new Attachment(driverId);
			if (storage.Quota - storage.Used >= fileSize)
			{
				if (CopyAtttachment(origFilePath, backupFilePath))
				{
					attachment.AttachmentSetLoc(new WebClient(), new Dictionary<object, object>
						{
							{ "loc", (int)Attachment.Location.Dropbox },
							{ "object_id", objectId },
							{ "file_path", backupFileRelativePath }
						}
					);
					storage.Used += fileSize;
				}
			}
			else
			{
				// run at most 50 times to avoid infinite loop
				int retry = 50;
				while (retry > 0)
				{
					// delete the least accessed file until storage has enough space
					DirectoryInfo di = new DirectoryInfo(storage.Folder);
					FileInfo fi = di.GetFiles().OrderBy(f => f.LastAccessTime).First();
					try
					{
						logger.InfoFormat("Cloud storage has no quota, delete file {0}", fi.FullName);
						fi.Delete();
						storage.Used -= fi.Length;
					}
					catch
					{
						logger.WarnFormat("Unable to delete file {0}", fi.FullName);
					}

					if (storage.Quota - storage.Used >= fileSize)
						break;

					retry--;
				}

				if (storage.Quota - storage.Used >= fileSize)
				{
					if (CopyAtttachment(origFilePath, backupFilePath))
					{
						attachment.AttachmentSetLoc(new WebClient(), new Dictionary<object, object>
							{
								{ "loc", (int)Attachment.Location.Dropbox },
								{ "file_path", backupFileRelativePath }
							}
						);
						storage.Used += fileSize;
					}
				}
				else
				{
					logger.WarnFormat("Unable to allocate enough space for file {0}, size = {1}", origFilePath, fileSize);
				}
			}
		}

		private bool CopyAtttachment(string origFilePath, string backupFilePath)
		{
			try
			{
				logger.DebugFormat("Copy {0} to {1}", origFilePath, backupFilePath);
				File.Copy(origFilePath, backupFilePath, true);
				return true;
			}
			catch (Exception ex)
			{
				logger.Error("Unable to copy attachments to cloud storage folder", ex);
				return false;
			}
		}

		private void NotifyCloud(string relativePath)
		{
		}
	}
}
