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
using Waveface.Stream.Model;

namespace UT_WammerStation
{
	[TestClass]
	public class TestNewPostHandler
	{
		#region Const
		const string MONGODB_URL = @"mongodb://localhost:10319/safe=true";
		const string API_URL = @"http://localhost:8080/v2/posts/new";
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

			db.GetCollection<Driver>("drivers").RemoveAll();
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
			handler = new NewPostHandler(null);
			server.AddHandler("/v2/posts/new", handler);
			server.Start();
			server.TaskEnqueue += new EventHandler<TaskQueueEventArgs>(HttpRequestMonitor.Instance.OnTaskEnqueue);

			CloudServer.BaseUrl = "http://localhost:8080/v2/";

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
		public void NewPost_PostNewPost_Success()
		{
			var db = mongodb.GetDatabase("wammer");
			db.GetCollection("Post").RemoveAll();
			db.GetCollection("attachments").RemoveAll();
			db.GetCollection("attachments").Insert(
				new Attachment()
				{
					object_id = "12345",
					url = "http://unittest.com"
				});

			var response = CloudServer.request<NewPostResponse>(API_URL,
				new Dictionary<object, object>{ 
				{CloudServer.PARAM_API_KEY, "!@##%$&"},
				{CloudServer.PARAM_SESSION_TOKEN, "exist session token"},
				{CloudServer.PARAM_GROUP_ID, "123"},
				{CloudServer.PARAM_CONTENT, "Unit Test's content"},
				{CloudServer.PARAM_ATTACHMENT_ID_ARRAY,"[12345]"}});

			Assert.IsNotNull(response);

			var post = response.post;

			Assert.IsNotNull(post);
			Assert.AreEqual("Unit Test's content", post.content);

			var postInDB = PostCollection.Instance.FindOne(Query.EQ("_id", post.post_id));
			Assert.IsNotNull(postInDB);
			Assert.AreEqual("Unit Test's content", postInDB.content);
		}

		[TestMethod]
		[ExpectedException(typeof(WammerCloudException))]
		public void NewPost_PostWhenNoGroupID_ThrowWammerCloudException()
		{
			CloudServer.request<CloudResponse>(API_URL,
				new Dictionary<object, object>{ 
				{CloudServer.PARAM_API_KEY, "!@##%$&"},
				{CloudServer.PARAM_SESSION_TOKEN, "!@##%$&"},
				{CloudServer.PARAM_CONTENT, "Unit Test's content"}});
		}

		[TestMethod]
		[ExpectedException(typeof(WammerCloudException))]
		public void NewPost_PostWhenNoSessionToken_ThrowWammerCloudException()
		{
			CloudServer.request<CloudResponse>(API_URL,
				new Dictionary<object, object>{ 
				{CloudServer.PARAM_API_KEY, "!@##%$&"},
				{CloudServer.PARAM_GROUP_ID, "0"},
				{CloudServer.PARAM_CONTENT, "Unit Test's content"}});
		}

		[TestMethod]
		[ExpectedException(typeof(WammerCloudException))]
		public void NewPost_PostWhenNoAPIKey_ThrowWammerCloudException()
		{
			CloudServer.request<CloudResponse>(API_URL,
				new Dictionary<object, object>{ 
				{CloudServer.PARAM_SESSION_TOKEN, "!@##%$&"},
				{CloudServer.PARAM_GROUP_ID, "0"},
				{CloudServer.PARAM_CONTENT, "Unit Test's content"}});
		}
		#endregion
	}
}
