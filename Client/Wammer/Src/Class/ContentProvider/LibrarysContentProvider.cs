using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.WindowsAPICodePack.Shell;

namespace Waveface
{
	/// <summary>
	/// 
	/// </summary>
	public class LibrarysContentProvider : ContentProviderBase
	{
		#region Const
		const string PICTURE_LIBRARYS_NAME = "Pictures";
		#endregion

		#region Var
		private IEnumerable<ContentType> _supportTypes;
		#endregion


		#region Public Property
		/// <summary>
		/// Gets the support types.
		/// </summary>
		/// <value>The support types.</value>
		public override IEnumerable<ContentType> SupportTypes
		{
			get { return _supportTypes ?? (_supportTypes = new ContentType[] { ContentType.Photo }); }
		} 
		#endregion


		#region Private Method
		/// <summary>
		/// Determines whether [is win vista or later].
		/// </summary>
		/// <returns>
		/// 	<c>true</c> if [is win vista or later]; otherwise, <c>false</c>.
		/// </returns>
		private static bool isWinVistaOrLater()
		{
			bool isWinVistaOrLater;

			var os = Environment.OSVersion;
			if (os.Platform == PlatformID.Win32NT && os.Version.Major == 6)
				isWinVistaOrLater = true;
			else
				isWinVistaOrLater = false;
			return isWinVistaOrLater;
		}

		/// <summary>
		/// Gets the librarys files.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<string> GetLibrarysFiles()
		{
			using (ShellLibrary library = ShellLibrary.Load(PICTURE_LIBRARYS_NAME, false))
			{
				foreach (ShellFolder folder in library)
				{
					var folderPath = folder.ParsingName;
					foreach(var file in EnumerateFiles(folderPath))
						yield return file;
				}
			}
		}

		/// <summary>
		/// Gets my pictres files.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<string> GetMyPictresFiles()
		{
			var path = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
			foreach (var file in EnumerateFiles(path))
				yield return file;
		}
		#endregion


		#region Public Method
		/// <summary>
		/// Gets the contents.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<IContent> GetContents()
		{
			var files = isWinVistaOrLater() ? GetLibrarysFiles() : GetMyPictresFiles();
			return from file in files
				   let extension = Path.GetExtension(file)
				   where extension.Equals(".jpg", StringComparison.CurrentCultureIgnoreCase) ||
				   extension.Equals(".png", StringComparison.CurrentCultureIgnoreCase) ||
				   extension.Equals(".bmp", StringComparison.CurrentCultureIgnoreCase)
				   select (new Content(file, ContentType.Photo) as IContent);
		} 
		#endregion
	}
}
