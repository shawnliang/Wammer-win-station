using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wammer.Model
{
	class RetryQueueCollection: Collection<GenericData>
	{
		private static RetryQueueCollection instance;

		static RetryQueueCollection()
		{
			instance = new RetryQueueCollection();
		}

		private RetryQueueCollection()
			: base("retry_queue")
		{
		}

		public static RetryQueueCollection Instance
		{
			get { return instance; }
		}
	}
}
