using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Waveface
{
	/// <summary>
	/// 
	/// </summary>
	public interface IContentProvider
	{
		#region Property
		/// <summary>
		/// Gets the support types.
		/// </summary>
		/// <value>The support types.</value>
		IEnumerable<ContentType> SupportTypes { get; }
		#endregion


		#region Method
		/// <summary>
		/// Determines whether the specified type is support.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns>
		/// 	<c>true</c> if the specified type is support; otherwise, <c>false</c>.
		/// </returns>
		Boolean IsSupport(ContentType type);

		/// <summary>
		/// Gets the contents.
		/// </summary>
		/// <returns></returns>
		IEnumerable<IContent> GetContents();
		#endregion
	}
}
