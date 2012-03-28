using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wammer.Station;

namespace Wammer
{
	/// <summary>
	/// 
	/// </summary>
	public class TaskQueueEventArgs:EventArgs
	{
		#region Var
		private ITask _task;
		private IHttpHandler _handler;
		#endregion

		#region Property
		/// <summary>
		/// Gets or sets the task.
		/// </summary>
		/// <value>The task.</value>
		public ITask Task
		{
			get { return _task; }
			private set { _task = value; }
		}

		/// <summary>
		/// Gets or sets the handler.
		/// </summary>
		/// <value>The handler.</value>
		public IHttpHandler Handler
		{
			get { return _handler; }
			private set { _handler = value; }
		}
		#endregion
		
		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="TaskQueueEventArgs"/> class.
		/// </summary>
		/// <param name="task">The task.</param>
		/// <param name="handler">The handler.</param>
		public TaskQueueEventArgs(ITask task, IHttpHandler handler)
		{
			this.Task = task;
			this.Handler = handler;
		} 
		#endregion
	}
}
