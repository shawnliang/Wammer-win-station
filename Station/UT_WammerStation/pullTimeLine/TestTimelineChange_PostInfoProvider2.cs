using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wammer.Station.Timeline;
using Wammer.Cloud;
using System.Net;
using Wammer.Model;

namespace UT_WammerStation
{
	[TestClass]
	public class TestTimelineChange_PostInfoProvider2
	{
		private UserTrackResponse answer = null;
		private FakeCloud cloud = null;

		[TestInitialize]
		public void Setup()
		{
			PostFetchByFilterResponse r = new PostFetchByFilterResponse
			{
				api_ret_code = 0,
				api_ret_message = "success",
				get_count = 3,
				group_id = "gid",
				remaining_count = 3,
				status = 200,
				posts = new List<PostInfo>()
				{
					new PostInfo { post_id = "1" },
					new PostInfo{ post_id = "2"}
				}
			};

			cloud = new FakeCloud(r);
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
			List<PostInfo> posts = p.RetrievePosts(new WebClient(),
				new List<string>() { "1", "2", "3" },
				new Driver
				{
					user_id = "u1",
					groups = new List<UserGroup>
					{
						new UserGroup { group_id = "gid" } 
					}
				});

			Assert.IsNotNull(posts);
			Assert.AreEqual(2, posts.Count);
			Assert.AreEqual("1", posts[0].post_id);
			Assert.AreEqual("2", posts[1].post_id);
		}
	}
}
