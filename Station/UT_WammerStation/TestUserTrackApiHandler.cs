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
		[ExpectedException(typeof(Wammer.Station.WammerStationException))]
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
			handler.GetUserTrack("groupId", "2012-04-04T10:00:00Z", true);
		}

		[TestMethod]
		[ExpectedException(typeof(Wammer.Station.WammerStationException))]
		public void GetNothingIfNotSuchUser()
		{
			var mockDB = new Mock<IUserTrackHandlerDB>();
			mockDB.Setup(x => x.GetUserByGroupId("groupId")).Returns((Driver)null);

			UserTrackHandlerImp handler = new UserTrackHandlerImp(mockDB.Object);
			handler.GetUserTrack("groupId", "2012-04-04T10:00:00Z", true);
		}

		[TestMethod]
		public void GetOneUserTrack()
		{
			var mockDB = new Mock<IUserTrackHandlerDB>();
			mockDB.Setup(x => x.GetUserByGroupId("groupId")).Returns(
				new Driver
				{
					user_id = "user1",
					sync_range = new SyncRange { end_time = "2012-04-01T00:00:00Z", start_time = "2012-01-01T00:00:00Z", first_post_time = "2012-01-01T00:00:00Z"},
					is_change_history_synced = true,
				});

			UserTracks ut1 = new UserTracks
			{
				latest_timestamp = "2012-04-10T00:00:00Z",
				post_id_list = new List<string> { "post1", "post2" },
				usertrack_list = new List<UserTrackDetail> { 
							new UserTrackDetail { target_type = "target_type1"}}
			};

			mockDB.Setup(x => x.GetUserTracksSince("groupId", new DateTime(2012, 1, 1, 0,0,0, DateTimeKind.Utc))).Returns(
				new List<UserTracks> { ut1 });


			UserTrackHandlerImp handler = new UserTrackHandlerImp(mockDB.Object);
			UserTrackResponse res = handler.GetUserTrack("groupId", "2012-01-01T00:00:00Z", true);

			Assert.AreEqual(0, res.api_ret_code);
			Assert.AreEqual("success", res.api_ret_message);
			Assert.AreEqual(1, res.get_count);
			Assert.AreEqual("groupId", res.group_id);
			Assert.AreEqual(ut1.latest_timestamp, res.latest_timestamp);
			Assert.AreEqual(2, res.post_id_list.Count);
			Assert.AreEqual("post1", res.post_id_list[0]);
			Assert.AreEqual("post2", res.post_id_list[1]);
			Assert.AreEqual(0, res.remaining_count);
			Assert.AreEqual(200, res.status);
			Assert.IsNotNull(res.timestamp);
			Assert.AreEqual(1, res.usertrack_list.Count);
			Assert.AreEqual(ut1.usertrack_list[0], res.usertrack_list[0]);
		}

		[TestMethod]
		public void Merge2UserTracksIntoOne()
		{
			var mockDB = new Mock<IUserTrackHandlerDB>();
			mockDB.Setup(x => x.GetUserByGroupId("groupId")).Returns(
				new Driver
				{
					user_id = "user1",
					sync_range = new SyncRange { end_time = "2012-04-01T00:00:00Z", start_time = "2012-01-01T00:00:00Z", first_post_time = "2012-01-01T00:00:00Z" },
					is_change_history_synced = true,
				});

			UserTracks ut1 = new UserTracks
			{
				latest_timestamp = "2012-04-10T00:00:00Z",
				post_id_list = new List<string> { "post1", "post2" },
				usertrack_list = new List<UserTrackDetail> { 
							new UserTrackDetail { target_type = "target_type1"}}
			};

			UserTracks ut2 = new UserTracks
			{
				latest_timestamp = "2012-04-11T00:00:00Z",
				post_id_list = new List<string> { "post1", "post3" },
				usertrack_list = new List<UserTrackDetail> { 
							new UserTrackDetail { target_type = "target_type2"}}
			};
			mockDB.Setup(x => x.GetUserTracksSince("groupId", new DateTime(2012, 1, 1, 0, 0, 0, DateTimeKind.Utc))).Returns(
				new List<UserTracks> { ut1, ut2 });


			UserTrackHandlerImp handler = new UserTrackHandlerImp(mockDB.Object);
			UserTrackResponse res = handler.GetUserTrack("groupId", "2012-01-01T00:00:00Z", true);

			Assert.AreEqual(0, res.api_ret_code);
			Assert.AreEqual("success", res.api_ret_message);
			Assert.AreEqual(2, res.get_count);
			Assert.AreEqual("groupId", res.group_id);
			Assert.AreEqual(ut2.latest_timestamp, res.latest_timestamp);
			Assert.AreEqual(3, res.post_id_list.Count);
			Assert.AreEqual("post1", res.post_id_list[0]);
			Assert.AreEqual("post2", res.post_id_list[1]);
			Assert.AreEqual("post3", res.post_id_list[2]);
			Assert.AreEqual(0, res.remaining_count);
			Assert.AreEqual(200, res.status);
			Assert.IsNotNull(res.timestamp);
			Assert.AreEqual(2, res.usertrack_list.Count);
			Assert.AreEqual(ut1.usertrack_list[0], res.usertrack_list[0]);
			Assert.AreEqual(ut2.usertrack_list[0], res.usertrack_list[1]);
		}
	}
}

