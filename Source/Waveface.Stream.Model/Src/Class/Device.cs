
using MongoDB.Bson.Serialization.Attributes;
namespace Waveface.Stream.Model
{
	[BsonIgnoreExtraElements]
	public class Device
	{
		[BsonIgnoreIfNull]
		public string device_name { get; set; }
		[BsonIgnoreIfNull]
		public string device_id { get; set; }
	}
}
