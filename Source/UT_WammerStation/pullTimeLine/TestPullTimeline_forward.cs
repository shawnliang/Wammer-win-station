﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.Station.Timeline;

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
				new DummyPostInfoProvider(), new DummyTimelineSyncerDB(), new ChangeLogsApi());

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
				new DummyPostInfoProvider(), new DummyTimelineSyncerDB(), new ChangeLogsApi());

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
			DateTime since = new DateTime(2012, 1, 2, 13, 23, 42, DateTimeKind.Utc);
			Driver user = new Driver
			{
				user_id = "user",
				sync_range = new SyncRange() { next_seq_num = 5, chlog_min_seq = 1, chlog_max_seq = 4 },
				session_token = "token",
				groups = this.groups,
				is_change_history_synced = true
			};

			Mock<IChangeLogsApi> api = new Mock<IChangeLogsApi>(MockBehavior.Strict);
			api.Setup(x => x.GetChangeHistory(user, user.sync_range.next_seq_num))
				.Returns(new ChangeLogResponse
				{
					group_id = user.groups[0].group_id,
					next_seq_num = 1000,
					post_list = new List<PostListItem> {
													new PostListItem{ post_id = "post1", seq_num = 999 },
													new PostListItem{ post_id = "post2", seq_num = 997 },
													new PostListItem{ post_id = "post3", seq_num = 998 },
							},
					changelog_list = new List<UserTrackDetail>{
							 						new UserTrackDetail { seq_num = 997, target_id="post2"},
													new UserTrackDetail { seq_num = 998, target_id="post3"},
													new UserTrackDetail { seq_num = 999, target_id="post1"},
							},
					remaining_count = 0
				})
				.Verifiable();


			Mock<IPostProvider> postInfo = new Mock<IPostProvider>(MockBehavior.Strict);
			postInfo.Setup(x => x.RetrievePosts(user, new List<string> { "post1", "post2", "post3" }))
				.Returns(new List<PostInfo> { 
							new PostInfo { post_id = "post1"},
							new PostInfo { post_id = "post2"},
							new PostInfo { post_id = "post3"}})
				.Verifiable();

			Mock<ITimelineSyncerDB> db = new Mock<ITimelineSyncerDB>(MockBehavior.Strict);
			db.Setup(x => x.SaveUserTracks(It.Is<UserTracks>(
				ut =>
						ut.post_id_list.Count == 3 &&
						ut.post_id_list.Contains("post1") &&
						ut.post_id_list.Contains("post2") &&
						ut.post_id_list.Contains("post3") &&
						ut.group_id == groups[0].group_id
				)))
				.Verifiable();
			db.Setup(x => x.SavePost(It.Is<PostInfo>(p => p.post_id == "post1"))).Verifiable();
			db.Setup(x => x.SavePost(It.Is<PostInfo>(p => p.post_id == "post2"))).Verifiable();
			db.Setup(x => x.SavePost(It.Is<PostInfo>(p => p.post_id == "post3"))).Verifiable();
			db.Setup(x => x.UpdateDriverSyncRange(user.user_id, It.Is<SyncRange>(s => s.next_seq_num == 1000 && s.chlog_min_seq == 1 && s.chlog_max_seq == 999))).Verifiable();

			TimelineSyncer syncer = new TimelineSyncer(postInfo.Object, db.Object, api.Object);
			syncer.PostsRetrieved += new EventHandler<TimelineSyncEventArgs>(syncer_PostsRetrieved);
			syncer.PullForward(user);

			db.VerifyAll();
			api.VerifyAll();
			postInfo.VerifyAll();
		}

		[TestMethod]
		public void TestPullForward_SetChMinMaxValueToValidRange()
		{
			DateTime since = new DateTime(2012, 1, 2, 13, 23, 42, DateTimeKind.Utc);
			Driver user = new Driver
			{
				user_id = "user",
				sync_range = new SyncRange() { next_seq_num = 5, chlog_min_seq = int.MaxValue, chlog_max_seq = int.MaxValue },
				session_token = "token",
				groups = this.groups,
				is_change_history_synced = true
			};

			Mock<IChangeLogsApi> api = new Mock<IChangeLogsApi>(MockBehavior.Strict);
			api.Setup(x => x.GetChangeHistory(user, user.sync_range.next_seq_num))
				.Returns(new ChangeLogResponse
				{
					group_id = user.groups[0].group_id,
					next_seq_num = 1000,
					post_list = new List<PostListItem> {
													new PostListItem{ post_id = "post1", seq_num = 999 },
													new PostListItem{ post_id = "post2", seq_num = 997 },
													new PostListItem{ post_id = "post3", seq_num = 998 },
							},
					changelog_list = new List<UserTrackDetail>{
							 						new UserTrackDetail { seq_num = 997, target_id="post2"},
													new UserTrackDetail { seq_num = 998, target_id="post3"},
													new UserTrackDetail { seq_num = 999, target_id="post1"},
							},
					remaining_count = 0
				})
				.Verifiable();


			Mock<IPostProvider> postInfo = new Mock<IPostProvider>(MockBehavior.Strict);
			postInfo.Setup(x => x.RetrievePosts(user, new List<string> { "post1", "post2", "post3" }))
				.Returns(new List<PostInfo> { 
							new PostInfo { post_id = "post1"},
							new PostInfo { post_id = "post2"},
							new PostInfo { post_id = "post3"}})
				.Verifiable();

			Mock<ITimelineSyncerDB> db = new Mock<ITimelineSyncerDB>(MockBehavior.Strict);
			db.Setup(x => x.SaveUserTracks(It.Is<UserTracks>(
				ut =>
						ut.post_id_list.Count == 3 &&
						ut.post_id_list.Contains("post1") &&
						ut.post_id_list.Contains("post2") &&
						ut.post_id_list.Contains("post3") &&
						ut.group_id == groups[0].group_id
				)))
				.Verifiable();
			db.Setup(x => x.SavePost(It.Is<PostInfo>(p => p.post_id == "post1"))).Verifiable();
			db.Setup(x => x.SavePost(It.Is<PostInfo>(p => p.post_id == "post2"))).Verifiable();
			db.Setup(x => x.SavePost(It.Is<PostInfo>(p => p.post_id == "post3"))).Verifiable();
			db.Setup(x => x.UpdateDriverSyncRange(user.user_id, It.Is<SyncRange>(s => s.next_seq_num == 1000 && s.chlog_min_seq == 997 && s.chlog_max_seq == 999))).Verifiable();

			TimelineSyncer syncer = new TimelineSyncer(postInfo.Object, db.Object, api.Object);
			syncer.PostsRetrieved += new EventHandler<TimelineSyncEventArgs>(syncer_PostsRetrieved);
			syncer.PullForward(user);

			db.VerifyAll();
			api.VerifyAll();
			postInfo.VerifyAll();
		}

		[TestMethod]
		public void TooManyRecoredsErrorWhilePullingForward()
		{
			Driver user = new Driver
			{
				user_id = "user",
				sync_range = new SyncRange() { next_seq_num = 5 },
				session_token = "token",
				groups = this.groups,
				is_change_history_synced = true
			};

			Mock<IChangeLogsApi> api = new Mock<IChangeLogsApi>(MockBehavior.Strict);
			api.Setup(x => x.GetChangeHistory(user, user.sync_range.next_seq_num))
				.Throws(new WammerCloudException("", "", (int)Wammer.Station.UserTrackApiError.TooManyRecord))
				.Verifiable();

			Mock<IPostProvider> postInfo = new Mock<IPostProvider>(MockBehavior.Strict);
			postInfo.Setup(x => x.GetPostsBySeq(user, user.sync_range.next_seq_num, It.Is<int>(limit => limit > 0)))
				.Returns(new PostFetchByFilterResponse
				{
					remaining_count = 0,
					group_id = user.groups[0].group_id,
					posts = new List<PostInfo>
					{
						new PostInfo{ post_id = "post1", seq_num = 10},
						new PostInfo{ post_id = "post2", seq_num = 20},
						new PostInfo{ post_id = "post3", seq_num = 15},
					},
					get_count = 3
				})
				.Verifiable();

			Mock<ITimelineSyncerDB> db = new Mock<ITimelineSyncerDB>(MockBehavior.Strict);
			db.Setup(x => x.SavePost(It.Is<PostInfo>(p => p.post_id == "post1" && p.seq_num == 10))).Verifiable();
			db.Setup(x => x.SavePost(It.Is<PostInfo>(p => p.post_id == "post2" && p.seq_num == 20))).Verifiable();
			db.Setup(x => x.SavePost(It.Is<PostInfo>(p => p.post_id == "post3" && p.seq_num == 15))).Verifiable();
			db.Setup(x => x.UpdateDriverSyncRange(user.user_id, It.Is<SyncRange>(s => s.next_seq_num == 21 && s.chlog_min_seq == int.MaxValue && s.chlog_max_seq == int.MaxValue))).Verifiable();
			db.Setup(x => x.UpdateDriverChangeHistorySynced(user.user_id, true)).Verifiable();

			TimelineSyncer syncer = new TimelineSyncer(postInfo.Object, db.Object, api.Object);
			syncer.PullForward(user);
		}

		[TestMethod]
		public void SeqPurgedWhilePullingForward()
		{
			Driver user = new Driver
			{
				user_id = "user",
				sync_range = new SyncRange() { next_seq_num = 5 },
				session_token = "token",
				groups = this.groups,
				is_change_history_synced = true
			};

			Mock<IChangeLogsApi> api = new Mock<IChangeLogsApi>(MockBehavior.Strict);
			api.Setup(x => x.GetChangeHistory(user, user.sync_range.next_seq_num))
				.Throws(new WammerCloudException("", "", (int)Wammer.Station.UserTrackApiError.SeqNumPurged))
				.Verifiable();

			Mock<IPostProvider> postInfo = new Mock<IPostProvider>(MockBehavior.Strict);
			postInfo.Setup(x => x.GetPostsBySeq(user, user.sync_range.next_seq_num, It.Is<int>(limit => limit > 0)))
				.Returns(new PostFetchByFilterResponse
				{
					remaining_count = 0,
					group_id = user.groups[0].group_id,
					posts = new List<PostInfo>
					{
						new PostInfo{ post_id = "post1", seq_num = 10},
						new PostInfo{ post_id = "post2", seq_num = 20},
						new PostInfo{ post_id = "post3", seq_num = 15},
					},
					get_count = 3
				})
				.Verifiable();

			Mock<ITimelineSyncerDB> db = new Mock<ITimelineSyncerDB>(MockBehavior.Strict);
			db.Setup(x => x.SavePost(It.Is<PostInfo>(p => p.post_id == "post1" && p.seq_num == 10))).Verifiable();
			db.Setup(x => x.SavePost(It.Is<PostInfo>(p => p.post_id == "post2" && p.seq_num == 20))).Verifiable();
			db.Setup(x => x.SavePost(It.Is<PostInfo>(p => p.post_id == "post3" && p.seq_num == 15))).Verifiable();
			db.Setup(x => x.UpdateDriverSyncRange(user.user_id, It.Is<SyncRange>(s => s.next_seq_num == 21 && s.chlog_min_seq == int.MaxValue && s.chlog_max_seq == int.MaxValue))).Verifiable();
			db.Setup(x => x.UpdateDriverChangeHistorySynced(user.user_id, true)).Verifiable();

			TimelineSyncer syncer = new TimelineSyncer(postInfo.Object, db.Object, api.Object);
			syncer.PullForward(user);
		}

		[TestMethod]
		public void DoNothingIfNoUpdate()
		{
			int since = 1000;

			DummyPostInfoProvider postInfo = new DummyPostInfoProvider();
			DummyTimelineSyncerDB db = new DummyTimelineSyncerDB();
			Mock<IChangeLogsApi> api = new Mock<IChangeLogsApi>();
			api.Setup(x => x.GetChangeHistory(It.IsAny<Driver>(), since)).Returns(new ChangeLogResponse { group_id = this.groups[0].group_id });

			TimelineSyncer syncer = new TimelineSyncer(postInfo, db, api.Object);
			syncer.PostsRetrieved += new EventHandler<TimelineSyncEventArgs>(syncer_PostsRetrieved);



			Driver user = new Driver
			{
				user_id = "user",
				sync_range = new SyncRange() { next_seq_num = since },
				session_token = "token",
				groups = this.groups,
				is_change_history_synced = true
			};
			syncer.PullForward(user);

			Assert.IsNull(db.UpdateSyncRange_syncRange);
			Assert.IsNull(db.UpdateSyncRange_userId);
			Assert.IsNull(this.RetrievedPosts);
		}

		[TestMethod]
		public void newBodyIsAvailable()
		{
			//DateTime since = new DateTime(2012, 2, 2, 13, 23, 42, DateTimeKind.Utc);
			int since = 1000;

			Mock<IPostProvider> postProvider = new Mock<IPostProvider>(MockBehavior.Strict);
			Mock<ITimelineSyncerDB> db = new Mock<ITimelineSyncerDB>(MockBehavior.Strict);
			Mock<IChangeLogsApi> utApi = new Mock<IChangeLogsApi>(MockBehavior.Strict);

			utApi.Setup(x => x.GetChangeHistory(It.IsAny<Driver>(), since)).
				Returns(new ChangeLogResponse
				{
					changelog_list = new List<UserTrackDetail> {
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
			db.Setup(x => x.UpdateDriverSyncRange("user", It.IsAny<SyncRange>())).Verifiable();
			AttachmentAvailableEventArgs saved_args = null;
			TimelineSyncer syncer = new TimelineSyncer(postProvider.Object, db.Object, utApi.Object);
			syncer.AttachmentAvailable += new EventHandler<AttachmentAvailableEventArgs>(
				(sender, args) => { saved_args = args; });

			Driver user = new Driver
			{
				user_id = "user",
				sync_range = new SyncRange()
				{
					start_time = DateTime.UtcNow,
					first_post_time = DateTime.UtcNow,
					next_seq_num = since
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

	class FakeUserTracksApi : IChangeLogsApi
	{
		public ChangeLogResponse GetChangeHistory(Wammer.Model.Driver user, int since)
		{
			Assert.AreEqual("token", user.session_token);

			return new ChangeLogResponse()
			{
				get_count = 3,
				group_id = "group1",
				post_list = new List<PostListItem> { 
								new PostListItem { post_id = "post1"},
								new PostListItem { post_id = "post2"},
								new PostListItem { post_id = "post3"}
				},
				next_seq_num = 1000
			};
		}
	}


}