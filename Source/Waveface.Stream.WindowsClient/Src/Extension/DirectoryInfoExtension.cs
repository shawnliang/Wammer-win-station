using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Waveface.Stream.WindowsClient
{
	public static class DirectoryInfoExtension
	{
		#region Runtime Const
		private static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
		#endregion


		#region Private Method
		private static IEnumerable<string> EnumerateFiles(string path, string searchPattern = "*.*", SearchOption searchOption = SearchOption.AllDirectories)
		{
			IntPtr hFind = INVALID_HANDLE_VALUE;
			WIN32_FIND_DATA FindFileData = default(WIN32_FIND_DATA);

			hFind = NativeMethods.FindFirstFile(Path.Combine(path, searchPattern), ref  FindFileData);
			if (hFind != INVALID_HANDLE_VALUE)
			{
				do
				{
					if (FindFileData.cFileName.Equals(@".") || FindFileData.cFileName.Equals(@".."))
						continue;

					var folderOrFilePath = Path.Combine(path, FindFileData.cFileName);
					if (searchOption == SearchOption.AllDirectories && ((FindFileData.dwFileAttributes & FileAttributes.Directory) == FileAttributes.Directory))
					{
						foreach (var file in EnumerateFiles(folderOrFilePath))
							yield return file;
					}
					else
					{
						yield return folderOrFilePath;
					}
				}
				while (NativeMethods.FindNextFile(hFind, ref  FindFileData));
			}
			NativeMethods.FindClose(hFind);
		}
		#endregion


		#region Public Method
		/// <summary>
		/// Enumerates the files.
		/// </summary>
		/// <param name="directoryInfo">The directory info.</param>
		/// <param name="searchPattern">The search pattern.</param>
		/// <param name="searchOption">The search option.</param>
		/// <returns></returns>
		public static IEnumerable<string> EnumerateFiles(this DirectoryInfo directoryInfo, string searchPattern = "*.*", SearchOption searchOption = SearchOption.AllDirectories)
		{
			var path = directoryInfo.FullName;
			return EnumerateFiles(path, searchPattern, searchOption);
		}

		/// <summary>
		/// Gets the size.
		/// </summary>
		/// <param name="dirInfo">The dir info.</param>
		/// <returns></returns>
		public static long GetSize(this DirectoryInfo dirInfo)
		{
			Type tp = Type.GetTypeFromProgID("Scripting.FileSystemObject");
			object fso = Activator.CreateInstance(tp);
			object fd = tp.InvokeMember("GetFolder", BindingFlags.InvokeMethod, null, fso, new object[] { dirInfo.FullName });
			long ret = Convert.ToInt64(tp.InvokeMember("Size", BindingFlags.GetProperty, null, fd, null));
			Marshal.ReleaseComObject(fso);
			return ret;
		}
		#endregion
	}
}
