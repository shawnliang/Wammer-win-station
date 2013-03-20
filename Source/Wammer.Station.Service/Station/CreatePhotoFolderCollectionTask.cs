using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using Wammer.Cloud;
using Wammer.Station.Retry;
using Waveface.Stream.Model;

namespace Wammer.Station
{
	[Serializable]
	public class CreateFolderCollectionTask : DelayedRetryTask
	{
		public Dictionary<string, FolderCollection> collections { get; set; }
		public string group_id { get; set; }
		public int retry { get; set; }

		public CreateFolderCollectionTask()
			: base(TaskPriority.Medium)
		{
			collections = new Dictionary<string, FolderCollection>();
		}

		public CreateFolderCollectionTask(Dictionary<string, FolderCollection> collections, string group_id)
			: base(TaskPriority.Medium)
		{
			this.collections = collections;
			this.group_id = group_id;
		}

		protected override void Run()
		{
			var keys = collections.Keys.ToList();
			var user = DriverCollection.Instance.FindDriverByGroupId(group_id);

			if (user == null)
				return;

			foreach (var key in keys)
			{
				var folderCollection = collections[key];

				var collectionId = Guid.NewGuid().ToString();

				if (hasBeenImported(folderCollection, user.user_id))
				{
					folderCollection.FolderName += "_" + DateTime.Now.ToString("yyyyMMddHHmm");
				}

				var attachmentIDs = folderCollection.Objects;
				var coverAttachmentID = attachmentIDs.FirstOrDefault();

				CollectionApi.CreateCollection(
					user.session_token, CloudServer.APIKey, folderCollection.FolderName, attachmentIDs, collectionId,
					coverAttachmentID, null, null, folderCollection.FolderPath);

				collections.Remove(key);


				CollectionCollection.Instance.Save(
					new Waveface.Stream.Model.Collection
					{
						attachment_id_array = attachmentIDs,
						collection_id = collectionId,
						import_folder = folderCollection.FolderPath,
						name = folderCollection.FolderName,
						creator_id = user.user_id,
						cover = coverAttachmentID
					});
			}
		}

		private bool hasBeenImported(FolderCollection folder, string user_id)
		{
			return CollectionCollection.Instance.FindOne(
				Query.And(
					Query.EQ("creator_id", user_id),
					Query.EQ("import_folder", folder.FolderPath)
				)) != null;
		}

		public override void ScheduleToRun()
		{
			if (retry++ > 100)
			{
				foreach (var key in collections.Keys)
				{
					this.LogWarnMsg("Unabel to create collections: " + key);
				}
			}

			TaskQueue.Enqueue(this, this.priority);
		}
	}
}
