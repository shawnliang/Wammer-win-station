using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace Waveface.Stream.Model
{
	[BsonIgnoreExtraElements]
	public class ExtraParameter
	{
		[BsonIgnoreIfNull]
		public string name { get; set; }

		[BsonIgnoreIfNull]
		public List<string> values { get; set; }
	}
}
