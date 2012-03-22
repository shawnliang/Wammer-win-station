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
		public bool HasMore { get; set; }
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
			
			string lastSyncTimeInDB = null;
			if (user.sync_range != null && !string.IsNullOrEmpty(user.sync_range.change_log_sync_time))
				lastSyncTimeInDB = user.sync_range.change_log_sync_time;

			string lastSyncTime = lastSyncTimeInDB;
			using (System.Net.WebClient agent = new System.Net.WebClient())
			{

				bool hasMoreData = false;
				List<string> changedPostIds = new List<string>();

				do
				{
					ChangeHistory changeHistory = postInfoProvider.RetrieveChangedPosts(agent,
						user.session_token, lastSyncTime, CloudServer.APIKey, user.groups[0].group_id);

					if (changeHistory.LastSyncTime == lastSyncTimeInDB)
						hasMoreData = false;
					else
					{
						changedPostIds.AddRange(changeHistory.ChangedPostIds);
						hasMoreData = changeHistory.HasMore;
						lastSyncTime = changeHistory.LastSyncTime;
					}
				}
				while (hasMoreData);


				if (changedPostIds.Count == 0)
					return new List<PostInfo>();

				List<PostInfo> finalResult = BatchGetPostInfo(user, agent, changedPostIds);			
				userInfoUpdator.UpdateChangeLogSyncTime(user.user_id, lastSyncTime);

				return finalResult;
			}
		}

		private List<PostInfo> BatchGetPostInfo(Driver user, System.Net.WebClient agent, List<string> changedPostIds)
		{
			List<PostInfo> finalResult = new List<PostInfo>();
			List<string> batch = new List<string>(100);

			foreach (string postId in changedPostIds)
			{
				batch.Add(postId);
				if (batch.Count == 100)
				{
					List<PostInfo> batchResult = postInfoProvider.RetrievePosts(agent, batch, user);
					finalResult.AddRange(batchResult);
					batch = new List<string>();
				}
			}

			if (batch.Count > 0)
			{
				List<PostInfo> batchResult = postInfoProvider.RetrievePosts(agent, batch, user);
				finalResult.AddRange(batchResult);
			}
			return finalResult;
		}
	}
}
