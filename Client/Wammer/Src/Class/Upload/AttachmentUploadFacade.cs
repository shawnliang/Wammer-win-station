using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Waveface.Upload
{
	public class AttachmentUploadFacade
	{
		private UploadQueue queue;
		private Uploader uploader;

		public AttachmentUploadFacade(string runtimeDataPath, string user_id)
		{
			queue = new UploadQueue(runtimeDataPath, user_id);
			queue.Load();

			uploader = new Uploader(queue);
			uploader.Start();
		}

		public void Stop()
		{
			queue.AddStopItem();
			uploader.Join();
		}

		public void Add(string file_path, string object_id, string post_id)
		{
			Add(new UploadItem
			{
				file_path = file_path,
				object_id = object_id,
				post_id = post_id
			});
		}

		public void Add(params UploadItem[] uploadItem)
		{
			queue.AddLast(uploadItem);
		}
	}
}
