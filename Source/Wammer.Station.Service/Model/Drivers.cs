using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.Builders;
using Wammer.Cloud;

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

		[BsonIgnoreIfNull]
		public bool isDataImportQueried { get; set; }

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
		[BsonIgnoreIfNull]
		public DateTime start_time { get; set; }

		[BsonIgnoreIfNull]
		public DateTime? first_post_time { get; set; }
		
		[BsonIgnoreIfNull]
		public int next_seq_num { get; set; }

		/// <summary>
		/// Minimum seq number of this user's changelogs
		/// </summary>
		[BsonDefaultValue(0)]
		public int chlog_min_seq { get; set; }

		/// <summary>
		/// Maximum seq number of this user's changelogs
		/// </summary>
		[BsonDefaultValue(0)]
		public int chlog_max_seq { get; set; }

		public SyncRange Clone()
		{
			return (SyncRange) MemberwiseClone();
		}

		public override bool Equals(object obj)
		{
			if (obj == this)
				return true;

			if (obj is SyncRange)
			{
				var rhs = obj as SyncRange;
				return start_time == rhs.start_time &&
						first_post_time.Value == rhs.first_post_time.Value &&
						next_seq_num == rhs.next_seq_num &&
						chlog_max_seq == rhs.chlog_max_seq &&
						chlog_min_seq == rhs.chlog_min_seq;
			}
			else
				return false;
		}

		public override int GetHashCode()
		{
			return start_time.GetHashCode() + first_post_time.GetHashCode() + next_seq_num + chlog_max_seq + chlog_min_seq;
		}
	}
}
