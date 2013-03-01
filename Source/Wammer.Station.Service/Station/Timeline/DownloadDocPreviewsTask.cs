using MongoDB.Bson;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wammer.Cloud;
using Waveface.Stream.Model;

namespace Wammer.Station.Timeline
{
	class DownloadDocPreviewsTask : Retry.DelayedRetryTask, IResourceDownloadTask
	{
		public string doc_id { get; set; }
		public int nPreviewsDownloaded { get; set; }
		public int retry { get; set; }
		public List<string> downloadedPreviews { get; set; }
		public string user_id { get; set; }
		public DownloadDocPreviewsTask(string doc_id, string user_id)
			: base(TaskPriority.Low)
		{
			this.retry = 100;
			this.doc_id = doc_id;
			this.user_id = user_id;
			this.downloadedPreviews = new List<string>();
		}

		protected override void Run()
		{
			var att = AttachmentCollection.Instance.FindOneById(doc_id);
			if (att == null)
				return;

			if (att.HasDocPreviews())
				return;

			var user = DriverCollection.Instance.FindDriverByGroupId(att.group_id);
			if (user == null)
				return;

			var docInCloud = AttachmentApi.GetInfo(doc_id, user.session_token);
			while (nPreviewsDownloaded < docInCloud.doc_meta.preview_pages)
			{
				var downloadIndex = nPreviewsDownloaded + 1;

				//TODO: StringBuffer?!
				var url = CloudServer.BaseUrl + "attachments/view?" +
					"object_id=" + HttpUtility.UrlEncode(doc_id) + "&" +
					"target=preview&" +
					"page=" + downloadIndex.ToString() + "&" +
					"session_token=" + HttpUtility.UrlEncode(user.session_token) + "&" +
					"apikey=" + CloudServer.APIKey;

				var downloadResult = AttachmentApi.DownloadObject(url);

				var previewFile = string.Format(@"{0}\{1}.jpg", doc_id, downloadIndex.ToString("d8"));
				var savedFile = FileStorage.SaveToCacheFolder(user.user_id, previewFile, new ArraySegment<byte>(downloadResult.Image));

				downloadedPreviews.Add(savedFile);

				nPreviewsDownloaded++;
			}

			if (this.downloadedPreviews.Any())
			{
				AttachmentCollection.Instance.Update(
					Query.EQ("_id", doc_id),
					Update
						.Set("type", (int)AttachmentType.doc)
						.Set("group_id", user.groups[0].group_id)
						.Set("creator_id", docInCloud.creator_id)
						.Set("device_id", docInCloud.creator_id)
						.Set("file_name", docInCloud.file_name)
						.Set("file_path", docInCloud.file_path)
						.Set("doc_meta.preview_pages", this.downloadedPreviews.Count)
						.Set("doc_meta.preview_files", new BsonArray(this.downloadedPreviews)),
					MongoDB.Driver.UpdateFlags.Upsert);
			}
		}

		public override void ScheduleToRun()
		{
			if (--retry > 0)
				BodySyncQueue.Instance.Enqueue(this, this.priority);
		}

		public string Name
		{
			get { return doc_id + "previews"; }
		}

		public string UserId
		{
			get { return user_id; }
		}
	}
}
