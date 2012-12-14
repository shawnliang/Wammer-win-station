using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Waveface.Stream.Model
{
	[BsonIgnoreExtraElements]
	public class PostCheckIn
	{
		[BsonIgnoreIfNull]
		public string name { get; set; }

		[BsonIgnoreIfNull]
		public float? latitude { get; set; }

		[BsonIgnoreIfNull]
		public float? longitude { get; set; }
	}
}
