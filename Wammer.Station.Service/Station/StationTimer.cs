using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wammer.Station
{
	class StationTimer
	{
#if DEBUG
		private const long STATUS_CHECK_PERIOD = 5 * 1000; // run heartbeat frequently in debug mode
#else
		private const long STATUS_CHECK_PERIOD = 10 * 60 * 1000; // TODO: remove hardcode
#endif
		private StatusChecker statusChecker;

		public StationTimer()
		{
			statusChecker = new StatusChecker(STATUS_CHECK_PERIOD);
		}

		public void Stop()
		{
			statusChecker.Stop();
		}
	}
}
