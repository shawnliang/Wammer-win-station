using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wammer.Cloud;
using Wammer.Model;

namespace Wammer.Station.TimelineChange
{
	public interface IUserInfoUpdator
	{
		void UpdateChangeLogSyncTime(string user_id, string time);
	}

	public interface IPostInfoProvider
	{
		ChangeHistory RetrieveChangedPosts(System.Net.WebClient agent, string session_token, string start_time, string apikey, string group_id);
		List<PostInfo> RetrievePosts(System.Net.WebClient agent, List<string> posts, Driver user);
	}

	public class ChangeHistory
	{
		public List<string> ChangedPostIds { get; set; }
		public string LastSyncTime {get;set;}
	}

	public class TimelineChangeHistory
	{
		private IPostInfoProvider postInfoProvider;
		private IUserInfoUpdator userInfoUpdator;

		public TimelineChangeHistory(IPostInfoProvider postInfoProvider, IUserInfoUpdator userInfoUpdator)
		{
			this.postInfoProvider = postInfoProvider;
			this.userInfoUpdator = userInfoUpdator;
		}

		public List<PostInfo> GetChangedPosts(Driver user)
		{
			string lastSyncTime = null;
			if (user.sync_range != null && !string.IsNullOrEmpty(user.sync_range.change_log_sync_time))
				lastSyncTime = user.sync_range.change_log_sync_time;

			using (System.Net.WebClient agent = new System.Net.WebClient())
			{
				ChangeHistory changeHistory =
					postInfoProvider.RetrieveChangedPosts(agent, user.session_token, lastSyncTime, CloudServer.APIKey, user.groups[0].group_id);

				if (changeHistory == null ||
					changeHistory.ChangedPostIds == null ||
					changeHistory.ChangedPostIds.Count == 0)
					return new List<PostInfo>();

				List<PostInfo> changedPosts = postInfoProvider.RetrievePosts(agent, changeHistory.ChangedPostIds, user);

				userInfoUpdator.UpdateChangeLogSyncTime(user.user_id, changeHistory.LastSyncTime);

				return changedPosts;
			}
		}
	}
}
