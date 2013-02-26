using System;
using System.IO;

namespace Waveface.Stream.WindowsClient
{
	public class RecentDocumentWatcher
	{

		#region Static Var
		private static RecentDocumentWatcher _instance;
		#endregion

		#region Public Static Property
		public static RecentDocumentWatcher Instance
		{
			get
			{
				return _instance ?? (_instance = new RecentDocumentWatcher());
			}
		}
		#endregion


		#region Public Property
		public Boolean Enabled
		{
			get
			{
				return watcher.EnableRaisingEvents;
			}
			set
			{
				watcher.EnableRaisingEvents = value;
			}
		}
		#endregion

		private FileSystemWatcher watcher;


		#region Constructor
		private RecentDocumentWatcher()
		{
			var recentDir = Environment.GetFolderPath(Environment.SpecialFolder.Recent);
			watcher = new FileSystemWatcher(recentDir, "*.lnk");

			watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime;
			watcher.Changed += watcher_Touched;
			watcher.Created += watcher_Touched;
		}
		#endregion


		public event EventHandler<FileTouchEventArgs> FileTouched;

		public void Start()
		{
			this.Enabled = true;
		}

		public void Stop()
		{
			this.Enabled = false;
		}

		void watcher_Touched(object sender, FileSystemEventArgs e)
		{
			try
			{
				if (e.ChangeType == WatcherChangeTypes.Changed || e.ChangeType == WatcherChangeTypes.Created)
				{
					var target = getShortcutTargetFile(e.FullPath);

					if (string.IsNullOrEmpty(target) || !File.Exists(target))
						return;

					var ext = Path.GetExtension(target);

					if (ext.Equals(".pdf", StringComparison.InvariantCultureIgnoreCase) ||
						ext.Equals(".ppt", StringComparison.InvariantCultureIgnoreCase) ||
						ext.Equals(".pptx", StringComparison.InvariantCultureIgnoreCase))
					{
						var handler = FileTouched;
						if (handler != null)
							handler(this, new FileTouchEventArgs(target));
					}
				}
			}
			catch (Exception ex)
			{
				System.Windows.Forms.MessageBox.Show(ex.ToString());
			}
		}

		public static string getShortcutTargetFile(string shortcutFilename)
		{
			string pathOnly = System.IO.Path.GetDirectoryName(shortcutFilename);
			string filenameOnly = System.IO.Path.GetFileName(shortcutFilename);


			var t = Type.GetTypeFromCLSID(new Guid("13709620-C279-11CE-A49E-444553540000"));
			var shell = Activator.CreateInstance(t);

			var folder = t.InvokeMember("NameSpace", System.Reflection.BindingFlags.InvokeMethod, null, shell, new object[] { pathOnly });
			if (folder == null)
				return null;

			var folderItem = folder.GetType().InvokeMember("ParseName", System.Reflection.BindingFlags.InvokeMethod, null, folder, new object[] { filenameOnly });
			if (folderItem == null)
				return null;

			var getLinkResult = folderItem.GetType().InvokeMember("GetLink", System.Reflection.BindingFlags.GetProperty, null, folderItem, null);
			if (getLinkResult == null)
				return null;

			var path = getLinkResult.GetType().InvokeMember("Path", System.Reflection.BindingFlags.GetProperty, null, getLinkResult, null);
			return (string)path;
		}

	}

	public class FileTouchEventArgs : EventArgs
	{
		public string File { get; private set; }

		public FileTouchEventArgs(string file)
		{
			File = file;
		}
	}
}
