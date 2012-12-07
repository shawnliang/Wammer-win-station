using System;
using System.Globalization;

namespace Waveface.Stream.Model
{
	public static class DateTimeExtenstion
	{
		public static string ToUTCISO8601ShortString(this DateTime time)
		{
			return string.Concat(time.ToUniversalTime().ToString("s", CultureInfo.InvariantCulture), "Z");
		}

		public static string ToUTCISO8601LongString(this DateTime time)
		{
			return time.ToUniversalTime().ToString("o", CultureInfo.InvariantCulture);
		}

		public static string ToLocalISO8601ShortString(this DateTime time)
		{
			return string.Concat(time.ToLocalTime().ToString("s", CultureInfo.InvariantCulture), "Z");
		}

		public static string ToLocalISO8601LongString(this DateTime time)
		{
			return time.ToLocalTime().ToString("o", CultureInfo.InvariantCulture);
		}
	}
}
