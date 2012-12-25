using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Waveface.Stream.Model
{
	public class PostGps
	{
		public float? latitude { get; set; }

		public int? zoom_level { get; set; }

		public string name { get; set; }

		public float? longitude { get; set; }

		public List<String> region_tags { get; set; }
	}
}
