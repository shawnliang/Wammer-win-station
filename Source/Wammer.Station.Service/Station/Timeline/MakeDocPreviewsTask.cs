using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wammer.Model;
using System.Diagnostics;
using System.IO;
using MongoDB.Driver.Builders;
using MongoDB.Bson;

namespace Wammer.Station.Timeline
{
	class MakeDocPreviewsTask : Retry.DelayedRetryTask
	{
		private string doc_id;
		private int retryCount = 50;

		public MakeDocPreviewsTask(string doc_id)
			:base(TaskPriority.Medium)
		{
			this.doc_id = doc_id;
		}


		protected override void Run()
		{
			var att = AttachmentCollection.Instance.FindOneById(doc_id);
			if (att == null)
				return;

			var user = DriverCollection.Instance.FindDriverByGroupId(att.group_id);
			if (user == null)
				return;

			var storage = new FileStorage(user);
			var full_doc_path = Path.Combine(storage.basePath, att.saved_file_name);
			if (!Path.IsPathRooted(full_doc_path))
				full_doc_path = Path.Combine(Environment.CurrentDirectory, full_doc_path);

			var result = Doc.ImportDoc.MakeDocPreview(doc_id, full_doc_path, user.user_id);

			AttachmentCollection.Instance.Update(
				Query.EQ("_id", doc_id),
				Update.Set("doc_meta.preview_files", new BsonArray(result.files))
			);
		}

		public override void ScheduleToRun()
		{
		}
	}
}
