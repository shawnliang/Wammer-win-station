using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace Wammer.Model
{
	[BsonIgnoreExtraElements]
	public class StationInfo
	{	
		[BsonId]
		public string Id { get; set; }
		[BsonIgnoreIfNull]
		public string SessionToken { get; set; }
		[BsonIgnoreIfNull]
		public DateTime LastLogOn { get; set; }
		[BsonIgnoreIfNull]
		public string Location { get; set; }
	}

	public class StationCollection
	{
		private static MongoCollection<StationInfo> collection = Database.wammer.GetCollection<StationInfo>("station");

		public static void RemoveAll()
		{
			collection.RemoveAll();
		}

		public static StationInfo FindOne()
		{
			return collection.FindOne();
		}

		public static StationInfo FindOne(IMongoQuery query)
		{
			return collection.FindOne(query);
		}

		public static void Save(StationInfo station)
		{
			collection.Save(station);
		}
	}
}
