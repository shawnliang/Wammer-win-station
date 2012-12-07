
namespace Wammer.Station.AttachmentUpload
{
	class AttachmentUploadQueueHelper
	{
		private static AttachmentUploadQueue queue;

		static AttachmentUploadQueueHelper()
		{
			queue = new AttachmentUploadQueue();
		}

		public static AttachmentUploadQueue Instance
		{
			get { return queue; }
		}
	}
}
