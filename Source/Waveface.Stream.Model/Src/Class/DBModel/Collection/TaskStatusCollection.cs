using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace Waveface.Stream.Model
{
	public class TaskStatusCollection : DBCollection<ImportTaskStaus>
	{
		private static TaskStatusCollection instance;

		static TaskStatusCollection()
		{
			instance = new TaskStatusCollection();
		}

		private TaskStatusCollection()
			: base("task_status")
		{
		}

		public static TaskStatusCollection Instance
		{
			get { return instance; }
		}

		public void HideAll()
		{
			instance.Update(Query.Null, MongoDB.Driver.Builders.Update.Set("Hidden", true), UpdateFlags.Multi);
		}

		public void AbortAllIncompleteTasks()
		{
			instance.Update(
				Query.EQ("IsCopyComplete", false),
				MongoDB.Driver.Builders.Update.Set("IsCopyComplete", true).Set("Error", "Aborted due to system restarts. Please import again."),
				UpdateFlags.Multi);
		}
	}
}
