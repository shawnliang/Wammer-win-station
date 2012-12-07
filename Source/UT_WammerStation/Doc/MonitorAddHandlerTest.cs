using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using Wammer.Model;
using Wammer.Station.Doc;

namespace UT_WammerStation.Doc
{
	/// <summary>
	/// Summary description for UnitTest1
	/// </summary>
	[TestClass]
	public class MonitorAddHandlerTest
	{
		public MonitorAddHandlerTest()
		{
		}

		#region Additional test attributes

		#endregion

		[TestMethod]
		public void AddNewDocToStream()
		{
			var file = @"c:\a\file.ppt";
			var docAtt = new Attachment { object_id = Guid.NewGuid().ToString(), file_path = file };

			Mock<IMonitorAddHandlerDB> db = new Mock<IMonitorAddHandlerDB>(MockBehavior.Strict);
			db.Setup(x => x.FindLatestVersion(file, "user1")).Returns(null as Wammer.Model.Attachment).Verifiable();
			db.Setup(x => x.SaveMonitorItemDB(
				It.Is<MonitorItem>(item =>
					item.user_id == "user1" &&
						item.path == file &&
						item.last_modify_time == DateTime.MinValue &&
						item.id == file + "/user1"
				))).Verifiable();

			Mock<IMonitorAddHandlerUtility> util = new Mock<IMonitorAddHandlerUtility>();

			var imp = new MonitorAddHandlerImp(db.Object, util.Object);
			imp.Process("apikey", "session", "user1", file);

			db.VerifyAll();
			util.VerifyAll();
		}

		[TestMethod]
		public void AddOldDocWhichIsAlreadyBeingMonitored()
		{
			var file = @"c:\a\file.ppt";
			var docAtt = new Attachment { object_id = Guid.NewGuid().ToString(), file_path = file };
			var monitorItem = new MonitorItem(file, "user1") { last_modify_time = DateTime.Now };
			Mock<IMonitorAddHandlerDB> db = new Mock<IMonitorAddHandlerDB>();
			db.Setup(x => x.FindLatestVersion(file, "user1")).Returns(docAtt).Verifiable();
			db.Setup(x => x.FindMonitorItem(monitorItem.id)).Returns(monitorItem).Verifiable();

			Mock<IMonitorAddHandlerUtility> util = new Mock<IMonitorAddHandlerUtility>();
			util.Setup(x => x.UpdateDocOpenTimeAsync("session", "apikey", docAtt.object_id, It.IsAny<DateTime>())).Verifiable();

			var imp = new MonitorAddHandlerImp(db.Object, util.Object);
			imp.Process("apikey", "session", "user1", file);

			db.VerifyAll();
			util.VerifyAll();
		}

		[TestMethod]
		public void AddOldDocWhichIsNotMoniteredNow()
		{
			var file = @"c:\a\file.ppt";
			var docAtt = new Attachment { object_id = Guid.NewGuid().ToString(), file_path = file, file_modify_time = DateTime.Now };
			var monitorItem = new MonitorItem(file, "user1");

			Mock<IMonitorAddHandlerDB> db = new Mock<IMonitorAddHandlerDB>();
			db.Setup(x => x.FindLatestVersion(file, "user1")).Returns(docAtt).Verifiable();
			db.Setup(x => x.FindMonitorItem(monitorItem.id)).Returns(null as MonitorItem).Verifiable();
			db.Setup(x => x.SaveMonitorItemDB(It.Is<MonitorItem>(
				y =>
					y.user_id == "user1" &&
					y.path == file &&
					y.last_modify_time == docAtt.file_modify_time))).Verifiable();

			Mock<IMonitorAddHandlerUtility> util = new Mock<IMonitorAddHandlerUtility>();

			var imp = new MonitorAddHandlerImp(db.Object, util.Object);
			imp.Process("apikey", "session", "user1", file);

			db.VerifyAll();
		}
	}
}
