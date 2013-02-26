using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
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
				return Resources.NOTHING_IMPORT;
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
						desc += "\r\n" + string.Format(Resources.TASK_QUEUED_PATTERN, pendings.Count());

					hasProgress = foreground.GetProgress(out max, out cur);
					return;
				}
				else
				{
					var aggregated = aggregateTasks(runnings);
					desc = aggregated.Description();
					hasProgress = aggregated.GetProgress(out max, out cur);

					if (pendings.Count() > 0)
						desc += "\r\n" + string.Format(Resources.TASK_QUEUED_PATTERN, pendings.Count());
				}
			}
			else if (pendings.Count() > 0)
			{
				desc = string.Format(Resources.WAITING_IMPORT, pendings.Count());
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
				return string.Format(Resources.INDEXING_FILE_PATTERN, task.Indexed + task.Skipped, task.Total);
			}
			else if (task.IsCopying())
			{
				return string.Format(Resources.COPYING_FILE_PATTERN, task.Copied, task.Indexed);
			}
			else if (task.IsThumbnailing())
			{
				return string.Format(Resources.GENERATING_PREVIEW_PATTERN, task.Thumbnailed, task.Indexed);
			}
			else if (task.IsUploading())
			{
				//var toUploadStr = "";
				//var toUpload = task.UploadSize/1024/1024;
				//if (toUpload < 1)
				//	toUploadStr = "~1";
				//else
				//	toUploadStr = toUpload.ToString();

				//return string.Format("Syncing to AOStream Cloud ({0}/{1}MB)", task.UploadedSize/1024/1024, toUploadStr);
				return string.Empty;
			}
			else if (task.IsCompleteSuccessfully())
			{
				//return "Completed successfully";
				return string.Empty;
			}
			else if (!string.IsNullOrEmpty(task.Error))
			{
				//return "Import unsuccessfully. " + task.Error;
				return string.Empty;
			}
			else
			{
				throw new NotImplementedException();
			}
		}
	}
}
