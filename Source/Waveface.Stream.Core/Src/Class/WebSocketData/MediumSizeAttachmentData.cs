using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Waveface.Stream.Core
{
	public class MediumSizeAttachmentData
	{
		#region Public Property
		[JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
		public String ID { get; set; }

		[JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
		public String Title { get; set; }

		[JsonProperty("file_name", NullValueHandling = NullValueHandling.Ignore)]
		public String FileName { get; set; }

		[JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
		public int Type { get; set; }

		[JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
		public String Url { get; set; }

		[JsonProperty("timestamp", NullValueHandling = NullValueHandling.Ignore)]
		public string TimeStamp { get; set; }

		[JsonProperty("meta_data", NullValueHandling = NullValueHandling.Ignore)]
		public MediumSizeMetaData MetaData { get; set; } 
		#endregion
	}
}
