using System;
using System.Diagnostics;
using log4net;

namespace Wammer.PerfMonitor
{
	public interface IPerfCounter
	{
		long Value { get; set; }

		void Increment();
		void IncrementBy(long value);
		void Decrement();
	}

	public class PerfCounter : IPerfCounter
	{
		#region class variables
		public const string CATEGORY_NAME = "Waveface Station";
		public const string AVG_TIME_PER_ATTACHMENT_UPLOAD = "Average time per attachment upload";
		public const string AVG_TIME_PER_ATTACHMENT_UPLOAD_BASE = "Average time per attachment upload base";
		public const string UPSTREAM_RATE = "Upstream rate (bytes/sec)";
		public const string DWSTREAM_RATE = "Downstream rate (bytes/sec)";
		public const string UP_REMAINED_COUNT = "Atachement upload remained count";
		public const string DW_REMAINED_COUNT = "Atachement download remained count";
		public const string ITEMS_IN_QUEUE = "Items in queue";
		public const string ITEMS_IN_PROGRESS = "Items in progress";
		private static bool CategoryExists;
		private static ILog Logger = LogManager.GetLogger("PerfCounter");
		#endregion

		#region instance variables
		PerformanceCounter Counter;		
		#endregion

		public long Value
		{
			get { return Counter.RawValue; }
			set { Counter.RawValue = value; }
		}

		static PerfCounter()
		{
			CategoryExists = PerformanceCounterCategory.Exists(CATEGORY_NAME);
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
				Logger.Warn("Unable to create counter: " + counterName, e);
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
	}

	class NullPerfCounter : IPerfCounter
	{
		public void Increment() { }
		public void IncrementBy(long value) { }
		public void Decrement() { }

		public long Value
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}
	}
}
