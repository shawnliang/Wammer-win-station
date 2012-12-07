using MongoDB.Bson.Serialization.Attributes;

namespace Waveface.Stream.Model
{
	[BsonIgnoreExtraElements]
	public class Comment
	{
		[BsonIgnoreIfNull]
		public string content { get; set; }

		[BsonIgnoreIfNull]
		public string timestamp { get; set; }

		[BsonIgnoreIfNull]
		public string creator_id { get; set; }

		[BsonIgnoreIfNull]
		public string code_name { get; set; }

		[BsonIgnoreIfNull]
		public string device_id { get; set; }
	}
}
