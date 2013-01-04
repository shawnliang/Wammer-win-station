using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Waveface.Stream.Model;

namespace Waveface.Stream.Core
{
	/// <summary>
	/// 
	/// </summary>
	public class MediumSizePostData
	{
		#region Var
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
		/// Gets or sets the content.
		/// </summary>
		/// <value>
		/// The content.
		/// </value>
		[JsonProperty("content", NullValueHandling = NullValueHandling.Ignore)]
		public string Content { get; set; }

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
				return AttachmentIDs == null ? 0 : AttachmentIDs.Count();
			}
		}

		/// <summary>
		/// Gets or sets the type.
		/// </summary>
		/// <value>
		/// The type.
		/// </value>
		[JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
		public PostType Type { get; set; }

		/// <summary>
		/// Gets or sets the time stamp.
		/// </summary>
		/// <value>
		/// The time stamp.
		/// </value>
		[JsonProperty("timestamp", NullValueHandling = NullValueHandling.Ignore)]
		public DateTime? TimeStamp { get; set; }

		/// <summary>
		/// Gets or sets the attachment I ds.
		/// </summary>
		/// <value>
		/// The attachment I ds.
		/// </value>
		[JsonProperty("attachment_id_array", NullValueHandling = NullValueHandling.Ignore)]
		public IEnumerable<string> AttachmentIDs { get; set; }

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

		/// <summary>
		/// Gets or sets the check ins.
		/// </summary>
		/// <value>
		/// The check ins.
		/// </value>
		[JsonProperty("checkins", NullValueHandling = NullValueHandling.Ignore)]
		public IEnumerable<PostCheckInData> CheckIns { get; set; }

		/// <summary>
		/// Gets or sets the GPS.
		/// </summary>
		/// <value>
		/// The GPS.
		/// </value>
		[JsonProperty("location", NullValueHandling = NullValueHandling.Ignore)]
		public PostGpsData Location { get; set; }

		/// <summary>
		/// Gets or sets the people.
		/// </summary>
		/// <value>
		/// The people.
		/// </value>
		[JsonProperty("friends", NullValueHandling = NullValueHandling.Ignore)]
		public IEnumerable<FriendData> Friends { get; set; }

		/// <summary>
		/// Gets or sets the extra params.
		/// </summary>
		/// <value>
		/// The extra params.
		/// </value>
		[JsonProperty("extra_parameters", NullValueHandling = NullValueHandling.Ignore)]
		public IEnumerable<PostExtraData> ExtraParams { get; set; }

		/// <summary>
		/// Gets or sets the tags.
		/// </summary>
		/// <value>
		/// The tags.
		/// </value>
		[JsonProperty("tags", NullValueHandling = NullValueHandling.Ignore)]
		public IEnumerable<String> Tags { get; set; }
		#endregion


		#region Public Method
		/// <summary>
		/// Shoulds the serialize extra params.
		/// </summary>
		/// <returns></returns>
		public bool ShouldSerializeExtraParams()
		{
			return ExtraParams != null && ExtraParams.Any();
		}

		/// <summary>
		/// Shoulds the serialize GPS.
		/// </summary>
		/// <returns></returns>
		public bool ShouldSerializeLocation()
		{
			return Location != null && (Location.Name != null || (Location.Latitude != null && Location.Longitude != null));
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
			return Tags != null && Tags.Any();
		}

		/// <summary>
		/// Shoulds the serialize attachment I ds.
		/// </summary>
		/// <returns></returns>
		public bool ShouldSerializeAttachmentIDs()
		{
			return AttachmentIDs != null && AttachmentIDs.Any();
		}

		/// <summary>
		/// Shoulds the serialize cover attachment ID.
		/// </summary>
		/// <returns></returns>
		public bool ShouldSerializeCoverAttachmentID()
		{
			return !String.IsNullOrEmpty(CoverAttachmentID);
		}

		///// <summary>
		///// Shoulds the content of the serialize.
		///// </summary>
		///// <returns></returns>
		//public bool ShouldSerializeContent()
		//{
		//	return !String.IsNullOrEmpty(Content);
		//}

		/// <summary>
		/// Shoulds the serialize ID.
		/// </summary>
		/// <returns></returns>
		public bool ShouldSerializeID()
		{
			return !String.IsNullOrEmpty(ID);
		}

		/// <summary>
		/// Shoulds the serialize attachment count.
		/// </summary>
		/// <returns></returns>
		public bool ShouldSerializeAttachmentCount()
		{
			return AttachmentCount > 0;
		}

		///// <summary>
		///// Shoulds the serialize comment count.
		///// </summary>
		///// <returns></returns>
		//public bool ShouldSerializeCommentCount()
		//{
		//	return CommentCount > 0;
		//}

		/// <summary>
		/// Shoulds the serialize check ins.
		/// </summary>
		/// <returns></returns>
		public bool ShouldSerializeCheckIns()
		{
			return CheckIns != null && CheckIns.Any();
		}

		public bool ShouldSerializeFriends()
		{
			return Friends != null && Friends.Any();
		}
		#endregion
	}
}
