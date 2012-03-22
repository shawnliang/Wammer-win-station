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

namespace UT_WammerStation.LotsOfPosts
{
	[TestClass]
	public class TestUserTrack_LotsOfPosts
	{
		public static int POST_COUNT;
		
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
		public void TestGetChangedPosts_RetrieveEvery100Posts_total500()
		{
			POST_COUNT = 500;

			TimelineChangeHistory changeHistory = new TimelineChangeHistory(
				postInfoProvider, userInfoUpdator);

			List<PostInfo> posts = changeHistory.GetChangedPosts(user);
			Assert.AreEqual(POST_COUNT, posts.Count);
			for (int i = 0; i < posts.Count; i++)
				Assert.AreEqual(i.ToString(), posts[i].post_id);
		}

		[TestMethod]
		public void TestGetChangedPosts_RetrieveEvery100Posts_total139()
		{
			POST_COUNT = 139;

			TimelineChangeHistory changeHistory = new TimelineChangeHistory(
				postInfoProvider, userInfoUpdator);

			List<PostInfo> posts = changeHistory.GetChangedPosts(user);
			Assert.AreEqual(POST_COUNT, posts.Count);
			for (int i = 0; i < posts.Count; i++)
				Assert.AreEqual(i.ToString(), posts[i].post_id);
		}

		[TestMethod]
		public void TestGetChangedPosts_RetrieveEvery100Posts_total39()
		{
			POST_COUNT = 39;

			TimelineChangeHistory changeHistory = new TimelineChangeHistory(
				postInfoProvider, userInfoUpdator);

			List<PostInfo> posts = changeHistory.GetChangedPosts(user);
			Assert.AreEqual(POST_COUNT, posts.Count);
			for (int i = 0; i < posts.Count; i++)
				Assert.AreEqual(i.ToString(), posts[i].post_id);
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
			ChangeHistory ret = new ChangeHistory { HasMore = false, ChangedPostIds = new List<string>(), LastSyncTime = "time1" };
			for (int i = 0; i < TestUserTrack_LotsOfPosts.POST_COUNT; i++)
			{
				ret.ChangedPostIds.Add(i.ToString());
			}

			return ret;
		}

		public List<PostInfo> RetrievePosts(WebClient agent, List<string> posts, Driver user)
		{
			Assert.IsTrue(posts.Count <= 100);

			List<PostInfo> ret = new List<PostInfo>();

			for (int k = 0; k < posts.Count; k++)
			{
				int postId = 100 * j + k;

				Assert.AreEqual(postId.ToString(), posts[k]);
				ret.Add(new PostInfo() {
					post_id = posts[k]
				});
			}

			j++;

			return ret;
		}
	}
}
