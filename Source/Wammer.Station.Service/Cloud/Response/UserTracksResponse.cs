using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using Waveface.Stream.Model;

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
}