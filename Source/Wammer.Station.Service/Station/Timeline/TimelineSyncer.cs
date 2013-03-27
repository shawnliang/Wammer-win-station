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
		void UpdateUserPostNextSeq(string userId, int postNextSeq);
		void UpdateUserObjectNextTime(string userId, DateTime nextObjTime);
		void UpdateUserChangeLogNextSeq(string userId, int nextSeq);
	}

	public class TimelineSyncerDB : ITimelineSyncerDB
	{
		public void UpdateDriverSyncRange(string userId, SyncRange syncRange)
		{
			DriverCollection.Instance.Update(
				Query.EQ("_id", userId),
				Update.Set("sync_range", syncRange.ToBsonDocument()));
		}

		public void UpdateUserPostNextSeq(string userId, int postNextSeq)
		{
			DriverCollection.Instance.Update(
				Query.EQ("_id", userId),
				Update.Set("sync_range.post_next_seq", postNextSeq));
		}

		public void UpdateUserObjectNextTime(string userId, DateTime nextObjTime)
		{
			DriverCollection.Instance.Update(
				Query.EQ("_id", userId),
				Update.Set("sync_range.obj_next_time", nextObjTime));
		}

		public void UpdateUserChangeLogNextSeq(string userId, int nextSeq)
		{
			DriverCollection.Instance.Update(
				Query.EQ("_id", userId),
				Update.Set("sync_range.change_log_next_seq", nextSeq));
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
		public event EventHandler<AttachmentDeleteEventArgs> AttachmentDelete;

		public TimelineSyncer()
		{
			PostProvider = new PostProvicer();
			ChangeLogProvider = new ChangeLogsApi();
			DB = new TimelineSyncerDB();
		}

		public bool PullTimeline(Driver user)
		{
			bool hasNewEvents = false;
			bool hasNewAttachments = false;
			SyncRange syncRange = user.sync_range != null ? user.sync_range : new SyncRange();


			int postNextSeq;
			pullNewEvents(user, out hasNewEvents, out postNextSeq);
			syncRange.post_next_seq = postNextSeq;

			pullNewAttachments(user, out hasNewAttachments, syncRange.obj_next_time);

			if (syncRange.change_log_next_seq == 0)
			{
				syncRange.change_log_next_seq = syncRange.post_next_seq;
				DB.UpdateUserChangeLogNextSeq(user.user_id, syncRange.change_log_next_seq);
			}
			PullChangeLog(user, syncRange.change_log_next_seq);

			return hasNewEvents || hasNewAttachments;
		}

		private void PullChangeLog(Driver user, int changeLogNextSeq)
		{
			try
			{
				var hasNewChangeLog = false;
				var next_seq = changeLogNextSeq;
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
										OnAttachmentDelete(new AttachmentDeleteEventArgs() { attachmentIDs = new string[] { change.target_id }, user_id = user.user_id });
									}
								}
							}
						}
						hasNewChangeLog = true;
					}

				} while (has_more);

				if (hasNewChangeLog)
				{
					DB.UpdateUserChangeLogNextSeq(user.user_id, next_seq);
				}
			}
			catch (Exception)
			{
			}
		}

		private void pullNewAttachments(Driver user, out bool hasNewAttachments, DateTime objNextTime)
		{
			hasNewAttachments = false;

			var since = objNextTime;
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
				DB.UpdateUserObjectNextTime(user.user_id, until);
			}
		}

		private void pullNewEvents(Driver user, out bool hasNewEvents, out int postNextSeq)
		{
			hasNewEvents = false;
			var next_seq = 0;

			if (user.sync_range != null)
				next_seq = user.sync_range.post_next_seq;

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
				DB.UpdateUserPostNextSeq(user.user_id, next_seq);
			}

			postNextSeq = next_seq;
		}

		private void OnAttachmentDelete(AttachmentDeleteEventArgs e)
		{
			if (AttachmentDelete == null)
				return;
			AttachmentDelete(this, e);
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

	public class AttachmentDeleteEventArgs : EventArgs
	{
		public IEnumerable<string> attachmentIDs { get; set; }
		public string user_id { get; set; }
	}
}