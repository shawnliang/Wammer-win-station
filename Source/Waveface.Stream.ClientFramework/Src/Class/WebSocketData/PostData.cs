using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Waveface.Stream.ClientFramework
{
    public class PostData
    {
        #region Property
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

        [JsonProperty("gps", NullValueHandling = NullValueHandling.Ignore)]
        public PostGpsData Gps { get; set; }

        [JsonProperty("people", NullValueHandling = NullValueHandling.Ignore)]
        public List<PeopleData> People { get; set; }

        [JsonProperty("extra_parameters", NullValueHandling = NullValueHandling.Ignore)]
        public List<PostExtraData> ExtraParams { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<String> tags { get; set; } 
        #endregion


        #region Method
        /// <summary>
        /// Shoulds the serialize people.
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializePeople()
        {
            return People != null && People.Count > 0;
        }

        /// <summary>
        /// Shoulds the serialize extra params.
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeExtraParams()
        {
            return ExtraParams != null && ExtraParams.Count > 0;
        }

        /// <summary>
        /// Shoulds the serialize GPS.
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeGps()
        {
            return Gps != null && (Gps.name != null || (Gps.latitude != null && Gps.longitude != null));
        }
        #endregion
    }
}
