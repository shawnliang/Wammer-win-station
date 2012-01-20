using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wammer.Station
{
	public class StationTimer
	{
#if DEBUG
		private const long STATUS_CHECK_PERIOD = 30 * 1000; // run heartbeat frequently in debug mode
#else
		private const long STATUS_CHECK_PERIOD = 10 * 60 * 1000; // TODO: remove hardcode
#endif
		private StatusChecker statusChecker;

		public StationTimer(HttpServer functionServer)
		{
			statusChecker = new StatusChecker(STATUS_CHECK_PERIOD, functionServer);
		}

		public void Start()
		{
			statusChecker.Start();
		}

		public void Stop()
		{
			statusChecker.Stop();
		}

		public void Close()
		{
			statusChecker.Close();
		}
	}
}
