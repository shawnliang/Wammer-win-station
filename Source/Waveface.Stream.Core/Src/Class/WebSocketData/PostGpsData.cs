using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Waveface.Stream.Core
{
	public class PostGpsData
	{
		#region Public Property
		[JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
		public String Name { get; set; }

		[JsonProperty("latitude", NullValueHandling = NullValueHandling.Ignore)]
		public double? Latitude { get; set; }

		[JsonProperty("longitude", NullValueHandling = NullValueHandling.Ignore)]
		public double? Longitude { get; set; }

		[JsonProperty("zoom_level", NullValueHandling = NullValueHandling.Ignore)]
		public int? ZoomLevel { get; set; }

		[JsonProperty("map", NullValueHandling = NullValueHandling.Ignore)]
		public string Map { get; set; }

		[JsonProperty("region_tags", NullValueHandling = NullValueHandling.Ignore)]
		public IEnumerable<String> RegionTags { get; set; }
		#endregion


		#region Public Method
		/// <summary>
		/// Shoulds the serialize region tags.
		/// </summary>
		/// <returns></returns>
		public bool ShouldSerializeRegionTags()
		{
			return RegionTags != null && RegionTags.Any();
		}
		#endregion
	}
}
