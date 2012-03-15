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
	public class Driver
	{
		[BsonIgnore]
		public static MongoCollection<Driver> collection = Database.wammer.GetCollection<Driver>("drivers");

		[BsonId]
		public string user_id { get; set; }
		[BsonIgnoreIfNull]
		public string email { get; set; }
		[BsonIgnoreIfNull]
		public string folder { get; set; }
		[BsonIgnoreIfNull]
		public List<UserGroup> groups { get; set; }
		[BsonIgnoreIfNull]
		public string session_token { get; set; }
		
		[BsonDefaultValue(true)]
		[BsonIgnoreIfNull]
		public bool isPrimaryStation { get; set; }

		[BsonIgnoreIfNull]
		public SyncRange sync_range { get; set; }

		public Driver()
		{
			groups = new List<UserGroup>();
		}

	}

	public class DriverCollection : Collection<Driver>
	{
		private static DriverCollection instance;

		static DriverCollection()
		{
			instance = new DriverCollection();
		}

		private DriverCollection()
			:base("drivers")
		{
		}

		public static DriverCollection Instance
		{
			get { return instance; }
		}
	}

	public class SyncRange
	{
		public string start_time { get; set; }
		public string end_time { get; set; }
		public string first_post_time { get; set; }
	}
}
