using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Wammer.Station.LocalUserTrack;

namespace UT_WammerStation.LocalUserTrack
{
	[TestClass]
	public class TestLocalUTMgr
	{

		[TestMethod]
		public void SmokeTest()
		{
			LocalUserTrackManager mgr = new LocalUserTrackManager();
			var tracks = mgr.getUserTracksBySession("g2", "session1");
			Assert.IsFalse(tracks.Any());
		}

		[TestMethod]
		public void TestAdd2Get2()
		{
			LocalUserTrackManager mgr = new LocalUserTrackManager();
			mgr.AddUserTrack("g1", "o1", "p1");
			mgr.AddUserTrack("g1", "o2", "p2");

			var tracks = mgr.getUserTracksBySession("g1", "session1");
			Assert.AreEqual(2, tracks.Count());

			var ut = tracks.Where(x => x.target_id == "o1").First();
			Assert.AreEqual("o1", ut.target_id);
			Assert.AreEqual("g1", ut.group_id);
			Assert.AreEqual("attachment", ut.target_type);
			Assert.AreEqual(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc), ut.timestamp);
			Assert.AreEqual(1, ut.actions.Count);
			Assert.AreEqual("image.medium", ut.actions[0].target_type);
			Assert.AreEqual("p1", ut.actions[0].post_id);
			Assert.AreEqual("add", ut.actions[0].action);

			ut = tracks.Where(x => x.target_id == "o2").First();
			Assert.AreEqual("o2", ut.target_id);
			Assert.AreEqual("g1", ut.group_id);
			Assert.AreEqual("attachment", ut.target_type);
			Assert.AreEqual(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc), ut.timestamp);
			Assert.AreEqual(1, ut.actions.Count);
			Assert.AreEqual("image.medium", ut.actions[0].target_type);
			Assert.AreEqual("p2", ut.actions[0].post_id);
			Assert.AreEqual("add", ut.actions[0].action);
		}

		[TestMethod]
		public void TestAdd2Remove1Get1()
		{
			LocalUserTrackManager mgr = new LocalUserTrackManager();
			mgr.AddUserTrack("g1", "o1", "p1");
			mgr.AddUserTrack("g1", "o2", "p2");
			mgr.RemoveUserTrack("g1", "o1");

			var tracks = mgr.getUserTracksBySession("g1", "session1");
			Assert.AreEqual(1, tracks.Count());

			var ut = tracks.Where(x => x.target_id == "o2").First();
			Assert.AreEqual("o2", ut.target_id);
			Assert.AreEqual("g1", ut.group_id);

		}

		[TestMethod]
		public void TestDifferentGroupsDoesNotMix()
		{
			LocalUserTrackManager mgr = new LocalUserTrackManager();
			mgr.AddUserTrack("g1", "o1", "p1");
			mgr.AddUserTrack("g2", "o2", "p2");


			var tracks = mgr.getUserTracksBySession("g1", "session1");
			Assert.AreEqual(1, tracks.Count());

			var ut = tracks.Where(x => x.target_id == "o1").First();
			Assert.AreEqual("o1", ut.target_id);
			Assert.AreEqual("g1", ut.group_id);

			tracks = mgr.getUserTracksBySession("g2", "session2");
			Assert.AreEqual(1, tracks.Count());

			ut = tracks.Where(x => x.target_id == "o2").First();
			Assert.AreEqual("o2", ut.target_id);
			Assert.AreEqual("g2", ut.group_id);
		}

		[TestMethod]
		public void TestSameSessionWontGetSameUserTrack()
		{
			LocalUserTrackManager mgr = new LocalUserTrackManager();
			mgr.AddUserTrack("g1", "o1", "p1");
			mgr.getUserTracksBySession("g1", "session1");

			mgr.AddUserTrack("g1", "o2", "p2");
			var tracks = mgr.getUserTracksBySession("g1", "session1");
			Assert.AreEqual(1, tracks.Count());
			Assert.AreEqual("o2", tracks.First().target_id);
		}


		[TestMethod]
		public void TestAdd3Get3Remove3()
		{
			LocalUserTrackManager mgr = new LocalUserTrackManager();
			mgr.AddUserTrack("g1", "o1", "p1");
			mgr.AddUserTrack("g1", "o2", "p2");
			mgr.AddUserTrack("g1", "o3", "p3");
			var tracks = mgr.getUserTracksBySession("g1", "session1");
			Assert.AreEqual(3, tracks.Count());

			mgr.RemoveUserTrack("g1", "o1");
			mgr.RemoveUserTrack("g1", "o2");
			mgr.RemoveUserTrack("g1", "o3");

			tracks = mgr.getUserTracksBySession("g1", "session1");
			Assert.AreEqual(0, tracks.Count());
		}


	}
}
