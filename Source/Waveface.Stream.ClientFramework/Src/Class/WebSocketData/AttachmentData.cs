using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Waveface.Stream.ClientFramework
{
    public class AttachmentData
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public String id { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public String file_name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public String url { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string timestamp { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ImageMetaData image_meta { get; set; }
    }
}
