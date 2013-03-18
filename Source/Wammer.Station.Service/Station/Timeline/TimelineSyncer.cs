using MongoDB.Bson;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Web;
using Wammer.Cloud;
using Waveface.Stream.Model;

namespace Wammer.Station.Timeline
{

	public interface ITimelineSyncerDB
	{
		void UpdateDriverSyncRange(string userId, SyncRange syncRange);
	}

	public class TimelineSyncerDB : ITimelineSyncerDB
	{
		public void UpdateDriverSyncRange(string userId, SyncRange syncRange)
		{
			DriverCollection.Instance.Update(
				Query.EQ("_id", userId),
				Update.Set("sync_range", syncRange.ToBsonDocument()));
		}
	}

	public interface IPostProvider
	{
		PostFetchBySeqResponse GetPostsBySeq(Driver user, int seq, int limit);
	}

	public class PostProvicer : IPostProvider
	{
		public PostFetchBySeqResponse GetPostsBySeq(Driver user, int seq, int limit)
		{
			var api = new PostApi(user);
			return api.PostFetchBySeq(seq, limit);
		}
	}

	public class TimelineSyncer
	{
		private IPostProvider PostProvider { get; set; }
		private IChangeLogsApi ChangeLogProvider { get; set; }
		private ITimelineSyncerDB DB { get; set; }

		public event EventHandler<TimelineSyncEventArgs> PostsRetrieved;
		public event EventHandler<AttachmentModifiedEventArgs> AttachmentModified;
		public event EventHandler<AttachmentHideEventArgs> AttachmentHided;

		public TimelineSyncer()
		{
			PostProvider = new PostProvicer();
			ChangeLogProvider = new ChangeLogsApi();
			DB = new TimelineSyncerDB();
		}

		public bool PullTimeline(Driver user)
		{
			bool hasNewEvents;
			bool hasNewAttachments;
			SyncRange syncRange;

			pullNewEvents(user, out hasNewEvents, out syncRange);

			pullNewAttachments(user, out hasNewAttachments, syncRange);

			return hasNewEvents || hasNewAttachments;
		}

		private void PullChangeLog(Driver user, out bool hasNewChangeLog, SyncRange syncRange)
		{
			hasNewChangeLog = false;
			syncRange = new SyncRange();

			if (user.sync_range != null)
				syncRange = user.sync_range;

			var next_seq = syncRange.change_log_next_seq;
			var has_more = false;
			do
			{
				var result = ChangeLogProvider.GetChangeHistory(user, next_seq);

				has_more = result.remaining_count > 0;
				next_seq = result.next_seq_num;

				if (result.changelog_list != null && result.changelog_list.Count > 0)
				{
					foreach (var change in result.changelog_list)
					{
						if (change.target_type == "attachment")
						{
							foreach (var action in change.actions)
							{
								if (action.action == "delete")
								{
									OnAttachmentHided(new AttachmentHideEventArgs() { attachmentIDs = action.target_id_list, user_id = user.user_id});
									hasNewChangeLog = true;
								}
							}
						}
					}
				}

			} while (has_more);

			if (hasNewChangeLog)
			{
				syncRange.change_log_next_seq = next_seq;
				DB.UpdateDriverSyncRange(user.user_id, syncRange);
			}
		}

		private void pullNewAttachments(Driver user, out bool hasNewAttachments, SyncRange syncRange)
		{
			hasNewAttachments = false;

			if (syncRange == null)
				syncRange = new SyncRange();

			var since = syncRange.obj_next_time;
			var until = DateTime.Now.TrimToSec();
			var start = 0;
			var count = 100;

			var hasMoreData = false;

			do
			{
				AttachmentSearchResult result = AttachmentApi.Search(user.session_token, CloudServer.APIKey, since, until, count, start);
				if (result.results != null && result.results.Count > 0)
				{
					OnAttachmentModified(user, result.results);
					hasNewAttachments = true;
				}

				hasMoreData = !string.IsNullOrEmpty(result.next_page);


				if (hasMoreData)
				{
					var paras = HttpUtility.ParseQueryString(result.next_page);
					start = Convert.ToInt32(paras["start"]);
					count = Convert.ToInt32(paras["count"]);
				}

			} while (hasMoreData);

			if (hasNewAttachments)
			{
				syncRange.obj_next_time = until;
				DB.UpdateDriverSyncRange(user.user_id, syncRange);
			}
		}

		private void pullNewEvents(Driver user, out bool hasNewEvents, out SyncRange syncRange)
		{
			hasNewEvents = false;
			syncRange = new SyncRange();

			if (user.sync_range != null)
				syncRange = user.sync_range;

			var next_seq = syncRange.post_next_seq;
			var has_more = false;
			do
			{
				var result = PostProvider.GetPostsBySeq(user, next_seq, 50);

				has_more = result.remaining_count > 0;
				next_seq = result.next_datum;

				if (result.posts != null)
				{
					OnPostsRetrieved(user, result.posts);
					hasNewEvents = true;
				}

			} while (has_more);

			if (hasNewEvents)
			{
				syncRange.post_next_seq = next_seq;
				DB.UpdateDriverSyncRange(user.user_id, syncRange);
			}
		}

		private void OnAttachmentHided(AttachmentHideEventArgs e)
		{
			if (AttachmentHided == null)
				return;
			AttachmentHided(this, e);
		}

		private void OnAttachmentModified(Driver user, List<AttachmentInfo> atts)
		{
			EventHandler<AttachmentModifiedEventArgs> handler = AttachmentModified;
			if (handler != null)
				handler(this, new AttachmentModifiedEventArgs { attachments = atts, user_id = user.user_id });
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

	public class AttachmentModifiedEventArgs : EventArgs
	{
		public List<AttachmentInfo> attachments { get; set; }
		public string user_id { get; set; }
	}

	public class AttachmentHideEventArgs : EventArgs
	{
		public IEnumerable<string> attachmentIDs { get; set; }
		public string user_id { get; set; }
	}
}