using System;
using System.Collections.Generic;

namespace Waveface.Stream.Model
{
	public class PostGps
	{
		public double? latitude { get; set; }

		public int? zoom_level { get; set; }

		public string name { get; set; }

		public double? longitude { get; set; }

		public List<String> region_tags { get; set; }
	}
}
