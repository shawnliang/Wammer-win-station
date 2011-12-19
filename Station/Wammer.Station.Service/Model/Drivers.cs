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

	public class OldDriverCollection : Collection<Driver>
	{
		private static OldDriverCollection instance;

		static OldDriverCollection()
		{
			instance = new OldDriverCollection();
		}

		private OldDriverCollection()
			: base("oldDrivers")
		{
		}

		public static OldDriverCollection Instance
		{
			get { return instance; }
		}
	}
}
