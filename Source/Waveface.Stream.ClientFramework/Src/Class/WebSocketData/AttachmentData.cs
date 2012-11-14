using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Waveface.Stream.ClientFramework
{
    public class AttachmentData
    {
        public String id { get; set; }

        public String file_name { get; set; }

        public String url { get; set; }

        public string timestamp { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ImageMetaData image_meta { get; set; }
    }
}
