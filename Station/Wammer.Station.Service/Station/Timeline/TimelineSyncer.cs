using System;
using System.Collections.Generic;
using System.Linq;
using Wammer.Cloud;
using Wammer.Model;

namespace Wammer.Station.Timeline
{
	public interface IPostProvider
	{
		PostResponse GetLastestPosts(Driver user, int limit);
		PostResponse GetPostsBefore(Driver user, DateTime before, int limit);
		List<PostInfo> RetrievePosts(Driver user, List<string> posts);
	}

	public interface ITimelineSyncerDB
	{
		void SavePost(PostInfo post);
		void SaveUserTracks(UserTracks userTracks);
		void UpdateDriverSyncRange(string userId, SyncRange syncRange);
		void UpdateDriverChangeHistorySynced(string userId, bool isSynced);
	}

	public class TimelineSyncer
	{
		private readonly ITimelineSyncerDB db;
		private readonly IPostProvider postProvider;
		private readonly IUserTrackApi userTrack;

		public TimelineSyncer(IPostProvider postProvider, ITimelineSyncerDB db, IUserTrackApi userTrack)
		{
			this.postProvider = postProvider;
			this.db = db;
			this.userTrack = userTrack;
		}

		public event EventHandler<TimelineSyncEventArgs> PostsRetrieved;
		public event EventHandler<BodyAvailableEventArgs> BodyAvailable;

		/// <summary>
		/// Use PullTimeline() instead.
		/// Pull user's timeline base on his sync_range and save timeline posts to db
		/// </summary>
		/// <param name="user"></param>
		public void PullBackward(Driver user, int limit = 200)
		{
			if (user == null)
				throw new ArgumentNullException("user");

			if (user.sync_range != null && user.sync_range.first_post_time.HasValue)
				throw new InvalidOperationException("Has already pulled the oldest post");

			var res = user.sync_range == null
						? postProvider.GetLastestPosts(user, limit)
						: postProvider.GetPostsBefore(user, user.sync_range.start_time, limit);

			foreach (var post in res.posts)
				db.SavePost(post);

			OnPostsRetrieved(user, res.posts);

			if (res.posts.Count > 0)
			{
				db.UpdateDriverSyncRange(user.user_id, new SyncRange
				                                       	{
				                                       		start_time = res.posts.Last().timestamp,
				                                       		end_time =
				                                       			(user.sync_range == null)
				                                       				? res.posts.First().timestamp
				                                       				: user.sync_range.end_time,
				                                       		first_post_time =
				                                       			(res.HasMoreData) ? null as DateTime? : res.posts.Last().timestamp
				                                       	});
			}

		}

		/// <summary>
		/// Use PullTimeline() instead.
		/// Calls user track api to get changed posts and saves to db
		/// </summary>
		/// <param name="user"></param>
		public void PullForward(Driver user)
		{
			if (user == null)
				throw new ArgumentNullException("user");

			if (user.sync_range == null || user.sync_range.end_time == DateTime.MinValue)
				throw new InvalidOperationException("Should call PullBackward() first");

			DateTime since = user.sync_range.end_time;

			UserTrackResponse res;

			try
			{
				res = userTrack.GetChangeHistory(user, since);
			}
			catch (WammerCloudException e)
			{
				// when no more data, cloud returns a empty last_timestamp which makes
				// deserializing to json object parses error and a ArgumentOutOfRangeException
				// is thrown.
				//
				// Use this exception as the exit criteria
				if (e.InnerException is ArgumentOutOfRangeException)
					return;
				throw;
			}

			db.SaveUserTracks(new UserTracks(res));

			ProcChangedPosts(user, res);
			ProcNewAttachments(res);
		}

		private void ProcChangedPosts(Driver user, UserTrackResponse res)
		{
			var post_id_set = new HashSet<string>();

			if (res.usertrack_list != null)
			{
				var postIds = res.usertrack_list.Where(x => x.target_type == "attachment" && x.actions[0].target_type == "image.medium").
					Select(x => x.actions[0].post_id);

				foreach (var postId in postIds)
					post_id_set.Add(postId);
			}

			if (res.post_id_list != null)
			{
				foreach (var postId in res.post_id_list)
					post_id_set.Add(postId);
			}


			if (post_id_set.Count > 0)
			{
				List<PostInfo> changedPost = postProvider.RetrievePosts(user, post_id_set.ToList());
				foreach (PostInfo post in changedPost)
					db.SavePost(post);

				OnPostsRetrieved(user, changedPost);

				db.UpdateDriverSyncRange(user.user_id,
				                         new SyncRange
				                         	{
				                         		start_time = user.sync_range.start_time,
				                         		end_time = res.latest_timestamp,
				                         		first_post_time = user.sync_range.first_post_time,
				                         	});
			}
		}

		private void ProcNewAttachments(UserTrackResponse res)
		{
			if (res.usertrack_list == null)
				return;

			foreach (UserTrackDetail track in res.usertrack_list)
			{
				if (track.target_type == "attachment" &&
				    track.actions != null)
				{
					if (track.actions.Any(action => action.target_type.Contains("origin")))
					{
						OnBodyAvailable(new BodyAvailableEventArgs(track.target_id, track.user_id, track.group_id));
					}
				}
			}
		}

		private void OnBodyAvailable(BodyAvailableEventArgs args)
		{
			EventHandler<BodyAvailableEventArgs> handler = BodyAvailable;
			if (handler != null)
				handler(this, args);
		}

		public void PullTimeline(Driver user)
		{
			if (user == null)
				throw new ArgumentNullException("user");

			if (HasNeverSynced(user))
				PullBackward(user);
			else if (HasUnsyncedOldPosts(user))
				PullBackward(user);
			else if (!user.is_change_history_synced)
				PullOldChangeLog(user);
			else
				PullForward(user);
		}

		private void PullOldChangeLog(Driver user)
		{
			var api = new UserTracksApi();
			DateTime since = DateTime.MinValue;

			UserTrackResponse res;

			do
			{
				res = api.GetChangeHistory(user, since);

				db.SaveUserTracks(new UserTracks(res));
				since = res.latest_timestamp.AddSeconds(1.0);
			} while (since <= user.sync_range.end_time);

			// Last user track response could contain unsynced posts.
			List<PostInfo> newPosts = postProvider.RetrievePosts(user, res.post_id_list);
			foreach (PostInfo post in newPosts)
				db.SavePost(post);

			OnPostsRetrieved(user, newPosts);

			SyncRange newSyncRange = user.sync_range.Clone();
			newSyncRange.end_time = since;
			db.UpdateDriverSyncRange(user.user_id, newSyncRange);
			db.UpdateDriverChangeHistorySynced(user.user_id, true);
		}

		private static bool HasUnsyncedOldPosts(Driver user)
		{
			return !user.sync_range.first_post_time.HasValue;
		}

		private static bool HasNeverSynced(Driver user)
		{
			return user.sync_range == null || user.sync_range.end_time == DateTime.MinValue;
		}

		private void OnPostsRetrieved(Driver driver, List<PostInfo> posts)
		{
			EventHandler<TimelineSyncEventArgs> handler = PostsRetrieved;
			if (handler != null)
			{
				handler(this, new TimelineSyncEventArgs(driver, posts));
			}
		}
	}

	public class TimelineSyncEventArgs : EventArgs
	{
		public TimelineSyncEventArgs(Driver driver, ICollection<PostInfo> posts)
		{
			Driver = driver;
			Posts = posts;
		}

		public Driver Driver { get; private set; }
		public ICollection<PostInfo> Posts { get; private set; }
	}

	public class BodyAvailableEventArgs : EventArgs
	{
		public BodyAvailableEventArgs(string object_id, string user_id, string group_id)
		{
			this.object_id = object_id;
			this.user_id = user_id;
			this.group_id = group_id;
		}

		public string object_id { get; private set; }
		public string user_id { get; private set; }
		public string group_id { get; private set; }
	}
}