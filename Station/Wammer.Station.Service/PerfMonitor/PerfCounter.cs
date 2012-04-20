using System;
using System.Diagnostics;
using log4net;

namespace Wammer.PerfMonitor
{
	public interface IPerfCounter
	{
		void Increment();
		void IncrementBy(long value);
		void Decrement();
		CounterSample NextSample();
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
		//private static bool CategoryExists = PerformanceCounterCategory.Exists(CATEGORY_NAME);
		private static ILog _logger;
		#endregion

		#region Var
		PerformanceCounter Counter;		
		#endregion

		#region Private Static Property
		private static ILog m_Logger
		{
			get
			{
 				if(_logger == null)
					_logger = LogManager.GetLogger("PerfCounter");
				return _logger;
			}
		}
		#endregion

		static PerfCounter()
		{
		}

		private PerfCounter(PerformanceCounter counter)
		{
			this.Counter = counter;
		}

		public static IPerfCounter GetCounter(string counterName, Boolean needInitValue = true)
		{
			try
			{
				PerformanceCounter counter = new PerformanceCounter(CATEGORY_NAME, counterName, false);

				if (needInitValue)
					counter.RawValue = 0;

				return new PerfCounter(counter);
			}
			catch (Exception e)
			{
				m_Logger.Warn("Unable to create counter: " + counterName, e);
				return new NullPerfCounter();
			}
		}

		public void Increment()
		{
			Counter.Increment();
		}

		public void IncrementBy(long value)
		{
			Counter.IncrementBy(value);
		}

		public void Decrement()
		{
			Counter.Decrement();
		}

		public CounterSample NextSample()
		{
			return Counter.NextSample();
		}

		public float NextValue()
		{
			return Counter.NextValue();
		}
	}

	class NullPerfCounter : IPerfCounter
	{
		public void Increment() { }
		public void IncrementBy(long value) { }
		public void Decrement() { }
		public CounterSample NextSample() { return new CounterSample(); }
		public float NextValue() { return 0; }
	}
}
