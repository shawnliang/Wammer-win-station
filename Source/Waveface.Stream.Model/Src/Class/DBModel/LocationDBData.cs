using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Waveface.Stream.Model
{
	[BsonIgnoreExtraElements]
	public class LocationDBData
	{
		#region Public Property
		[BsonId]
		public string ID { get; set; }

		[BsonIgnoreIfNull]
		[BsonElement("name")]
		public string Name { get; set; }

		[BsonIgnoreIfNull]
		[BsonElement("latitude")]
		public float? Latitude { get; set; }

		[BsonIgnoreIfNull]
		[BsonElement("longitude")]
		public float? Longitude { get; set; }

		[BsonIgnoreIfNull]
		[BsonElement("zoom_level")]
		public int? ZoomLevel { get; set; }

		[BsonIgnoreIfNull]
		[BsonElement("tags")]
		public IEnumerable<String> Tags { get; set; } 
		#endregion


		#region Public Method
		/// <summary>
		/// Shoulds the serialize tags.
		/// </summary>
		/// <returns></returns>
		public bool ShouldSerializeTags()
		{
			return Tags != null && Tags.Any();
		}
		#endregion
	}
}
