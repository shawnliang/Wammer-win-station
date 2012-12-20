using Newtonsoft.Json;

namespace Waveface.Stream.Core
{
	public class DeviceData
	{
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string device_name { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string device_id { get; set; }
	}
}
