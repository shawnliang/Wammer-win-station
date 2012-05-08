using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Wammer.Station;
using Wammer.Utility;

namespace Wammer.PostUpload
{
	public class PostUploadTaskRunner: AbstrackTaskRunner
	{
		private PostUploadTaskQueue queue;
		private BackOff backoff = new BackOff(1, 2, 3, 5, 8, 13, 21, 34, 55, 89, 144, 233);
		private ManualResetEvent quitEvent = new ManualResetEvent(false);

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
					backoff.ResetLevel();
				}
				catch (Exception e)
				{
					this.LogDebugMsg("Error while executing post upload task.", e);
					if (task != null)
					{
						queue.Undo(task);
					}
					backoff.IncreaseLevel();

					if (quitEvent.WaitOne(backoff.NextValue() * 1000))
						return;
				}
			}
		}

		public override void Start()
		{
			quitEvent.Reset();
			base.Start();
		}

		public override void Stop()
		{
			exit = true;
			quitEvent.Set();
			queue.Enqueue(new NullPostUploadTask());
			base.Stop();
		}
	}
}
