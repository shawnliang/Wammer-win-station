using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;

namespace Waveface.Stream.Model
{
	[BsonIgnoreExtraElements]
	public class PostInfo
	{
        //[BsonIgnoreIfNull]
        //public List<AttachmentInfo> attachments { get; set; }

		[BsonIgnoreIfNull]
		public int attachment_count { get; set; }

		[BsonIgnoreIfNull]
		public string event_time { get; set; }

		[BsonIgnoreIfNull]
		public List<Comment> comments { get; set; }

		[BsonIgnoreIfNull]
		public string content { get; set; }

		[BsonIgnoreIfNull]
		public int comment_count { get; set; }

		[BsonIgnoreIfNull]
		public string hidden { get; set; }

		[BsonIgnoreIfNull]
		public Preview preview { get; set; }

		[BsonIgnoreIfNull]
		public string type { get; set; }

		[BsonIgnoreIfNull]
		public List<string> style { get; set; }

		[BsonIgnoreIfNull]
		public DateTime update_time { get; set; }

		[BsonIgnoreIfNull]
		public Boolean isImported { get; set; }

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
		public string cover_attach { get; set; }

        //[BsonIgnoreIfNull]
        //public int seq_num { get; set; }


        [BsonIgnoreIfNull]
        public List<Person> people { get; set; }

        [BsonIgnoreIfNull]
        public List<ExtraParameter> extra_parameters { get; set; }

        [BsonIgnoreIfNull]
        public PostGps gps { get; set; }

        [BsonIgnoreIfNull]
        public List<string> tags { get; set; }
	}
}
