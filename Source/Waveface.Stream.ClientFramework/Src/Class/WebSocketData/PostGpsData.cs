using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Waveface.Stream.ClientFramework
{
    public class PostGpsData
    {
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public String Name { get; set; }

        [JsonProperty("latitude", NullValueHandling = NullValueHandling.Ignore)]
        public float? Latitude { get; set; }

        [JsonProperty("longitude", NullValueHandling = NullValueHandling.Ignore)]
        public float? Longitude { get; set; }

        [JsonProperty("zoom_level", NullValueHandling = NullValueHandling.Ignore)]
        public int? ZoomLevel { get; set; }
    }
}
