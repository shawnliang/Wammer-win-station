using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wammer.Station
{
	class StationTimer
	{
		private const long STATUS_CHECK_PERIOD = 10 * 60 * 1000; // TODO: remove hardcode
		private StatusChecker statusChecker;

		public StationTimer()
		{
			this.statusChecker = new StatusChecker(STATUS_CHECK_PERIOD);
		}
	}
}
