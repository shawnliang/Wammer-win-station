using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.WindowsAPICodePack.Shell;

namespace Waveface.Stream.WindowsClient
{
	/// <summary>
	/// 
	/// </summary>
	public class LibrariesContentProvider : ContentProviderBase
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

		/// <summary>
		/// Gets the type.
		/// </summary>
		/// <value>The type.</value>
		public override ContentProviderType Type
		{
			get { return ContentProviderType.Libraries; }
		}
		#endregion


		#region Private Method
        private bool IsWinVistaOrLater()
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
			var files = IsWinVistaOrLater() ? GetLibrarysFiles() : GetMyPictresFiles();
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
