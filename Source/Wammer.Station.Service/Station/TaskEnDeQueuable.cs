using System;

namespace Wammer.Station
{
	public class DequeuedTask<T> where T : ITask
	{
		public DequeuedTask(T t, string key)
		{
			Task = t;
			Key = key;
		}

		public T Task { get; private set; }
		public string Key { get; private set; }
	}


	public interface ITaskEnqueuable<in T> where T : ITask
	{
		void Enqueue(T task, TaskPriority priority);
	}

	public interface ITaskDequeuable<T> where T : ITask
	{
		DequeuedTask<T> Dequeue();
		void AckDequeue(DequeuedTask<T> task);
		void EnqueueDummyTask();
	}

	public class TaskEventArgs : EventArgs
	{
		public ITask task { get; private set; }

		public TaskEventArgs(ITask t)
		{
			task = t;
		}
	}
}