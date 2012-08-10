using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wammer.Cloud;
using Wammer.Model;

using Wammer.Station.APIHandler;
using Moq;

namespace UT_WammerStation_TestUserTrackApiHandler
{
	[TestClass]
	public class TestUserTrackApiHandler
	{
		[TestMethod]
		public void GetNothingIfUserTrackSyncingIsNotComplete()
		{
			var mockDB = new Mock<IUserTrackHandlerDB>();
			mockDB.Setup(x => x.GetUserByGroupId("groupId")).Returns(
				new Driver
				{
					user_id = "user1",
					is_change_history_synced = false
				});

			UserTrackHandlerImp handler = new UserTrackHandlerImp(mockDB.Object);
			var result = handler.GetUserTrack("groupId", 100, true);
			Assert.AreEqual(100, result.next_seq_num);
			Assert.IsNull(result.post_list);
			Assert.IsNull(result.changelog_list);
		}

		[TestMethod]
		[ExpectedException(typeof(Wammer.Station.WammerStationException))]
		public void GetNothingIfNotSuchUser()
		{
			var mockDB = new Mock<IUserTrackHandlerDB>();
			mockDB.Setup(x => x.GetUserByGroupId("groupId")).Returns((Driver)null);

			UserTrackHandlerImp handler = new UserTrackHandlerImp(mockDB.Object);
			handler.GetUserTrack("groupId", 157, true);
		}

		[TestMethod]
		public void GetOneUserTrack()
		{
			var mockDB = new Mock<IUserTrackHandlerDB>();
			mockDB.Setup(x => x.GetUserByGroupId("groupId")).Returns(
				new Driver
				{
					user_id = "user1",
					sync_range = new SyncRange
					{
						start_time = new DateTime(2012, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						first_post_time = new DateTime(2012, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						chlog_min_seq = 1,
						chlog_max_seq = 102
					},
					is_change_history_synced = true,
				});

			UserTracks ut1 = new UserTracks
			{
				//latest_timestamp = new DateTime(2012, 4, 10, 0, 0, 0, DateTimeKind.Utc),
				next_seq_num = 103,
				post_id_list = new List<string> { "post1", "post2" },
				usertrack_list = new List<UserTrackDetail> { 
							new UserTrackDetail { target_type = "target_type1"}}
			};

			mockDB.Setup(x => x.GetUserTracksSince("groupId", 100)).Returns(
				new List<UserTracks> { ut1 });


			UserTrackHandlerImp handler = new UserTrackHandlerImp(mockDB.Object);
			var res = handler.GetUserTrack("groupId", 100, true);

			Assert.AreEqual(0, res.api_ret_code);
			Assert.AreEqual("success", res.api_ret_message);
			Assert.AreEqual(1, res.get_count);
			Assert.AreEqual("groupId", res.group_id);
			Assert.AreEqual(ut1.next_seq_num, res.next_seq_num);
			Assert.AreEqual(2, res.post_list.Count);
			Assert.AreEqual("post1", res.post_list[0].post_id);
			Assert.AreEqual("post2", res.post_list[1].post_id);
			Assert.AreEqual(0, res.remaining_count);
			Assert.AreEqual(200, res.status);
			Assert.IsNotNull(res.timestamp);
			Assert.AreEqual(1, res.changelog_list.Count);
			Assert.AreEqual(ut1.usertrack_list[0], res.changelog_list[0]);
		}

		[TestMethod]
		public void Merge2UserTracksIntoOne()
		{
			var mockDB = new Mock<IUserTrackHandlerDB>();
			mockDB.Setup(x => x.GetUserByGroupId("groupId")).Returns(
				new Driver
				{
					user_id = "user1",
					sync_range = new SyncRange
					{
						start_time = new DateTime(2012, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						first_post_time = new DateTime(2012, 1, 1, 0, 0, 0, DateTimeKind.Utc),
						chlog_min_seq = 99,
						chlog_max_seq = 101
					},
					is_change_history_synced = true,
				});

			UserTracks ut1 = new UserTracks
			{
				//latest_timestamp = new DateTime(2012, 4, 10, 0,0,0, DateTimeKind.Utc),
				next_seq_num = 100,
				post_id_list = new List<string> { "post1", "post2" },
				usertrack_list = new List<UserTrackDetail> { 
							new UserTrackDetail { target_type = "target_type1"}}
			};

			UserTracks ut2 = new UserTracks
			{
				//latest_timestamp = new DateTime(2012, 4, 11, 0, 0, 0, DateTimeKind.Utc),
				next_seq_num = 102,
				post_id_list = new List<string> { "post1", "post3" },
				usertrack_list = new List<UserTrackDetail> { 
							new UserTrackDetail { target_type = "target_type2"}}
			};
			mockDB.Setup(x => x.GetUserTracksSince("groupId", 99)).Returns(
				new List<UserTracks> { ut1, ut2 });


			UserTrackHandlerImp handler = new UserTrackHandlerImp(mockDB.Object);
			var res = handler.GetUserTrack("groupId", 99, true);

			Assert.AreEqual(0, res.api_ret_code);
			Assert.AreEqual("success", res.api_ret_message);
			Assert.AreEqual(2, res.get_count);
			Assert.AreEqual("groupId", res.group_id);
			Assert.AreEqual(ut2.next_seq_num, res.next_seq_num);
			Assert.AreEqual(3, res.post_list.Count);
			Assert.AreEqual("post1", res.post_list[0].post_id);
			Assert.AreEqual("post2", res.post_list[1].post_id);
			Assert.AreEqual("post3", res.post_list[2].post_id);
			Assert.AreEqual(0, res.remaining_count);
			Assert.AreEqual(200, res.status);
			Assert.IsNotNull(res.timestamp);
			Assert.AreEqual(2, res.changelog_list.Count);
			Assert.AreEqual(ut1.usertrack_list[0], res.changelog_list[0]);
			Assert.AreEqual(ut2.usertrack_list[0], res.changelog_list[1]);
		}

		[TestMethod]
		public void ReturnsNothingIfGivenSeqIsTooLarge()
		{
			var mockDB = new Mock<IUserTrackHandlerDB>();
			mockDB.Setup(x => x.GetUserByGroupId("groupId")).Returns(
				new Driver
				{
					user_id = "user1",
					is_change_history_synced = true,
					sync_range = new SyncRange
					{
						chlog_min_seq = 1,
						chlog_max_seq = 100,
					}
				});

			UserTrackHandlerImp handler = new UserTrackHandlerImp(mockDB.Object);
			var result = handler.GetUserTrack("groupId", 500, true);
			Assert.AreEqual(101, result.next_seq_num);
			Assert.IsNull(result.post_list);
			Assert.IsNull(result.changelog_list);
		}

		[TestMethod]
		public void ReturnsErrorIfGivenSeqIsTooSmall()
		{
			var mockDB = new Mock<IUserTrackHandlerDB>();
			mockDB.Setup(x => x.GetUserByGroupId("groupId")).Returns(
				new Driver
				{
					user_id = "user1",
					is_change_history_synced = true,
					sync_range = new SyncRange
					{
						chlog_min_seq = 50,
						chlog_max_seq = 100
					}
				});

			UserTrackHandlerImp handler = new UserTrackHandlerImp(mockDB.Object);

			try
			{
				var result = handler.GetUserTrack("groupId", 20, true);
			}
			catch (Wammer.Station.WammerStationException e)
			{
				Assert.AreEqual(e.WammerError, (int)Wammer.Station.UserTrackApiError.SeqNumPurged);
				return;
			}

			Assert.Fail("Should throw UserTrackApiError.SeqNumPurged");
		}
	}
}

