using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Waveface.Stream.ClientFramework
{
    /// <summary>
    /// 
    /// </summary>
    public class PostData
    {
        #region Public Property
        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>
        /// The ID.
        /// </value>
        [JsonProperty("id")]
        public string ID { get; set; }

        /// <summary>
        /// Gets or sets the attachment count.
        /// </summary>
        /// <value>
        /// The attachment count.
        /// </value>
        [JsonProperty("attachment_count")]
        public int AttachmentCount { get; set; }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        [JsonProperty("content", NullValueHandling = NullValueHandling.Ignore)]
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the comment count.
        /// </summary>
        /// <value>
        /// The comment count.
        /// </value>
        [JsonProperty("comment_count")]
        public int CommentCount { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the time stamp.
        /// </summary>
        /// <value>
        /// The time stamp.
        /// </value>
        [JsonProperty("timestamp")]
        public string TimeStamp { get; set; }

        /// <summary>
        /// Gets or sets the name of the code.
        /// </summary>
        /// <value>
        /// The name of the code.
        /// </value>
        [JsonProperty("code_name", NullValueHandling = NullValueHandling.Ignore)]
        public string CodeName { get; set; }

        /// <summary>
        /// Gets or sets the attachment I ds.
        /// </summary>
        /// <value>
        /// The attachment I ds.
        /// </value>
        [JsonProperty("attachment_id_array", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> AttachmentIDs { get; set; }

        /// <summary>
        /// Gets or sets the favorite.
        /// </summary>
        /// <value>
        /// The favorite.
        /// </value>
        [JsonProperty("favorite")]
        public int Favorite { get; set; }

        /// <summary>
        /// Gets or sets the cover attachment ID.
        /// </summary>
        /// <value>
        /// The cover attachment ID.
        /// </value>
        [JsonProperty("cover_attach", NullValueHandling = NullValueHandling.Ignore)]
        public string CoverAttachmentID { get; set; }

        /// <summary>
        /// Gets or sets the summary attachments.
        /// </summary>
        /// <value>
        /// The summary attachments.
        /// </value>
        [JsonProperty("summary_attachments", NullValueHandling = NullValueHandling.Ignore)]
        public List<AttachmentData> SummaryAttachments { get; set; }

        /// <summary>
        /// Gets or sets the GPS.
        /// </summary>
        /// <value>
        /// The GPS.
        /// </value>
        [JsonProperty("gps", NullValueHandling = NullValueHandling.Ignore)]
        public PostGpsData Gps { get; set; }

        /// <summary>
        /// Gets or sets the people.
        /// </summary>
        /// <value>
        /// The people.
        /// </value>
        [JsonProperty("people", NullValueHandling = NullValueHandling.Ignore)]
        public List<PeopleData> People { get; set; }

        /// <summary>
        /// Gets or sets the extra params.
        /// </summary>
        /// <value>
        /// The extra params.
        /// </value>
        [JsonProperty("extra_parameters", NullValueHandling = NullValueHandling.Ignore)]
        public List<PostExtraData> ExtraParams { get; set; }

        /// <summary>
        /// Gets or sets the tags.
        /// </summary>
        /// <value>
        /// The tags.
        /// </value>
        [JsonProperty("tags", NullValueHandling = NullValueHandling.Ignore)]
        public List<String> Tags { get; set; } 
        #endregion


        #region Public Method
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

        /// <summary>
        /// Shoulds the serialize summary attachments.
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeSummaryAttachments()
        {
            return SummaryAttachments != null && SummaryAttachments.Count > 0;
        }

        /// <summary>
        /// Shoulds the serialize tags.
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeTags()
        {
            return Tags != null && Tags.Count > 0;
        }

        /// <summary>
        /// Shoulds the serialize attachment I ds.
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeAttachmentIDs()
        {
            return AttachmentIDs != null && AttachmentIDs.Count > 0;
        }
        #endregion
    }
}
