using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using log4net;

namespace Wammer.Station
{
	class TaskRunner
	{
		#region Var
		private Thread _thread; 
		#endregion

		#region Private Property
		private Thread m_Thread
		{
			get
			{
				if (_thread == null)
					_thread = new Thread(Do);
				return _thread;
			}
			set
			{
				_thread = value;
			}
		}
		#endregion

		private static ILog logger = LogManager.GetLogger("TaskRunner");		
		private BodySyncTaskQueue queue;
		private volatile bool exit;

		public event EventHandler TaskExecuted;

		public TaskRunner(BodySyncTaskQueue queue)
		{
			this.queue = queue;
			this.exit = false;
		}

		public void Start()
		{
			exit = false;

			if (m_Thread.ThreadState != ThreadState.Running)
				m_Thread.Start();
		}

		public void Stop()
		{
			exit = true;
			if (!m_Thread.Join(5000))
			{
				try
				{
					m_Thread.Abort();
				}
				catch 
				{
				}				
			}
			m_Thread = null;
			exit = false;
		}

		private void Do()
		{
			while (!exit)
			{
				try
				{
					ITask task = queue.Dequeue();
					task.Execute();
				}
				catch (Exception e)
				{
					logger.Warn(e);
				}
				finally
				{
					OnTaskExecuted(EventArgs.Empty);
				}
			}
		}

		private void OnTaskExecuted(EventArgs arg)
		{
			EventHandler handler = TaskExecuted;
			if (handler != null)
				handler(this, arg);
		}
	}
}
