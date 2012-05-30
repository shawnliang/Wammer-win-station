using System;
using System.Threading;
using Wammer.Station;
using Wammer.Utility;
using Wammer.Cloud;
using System.Net;

namespace Wammer.PostUpload
{
	public class PostUploadTaskRunner : AbstrackTaskRunner
	{
		private readonly BackOff backoff = new BackOff(1, 2, 3, 5, 8, 13, 21, 34, 55, 89, 144, 233);
		private readonly PostUploadTaskQueue queue;
		private readonly ManualResetEvent quitEvent = new ManualResetEvent(false);

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

					if (e is WammerCloudException && 
						e.InnerException is WebException &&
						(e.InnerException as WebException).Status != WebExceptionStatus.ProtocolError)
					{
						backoff.IncreaseLevel();

						if (quitEvent.WaitOne(backoff.NextValue() * 1000))
							return;
					}
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