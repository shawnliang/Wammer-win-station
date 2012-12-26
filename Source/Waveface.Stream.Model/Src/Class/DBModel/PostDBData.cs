using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Waveface.Stream.Model
{
	/// <summary>
	/// 
	/// </summary>
	[BsonIgnoreExtraElements]
	public class PostDBData
	{
		#region Var
		private string _coverAttachID; 
		#endregion


		#region Public Property
		/// <summary>
		/// Gets or sets the post_id.
		/// </summary>
		/// <value>
		/// The post_id.
		/// </value>
		[BsonId]
		public string ID { get; set; }

		/// <summary>
		/// Gets or sets the type.
		/// </summary>
		/// <value>
		/// The type.
		/// </value>
		[BsonIgnoreIfNull]
		[BsonElement("type")]
		public PostType Type { get; set; }

		/// <summary>
		/// Gets or sets the cover_attach.
		/// </summary>
		/// <value>
		/// The cover_attach.
		/// </value>
		[BsonIgnoreIfNull]
		[BsonElement("cover_attachment_id")]
		public string CoverAttachmentID
		{
			get
			{
				return (string.IsNullOrEmpty(_coverAttachID) && AttachmentIDs != null) ? AttachmentIDs.FirstOrDefault() : _coverAttachID;
			}
			set
			{
				_coverAttachID = value;
			}
		}

		/// <summary>
		/// Gets or sets the attachment_id_array.
		/// </summary>
		/// <value>
		/// The attachment_id_array.
		/// </value>
		[BsonIgnoreIfNull]
		[BsonElement("attachment_id_array")]
		public IEnumerable<string> AttachmentIDs { get; set; }

		///// <summary>
		///// Gets or sets the content.
		///// </summary>
		///// <value>
		///// The content.
		///// </value>
		//[BsonIgnoreIfNull]
		//[BsonElement("content")]
		//public string Content { get; set; }

		///// <summary>
		///// Gets or sets the code_name.
		///// </summary>
		///// <value>
		///// The code_name.
		///// </value>
		//[BsonIgnoreIfNull]
		//[BsonElement("code_name")]
		//public string CodeName { get; set; }

		///// <summary>
		///// Gets or sets the device_id.
		///// </summary>
		///// <value>
		///// The device_id.
		///// </value>
		//[BsonIgnoreIfNull]
		//[BsonElement("device_id")]
		//public string DeviceID { get; set; }

		/// <summary>
		/// Gets or sets the creator_id.
		/// </summary>
		/// <value>
		/// The creator_id.
		/// </value>
		[BsonIgnoreIfNull]
		[BsonElement("creator_id")]
		public string CreatorID { get; set; }

		/// <summary>
		/// Gets or sets the hidden.
		/// </summary>
		/// <value>
		/// The hidden.
		/// </value>
		[BsonIgnoreIfNull]
		[BsonIgnoreIfDefault]
		[BsonDefaultValue(true)]
		[BsonElement("visibility")]
		public Boolean Visibility { get; set; }

		/// <summary>
		/// Gets or sets the checkins.
		/// </summary>
		/// <value>
		/// The checkins.
		/// </value>
		[BsonIgnoreIfNull]
		[BsonElement("checkin_id_array")]
		public IEnumerable<String> CheckinIDs { get; set; }

		/// <summary>
		/// Gets or sets the people.
		/// </summary>
		/// <value>
		/// The people.
		/// </value>
		[BsonIgnoreIfNull]
		[BsonElement("friend_id_array")]
		public IEnumerable<String> FriendIDs { get; set; }

		/// <summary>
		/// Gets or sets the extra_parameters.
		/// </summary>
		/// <value>
		/// The extra_parameters.
		/// </value>
		[BsonIgnoreIfNull]
		[BsonElement("extra_params")]
		public IEnumerable<ExtraParameter> ExtraParams { get; set; }

		/// <summary>
		/// Gets or sets the event_time.
		/// </summary>
		/// <value>
		/// The event_time.
		/// </value>
		[BsonIgnoreIfNull]
		[BsonElement("event_since_time")]
		public DateTime? EventSinceTime { get; set; }

		/// <summary>
		/// Gets or sets the event_time.
		/// </summary>
		/// <value>
		/// The event_time.
		/// </value>
		[BsonIgnoreIfNull]
		[BsonElement("event_until_time")]
		public DateTime? EventUntilTime { get; set; }

		/// <summary>
		/// Gets or sets the update_time.
		/// </summary>
		/// <value>
		/// The update_time.
		/// </value>
		[BsonIgnoreIfNull]
		[BsonElement("modify_time")]
		public DateTime? ModifyTime { get; set; }

		/// <summary>
		/// Gets or sets the tags.
		/// </summary>
		/// <value>
		/// The tags.
		/// </value>
		[BsonIgnoreIfNull]
		[BsonElement("tags")]
		public IEnumerable<string> Tags { get; set; } 
		#endregion


		#region Public Method
		public bool ShouldSerializeAttachmentIDs()
		{
			return AttachmentIDs != null && AttachmentIDs.Any();
		} 

		public bool ShouldSerializeFriendIDs()
		{
			return FriendIDs != null && FriendIDs.Any();
		}

		public bool ShouldSerializeCheckinIDs()
		{
			return CheckinIDs != null && CheckinIDs.Any();
		}

		public bool ShouldSerializeExtraParams()
		{
			return ExtraParams != null && ExtraParams.Any();
		}

		public bool ShouldSerializeTags()
		{
			return Tags != null && Tags.Any();
		}


		public bool ShouldSerializeCoverAttachmentID()
		{
			return !String.IsNullOrEmpty(CoverAttachmentID);
		} 
		#endregion
	}
}
