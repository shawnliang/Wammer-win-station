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
		public string BasePath { get; private set; }

		public FileStorage(Driver driver)
		{
			if (driver == null)
				throw new ArgumentNullException("driver");

			BasePath = driver.folder;

			if (!Directory.Exists(BasePath))
				CreateFolder(BasePath);
		}

		private static void CreateFolder(string basePath)
		{
			if (!string.IsNullOrEmpty(basePath) && !Directory.Exists(basePath))
				Directory.CreateDirectory(basePath);
		}

		public void SaveAttachment(Attachment attachment)
		{
			SaveFile(attachment.saved_file_name, attachment.RawData);
		}

		public void SaveFile(string filename, ArraySegment<byte> data)
		{
			string filePath = Path.Combine(BasePath, filename);
			string tempFile = filePath + @".tmp";

			using (FileStream stream = File.Open(tempFile, FileMode.Create))
			{
				stream.Write(data.Array, data.Offset, data.Count);
			}

			if (File.Exists(filePath))
				File.Delete(filePath);

			File.Move(tempFile, filePath);
		}

		public static string GetTempFile(Driver user)
		{
			if (user == null || user.folder == null)
				throw new ArgumentNullException("user", "user or user.folder is null");

			return Path.Combine(user.folder, "temp_" + Guid.NewGuid().ToString());
		}

		public FileStream Load(string filename)
		{
			string filePath = Path.Combine(BasePath, filename);
			return File.OpenRead(filePath);
		}

		public IAsyncResult BeginSave(string filename, byte[] data, AsyncCallback callback,
		                              object userObject)
		{
			string filePath = Path.Combine(BasePath, filename);
			string tempFile = filePath + @".tmp";

			var fs = new FileStream(tempFile, FileMode.Create,
			                        FileAccess.Write, FileShare.None, 4096, true);

			return new FileStorageAsyncResult(
				fs.BeginWrite(data, 0, data.Length, callback, userObject),
				fs) {TempFile = tempFile, TargetFile = filePath};
		}

		public void EndSave(IAsyncResult async)
		{
			var fsAsync = (FileStorageAsyncResult) async;
			try
			{
				fsAsync.OutputStream.EndWrite(fsAsync.InnerObject);
			}
			finally
			{
				fsAsync.OutputStream.Close();
			}

			if (File.Exists(fsAsync.TempFile))
			{
				if (File.Exists(fsAsync.TargetFile))
					File.Delete(fsAsync.TargetFile);

				File.Move(fsAsync.TempFile, fsAsync.TargetFile);
			}
		}

		public FileStream LoadByNameWithNoSuffix(string objectId)
		{
			string[] files = Directory.GetFiles(BasePath, objectId + ".*");
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
			return FileStorageHelper.GetUsedSize(BasePath);
		}
	}


	internal class FileStorageAsyncResult : IAsyncResult
	{
		private readonly IAsyncResult fileStreamAsyncResult;
		private readonly FileStream fs;

		public FileStorageAsyncResult(IAsyncResult innerObject, FileStream fs)
		{
			fileStreamAsyncResult = innerObject;
			this.fs = fs;
		}

		public string TempFile { get; set; }
		public string TargetFile { get; set; }

		public IAsyncResult InnerObject
		{
			get { return fileStreamAsyncResult; }
		}

		public FileStream OutputStream
		{
			get { return fs; }
		}

		#region IAsyncResult Members

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

		#endregion
	}
}