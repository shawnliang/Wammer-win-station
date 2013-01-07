using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;

namespace Waveface.Stream.Model
{
	/// <summary>
	/// The Driver model class, that can specified which user will be servicied
	/// </summary>
	[BsonIgnoreExtraElements]
	public class Driver
	{
		#region Var

		private List<UserGroup> _groups;
		private List<UserStation> _stations;

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

		///// <summary>
		///// Gets or sets the friend I ds.
		///// </summary>
		///// <value>
		///// The friend I ds.
		///// </value>
		//[BsonIgnoreIfNull]
		//[BsonElement("friend_id_array")]
		//public IEnumerable<String> FriendIDs { get; set; }

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
			get { return _groups ?? (_groups = new List<UserGroup>()); }
			set { _groups = value; }
		}

		/// <summary>
		/// Gets or sets the stations
		/// </summary>
		/// <value>The stations.</value>
		[BsonIgnoreIfNull]
		public List<UserStation> stations
		{
			get { return _stations ?? (_stations = new List<UserStation>()); }
			set { _stations = value; }
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


		public bool isPaidUser { get; set; }

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


		/// <summary>
		/// Size limit of all origin attachments
		/// </summary>
		public long origin_limit { get; set; }

		/// <summary>
		/// Current size of all origin attachments
		/// </summary>
		public long cur_origin_size { get; set; }

		#endregion

		public bool ReachOriginSizeLimit()
		{
			return origin_limit > 0 && cur_origin_size >= origin_limit;
		}
	}

	public class DriverCollection : DBCollection<Driver>
	{
		#region Var
		private static DriverCollection _instance;
		#endregion

		#region Property
		public static DriverCollection Instance
		{
			get { return _instance ?? (_instance = new DriverCollection()); }
		}
		#endregion

		private DriverCollection()
			: base("drivers")
		{
		}


		public Driver FindDriverByGroupId(string group_id)
		{
			if (group_id == null)
				throw new ArgumentNullException("group_id");

			return Instance.FindOne(Query.ElemMatch("groups", Query.EQ("group_id", group_id)));
		}

		public string GetGroupIdByUser(string user_id)
		{
			var driver = Instance.FindOneById(user_id);
			if (driver == null)
				return null;

			return driver.groups[0].group_id;
		}
	}

	[BsonIgnoreExtraElements]
	public class SyncRange
	{
		// used as next_seq_num when calling posts/fetchBySeq to sync posts
		public int post_next_seq { get; set; }

		// used as modified_time_since when calling attachments/search to sync attachments
		public DateTime obj_next_time { get; set; }

		// enable periodical sync
		public bool enable { get; set; }
	}
}
