using System;
using System.IO;
using System.Threading;

using Wammer.Model;
using Wammer.Utility;

namespace Wammer.Station
{
	public interface IFileStorage
	{
		void SaveAttachment(Attachment attachment);
		//string SaveFile(string filename, ArraySegment<byte> data);
		long GetAvailSize();
		long GetUsedSize();
	}

	public class FileStorage
	{
		private readonly string basePath;

		public FileStorage(Driver driver)
		{
			this.basePath = driver.folder;
			CreateFolder(basePath);
		}

		private static void CreateFolder(string basePath)
		{
			
			if (basePath != "" && !Directory.Exists(basePath))
				Directory.CreateDirectory(basePath);
		}

		public void SaveAttachment(Attachment attachment)
		{
			SaveFile(attachment.saved_file_name, attachment.RawData);
		}

		public void SaveFile(string filename, ArraySegment<byte> data)
        {
            string filePath = Path.Combine(basePath, filename);
            string tempFile = System.IO.Path.GetTempFileName();

            using (FileStream stream = File.Open(tempFile, FileMode.Create))
            {
                stream.Write(data.Array, data.Offset, data.Count);
            }

            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);
            System.IO.File.Move(tempFile, filePath);
		}

		public FileStream Load(string filename)
		{
			string filePath = Path.Combine(basePath, filename);
			return File.OpenRead(filePath);
		}

		public IAsyncResult BeginSave(string filename, byte[] data, AsyncCallback callback,
																				object userObject)
		{
			string filePath = Path.Combine(basePath, filename);
            string tempFile = System.IO.Path.GetTempFileName();

            FileStream fs = new FileStream(tempFile, FileMode.Create,
								FileAccess.Write, FileShare.None, 4096, true);

            return new FileStorageAsyncResult(
                fs.BeginWrite(data, 0, data.Length, callback, userObject),
                fs, userObject) { TempFile = tempFile, TargetFile = filePath };
		}

		public void EndSave(IAsyncResult async)
		{
			FileStorageAsyncResult fsAsync = (FileStorageAsyncResult)async;
			try
			{
				fsAsync.OutputStream.EndWrite(fsAsync.InnerObject);
			}
            finally
            {
                fsAsync.OutputStream.Close();
            }

            if (System.IO.File.Exists(fsAsync.TempFile))
            {
                if (System.IO.File.Exists(fsAsync.TargetFile))
                    System.IO.File.Delete(fsAsync.TargetFile);
                System.IO.File.Move(fsAsync.TempFile, fsAsync.TargetFile);
            }
        }

		public FileStream LoadByNameWithNoSuffix(string objectId)
		{
			string[] files = Directory.GetFiles(basePath, objectId + ".*");
			if (files == null || files.Length == 0)
				throw new FileNotFoundException("attachment " + objectId + " is not found");

			return File.OpenRead(files[0]);
		}

		public long GetAvailSize()
		{
			return FileStorageHelper.GetAvailSize(Environment.CurrentDirectory);
		}

		public long GetUsedSize()
		{
			return FileStorageHelper.GetUsedSize(basePath);
		}
	}


	class FileStorageAsyncResult : IAsyncResult
    {
        public string TempFile { get; set; }
        public string TargetFile { get; set; }

		private IAsyncResult fileStreamAsyncResult;
		private FileStream fs;
		private object userObject;

		public FileStorageAsyncResult(IAsyncResult innerObject, FileStream fs,
			object userObject)
		{
			this.fileStreamAsyncResult = innerObject;
			this.fs = fs;
			this.userObject = userObject;
		}

		public object AsyncState
		{
			get { return fileStreamAsyncResult.AsyncState; }
		}

		public WaitHandle AsyncWaitHandle
		{
			get { return fileStreamAsyncResult.AsyncWaitHandle; }
		}

		public bool CompletedSynchronously
		{
			get { return fileStreamAsyncResult.CompletedSynchronously; }
		}

		public bool IsCompleted
		{
			get { return fileStreamAsyncResult.IsCompleted; }
		}

		public IAsyncResult InnerObject
		{
			get { return fileStreamAsyncResult; }
		}

		public FileStream OutputStream
		{
			get { return this.fs; }
		}
	}
}
