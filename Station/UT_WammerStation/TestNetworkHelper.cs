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
		public void TestIPAddressCompare()
		{
			IPAddress ip1 = IPAddress.Parse("10.0.0.0");
			IPAddress ip2 = IPAddress.Parse("10.0.0.0");

			Assert.IsTrue(ip1.Equals(ip2));
		}
	}
}
