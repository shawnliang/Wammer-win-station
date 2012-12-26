using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
	}
}
