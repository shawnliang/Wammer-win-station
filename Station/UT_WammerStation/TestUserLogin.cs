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
	public class TestUserLogin
	{
		#region Const
		const string MONGODB_URL = @"mongodb://localhost:10319/safe=true";
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
			db.GetCollection("LoginedSession").RemoveAll();

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
			handler = new UserLoginHandler();
			server.AddHandler("/v2/auth/login", handler);
			server.Start();

			CloudServer.BaseUrl = "http://localhost/v2/";			

			mongodb.GetDatabase("wammer").GetCollection<Driver>("drivers").Insert(
				new Driver
				{
					user_id = "exist_uid",
					email = "exist@gmail.com",
					folder = "resource\\user_exist_uid",
					groups = new List<UserGroup> { new UserGroup { group_id = "123" } }
				});
		}

		[TestCleanup]
		public void tearDown()
		{
			Reset();
		}

		[TestMethod]
		[ExpectedException(typeof(WammerCloudException))]
		public void UserLogin_LoginNotExistUser_ThrowWammerCloudException()
		{
			using (FakeCloud cloud = new FakeCloud(string.Empty))
			{
				CloudServer.request<CloudResponse>(new WebClient(), "http://localhost:8080/v2/auth/login",
					new Dictionary<object, object>{ 
					{ "email", "user1@gmail.com"}, 
					{ "password", "12345"} ,
					{"apikey", "!@#!$%@^$%&*%)(%#$"}});
			}
		}

		[TestMethod]
		public void UserLogin_LoginExistUser_LoginSuccess()
		{
			string loginedJson = @"{
    ""status"": 200,
    ""session_token"": ""cV91P1HmYm5PqR9Z75DxAd0t.H5MOppJa3gpk26o6bjc3RDyAmOMNGMc4Xj8znaAMRvY"",
    ""session_expires"": ""2012-01-07T17:15:59Z"",
    ""timestamp"": ""2011-11-08T17:15:59.789Z"",
    ""api_ret_code"": 0,
    ""api_ret_message"": ""success"",
    ""debug"": {
        ""connection_id"": ""132077255971500244""
    },
    ""device"": {
        ""device_name"": ""CamgeIphone"",
        ""device_id"": ""device01-ea71-44b9-941d-5285568114b6""
    },
    ""apikey"": {
        ""apikey"": ""e96546fa-3ed5-540a-9ef2-1f8ce1dc60f2"",
        ""name"": ""Android""
    },
    ""user"": {
        ""user_id"": ""driver01-ea71-44b9-941d-5285568114b6"",
        ""devices"": [
            {
                ""device_id"": ""device0001"",
                ""device_name"": ""Driver01's Automation""
            },
            {
                ""device_name"": ""CamgeIphone"",
                ""device_id"": ""device01-ea71-44b9-941d-5285568114b6""
            }
        ],
        ""state"": ""registered"",
        ""avatar_url"": ""http://www.gravatar.com/avatar/cc6a52227aa2f7a09e7e0221efa65ae6.jpg"",
        ""verified"": true,
        ""nickname"": ""=羅健志 (dr01)="",
        ""email"": ""camge.lo@waveface.com""
    },
    ""groups"": [
        {
            ""group_id"": ""8f0c5944-403c-4a81-977d-b96b0eff8177"",
            ""description"": """",
            ""station_id"": ""30571ece-0ab2-11e1-9cb2-3c0754038dd9"",
            ""creator_id"": ""c971ab50-8ffb-4592-8ef6-4a22ca0e23da"",
            ""name"": ""steven group""
        }
    ],
    ""stations"": [
        {
            ""status"": ""connected"",
            ""timestamp"": 1320829620,
            ""station_id"": ""30571ece-0ab2-11e1-9cb2-3c0754038dd9"",
            ""creator_id"": ""c971ab50-8ffb-4592-8ef6-4a22ca0e23da"",
            ""location"": ""http://myhome:8088/api"",
            ""last_seen"": 1320829620
        }
    ]
}";

			using (FakeCloud cloud = new FakeCloud(loginedJson))
			{
				CloudServer.request<CloudResponse>(new WebClient(), "http://localhost:8080/v2/auth/login",
					new Dictionary<object, object>{ 
					{ "email", "exist@gmail.com"}, 
					{ "password", "12345"} ,
					{"apikey", "!@#!$%@^$%&*%)(%#$"}});
			}
		} 
		#endregion
	}
}
