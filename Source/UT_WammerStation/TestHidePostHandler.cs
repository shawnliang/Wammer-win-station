using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using Wammer;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.PerfMonitor;
using Wammer.Station;

namespace UT_WammerStation
{
	[TestClass]
	public class TestHidePostHandler
	{
		#region Const
		const string MONGODB_URL = @"mongodb://localhost:10319/safe=true";
		const string API_URL = @"http://localhost:8080/v2/posts/hide";
		#endregion


		#region Var
		static MongoServer mongodb;
		HttpServer server;
		HttpHandler handler;
		#endregion

		#region Private Method
		private void Reset()
		{
			var db = mongodb.GetDatabase("wammer");

			db.GetCollection("drivers").RemoveAll();
			db.GetCollection("station").RemoveAll();
			db.GetCollection("Post").RemoveAll();
			db.GetCollection("attachments").RemoveAll();

			if (server != null)
			{
				server.Dispose();
				server = null;
			}
		}
		#endregion


		#region Public Method
		[ClassInitialize()]
		public static void MyClassInitialize(TestContext testContext)
		{
			mongodb = MongoServer.Create(MONGODB_URL);
		}

		[TestInitialize]
		public void setUp()
		{
			Reset();

			server = new HttpServer(8080);
			handler = new HidePostHandler(null);
			server.AddHandler("/v2/posts/hide", handler);
			server.Start();
			server.TaskEnqueue += new EventHandler<TaskQueueEventArgs>(HttpRequestMonitor.Instance.OnTaskEnqueue);

			CloudServer.BaseUrl = "http://localhost/v2/";

			mongodb.GetDatabase("wammer").GetCollection("drivers").Insert(
				new Driver
				{
					user_id = "exist_uid",
					email = "exist@gmail.com",
					folder = "resource\\user_exist_uid",
					session_token = "exist session token",
					groups = new List<UserGroup> { new UserGroup { group_id = "123" } }
				});

			mongodb.GetDatabase("wammer").GetCollection("LoginedSession").Insert(
				new LoginedSession()
				{
					session_token = "exist session token",
					apikey = new Apikey() { name = "window" }
				});
		}

		[TestCleanup]
		public void tearDown()
		{
			Reset();
		}

		[TestMethod]
		public void HodePost_HideSpecificedPost_Success()
		{
			var db = mongodb.GetDatabase("wammer");
			db.GetCollection("Post").RemoveAll();
			db.GetCollection("attachments").RemoveAll();

			PostCollection.Instance.Save(
				new PostInfo()
				{
					post_id = "001",
					preview = new Preview()
					{
						title = "unit test original preview"
					},
					content = "unit test original content",
					hidden = "false"
				});
			AttachmentCollection.Instance.Save(
				new Attachment()
				{
					object_id = "12345",
					url = "http://unittest.com"
				});

			var response = CloudServer.request<HidePostResponse>(API_URL,
				new Dictionary<object, object>{ 
				{CloudServer.PARAM_API_KEY, "!@##%$&"},
				{CloudServer.PARAM_SESSION_TOKEN, "!@##%$&"},
				{CloudServer.PARAM_GROUP_ID, "!@##%$&"},
				{CloudServer.PARAM_POST_ID, "001"}});

			Assert.IsNotNull(response);

			var postID = response.post_id;

			Assert.IsTrue(!string.IsNullOrEmpty(postID));

			var post = PostCollection.Instance.FindOne(Query.EQ("_id", postID));

			Assert.IsNotNull(post);
			Assert.IsTrue(post.hidden.Equals("true", StringComparison.CurrentCultureIgnoreCase));
		}

		[TestMethod]
		[ExpectedException(typeof(WammerCloudException))]
		public void HidePost_WithoutAPIKey_ThrowWammerCloudException()
		{
			CloudServer.request<CloudResponse>(API_URL,
				new Dictionary<object, object>{ 
                {CloudServer.PARAM_GROUP_ID, "!@##%$&"},
                {CloudServer.PARAM_SESSION_TOKEN, "!@##%$&"},
                {CloudServer.PARAM_POST_ID, "!@##%$&"}});
		}

		[TestMethod]
		[ExpectedException(typeof(WammerCloudException))]
		public void HidePost_WithoutSessionToken_ThrowWammerCloudException()
		{
			CloudServer.request<CloudResponse>(API_URL,
				new Dictionary<object, object>{ 
                {CloudServer.PARAM_API_KEY, "!@##%$&"},
                {CloudServer.PARAM_GROUP_ID, "!@##%$&"},
                {CloudServer.PARAM_POST_ID, "!@##%$&"}});
		}

		[TestMethod]
		[ExpectedException(typeof(WammerCloudException))]
		public void HidePost_WithoutGroupID_ThrowWammerCloudException()
		{
			CloudServer.request<CloudResponse>(API_URL,
				new Dictionary<object, object>{ 
                {CloudServer.PARAM_API_KEY, "!@##%$&"},
                {CloudServer.PARAM_SESSION_TOKEN, "!@##%$&"},
                {CloudServer.PARAM_POST_ID, "!@##%$&"}});
		}

		[TestMethod]
		[ExpectedException(typeof(WammerCloudException))]
		public void HidePost_WithoutPostID_ThrowWammerCloudException()
		{
			CloudServer.request<CloudResponse>(API_URL,
				new Dictionary<object, object>{ 
                {CloudServer.PARAM_API_KEY, "!@##%$&"},
                {CloudServer.PARAM_GROUP_ID, "!@##%$&"},
                {CloudServer.PARAM_SESSION_TOKEN, "!@##%$&"}});
		}
		#endregion
	}
}
