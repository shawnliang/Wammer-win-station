using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

using Wammer.Cloud;
using Wammer.Station;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Attributes;

namespace Wammer.Model
{
	[BsonIgnoreExtraElements]
	public class Drivers
	{
		[BsonIgnore]
		public static MongoCollection<Drivers> collection = Database.wammer.GetCollection<Drivers>("drivers");

		[BsonId]
		public string user_id { get; set; }
		public string email { get; set; }
		public string folder { get; set; }
		public List<UserGroup> groups { get; set; }
		public string session_token { get; set; }

		public Drivers()
		{
			groups = new List<UserGroup>();
		}
	}
}
