using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;

namespace Wammer.Station
{
	public class FileStorage
	{
		private readonly string basePath;

		public FileStorage(string basePath)
		{
			this.basePath = basePath;
			CreateFolder(basePath);
		}

		private static void CreateFolder(string basePath)
		{
			if (!Directory.Exists(basePath))
				Directory.CreateDirectory(basePath);
		}

		public void Save(string filename, byte[] data)
		{
			string filePath = Path.Combine(basePath, filename);

			using (BinaryWriter w = new BinaryWriter(File.Open(filePath, FileMode.Create)))
			{
				w.Write(data);
			}
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

			FileStream fs = new FileStream(filePath, FileMode.Create,
								FileAccess.Write, FileShare.None, 4096, true);

			return new FileStorageAsyncResult(
				fs.BeginWrite(data, 0, data.Length, callback, userObject),
				fs, userObject);
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
            string root = Path.GetPathRoot(Environment.CurrentDirectory);
            DriveInfo di = new DriveInfo(root);
            return di.AvailableFreeSpace;
        }

        public long GetUsedSize()
        {
            DirectoryInfo d = new DirectoryInfo(basePath);
            return DirSize(d);
        }
        
        private long DirSize(DirectoryInfo d)
        {
            long size = 0;
            // add file sizes
            FileInfo[] fis = d.GetFiles();
            foreach (FileInfo fi in fis)
            {
                size += fi.Length;
            }
            // add subdirectory sizes
            DirectoryInfo[] dis = d.GetDirectories();
            foreach (DirectoryInfo di in dis)
            {
                size += DirSize(di);
            }
            return size;
        }
	}


	class FileStorageAsyncResult : IAsyncResult
	{
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
