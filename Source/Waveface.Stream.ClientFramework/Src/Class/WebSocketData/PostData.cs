using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Waveface.Stream.ClientFramework
{
    public class PostData
    {
        public string id { get; set; }

        public int attachment_count { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string content { get; set; }

        public int comment_count { get; set; }

        public string type { get; set; }

        public string timestamp { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string code_name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string> attachment_id_array { get; set; }

        public int favorite { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string cover_attach { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<AttachmentData> summary_attachments { get; set; }
    }
}
