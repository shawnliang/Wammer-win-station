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
				return "Indexing files";
			}
			else if (task.IsCopying())
			{
				return "Copying files to AOStream";
			}
			else if (task.IsThumbnailing())
			{
				return "Generating thumbnails";
			}
			else if (task.IsUploading())
			{
				return "Syncing to AOStream Cloud";
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
