namespace Wammer.Model
{
	/// <summary>
	/// 
	/// </summary>
	public class CollectionCollection : Collection<Collection>
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


		public override MongoDB.Driver.SafeModeResult Save(Collection doc)
		{
			var collectionID = doc.collection_id;
			var collection = FindOneById(collectionID);

			var ret = base.Save(doc);

			if (collection == null)
			{
				Waveface.Stream.Core.SystemEventSubscriber.Instance.TriggerCollectionAddedEvent(collectionID);
			}
			else
			{
				Waveface.Stream.Core.SystemEventSubscriber.Instance.TriggerCollectionUpdatedEvent(collectionID);
			}

			return ret;
		}
	}
}