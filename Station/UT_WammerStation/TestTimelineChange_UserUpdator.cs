using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wammer.Station.Timeline;
using Wammer.Model;
using MongoDB.Driver.Builders;

namespace UT_WammerStation
{
	[TestClass]
	public class TestTimelineChange_UserUpdator
	{
		[TestMethod]
		public void TestUpdateSyncTime()
		{
			DriverCollection.Instance.Save(new Driver() { user_id = "u1" });

			UserInfoUpdator updator = new UserInfoUpdator();
			updator.UpdateChangeLogSyncTime("u1", "2012-3-4T1:2:3Z");

			Driver savedData = DriverCollection.Instance.FindOne(Query.EQ("_id", "u1"));
			Assert.AreEqual("2012-3-4T1:2:3Z", savedData.change_log_sync_time);
		}
	}
}
