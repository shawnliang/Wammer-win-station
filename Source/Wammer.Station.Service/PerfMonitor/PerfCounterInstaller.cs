using System.ComponentModel;
using System.Diagnostics;
using Waveface.Stream.Model;

namespace Wammer.PerfMonitor
{
	public class PerfCounterInstaller
	{
		public static void Install()
		{
			try
			{
				if (PerformanceCounterCategory.Exists(PerfCounter.CATEGORY_NAME))
					PerformanceCounterCategory.Delete(PerfCounter.CATEGORY_NAME);
			}
			catch (Win32Exception)
			{
				// fix corrupted performance counter
				using (var p = new Process())
				{
					p.StartInfo = new ProcessStartInfo("lodctr.exe", "/R")
					{
						WindowStyle = ProcessWindowStyle.Hidden,
						CreateNoWindow = true
					};
					p.Start();
					p.WaitForExit();
				}

				// try delete performance counter again
				if (PerformanceCounterCategory.Exists(PerfCounter.CATEGORY_NAME))
					PerformanceCounterCategory.Delete(PerfCounter.CATEGORY_NAME);
			}

			var counters = new CounterCreationDataCollection
			               	{
			               		// 1. Average time per attachment upload
			               		new CounterCreationData(
			               			PerfCounter.AVG_TIME_PER_ATTACHMENT_UPLOAD,
			               			PerfCounter.AVG_TIME_PER_ATTACHMENT_UPLOAD,
			               			PerformanceCounterType.AverageTimer32),
			               		// 2. Average time per attachment upload base
			               		new CounterCreationData(
			               			PerfCounter.AVG_TIME_PER_ATTACHMENT_UPLOAD_BASE,
			               			PerfCounter.AVG_TIME_PER_ATTACHMENT_UPLOAD_BASE,
			               			PerformanceCounterType.AverageBase),
			               		// 3. upstream bytes/sec
			               		new CounterCreationData(
			               			PerfCounter.UPSTREAM_RATE,
			               			PerfCounter.UPSTREAM_RATE,
			               			PerformanceCounterType.RateOfCountsPerSecond64),
			               		// 4. dwstream bytes/sec
			               		new CounterCreationData(
			               			PerfCounter.DWSTREAM_RATE,
			               			PerfCounter.DWSTREAM_RATE,
			               			PerformanceCounterType.RateOfCountsPerSecond64),
			               		// 5. upload remained count
			               		new CounterCreationData(
			               			PerfCounter.UP_REMAINED_COUNT,
			               			PerfCounter.UP_REMAINED_COUNT,
			               			PerformanceCounterType.NumberOfItems32),
			               		// 6. download remained count
			               		new CounterCreationData(
			               			PerfCounter.DW_REMAINED_COUNT,
			               			PerfCounter.DW_REMAINED_COUNT,
			               			PerformanceCounterType.NumberOfItems32),
			               		// 7. Items in queue
			               		new CounterCreationData(
			               			PerfCounter.ITEMS_IN_QUEUE,
			               			PerfCounter.ITEMS_IN_QUEUE,
			               			PerformanceCounterType.NumberOfItems32),
			               		// 8. Items in progress
			               		new CounterCreationData(
			               			PerfCounter.ITEMS_IN_PROGRESS,
			               			PerfCounter.ITEMS_IN_PROGRESS,
			               			PerformanceCounterType.NumberOfItems32),
			               		// 9. Average time per http request
			               		new CounterCreationData(
			               			PerfCounter.AVG_TIME_PER_HTTP_REQUEST,
			               			PerfCounter.AVG_TIME_PER_HTTP_REQUEST,
			               			PerformanceCounterType.AverageTimer32),
			               		// 10. Average time per http request base
			               		new CounterCreationData(
			               			PerfCounter.AVG_TIME_PER_HTTP_REQUEST_BASE,
			               			PerfCounter.AVG_TIME_PER_HTTP_REQUEST_BASE,
			               			PerformanceCounterType.AverageBase),
			               		// 11. Http request throughput (reqs/sec)
			               		new CounterCreationData(
			               			PerfCounter.HTTP_REQUEST_THROUGHPUT,
			               			PerfCounter.HTTP_REQUEST_THROUGHPUT,
			               			PerformanceCounterType.RateOfCountsPerSecond64),
			               		// 12. Http reqeuests in queue
			               		new CounterCreationData(
			               			PerfCounter.HTTP_REQUESTS_IN_QUEUE,
			               			PerfCounter.HTTP_REQUESTS_IN_QUEUE,
			               			PerformanceCounterType.NumberOfItems32),
			               		// 13. Post upload tasks in queue
			               		new CounterCreationData(
			               			PerfCounter.POST_IN_QUEUE,
			               			PerfCounter.POST_IN_QUEUE,
			               			PerformanceCounterType.NumberOfItems32),

								// 14. Attachment upload count
			               		new CounterCreationData(
			               			PerfCounter.ATTACHMENT_UPLOAD_COUNT,
			               			PerfCounter.ATTACHMENT_UPLOAD_COUNT,
			               			PerformanceCounterType.NumberOfItems32),

								// 15. Small thumbnail generate count
			               		new CounterCreationData(
			               			PerfCounter.SMALL_THUMBNAIL_GENERATE_COUNT,
			               			PerfCounter.SMALL_THUMBNAIL_GENERATE_COUNT,
			               			PerformanceCounterType.NumberOfItems32),

								// 16. Medium thumbnail generate count
			               		new CounterCreationData(
			               			PerfCounter.MEDIUM_THUMBNAIL_GENERATE_COUNT,
									PerfCounter.MEDIUM_THUMBNAIL_GENERATE_COUNT,
			               			PerformanceCounterType.NumberOfItems32)
			               	};

			// Add new category
			PerformanceCounterCategory.Create(
				PerfCounter.CATEGORY_NAME,
				"Performance counters of Waveface Station",
				PerformanceCounterCategoryType.SingleInstance,
				counters);
		}
	}
}