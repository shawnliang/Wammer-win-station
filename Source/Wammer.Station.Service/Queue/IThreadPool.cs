using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wammer.Station;

namespace Wammer.Queue
{
	public interface IThreadPool
	{
		void QueueWorkItem(System.Threading.WaitCallback callback);
	}
}
