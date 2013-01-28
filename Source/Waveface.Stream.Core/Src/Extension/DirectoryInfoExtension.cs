using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

public static class DirectoryInfoExtension
{
	public delegate bool PathCallback(string path);
	public delegate void PathErrorCallback(string path, Exception err);

	#region PInvoke
	[DllImport("kernel32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
	private static extern IntPtr FindFirstFile(string pFileName, ref  WIN32_FIND_DATA pFindFileData);

	[DllImport("kernel32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
	private static extern bool FindNextFile(IntPtr hndFindFile, ref  WIN32_FIND_DATA lpFindFileData);

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern bool FindClose(IntPtr hndFindFile);
	#endregion

	#region Struct
	[Serializable, StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto), BestFitMapping(false)]
	internal struct WIN32_FIND_DATA
	{
		public FileAttributes dwFileAttributes;
		public uint ftCreationTime_dwLowDateTime;
		public uint ftCreationTime_dwHighDateTime;
		public uint ftLastAccessTime_dwLowDateTime;
		public uint ftLastAccessTime_dwHighDateTime;
		public uint ftLastWriteTime_dwLowDateTime;
		public uint ftLastWriteTime_dwHighDateTime;
		public uint nFileSizeHigh;
		public uint nFileSizeLow;
		public int dwReserved0;
		public int dwReserved1;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
		public string cFileName;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
		public string cAlternateFileName;
	}
	#endregion

	#region Runtime Const
	private static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
	#endregion


	#region Private Method
	private static IEnumerable<string> EnumerateFiles(string path, string searchPattern = "*.*", SearchOption searchOption = SearchOption.AllDirectories)
	{
		IntPtr hFind = INVALID_HANDLE_VALUE;
		WIN32_FIND_DATA FindFileData = default(WIN32_FIND_DATA);

		hFind = FindFirstFile(Path.Combine(path, searchPattern), ref  FindFileData);
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
			while (FindNextFile(hFind, ref  FindFileData));
		}
		FindClose(hFind);
	}

	private static void searchFiles(string path, string[] patterns, PathCallback fileCB, PathCallback folderCB, PathErrorCallback errorCB)
	{
		if (folderCB != null && !folderCB(path))
			return;

		foreach (var pattern in patterns)
		{
			var files = Directory.GetFiles(path, pattern, SearchOption.TopDirectoryOnly);

			if (files == null)
				continue;

			foreach (var file in files)
			{
				if (!fileCB(file))
					return;
			}
		}


		var subdirs = Directory.GetDirectories(path, "*.*", SearchOption.TopDirectoryOnly);

		if (subdirs == null)
			return;

		foreach (var subdir in subdirs)
		{
			try
			{
				searchFiles(subdir, patterns, fileCB, folderCB, errorCB);
			}
			catch (Exception e)
			{
				if (errorCB != null)
					errorCB(subdir, e);
			}
		}
	}

	#endregion


	#region Public Method
	public static void SearchFiles(this DirectoryInfo dir, string[] patterns, PathCallback fileCB, PathCallback folderCB = null, PathErrorCallback errorCB = null)
	{
		if (fileCB == null)
			throw new ArgumentNullException("fileCB");
		if (patterns == null)
			throw new ArgumentNullException("patterns");

		searchFiles(dir.FullName, patterns, fileCB, folderCB, errorCB);
	}


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
