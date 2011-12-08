using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;

using Wammer.Model;
using Wammer.Utility;

namespace Wammer.Station
{
	public interface IFileStorage
	{
		void SaveAttachment(Attachments attachment);
		void SaveFile(string filename, byte[] data);
		long GetAvailSize();
		long GetUsedSize();
	}

	public class FileStorage
	{
		private readonly string basePath;

		public FileStorage(Drivers driver)
		{
			this.basePath = driver.folder;
			CreateFolder(basePath);
		}

		private static void CreateFolder(string basePath)
		{
			if (!Directory.Exists(basePath))
				Directory.CreateDirectory(basePath);
		}

		public void SaveAttachment(Attachments attachment)
		{
			SaveFile(attachment.saved_file_name, attachment.RawData);
		}

		public void SaveFile(string filename, byte[] data)
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
			return FileStorageHelper.GetAvailSize(Environment.CurrentDirectory);
		}

		public long GetUsedSize()
		{
			return FileStorageHelper.GetUsedSize(basePath);
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
