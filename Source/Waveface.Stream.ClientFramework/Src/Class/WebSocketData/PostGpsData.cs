using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Waveface.Stream.ClientFramework
{
    public class PostGpsData
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public String name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public float? latitude { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public float? longitude { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? zoom_level { get; set; }
    }
}
