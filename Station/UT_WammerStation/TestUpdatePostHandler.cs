using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.IO;
using System.Web;
using System.ServiceModel;
using System.ServiceModel.Web;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Wammer.Cloud;
using Wammer.Station;
using Wammer.Model;
using Wammer.Station.Management;

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
			handler = new NewPostHandler(null);
			server.AddHandler("/v2/posts/update", handler);
			server.Start();

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

			mongodb.GetDatabase("wammer").GetCollection<LoginedSession>("LoginedSession").Insert(
				new LoginedSession()
				{
					session_token = "exist session token",
					apikey = new Apikey() { name = "window"}
				});	
		}

		[TestCleanup]
		public void tearDown()
		{
			Reset();
		}

		//[TestMethod]
		//public void UpdatePost_UpdateContent_Success()
		//{
		//    var db = mongodb.GetDatabase("wammer");
		//    db.GetCollection("Post").RemoveAll();
		//    db.GetCollection<LoginedSession>("attachments").RemoveAll();
		//    db.GetCollection<LoginedSession>("attachments").Insert(
		//        new Attachment()
		//        {
		//            object_id = "12345",
		//            url = "http://unittest.com"
		//        });

		//    var response = CloudServer.request<NewPostResponse>(new WebClient(), API_URL,
		//        new Dictionary<object, object>{ 
		//        {CloudServer.PARAM_API_KEY, "!@##%$&"},
		//        {CloudServer.PARAM_SESSION_TOKEN, "exist session token"},
		//        {CloudServer.PARAM_GROUP_ID, "123"},
		//        {CloudServer.PARAM_CONTENT, "Unit Test's content"},
		//        {CloudServer.PARAM_ATTACHMENT_ID_ARRAY,"[12345]"}});

		//    Assert.IsNotNull(response);

		//    var post = response.posts.FirstOrDefault();

		//    Assert.IsNotNull(post);
		//    Assert.AreEqual("Unit Test's content", post.content);

		//    Assert.IsNotNull(post.attachments);
		//    Assert.IsTrue(post.attachments.Count > 0);
		//    Assert.AreEqual("http://unittest.com", post.attachments[0].url);

		//    var postInDB = PostCollection.Instance.FindOne(Query.EQ("_id", post.post_id));
		//    Assert.IsNotNull(postInDB);
		//    Assert.AreEqual("Unit Test's content", postInDB.content);
		//}

		[TestMethod]
		public void UpdatePost_UpdateContent_Success()
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

			var response = CloudServer.request<UpdatePostResponse>(new WebClient(), API_URL,
				new Dictionary<object, object>{ 
				{CloudServer.PARAM_API_KEY, "!@##%$&"},
				{CloudServer.PARAM_SESSION_TOKEN, "!@##%$&"},
				{CloudServer.PARAM_GROUP_ID, "!@##%$&"},
				{CloudServer.PARAM_POST_ID, "!@##%$&"},
				{CloudServer.PARAM_LAST_UPDATE_TIME, "!@##%$&"},
				{CloudServer.PARAM_CONTENT, "unit test content"}});


		}

		[TestMethod]
		public void UpdatePost_UpdatePreview_Success()
		{
		}

		[TestMethod]
		public void UpdatePost_UpdateAttachementIDArray_Success()
		{
		}

		[TestMethod]
		public void UpdatePost_UpdateCoverAttach_Success()
		{
		}

		[TestMethod]
		public void UpdatePost_UpdateFavorite_Success()
		{
		}

		[TestMethod]
		[ExpectedException(typeof(WammerCloudException))]
		public void UpdatePost_WhenNoGroupID_ThrowWammerCloudException()
		{
			CloudServer.request<CloudResponse>(new WebClient(), API_URL,
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
			CloudServer.request<CloudResponse>(new WebClient(), API_URL,
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
			CloudServer.request<CloudResponse>(new WebClient(), API_URL,
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
			CloudServer.request<CloudResponse>(new WebClient(), API_URL,
				new Dictionary<object, object>{ 
				{CloudServer.PARAM_API_KEY, "!@##%$&"},
				{CloudServer.PARAM_GROUP_ID, "!@##%$&"},
				{CloudServer.PARAM_SESSION_TOKEN, "!@##%$&"},
				{CloudServer.PARAM_POST_ID, "!@##%$&"}});
		}
		#endregion
	}
}
