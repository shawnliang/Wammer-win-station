﻿namespace Waveface.Stream.Model
{
	/// <summary>
	/// 
	/// </summary>
	public class CollectionCollection : DBCollection<Collection>
	{
		#region Static Var

		private static CollectionCollection instance;

		#endregion

		#region Constructor
		/// <summary>
		/// Prevents a default instance of the <see cref="CollectionCollection" /> class from being created.
		/// </summary>
		private CollectionCollection()
			: base("collections")
		{
		}

		#endregion

		#region Public Static Method

		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static CollectionCollection Instance
		{
			get { return instance ?? (instance = new CollectionCollection()); }
		}

		#endregion
	}
}