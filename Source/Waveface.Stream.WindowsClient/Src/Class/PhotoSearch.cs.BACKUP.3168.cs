using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Waveface.Stream.ClientFramework;
using Waveface.Stream.Model;

namespace Waveface.Stream.WindowsClient
{
	class PhotoSearch : IPhotoSearch
	{
		private List<string> ignorePaths= new List<string>();		
		private HashSet<PathAndPhotoCount> _interestedPaths;
		private string _picasaDBStoragePath;
		private string _albumPathPMPFileName;
		private Dictionary<string, int> m_InterestedFileCountInPhotos = new Dictionary<string, int>();
		private BackgroundWorker backgroundWorker1;


		/// <summary>
		/// Gets the m_ picasa DB storage path.
		/// </summary>
		/// <value>The m_ picasa DB storage path.</value>
		private string m_PicasaDBStoragePath
		{
			get
			{
				return _picasaDBStoragePath ??
					(_picasaDBStoragePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), PICASA_DB_RELATIVED_STORAGE_PATH));
			}
		}

		/// <summary>
		/// Gets the name of the m_ album path PMP file.
		/// </summary>
		/// <value>The name of the m_ album path PMP file.</value>
		private string m_AlbumPathPMPFileName
		{
<<<<<<< HEAD:Source/Waveface.Stream.WindowsClient/Src/Class/PhotoSearch.cs
			get
			{
				return _albumPathPMPFileName ??
					(_albumPathPMPFileName = Path.Combine(m_PicasaDBStoragePath, ALBUM_PATH_PMP_FILENAME));
			}
=======
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
>>>>>>> develop:Station/StationSystemTray/Src/Class/PhotoSearch.cs
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

		/// <summary>
		/// Gets or sets the m_ interested extensions.
		/// </summary>
		/// <value>The m_ interested extensions.</value>
		private HashSet<String> m_InterestedExtensions { get; set; }


		public PhotoSearch()
		{
			m_InterestedExtensions = new HashSet<String> { ".jpg", ".jpeg", ".bmp", ".png" };
			ignorePath.Add(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));
			ignorePath.Add(Environment.GetFolderPath(Environment.SpecialFolder.System));
			ignorePath.Add(Environment.GetFolderPath(Environment.SpecialFolder.InternetCache));
			ignorePath.Add(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles));
		}


		public void StartSearchAsync()
		{
			backgroundWorker1 = new BackgroundWorker();
			backgroundWorker1.DoWork += backgroundWorker1_DoWork;
			backgroundWorker1.RunWorkerAsync();
		}

		/// <summary>
		/// Handles the DoWork event of the backgroundWorker1 control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.ComponentModel.DoWorkEventArgs"/> instance containing the event data.</param>
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
				if (isUnderIgnorePath(path))
					return;

				var jpgCount = JpgFileCount(path);
				if (jpgCount > 0)
				{
					AddInterestedPath(path, jpgCount);
				}

				var subdirs = Directory.GetDirectories(path, "*.*", SearchOption.TopDirectoryOnly);

				foreach (var subdir in subdirs)
				{
<<<<<<< HEAD:Source/Waveface.Stream.WindowsClient/Src/Class/PhotoSearch.cs
					Search(file);
=======
					Search(subdir, folderFound);
>>>>>>> develop:Station/StationSystemTray/Src/Class/PhotoSearch.cs
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
			var jpg = Directory.GetFiles(path, "*.jpg");
			if (jpg != null && jpg.Any(HasDateTimeInExif))
			{
				return jpg.Length;
			}

			var jpeg = Directory.GetFiles(path, "*.jpeg");
			if (jpeg != null && jpeg.Any(HasDateTimeInExif))
				return jpg.Length;

			return 0;
		}

		/// <summary>
		/// Determines whether [is valid picasa format] [the specified file].
		/// </summary>
		/// <param name="file">The file.</param>
		/// <returns>
		/// 	<c>true</c> if [is valid picasa format] [the specified file]; otherwise, <c>false</c>.
		/// </returns>
		private Boolean IsValidPicasaFormat(string file)
		{
			using (var fs = File.OpenRead(file))
			{
				using (var br = new BinaryReader(fs))
				{
					return IsValidPicasaFormat(br);
				}
			}
		}


		/// <summary>
		/// Determines whether [is valid picasa format] [the specified br].
		/// </summary>
		/// <param name="br">The br.</param>
		/// <returns>
		/// 	<c>true</c> if [is valid picasa format] [the specified br]; otherwise, <c>false</c>.
		/// </returns>
		private Boolean IsValidPicasaFormat(BinaryReader br)
		{
			if (br == null)
				throw new ArgumentNullException("br");

			var position = br.BaseStream.Position;
			try
			{
				br.BaseStream.Seek(0, SeekOrigin.Begin);
				var magic = br.ReadBytes(4);
				if (magic[0] != 0xcd ||
					magic[1] != 0xcc ||
					magic[2] != 0xcc ||
					magic[3] != 0x3f)
				{
					return false;
				}

				var type = br.ReadInt16();

				if (0x1332 != br.ReadInt16())
				{
					return false;
				}

				if (0x00000002 != br.ReadInt32())
				{
					return false;
				}

				if (type != br.ReadInt16())
				{
					return false;
				}

				if (0x1332 != br.ReadInt16())
				{
					return false;
				}
				return true;
			}
			finally
			{
				br.BaseStream.Seek(position, SeekOrigin.Begin);
			}
		}

		private long GetFolderSize(string folder)
		{
			try
			{
				Type tp = Type.GetTypeFromProgID("Scripting.FileSystemObject");
				object fso = Activator.CreateInstance(tp);
				object fd = tp.InvokeMember("GetFolder", BindingFlags.InvokeMethod, null, fso, new object[] { folder });
				long ret = Convert.ToInt64(tp.InvokeMember("Size", BindingFlags.GetProperty, null, fd, null));
				Marshal.ReleaseComObject(fso);
				return ret;
			}
			catch (Exception)
			{
				return 0;
			}
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

<<<<<<< HEAD:Source/Waveface.Stream.WindowsClient/Src/Class/PhotoSearch.cs
		/// <summary>
		/// Checks the picasa format.
		/// </summary>
		/// <param name="file">The file.</param>
		private void CheckPicasaFormat(string file)
		{
			using (var fs = File.OpenRead(file))
			{
				using (var br = new BinaryReader(fs))
				{
					CheckPicasaFormat(br);
				}
			}
		}

		/// <summary>
		/// Checks the picasa format.
		/// </summary>
		/// <param name="br">The br.</param>
		private void CheckPicasaFormat(BinaryReader br)
		{
			if (!IsValidPicasaFormat(br))
				throw new FileFormatException("Incorrect picasa file format.");
		}




		/// <summary>
		/// Reads all string field.
		/// </summary>
		/// <param name="file">The file.</param>
		/// <returns></returns>
		private IEnumerable<string> ReadAllStringField(string file)
		{
			using (var fs = File.OpenRead(file))
			{
				using (var br = new BinaryReader(fs))
				{
					br.BaseStream.Seek(16, SeekOrigin.Begin);
					var number = br.ReadInt32();

					for (long i = 0; i < number; i++)
					{
						yield return getString(br);
					}
					yield break;
				}
			}
		}

		/// <summary>
		/// Reads all string field.
		/// </summary>
		/// <param name="br">The br.</param>
		/// <returns></returns>
		private IEnumerable<string> ReadAllStringField(BinaryReader br)
		{
			br.BaseStream.Seek(16, SeekOrigin.Begin);
			var number = br.ReadInt32();

			for (long i = 0; i < number; i++)
			{
				yield return getString(br);
			}
			yield break;
		}

		/// <summary>
		/// Gets the string.
		/// </summary>
		/// <param name="br">The br.</param>
		/// <returns></returns>
		private String getString(BinaryReader br)
		{
			var sb = new StringBuilder();
			int c;
			while ((c = br.Read()) != 0)
			{
				sb.Append((char)c);
			}
			return sb.ToString();
=======
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
>>>>>>> develop:Station/StationSystemTray/Src/Class/PhotoSearch.cs
		}

		public void ImportToStationAsync(IEnumerable<string> paths, string session_token)
		{
			var loginedSession = LoginedSessionCollection.Instance.FindOne(Query.EQ("_id", session_token));
			var groupID = loginedSession.groups.First().group_id;
			//StationAPI.Import(session_token, groupID, string.Format("[{0}]", string.Join(",", paths.ToArray())));

			var transaction = new ImportTranscation(
				loginedSession.user.user_id, session_token, StationAPI.API_KEY, paths);

			transaction.ImportDone += new EventHandler<ImportDoneEventArgs>(transaction_ImportDone);
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

		void transaction_ImportDone(object sender, ImportDoneEventArgs e)
		{
			var handler = ImportDone;
			if (handler != null)
				handler(this, e);
		}

		public event EventHandler<MetadataUploadEventArgs> MetadataUploaded;

		public event EventHandler<FileImportedEventArgs> FileImported;

		public event EventHandler<ImportDoneEventArgs> ImportDone;
	}
}
