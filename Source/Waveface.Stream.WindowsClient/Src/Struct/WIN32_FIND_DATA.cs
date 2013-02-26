using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

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