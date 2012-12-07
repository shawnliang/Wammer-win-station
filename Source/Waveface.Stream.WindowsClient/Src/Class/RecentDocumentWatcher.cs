using System;
using System.IO;
using System.Reflection;

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

			watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime;
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

		public static string getShortcutTargetFile(string shortcutFilename)
		{
			var type = Type.GetTypeFromProgID("WScript.Shell");

			object instance = Activator.CreateInstance(type);

			var result = type.InvokeMember("CreateShortCut", BindingFlags.InvokeMethod, null, instance, new object[] { shortcutFilename });

			var targetFile = result.GetType().InvokeMember("TargetPath", BindingFlags.GetProperty, null, result, null) as string;
			return targetFile;
		}

	}

	class FileTouchEventArgs : EventArgs
	{
		public string File { get; private set; }

		public FileTouchEventArgs(string file)
		{
			File = file;
		}
	}
}
