using Newtonsoft.Json;
using System.Collections.Generic;

namespace Waveface.Stream.Core
{
	public class GPSInfoData
	{
		[JsonProperty("gps_latituderef", NullValueHandling = NullValueHandling.Ignore)]
		public string GPSLatitudeRef { get; set; }

		[JsonProperty("gps_latitude", NullValueHandling = NullValueHandling.Ignore)]
		public List<List<int>> GPSLatitude { get; set; }

		[JsonProperty("gps_longituderef", NullValueHandling = NullValueHandling.Ignore)]
		public string GPSLongitudeRef { get; set; }

		[JsonProperty("gps_longitude", NullValueHandling = NullValueHandling.Ignore)]
		public List<List<int>> GPSLongitude { get; set; }
	}
}
