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
	public class MediumSizeCollcetionData
	{
		#region Private Property
		private string _coverAttachmentID;
		#endregion

		#region Public Property
		/// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>
        /// The ID.
        /// </value>
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string ID { get; set; }

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		[JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
		public string Name { get; set; }

        /// <summary>
        /// Gets or sets the attachment count.
        /// </summary>
        /// <value>
        /// The attachment count.
        /// </value>
        [JsonProperty("attachment_count")]
		public int AttachmentCount
		{
			get
			{
 				return AttachmentIDs == null? 0: AttachmentIDs.Count();
			}
		}

		/// <summary>
		/// Gets or sets the attachment I ds.
		/// </summary>
		/// <value>
		/// The attachment I ds.
		/// </value>
		[JsonProperty("attachment_id_array", NullValueHandling = NullValueHandling.Ignore)]
		public List<string> AttachmentIDs { get; set; }

        /// <summary>
        /// Gets or sets the time stamp.
        /// </summary>
        /// <value>
        /// The time stamp.
        /// </value>
        [JsonProperty("timestamp", NullValueHandling = NullValueHandling.Ignore)]
        public string TimeStamp { get; set; }

		/// <summary>
		/// Gets or sets the cover attachment ID.
		/// </summary>
		/// <value>
		/// The cover attachment ID.
		/// </value>
		[JsonProperty("cover_attach", NullValueHandling = NullValueHandling.Ignore)]
		public string CoverAttachmentID
		{
			get
			{
				return string.IsNullOrEmpty(_coverAttachmentID) && (AttachmentIDs != null) ? AttachmentIDs.FirstOrDefault() : _coverAttachmentID;
			}
			set
			{
				_coverAttachmentID = value;
			}
		}

        /// <summary>
        /// Gets or sets the summary attachments.
        /// </summary>
        /// <value>
        /// The summary attachments.
        /// </value>
        [JsonProperty("summary_attachments", NullValueHandling = NullValueHandling.Ignore)]
        public List<MediumSizeAttachmentData> SummaryAttachments { get; set; }
        #endregion


        #region Public Method
        /// <summary>
        /// Shoulds the serialize summary attachments.
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeSummaryAttachments()
        {
            return SummaryAttachments != null && SummaryAttachments.Count > 0;
        }

        /// <summary>
        /// Shoulds the serialize attachment I ds.
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeAttachmentIDs()
        {
            return AttachmentIDs != null && AttachmentIDs.Count > 0;
        }

        /// <summary>
        /// Shoulds the serialize time stamp.
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeTimeStamp()
        {
            return TimeStamp != null && TimeStamp.Length > 0;
        }

        /// <summary>
        /// Shoulds the serialize ID.
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeID()
        {
            return ID != null && ID.Length > 0;
        }

        /// <summary>
        /// Shoulds the serialize attachment count.
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeAttachmentCount()
        {
            return AttachmentCount > 0;
        }
        #endregion
    }
}
