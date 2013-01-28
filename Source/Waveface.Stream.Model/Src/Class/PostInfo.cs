using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Waveface.Stream.Model
{
	[BsonIgnoreExtraElements]
	public class PostInfo
	{
		private string _cover_attach;

		[BsonIgnoreIfNull]
		public List<AttachmentInfo> attachments { get; set; }

		[BsonIgnoreIfNull]
		public DateTime event_start_time { get; set; }

		[BsonIgnoreIfNull]
		public DateTime event_end_time { get; set; }

		[BsonIgnoreIfNull]
		public string content { get; set; }

		[BsonIgnoreIfNull]
		public string content_auto { get; set; }

		[BsonIgnoreIfNull]
		public bool hidden { get; set; }

		[BsonIgnoreIfNull]
		public Preview preview { get; set; }

		[BsonIgnoreIfNull]
		public string type { get; set; }

		[BsonIgnoreIfNull]
		public string event_type { get; set; }

		[BsonIgnoreIfNull]
		public DateTime update_time { get; set; }


		/// <summary>
		/// post create time
		/// </summary>
		[BsonIgnoreIfNull]
		public DateTime timestamp { get; set; }

		[BsonId]
		public string post_id { get; set; }

		[BsonIgnoreIfNull]
		public string code_name { get; set; }

		[BsonIgnoreIfNull]
		public List<string> attachment_id_array { get; set; }

		[BsonIgnoreIfNull]
		public string device_id { get; set; }

		[BsonIgnoreIfNull]
		public string group_id { get; set; }

		[BsonIgnoreIfNull]
		public int favorite { get; set; }

		[BsonIgnoreIfNull]
		public string soul { get; set; }

		[BsonIgnoreIfNull]
		public string creator_id { get; set; }

		[BsonIgnoreIfNull]
		public string cover_attach 
		{
			get 
			{
				return (string.IsNullOrEmpty(_cover_attach) && attachment_id_array != null) ? attachment_id_array.FirstOrDefault() : _cover_attach; 
			}
			set
			{
				_cover_attach = value;
			}
		}

		[BsonIgnoreIfNull]
		public int seq_num { get; set; }

		[BsonIgnoreIfNull]
		public List<PostCheckIn> checkins { get; set; }

		[BsonIgnoreIfNull]
		public List<FriendInfo> people { get; set; }

		[BsonIgnoreIfNull]
		public List<ExtraParameter> extra_parameters { get; set; }

		[BsonIgnoreIfNull]
		public PostGps gps { get; set; }

		[BsonIgnoreIfNull]
		public List<string> tags { get; set; }
	}
}
