using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wammer.Cloud;
using Wammer.Model;
using System.Collections.Specialized;

using Wammer.Utility;

namespace UT_WammerStation
{
	[TestClass]
	public class JsonFormatTest
	{
		[TestMethod]
		public void TestUserLoginResponse()
		{
			UserLogInResponse res = fastJSON.JSON.Instance.ToObject<UserLogInResponse>(
				"{\"status\": 200, \"timestamp\": \"2011-01-01T10:20:30Z\"," +
				  "\"session_token\": \"token1\"}");

			Assert.AreEqual(200, res.status);
			Assert.AreEqual(new DateTime(2011, 1, 1, 10, 20, 30, DateTimeKind.Utc), res.timestamp.ToUniversalTime());
			Assert.AreEqual("token1", res.session_token);
		}

		[TestMethod]
		public void TestStationSignUpResponse()
		{
			StationSignUpResponse res = fastJSON.JSON.Instance.ToObject<StationSignUpResponse>(
				"{\"status\": 200, \"timestamp\": \"2011-01-01T10:20:30Z\"," +
				  "\"session_token\": \"token1\", " +
				  "\"station\": {\"station_id\": \"sid1\", \"creator_id\": \"cid1\", \"timestamp\": \"2011-10-20T10:20:30Z\", \"name\": \"name1\"} }");

			Assert.AreEqual(200, res.status);
			Assert.AreEqual(new DateTime(2011, 1, 1, 10, 20, 30, DateTimeKind.Utc), res.timestamp.ToUniversalTime());
			Assert.AreEqual("token1", res.session_token);
		}

		[TestMethod]
		public void TestStationLogOnResponse()
		{
			StationLogOnResponse res = fastJSON.JSON.Instance.ToObject<StationLogOnResponse>(
				"{\"status\": 200, \"timestamp\": \"2011-01-01T10:20:30Z\"," +
				  "\"session_token\": \"token1\" }");

			Assert.AreEqual(200, res.status);
			Assert.AreEqual(new DateTime(2011, 1, 1, 10, 20, 30, DateTimeKind.Utc), res.timestamp.ToUniversalTime());
			Assert.AreEqual("token1", res.session_token);
		}

		[TestMethod]
		public void TestAttachment_RawDataIsNotSerialized()
		{
			Attachment a = new Attachment
			{
				RawData = new ArraySegment<byte>(new byte[100]),
				title = "123"
			};

			string json = a.ToFastJSON();

			Attachment result = fastJSON.JSON.Instance.ToObject<Attachment>(json);
			Assert.IsNull(result.RawData.Array);
		}

		[TestMethod]
		public void testJsonArray()
		{
			MyClass2 obj = new MyClass2 { Array = new List<MyClass3>() };
			obj.Array.Add(new MyClass3 { Data = "123" });
			obj.Array.Add(new MyClass3 { Data = "456" });

			string json = obj.ToFastJSON();
			Assert.AreEqual("{\"Array\":[{\"Data\":\"123\"},{\"Data\":\"456\"}]}", json);
		}

		public class A1
		{
			public DateTime date { get; set; }
		}

		[TestMethod]
		public void testDateTimeLocal()
		{
			fastJSON.JSON.Instance.UseUTCDateTime = true;
			A1 a = new A1 { date = new DateTime(2000, 1, 2, 3, 4, 5, DateTimeKind.Local) };
			string json = a.ToFastJSON();
			Assert.AreEqual("{\"date\":\"" + a.date.ToCloudTimeString() + "\"}", json);
		}

		[TestMethod]
		public void testDateTimeUtc()
		{
			fastJSON.JSON.Instance.UseUTCDateTime = true;
			A1 a = new A1 { date = new DateTime(2000, 1, 2, 3, 4, 5, DateTimeKind.Utc) };
			string json = a.ToFastJSON();
			Assert.AreEqual("{\"date\":\"" + a.date.ToCloudTimeString() + "\"}", json);
		}
	}
}
