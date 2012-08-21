using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Waveface.Upload
{
	class UploadQueue
	{
		private readonly LinkedList<UploadItem> queue = new LinkedList<UploadItem>();
		private readonly Semaphore sem = new Semaphore(0, int.MaxValue);
		private readonly object cs = new object();
		private readonly UploadQueuePersistency storage;

		public UploadQueue(string runtimeDataPath, string user_id)
		{
			storage = new UploadQueuePersistency(runtimeDataPath, user_id);
		}

		public void AddLast(params UploadItem[] items)
		{
			lock (cs)
			{
				foreach (var item in items)
					queue.AddLast(item);
				storage.Add(items);
			}
			sem.Release();
		}

		public void AddStopItem()
		{
			lock (cs)
			{
				queue.AddFirst(new UploadItem());
			}
			sem.Release();
		}

		public UploadItem Pop()
		{
			sem.WaitOne();
			lock (cs)
			{
				var first = queue.First.Value;
				queue.RemoveFirst();
				return first;
			}
		}

		public void ConfirmPop(UploadItem item)
		{
			lock (cs)
			{
				storage.Remove(item);
			}
		}

		public void Load()
		{
			lock (cs)
			{
				foreach (var item in storage.Load())
					queue.AddLast(item);
			}
			sem.Release();
		}
	}

	public class UploadItem
	{
		public string post_id { get; set; }
		public string object_id { get; set; }
		public string file_path { get; set; }

		public bool IsEmpty()
		{
			return
				string.IsNullOrEmpty(post_id) &&
				string.IsNullOrEmpty(object_id) &&
				string.IsNullOrEmpty(file_path);
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
				return true;

			if (obj is UploadItem)
			{
				var rhs = (UploadItem)obj;
				return object_id == rhs.object_id;
			}
			else
				return false;
		}

		public override int GetHashCode()
		{
			if (object_id == null)
				return string.Empty.GetHashCode();
			else
				return object_id.GetHashCode();
		} 
	}
}
