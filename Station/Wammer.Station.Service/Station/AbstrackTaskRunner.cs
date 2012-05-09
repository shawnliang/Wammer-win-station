using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using log4net;

namespace Wammer.Station
{
	public abstract class AbstrackTaskRunner
	{
		protected Thread _thread; 

		protected Thread m_Thread
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
		protected volatile bool exit = false;

		public virtual void Start()
		{
			exit = false;

			if (m_Thread.ThreadState == ThreadState.Unstarted)
				m_Thread.Start();
		}

		public virtual void Stop()
		{
			exit = true;
			if (m_Thread.ThreadState != ThreadState.Unstarted)
			{
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
			}
			m_Thread = null;
			exit = false;
		}

		protected abstract void Do();
	}
}
