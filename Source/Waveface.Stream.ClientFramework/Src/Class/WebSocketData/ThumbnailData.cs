using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Waveface.Stream.ClientFramework
{
    public class ThumbnailData
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public String url { get; set; }

        public int width { get; set; }

        public int height { get; set; }
    }
}
