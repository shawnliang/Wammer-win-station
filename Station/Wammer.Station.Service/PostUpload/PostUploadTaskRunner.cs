using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Wammer.Station;

namespace Wammer.PostUpload
{
	public class PostUploadTaskRunner: AbstrackTaskRunner
	{
		private PostUploadTaskQueue queue;
		private const int maxTimeout = 16 * 1000;
		private int timeout = 0;

		public PostUploadTaskRunner(PostUploadTaskQueue queue)
		{
			this.queue = queue;
		}

		protected override void Do()
		{
			while (!exit)
			{
				PostUploadTask task = null;
				try
				{
					task = queue.Dequeue();
					task.Execute();
					queue.Done(task);
					ResetTimeout();
				}
				catch (Exception e)
				{
					this.LogDebugMsg("Error while executing post upload task.", e);
					if (task != null)
					{
						queue.Undo(task);
					}
					ExtendTimeout();
					Thread.Sleep(timeout);
				}
			}
		}

		private void ResetTimeout()
		{
			timeout = 0;
		}

		private void ExtendTimeout()
		{
			if (timeout <= maxTimeout)
			{
				timeout = (timeout == 0 ? 1000 : timeout * 2);
			}
		}
	}
}
