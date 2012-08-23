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
		private IEnumerable<IContent> GetLibrarysContents()
		{
			using (ShellLibrary library = ShellLibrary.Load(PICTURE_LIBRARYS_NAME, false))
			{
				foreach (ShellFolder folder in library)
				{
					Console.WriteLine(folder);
				}
			}
		}
		#endregion


		#region Public Method
		/// <summary>
		/// Gets the contents.
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<IContent> GetContents()
		{
		} 
		#endregion
	}
}
