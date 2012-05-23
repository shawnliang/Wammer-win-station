using System;

namespace Wammer.Station
{
	public class DequeuedTask<T> where T : ITask
	{
		public DequeuedTask(T t, object key)
		{
			Task = t;
			Key = key;
		}

		public T Task { get; private set; }
		public object Key { get; private set; }
	}


	public interface ITaskEnqueuable<in T> where T : ITask
	{
		void Enqueue(T task, TaskPriority priority);
	}

	public interface ITaskDequeuable<T> where T : ITask
	{
		Boolean IsPersistenceQueue { get; }

		DequeuedTask<T> Dequeue();
		void AckDequeue(DequeuedTask<T> task);
		void EnqueueDummyTask();
	}

	
}