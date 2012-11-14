using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wammer.Station.Retry
{
	public class RetryQueueHelper
	{
		private static RetryQueue queue;

		static RetryQueueHelper()
		{
			queue = new RetryQueue(new RetryQueuePersistentStorage());
		}

		public static RetryQueue Instance
		{
			get { return queue; }
		}
	}
}
