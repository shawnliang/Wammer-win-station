using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Wammer.Station;

namespace Wammer.PostUpload
{
	public class PostUploadTaskRunner: AbstrackTaskRunner
	{
		private PostUploadTaskQueue queue;

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
				}
				catch (Exception e)
				{
					this.LogDebugMsg("Error while executing post upload task.", e);
					if (task != null)
					{
						queue.Undo(task);
					}
				}
			}
		} 
	}
}
