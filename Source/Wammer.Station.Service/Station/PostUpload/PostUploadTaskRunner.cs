using System;
using System.Net;
using System.Threading;
using Wammer.Cloud;
using Wammer.Station;
using Wammer.Utility;

namespace Wammer.PostUpload
{
	public class PostUploadTaskRunner : AbstrackTaskRunner
	{
		private readonly BackOff backoff = new BackOff(1, 2, 3, 5, 8, 13, 21, 34, 55, 89, 144, 233);
		private readonly PostUploadTaskQueue queue;
		private readonly ManualResetEvent quitEvent = new ManualResetEvent(false);

		public event EventHandler<PostUpsertEventArgs> PostUpserted;


		public PostUploadTaskRunner(PostUploadTaskQueue queue)
		{
			this.queue = queue;
		}

		protected override void Do()
		{
			while (!exit.GoExit)
			{
				PostUploadTask task = null;
				try
				{
					task = queue.Dequeue();
					task.Execute();
					queue.Done(task);

					OnPostUpserted(task);
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

		public override void StopAsync()
		{
			exit.GoExit = true;
			quitEvent.Set();

			queue.Enqueue(new NullPostUploadTask());
		}

		public override void Start()
		{
			quitEvent.Reset();
			base.Start();
		}

		protected void OnPostUpserted(PostUploadTask task)
		{
			var handler = PostUpserted;
			if (handler != null)
			{
				var session = string.Empty;

				if (task.Parameters.ContainsKey(CloudServer.PARAM_SESSION_TOKEN))
					session = task.Parameters[CloudServer.PARAM_SESSION_TOKEN];

				handler(this, new PostUpsertEventArgs(task.PostId, session, task.UserId));
			}
		}
	}
}