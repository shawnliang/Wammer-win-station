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
			return (new DirectoryInfo(path)).EnumerateFiles();
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
