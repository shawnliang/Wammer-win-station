using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.Station.Timeline;
using Wammer.Utility;
using Waveface.Stream.Model;

namespace UT_WammerStation.pullTimeLine
{
	class DummyPostInfoProvider : IPostProvider
	{
		// saved params and return values of GetLastestPosts
		public Driver GetLastestPosts_user { get; private set; }
		public System.Net.WebClient GetLastestPosts_agent { get; private set; }
		public int GetLastestPosts_limit { get; private set; }
		public PostResponse GetLastestPosts_return { get; set; }

		// saved params and return values of GetPostsBefore
		public System.Net.WebClient GetPostsBefore_agent { get; private set; }
		public Driver GetPostsBefore_user { get; private set; }
		public DateTime GetPostsBefore_before { get; private set; }
		public int GetPostsBefore_limit { get; private set; }
		public PostResponse GetPostsBefore_return { get; set; }

		// RetrievePosts
		public Driver RetrievePosts_user { get; private set; }
		public List<string> RetrievePosts_posts { get; private set; }
		public List<PostInfo> RetrievePosts_return { get; set; }
		public PostResponse GetLastestPosts(Driver user, int limit)
		{
			GetLastestPosts_user = user;
			GetLastestPosts_agent = new DefaultWebClient();
			GetLastestPosts_limit = limit;
			return GetLastestPosts_return;
		}

		public PostResponse GetPostsBefore(Driver user, DateTime before, int limit)
		{
			this.GetPostsBefore_agent = new DefaultWebClient();
			this.GetPostsBefore_user = user;
			this.GetPostsBefore_limit = limit;
			this.GetPostsBefore_before = before;

			return this.GetPostsBefore_return;
		}

		public List<PostInfo> RetrievePosts(Driver user, List<string> posts)
		{
			this.RetrievePosts_user = user;
			this.RetrievePosts_posts = posts;

			return this.RetrievePosts_return;
		}


		public PostResponse GetPostsBySeq(Driver user, int seq, int limit)
		{
			throw new NotImplementedException();
		}
	}

	class DummyTimelineSyncerDB : ITimelineSyncerDB
	{
		public List<PostInfo> SavedPosts { get; private set; }

		// UpdateChangeLogSyncTime
		public string UpdateChangeLogSyncTime_userId { get; set; }
		public bool UpdateChangeLogSyncTime_synced { get; set; }

		// UpdateSyncRange
		public string UpdateSyncRange_userId { get; set; }
		public SyncRange UpdateSyncRange_syncRange { get; set; }



		public DummyTimelineSyncerDB()
		{
			SavedPosts = new List<PostInfo>();
		}

		public void SavePost(PostInfo post)
		{
			SavedPosts.Add(post);
		}

		public void SaveUserTracks(UserTracks ut)
		{
		}

		public void UpdateDriverSyncRange(string userId, SyncRange syncRange)
		{
			UpdateSyncRange_userId = userId;
			UpdateSyncRange_syncRange = syncRange;
		}

		public void UpdateDriverChangeHistorySynced(string userId, bool synced)
		{
			UpdateChangeLogSyncTime_synced = synced;
			UpdateChangeLogSyncTime_userId = userId;
		}
	}

	[TestClass]
	public class TestPullTimeline_backward
	{
		ICollection<PostInfo> RetrievedPosts;
		[TestInitialize]
		public void Setup()
		{
			RetrievedPosts = null;
		}

		[TestMethod]
		public void GetLatestPostsIfFirstlyGettingTimeline_HasNoMoreData()
		{
			DummyPostInfoProvider postInfoProvider = new DummyPostInfoProvider();
			postInfoProvider.GetLastestPosts_return = new PostGetLatestResponse
			{
				get_count = 1,
				total_count = 1,
				group_id = "group1",
				posts = new List<PostInfo> {
							new PostInfo { timestamp =  new DateTime(2012,1,2,3,4,5, DateTimeKind.Utc), post_id = "post1" }
				}
			};

			Driver user = new Driver
			{
				session_token = "session1",
				sync_range = null,	// firstly getting timeline
				groups = new List<UserGroup> {
						 					   new UserGroup { group_id = "group1" }
						 },
				user_id = "user1"
			};

			DummyTimelineSyncerDB db = new DummyTimelineSyncerDB();
			TimelineSyncer timelineSyncer = new TimelineSyncer(postInfoProvider, db, new ChangeLogsApi());
			timelineSyncer.PullBackward(user);

			// verify posts/getLatest cloud API is queried
			Assert.AreEqual(user, postInfoProvider.GetLastestPosts_user);
			Assert.IsNotNull(postInfoProvider.GetLastestPosts_agent);
			Assert.AreEqual(200, postInfoProvider.GetLastestPosts_limit);

			// verify retrieved posts are saved
			Assert.AreEqual(1, db.SavedPosts.Count);
			Assert.AreEqual(postInfoProvider.GetLastestPosts_return.posts.First(), db.SavedPosts[0]);

			// verify driver's sync range is updated
			Assert.AreEqual(postInfoProvider.GetLastestPosts_return.posts.First().timestamp, db.UpdateSyncRange_syncRange.start_time);
			Assert.AreEqual(postInfoProvider.GetLastestPosts_return.posts.First().timestamp, db.UpdateSyncRange_syncRange.first_post_time);
		}

		[TestMethod]
		public void GetLatestPostsIfFirstlyGettingTimeline_HasMoreData()
		{
			DummyPostInfoProvider postInfoProvider = new DummyPostInfoProvider();
			postInfoProvider.GetLastestPosts_return = new PostGetLatestResponse
			{
				get_count = 1,
				total_count = 10, // there are more data
				group_id = "group1",
				posts = new List<PostInfo> {
							new PostInfo { timestamp = new DateTime(2012,1,2,3,4,5, DateTimeKind.Utc), post_id = "post1" }
				}
			};

			Driver user = new Driver
			{
				session_token = "session1",
				sync_range = null,	// firstly getting timeline
				groups = new List<UserGroup> {
						 					   new UserGroup { group_id = "group1" }
						 },
				user_id = "user1"
			};

			DummyTimelineSyncerDB db = new DummyTimelineSyncerDB();
			TimelineSyncer timelineSyncer = new TimelineSyncer(postInfoProvider, db, new ChangeLogsApi());
			timelineSyncer.PullBackward(user);

			// verify driver's sync range is updated
			Assert.AreEqual(postInfoProvider.GetLastestPosts_return.posts.First().timestamp, db.UpdateSyncRange_syncRange.start_time);
			Assert.IsFalse(db.UpdateSyncRange_syncRange.first_post_time.HasValue);
		}

		[TestMethod]
		public void PullTimelineFromPrevTimestamp_HasMoreData()
		{
			DummyPostInfoProvider postInfoProvider = new DummyPostInfoProvider();

			PostInfo oldestPost = new PostInfo { timestamp = new DateTime(2012, 2, 3, 3, 4, 5, DateTimeKind.Utc), post_id = "post1" };

			postInfoProvider.GetPostsBefore_return = new PostFetchByFilterResponse
			{
				get_count = 1,
				remaining_count = 10,
				group_id = "group1",
				posts = new List<PostInfo> { oldestPost }
			};

			Driver user = new Driver
			{
				session_token = "session1",
				sync_range = new SyncRange
				{
					start_time = new DateTime(2012, 2, 1, 0, 0, 0, DateTimeKind.Utc),
				},
				groups = new List<UserGroup> {
						 					   new UserGroup { group_id = "group1" }
						 },
				user_id = "user1"
			};

			DummyTimelineSyncerDB db = new DummyTimelineSyncerDB();
			TimelineSyncer timelineSyncer = new TimelineSyncer(postInfoProvider, db, new ChangeLogsApi());
			timelineSyncer.PullBackward(user);

			//verify call get posts from prev's timestamp
			Assert.IsNotNull(postInfoProvider.GetPostsBefore_agent);
			Assert.AreEqual(user.sync_range.start_time, postInfoProvider.GetPostsBefore_before);
			Assert.AreEqual(200, postInfoProvider.GetPostsBefore_limit);
			Assert.AreEqual(user, postInfoProvider.GetPostsBefore_user);

			// verify returned posts are saved
			Assert.AreEqual(1, db.SavedPosts.Count);
			Assert.AreEqual(oldestPost, db.SavedPosts[0]);

			// verify new start_timestamp is updated
			Assert.AreEqual(user.user_id, db.UpdateSyncRange_userId);
			Assert.AreEqual(oldestPost.timestamp, db.UpdateSyncRange_syncRange.start_time);
			Assert.IsFalse(db.UpdateSyncRange_syncRange.first_post_time.HasValue);
		}

		[TestMethod]
		public void PullTimelineFromPrevTimestamp_NoMoreData()
		{
			Driver user = new Driver
			{
				session_token = "session1",
				sync_range = new SyncRange
				{
					start_time = new DateTime(2012, 1, 1, 0, 0, 0, DateTimeKind.Utc),
				},
				groups = new List<UserGroup> {
						 					   new UserGroup { group_id = "group1" }
						 },
				user_id = "user1"
			};

			Moq.Mock<IPostProvider> postInfo = new Moq.Mock<IPostProvider>(Moq.MockBehavior.Strict);

			PostInfo oldestPost = new PostInfo
			{
				timestamp = new DateTime(2012, 2, 3, 3, 4, 5, DateTimeKind.Utc),
				post_id = "post1"
			};

			postInfo.Setup(x => x.GetPostsBefore(user, user.sync_range.start_time, Moq.It.IsAny<int>()))
				.Returns(new PostFetchByFilterResponse
						{
							get_count = 1,
							remaining_count = 0,
							group_id = "group1",
							posts = new List<PostInfo> { oldestPost }
						})
				.Verifiable();

			Moq.Mock<ITimelineSyncerDB> db = new Moq.Mock<ITimelineSyncerDB>(Moq.MockBehavior.Strict);
			db.Setup(x => x.UpdateDriverSyncRange(user.user_id,
				new SyncRange
				{
					start_time = oldestPost.timestamp,
					first_post_time = oldestPost.timestamp
				}))
				.Verifiable();
			db.Setup(x => x.SavePost(oldestPost)).Verifiable();

			TimelineSyncer timelineSyncer = new TimelineSyncer(postInfo.Object, db.Object, new ChangeLogsApi());
			timelineSyncer.PullBackward(user);

			db.VerifyAll();
			postInfo.VerifyAll();
		}

		[TestMethod]
		public void FirePostRetrievedEvent()
		{
			DummyPostInfoProvider postInfoProvider = new DummyPostInfoProvider();

			PostInfo oldestPost = new PostInfo { timestamp = new DateTime(2012, 2, 3, 3, 4, 5, DateTimeKind.Utc), post_id = "post1" };

			postInfoProvider.GetPostsBefore_return = new PostFetchByFilterResponse
			{
				get_count = 1,
				remaining_count = 0,
				group_id = "group1",
				posts = new List<PostInfo> { oldestPost }
			};

			Driver user = new Driver
			{
				session_token = "session1",
				sync_range = new SyncRange
				{
					start_time = new DateTime(2012, 2, 1, 0, 0, 0, DateTimeKind.Utc)
				},
				groups = new List<UserGroup> {
						 					   new UserGroup { group_id = "group1" }
						 },
				user_id = "user1"
			};

			DummyTimelineSyncerDB db = new DummyTimelineSyncerDB();
			TimelineSyncer timelineSyncer = new TimelineSyncer(postInfoProvider, db, new ChangeLogsApi());
			timelineSyncer.PostsRetrieved += new EventHandler<TimelineSyncEventArgs>(timelineSyncer_PostsRetrieved);
			timelineSyncer.PullBackward(user);

			// verify retrieved posts are callbacked
			Assert.AreEqual(postInfoProvider.GetPostsBefore_return.posts, RetrievedPosts);
		}

		void timelineSyncer_PostsRetrieved(object sender, TimelineSyncEventArgs e)
		{
			RetrievedPosts = e.Posts;
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void ErrorIfUserHasAlreadyGottenAllOldTimeline()
		{
			DummyPostInfoProvider postInfoProvider = new DummyPostInfoProvider();

			Driver user = new Driver
			{
				session_token = "session1",
				sync_range = new SyncRange
				{
					start_time = new DateTime(2012, 2, 1, 0, 0, 0, DateTimeKind.Utc),
					first_post_time = new DateTime(2012, 2, 1, 0, 0, 0, DateTimeKind.Utc)
				},
				groups = new List<UserGroup> { new UserGroup { group_id = "group1" } },
				user_id = "user1"
			};

			DummyTimelineSyncerDB db = new DummyTimelineSyncerDB();
			TimelineSyncer timelineSyncer = new TimelineSyncer(postInfoProvider, db, new ChangeLogsApi());
			timelineSyncer.PullBackward(user);
		}
	}
}
