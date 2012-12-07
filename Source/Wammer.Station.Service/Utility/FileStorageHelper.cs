using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Wammer.Utility
{
	public class FileStorageHelper
	{
		public static long GetAvailSize(string path)
		{
			string root = Path.GetPathRoot(path);
			Debug.Assert(root != null, "root != null");
			var di = new DriveInfo(root);
			return di.AvailableFreeSpace;
		}

		public static long GetUsedSize(string path)
		{
			var d = new DirectoryInfo(path);
			return DirSize(d);
		}

		/// <summary>
		/// Dirs the size.
		/// </summary>
		/// <param name="d">The d.</param>
		/// <returns></returns>
		private static long DirSize(DirectoryInfo d)
		{
			Type tp = Type.GetTypeFromProgID("Scripting.FileSystemObject");
			object fso = Activator.CreateInstance(tp);
			object fd = tp.InvokeMember("GetFolder", BindingFlags.InvokeMethod, null, fso, new object[] { d.FullName });
			long ret = Convert.ToInt64(tp.InvokeMember("Size", BindingFlags.GetProperty, null, fd, null));
			Marshal.ReleaseComObject(fso);
			return ret;
		}
	}
}
