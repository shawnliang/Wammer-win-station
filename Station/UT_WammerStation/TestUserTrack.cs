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
	public class TestUserTrack
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

				//No sync_range
			};

			FakePostInfoProvider postInfoProvider = new FakePostInfoProvider();
			FakeUserInfoUpdator  userInfoUpdator = new FakeUserInfoUpdator();

			TimelineChangeHistory changeHistory = new TimelineChangeHistory(
				postInfoProvider, userInfoUpdator);

			List<PostInfo> posts = changeHistory.GetChangedPosts(user);

			Assert.AreEqual(user.groups[0].group_id, postInfoProvider.passedGroupId);
			Assert.AreEqual(user.session_token, postInfoProvider.passedSessionToken);
			Assert.AreEqual(null, postInfoProvider.passedStartTime);

			Assert.AreEqual(2, posts.Count);

			Assert.AreEqual("2000-10-10T01:02:03Z", userInfoUpdator.SavedTime);
			Assert.AreEqual(user.user_id, userInfoUpdator.SavedUserId);
		}

		[TestMethod]
		public void TestGetChangedPosts_lastSyncTimeDoesNotExist()
		{
			Driver user = new Driver
			{
				email = "user1@w.com",
				user_id = "user1_id",
				session_token = "user1_token",
				groups = new List<Wammer.Cloud.UserGroup> {
					new UserGroup() { group_id = "user1_group_id" }
				},
				sync_range = new SyncRange() //No sync_range.changeLogSyncTime
			};

			FakePostInfoProvider postInfoProvider = new FakePostInfoProvider();
			FakeUserInfoUpdator userInfoUpdator = new FakeUserInfoUpdator();

			TimelineChangeHistory changeHistory = new TimelineChangeHistory(
				postInfoProvider, userInfoUpdator);

			List<PostInfo> posts = changeHistory.GetChangedPosts(user);

			Assert.AreEqual(user.groups[0].group_id, postInfoProvider.passedGroupId);
			Assert.AreEqual(user.session_token, postInfoProvider.passedSessionToken);
			Assert.AreEqual(null, postInfoProvider.passedStartTime);

			Assert.AreEqual(2, posts.Count);

			Assert.AreEqual("2000-10-10T01:02:03Z", userInfoUpdator.SavedTime);
			Assert.AreEqual(user.user_id, userInfoUpdator.SavedUserId);
		}

		[TestMethod]
		public void TestGetChangedPosts_lastSyncTimeExist()
		{
			Driver user = new Driver
			{
				email = "user1@w.com",
				user_id = "user1_id",
				session_token = "user1_token",
				groups = new List<Wammer.Cloud.UserGroup> {
					new UserGroup() { group_id = "user1_group_id" }
				},
				sync_range = new SyncRange() {  change_log_sync_time = "2010-03-04T05:06:07Z"}
			};

			FakePostInfoProvider postInfoProvider = new FakePostInfoProvider();
			FakeUserInfoUpdator userInfoUpdator = new FakeUserInfoUpdator();

			TimelineChangeHistory changeHistory = new TimelineChangeHistory(
				postInfoProvider, userInfoUpdator);

			List<PostInfo> posts = changeHistory.GetChangedPosts(user);

			Assert.AreEqual(user.groups[0].group_id, postInfoProvider.passedGroupId);
			Assert.AreEqual(user.session_token, postInfoProvider.passedSessionToken);
			Assert.AreEqual(user.sync_range.change_log_sync_time, postInfoProvider.passedStartTime);

			Assert.AreEqual(2, posts.Count);
			Assert.AreEqual("2000-10-10T01:02:03Z", userInfoUpdator.SavedTime);
			Assert.AreEqual(user.user_id, userInfoUpdator.SavedUserId);
		}

		[TestMethod]
		public void TestGetChangedPosts_lastSyncTimeIsTheSame_ReturnNothing()
		{
			Driver user = new Driver
			{
				email = "user1@w.com",
				user_id = "user1_id",
				session_token = "user1_token",
				groups = new List<Wammer.Cloud.UserGroup> {
					new UserGroup() { group_id = "user1_group_id" }
				},
				sync_range = new SyncRange() { change_log_sync_time = "2000-10-10T01:02:03Z" }
			};

			FakePostInfoProvider postInfoProvider = new FakePostInfoProvider();
			FakeUserInfoUpdator userInfoUpdator = new FakeUserInfoUpdator();

			TimelineChangeHistory changeHistory = new TimelineChangeHistory(
				postInfoProvider, userInfoUpdator);

			List<PostInfo> posts = changeHistory.GetChangedPosts(user);

			Assert.AreEqual(0, posts.Count);
		}
	}


	class FakeUserInfoUpdator : IUserInfoUpdator
	{
		public string SavedTime { get; private set; }
		public string SavedUserId { get; private set; }

		public void UpdateChangeLogSyncTime(string userId, string time)
		{
			SavedTime = time;
			SavedUserId = userId;
		}
	}


	

	class FakePostInfoProvider : IPostInfoProvider
	{
		public string passedSessionToken { get; private set;}
		public string passedStartTime { get; private set; }
		public string passedGroupId { get; private set; }

		public ChangeHistory RetrieveChangedPosts(WebClient agent, string session_token, string start_time, string apikey, string group_id)
		{
			passedGroupId = group_id;
			passedSessionToken = session_token;
			passedStartTime = start_time;

			return new ChangeHistory
			{
				ChangedPostIds = new List<string>() { "1", "2" },
				LastSyncTime = "2000-10-10T01:02:03Z"
			};
		}

		public List<PostInfo> RetrievePosts(WebClient agent, List<string> posts, Driver user)
		{
			return new List<PostInfo> {
						 new PostInfo {
							 post_id = "post1"
						 },
						 new PostInfo {
							 post_id = "post2"
						 }
			};
		}
	}
}
