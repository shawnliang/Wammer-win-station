using System;
using Wammer.Station;

namespace Wammer
{
	/// <summary>
	/// 
	/// </summary>
	public class TaskQueueEventArgs : EventArgs
	{
		#region Property

		/// <summary>
		/// Gets or sets the task.
		/// </summary>
		/// <value>The task.</value>
		public ITask Task { get; private set; }

		/// <summary>
		/// Gets or sets the handler.
		/// </summary>
		/// <value>The handler.</value>
		public IHttpHandler Handler { get; private set; }

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="TaskQueueEventArgs"/> class.
		/// </summary>
		/// <param name="task">The task.</param>
		/// <param name="handler">The handler.</param>
		public TaskQueueEventArgs(ITask task, IHttpHandler handler)
		{
			Task = task;
			Handler = handler;
		}

		#endregion
	}
}