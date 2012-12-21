using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Wammer.Utility;
using Waveface.Stream.Model;

namespace UT_WammerStation
{
	[TestClass]
	public class TestTimeHelper
	{
		[TestMethod]
		public void ParseTime()
		{
			DateTime result = TimeHelper.ParseCloudTimeString("2010-10-01T01:02:30Z");
			Assert.AreEqual(new DateTime(2010, 10, 1, 1, 2, 30, DateTimeKind.Utc), result);
			Assert.AreEqual(DateTimeKind.Utc, result.Kind);
		}
	}
}
