using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.Builders;
using System.Collections.Generic;
using System.Linq;
using Wammer.Cloud;

namespace Wammer.Model
{
	[BsonIgnoreExtraElements]
	public class UserTracks
	{
		public UserTracks()
		{
		}

		public UserTracks(ChangeLogResponse res)
		{
			post_id_list = res.post_list == null ? null : res.post_list.Select(x => x.post_id).ToList();
			group_id = res.group_id;
			usertrack_list = res.changelog_list;
			next_seq_num = res.next_seq_num;
		}

		[BsonId]
		public BsonObjectId id { get; set; }

		[BsonIgnoreIfNull]
		public List<string> post_id_list { get; set; }

		[BsonIgnoreIfNull]
		public string group_id { get; set; }

		[BsonIgnoreIfNull]
		public int next_seq_num { get; set; }

		[BsonIgnoreIfNull]
		public List<UserTrackDetail> usertrack_list { get; set; }
	}

	public class UserTrackCollection : Collection<UserTracks>
	{
		#region Var
		private static UserTrackCollection _instance;
		#endregion


		static UserTrackCollection()
		{
			_instance = new UserTrackCollection();
			_instance.collection.EnsureIndex(new IndexKeysBuilder().Ascending("next_seq_num"));
			_instance.collection.EnsureIndex(new IndexKeysBuilder().Ascending("group_id"));

			try
			{
				// purge user tracks if they grow too large.
				if (_instance.collection.Count() > 100)
				{
					_instance.collection.RemoveAll();

					// int.MaxValue is the initial value if there is not user tracks available
					var update = MongoDB.Driver.Builders.Update.
						Set("sync_range.chlog_min_seq", int.MaxValue).
						Set("sync_range.chlog_max_seq", int.MaxValue);

					DriverCollection.Instance.Update(Query.Exists("sync_range"), update);
				}
			}
			catch
			{
			}
		}

		#region Property
		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static UserTrackCollection Instance
		{
			get { return _instance; }
		}
		#endregion


		private UserTrackCollection()
			: base("usertracks")
		{
		}
	}
}