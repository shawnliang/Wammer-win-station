using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wammer.Utility;

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
		}
	}
}
