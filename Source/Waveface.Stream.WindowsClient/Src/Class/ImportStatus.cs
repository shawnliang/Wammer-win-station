using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver.Builders;
using Waveface.Stream.Model;
using Waveface.Stream.WindowsClient.Properties;

namespace Waveface.Stream.WindowsClient
{
	public class ImportStatus
	{
		private IEnumerable<ImportTaskStaus> tasks;

		private ImportStatus(IEnumerable<ImportTaskStaus> tasks)
		{
			this.tasks = tasks;
		}

		public IEnumerable<ImportTaskStaus> GetPendingTasks()
		{
			return tasks.Where(t => t.IsPending());
		}

		public IEnumerable<ImportTaskStaus> GetRunningTasks()
		{
			return tasks.Where(t => t.IsInProgress());
		}

		public static ImportStatus Lookup(string user_id)
		{
			var unfinishedTasks = TaskStatusCollection.Instance.Find(
				Query.And(
					Query.EQ("UserId", user_id),
					Query.NE("Hidden", true)
				)
			).SetSortOrder(SortBy.Ascending("Time")).Where(x => x.IsPending() || x.IsInProgress());


			return new ImportStatus(unfinishedTasks);
		}

		public string Description()
		{
			if (HasTasks())
				return tasks.First().Description();
			else
				return "Nothing to import";
		}

		public bool HasTasks()
		{
			return tasks.Any();
		}
	}


	public static class ImportTaskStatusExtension
	{
		public static string Description(this ImportTaskStaus task)
		{
			if (task.IsPending())
			{
				return "Waiting";
			}
			else if (task.IsEnumerating())
			{
				return "Scanning folders";
			}
			else if (task.IsIndexing())
			{
				return string.Format("Indexing files ({0}/{1})", task.Indexed + task.Skipped, task.Total);
			}
			else if (task.IsCopying())
			{
				return string.Format("Copying files to AOStream ({0}/{1})", task.Copied, task.Indexed);
			}
			else if (task.IsThumbnailing())
			{
				return string.Format("Generating thumbnails ({0}/{1})", task.Thumbnailed, task.Indexed);
			}
			else if (task.IsUploading())
			{
				var toUploadStr = "";
				var toUpload = task.UploadSize/1024/1024;
				if (toUpload < 1)
					toUploadStr = "~1";
				else
					toUploadStr = toUpload.ToString();

				return string.Format("Syncing to AOStream Cloud ({0}/{1}MB)", task.UploadedSize/1024/1024, toUploadStr);
			}
			else if (task.IsCompleteSuccessfully())
			{
				return "Completed successfully";
			}
			else if (string.IsNullOrEmpty(task.Error))
			{
				return "Import unsuccessfully. " + task.Error;
			}
			else
			{
				throw new NotImplementedException();
			}
		}
	}
}
