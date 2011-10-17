using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wammer.Cloud;
using System.Collections.Specialized;

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

	}
}
