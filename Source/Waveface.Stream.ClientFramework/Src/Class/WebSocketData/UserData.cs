using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Waveface.Stream.ClientFramework
{
    public class UserData
    {
        public string session_token { get; set; }

        public string nickname { get; set; }

        public string email { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<DeviceData> devices { get; set; }
    }
}
