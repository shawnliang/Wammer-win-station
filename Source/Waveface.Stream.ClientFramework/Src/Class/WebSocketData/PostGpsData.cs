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

        public float latitude { get; set; }

        public float longitude { get; set; }

        public int zoom_level { get; set; }
    }
}
