using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;

namespace Waveface.Stream.Model
{
	[BsonIgnoreExtraElements]
	public class Preview
	{
		[BsonIgnoreIfNull]
		public string description { get; set; }

		[BsonIgnoreIfNull]
		public string title { get; set; }

		[BsonIgnoreIfNull]
		public string url { get; set; }

		[BsonIgnoreIfNull]
		public string provider_display { get; set; }

		[BsonIgnoreIfNull]
		public string favicon_url { get; set; }

		[BsonIgnoreIfNull]
		public string thumbnail_url { get; set; }

		[BsonIgnoreIfNull]
		public string type { get; set; }

		[BsonIgnoreIfNull]
		public List<Image> images { get; set; }

		#region inner classes
		[BsonIgnoreExtraElements]
		public class Image
		{
			[BsonIgnoreIfNull]
			public string url;
			[BsonIgnoreIfNull]
			public string type;
		}
		#endregion

	}
}
