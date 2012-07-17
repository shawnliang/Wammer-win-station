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
		PostResponse GetPostsBySeq(Driver user, int seq, int limit);
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
		private readonly IChangeLogsApi changelogs;
		private readonly IPostProvider postProvider;

		public TimelineSyncer(IPostProvider postProvider, ITimelineSyncerDB db, IChangeLogsApi changelogs)
		{
			this.postProvider = postProvider;
			this.db = db;
			this.changelogs = changelogs;
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
				var range = new SyncRange
					{
						start_time = res.posts.Min(x => x.timestamp),
						first_post_time =
							(res.HasMoreData) ? null as DateTime? : res.posts.Min(x => x.timestamp)
					};

				db.UpdateDriverSyncRange(user.user_id, range);
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

			if (!user.is_change_history_synced)
				throw new InvalidOperationException("Should call PullBackward() first");

			try
			{
				var res = changelogs.GetChangeHistory(user, user.sync_range.next_seq_num);
				db.SaveUserTracks(new UserTracks(res));

				ProcChangedPosts(user, res);
				ProcNewAttachments(res);
			}
			catch(WammerCloudException e)
			{
				if (e.WammerError == (int)Wammer.Station.UserTrackApiError.TooManyUserTracks)
				{
					int next_seq = RetrieveAllPostsBySeq(user, user.sync_range.next_seq_num);
					UpdateDBForUserTrackBackFilled(user, next_seq);
				}
				else
					throw;
			}
		}

		private void ProcChangedPosts(Driver user, ChangeLogResponse res)
		{
			var post_id_set = new HashSet<string>();

			if (res.changelog_list != null)
			{
				var postIds = res.changelog_list.Where(x => x.target_type == "attachment" && x.actions[0].target_type == "image.medium").
					Select(x => x.actions[0].post_id);

				foreach (var postId in postIds)
					post_id_set.Add(postId);
			}

			if (res.post_list != null)
			{
				foreach (var postItem in res.post_list)
					post_id_set.Add(postItem.post_id);
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
												next_seq_num = res.next_seq_num,
				                         		first_post_time = user.sync_range.first_post_time,
				                         	});
			}
		}

		private void ProcNewAttachments(ChangeLogResponse res)
		{
			if (res.changelog_list == null)
				return;

			foreach (UserTrackDetail track in res.changelog_list)
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
			int since_seq_num = 1;
			ChangeLogResponse res = new ChangeLogResponse { post_list = new List<PostListItem>() };

			try
			{
				do
				{
					res = changelogs.GetChangeHistory(user, since_seq_num);

					db.SaveUserTracks(new UserTracks(res));
					since_seq_num = res.next_seq_num;

				} while (res.remaining_count > 0);
			}
			catch (WammerCloudException e)
			{
				if (e.WammerError == (int)UserTrackApiError.TooManyUserTracks)
				{
					int next_seq_num = RetrieveAllPostsBySeq(user, since_seq_num);
					UpdateDBForUserTrackBackFilled(user, next_seq_num);
					return;
				}
				else
					throw;
			}


			// Last user track response could contain unsynced posts.
			List<PostInfo> newPosts = postProvider.RetrievePosts(user, res.post_list.Select(x=>x.post_id).ToList());
			foreach (PostInfo post in newPosts)
				db.SavePost(post);

			OnPostsRetrieved(user, newPosts);

			UpdateDBForUserTrackBackFilled(user, res.next_seq_num);
		}

		private void UpdateDBForUserTrackBackFilled(Driver user, int next_seq_num)
		{
			SyncRange range = user.sync_range.Clone();
			range.next_seq_num = next_seq_num;
			db.UpdateDriverSyncRange(user.user_id, range);
			db.UpdateDriverChangeHistorySynced(user.user_id, true);
		}

		private int RetrieveAllPostsBySeq(Driver user, int since_seq_num)
		{
			PostResponse result = null;
			int since = since_seq_num;

			do
			{
				result = postProvider.GetPostsBySeq(user, since, 100);

				if (result.posts != null)
				{
					foreach (var post in result.posts)
					{
						db.SavePost(post);
						if (post.seq_num >= since)
							since = post.seq_num + 1;
					}

					OnPostsRetrieved(user, result.posts);
				}

			} while (result.HasMoreData);

			return since;
		}

		private static bool HasUnsyncedOldPosts(Driver user)
		{
			return !user.sync_range.first_post_time.HasValue;
		}

		private static bool HasNeverSynced(Driver user)
		{
			return user.sync_range == null || user.sync_range.start_time == DateTime.MinValue;
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