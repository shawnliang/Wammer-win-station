using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

using Wammer.Model;
using Wammer.Cloud;
using MongoDB.Driver.Builders;

namespace Wammer.Station
{
	class CloudStorageSync
	{
		private log4net.ILog logger = log4net.LogManager.GetLogger(typeof(CloudStorageSync));
		private object cs = new object();

		public void HandleAttachmentSaved(object sender, AttachmentEventArgs evt)
		{
			if (evt.Attachment.saved_file_name == null) // this attachment is a thumbnail
				return;

			// just for avoiding race condition, might cause bad performance
			lock (cs)
			{
				CloudStorage cloudstorage = CloudStorage.collection.FindOne(Query.EQ("Type", "dropbox"));
				if (cloudstorage != null)
				{
					logger.DebugFormat("Trying to backup file {0} to Dropbox", evt.Attachment.saved_file_name);
					new DropboxFileStorage(evt.Driver, cloudstorage).SaveAttachment(evt.Attachment);
				}
			}
		}
	}
}
