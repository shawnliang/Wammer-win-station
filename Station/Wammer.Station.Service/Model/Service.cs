using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace Wammer.Model
{
	public class Service
	{
		[BsonId]
		public string Id { get; set; }
		[BsonIgnoreIfNull]
		public string OfflineKey { get; set; }
	}


	public static class ServiceCollection
	{
		private static MongoCollection<Service> collection =
			Database.wammer.GetCollection<Service>("service");

		public static Service FindOne(IMongoQuery query)
		{
			return collection.FindOne(query);
		}

		public static void Save(Service svc)
		{
			collection.Save(svc);
		}
	}
}
