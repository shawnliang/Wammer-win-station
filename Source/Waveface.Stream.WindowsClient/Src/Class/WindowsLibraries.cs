using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Waveface.Stream.WindowsClient
{
	public static class WindowsLibraries
	{
		#region Const
		const string PICTURE_LIBRARYS_NAME = "Pictures";
		#endregion


		#region Private Method
		private static bool IsWinVistaOrLater()
		{
			bool isWinVistaOrLater;

			var os = Environment.OSVersion;
			if (os.Platform == PlatformID.Win32NT && os.Version.Major == 6)
				isWinVistaOrLater = true;
			else
				isWinVistaOrLater = false;
			return isWinVistaOrLater;
		}
		#endregion


		#region Public Method
		public static IEnumerable<string> GetLibrariesFolders()
		{
			if (!IsWinVistaOrLater())
			{
				yield return Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
				yield break;
			}

			using (ShellLibrary library = ShellLibrary.Load("Pictures", false))
			{
				foreach (ShellFolder folder in library)
				{
					yield return folder.ParsingName;
				}
			}
		}

		public static IEnumerable<string> GetLibrariesFiles()
		{
			return from folder in GetLibrariesFolders()
				   from file in new DirectoryInfo(folder).EnumerateFiles()
				   select file;
		}
		#endregion
	}
}
