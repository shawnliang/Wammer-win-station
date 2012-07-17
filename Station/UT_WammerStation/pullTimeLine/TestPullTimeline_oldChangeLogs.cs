using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wammer.Station.Timeline;
using Moq;
using Wammer.Cloud;
using Wammer.Model;

namespace UT_WammerStation.pullTimeLine
{
	[TestClass]
	public class TestPullTimeline_oldChangeLogs
	{
		Driver user;

		[TestInitialize]
		public void Setup()
		{
			user = new Driver
			{
				groups = new List<UserGroup> { new UserGroup { group_id = "group1" } },
				user_id = "user1",
				sync_range = new SyncRange
				{
					start_time = DateTime.UtcNow.AddDays(-1.0),
					first_post_time = DateTime.UtcNow.AddDays(-1.0),
					next_seq_num = 0,
				},
				is_change_history_synced = false
			};
		}

		[TestMethod]
		public void pullOldChangedLogs()
		{
			Mock<IChangeLogsApi> api = new Mock<IChangeLogsApi>(MockBehavior.Strict);
			var res1 = new ChangeLogResponse
			{
				api_ret_code = 200,
				group_id = user.groups[0].group_id,
				get_count = 1,
				changelog_list = new List<UserTrackDetail>
				{
					new UserTrackDetail{ seq_num = 1 }
				},
				post_list = new List<PostListItem>{ new PostListItem { post_id = "post1"}},
				next_seq_num = 2,
				remaining_count = 1,
			};
			api.Setup(x => x.GetChangeHistory(user, 1)).Returns(res1).Verifiable();

			var res2 = new ChangeLogResponse
			{
				api_ret_code = 200,
				group_id = user.groups[0].group_id,
				get_count = 1,
				changelog_list = new List<UserTrackDetail>
				{
					new UserTrackDetail{ seq_num = 2 }
				},
				post_list = new List<PostListItem> { new PostListItem { post_id = "post2" } },
				next_seq_num = 3,
				remaining_count = 0,
			};

			api.Setup(x => x.GetChangeHistory(user, 2)).Returns(res2).Verifiable();

			Mock<IPostProvider> postProvider = new Mock<IPostProvider>(MockBehavior.Strict);
			postProvider.Setup(x => x.RetrievePosts(user, 
				It.Is<List<string>>(list => list.Count == 1 && list.Contains("post2"))))
				.Returns(new List<PostInfo>{ new PostInfo {post_id = "post2"} })
				.Verifiable();

			Mock<ITimelineSyncerDB> db = new Mock<ITimelineSyncerDB>(MockBehavior.Strict);
			db.Setup(x => x.SaveUserTracks(
				It.Is<UserTracks>(ut =>
							ut.group_id == user.groups[0].group_id &&
							ut.post_id_list.Count == res1.post_list.Count &&
							ut.post_id_list.FindAll(id => res1.post_list.Any(p => p.post_id == id) != null).Count == ut.post_id_list.Count &&
							ut.usertrack_list == res1.changelog_list
				))).Verifiable();

			db.Setup(x => x.SaveUserTracks(
				It.Is<UserTracks>(ut =>
							ut.group_id == user.groups[0].group_id &&
							ut.post_id_list.Count == res2.post_list.Count &&
							ut.post_id_list.FindAll(id => res2.post_list.Any(p => p.post_id == id) != null).Count == ut.post_id_list.Count &&
							ut.usertrack_list == res2.changelog_list
				))).Verifiable();

			db.Setup(x => x.SavePost(It.Is<PostInfo>(p => p.post_id == "post2"))).Verifiable();

			db.Setup(x => x.UpdateDriverSyncRange(
				user.user_id, It.Is<SyncRange>(sc => sc.next_seq_num == 3))).Verifiable();
			db.Setup(x => x.UpdateDriverChangeHistorySynced(user.user_id, true)).Verifiable();

			TimelineSyncer syncer = new TimelineSyncer(postProvider.Object, db.Object, api.Object);
			syncer.PullTimeline(user);


			db.VerifyAll();
			api.VerifyAll();
			postProvider.VerifyAll();

		}

		[TestMethod]
		public void pullOldChangeLogsButTooMany()
		{
			Mock<IChangeLogsApi> api = new Mock<IChangeLogsApi>(MockBehavior.Strict);
			api.Setup(x => x.GetChangeHistory(user, 1)).Throws(
				new WammerCloudException("error", "error", (int)Wammer.Station.UserTrackApiError.TooManyUserTracks))
				.Verifiable();

			Mock<IPostProvider> postProvider = new Mock<IPostProvider>(MockBehavior.Strict);
			postProvider.Setup(x => x.GetPostsBySeq(user, 1, It.Is<int>(limit => limit > 0)))
				.Returns(new PostFetchByFilterResponse()
				{
					posts = new List<PostInfo> { new PostInfo {  post_id = "post1", seq_num = 100 } },
					get_count = 1,
					group_id = user.groups[0].group_id,
					remaining_count = 1
				}).Verifiable();

			postProvider.Setup(x => x.GetPostsBySeq(user, 101, It.Is<int>(limit => limit>0)))
				.Returns(new PostFetchByFilterResponse()
				{
					posts = new List<PostInfo> { new PostInfo { post_id = "post2", seq_num = 101 } },
					get_count = 1,
					group_id = user.groups[0].group_id,
					remaining_count = 0,
				}).Verifiable();

			Mock<ITimelineSyncerDB> db = new Mock<ITimelineSyncerDB>(MockBehavior.Strict);
			db.Setup(x => x.SavePost(It.Is<PostInfo>(p => p.post_id == "post1" && p.seq_num == 100)))
				.Verifiable();
			db.Setup(x => x.SavePost(It.Is<PostInfo>(p => p.post_id == "post2" && p.seq_num == 101)))
				.Verifiable();

			db.Setup(x => x.UpdateDriverSyncRange(user.user_id, It.Is<SyncRange>(sc => sc.next_seq_num == 102)))
				.Verifiable();
			db.Setup(x => x.UpdateDriverChangeHistorySynced(user.user_id, true)).Verifiable();


			TimelineSyncer syncer = new TimelineSyncer(postProvider.Object, db.Object, api.Object);
			syncer.PullTimeline(user);


			db.VerifyAll();
			api.VerifyAll();
			postProvider.VerifyAll();
		}
	}
}
