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
		private IPerfCounter inQueue;

		private static ILog logger = LogManager.GetLogger("HttpRequestMonitor");

		private static HttpRequestMonitor _instance;

		public static HttpRequestMonitor Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new HttpRequestMonitor();
				}
				return _instance;
			}
		}

		public HttpRequestMonitor()
		{
			avgTime = PerfCounter.GetCounter(PerfCounter.AVG_TIME_PER_HTTP_REQUEST);
			avgTimeBase = PerfCounter.GetCounter(PerfCounter.AVG_TIME_PER_HTTP_REQUEST_BASE);
			throughput = PerfCounter.GetCounter(PerfCounter.HTTP_REQUEST_THROUGHPUT);
			inQueue = PerfCounter.GetCounter(PerfCounter.HTTP_REQUESTS_IN_QUEUE);
		}

		public void OnProcessSucceeded(object sender, Wammer.Station.HttpHandlerEventArgs evt)
		{
			try
			{
				inQueue.Decrement();
				avgTime.IncrementBy(evt.DurationInTicks);
				avgTimeBase.Increment();
				throughput.Increment();
			}
			catch (Exception e)
			{
				logger.Warn("Unable to write performance data.", e);
			}
		}

		public void OnTaskEnqueue(object sender,  TaskQueueEventArgs e)
		{
			this.Enqueue();
		}

		public void Enqueue()
		{
			try
			{
				inQueue.Increment();
			}
			catch (Exception e)
			{
				logger.Warn("Unable to write performance data.", e);
			}
		}
	}
}
