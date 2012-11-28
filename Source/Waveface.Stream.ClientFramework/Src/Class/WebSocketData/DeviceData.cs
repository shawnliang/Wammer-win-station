using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Waveface.Stream.ClientFramework
{
    public class DeviceData
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string device_name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string device_id { get; set; }
    }
}
