using Microsoft.VisualStudio.TestTools.UnitTesting;

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
