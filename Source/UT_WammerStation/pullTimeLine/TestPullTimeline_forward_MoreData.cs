using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wammer.Station.Timeline;
using Wammer.Cloud;
using Wammer.Model;
using System.Net;

namespace UT_WammerStation.pullTimeLine
{
	[TestClass]
	public class TestPullTimeline_forward_MoreData
	{

		class FakePostProvider : IPostProvider
		{
			public PostResponse GetLastestPosts(System.Net.WebClient agent, Driver user, int limit)
			{
				Assert.Fail("Should not come here");
				return null;
			}
			public PostResponse GetPostsBefore(System.Net.WebClient agent, Driver user, string before, int limit)
			{
				Assert.Fail("Should not come here");
				return null;
			}
			public List<PostInfo> RetrievePosts(System.Net.WebClient agent, Driver user, List<string> posts)
			{
				Assert.AreEqual(3, posts.Count);

				return new List<PostInfo>{
					new PostInfo{ post_id = "post1"},
					new PostInfo{ post_id = "post2"},
					new PostInfo{ post_id = "post3"}
				};
			}
		}

		class FakeUserTracksApi : IUserTrackApi
		{
			public int RemainCalledNum = 3;
			public UserTrackResponse GetChangeHistory(WebClient agent, Wammer.Model.Driver user, string since)
			{
				Assert.IsTrue(RemainCalledNum > 0);

				try
				{
					if (RemainCalledNum == 3)
					{
						return new UserTrackResponse
						{
							post_id_list = new List<string> { "post1" },
							latest_timestamp = DateTime.Now.AddDays(-2.0).ToString("u"),
							remaining_count = 2,
							get_count = 1
						};
					}
					else if (RemainCalledNum == 2)
					{
						return new UserTrackResponse
						{
							post_id_list = new List<string> { "post2" },
							latest_timestamp = DateTime.Now.AddDays(-1.0).ToString("u"),
							remaining_count = 1,
							get_count = 1
						};
					}
					else
					{
						return new UserTrackResponse
						{
							post_id_list = new List<string> { "post3" },
							latest_timestamp = DateTime.Now.AddDays(1.0).ToString("u"),
							remaining_count = 0,
							get_count =1
						};
					}
				}
				finally
				{
					RemainCalledNum--;
				}
			}

		}

		DummyTimelineSyncerDB db;
		FakePostProvider postProvider;
		FakeUserTracksApi userTrack;
		Driver user;

		[TestInitialize]
		public void SetUp()
		{
			db = new DummyTimelineSyncerDB();
			postProvider = new FakePostProvider();
			userTrack = new FakeUserTracksApi();

			user = new Driver
			{
				sync_range = new SyncRange
				{
					start_time = DateTime.Now.AddDays(-10.0).ToString("u"),
					end_time = DateTime.Now.ToString("u"),
					first_post_time = DateTime.Now.AddDays(-10.0).ToString("u")
				}
			};
		}

		[TestMethod]
		public void PullAllChangeHistoryUntilSyncEndTime()
		{
			TimelineSyncer syncer = new TimelineSyncer(postProvider, db, userTrack);
			syncer.PullForward(user);

			// verify the post whose update time is after sync_range.end_time is saved
			Assert.AreEqual(1, db.SavedPosts.Count);
			Assert.AreEqual("post3", db.SavedPosts[0].post_id);

			// verify Driver.change_log_sync_time is update
			Assert.AreEqual(user.user_id, db.UpdateChangeLogSyncTime_userId);
			Assert.IsTrue(db.UpdateChangeLogSyncTime_time.CompareTo(user.sync_range.end_time) > 0);


		}
	}
}
