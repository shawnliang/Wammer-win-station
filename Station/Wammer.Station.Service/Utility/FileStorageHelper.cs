using System.IO;

namespace Wammer.Utility
{
	public class FileStorageHelper
	{
		public static long GetAvailSize(string path)
		{
			string root = Path.GetPathRoot(path);
			DriveInfo di = new DriveInfo(root);
			return di.AvailableFreeSpace;
		}

		public static long GetUsedSize(string path)
		{
			DirectoryInfo d = new DirectoryInfo(path);
			return DirSize(d);
		}

		private static long DirSize(DirectoryInfo d)
		{
			long size = 0;
			// add file sizes
			FileInfo[] fis = d.GetFiles();
			foreach (FileInfo fi in fis)
			{
				size += fi.Length;
			}
			// add subdirectory sizes
			DirectoryInfo[] dis = d.GetDirectories();
			foreach (DirectoryInfo di in dis)
			{
				size += DirSize(di);
			}
			return size;
		}
	}
}
