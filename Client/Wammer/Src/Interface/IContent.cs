using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Waveface
{
	/// <summary>
	/// 
	/// </summary>
	public interface IContent
	{
		#region Property
		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>The name.</value>
		String Name { get; }

		/// <summary>
		/// Gets the file path.
		/// </summary>
		/// <value>The file path.</value>
		String FilePath { get; }

		/// <summary>
		/// Gets the type.
		/// </summary>
		/// <value>The type.</value>
		ContentType Type { get; }
		#endregion
	}
}
