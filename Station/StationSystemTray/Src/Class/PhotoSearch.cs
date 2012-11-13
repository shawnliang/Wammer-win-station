using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ComponentModel;
using System.Threading;
using Microsoft.WindowsAPICodePack.Shell;
using Wammer.Model;
using MongoDB.Driver.Builders;
using System.Reflection;
using System.Runtime.InteropServices;
using Wammer.Station;

namespace StationSystemTray
{
	class PhotoSearch : IPhotoSearch
	{

		const string PICASA_DB_RELATIVED_STORAGE_PATH = @"Google\Picasa2\db3";
		const string ALBUM_PATH_PMP_FILENAME = "albumdata_filename.pmp";

		private HashSet<string> ignorePath = new HashSet<string>();
		private HashSet<PathAndPhotoCount> _interestedPaths;
		private Dictionary<string, int> m_InterestedFileCountInPhotos = new Dictionary<string, int>();
		private BackgroundWorker backgroundWorker1;

		public event EventHandler<MetadataUploadEventArgs> MetadataUploaded;

		public event EventHandler<FileImportedEventArgs> FileImported;

		public event EventHandler<Wammer.Station.ImportDoneEventArgs> ImportDone;


		public PhotoSearch()
		{
			ignorePath.Add(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));
			ignorePath.Add(Environment.GetFolderPath(Environment.SpecialFolder.System));
			ignorePath.Add(Environment.GetFolderPath(Environment.SpecialFolder.InternetCache));
			ignorePath.Add(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles));
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
					Search(drive.Name);
			}
		}

		private void Search(string path)
		{
			try
			{
				if (ignorePath.Contains(path))
					return;

				var jpgCount = JpgFileCount(path);
				if (jpgCount > 0)
				{
					AddInterestedPath(path, jpgCount);
				}

				var files = Directory.GetDirectories(path, "*.*", SearchOption.TopDirectoryOnly);

				foreach (var file in files)
				{
					Search(file);
				}

			}
			catch (Exception e)
			{
			}
		}

		private static bool HasDateTimeInExif(string file)
		{
			try
			{
				var exifExtract = new Wammer.Station.AttachmentUpload.ExifExtractor();
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

			string[] unInterestedFolders = new string[] 
			{
				Environment.GetEnvironmentVariable("windir"),
				Environment.GetEnvironmentVariable("ProgramData"),
				Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
				Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
				Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
				Environment.GetEnvironmentVariable("ProgramW6432"),
				@"c:\$Recycle.bin"
			};

			foreach (var unInterestedFolder in unInterestedFolders)
			{
				if (path.StartsWith(unInterestedFolder, StringComparison.CurrentCultureIgnoreCase))
					return;
			}

			foreach (var interestedPath in m_InterestedPaths)
			{
				if (path.StartsWith(interestedPath.path, StringComparison.CurrentCultureIgnoreCase))
					return;
			}
			m_InterestedPaths.Add(new PathAndPhotoCount(path, photoCount));
		}

		public void ImportToStationAsync(IEnumerable<string> paths, string session_token)
		{
			var loginedSession = LoginedSessionCollection.Instance.FindOne(Query.EQ("_id", session_token));
			var groupID = loginedSession.groups.First().group_id;
			var transaction = new Wammer.Station.Management.ImportTranscation(
				loginedSession.user.user_id, session_token, StationAPI.API_KEY, paths);

			transaction.ImportDone += new EventHandler<Wammer.Station.ImportDoneEventArgs>(transaction_ImportDone);
			transaction.FileImported += new EventHandler<FileImportedEventArgs>(transaction_FileImported2);
			transaction.MetadataUploaded += new EventHandler<MetadataUploadEventArgs>(transaction_MetadataUploaded);

			transaction.ImportFileAsync();
		}

		void transaction_MetadataUploaded(object sender, MetadataUploadEventArgs e)
		{
			var handler = MetadataUploaded;
			if (handler != null)
				handler(this, e);
		}

		void transaction_FileImported2(object sender, FileImportedEventArgs e)
		{
			var handler = FileImported;
			if (handler != null)
				handler(this, e);
		}

		void transaction_ImportDone(object sender, Wammer.Station.ImportDoneEventArgs e)
		{
			var handler = ImportDone;
			if (handler != null)
				handler(this, e);
		}
	}
}
