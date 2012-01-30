using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Wammer.Station;
using MongoDB.Driver;
using MongoDB.Bson;

namespace Wammer.Model
{
	public class Database
	{
		public static MongoDB.Driver.MongoServer mongodb;
		public static MongoDB.Driver.MongoDatabase wammer;

		static Database()
		{
			mongodb = MongoDB.Driver.MongoServer.Create(
									string.Format("mongodb://127.0.0.1:{0}/?safe=true;connectTimeoutMS=10000",
									StationRegistry.GetValue("dbPort", 10319))); // TODO: Remove Hard code
			wammer = mongodb.GetDatabase("wammer");
		}


		public static void RestoreCollection(string collectionName, string backupCollectionName)
		{
			MongoCollection<BsonDocument> collection = wammer.GetCollection(collectionName);
			MongoCollection<BsonDocument> backup =  wammer.GetCollection(backupCollectionName);

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

		public void Save(T driver)
		{
			collection.Save(driver);
		}
		public void Save(BsonDocument driver)
		{
			collection.Save(driver);
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
	}
}
