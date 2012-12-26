using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using Waveface.Stream.ClientFramework;
using Waveface.Stream.Core;
using Waveface.Stream.Model;

namespace Waveface.Stream.WindowsClient
{
	class PhotoSearch : IPhotoSearch
	{
		private List<string> ignorePaths = new List<string>();
		private HashSet<PathAndPhotoCount> _interestedPaths;
		private Dictionary<string, int> m_InterestedFileCountInPhotos = new Dictionary<string, int>();
		private BackgroundWorker backgroundWorker1;

		public delegate void pathFoundDelegate(string path, int photoCount);

		public PhotoSearch()
		{
			var paths = new string[] {
				Environment.GetEnvironmentVariable("windir"),
				Environment.GetEnvironmentVariable("ProgramData"),
				Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
				Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
				Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
				Environment.GetEnvironmentVariable("ProgramW6432"),
				@"c:\$Recycle.bin"
			};

			ignorePaths.AddRange(paths.Where(x => !string.IsNullOrEmpty(x)));
		}


		public IEnumerable<PathAndPhotoCount> InterestedPaths
		{
			get { return m_InterestedPaths; }
		}

		/// <summary>
		/// Gets the m_ interested paths.
		/// </summary>
		/// <value>The m_ interested paths.</value>
		private HashSet<PathAndPhotoCount> m_InterestedPaths
		{
			get
			{
				return _interestedPaths ?? (_interestedPaths = new HashSet<PathAndPhotoCount>());
			}
		}

		public void StartSearchAsync()
		{
			backgroundWorker1 = new BackgroundWorker();
			backgroundWorker1.DoWork += backgroundWorker1_DoWork;
			backgroundWorker1.RunWorkerAsync();
		}

		private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
		{
			Thread.CurrentThread.Priority = ThreadPriority.Lowest;

			var drives = DriveInfo.GetDrives();
			foreach (var drive in drives)
			{
				if (drive.DriveType == DriveType.Fixed)
					Search(drive.Name, (path, count) => AddInterestedPath(path, count));
			}
		}

		/// <summary>
		/// Searches valid photos with jpg/jpeg extension from a path
		/// </summary>
		/// <param name="path"></param>
		/// <param name="folderFound">custom action when a path containing photo is found</param>
		/// <returns>photo count under path and its subdirectories</returns>
		public void Search(string path, PhotoFolderFound folderFound = null)
		{
			try
			{
				if (isUnderIgnorePath(path))
					return;

				var jpgCount = JpgFileCount(path);
				if (jpgCount > 0 && folderFound != null)
				{
					folderFound(path, jpgCount);
				}

				var subdirs = Directory.GetDirectories(path, "*.*", SearchOption.TopDirectoryOnly);

				foreach (var subdir in subdirs)
				{
					Search(subdir, folderFound);
				}
			}
			catch
			{
			}
		}

		private static bool HasDateTimeInExif(string file)
		{
			try
			{
				var exifExtract = new ExifExtractor();
				var exif = exifExtract.extract(file);

				return exif != null &&
					(!string.IsNullOrEmpty(exif.DateTimeOriginal) || string.IsNullOrEmpty(exif.DateTimeOriginal));
			}
			catch
			{
				return false;
			}
		}

		private static int JpgFileCount(string path)
		{
			var total = 0;

			var jpg = Directory.GetFiles(path, "*.jpg");
			if (jpg != null && jpg.Any(HasDateTimeInExif))
			{
				total += jpg.Length;
			}

			var jpeg = Directory.GetFiles(path, "*.jpeg");
			if (jpeg != null && jpeg.Any(HasDateTimeInExif))
			{
				total += jpeg.Length;
			}

			return total;
		}

		private void AddInterestedPath(String path, int photoCount)
		{
			if (string.IsNullOrEmpty(path))
				return;

			if (!Path.IsPathRooted(path) || !Directory.Exists(path))
				return;

			if ((new Uri(path)).IsUnc)
				return;

			if ((new DirectoryInfo(path)).Attributes.HasFlag(FileAttributes.Hidden))
				return;

			if (m_InterestedPaths.Contains(new PathAndPhotoCount(path, photoCount)))
				return;

			var systemResourcePath = StationRegistry.GetValue("ResourceFolder", "").ToString();
			if (systemResourcePath.Length != 0 && path.StartsWith(systemResourcePath, StringComparison.CurrentCultureIgnoreCase))
				return;

			var pathRoot = Path.GetPathRoot(path);

			HashSet<String> interruptPath = new HashSet<String>()
			{
				pathRoot,
				Environment.GetEnvironmentVariable("USERPROFILE")
			};

			if (m_InterestedFileCountInPhotos.ContainsKey(path))
			{
				var interestedFileCount = m_InterestedFileCountInPhotos[path];
				var previousPath = path;
				var tempPath = path;
				while (!interruptPath.Contains(tempPath))
				{
					if (tempPath != path && interestedFileCount == m_InterestedFileCountInPhotos[tempPath])
					{
						break;
					}
					interestedFileCount = m_InterestedFileCountInPhotos[tempPath];
					previousPath = tempPath;
					tempPath = Path.GetDirectoryName(tempPath);
				}

				path = previousPath;
			}

			if (path == pathRoot)
				return;

			if (isUnderIgnorePath(path))
				return;

			foreach (var interestedPath in m_InterestedPaths)
			{
				if (path.StartsWith(interestedPath.path, StringComparison.CurrentCultureIgnoreCase))
					return;
			}
			m_InterestedPaths.Add(new PathAndPhotoCount(path, photoCount));
		}

		private bool isUnderIgnorePath(String path)
		{
			bool underEx = false;
			foreach (var unInterestedFolder in ignorePaths)
			{
				if (path.StartsWith(unInterestedFolder, StringComparison.CurrentCultureIgnoreCase))
				{
					underEx = true;
					break;
				}
			}
			return underEx;
		}

		public void ImportToStationAsync(IEnumerable<string> paths, string session_token)
		{
			var loginedSession = LoginedSessionCollection.Instance.FindOne(Query.EQ("_id", session_token));
			var groupID = loginedSession.groups.First().group_id;
			
			StationAPI.ImportPhoto(session_token, groupID, paths);
		}

	}
}
