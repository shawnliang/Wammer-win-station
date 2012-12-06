using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Wammer.Model
{
	[BsonIgnoreExtraElements]
	public class Collection
	{
		[BsonIgnoreIfNull]
		public string name { get; set; }

		[BsonIgnoreIfNull]
		public int seq_num { get; set; }

		[BsonIgnoreIfNull]
		public List<String> attachment_id_array { get; set; }

		[BsonIgnoreIfNull]
		public string creator_id { get; set; }

		[BsonIgnoreIfNull]
		public string create_time { get; set; }

		[BsonIgnoreIfNull]
		public string modify_time { get; set; }

		[BsonId]
		public string collection_id { get; set; }

		[BsonIgnoreIfNull]
		public String cover { get; set; }

		[BsonIgnoreIfNull]
		public bool hidden { get; set; }

		[BsonIgnoreIfNull]
		public bool smart { get; set; }

		[BsonIgnoreIfNull]
		public bool manual { get; set; }
	}
}