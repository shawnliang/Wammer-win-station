using System;
using MongoDB.Bson;
using MongoDB.Driver;
using StreamDB = Waveface.Stream.Model.Database;
using MongoDB.Bson.Serialization;

namespace Waveface.Stream.Model
{
	public class DBCollection<T> : MongoCollection<T>
	{
		#region Event
		public event EventHandler<CollectionChangedEventArgs> Saved;
		public event EventHandler<CollectionChangedEventArgs> Updated;
		#endregion

		#region Constructor
		protected DBCollection(string collectionName)
			: base(StreamDB.Wammer, new MongoCollectionSettings<T>(StreamDB.Wammer, collectionName))
		{
		}
		#endregion


		#region Protected Method
		/// <summary>
		/// Raises the <see cref="E:Saved" /> event.
		/// </summary>
		/// <param name="e">The <see cref="CollectionChangedEventArgs" /> instance containing the event data.</param>
		protected void OnSaved(CollectionChangedEventArgs e)
		{
			var handler = Saved;
			if (handler == null)
				return;
			handler(this, e);
		}

		/// <summary>
		/// Raises the <see cref="E:Updated" /> event.
		/// </summary>
		/// <param name="e">The <see cref="CollectionChangedEventArgs" /> instance containing the event data.</param>
		protected void OnUpdated(CollectionChangedEventArgs e)
		{
			var handler = Updated;
			if (handler == null)
				return;
			handler(this, e);
		}
		#endregion


		#region Public Method
		/// <summary>
		/// Saves the specified document.
		/// </summary>
		/// <param name="document">The document.</param>
		/// <returns></returns>
		public override SafeModeResult Save(T document)
		{
			var bs = document.ToBsonDocument();
			var id = bs["_id"].ToString();
			var obj = FindOneById(id);

			var ret = base.Save(document);

			if (obj == null)
			{
				OnSaved(new CollectionChangedEventArgs(id));
			}
			else
			{
				OnUpdated(new CollectionChangedEventArgs(id));
			}
			return ret;
		}

		/// <summary>
		/// Updates one or more matching documents in this collection (for multiple updates use UpdateFlags.Multi).
		/// </summary>
		/// <param name="query">The query (usually a QueryDocument or constructed using the Query builder).</param>
		/// <param name="update">The update to perform on the matching document.</param>
		/// <param name="flags">The flags for this Update.</param>
		/// <returns>
		/// A SafeModeResult (or null if SafeMode is not being used).
		/// </returns>
		public override SafeModeResult Update(IMongoQuery query, IMongoUpdate update, UpdateFlags flags)
		{
			var bs = query.ToBsonDocument();

			if (bs == null || !bs.Contains("_id"))
			{
				return base.Update(query, update, flags);
			}

			var id = bs["_id"].ToString();

			var ret = base.Update(query, update, flags);

			if (ret.UpdatedExisting)
			{
				OnUpdated(new CollectionChangedEventArgs(id)); 
			}
			else
			{
				OnSaved(new CollectionChangedEventArgs(id));
			}
			return ret;
		}
		#endregion
	}
}
