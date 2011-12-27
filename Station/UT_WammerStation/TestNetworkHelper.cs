using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Wammer.Utility;
using System.Net;

namespace UT_WammerStation
{
	[TestClass]
	public class TestNetworkHelper
	{
		[TestMethod]
		public void TestMethod1()
		{
			Assert.IsTrue(NetworkHelper.IsLinkLocal(IPAddress.Parse("169.254.0.0")));
			Assert.IsTrue(NetworkHelper.IsLinkLocal(IPAddress.Parse("169.254.255.255")));
			Assert.IsFalse(NetworkHelper.IsLinkLocal(IPAddress.Parse("10.1.1.1")));
			Assert.IsFalse(NetworkHelper.IsLinkLocal(IPAddress.Parse("192.168.1.1")));
		}

		[TestMethod]
		public void TestIPAddressCompare()
		{
			IPAddress ip1 = IPAddress.Parse("10.0.0.0");
			IPAddress ip2 = IPAddress.Parse("10.0.0.0");

			Assert.IsTrue(ip1.Equals(ip2));
		}
	}
}
