using System;
using System.IO;
using System.Net;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.Utility;

namespace Wammer.Station
{
	public class DropboxFileStorage : IFileStorage
	{
		//TODO: cloudstorage not use => remove argument
		private log4net.ILog logger = log4net.LogManager.GetLogger(typeof(DropboxFileStorage));
		private Driver driver;
		//private CloudStorage cloudstorage;
		private string basePath;

		public DropboxFileStorage(Driver driver, CloudStorage cloudstorage)
		{
			this.driver = driver;
			//this.cloudstorage = cloudstorage;
			this.basePath = Path.Combine(cloudstorage.Folder, driver.folder);
			if (!Directory.Exists(basePath))
			{
				logger.DebugFormat("Folder does not exist, create folder {0}", basePath);
				Directory.CreateDirectory(basePath);
			}
		}

		public void SaveAttachment(Attachment attachment)
		{
			// Currently no quota limit 
			//if (attachment.file_size > cloudstorage.Quota)
			//{
			//    logger.WarnFormat("File is too large, cannot not backup to Dropbox. file = {0}, size = {1}, quota = {2}", 
			//        attachment.saved_file_name, attachment.file_size, cloudstorage.Quota);
			//}

			//if (AllocateSpace(attachment))
			//{
				//string file = Path.Combine(driver.folder, attachment.saved_file_name);
				string SavedFilePath = GetDropboxFilePath(attachment.file_name);

				File.Copy(
					Path.Combine(driver.folder, attachment.saved_file_name),
					SavedFilePath, true);

				logger.DebugFormat("call setloc for file {0}", Path.GetFileName(SavedFilePath));
				AttachmentApi api = new AttachmentApi(driver);

				using (WebClient client = new WebClient())
				{
					api.AttachmentSetLoc(
						client,
						(int)AttachmentApi.Location.Dropbox,
						attachment.object_id,
						Path.Combine(driver.folder, Path.GetFileName(SavedFilePath))
					);
				}
			//}
		}

		public string GetDropboxFilePath(string filename)
		{
			string filePath = Path.Combine(basePath, filename);
			string ext = Path.GetExtension(filePath);
			string fileNameNoExt = Path.GetFileNameWithoutExtension(filePath);
			int index = 0;
			while (File.Exists(filePath))
			{
				index++;
				filePath = Path.Combine(basePath, fileNameNoExt + " (" + index.ToString() + ")" + ext);
			}
			
			return filePath;
		}

		public long GetAvailSize()
		{
			return FileStorageHelper.GetAvailSize(basePath);
		}

		public long GetUsedSize()
		{
			return FileStorageHelper.GetUsedSize(basePath);
		}

		//private bool AllocateSpace(Attachment attachment)
		//{
		//    // TODO: Implement this function in the future;
		//    return true;

		//    //if (cloudstorage.Quota - GetUsedSize() >= attachment.file_size)
		//    //    return true;

		//    //// run at most 100 times to avoid infinite loop
		//    //int retry = 100;
		//    //while (cloudstorage.Quota - GetUsedSize() < attachment.file_size)
		//    //{
		//    //    // delete the least accessed file until storage has enough space
		//    //    DirectoryInfo di = new DirectoryInfo(basePath);
		//    //    FileInfo fi = di.GetFiles().OrderBy(f => f.LastAccessTime).First();
		//    //    try
		//    //    {
		//    //        logger.InfoFormat("Cloud storage has no quota, delete file {0}", fi.FullName);
		//    //        fi.Delete();

		//    //        Attachment purgedAttachment = AttachmentCollection.Instance.FindOne(Query.EQ("file_name", fi.FullName));
		//    //        if (purgedAttachment != null)
		//    //        {
		//    //            AttachmentApi api = new AttachmentApi(driver.user_id);
		//    //            api.AttachmentUnsetLoc(
		//    //                new WebClient(), 
		//    //                (int)AttachmentApi.Location.Dropbox,
		//    //                purgedAttachment.object_id
		//    //            );
		//    //        }
		//    //    }
		//    //    catch
		//    //    {
		//    //        logger.WarnFormat("Unable to delete file {0}", fi.FullName);
		//    //    }

		//    //    if (--retry == 0)
		//    //        break;
		//    //}

		//    //if (cloudstorage.Quota - GetUsedSize() < attachment.file_size)
		//    //{
		//    //    logger.WarnFormat("Unable to allocate enough space for file {0}, size = {1}", 
		//    //        attachment.saved_file_name, attachment.file_size);
		//    //    return false;
		//    //}
		//    //else
		//    //{
		//    //    return true;
		//    //}
		//}
	}
}
