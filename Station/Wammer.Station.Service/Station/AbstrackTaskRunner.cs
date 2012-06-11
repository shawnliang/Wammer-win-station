using System.Threading;

namespace Wammer.Station
{
	public abstract class AbstrackTaskRunner
	{
		protected Thread _thread;

		protected volatile bool exit;

		protected Thread m_Thread
		{
			get { return _thread ?? (_thread = new Thread(Do)); }
			set { _thread = value; }
		}

		public virtual void Start()
		{
			exit = false;

			if (m_Thread.ThreadState == ThreadState.Unstarted)
				m_Thread.Start();
		}

		public virtual void JoinOrKill()
		{
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
		}

		protected abstract void Do();
		public abstract void StopAsync();
	}
}