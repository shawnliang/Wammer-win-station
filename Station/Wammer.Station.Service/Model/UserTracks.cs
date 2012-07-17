using System;
using System.Linq;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Wammer.Cloud;

namespace Wammer.Model
{
	[BsonIgnoreExtraElements]
	public class UserTracks
	{
		public UserTracks()
		{
		}

		public UserTracks(UserTrackResponse res)
		{
			post_id_list = res.post_id_list;
			group_id = res.group_id;
			latest_timestamp = res.latest_timestamp;
			usertrack_list = res.usertrack_list;
		}

		public UserTracks(ChangeLogResponse res)
		{
			post_id_list = res.post_list == null ? null : res.post_list.Select(x => x.post_id).ToList();
			group_id = res.group_id;
			latest_timestamp = res.latest_timestamp;
			usertrack_list = res.changelog_list;
		}

		[BsonId]
		public BsonObjectId id { get; set; }

		[BsonIgnoreIfNull]
		public List<string> post_id_list { get; set; }

		[BsonIgnoreIfNull]
		public string group_id { get; set; }

		[BsonIgnoreIfNull]
		public DateTime latest_timestamp { get; set; }

		[BsonIgnoreIfNull]
		public List<UserTrackDetail> usertrack_list { get; set; }
	}

	public class UserTrackCollection : Collection<UserTracks>
	{
		#region Var
		private static UserTrackCollection _instance; 
		#endregion

		#region Property
		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static UserTrackCollection Instance
		{
			get { return _instance ?? (_instance = new UserTrackCollection()); }
		}
		#endregion


		private UserTrackCollection()
			: base("usertracks")
		{
		}
	}
}