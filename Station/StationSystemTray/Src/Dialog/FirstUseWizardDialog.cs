using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using StationSystemTray.Src.Control;
using System.Threading;
using System.IO;
using Wammer.Station;
using Microsoft.WindowsAPICodePack.Shell;
using System.Runtime.InteropServices;
using System.Reflection;
using StationSystemTray.Src.Class;

namespace StationSystemTray
{
	public partial class FirstUseWizardDialog : Form
	{
		//TODO:暫時將處理邏輯放在這，待處理...

		#region Const
		const string PICASA_DB_RELATIVED_STORAGE_PATH = @"Google\Picasa2\db3";
		const string ALBUM_PATH_PMP_FILENAME = "albumdata_filename.pmp";
		#endregion


		#region Var
		private HashSet<String> _interestedPaths;
		private string _picasaDBStoragePath;
		private string _albumPathPMPFileName;
		private InstallAppMonitor m_installAppMonitor;
		#endregion


		#region Private Property
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

		/// <summary>
		/// Gets or sets the m_ original title.
		/// </summary>
		/// <value>The m_ original title.</value>
		private string m_OriginalTitle { get; set; } 

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
		public HashSet<String> m_InterestedExtensions { get; set; }
		#endregion


		#region Constructor
		public FirstUseWizardDialog(string user_id)
		{
			InitializeComponent();

			m_InterestedExtensions = new HashSet<String> { ".jpg", ".jpeg", ".bmp", ".png" };

			m_OriginalTitle = this.Text;
			m_installAppMonitor = new InstallAppMonitor(user_id);

			wizardControl1.PageChanged += new EventHandler(wizardControl1_PageChanged);
			wizardControl1.WizardPagesChanged += new EventHandler(wizardControl1_WizardPagesChanged);
			var buildPersonalCloud = new BuildPersonalCloudUserControl();
			buildPersonalCloud.OnAppInstall += m_installAppMonitor.OnAppInstall;
			buildPersonalCloud.OnAppInstallCanceled += m_installAppMonitor.OnAppInstallCanceled;

			wizardControl1.SetWizardPages(new Control[]
			{
				buildPersonalCloud,
				new BuildPersonalCloudUserControl(),
				new FileImportControl(),
				new ServiceImportControl(),
				new CongratulationControl()
			});

			backgroundWorker1.RunWorkerAsync();
		} 
		#endregion


		#region Private Method
		/// <summary>
		/// Updates the title.
		/// </summary>
		private void UpdateTitle()
		{
			this.Text = string.Format("{0} ({1} of {2})", m_OriginalTitle, wizardControl1.PageIndex, wizardControl1.PageCount);
		}

		/// <summary>
		/// Updates the button.
		/// </summary>
		private void UpdateButton()
		{
			var pageIndex = wizardControl1.PageIndex;
			button2.Enabled = (pageIndex > 1);
			button1.Text = (pageIndex < wizardControl1.PageCount) ? "Next" : "Done";
		}

		/// <summary>
		/// Processes the file import step.
		/// </summary>
		private void ProcessFileImportStep()
		{
			var importControl = wizardControl1.CurrentPage as FileImportControl;
			importControl.ClearInterestedPaths();
			importControl.AddInterestedPaths(m_InterestedPaths);
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
				}
			}, null, null);

			foreach (var path in paths)
			{
				AddInterestedPath(path);
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
			if (size < 1024)
				return;

			var systemResourcePath = StationRegistry.GetValue("ResourceFolder", "").ToString();
			if (path.StartsWith(systemResourcePath, StringComparison.CurrentCultureIgnoreCase))
				return;

			string[] unInterestedFolders = new string[] 
			{
				Environment.GetEnvironmentVariable("windir"),
				Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
				Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
				Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
				Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
				Environment.GetFolderPath(Environment.SpecialFolder.History),
				Environment.GetFolderPath(Environment.SpecialFolder.InternetCache),
				Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles),
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
		#endregion


		#region Event Process
		/// <summary>
		/// Handles the Load event of the FirstUseWizardDialog control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void FirstUseWizardDialog_Load(object sender, EventArgs e)
		{
			UpdateTitle();
		}

		/// <summary>
		/// Handles the Click event of the button1 control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void button1_Click(object sender, EventArgs e)
		{
			if (wizardControl1.PageIndex == wizardControl1.PageCount)
			{
				Close();
				return;
			}
			wizardControl1.NextPage();
		}

		/// <summary>
		/// Handles the Click event of the button2 control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void button2_Click(object sender, EventArgs e)
		{
			wizardControl1.PreviousPage();
		} 

		/// <summary>
		/// Handles the PageChanged event of the wizardControl1 control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void wizardControl1_PageChanged(object sender, EventArgs e)
		{
			UpdateTitle();
			UpdateButton();

			if (wizardControl1.CurrentPage is FileImportControl)
			{
				ProcessFileImportStep();
			}
		}

		/// <summary>
		/// Handles the WizardPagesChanged event of the wizardControl1 control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void wizardControl1_WizardPagesChanged(object sender, EventArgs e)
		{
			UpdateButton();
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
		#endregion
	}
}
