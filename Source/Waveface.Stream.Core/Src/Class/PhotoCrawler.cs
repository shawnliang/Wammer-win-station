using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Waveface.Stream.Model;

namespace Waveface.Stream.Core
{
	public class PhotoCrawler
	{

		public delegate void PhotoFolderFoundDelegate(string path, int count);

		private static List<string> ignorePath;
		private string StreamResourceDir = StationRegistry.GetValue("ResourceFolder", "").ToString();
			
		static PhotoCrawler()
		{
			string[] unInterestedFolders = new string[]
			{
				Environment.GetEnvironmentVariable("windir"),
				Environment.GetEnvironmentVariable("ProgramData"),
				Environment.GetEnvironmentVariable("ProgramW6432"),
				Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
				Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
				Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
				@"c:\$Recycle.bin"
			};

			ignorePath = new List<string>();
			ignorePath.AddRange(unInterestedFolders.Where(x => !string.IsNullOrEmpty(x)));
		}

		public void FindPhotoDirs(string path, PhotoFolderFoundDelegate photoFolderFound)
		{
			string curFolder = "";
			int photoCountInCurFolder = 0;

			findPhotos(new List<string> { path },

				(folder) => {

					if (!string.IsNullOrEmpty(curFolder) && photoCountInCurFolder > 0)
						photoFolderFound(curFolder, photoCountInCurFolder);

					// reset
					curFolder = folder;
					photoCountInCurFolder = 0;
				},

				(file) => {
					++photoCountInCurFolder;
				}
			);


			if (!string.IsNullOrEmpty(curFolder) && photoCountInCurFolder > 0)
				photoFolderFound(curFolder, photoCountInCurFolder);
		}


		public IEnumerable<string> FindPhotos(IEnumerable<string> fromPaths)
		{
			List<string> photoPaths = new List<string>();

			findPhotos(fromPaths, (folder) => { }, (file) => { photoPaths.Add(file); });

			return photoPaths;
		}

		private void findPhotos(IEnumerable<string> fromPaths, Action<string> folderAction, Action<string> fileAction)
		{
			var processedDir = new HashSet<string>();

			foreach (var path in fromPaths)
			{
				if (Directory.Exists(path))
				{
					var dir = new DirectoryInfo(path);
					dir.SearchFiles(new string[] { "*.jpg", "*.jpeg" },
						// file callback
						(file) => {

							if (Path.GetFileName(file).StartsWith(".") ||
								(new FileInfo(file).Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
								return true;

							fileAction(file); 
							return true;
						},

						// dir callback
						(folder) =>
						{
							if (processedDir.Contains(folder))
								return false;

							//if ((new DirectoryInfo(folder).Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
							//    return false;

							if (Path.GetFileName(folder).StartsWith("."))
								return false;

							foreach (var skipdir in ignorePath)
								if (folder.StartsWith(skipdir, StringComparison.InvariantCultureIgnoreCase))
									return false;

							if (StreamResourceDir.Length != 0 && path.StartsWith(StreamResourceDir, StringComparison.CurrentCultureIgnoreCase))
								return false;

							processedDir.Add(folder);
							
							folderAction(folder);

							return true;
						});
				}
				else if (File.Exists(path))
				{
					var ext = Path.GetExtension(path);
					if (ext.Equals(".jpg", StringComparison.InvariantCultureIgnoreCase) ||
						ext.Equals(".jpeg", StringComparison.InvariantCultureIgnoreCase))
					{
						fileAction(path);
					}
				}
			}
		}
	}
}