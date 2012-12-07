using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ServiceProcess;
using Wammer.Utility;

namespace UT_WammerStation
{
	[TestClass]
	public class TestServiceHelper
	{
		[TestMethod]
		public void TestGetSetServiceAutoStart()
		{
			Assert.IsTrue(ServiceHelper.IsServiceAutoStart(new ServiceController("dnscache")));
			Assert.IsTrue(!ServiceHelper.IsServiceAutoStart(new ServiceController("defragsvc")));
		}

	}
}
