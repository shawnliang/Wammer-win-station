using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using log4net;

namespace Wammer.PerfMonitor
{
	public interface IPerfCounter
	{
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
		public const string THUMBNAIL_UPSTREAM_RATE = "Thumbnail upstream rate (bytes/sec)";

		private static bool CategoryExists;
		private static ILog Logger = LogManager.GetLogger("PerfCounter");
		#endregion

		#region instance variables
		PerformanceCounter Counter;
		#endregion

		static PerfCounter()
		{
			CategoryExists = PerformanceCounterCategory.Exists(CATEGORY_NAME);
		}

		private PerfCounter(PerformanceCounter counter)
		{
			this.Counter = counter;
		}

		public static IPerfCounter GetCounter(string counterName)
		{
			try
			{
				PerformanceCounter counter = new PerformanceCounter(CATEGORY_NAME, counterName, false);
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

	class NullPerfCounter:IPerfCounter
	{
		public void Increment() {}
		public void IncrementBy(long value) {}
		public void Decrement() {}
	}
}
