using Newtonsoft.Json;
using System;

namespace Waveface.Stream.Core
{
	public class LargeSizeAttachmentData
	{
		[JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
		public String ID { get; set; }

		[JsonProperty("file_name", NullValueHandling = NullValueHandling.Ignore)]
		public String FileName { get; set; }

		[JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
		public int Type { get; set; }

		[JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
		public String Url { get; set; }

		[JsonProperty("timestamp", NullValueHandling = NullValueHandling.Ignore)]
		public string TimeStamp { get; set; }

		[JsonProperty("meta_data", NullValueHandling = NullValueHandling.Ignore)]
		public LargeSizeMetaData MetaData { get; set; }
	}
}
