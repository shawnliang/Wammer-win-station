using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Wammer.Model
{
	/// <summary>
	/// 
	/// </summary>
	[BsonIgnoreExtraElements]
	public class LoginedSession
	{
		#region Public Property

		[BsonIgnoreIfNull]
		public int status { get; set; }

		[BsonId]
		public string session_token { get; set; }

		[BsonIgnoreIfNull]
		public string session_expires { get; set; }

		[BsonIgnoreIfNull]
		public string timestamp { get; set; }

		[BsonIgnoreIfNull]
		public int api_ret_code { get; set; }

		[BsonIgnoreIfNull]
		public string api_ret_message { get; set; }

		[BsonIgnoreIfNull]
		public Device device { get; set; }

		[BsonIgnoreIfNull]
		public Apikey apikey { get; set; }

		[BsonIgnoreIfNull]
		public LoginedUserInfo user { get; set; }

		[BsonIgnoreIfNull]
		public List<Group> groups { get; set; }

		[BsonIgnoreIfNull]
		public List<Station> stations { get; set; }

		#endregion
	}

	[BsonIgnoreExtraElements]
	public class Device
	{
		[BsonIgnoreIfNull]
		public string device_name { get; set; }
		[BsonIgnoreIfNull]
		public string device_id { get; set; }
		[BsonIgnoreIfNull]
		public string device_type { get; set; }

		// HACK - don't serialize this field to mongo db because 
		//        WavefaceWindowsClient cannot deserialize mongo time (ISODate("....."))
		[BsonIgnore]
		public DateTime last_visit { get; set; }
	}

	[BsonIgnoreExtraElements]
	public class Apikey
	{
		[BsonIgnoreIfNull]
		public string apikey { get; set; }
		[BsonIgnoreIfNull]
		public string name { get; set; }
	}

	[BsonIgnoreExtraElements]
	public class LoginedUserInfo
	{
		[BsonIgnoreIfNull]
		public string user_id { get; set; }
		[BsonIgnoreIfNull]
		public List<Device> devices { get; set; }
		[BsonIgnoreIfNull]
		public string state { get; set; }
		[BsonIgnoreIfNull]
		public string avatar_url { get; set; }
		[BsonIgnoreIfNull]
		public bool verified { get; set; }
		[BsonIgnoreIfNull]
		public string nickname { get; set; }
		[BsonIgnoreIfNull]
		public string email { get; set; }
		[BsonIgnoreIfNull]
		public List<SNS> sns { get; set; }
	}

	[BsonIgnoreExtraElements]
	public class SNS
	{
		[BsonIgnoreIfNull]
		public bool enabled { get; set; }
		[BsonIgnoreIfNull]
		public string snsid { get; set; }
		[BsonIgnoreIfNull]
		public List<string> status { get; set; }
		[BsonIgnoreIfNull]
		public string type { get; set; }
		[BsonIgnoreIfNull]
		public string lastSync { get; set; }
		[BsonIgnoreIfNull]
		public string toDate { get; set; }
	}

	[BsonIgnoreExtraElements]
	public class Group
	{
		[BsonIgnoreIfNull]
		public string group_id { get; set; }
		[BsonIgnoreIfNull]
		public string description { get; set; }
		[BsonIgnoreIfNull]
		public string station_id { get; set; }
		[BsonIgnoreIfNull]
		public string creator_id { get; set; }
		[BsonIgnoreIfNull]
		public string name { get; set; }
	}

	[BsonIgnoreExtraElements]
	public class Station
	{
		[BsonIgnoreIfNull]
		public string status { get; set; }
		[BsonIgnoreIfNull]
		public int timestamp { get; set; }
		[BsonIgnoreIfNull]
		public string station_id { get; set; }
		[BsonIgnoreIfNull]
		public string creator_id { get; set; }
		[BsonIgnoreIfNull]
		public string location { get; set; }
		[BsonIgnoreIfNull]
		public int last_seen { get; set; }
	}
}