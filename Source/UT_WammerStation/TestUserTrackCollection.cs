using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wammer.Model;
using MongoDB.Driver;

namespace UT_WammerStation
{
	[TestClass]
	public class TestUserTrackCollection
	{
		[TestMethod]
		public void TestSaveAndLoad()
		{
			UserTrackCollection.Instance.RemoveAll();
			UserTrackCollection.Instance.Save(new UserTracks { group_id = "123" });
			UserTrackCollection.Instance.Save(new UserTracks { group_id = "456" });

			MongoCursor<UserTracks> uts = UserTrackCollection.Instance.FindAll();
			Assert.AreEqual(2L, uts.Count());
		}
	}
}
