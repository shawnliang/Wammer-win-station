using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;

public static class EnumExtension
{
	public static IEnumerable<T> GetCustomAttributes<T>(this Enum e)
	{
		return e.GetType().GetField(e.ToString()).GetCustomAttributes(typeof(T), false).Cast<T>();
	}

	public static T GetCustomAttribute<T>(this Enum e)
	{
		return GetCustomAttributes<T>(e).FirstOrDefault();
	}

	public static bool HasFlag(this Enum variable, Enum value)
	{
		if (variable == null)
			return false;

		if (value == null)
			throw new ArgumentNullException("value");

		// Not as good as the .NET 4 version of this function, but should be good enough
		if (!Enum.IsDefined(variable.GetType(), value))
		{
			throw new ArgumentException(string.Format(
				"Enumeration type mismatch.  The flag is of type '{0}', was expecting '{1}'.",
				value.GetType(), variable.GetType()));
		}

		ulong num = Convert.ToUInt64(value);
		return ((Convert.ToUInt64(variable) & num) == num);

	}
}
