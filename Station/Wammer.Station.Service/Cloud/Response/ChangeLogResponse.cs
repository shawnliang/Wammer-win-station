using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace Wammer.Cloud
{
	public class ChangeLogResponse : CloudResponse
	{
		public string group_id { get; set; }
		public int get_count { get; set; }
		public DateTime latest_timestamp { get; set; }
		public int remaining_count { get; set; }
		public List<PostListItem> post_list { get; set; }
		public List<AttachmentListItem> attachment_list { get; set; }
		public int next_seq_num { get; set; }
		public List<UserTrackDetail> changelog_list { get; set; }
	}


	[BsonIgnoreExtraElements]
	public class PostListItem
	{
		[BsonIgnoreIfNull]
		public string post_id { get; set; }
		[BsonIgnoreIfNull]
		public DateTime update_time { get; set; }
		[BsonIgnoreIfNull]
		public int seq_num { get; set; }
	}

	[BsonIgnoreExtraElements]
	public class AttachmentListItem
	{
		[BsonIgnoreIfNull]
		public string attachment_id { get; set; }
		[BsonIgnoreIfNull]
		public DateTime update_time { get; set; }
		
		/// <summary>
		/// Always -1
		/// </summary>
		[BsonIgnoreIfNull]
		public int seq_num { get; set; }
	}
}