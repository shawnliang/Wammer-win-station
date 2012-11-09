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

		private HashSet<String> _interestedPaths;
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
			get
			{
				return _albumPathPMPFileName ??
					(_albumPathPMPFileName = Path.Combine(m_PicasaDBStoragePath, ALBUM_PATH_PMP_FILENAME));
			}
		}


		public IEnumerable<string> InterestedPaths
		{
			get { return m_InterestedPaths; }
		}

		/// <summary>
		/// Gets the m_ interested paths.
		/// </summary>
		/// <value>The m_ interested paths.</value>
		private HashSet<String> m_InterestedPaths
		{
			get
			{
				return _interestedPaths ?? (_interestedPaths = new HashSet<String>());
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


			var interestedPaths = new List<string>();
			using (ShellLibrary library = ShellLibrary.Load("Pictures", false))
			{
				foreach (ShellFolder folder in library)
				{
					var folderPath = folder.ParsingName;

					if (folderPath.Length == 0)
						continue;

					interestedPaths.Add(folderPath);
				}
			}

			var recentlyPaths = (from file in RecentlyFileHelper.GetRecentlyFiles()
								 select Path.GetDirectoryName(file));

			recentlyPaths = recentlyPaths.Distinct();
			interestedPaths.AddRange(recentlyPaths.Distinct());


			if (Directory.Exists(m_PicasaDBStoragePath) &&
				File.Exists(m_AlbumPathPMPFileName) &&
				IsValidPicasaFormat(m_AlbumPathPMPFileName))
			{
				interestedPaths.AddRange(ReadAllStringField(m_AlbumPathPMPFileName));
			}

			AddInterestedPath(interestedPaths);


			foreach (var drive in DriveInfo.GetDrives())
			{
				if (drive.DriveType == DriveType.Fixed)
					MonitorPath(drive.Name);
			}
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

		private void MonitorPath(string searchPath)
		{
			var files = new HashSet<string>();
			var paths = new HashSet<string>();
			var scanner = new MFTScanner();


			scanner.FindAllFiles(searchPath, (filePath, fileSize) =>
			{
				var extension = Path.GetExtension(filePath).ToLower();

				if (m_InterestedExtensions.Contains(extension))
				{
					var path = Path.GetDirectoryName(filePath);
					if (!paths.Contains(path))
					{
						paths.Add(path);
					}
					files.Add(filePath);

					var pathRoot = Path.GetPathRoot(path);
					while (!path.Equals(pathRoot, StringComparison.CurrentCultureIgnoreCase))
					{
						if (!m_InterestedFileCountInPhotos.ContainsKey(path))
						{
							m_InterestedFileCountInPhotos[path] = 0;
							continue;
						}

						m_InterestedFileCountInPhotos[path] += 1;
						path = Path.GetDirectoryName(path);
					}
				}
			}, null, null);

			AddInterestedPath(paths);
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

		private void AddInterestedPath(String path)
		{
			if (string.IsNullOrEmpty(path))
				return;

			if (!Path.IsPathRooted(path) || !Directory.Exists(path))
				return;

			if ((new Uri(path)).IsUnc)
				return;

			if ((new DirectoryInfo(path)).Attributes.HasFlag(FileAttributes.Hidden))
				return;

			if (m_InterestedPaths.Contains(path))
				return;

			var size = GetFolderSize(path);
			if (size < 100 * 1024 * 1024)
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
				if (path.StartsWith(interestedPath, StringComparison.CurrentCultureIgnoreCase))
					return;
			}
			m_InterestedPaths.Add(path);
		}

		private void AddInterestedPath(IEnumerable<String> paths)
		{
			foreach (var path in paths.OrderBy(item => item.Length))
			{
				AddInterestedPath(path);
			}
		}

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
		}

		public void ImportToStation(IEnumerable<string> paths, string session_token)
		{
			var loginedSession = LoginedSessionCollection.Instance.FindOne(Query.EQ("_id", session_token));
			var groupID = loginedSession.groups.First().group_id;
			StationAPI.Import(session_token, groupID, string.Format("[{0}]", string.Join(",", paths.ToArray())));
		}
	}
}
