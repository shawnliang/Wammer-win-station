using System;

namespace Waveface.Stream.WindowsClient
{
	/// <summary>
	/// 
	/// </summary>
	public interface IContent
	{
		#region Property
		/// <summary>
		/// Gets the name of the file.
		/// </summary>
		/// <value>The name of the file.</value>
		String FileName { get; }

		/// <summary>
		/// Gets the file path.
		/// </summary>
		/// <value>The file path.</value>
		String FilePath { get; }

		/// <summary>
		/// Gets the path.
		/// </summary>
		/// <value>The path.</value>
		String Path { get; }

		/// <summary>
		/// Gets the type.
		/// </summary>
		/// <value>The type.</value>
		ContentType Type { get; }
		#endregion
	}
}
