using MongoDB.Bson.Serialization.Attributes;

namespace Waveface.Stream.Model
{
	[BsonIgnoreExtraElements]
	public class PostGps
	{
		[BsonIgnoreIfNull]
		public float? latitude { get; set; }

		[BsonIgnoreIfNull]
		public int? zoom_level { get; set; }

		[BsonIgnoreIfNull]
		public string name { get; set; }

		[BsonIgnoreIfNull]
		public float? longitude { get; set; }
	}
}
