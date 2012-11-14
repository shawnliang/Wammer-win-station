using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Wammer.Utility;
using System.ServiceProcess;

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
