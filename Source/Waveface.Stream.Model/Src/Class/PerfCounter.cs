using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Waveface.Stream.Model
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
		public const string ATTACHMENT_UPLOAD_COUNT = "Attachment upload count";
		public const string SMALL_THUMBNAIL_GENERATE_COUNT = "Small thumbnail generate count";
		public const string MEDIUM_THUMBNAIL_GENERATE_COUNT = "Medium thumbnail generate count";
		#endregion



		#region Static Var
		private static Dictionary<string, IPerfCounter> _counterPool;
		private static Object _counterPoolLockObj;
		#endregion


		#region Var
		private float _value;
		#endregion


		#region Private Static Property

		/// <summary>
		/// Gets the m_ counter pool.
		/// </summary>
		/// <value>The m_ counter pool.</value>
		private static Dictionary<string, IPerfCounter> m_CounterPool
		{
			get { return _counterPool ?? (_counterPool = new Dictionary<string, IPerfCounter>()); }
		}

		/// <summary>
		/// Gets the m_ counter pool lock obj.
		/// </summary>
		/// <value>The m_ counter pool lock obj.</value>
		private static Object m_CounterPoolLockObj
		{
			get
			{
				return _counterPoolLockObj ?? (_counterPoolLockObj = new Object());
			}
		}
		#endregion


		#region Private Property
		/// <summary>
		/// Gets or sets the m_ counter.
		/// </summary>
		/// <value>The m_ counter.</value>
		private PerformanceCounter m_Counter { get; set; }

		/// <summary>
		/// Gets or sets the m_ value update time.
		/// </summary>
		/// <value>The m_ value update time.</value>
		private DateTime m_ValueUpdateTime { get; set; }

		/// <summary>
		/// Gets the m_ value.
		/// </summary>
		/// <value>The m_ value.</value>
		private float m_Value
		{
			get
			{
				DateTime now = DateTime.Now;
				if ((now - m_ValueUpdateTime).TotalSeconds >= 1)
				{
					_value = m_Counter.NextValue();
					m_ValueUpdateTime = now;
				}

				return _value;
			}
		}
		#endregion



		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="PerfCounter"/> class.
		/// </summary>
		/// <param name="counter">The counter.</param>
		private PerfCounter(PerformanceCounter counter)
		{
			m_Counter = counter;
		}
		#endregion


		#region Public Static Method
		/// <summary>
		/// Gets the counter.
		/// </summary>
		/// <param name="counterName">Name of the counter.</param>
		/// <returns></returns>
		public static IPerfCounter GetCounter(string counterName, bool resetValueWhenCreate = true)
		{
			try
			{
				lock (m_CounterPoolLockObj)
				{
					if (m_CounterPool.ContainsKey(counterName))
						return m_CounterPool[counterName];

					var counter = new PerformanceCounter(CATEGORY_NAME, counterName, false);
					if (resetValueWhenCreate)
						counter.RawValue = 0;
					m_CounterPool[counterName] = new PerfCounter(counter);

					return m_CounterPool[counterName];
				}
			}
			catch (Exception e)
			{
				//log4net.LogManager.GetLogger("PerfCounter").Error("Unable to create perf counter: " + counterName, e);
				return new NullPerfCounter();
			}
		}
		#endregion



		#region Public Method
		/// <summary>
		/// Increments this instance.
		/// </summary>
		public void Increment()
		{
			//lock (m_Counter)
			{
				m_Counter.Increment();
			}
		}

		/// <summary>
		/// Increments the by.
		/// </summary>
		/// <param name="value">The value.</param>
		public void IncrementBy(long value)
		{
			//lock (m_Counter)
			{
				m_Counter.IncrementBy(value);
			}
		}

		/// <summary>
		/// Decrements this instance.
		/// </summary>
		public void Decrement()
		{
			//lock (m_Counter)
			{
				//Debug.Assert(m_Value > 0);

				//if (m_Value <= 0)
				//    return;

				m_Counter.Decrement();
			}
		}

		/// <summary>
		/// Nexts the value.
		/// </summary>
		/// <returns></returns>
		public float NextValue()
		{
			//lock (m_Counter)
			{
				return m_Value;
			}
		}
		#endregion
	}
}