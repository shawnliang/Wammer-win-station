using System;
using System.Reflection;
using System.Linq;
using System.ComponentModel;

public static class ObjectExtension
{
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

	public static void RaiseEvent<TEventArgs>(this object obj, EventHandler<TEventArgs> handler, TEventArgs e) where TEventArgs : EventArgs
	{
		RaiseEvent(obj, handler, () => e);
	}

	public static void RaiseEvent<TEventArgs>(this object obj, EventHandler<TEventArgs> handler, Func<TEventArgs> func) where TEventArgs : EventArgs
	{
		if (handler == null)
			return;
		handler(obj, func());
	}

	public static void Reset(this object obj)
	{
		foreach (var p in obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
		{
			if (!p.CanWrite)
				continue;
			var defaultValue = (p.GetCustomAttributes(typeof(DefaultValueAttribute), false) as DefaultValueAttribute[]).FirstOrDefault();
			p.SetValue(obj, (defaultValue == null) ? (p.PropertyType.IsValueType ? Activator.CreateInstance(p.PropertyType) : null) : defaultValue.Value, null);
		}
	}
}