using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wammer.Station.TimelineChange;
using Wammer.Cloud;
using System.Net;

namespace UT_WammerStation
{
	[TestClass]
	public class TestTimelineChange_PostInfoProvider
	{
		private UserTrackResponse answer = null;
		private FakeCloud cloud = null;

		[TestInitialize]
		public void Setup()
		{
			answer = new UserTrackResponse
			{
				api_ret_code = 0,
				api_ret_message = "success",
				get_count = 3,
				group_id = "group_id1",
				lastest_timestamp = "timestamp1",
				post_id_list = new List<string> { "post1", "post2", "post3" },
				remaining_count = 0,
				status = 200,

			};
			cloud = new FakeCloud(answer);
			CloudServer.BaseUrl = "http://localhost:80/v2/";
		}

		[TestCleanup]
		public void TearDown()
		{
			cloud.Dispose();
		}

		[TestMethod]
		public void TestGetChangeList()
		{
			PostInfoProvider p = new PostInfoProvider();
			ChangeHistory history = p.RetrieveChangedPosts(new WebClient(), 
				"session1", "start_time", "apikey", "group_id1");

			Assert.IsNotNull(history);
			Assert.AreEqual(3, history.ChangedPostIds.Count);
			Assert.AreEqual("post1", history.ChangedPostIds[0]);
			Assert.AreEqual("post2", history.ChangedPostIds[1]);
			Assert.AreEqual("post3", history.ChangedPostIds[2]);
			Assert.AreEqual(answer.lastest_timestamp, history.LastSyncTime);
		}
	}
}
