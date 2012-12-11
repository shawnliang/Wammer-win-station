using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

public static class FileInfoExtension
{
	public static string GetAssociatedExeFile(this FileInfo fileInfo, bool throwException = true)
	{
		return FileHelper.GetAssociatedExeFile(fileInfo.FullName, throwException);
	}

}