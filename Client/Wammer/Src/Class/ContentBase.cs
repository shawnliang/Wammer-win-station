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
		private string _name;
		private string _filePath;
		#endregion

		#region Property
		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name 
		{
			get 
			{
				return _name ?? (_name = Path.GetFileName(FilePath));
			}
			protected set
			{
				_name = value;
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
		/// <param name="name">The name.</param>
		/// <param name="type">The type.</param>
		public ContentBase(string filePath, string name, ContentType type)
		{
			this.Name = name;
			this.FilePath = filePath;
			this.Type = type;
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
