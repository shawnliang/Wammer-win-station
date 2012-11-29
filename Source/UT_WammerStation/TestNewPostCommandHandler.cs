using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using Wammer;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.PerfMonitor;
using Wammer.Station;

namespace UT_WammerStation
{
	[TestClass]
	public class TestNewPostCommandHandler
	{
		#region Const
		const string MONGODB_URL = @"mongodb://localhost:10319/safe=true";
		const string API_URL = @"http://localhost:8080/v2/posts/newComment";
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
			db.GetCollection<LoginedSession>("attachments").RemoveAll();

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
			handler = new NewPostCommentHandler(null);
			server.AddHandler("/v2/posts/newComment", handler);
			server.Start();
			server.TaskEnqueue += new EventHandler<TaskQueueEventArgs>(HttpRequestMonitor.Instance.OnTaskEnqueue);

			CloudServer.BaseUrl = "http://localhost/v2/";			

			mongodb.GetDatabase("wammer").GetCollection<Driver>("drivers").Insert(
				new Driver
				{
					user_id = "exist_uid",
					email = "exist@gmail.com",
					folder = "resource\\user_exist_uid",
					session_token = "exist session token",
					groups = new List<UserGroup> { new UserGroup { group_id = "123" } }
				});

			LoginedSessionCollection.Instance.Save(
				new LoginedSession()
				{
					session_token = "exist session token",
					device = new Device() { device_id = "unit test device id", device_name = "unit test device"},
					apikey = new Apikey() { name = "window"}
				});	
		}

		[TestCleanup]
		public void tearDown()
		{
			Reset();
		}


		[TestMethod]
		public void NewPostComment_AddComment_Success()
		{
			var db = mongodb.GetDatabase("wammer");
			db.GetCollection("Post").RemoveAll();

			var existedPost = new PostInfo()
				{
					post_id = "unitTestID",
					content = "unit test original content",
					comment_count = 1,
					comments = new List<Comment>(){ new Comment()
					{
						content = "first unit test post comment"
					}}
				};

			PostCollection.Instance.Save(existedPost);	
			var response = CloudServer.request<NewPostCommentResponse>(API_URL,
				new Dictionary<object, object>{ 
				{CloudServer.PARAM_API_KEY, "!@##%$&"},
				{CloudServer.PARAM_GROUP_ID, "123"},
				{CloudServer.PARAM_SESSION_TOKEN, "exist session token"},
				{CloudServer.PARAM_POST_ID, "unitTestID"},
				{CloudServer.PARAM_CONTENT, "secondary unit test post comment"}});

			Assert.IsNotNull(response);

			var post = response.post;
			Assert.IsNotNull(post);
			Assert.AreEqual(2, post.comment_count);

			Assert.IsNotNull(post.comments);
			Assert.AreEqual(post.comment_count, post.comments.Count);

			var comment = post.comments.Last();
			Assert.IsNotNull(comment);
			Assert.AreEqual("secondary unit test post comment", comment.content);
		}

	
		[TestMethod]
		[ExpectedException(typeof(WammerCloudException))]
		public void NewPostComment_WithoutAPIKey_ThrowWammerCloudException()
		{
			CloudServer.request<CloudResponse>(API_URL,
				new Dictionary<object, object>{ 
				{CloudServer.PARAM_GROUP_ID, "!@##%$&"},
				{CloudServer.PARAM_SESSION_TOKEN, "!@##%$&"},
				{CloudServer.PARAM_POST_ID, "!@##%$&"},
				{CloudServer.PARAM_CONTENT, "!@##%$&"}});
		}

		[TestMethod]
		[ExpectedException(typeof(WammerCloudException))]
		public void NewPostComment_WithoutGroupID_ThrowWammerCloudException()
		{
			CloudServer.request<CloudResponse>(API_URL,
				new Dictionary<object, object>{ 
				{CloudServer.PARAM_API_KEY, "!@##%$&"},
				{CloudServer.PARAM_SESSION_TOKEN, "!@##%$&"},
				{CloudServer.PARAM_POST_ID, "!@##%$&"},
				{CloudServer.PARAM_CONTENT, "!@##%$&"}});
		}

		[TestMethod]
		[ExpectedException(typeof(WammerCloudException))]
		public void NewPostComment_WithoutSessionToken_ThrowWammerCloudException()
		{
			CloudServer.request<CloudResponse>(API_URL,
				new Dictionary<object, object>{ 
				{CloudServer.PARAM_API_KEY, "!@##%$&"},
				{CloudServer.PARAM_GROUP_ID, "!@##%$&"},
				{CloudServer.PARAM_POST_ID, "!@##%$&"},
				{CloudServer.PARAM_CONTENT, "!@##%$&"}});
		}

		[TestMethod]
		[ExpectedException(typeof(WammerCloudException))]
		public void NewPostComment_WithoutPostID_ThrowWammerCloudException()
		{
			CloudServer.request<CloudResponse>(API_URL,
				new Dictionary<object, object>{ 
				{CloudServer.PARAM_API_KEY, "!@##%$&"},
				{CloudServer.PARAM_GROUP_ID, "!@##%$&"},
				{CloudServer.PARAM_SESSION_TOKEN, "!@##%$&"},
				{CloudServer.PARAM_CONTENT, "!@##%$&"}});
		}

		[TestMethod]
		[ExpectedException(typeof(WammerCloudException))]
		public void NewPostComment_WithoutComment_ThrowWammerCloudException()
		{
			CloudServer.request<CloudResponse>(API_URL,
				new Dictionary<object, object>{ 
				{CloudServer.PARAM_API_KEY, "!@##%$&"},
				{CloudServer.PARAM_GROUP_ID, "!@##%$&"},
				{CloudServer.PARAM_SESSION_TOKEN, "!@##%$&"},
				{CloudServer.PARAM_POST_ID, "!@##%$&"}});
		}
		#endregion
	}
}
