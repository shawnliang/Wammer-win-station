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
		private int PendingTasks { get; set; }
		private List<string> ImportSources { get; set; }

		public int TotalFiles { get; set; }
		public int ProcessedFiles { get; set; }

		public string GetDescription()
		{
			if (!IsInProgress())
			{
				if (PendingTasks > 0)
					return string.Format(Resources.IMPORT_DESC_WAITING, PendingTasks);
				else
					return Resources.IMPORT_DESC_DONE;
			}
			else
			{
				var folders = (ImportSources.Count == 1) ?
					ImportSources.First() : 
					string.Format(Resources.IMPORT_DESC_FOLDERS, ImportSources.Count);

				if (PendingTasks > 0)
					return string.Format(Resources.IMPORT_DESC_IN_PROGRESS_HAS_PENDING, folders, PendingTasks);
				else
					return string.Format(Resources.IMPORT_DESC_IN_PROGRESS_NO_PENDING, folders);
			}
		}

		public bool IsInProgress()
		{
			return TotalFiles > 0;
		}

		public static ImportStatus Lookup(string user_id)
		{
			var unfinishedTasks = TaskStatusCollection.Instance.Find(
				Query.And(
					Query.EQ("UserId", user_id),
					Query.NE("IsComplete", true)
				)
			);

			var pendingTasks = unfinishedTasks.Where(x=>x.IsPending());
			var runningTasks = unfinishedTasks.Where(x=>!x.IsPending());

			var sources = new List<string>();
			foreach(var t in runningTasks)
			{
				sources.AddRange(t.Sources);
			}

			return new ImportStatus
			{
				PendingTasks = pendingTasks.Count(),
				ProcessedFiles = runningTasks.Sum(x => x.SuccessCount + x.FailedFiles.Count),
				TotalFiles = runningTasks.Sum(x => x.TotalFiles),
				ImportSources = sources
			};
		}
	}
}
