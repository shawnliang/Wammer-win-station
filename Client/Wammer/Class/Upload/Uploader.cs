using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using NLog;

namespace Waveface.Upload
{
	class Uploader
	{
		private readonly UploadQueue queue;
		private readonly Thread thread;
		private bool exit = false;
		private static Logger logger = LogManager.GetCurrentClassLogger();

		public Uploader(UploadQueue queue)
		{
			this.queue = queue;
			this.thread = new Thread(this.run);
		}

		public void Start()
		{
			this.thread.Start();
		}

		public void Join()
		{
			this.thread.Join();
		}

		private void run()
		{
			while (!exit)
			{
				var uploadItem = queue.Pop();
				if (uploadItem.IsEmpty())
					break; // empty item means stop.

				try
				{
					Main.Current.RT.REST.File_UploadFile(string.Empty,
						uploadItem.file_path, uploadItem.object_id, true, uploadItem.post_id);

					queue.ConfirmPop(uploadItem);
				}
				catch (WebException e)
				{
					handleWebException(uploadItem, e);
				}
				catch (Exception e)
				{
					logger.ErrorException("attachment upload failed. Drop this item.", e);
					queue.ConfirmPop(uploadItem);
				}
			}
		}

		private void handleWebException(UploadItem uploadItem, WebException e)
		{
			logger.WarnException("attachment upload failed", e);

			if (e.Status == WebExceptionStatus.ProtocolError)
				// The attachment is rejected. We should not send this file anymore.
				queue.ConfirmPop(uploadItem);
			else
				// station down? try again later
				queue.AddLast(uploadItem);
		}
	}
}
