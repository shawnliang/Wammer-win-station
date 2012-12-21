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
using Waveface.Stream.Model;

namespace UT_WammerStation
{
	[TestClass]
	public class TestUpdatePostHandler
	{
		#region Const
		const string MONGODB_URL = @"mongodb://localhost:10319/safe=true";
		const string API_URL = @"http://localhost:8080/v2/posts/update";
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
			handler = new UpdatePostHandler(null);
			server.AddHandler("/v2/posts/update", handler);
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
		public void UpdatePost_UpdateContent_Success()
		{
			var db = mongodb.GetDatabase("wammer");
			db.GetCollection("Post").RemoveAll();
			db.GetCollection("attachments").RemoveAll();

			PostCollection.Instance.Save(
				new PostInfo()
				{
					post_id = "unitTestID",
					content = "unit test original content"
				});
			AttachmentCollection.Instance.Save(
				new Attachment()
				{
					object_id = "12345",
					url = "http://unittest.com"
				});

			var response = CloudServer.request<UpdatePostResponse>(API_URL,
				new Dictionary<object, object>{ 
				{CloudServer.PARAM_API_KEY, "!@##%$&"},
				{CloudServer.PARAM_SESSION_TOKEN, "!@##%$&"},
				{CloudServer.PARAM_GROUP_ID, "!@##%$&"},
				{CloudServer.PARAM_POST_ID, "unitTestID"},
				{CloudServer.PARAM_LAST_UPDATE_TIME, "!@##%$&"},
				{CloudServer.PARAM_CONTENT, "unit test updated content"}});

			Assert.IsNotNull(response);

			var post = response.post;

			Assert.IsNotNull(post);
			Assert.AreEqual("unit test updated content", post.content);
		}

		[Ignore]
		[TestMethod]
		public void UpdatePost_UpdatePreview_Success()
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
					content = "unit test original content"
				});
			AttachmentCollection.Instance.Save(
				new Attachment()
				{
					object_id = "12345",
					url = "http://unittest.com"
				});

			var previewJSON = fastJSON.JSON.Instance.ToJSON(new Preview()
					{
						title = "unit test updated preview"
					});

			var response = CloudServer.request<UpdatePostResponse>(API_URL,
				new Dictionary<object, object>{ 
				{CloudServer.PARAM_TYPE, "link"},
				{CloudServer.PARAM_API_KEY, "!@##%$&"},
				{CloudServer.PARAM_SESSION_TOKEN, "!@##%$&"},
				{CloudServer.PARAM_GROUP_ID, "!@##%$&"},
				{CloudServer.PARAM_POST_ID, "001"},
				{CloudServer.PARAM_LAST_UPDATE_TIME, "!@##%$&"},
				{CloudServer.PARAM_PREVIEW,previewJSON }});

			Assert.IsNotNull(response);

			var post = response.post;

			Assert.IsNotNull(post);
			Assert.AreEqual(previewJSON, fastJSON.JSON.Instance.ToJSON(post.preview));
		}

		[TestMethod]
		public void UpdatePost_UpdateAttachementIDArray_Success()
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
					attachment_id_array = new List<string> { "123" },
					content = "unit test original content"
				});
			AttachmentCollection.Instance.Save(
				new Attachment()
				{
					object_id = "456",
					url = "http://unittest.com"
				});

			var response = CloudServer.request<UpdatePostResponse>(API_URL,
				new Dictionary<object, object>{
				{CloudServer.PARAM_TYPE, "image"},
				{CloudServer.PARAM_API_KEY, "!@##%$&"},
				{CloudServer.PARAM_SESSION_TOKEN, "exist session token"},
				{CloudServer.PARAM_GROUP_ID, "!@##%$&"},
				{CloudServer.PARAM_POST_ID, "001"},
				{CloudServer.PARAM_LAST_UPDATE_TIME, "!@##%$&"},
				{CloudServer.PARAM_ATTACHMENT_ID_ARRAY, "[456]"}});

			Assert.IsNotNull(response);

			var post = response.post;

			Assert.IsNotNull(post);
			Assert.IsNotNull(post.attachment_id_array);
			Assert.AreEqual(1, post.attachment_id_array.Count);
			Assert.AreEqual("456", post.attachment_id_array.FirstOrDefault());
		}

		[TestMethod]
		public void UpdatePost_UpdateCoverAttach_Success()
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
					cover_attach = "unit test original cover attach",
					content = "unit test original content"
				});
			AttachmentCollection.Instance.Save(
				new Attachment()
				{
					object_id = "12345",
					url = "http://unittest.com"
				});

			var response = CloudServer.request<UpdatePostResponse>(API_URL,
				new Dictionary<object, object>{ 
				{CloudServer.PARAM_API_KEY, "!@##%$&"},
				{CloudServer.PARAM_SESSION_TOKEN, "!@##%$&"},
				{CloudServer.PARAM_GROUP_ID, "!@##%$&"},
				{CloudServer.PARAM_POST_ID, "001"},
				{CloudServer.PARAM_LAST_UPDATE_TIME, "!@##%$&"},
				{CloudServer.PARAM_COVER_ATTACH, "unit test updated cover attach"}});

			Assert.IsNotNull(response);

			var post = response.post;

			Assert.IsNotNull(post);
			Assert.AreEqual("unit test updated cover attach", post.cover_attach);
		}

		[TestMethod]
		public void UpdatePost_UpdateFavorite_Success()
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
					cover_attach = "unit test original cover attach",
					favorite = 0,
					content = "unit test original content"
				});
			AttachmentCollection.Instance.Save(
				new Attachment()
				{
					object_id = "12345",
					url = "http://unittest.com"
				});

			var response = CloudServer.request<UpdatePostResponse>(API_URL,
				new Dictionary<object, object>{ 
				{CloudServer.PARAM_API_KEY, "!@##%$&"},
				{CloudServer.PARAM_SESSION_TOKEN, "!@##%$&"},
				{CloudServer.PARAM_GROUP_ID, "!@##%$&"},
				{CloudServer.PARAM_POST_ID, "001"},
				{CloudServer.PARAM_LAST_UPDATE_TIME, "!@##%$&"},
				{CloudServer.PARAM_FAVORITE, 1}});

			Assert.IsNotNull(response);

			var post = response.post;

			Assert.IsNotNull(post);
			Assert.AreEqual(1, post.favorite);
		}

		[TestMethod]
		[ExpectedException(typeof(WammerCloudException))]
		public void UpdatePost_WhenNoGroupID_ThrowWammerCloudException()
		{
			CloudServer.request<CloudResponse>(API_URL,
				new Dictionary<object, object>{ 
				{CloudServer.PARAM_API_KEY, "!@##%$&"},
				{CloudServer.PARAM_SESSION_TOKEN, "!@##%$&"},
				{CloudServer.PARAM_POST_ID, "!@##%$&"},
				{CloudServer.PARAM_LAST_UPDATE_TIME, "!@##%$&"}});
		}

		[TestMethod]
		[ExpectedException(typeof(WammerCloudException))]
		public void UpdatePost_WhenNoSessionToken_ThrowWammerCloudException()
		{
			CloudServer.request<CloudResponse>(API_URL,
				new Dictionary<object, object>{ 
				{CloudServer.PARAM_API_KEY, "!@##%$&"},
				{CloudServer.PARAM_GROUP_ID, "!@##%$&"},
				{CloudServer.PARAM_POST_ID, "!@##%$&"},
				{CloudServer.PARAM_LAST_UPDATE_TIME, "!@##%$&"}});
		}


		[TestMethod]
		[ExpectedException(typeof(WammerCloudException))]
		public void UpdatePost_WhenNoPostID_ThrowWammerCloudException()
		{
			CloudServer.request<CloudResponse>(API_URL,
				new Dictionary<object, object>{ 
				{CloudServer.PARAM_API_KEY, "!@##%$&"},
				{CloudServer.PARAM_GROUP_ID, "!@##%$&"},
				{CloudServer.PARAM_SESSION_TOKEN, "!@##%$&"},
				{CloudServer.PARAM_LAST_UPDATE_TIME, "!@##%$&"}});
		}

		[TestMethod]
		[ExpectedException(typeof(WammerCloudException))]
		public void UpdatePost_WhenNoLastUpdateTime_ThrowWammerCloudException()
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
