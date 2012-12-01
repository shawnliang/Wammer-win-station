using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Shell32;

namespace Waveface.Stream.WindowsClient
{
	class RecentDocumentWatcher
	{
		private FileSystemWatcher watcher;

		public event EventHandler<FileTouchEventArgs> FileTouched;

		public RecentDocumentWatcher()
		{
			var recentDir = Environment.GetFolderPath(Environment.SpecialFolder.Recent);
			watcher = new FileSystemWatcher(recentDir, "*.lnk");

			watcher.NotifyFilter = NotifyFilters.LastWrite| NotifyFilters.CreationTime;
			watcher.Changed += watcher_Touched;
			watcher.Created += watcher_Touched;
		}

		public void Start()
		{
			watcher.EnableRaisingEvents = true;
		}

		void watcher_Touched(object sender, FileSystemEventArgs e)
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

		private static string getShortcutTargetFile(string shortcutFilename)
		{
			string pathOnly = System.IO.Path.GetDirectoryName(shortcutFilename);
			string filenameOnly = System.IO.Path.GetFileName(shortcutFilename);

			Shell shell = new Shell();
			Folder folder = shell.NameSpace(pathOnly);
			FolderItem folderItem = folder.ParseName(filenameOnly);
			if (folderItem != null)
			{
				Shell32.ShellLinkObject link = (Shell32.ShellLinkObject)folderItem.GetLink;
				return link.Path;
			}

			return string.Empty;
		}
	}

	class FileTouchEventArgs: EventArgs
	{
		public string File { get; private set; }
		
		public FileTouchEventArgs(string file)
		{
			File = file;
		}
	}
}
