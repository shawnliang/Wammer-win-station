using System.Diagnostics;

namespace Wammer.PerfMonitor
{
	public class PerfCounterInstaller
	{
		public static void Install()
		{
			if (PerformanceCounterCategory.Exists(PerfCounter.CATEGORY_NAME))
				PerformanceCounterCategory.Delete(PerfCounter.CATEGORY_NAME);

			CounterCreationDataCollection counters = new CounterCreationDataCollection();

			// 1. Average time per attachment upload
			counters.Add(new CounterCreationData(
				PerfCounter.AVG_TIME_PER_ATTACHMENT_UPLOAD,
				PerfCounter.AVG_TIME_PER_ATTACHMENT_UPLOAD,
				PerformanceCounterType.AverageTimer32));

			// 2. Average time per attachment upload base
			counters.Add(new CounterCreationData(
				PerfCounter.AVG_TIME_PER_ATTACHMENT_UPLOAD_BASE,
				PerfCounter.AVG_TIME_PER_ATTACHMENT_UPLOAD_BASE,
				PerformanceCounterType.AverageBase));


			// 3. Thumbnail upstream bytes/sec
			counters.Add(new CounterCreationData(
				PerfCounter.THUMBNAIL_UPSTREAM_RATE,
				PerfCounter.THUMBNAIL_UPSTREAM_RATE,
				PerformanceCounterType.RateOfCountsPerSecond64));

			// 4. Items in queue
			counters.Add(new CounterCreationData(
				PerfCounter.ITEMS_IN_QUEUE,
				PerfCounter.ITEMS_IN_QUEUE,
				PerformanceCounterType.NumberOfItems32));

			// 5. Items in progress
			counters.Add(new CounterCreationData(
				PerfCounter.ITEMS_IN_PROGRESS,
				PerfCounter.ITEMS_IN_PROGRESS,
				PerformanceCounterType.NumberOfItems32));

			// Add new category
			PerformanceCounterCategory.Create(
				PerfCounter.CATEGORY_NAME,
				"Performance counters of Waveface Station",
				PerformanceCounterCategoryType.SingleInstance,
				counters);
		}
	}
}
