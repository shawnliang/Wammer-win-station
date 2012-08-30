using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace Waveface
{
	public abstract class ContentProviderBase : IContentProvider
	{
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


		#region Peoprty
		/// <summary>
		/// Gets the support types.
		/// </summary>
		/// <value>The support types.</value>
		public abstract IEnumerable<ContentType> SupportTypes { get; }
		#endregion


		#region Protected Method
		/// <summary>
		/// Enumerates the files.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <param name="searchPattern">The search pattern.</param>
		/// <param name="searchOption">The search option.</param>
		/// <returns></returns>
		protected IEnumerable<string> EnumerateFiles(string path, string searchPattern = "*.*", SearchOption searchOption = SearchOption.AllDirectories)
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
		#endregion


		#region Public Method
		/// <summary>
		/// Determines whether the specified type is support.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns>
		/// 	<c>true</c> if the specified type is support; otherwise, <c>false</c>.
		/// </returns>
		public bool IsSupport(ContentType type)
		{
			var supportTypes = this.SupportTypes;
			if (supportTypes == null)
				return false;
			return supportTypes.Contains(type);
		}

		/// <summary>
		/// Gets the contents.
		/// </summary>
		/// <returns></returns>
		public abstract IEnumerable<IContent> GetContents();
		#endregion
	}
}
