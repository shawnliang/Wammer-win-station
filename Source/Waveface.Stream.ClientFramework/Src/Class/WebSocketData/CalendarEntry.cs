using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Waveface.Stream.ClientFramework
{
    public class CalendarEntry
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string since_date { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string until_date { get; set; }

        public int post_count { get; set; }

        public int attachment_count { get; set; }
    }
}
