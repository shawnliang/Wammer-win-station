using System.IO;

public static class FileInfoExtension
{
	public static string GetAssociatedExeFile(this FileInfo fileInfo, bool throwException = true)
	{
		return FileHelper.GetAssociatedExeFile(fileInfo.FullName, throwException);
	}

}