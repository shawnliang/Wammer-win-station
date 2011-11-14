using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace Wammer.Model
{
	public class StationInfo
	{
		public static MongoCollection<StationInfo> collection = Database.wammer.GetCollection<StationInfo>("station");

		[BsonId]
		public string Id { get; set; }
		public string SessionToken { get; set; }
		public DateTime LastLogOn { get; set; }
		public IPAddress Location { get; set; }
	}
}
