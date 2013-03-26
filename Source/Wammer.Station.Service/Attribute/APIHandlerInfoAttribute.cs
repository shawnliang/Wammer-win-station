using System;

namespace Wammer
{
	public sealed class APIHandlerInfoAttribute : Attribute
	{
		#region Property
		/// <summary>
		/// Gets or sets the type.
		/// </summary>
		/// <value>The type.</value>
		public APIHandlerType Type { get; private set; }

		/// <summary>
		/// Gets or sets the path.
		/// </summary>
		/// <value>The API path.</value>
		public string Path { get; private set; }
		#endregion


		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="APIHandlerInfoAttribute"/> class.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="path">The path.</param>
		public APIHandlerInfoAttribute(APIHandlerType type, string path)
		{
			Type = type;
			Path = path;
		}
		#endregion
	}
}
