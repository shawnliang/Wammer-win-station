using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using log4net;

public static class ObjectExtension
{
	#region Var

	private static Dictionary<object, ILog> _loggerPool;

	#endregion

	#region Private Property

	private static Dictionary<object, ILog> m_LoggerPool
	{
		get { return _loggerPool ?? (_loggerPool = new Dictionary<object, ILog>()); }
	}

	#endregion

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
				(p.GetCustomAttributes(typeof (DefaultValueAttribute), false) as DefaultValueAttribute[]).FirstOrDefault();
			p.SetValue(obj,
			           (defaultValue == null)
			           	? (p.PropertyType.IsValueType ? Activator.CreateInstance(p.PropertyType) : null)
			           	: defaultValue.Value, null);
		}
	}

	public static ILog GetLogger(this object obj)
	{
		lock (obj)
		{
			if (!m_LoggerPool.ContainsKey(obj))
				m_LoggerPool[obj] = LogManager.GetLogger(obj.GetType().Name);

			return m_LoggerPool[obj];
		}
	}

	public static void LogDebugMsg(this object obj, string msg)
	{
		if (string.IsNullOrEmpty(msg))
			return;

		obj.GetLogger().Debug(msg);
	}

	public static void LogDebugMsg(this object obj, string msg, Exception e)
	{
		if (string.IsNullOrEmpty(msg))
			return;

		obj.GetLogger().Debug(msg, e);
	}

	public static void LogWarnMsg(this object obj, string msg)
	{
		if (string.IsNullOrEmpty(msg))
			return;

		obj.GetLogger().Warn(msg);
	}


	public static void LogWarnMsg(this object obj, string msg, Exception e)
	{
		if (string.IsNullOrEmpty(msg))
			return;

		obj.GetLogger().Warn(msg, e);
	}

	public static void LogErrorMsg(this object obj, string msg)
	{
		if (string.IsNullOrEmpty(msg))
			return;

		obj.GetLogger().Error(msg);
	}

	public static void LogFatalMsg(this object obj, string msg)
	{
		if (string.IsNullOrEmpty(msg))
			return;

		obj.GetLogger().Fatal(msg);
	}

	#endregion
}