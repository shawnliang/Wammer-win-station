using System;
using System.Diagnostics;
using log4net;
using System.Collections.Generic;

namespace Wammer.PerfMonitor
{
	public interface IPerfCounter
	{
		void Increment();
		void IncrementBy(long value);
		void Decrement();
		float NextValue();
	}

	public class PerfCounter : IPerfCounter
	{
		#region Const

		public const string CATEGORY_NAME = "Waveface Station";
		public const string AVG_TIME_PER_ATTACHMENT_UPLOAD = "Average time per attachment upload";
		public const string AVG_TIME_PER_ATTACHMENT_UPLOAD_BASE = "Average time per attachment upload base";
		public const string UPSTREAM_RATE = "Upstream rate (bytes/sec)";
		public const string DWSTREAM_RATE = "Downstream rate (bytes/sec)";
		public const string UP_REMAINED_COUNT = "Atachement upload remained count";
		public const string DW_REMAINED_COUNT = "Atachement download remained count";
		public const string ITEMS_IN_QUEUE = "Items in queue";
		public const string ITEMS_IN_PROGRESS = "Items in progress";
		public const string AVG_TIME_PER_HTTP_REQUEST = "Average time per http request";
		public const string AVG_TIME_PER_HTTP_REQUEST_BASE = "Average time per http request base";
		public const string HTTP_REQUEST_THROUGHPUT = "Http request throughput (reqs/sec)";
		public const string HTTP_REQUESTS_IN_QUEUE = "Http requests in queue";
		public const string POST_IN_QUEUE = "Post upload tasks in queue";

		#endregion

		#region Static Var		

		private static ILog _logger;
		private static Dictionary<string, IPerfCounter> _counterPool;

		#endregion

		#region Var

		private readonly PerformanceCounter Counter;
		private float _value;

		#endregion

		#region Private Static Property

		private static ILog m_Logger
		{
			get { return _logger ?? (_logger = LogManager.GetLogger("PerfCounter")); }
		}

		private static Dictionary<string, IPerfCounter> m_CounterPool
		{
			get { return _counterPool ?? (_counterPool = new Dictionary<string, IPerfCounter>()); }
		}

		#endregion


		#region Private Property

		private DateTime m_ValueUpdateTime { get; set; }

		private float m_Value
		{
			get
			{
				DateTime now = DateTime.Now;
				if ((now - m_ValueUpdateTime).TotalSeconds >= 1)
				{
					_value = Counter.NextValue();
					m_ValueUpdateTime = now;
				}

				return _value;
			}
		}

		#endregion

		private PerfCounter(PerformanceCounter counter)
		{
			Counter = counter;
		}

		#region IPerfCounter Members

		public void Increment()
		{
			lock (Counter)
			{
				Counter.Increment();
			}
		}

		public void IncrementBy(long value)
		{
			lock (Counter)
			{
				Counter.IncrementBy(value);
			}
		}

		public void Decrement()
		{
			lock (Counter)
			{
				Debug.Assert(m_Value > 0);

				if (m_Value <= 0)
					return;

				Counter.Decrement();
			}
		}


		public float NextValue()
		{
			return m_Value;
		}

		#endregion

		public static IPerfCounter GetCounter(string counterName, Boolean needInitValue = true)
		{
			try
			{
				if(m_CounterPool.ContainsKey(counterName))
					return m_CounterPool[counterName];

				var counter = new PerformanceCounter(CATEGORY_NAME, counterName, false);

				if (needInitValue)
					counter.RawValue = 0;

				m_CounterPool[counterName] = new PerfCounter(counter);

				return m_CounterPool[counterName];
			}
			catch (Exception e)
			{
				m_Logger.Warn("Unable to create counter: " + counterName, e);
				return new NullPerfCounter();
			}
		}
	}

	internal class NullPerfCounter : IPerfCounter
	{
		#region IPerfCounter Members

		public void Increment()
		{
		}

		public void IncrementBy(long value)
		{
		}

		public void Decrement()
		{
		}

		public float NextValue()
		{
			return 0;
		}

		#endregion
	}
}