using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

using Wammer.Model;
using Wammer.Utility;
using Wammer.Cloud;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace Wammer.Station
{
	public class DropboxFileStorage : IFileStorage
	{
		private log4net.ILog logger = log4net.LogManager.GetLogger(typeof(DropboxFileStorage));
		private Drivers driver;
		private CloudStorage cloudstorage;
		private string basePath;

		public DropboxFileStorage(Drivers driver, CloudStorage cloudstorage)
		{
			this.driver = driver;
			this.cloudstorage = cloudstorage;
			this.basePath = Path.Combine(cloudstorage.Folder, driver.folder);
			if (!Directory.Exists(basePath))
			{
				logger.DebugFormat("Folder does not exist, create folder {0}", basePath);
				Directory.CreateDirectory(basePath);
			}
		}

		public void SaveAttachment(Attachments attachment)
		{
			if (attachment.file_size > cloudstorage.Quota)
			{
				logger.WarnFormat("File is too large, cannot not backup to Dropbox. file = {0}, size = {1}, quota = {2}", 
					attachment.saved_file_name, attachment.file_size, cloudstorage.Quota);
			}

			if (AllocateSpace(attachment))
			{
				SaveFile(attachment.file_name, attachment.RawData);

				AttachmentApi api = new AttachmentApi(driver.user_id);
				api.AttachmentSetLoc(
					new WebClient(),
					(int)AttachmentApi.Location.Dropbox,
					attachment.object_id,
					Path.Combine(driver.folder, attachment.saved_file_name)
				);
			}
		}

		public void SaveFile(string filename, byte[] data)
		{
			string filePath = Path.Combine(basePath, filename);

			using (BinaryWriter w = new BinaryWriter(File.Open(filePath, FileMode.Create)))
			{
				w.Write(data);
			}
		}

		public long GetAvailSize()
		{
			return FileStorageHelper.GetAvailSize(basePath);
		}

		public long GetUsedSize()
		{
			return FileStorageHelper.GetUsedSize(basePath);
		}

		private bool AllocateSpace(Attachments attachment)
		{
			if (cloudstorage.Quota - GetUsedSize() >= attachment.file_size)
				return true;

			// run at most 100 times to avoid infinite loop
			int retry = 100;
			while (cloudstorage.Quota - GetUsedSize() < attachment.file_size)
			{
				// delete the least accessed file until storage has enough space
				DirectoryInfo di = new DirectoryInfo(basePath);
				FileInfo fi = di.GetFiles().OrderBy(f => f.LastAccessTime).First();
				try
				{
					logger.InfoFormat("Cloud storage has no quota, delete file {0}", fi.FullName);
					fi.Delete();

					Attachments purgedAttachment = Attachments.collection.FindOne(Query.EQ("file_name", fi.FullName));
					if (purgedAttachment != null)
					{
						AttachmentApi api = new AttachmentApi(driver.user_id);
						api.AttachmentUnsetLoc(
							new WebClient(), 
							(int)AttachmentApi.Location.Dropbox,
							purgedAttachment.object_id
						);
					}
				}
				catch
				{
					logger.WarnFormat("Unable to delete file {0}", fi.FullName);
				}

				if (--retry == 0)
					break;
			}

			if (cloudstorage.Quota - GetUsedSize() < attachment.file_size)
			{
				logger.WarnFormat("Unable to allocate enough space for file {0}, size = {1}", 
					attachment.saved_file_name, attachment.file_size);
				return false;
			}
			else
			{
				return true;
			}
		}
	}
}
