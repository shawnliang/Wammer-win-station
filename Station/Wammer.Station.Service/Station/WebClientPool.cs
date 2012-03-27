using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Wammer.Station
{
	class WebClientPool
	{
		private static Stack<WebClient> webClients = new Stack<WebClient>();
		private static System.Threading.Semaphore sem = new System.Threading.Semaphore(20, 20);

		public static WebClientProxy GetFreeClient()
		{
			sem.WaitOne();

			lock (webClients)
			{
				if (webClients.Count > 0)
					return new WebClientProxy(webClients.Pop());
				else
					return new WebClientProxy(new WebClient());
			}
		}

		public static void Return(WebClient agent)
		{
			lock (webClients)
			{
				webClients.Push(agent);
			}

			sem.Release();
		}
	}


	class WebClientProxy: IDisposable
	{
		public WebClient Agent { get; private set; }

		public WebClientProxy(WebClient agent)
		{
			this.Agent = agent;
		}

		public void Dispose()
		{
			WebClientPool.Return(this.Agent);
		}
	}
}
