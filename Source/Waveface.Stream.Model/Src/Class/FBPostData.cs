using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Waveface.Stream.Model
{
    public class FBPostData
    {
        public string device_id { get; set; }

        public string device_name { get; set; }

        public string device { get; set; }

        public string api_key { get; set; }

        public string xurl { get; set; }

        public string locale { get; set; }

        public string show_tutorial { get; set; }

        public string session_token { get; set; }
    }
}
