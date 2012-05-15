using System;
using MongoDB.Bson;
using MongoDB.Driver;
using Wammer.Station;
using log4net;

namespace Wammer.Model
{
	public class Database
	{
		public static MongoServer mongodb;
		public static MongoDatabase wammer;
		private static readonly ILog logger = LogManager.GetLogger(typeof(Database));

		static Database()
		{
			mongodb = MongoServer.Create(
				string.Format("mongodb://127.0.0.1:{0}/?safe=true;connectTimeoutMS=10000",
							  StationRegistry.GetValue("dbPort", 10319))); // TODO: Remove Hard code
			wammer = mongodb.GetDatabase("wammer");

			int retry = 5;
			while (retry > 0)
			{
				try
				{
					mongodb.Connect(TimeSpan.FromSeconds(1));
					break;
				}
				catch (MongoConnectionException e)
				{
					logger.WarnFormat("Unable to connect to mongodb server, retry={0}, exception={1}", retry, e.Message);
					retry--;
				}
			}
			if (retry == 0)
				throw new InvalidOperationException("Unable to connect to mongodb server");
		}


		public static void RestoreCollection(string collectionName, string backupCollectionName)
		{
			MongoCollection<BsonDocument> collection = wammer.GetCollection(collectionName);
			MongoCollection<BsonDocument> backup = wammer.GetCollection(backupCollectionName);

			foreach (BsonDocument doc in backup.FindAll())
				collection.Save(doc);

			backup.RemoveAll();
		}
	}


	public class Collection<T>
	{
		protected MongoCollection<T> collection;

		protected Collection(string collectionName)
		{
			collection = Database.wammer.GetCollection<T>(collectionName);
		}

		public T FindOne(IMongoQuery query)
		{
			return collection.FindOne(query);
		}

		public T FindOne()
		{
			return collection.FindOne();
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

		public void Remove(IMongoQuery query)
		{
			collection.Remove(query);
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