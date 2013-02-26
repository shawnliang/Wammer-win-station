using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;


public class NoAssociatedFileTypeException : ApplicationException
{
}

public static class FileHelper
{
	#region Const
	const int SE_ERR_FNF = 2;
	const int SE_ERR_OOM = 8;
	const int SE_ERR_NOASSOC = 31;
	#endregion

	[DllImport("shell32.dll")]
	static extern int FindExecutable(string lpFile, string lpDirectory, [Out] StringBuilder lpResult);


	public static string GetAssociatedExeFile(string file, bool throwException = true)
	{
		if (string.IsNullOrEmpty(file))
			throw new ArgumentNullException("file");

		file = (new Uri(file)).LocalPath;

		StringBuilder sb = new StringBuilder(512);
		int result = FindExecutable(file, null, sb);

		if (result > 32)
		{
			return sb.ToString();

		}
		else
		{
			if (!throwException)
			{
				return string.Empty;
			}

			switch (result)
			{
				case SE_ERR_FNF:
					throw new FileNotFoundException();
				case SE_ERR_NOASSOC:
					throw new NoAssociatedFileTypeException();
				case SE_ERR_OOM:
					throw new OutOfMemoryException();
			}
			throw new Exception();
		}
	}

}