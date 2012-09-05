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
		public event EventHandler<AttachmentAvailableEventArgs> AttachmentAvailable;

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
		public bool PullForward(Driver user)
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

				return res.post_list != null && res.post_list.Count > 0;
			}
			catch(WammerCloudException e)
			{
				if (changeLogsNotAvailable(e))
				{
					int next_seq = RetrieveAllPostsBySeq(user, user.sync_range.next_seq_num);
					//UpdateDBForUserTrackBackFilled(user, next_seq);
					
					var syncRange = user.sync_range.Clone();
					syncRange.chlog_max_seq = syncRange.chlog_min_seq = int.MaxValue;
					syncRange.next_seq_num = next_seq;

					db.UpdateDriverSyncRange(user.user_id, syncRange);

					return true;
				}
				else
					throw;
			}
		}

		private static bool changeLogsNotAvailable(WammerCloudException e)
		{
			return	e.WammerError == (int)UserTrackApiError.SeqNumPurged ||
					e.WammerError == (int)UserTrackApiError.TooManyRecord;
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
												chlog_min_seq = user.sync_range.chlog_min_seq == int.MaxValue ? res.changelog_list.Where(x => x.seq_num > 0).Min(x => x.seq_num) : user.sync_range.chlog_min_seq,
												chlog_max_seq = res.changelog_list.Max(x => x.seq_num)
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
						OnAttachmentAvailable(new AttachmentAvailableEventArgs(track.target_id, track.user_id, track.group_id, ImageMeta.Origin));
					}
					else if (track.actions.Any(action => action.target_type == "image.medium"))
					{
						OnAttachmentAvailable(new AttachmentAvailableEventArgs(track.target_id, track.user_id, track.group_id, ImageMeta.Medium));
					}
				}
			}
		}

		private void OnAttachmentAvailable(AttachmentAvailableEventArgs args)
		{
			EventHandler<AttachmentAvailableEventArgs> handler = AttachmentAvailable;
			if (handler != null)
				handler(this, args);
		}

		/// <summary>
		/// Pull user timeline
		/// </summary>
		/// <param name="user"></param>
		/// <returns>Returns true if a post change is received; otherwise false</returns>
		public bool PullTimeline(Driver user)
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
				return PullForward(user);

			// Always return true for PullBackward() and PullOldChangeLog() because it will prevent data leakage
			return true;
		}

		private static int min(int a, int b)
		{
			return a < b ? a : b;
		}

		private static int max(int a, int b)
		{
			return a > b ? a : b;
		}

		private void PullOldChangeLog(Driver user)
		{
			int since_seq_num = 1;
			ChangeLogResponse res = new ChangeLogResponse { post_list = new List<PostListItem>() };

			int min_chlog_seq = int.MaxValue;
			int max_chlog_seq = int.MinValue;

			try
			{
				do
				{
					res = changelogs.GetChangeHistory(user, since_seq_num);

					db.SaveUserTracks(new UserTracks(res));
					since_seq_num = res.next_seq_num;

					if (res.changelog_list != null && res.changelog_list.Count > 0)
					{
						int min_local = res.changelog_list.Max(x => x.seq_num);
						int max_local = res.changelog_list.Min(x => x.seq_num);

						min_chlog_seq = min(min_local, min_chlog_seq);
						max_chlog_seq = max(max_local, max_chlog_seq);
					}

				} while (res.remaining_count > 0);
			}
			catch (WammerCloudException e)
			{
				if (changeLogsNotAvailable(e))
				{
					int next_seq_num = RetrieveAllPostsBySeq(user, since_seq_num);
					updateSyncRangeInDB(user, int.MaxValue, int.MaxValue, next_seq_num);
					db.UpdateDriverChangeHistorySynced(user.user_id, true);
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

			updateSyncRangeInDB(user, min_chlog_seq, max_chlog_seq, res.next_seq_num);
			db.UpdateDriverChangeHistorySynced(user.user_id, true);
		}

		private void updateSyncRangeInDB(Driver user, int chlog_min, int chlog_max, int next_seq)
		{
			var sync_range = user.sync_range.Clone();
			sync_range.next_seq_num = next_seq;
			sync_range.chlog_max_seq = chlog_max;
			sync_range.chlog_min_seq = chlog_min;
			db.UpdateDriverSyncRange(user.user_id, sync_range);
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

	public class AttachmentAvailableEventArgs : EventArgs
	{
		public AttachmentAvailableEventArgs(string object_id, string user_id, string group_id, ImageMeta meta)
		{
			this.object_id = object_id;
			this.user_id = user_id;
			this.group_id = group_id;
			this.meta = meta;
		}

		public string object_id { get; private set; }
		public string user_id { get; private set; }
		public string group_id { get; private set; }
		public ImageMeta meta { get; private set; }
	}
}