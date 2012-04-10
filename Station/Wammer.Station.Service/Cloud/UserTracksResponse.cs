using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;

namespace Wammer.Cloud
{
	[BsonIgnoreExtraElements]
	public class UserTrackAction
	{
		[BsonIgnoreIfNull]
		public string action { get; set; }
		[BsonIgnoreIfNull]
		public string target_type { get; set; }
		[BsonIgnoreIfNull]
		public List<string> target_id_list { get; set; }
	}

	[BsonIgnoreExtraElements]
	public class UserTrackDetail
	{
		[BsonIgnoreIfNull]
		public string group_id { get; set; }
		[BsonIgnoreIfNull]
		public string user_id { get; set; }
		[BsonIgnoreIfNull]
		public DateTime timestamp { get; set; }
		[BsonIgnoreIfNull]
		public string target_id { get; set; }
		[BsonIgnoreIfNull]
		public string target_type { get; set; }
		[BsonIgnoreIfNull]
		public List<UserTrackAction> actions { get; set; }
	}

	public class UserTrackResponse: CloudResponse
	{
		public int get_count { get; set; }
		public List<string> post_id_list { get; set; }
		public string group_id { get; set; }
		public string latest_timestamp { get; set; }
		public int remaining_count { get; set; }
		public List<UserTrackDetail> usertrack_list { get; set; }

		public UserTrackResponse()
			:base()
		{
		}
	}
}
