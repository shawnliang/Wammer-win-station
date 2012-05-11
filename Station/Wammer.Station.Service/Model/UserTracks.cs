using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Wammer.Cloud;

namespace Wammer.Model
{
	[BsonIgnoreExtraElements]
	public class UserTracks
	{
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
	}

	public class UserTrackCollection : Collection<UserTracks>
	{
		private static readonly UserTrackCollection instance;

		static UserTrackCollection()
		{
			instance = new UserTrackCollection();
		}

		private UserTrackCollection()
			:base("usertracks")
		{
		}

		public static UserTrackCollection Instance
		{
			get { return instance; }
		}
	}

}
