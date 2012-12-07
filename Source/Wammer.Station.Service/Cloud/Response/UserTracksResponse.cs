using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

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

		[BsonIgnoreIfNull]
		public string post_id { get; set; }
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

		[BsonIgnoreIfNull]
		public int seq_num { get; set; }
	}

	public class UserTrackResponse : CloudResponse
	{
		public int get_count { get; set; }
		public List<string> post_id_list { get; set; }
		public List<string> attachment_id_list { get; set; }
		public string group_id { get; set; }
		public DateTime latest_timestamp { get; set; }
		public int remaining_count { get; set; }
		public List<UserTrackDetail> usertrack_list { get; set; }
	}
}