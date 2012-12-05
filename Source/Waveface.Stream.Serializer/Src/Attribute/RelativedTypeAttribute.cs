using System;

namespace Waveface.Stream.Serializer
{
	/// <summary>
	/// 
	/// </summary>
	public class RelativedTypeAttribute : Attribute
	{
		#region "Property"

		/// <summary>
		/// Gets or sets the type.
		/// </summary>
		/// <value>The type.</value>
		public Type Type { get; private set; }

		#endregion

		#region "Constructor"

		/// <summary>
		/// Initializes a new instance of the <see cref="RelativedTypeAttribute" /> class.
		/// </summary>
		/// <param name="type">The type.</param>
		public RelativedTypeAttribute(Type type)
		{
			Type = type;
		}

		#endregion
	}
}