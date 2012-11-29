using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Waveface.Stream.Model
{
    public class ConnectMsg
    {
        public string session_token { get; set; }
        public string apikey { get; set; }
        public string user_id { get; set; }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(session_token) &&
                !string.IsNullOrEmpty(apikey) &&
                !string.IsNullOrEmpty(user_id);
        }
    }
}
