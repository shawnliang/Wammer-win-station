﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wammer.PerfMonitor
{
	class PostUploadMonitor
	{
		private IPerfCounter UploadNumCounter;

		public PostUploadMonitor()
		{
			UploadNumCounter = PerfCounter.GetCounter(PerfCounter.POST_IN_QUEUE);
		}

		public void PostUploadTaskEnqueued()
		{
			UploadNumCounter.Increment();
		}

		public void PostUploadTaskDone()
		{
			UploadNumCounter.Decrement();
		}
	}
}