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

		public static ImportStatusSummary Lookup(string user_id)
		{
			var unfinishedTasks = TaskStatusCollection.Instance.Find(
				Query.And(
					Query.EQ("UserId", user_id),
					Query.NE("Hidden", true)
				)
			).SetSortOrder(SortBy.Ascending("Time")).Where(x => x.IsPending() || x.IsInProgress());


			return new ImportStatusSummary(new ImportStatus(unfinishedTasks));
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

	public class ImportStatusSummary
	{
		private string desc = "";
		private long max;
		private long cur;
		private bool hasProgress;

		public ImportStatusSummary(ImportStatus status)
		{
			var runnings = status.GetRunningTasks();
			var pendings = status.GetPendingTasks();


			if (!status.HasTasks())
				return;

			if (runnings.Count() > 0)
			{
				var foreground = runnings.FirstOrDefault(t => t.IsInProgress() && !t.IsCopyComplete);

				if (foreground != null)
				{
					desc = foreground.Description();

					if (pendings.Count() > 0)
						desc += "\r\n" + string.Format("{0} tasks are queued.", pendings.Count());

					hasProgress = foreground.GetProgress(out max, out cur);
					return;
				}
				else
				{
					var aggregated = aggregateTasks(runnings);
					desc = aggregated.Description();
					hasProgress = aggregated.GetProgress(out max, out cur);

					if (pendings.Count() > 0)
						desc += "\r\n" + string.Format("{0} tasks are queued.");
				}
			}
			else if (pendings.Count() > 0)
			{
				desc = string.Format("Waiting to import... {0} tasks queued", pendings.Count());
			}
		}

		private ImportTaskStaus aggregateTasks(IEnumerable<ImportTaskStaus> runnings)
		{
			var result = runnings.First();

			foreach (var item in runnings.Skip(1))
			{
				result = result + item;
			}

			return result;
		}

		public string Description
		{
			get { return desc; }
		}

		public bool GetProgress(out long maximum, out long current)
		{
			maximum = max;
			current = cur;

			return hasProgress;
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
			else if (!string.IsNullOrEmpty(task.Error))
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
