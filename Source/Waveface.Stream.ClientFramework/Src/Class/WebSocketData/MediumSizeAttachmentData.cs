using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Waveface.Stream.ClientFramework
{
    public class MediumSizeAttachmentData
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public String ID { get; set; }

        [JsonProperty("file_name", NullValueHandling = NullValueHandling.Ignore)]
        public String FileName { get; set; }

        [JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
        public String Url { get; set; }

        [JsonProperty("timestamp", NullValueHandling = NullValueHandling.Ignore)]
        public string TimeStamp { get; set; }

        [JsonProperty("image_meta", NullValueHandling = NullValueHandling.Ignore)]
        public MediumSizeImageMetaData ImageMeta { get; set; }
    }
}
