using System;
using System.Xml.Serialization;

namespace Waveface.Stream.Model
{
	public class UserStation
	{
		public string status { get; set; }
		public string station_id { get; set; }
		public string creator_id { get; set; }
		public string location { get; set; }
		public long last_seen { get; set; }
		public string computer_name { get; set; }
		public string accessible { get; set; }
		public string public_location { get; set; }
		public bool upnp { get; set; }
		public string type { get; set; }

		[XmlIgnore]
		public DateTime LastSeen
		{
			get { return TimeHelper.ConvertToDateTime(last_seen); }

			set { last_seen = TimeHelper.ConvertToUnixTimeStamp(value); }
		}
	}
}
