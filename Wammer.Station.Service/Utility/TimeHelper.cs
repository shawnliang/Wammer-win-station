using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wammer.Utility
{
	static class TimeHelper
	{
		private static DateTime JAN_1_1970 = new DateTime(1970, 1, 1);

		public static long GetMillisecondsSince1970()
		{
			TimeSpan span = DateTime.Now - JAN_1_1970;
			return (long)span.TotalMilliseconds;
		}

		public static DateTime ConvertToDateTime(long unixTimeStamp)
		{
			return JAN_1_1970.AddMilliseconds(unixTimeStamp);
		}

		public static long ConvertToUnixTimeStamp(DateTime datetime)
		{
			return (long)(datetime - JAN_1_1970).TotalMilliseconds;
		}
	}
}
