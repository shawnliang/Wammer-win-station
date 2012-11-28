using MongoDB.Bson.Serialization.Attributes;

namespace Wammer.Model
{
	[BsonIgnoreExtraElements]
	public class GenericData
	{
		[BsonId]
		public string Id { get; set; }

		[BsonIgnoreIfNull]
		public byte[] Data { get; set; }
	}
}