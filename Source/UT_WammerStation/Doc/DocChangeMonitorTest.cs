using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.IO;
using Wammer.Model;
using Wammer.Station.Doc;
using Waveface.Stream.Model;

namespace UT_WammerStation.Doc
{
	[TestClass]
	public class DocChangeMonitorTest
	{
		[TestMethod]
		public void ProcessChangedDocs()
		{
			Mock<IDocumentChangeMonitorDB> db = new Mock<IDocumentChangeMonitorDB>();
			Mock<IDocumentChangeMonitorUtil> util = new Mock<IDocumentChangeMonitorUtil>();

			var monitor = new DocumentChangeMonitor(db.Object, util.Object);


			var retItems = new MonitorItem[] { 
				new MonitorItem("p1", "u1") { last_modify_time = DateTime.Now.AddDays(-3.0) },
				new MonitorItem("p2", "u2") { last_modify_time = DateTime.Now.AddDays(-4.0) },
			};

			db.Setup(x => x.FindAllMonitorItems()).Returns(retItems);
			var now = DateTime.Now;
			util.Setup(x => x.GetFileWriteTime("p1")).Returns(now);
			util.Setup(x => x.GetFileWriteTime("p2")).Returns(DateTime.Now.AddDays(-10.0));
			util.Setup(x => x.ProcessChangedDoc(retItems[0], now)).Verifiable();
			db.Setup(x => x.UpdateMonitorItem(
				It.Is<MonitorItem>(item =>
					item.last_modify_time == now &&
					item.user_id == "u1" &&
					item.path == "p1"
				))).Verifiable();

			monitor.ProcessChangedDocs();

			db.VerifyAll();
			util.VerifyAll();
		}

		[TestMethod]
		public void ErrorInGetFileWriteTime()
		{
			Mock<IDocumentChangeMonitorDB> db = new Mock<IDocumentChangeMonitorDB>();
			Mock<IDocumentChangeMonitorUtil> util = new Mock<IDocumentChangeMonitorUtil>();

			var monitor = new DocumentChangeMonitor(db.Object, util.Object);


			var retItems = new MonitorItem[] { 
				new MonitorItem("p1", "u1") { last_modify_time = DateTime.Now.AddDays(-3.0) },
				new MonitorItem("p2", "u2") { last_modify_time = DateTime.Now.AddDays(-4.0) },
			};

			db.Setup(x => x.FindAllMonitorItems()).Returns(retItems);
			var now = DateTime.Now;
			util.Setup(x => x.GetFileWriteTime("p1")).Throws(new IOException()).Verifiable();
			util.Setup(x => x.GetFileWriteTime("p2")).Returns(DateTime.Now.AddDays(-10.0)).Verifiable();

			monitor.ProcessChangedDocs();

			db.VerifyAll();
			util.VerifyAll();
		}

		[TestMethod]
		public void ErrorInPRocessChangedDoc()
		{
			Mock<IDocumentChangeMonitorDB> db = new Mock<IDocumentChangeMonitorDB>();
			Mock<IDocumentChangeMonitorUtil> util = new Mock<IDocumentChangeMonitorUtil>();

			var monitor = new DocumentChangeMonitor(db.Object, util.Object);


			var retItems = new MonitorItem[] { 
				new MonitorItem("p1", "u1") { last_modify_time = DateTime.Now.AddDays(-3.0) },
				new MonitorItem("p2", "u2") { last_modify_time = DateTime.Now.AddDays(-4.0) },
			};

			db.Setup(x => x.FindAllMonitorItems()).Returns(retItems);
			var now = DateTime.Now;
			util.Setup(x => x.GetFileWriteTime("p1")).Returns(now);
			util.Setup(x => x.GetFileWriteTime("p2")).Returns(DateTime.Now.AddDays(-10.0));
			util.Setup(x => x.ProcessChangedDoc(retItems[0], now)).Throws(new WammerCloudException()).Verifiable();


			monitor.ProcessChangedDocs();

			db.VerifyAll();
			util.VerifyAll();
		}
	}
}
