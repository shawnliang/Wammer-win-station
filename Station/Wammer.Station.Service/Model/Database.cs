using System;
using MongoDB.Bson;
using MongoDB.Driver;
using Wammer.Station;
using log4net;

namespace Wammer.Model
{
	public class Database
	{
		#region Const
		private const string MONGODB_URL = "mongodb://127.0.0.1:{0}/?safe=true;connectTimeoutMS=10000";
		private const int DEFAULT_MONGODB_PORT = 10319;
		#endregion

		#region Static Var
		private static readonly ILog logger = LogManager.GetLogger(typeof(Database));
		private static MongoServer _mongoDB;
		private static MongoDatabase _wammer;
		#endregion


		#region Private Static Property
		/// <summary>
		/// Gets the m_ mongo DB.
		/// </summary>
		/// <value>The m_ mongo DB.</value>
		private static MongoServer m_MongoDB
		{
			get
			{
				return _mongoDB ?? (_mongoDB = MongoServer.Create(
					string.Format(MONGODB_URL,
					StationRegistry.GetValue("dbPort", DEFAULT_MONGODB_PORT))));
			}
		}
		#endregion


		#region Public Static Property
		/// <summary>
		/// Gets the m_ wammer.
		/// </summary>
		/// <value>The m_ wammer.</value>
		public static MongoDatabase Wammer
		{
			get { return _wammer ?? (_wammer = m_MongoDB.GetDatabase("wammer")); }
		}
		#endregion


		#region Public Static Method
		public static Boolean TestConnection(int retryTimes)
		{
			while (retryTimes > 0)
			{
				try
				{
					var mongourl = string.Format(MONGODB_URL, StationRegistry.GetValue("dbPort", DEFAULT_MONGODB_PORT));
					var server = MongoServer.Create(mongourl);
					server.Connect(TimeSpan.FromSeconds(2));
					server.Disconnect();
					return true;
				}
				catch (Exception e)
				{
					logger.WarnFormat("Unable to connect to mongodb server, retry={0}, exception={1}", retryTimes--, e.Message);
				}
			}
			return false;
		}
		#endregion

		public static void RestoreCollection(string collectionName, string backupCollectionName)
		{
			MongoCollection<BsonDocument> collection = Wammer.GetCollection(collectionName);
			MongoCollection<BsonDocument> backup = Wammer.GetCollection(backupCollectionName);

			foreach (BsonDocument doc in backup.FindAll())
				collection.Save(doc);

			backup.RemoveAll();
		}
	}


	public class Collection<T>
	{
		protected MongoCollection<T> collection { get; set; }

		protected Collection(string collectionName)
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