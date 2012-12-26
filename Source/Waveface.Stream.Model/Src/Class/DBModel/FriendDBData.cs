using MongoDB.Bson.Serialization.Attributes;

namespace Waveface.Stream.Model
{
	[BsonIgnoreExtraElements]
	public class FriendDBData
	{
		[BsonId]
		public string ID { get; set; }

		[BsonIgnoreIfNull]
		[BsonElement("name")]
		public string Name { get; set; }

		[BsonIgnoreIfNull]
		[BsonElement("avatar")]
		public string Avatar { get; set; }
	}
}
