using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

public static class ObjectExtension
{
	#region Public Method

	public static Boolean IsNull(this object obj)
	{
		return obj == null;
	}

	public static void RaiseEvent(this object obj, EventHandler handler, EventArgs e)
	{
		RaiseEvent(obj, handler, () => e);
	}

	public static void RaiseEvent(this object obj, EventHandler handler, Func<EventArgs> func)
	{
		if (handler == null)
			return;
		handler(obj, func());
	}

	public static void RaiseEvent<TEventArgs>(this object obj, EventHandler<TEventArgs> handler, TEventArgs e)
		where TEventArgs : EventArgs
	{
		RaiseEvent(obj, handler, () => e);
	}

	public static void RaiseEvent<TEventArgs>(this object obj, EventHandler<TEventArgs> handler, Func<TEventArgs> func)
		where TEventArgs : EventArgs
	{
		if (handler == null)
			return;
		handler(obj, func());
	}

	public static void Reset(this object obj)
	{
		foreach (
			PropertyInfo p in obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
		{
			if (!p.CanWrite)
				continue;
			DefaultValueAttribute defaultValue =
				(p.GetCustomAttributes(typeof(DefaultValueAttribute), false) as DefaultValueAttribute[]).FirstOrDefault();
			p.SetValue(obj,
					   (defaultValue == null)
						? (p.PropertyType.IsValueType ? Activator.CreateInstance(p.PropertyType) : null)
						: defaultValue.Value, null);
		}
	}


	public static IEnumerable<T> GetCustomAttributes<T>(this object obj)
	{
		return obj.GetType().GetCustomAttributes(typeof(T), false).Cast<T>();
	}

	public static T GetCustomAttribute<T>(this object obj)
	{
		return GetCustomAttributes<T>(obj).FirstOrDefault();
	}

	public static IEnumerable<T> GetCustomAttributes<T>(this object obj, string memberName)
	{
		var m = obj.GetType().GetMember(memberName).FirstOrDefault();
		if (m == null)
		{
			return new T[0];
		}
		return m.GetCustomAttributes(typeof(T), false).OfType<T>();
	}

	public static T GetCustomAttribute<T>(this object obj, string memberName)
	{
		return GetCustomAttributes<T>(obj, memberName).FirstOrDefault();
	}
	#endregion
}