using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;

namespace Wammer.Station
{
	public enum FileType
	{
		None = 0,

		ImgOriginal = 100,
		ImgThumbnail1 = 101,
		ImgThumbnail2 = 102,
		ImgThumbnail3 = 103,
		ImgThumbnail4 = 104,
	}

	public class FileStorage
	{
		private readonly string basePath;

		public FileStorage(string basePath)
		{
			this.basePath = basePath;
			CreateFolder(basePath);

			foreach (int fileType in Enum.GetValues(typeof(FileType)))
			{
				string typeDir = Path.Combine(basePath, fileType.ToString());
				CreateFolder(typeDir);
			}
		}

		private static void CreateFolder(string basePath)
		{
			if (!Directory.Exists(basePath))
				Directory.CreateDirectory(basePath);
		}

		public void Save(string space, FileType type, string filename, byte[] data)
		{
			string spaceDir = Path.Combine(basePath, space);
			string typeDir = Path.Combine(spaceDir, type.ToString("d"));
			string filePath = Path.Combine(typeDir, filename);

			FileStream fs = File.Open(filePath, FileMode.Create, FileAccess.Write);
			using (BinaryWriter w = new BinaryWriter(fs))
			{
				w.Write(data);
			}
		}

		public IAsyncResult BeginSave(string space, FileType type,
			string filename, byte[] data, AsyncCallback callback, object userObject)
		{
			string spaceDir = Path.Combine(basePath, space);
			string typeDir = Path.Combine(spaceDir, type.ToString("d"));
			string filePath = Path.Combine(typeDir, filename);

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

		public static string GetSavedFile(string baseDir, string objectId, FileType type)
		{
			string spaceDir = Path.Combine(baseDir, "space1"); // TODO hardcode
			string typeDir = Path.Combine(spaceDir, type.ToString("d"));
			string[] files = Directory.GetFiles(typeDir, objectId + ".*");
			if (files == null || files.Length==0)
				throw new FileNotFoundException("object " + objectId + " is not found");

			return files[0];
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
