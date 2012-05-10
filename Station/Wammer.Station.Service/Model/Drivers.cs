using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Wammer.Cloud;
using System;
using MongoDB.Driver.Builders;

namespace Wammer.Model
{
	/// <summary>
	/// The Driver model class, that can specified which user will be servicied
	/// </summary>
	[BsonIgnoreExtraElements]
	public class Driver
	{
		#region Var
		private List<UserGroup> _groups;
		#endregion

		#region Public Property
		/// <summary>
		/// Gets or sets the user_id.
		/// </summary>
		/// <value>The user_id.</value>
		[BsonId]
		public string user_id { get; set; }

		/// <summary>
		/// Gets or sets the email.
		/// </summary>
		/// <value>The email.</value>
		[BsonIgnoreIfNull]
		public string email { get; set; }

		/// <summary>
		/// Gets or sets the folder.
		/// </summary>
		/// <value>The folder.</value>
		[BsonIgnoreIfNull]
		public string folder { get; set; }

		/// <summary>
		/// Gets or sets the groups.
		/// </summary>
		/// <value>The groups.</value>
		[BsonIgnoreIfNull]
		public List<UserGroup> groups
		{
			get
			{
				if (_groups == null)
					_groups = new List<UserGroup>();
				return _groups;
			}
			set
			{
				_groups = value;
			}
		}

		/// <summary>
		/// Gets or sets the session_token.
		/// </summary>
		/// <value>The session_token.</value>
		[BsonIgnoreIfNull]
		public string session_token { get; set; }
		
		/// <summary>
		/// Gets or sets a value indicating whether this instance is primary station.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is primary station; otherwise, <c>false</c>.
		/// </value>
		[BsonDefaultValue(true)]
		[BsonIgnoreIfNull]
		public bool isPrimaryStation { get; set; }

		/// <summary>
		/// Gets or sets the sync_range.
		/// </summary>
		/// <value>The sync_range.</value>
		[BsonIgnoreIfNull]
		public SyncRange sync_range { get; set; }

		/// <summary>
		/// Is UserTracks synced with timeline ?
		/// </summary>
		[BsonIgnoreIfNull]
		public bool is_change_history_synced { get; set; }

		/// <summary>
		/// Gets or sets the user.
		/// </summary>
		/// <value>The user.</value>
		[BsonIgnoreIfNull]
		public UserInfo user { get; set; }

		/// <summary>
		/// Gets or sets the ref_count.
		/// </summary>
		/// <value>The ref_count.</value>
		[BsonDefaultValue(1)]
		[BsonIgnoreIfNull]
		public int ref_count { get; set; }
		#endregion
	}

	public class DriverCollection : Collection<Driver>
	{
		private static DriverCollection instance;

		static DriverCollection()
		{
			instance = new DriverCollection();
		}

		private DriverCollection()
			:base("drivers")
		{
		}

		public static DriverCollection Instance
		{
			get { return instance; }
		}

		public Driver FindDriverByGroupId(string group_id)
		{
			if (group_id == null)
				throw new ArgumentNullException("group_id");

			return instance.FindOne(Query.ElemMatch("groups", Query.EQ("group_id", group_id)));
		}
	}

	[BsonIgnoreExtraElements]
	public class SyncRange
	{
		[BsonIgnoreIfNull]
		public DateTime start_time { get; set; }
		[BsonIgnoreIfNull]
		public DateTime end_time { get; set; }
		[BsonIgnoreIfNull]
		public Nullable<DateTime> first_post_time { get; set; }

		public SyncRange Clone()
		{
			return (SyncRange)this.MemberwiseClone();
		}
	}
}
