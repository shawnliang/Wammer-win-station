using System;
using System.Collections.Generic;
using System.IO;

namespace Wammer.Station
{
	public class FolderUtility : IFolderUtility
	{

		public bool IsOnSameDrive(string path1, string path2)
		{
			return Path.GetPathRoot(path1).Equals(Path.GetPathRoot(path2), StringComparison.InvariantCultureIgnoreCase);
		}

		public void MoveOnSameDrive(string src, string dest)
		{
			if (!Directory.Exists(dest))
				throw new IOException("Path does not exist: " + dest);

			foreach (var entry in Directory.GetFileSystemEntries(src))
			{
				var fileName = Path.GetFileName(entry);
				Directory.Move(entry, Path.Combine(dest, fileName));
			}

			Directory.Delete(src);
		}

		public void RecursiveCopy(string src, string dest)
		{
			if (!Directory.Exists(dest))
				throw new IOException("Path does not exist: " + dest);

			var srcInfo = new DirectoryInfo(src);
			var files = srcInfo.GetFiles();
			Array.ForEach(files, f => f.CopyTo(Path.Combine(dest, f.Name)));

			var subDirs = srcInfo.GetDirectories();
			foreach (var subDir in subDirs)
			{
				var destSubDir = Path.Combine(dest, subDir.Name);

				if (!Directory.Exists(destSubDir))
					Directory.CreateDirectory(destSubDir);

				RecursiveCopy(subDir.FullName, destSubDir);
			}
		}

		public void RecursiveDelete(string path)
		{
			Directory.Delete(path, true);
		}

		public IEnumerable<string> GetSubDirectories(string path)
		{
			return Directory.GetDirectories(path);
		}

		public IEnumerable<string> GetSubEntries(string path)
		{
			return Directory.GetFileSystemEntries(path);
		}
	}
}
