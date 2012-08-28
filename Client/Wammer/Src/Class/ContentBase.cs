using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Waveface
{
	/// <summary>
	/// 
	/// </summary>
	public abstract class ContentBase : IContent
	{
		#region Var
		private string _fileName;
		private string _path;
		private string _filePath;
		#endregion

		#region Property
		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>The name.</value>
		public string FileName 
		{
			get 
			{
				return _fileName ?? (_fileName = System.IO.Path.GetFileName(FilePath));
			}
		}

		/// <summary>
		/// Gets the file path.
		/// </summary>
		/// <value>The file path.</value>
		public string FilePath 
		{
			get
			{
				return _filePath ?? string.Empty;
			}
			protected set
			{
				_filePath = value;
			}
		}

		/// <summary>
		/// Gets the path.
		/// </summary>
		/// <value>The path.</value>
		public string Path
		{
			get
			{
				return _path ?? (_path = System.IO.Path.GetDirectoryName(FilePath));
			}
		}

		/// <summary>
		/// Gets the type.
		/// </summary>
		/// <value>The type.</value>
		public ContentType Type { get; protected set; }
		#endregion


		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="ContentBase"/> class.
		/// </summary>
		public ContentBase()
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ContentBase"/> class.
		/// </summary>
		/// <param name="filePath">The file path.</param>
		/// <param name="type">The type.</param>
		public ContentBase(string filePath, ContentType type)
		{
			this.FilePath = filePath;
			this.Type = type;
		}
		#endregion
	}
}
