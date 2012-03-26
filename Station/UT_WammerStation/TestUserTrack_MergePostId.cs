using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.Station;
using Wammer.Station.TimelineChange;
using System.Net;

namespace UT_WammerStation.MergePostId
{
	[TestClass]
	public class TestUserTrack
	{

		FakeUserInfoUpdator2 postInfoProvider = new FakeUserInfoUpdator2();
		FakeUserInfoUpdator22 userInfoUpdator = new FakeUserInfoUpdator22();
		Driver user;

		[TestInitialize]
		public void Setup()
		{
			user = new Driver
			{
				email = "user1@w.com",
				user_id = "user1_id",
				session_token = "user1_token",
				groups = new List<Wammer.Cloud.UserGroup> {
					new UserGroup() { group_id = "user1_group_id" }
				}
			};
		}

		[TestMethod]
		public void TestGetChangedPosts_DuplicatedPostIdIsResolved()
		{

			TimelineChangeHistory changeHistory = new TimelineChangeHistory(
				postInfoProvider, userInfoUpdator);

			List<PostInfo> posts = changeHistory.GetChangedPosts(user);
			Assert.AreEqual(1, posts.Count);
		}

	}

	class FakeUserInfoUpdator22 : IUserInfoUpdator
	{
		public string SavedTime { get; private set; }
		public string SavedUserId { get; private set; }

		public void UpdateChangeLogSyncTime(string userId, string time)
		{
			SavedTime = time;
			SavedUserId = userId;
		}
	}




	class FakeUserInfoUpdator2 : IPostInfoProvider
	{
		public string passedSessionToken { get; private set; }
		public string passedStartTime { get; private set; }
		public string passedGroupId { get; private set; }

		private int j = 0;

		public ChangeHistory RetrieveChangedPosts(WebClient agent, string session_token, string start_time, string apikey, string group_id)
		{
			ChangeHistory ret = new ChangeHistory {
				HasMore = false,
				ChangedPostIds = new List<string>(),
				LastSyncTime = "time1" 
			};

			ret.ChangedPostIds.Add("id");
			ret.ChangedPostIds.Add("id");
			ret.ChangedPostIds.Add("id");

			return ret;
		}

		public List<PostInfo> RetrievePosts(WebClient agent, List<string> posts, Driver user)
		{
			Assert.AreEqual(1, posts.Count);
			foreach (string postId in posts)
			{
				Assert.AreEqual("id", postId);
			}

			return new List<PostInfo> { new PostInfo { post_id = "id" } };
		}
	}
}
