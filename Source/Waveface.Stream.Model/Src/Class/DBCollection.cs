﻿using MongoDB.Bson;
using MongoDB.Driver;

namespace Waveface.Stream.Model
{

	public class DBCollection<T>
	{
		protected MongoCollection<T> collection { get; set; }

		protected DBCollection(string collectionName)
		{
			collection = Database.Wammer.GetCollection<T>(collectionName);
		}

		public T FindOne(IMongoQuery query)
		{
			return collection.FindOne(query);
		}

		public T FindOne()
		{
			return collection.FindOne();
		}

		public T FindOneById(BsonValue id)
		{
			return collection.FindOneById(id);
		}

		public K FindOneAs<K>(IMongoQuery query)
		{
			return collection.FindOneAs<K>(query);
		}

		public void Save(T doc)
		{
			collection.Save(doc);
		}

		public void Save(BsonDocument doc)
		{
			collection.Save(doc);
		}

		public void RemoveAll()
		{
			collection.RemoveAll();
		}

		public SafeModeResult Remove(IMongoQuery query)
		{
			return collection.Remove(query);
		}

		public MongoCursor<T> FindAll()
		{
			return collection.FindAll();
		}

		public MongoCursor<T> Find(IMongoQuery query)
		{
			return collection.Find(query);
		}

		public void Update(IMongoQuery query, IMongoUpdate update)
		{
			collection.Update(query, update);
		}

		public SafeModeResult Update(IMongoQuery query, IMongoUpdate update, UpdateFlags updateFlags)
		{
			return collection.Update(query, update, updateFlags);
		}
	}
}