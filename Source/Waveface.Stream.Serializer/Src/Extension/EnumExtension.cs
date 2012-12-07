using System;
using System.Collections.Generic;
using System.Linq;

namespace Waveface.Stream.Serializer
{
	public static class EnumExtension
	{
		public static IEnumerable<T> GetCustomAttributes<T>(this Enum e)
		{
			return
				e.GetType().GetField(e.ToString()).GetCustomAttributes(typeof(T), false).Cast<T>();
		}

		public static T GetCustomAttribute<T>(this Enum e)
		{
			return GetCustomAttributes<T>(e).FirstOrDefault();
		}
	}
}