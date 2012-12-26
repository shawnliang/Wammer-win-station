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

		[BsonRequired]
		[BsonElement("creator_id")]
		public string CreatorID { get; set; }

		[BsonIgnoreIfNull]
		[BsonElement("name")]
		public string Name { get; set; }

		[BsonRequired]
		[BsonElement("latitude")]
		public double? Latitude { get; set; }

		[BsonRequired]
		[BsonElement("longitude")]
		public double? Longitude { get; set; }

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
