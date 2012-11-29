using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Waveface.Stream.ClientFramework
{
    public class AttachmentGPSData
    {
        [JsonProperty("longitude", NullValueHandling = NullValueHandling.Ignore)]
        public double Longitude { get; set; }

        [JsonProperty("latitude", NullValueHandling = NullValueHandling.Ignore)]
        public double Latitude { get; set; }

        [JsonProperty("gps_date_stamp", NullValueHandling = NullValueHandling.Ignore)]
        public string GPSDateStamp { get; set; }

        [JsonProperty("gps_time_stamp", NullValueHandling = NullValueHandling.Ignore)]
        public List<object[]> GPSTimeStamp { get; set; }
    }
}
