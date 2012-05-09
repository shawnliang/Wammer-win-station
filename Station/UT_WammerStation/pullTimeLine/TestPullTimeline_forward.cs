using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wammer.Model;
using Wammer.Station.Timeline;
using Wammer.Cloud;
using System.Net;
using Moq;
using Wammer.Station;

namespace UT_WammerStation.pullTimeLine
{

	[TestClass]
	public class TestPullTimeline
	{
		List<UserGroup> groups;
		ICollection<PostInfo> RetrievedPosts;

		[TestInitialize]
		public void SetUp()
		{
			groups = new List<UserGroup> {
						 new UserGroup { group_id = "group1"}
			};

			RetrievedPosts = null;
		}

		[ExpectedException(typeof(InvalidOperationException))]
		[TestMethod]
		public void CannotPullForwardIfPullBackwardIsEverCalled()
		{
			TimelineSyncer syncer = new TimelineSyncer(
				new DummyPostInfoProvider(), new DummyTimelineSyncerDB(), new UserTracksApi());

			syncer.PullForward(new Driver
				{
								 user_id = "user",
								  sync_range = null,
								  session_token = "token",
								  groups = this.groups
				});
		}

		[ExpectedException(typeof(InvalidOperationException))]
		[TestMethod]
		public void CannotPullForwardIfPullBackwardIsEverCalled2()
		{
			TimelineSyncer syncer = new TimelineSyncer(
				new DummyPostInfoProvider(), new DummyTimelineSyncerDB(), new UserTracksApi());

			syncer.PullForward(new Driver
			{
				user_id = "user",
				sync_range = new SyncRange(),
				session_token = "token",
				groups = this.groups
			});
		}

		[TestMethod]
		public void TestPullForward()
		{
			DummyPostInfoProvider postInfo = new DummyPostInfoProvider();
			postInfo.RetrievePosts_return = new List<PostInfo> { 
						 new PostInfo { post_id = "post1"},
						 new PostInfo { post_id = "post1"},
						 new PostInfo { post_id = "post1"}
			};

			DummyTimelineSyncerDB db = new DummyTimelineSyncerDB();
			TimelineSyncer syncer = new TimelineSyncer(postInfo, db, new FakeUserTracksApi());
			syncer.PostsRetrieved += new EventHandler<TimelineSyncEventArgs>(syncer_PostsRetrieved);

			DateTime since = new DateTime(2012, 1, 2, 13, 23, 42, DateTimeKind.Utc);

			Driver user = new Driver
			{
				user_id = "user",
				sync_range = new SyncRange() { end_time = since },
				session_token = "token",
				groups = this.groups
			};
			syncer.PullForward(user);
			

			// verify get userTracks from prev time
			// Done in FakeUserTracksApi.GetChangeHistory()

			// verify retrieve post details by id
			Assert.AreEqual(user, postInfo.RetrievePosts_user);
			Assert.AreEqual(3, postInfo.RetrievePosts_posts.Count);
			Assert.AreEqual("post1", postInfo.RetrievePosts_posts[0]);
			Assert.AreEqual("post2", postInfo.RetrievePosts_posts[1]);
			Assert.AreEqual("post3", postInfo.RetrievePosts_posts[2]);

			// verify retrieved post are callbacked
			Assert.AreEqual(postInfo.RetrievePosts_return, this.RetrievedPosts);

			// verify driver's sync.end_time is updated.
			// "2012-02-02T13:23:42Z" is from the return value of FakeUserTracksApi.GetChangeHistory
			Assert.AreEqual(new DateTime(2012,2,2,13,23,42, DateTimeKind.Utc), db.UpdateSyncRange_syncRange.end_time); 
		}


		[TestMethod]
		public void TestRetrievedPostsAreSavedToDB()
		{
			DummyPostInfoProvider postInfo = new DummyPostInfoProvider();
			postInfo.RetrievePosts_return = new List<PostInfo> { 
						 new PostInfo { post_id = "post1"},
						 new PostInfo { post_id = "post1"},
						 new PostInfo { post_id = "post1"}
			};

			DummyTimelineSyncerDB db = new DummyTimelineSyncerDB();
			TimelineSyncer syncer = new TimelineSyncer(postInfo, db, new FakeUserTracksApi());
			syncer.PostsRetrieved += new EventHandler<TimelineSyncEventArgs>(syncer_PostsRetrieved);

			DateTime since = new DateTime(2012, 1, 2, 13, 23, 42, DateTimeKind.Utc);

			Driver user = new Driver
			{
				user_id = "user",
				sync_range = new SyncRange() { end_time = since },
				session_token = "token",
				groups = this.groups
			};
			syncer.PullForward(user);

			Assert.AreEqual(postInfo.RetrievePosts_return.Count, db.SavedPosts.Count);
			Assert.AreEqual(postInfo.RetrievePosts_return[0], db.SavedPosts[0]);
			Assert.AreEqual(postInfo.RetrievePosts_return[1], db.SavedPosts[1]);
			Assert.AreEqual(postInfo.RetrievePosts_return[2], db.SavedPosts[2]);
		}

		[TestMethod]
		public void DoNothingIfNoUpdate()
		{
			DateTime since = new DateTime(2012, 2, 2, 13, 23, 42, DateTimeKind.Utc);

			DummyPostInfoProvider postInfo = new DummyPostInfoProvider();
			DummyTimelineSyncerDB db = new DummyTimelineSyncerDB();
			Mock<IUserTrackApi> api = new Mock<IUserTrackApi>();
			api.Setup(x => x.GetChangeHistory(It.IsAny<WebClient>(), It.IsAny<Driver>(), since.AddSeconds(1.0))).Throws(new WammerCloudException("123", new ArgumentOutOfRangeException()));

			TimelineSyncer syncer = new TimelineSyncer(postInfo, db, api.Object);
			syncer.PostsRetrieved += new EventHandler<TimelineSyncEventArgs>(syncer_PostsRetrieved);

			

			Driver user = new Driver
			{
				user_id = "user",
				sync_range = new SyncRange() { end_time = since },
				session_token = "token",
				groups = this.groups
			};
			syncer.PullForward(user);

			Assert.IsNull(db.UpdateSyncRange_syncRange);
			Assert.IsNull(db.UpdateSyncRange_userId);
			Assert.IsNull(this.RetrievedPosts);
		}

		[TestMethod]
		public void newBodyIsAvailable()
		{
			DateTime since = new DateTime(2012, 2, 2, 13, 23, 42, DateTimeKind.Utc);

			Mock<IPostProvider> postProvider = new Mock<IPostProvider>(MockBehavior.Strict);
			Mock<ITimelineSyncerDB> db = new Mock<ITimelineSyncerDB>(MockBehavior.Strict);
			Mock<IUserTrackApi> utApi = new Mock<IUserTrackApi>(MockBehavior.Strict);

			utApi.Setup(x => x.GetChangeHistory(It.IsAny<WebClient>(), It.IsAny<Driver>(), since.AddSeconds(1.0))).
				Returns(new UserTrackResponse
				{
					usertrack_list = new List<UserTrackDetail> {
						new UserTrackDetail 
						{ 
							target_type = "attachment",
							group_id = "group_id",
							user_id = "user_id",
							target_id = "object_id",
							timestamp = DateTime.UtcNow,
							actions = new List<UserTrackAction>{
										 new UserTrackAction {
											action = "add",
											target_type = "image.origin"										 
										 }
							}
						}
					}
				}).Verifiable();


			db.Setup(x => x.SaveUserTracks(It.IsAny<UserTracks>())).Verifiable();

			BodyAvailableEventArgs saved_args = null;
			TimelineSyncer syncer = new TimelineSyncer(postProvider.Object, db.Object, utApi.Object);
			syncer.BodyAvailable += new EventHandler<BodyAvailableEventArgs>(
				(sender, args) => { saved_args = args; });

			Driver user = new Driver
			{
				user_id = "user",
				sync_range = new SyncRange() {
					start_time = DateTime.UtcNow,
					first_post_time = DateTime.UtcNow,
					end_time= since
				},
				is_change_history_synced = true,
				session_token = "token",
				groups = this.groups
			};
			syncer.PullTimeline(user);

			db.VerifyAll();
			utApi.VerifyAll();
			Assert.AreEqual("group_id", saved_args.group_id);
			Assert.AreEqual("user_id", saved_args.user_id);
			Assert.AreEqual("object_id", saved_args.object_id);
		}


		void syncer_PostsRetrieved(object sender, TimelineSyncEventArgs e)
		{
			RetrievedPosts = e.Posts;
		}
	}

	class FakeUserTracksApi: IUserTrackApi
	{
		public UserTrackResponse GetChangeHistory(WebClient agent, Wammer.Model.Driver user, DateTime since)
		{
			Assert.AreEqual("token", user.session_token);
			Assert.IsNotNull(agent);

			return new UserTrackResponse()
			{
				get_count = 3,
				group_id = "group1",
				latest_timestamp = new DateTime(2012, 2, 2, 13, 23, 42, DateTimeKind.Utc),
				post_id_list = new List<string> { "post1", "post2", "post3" }
			};
		}
	}


}
