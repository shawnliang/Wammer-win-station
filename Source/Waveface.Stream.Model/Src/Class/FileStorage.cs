using System;
using System.IO;
using Waveface.Stream.Model;
using System.Diagnostics;
using System.Linq;

namespace Waveface.Stream.Model
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
		public string basePath;
		private static string defaultResFolder;

		static FileStorage()
		{
			defaultResFolder = Path.Combine(GetStationPath(), "resource");

			//if (!Directory.Exists("cache"))
			//    Directory.CreateDirectory("cache");
		}

		public FileStorage(Driver driver)
		{
			if (driver == null)
				throw new ArgumentNullException("driver");

			if (driver.folder == null)
				throw new ArgumentNullException("driver.folder");


			basePath = Path.Combine(ResourceFolder, driver.folder);

			if (!Directory.Exists(basePath))
				CreateFolder(basePath);
		}

		private static string GetStationPath()
		{
            //var stationProcess = System.Diagnostics.Process.GetProcessesByName("Station.Service").FirstOrDefault();
			var stationPath = string.Empty;
            //if (stationProcess != null)
            //{
            //    stationPath = Path.GetDirectoryName(stationProcess.MainModule.FileName);
            //}
            //else
            //{
			stationPath = Path.Combine((string)StationRegistry.GetValue("InstallPath", ""), "WavefaceStation");
            //}

			return stationPath;
		}

		private static void CreateFolder(string basePath)
		{
			if (!string.IsNullOrEmpty(basePath) && !Directory.Exists(basePath))
				Directory.CreateDirectory(basePath);
		}

		/// <summary>
		/// Save data to cache folder
		/// </summary>
		/// <param name="filename">save file name</param>
		/// <param name="data">raw data</param>
		/// <returns>relative path to station's current folder</returns>
		public static string SaveToCacheFolder(string user_id, string filename, ArraySegment<byte> data)
		{
			var userCache = Path.Combine("cache", user_id);

			if (!Directory.Exists(userCache))
				Directory.CreateDirectory(userCache);


			string filePath = Path.Combine(userCache, filename);
			string tempFile = Path.Combine(userCache, Guid.NewGuid().ToString());

			using (FileStream stream = File.Open(tempFile, FileMode.Create))
			{
				stream.Write(data.Array, data.Offset, data.Count);
			}

			if (File.Exists(filePath))
				File.Delete(filePath);

			File.Move(tempFile, filePath);

			return filePath;
		}

		public static FileStream LoadFromCacheFolder(string filename)
		{
			return File.OpenRead(filename);
		}

		/// <summary>
		/// Tries to save file as the given filename. If there is already a file exist, 
		/// append the file name with (1)/(2)/...
		/// </summary>
		/// <param name="filename"></param>
		/// <param name="data"></param>
		/// <returns>actuall filename</returns>
		public string TrySaveFile(string filename, ArraySegment<byte> data)
		{
			createDirsInFileName(filename);

			string filePath = Path.Combine(basePath, filename);
			string tempFile = Path.Combine(basePath, Guid.NewGuid().ToString());

			using (FileStream stream = File.Open(tempFile, FileMode.Create))
			{
				stream.Write(data.Array, data.Offset, data.Count);
			}

			int num = 1;
			var dir = Path.GetDirectoryName(filePath);
			var filenameNoExt = Path.GetFileNameWithoutExtension(filePath);
			var ext = Path.GetExtension(filePath);

			bool success = false;

			while (!success)
			{
				try
				{
					File.Move(tempFile, filePath);
					success = true;
				}
				catch (IOException)
				{
					filePath = Path.Combine(dir, filenameNoExt) + " (" + num + ")" + ext;
					++num;
				}
			}

			return filePath.Substring(basePath.Length + 1); // + 1 for "\"
		}

		private void createDirsInFileName(string filename)
		{
			var dirName = Path.GetDirectoryName(filename);

			if (!string.IsNullOrEmpty(dirName))
			{
				var dirs = dirName.Split(new char[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);

				string dir = this.basePath;
				var index = 0;

				do
				{
					dir = Path.Combine(dir, dirs[index]);

					if (!Directory.Exists(dir))
						Directory.CreateDirectory(dir);

					++index;

				} while (index < dirs.Length);
			}
		}

		public static string GetTempFile(Driver user)
		{
			if (user == null || user.folder == null)
				throw new ArgumentNullException("user", "user or user.folder is null");

			return Path.Combine(ResourceFolder, Path.Combine(user.folder, "temp_" + Guid.NewGuid().ToString()));
		}

		public FileStream Load(string filename)
		{
			string filePath = Path.Combine(basePath, filename);
			return File.OpenRead(filePath);
		}

		public FileStream Load(string filename, ImageMeta meta)
		{
			if (meta == ImageMeta.Origin || meta == ImageMeta.None)
				return Load(filename);
			else
				return FileStorage.LoadFromCacheFolder(filename);
		}

		public string GetFullFilePath(string filename, ImageMeta meta)
		{
			if (meta == ImageMeta.Origin || meta == ImageMeta.None)
				return Path.Combine(basePath, filename);
			else
				return Path.Combine(GetStationPath(), filename);
		}


		public IAsyncResult BeginSave(string filename, byte[] data, AsyncCallback callback,
									  object userObject)
		{
			string filePath = Path.Combine(basePath, filename);
			string tempFile = filePath + @".tmp";

			var fs = new FileStream(tempFile, FileMode.Create,
									FileAccess.Write, FileShare.None, 4096, true);

			return new FileStorageAsyncResult(
				fs.BeginWrite(data, 0, data.Length, callback, userObject),
				fs) { TempFile = tempFile, TargetFile = filePath };
		}

		public void EndSave(IAsyncResult async)
		{
			var fsAsync = (FileStorageAsyncResult)async;
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

		public static string ResourceFolder
		{
			get
			{
				return StationRegistry.GetValue("ResourceFolder", defaultResFolder) as string;
			}

			set
			{
				StationRegistry.SetValue("ResourceFolder", value);
			}
		}
	}
}
