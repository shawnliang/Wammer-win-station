namespace Wammer.Station
{
	//class CloudStorageSync
	//{
	//    private log4net.ILog logger = log4net.LogManager.GetLogger(typeof(CloudStorageSync));
	//    private object cs = new object();

	//    public void HandleAttachmentSaved(object sender, AttachmentEventArgs evt)
	//    {
	//        try
	//        {
	//            TaskQueue.Enqueue(new SyncBodyToDropboxTask(evt), TaskPriority.Low, true);
	//        }
	//        catch (Exception e)
	//        {
	//            this.LogWarnMsg("Unable to enqueue SyncBodyToDropboxTask object", e);
	//        }
	//    }
	//}

	//class SyncBodyToDropboxTask : ITask
	//{
	//    private log4net.ILog logger = log4net.LogManager.GetLogger(typeof(CloudStorageSync));
	//    private static object cs = new object();
	//    private AttachmentEventArgs args;

	//    public SyncBodyToDropboxTask(AttachmentEventArgs args)
	//    {
	//        this.args = args;
	//    }

	//    public void Execute()
	//    {
	//        // just for avoiding race condition, might cause bad performance
	//        lock (cs)
	//        {
	//            CloudStorage cloudstorage = CloudStorageCollection.Instance.FindOne(Query.EQ("Type", "dropbox"));
	//            if (cloudstorage == null)
	//                return;

	//            Attachment body = Model.AttachmentCollection.Instance.FindOne(Query.EQ("_id", args.AttachmentId));
	//            if (body == null || body.saved_file_name == null)
	//                return;

	//            Driver user = DriverCollection.Instance.FindOne(Query.EQ("_id", args.UserId));
	//            if (user == null)
	//                return;


	//            logger.DebugFormat("Trying to backup file {0} to Dropbox", body.saved_file_name);
	//            new DropboxFileStorage(user, cloudstorage).SaveAttachment(body);
	//        }
	//    }
	//}
}