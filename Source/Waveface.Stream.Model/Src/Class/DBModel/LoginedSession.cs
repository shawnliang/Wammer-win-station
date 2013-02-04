using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace Waveface.Stream.Model
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
		public long? files_to_backup { get; set; }

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
	public class Apikey
	{
		public string apikey { get; set; }
		public string name { get; set; }
	}

	[BsonIgnoreExtraElements]
	public class LoginedUserInfo
	{
		public string user_id { get; set; }
		public List<Device> devices { get; set; }
		public string state { get; set; }
		public string avatar_url { get; set; }
		public bool verified { get; set; }
		public string nickname { get; set; }
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
		public string group_id { get; set; }
		public string description { get; set; }
		public string station_id { get; set; }
		public string creator_id { get; set; }
		public string name { get; set; }
	}

	[BsonIgnoreExtraElements]
	public class Station
	{
		public string status { get; set; }
		public int timestamp { get; set; }
		public string station_id { get; set; }
		public string creator_id { get; set; }
		public string location { get; set; }
		public int last_seen { get; set; }
	}
}