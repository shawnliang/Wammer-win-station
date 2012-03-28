using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

namespace Wammer.PerfMonitor
{
	public class HttpRequestMonitor
	{
		private IPerfCounter avgTime;
		private IPerfCounter avgTimeBase;
		private IPerfCounter throughput;

		private static ILog logger = LogManager.GetLogger("HttpRequestMonitor");

		public HttpRequestMonitor()
		{
			avgTime = PerfCounter.GetCounter(PerfCounter.AVG_TIME_PER_HTTP_REQUEST);
			avgTimeBase = PerfCounter.GetCounter(PerfCounter.AVG_TIME_PER_HTTP_REQUEST_BASE);
			throughput = PerfCounter.GetCounter(PerfCounter.HTTP_REQUEST_THROUGHPUT);
		}

		public void OnProcessSucceeded(object sender, Wammer.Station.HttpHandlerEventArgs evt)
		{
			try
			{
				avgTime.IncrementBy(evt.DurationInTicks);
				avgTimeBase.Increment();
				throughput.Increment();
			}
			catch (Exception e)
			{
				logger.Warn("Unable to write performance data.", e);
			}
		}
	}
}
