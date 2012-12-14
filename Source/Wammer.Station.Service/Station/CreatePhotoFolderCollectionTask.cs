using System;
using System.Collections.Generic;
using System.Linq;
using Wammer.Cloud;
using Wammer.Station.Retry;

namespace Wammer.Station
{
	[Serializable]
	public class CreateFolderCollectionTask : DelayedRetryTask
	{
		public Dictionary<string, FolderCollection> collections { get; set; }
		public string session { get; set; }
		public string apikey { get; set; }
		public int retry { get; set; }

		public CreateFolderCollectionTask()
			:base(TaskPriority.Medium)
		{
			collections = new Dictionary<string, FolderCollection>();
		}

		public CreateFolderCollectionTask(Dictionary<string, FolderCollection> collections, string session, string apikey)
			: base(TaskPriority.Medium)
		{
			this.collections = collections;
			this.session = session;
			this.apikey = apikey;
		}

		protected override void Run()
		{
			var keys = collections.Keys.ToList();

			foreach (var key in keys)
			{
				var folderCollection = collections[key];
				CollectionApi.CreateCollection(session, apikey, folderCollection.FolderName, folderCollection.Objects);
				collections.Remove(key);
			}
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
