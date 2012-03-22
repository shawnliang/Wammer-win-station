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

namespace UT_WammerStation
{
	[TestClass]
	public class TestUserTrack_HasMore
	{
		[TestMethod]
		public void TestGetChangedPosts()
		{
			Driver user = new Driver
			{
				email = "user1@w.com",
				user_id = "user1_id",
				session_token = "user1_token",
				groups = new List<Wammer.Cloud.UserGroup> {
					new UserGroup() { group_id = "user1_group_id" }
				}
			};

			FakeUserInfoUpdator2 postInfoProvider = new FakeUserInfoUpdator2();
			FakeUserInfoUpdator22 userInfoUpdator = new FakeUserInfoUpdator22();

			TimelineChangeHistory changeHistory = new TimelineChangeHistory(
				postInfoProvider, userInfoUpdator);

			List<PostInfo> posts = changeHistory.GetChangedPosts(user);

			Assert.AreEqual(3, posts.Count);
			Assert.AreEqual("0", posts[0].post_id);
			Assert.AreEqual("1", posts[1].post_id);
			Assert.AreEqual("2", posts[2].post_id);

			Assert.AreEqual("time2", userInfoUpdator.SavedTime);
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

		private int i = 0;

		public ChangeHistory RetrieveChangedPosts(WebClient agent, string session_token, string start_time, string apikey, string group_id)
		{
			passedGroupId = group_id;
			passedSessionToken = session_token;
			passedStartTime = start_time;

			ChangeHistory ret = new ChangeHistory
			{
				ChangedPostIds = new List<string>() { i.ToString() },
				LastSyncTime = "time" + i.ToString(),
				HasMore = true
			};

			++i;

			if (i == 1)
				Assert.AreEqual(null, start_time);
			else if (i==2)
				Assert.AreEqual("time0", start_time);
			else if (i == 3)
			{
				Assert.AreEqual("time1", start_time);
				ret.HasMore = false;
			}

			return ret;
		}

		public List<PostInfo> RetrievePosts(WebClient agent, List<string> posts, Driver user)
		{
			Assert.AreEqual(3, posts.Count);
			Assert.AreEqual("0", posts[0]);
			Assert.AreEqual("1", posts[1]);
			Assert.AreEqual("2", posts[2]);

			return new List<PostInfo> {
						 new PostInfo { post_id = "0"},
						 new PostInfo { post_id = "1"},
						 new PostInfo { post_id = "2"}
			};
		}
	}
}
