using System;
using System.IO;
using System.Threading;

namespace Wammer.Utility
{
	public static class StreamHelper
	{
		public static void Copy(Stream from, Stream to)
		{
			byte[] buffer = new byte[32768];
			int nRead;

			while ((nRead = from.Read(buffer, 0, buffer.Length))>0)
			{
				to.Write(buffer, 0, nRead);
			}
		}

		public static IAsyncResult BeginCopy(Stream from, Stream to, AsyncCallback callback, object state)
		{
			byte[] buffer = new byte[32768];

			StreamCopyState asyncState = new StreamCopyState(from, to, buffer, callback, state);
			from.BeginRead(buffer, 0, buffer.Length, asyncState.DataRead, null);

			return asyncState;
		}

		public static void EndCopy(IAsyncResult ar)
		{
			StreamCopyState result = (StreamCopyState)ar;

			if (result.Error != null)
				throw result.Error;
		}
	}

	class StreamCopyState : IAsyncResult
	{
		private readonly Stream from;
		private readonly Stream to;
		private readonly byte[] buffer;
		private readonly AsyncCallback completeCallback;
		private readonly AutoResetEvent doneEvent;

		public object AsyncState { get; private set; }
		public bool CompletedSynchronously { get; private set; }
		public bool IsCompleted { get; private set; }
		public Exception Error { get; private set; }

		public StreamCopyState(Stream from, Stream to, byte[] buffer, AsyncCallback completeCB, object state)
		{
			this.from = from;
			this.to = to;
			this.buffer = buffer;
			this.completeCallback = completeCB;

			this.doneEvent = new AutoResetEvent(false);
			this.AsyncState = state;
		}
		
		public WaitHandle AsyncWaitHandle
		{
			get { return doneEvent; }
		}

		public void DataRead(IAsyncResult ar)
		{
			try
			{
				int nRead = from.EndRead(ar);

				if (nRead == 0)
				{
					this.IsCompleted = true;
					this.completeCallback(this);
					this.doneEvent.Set();
					return;
				}

				to.BeginWrite(buffer, 0, nRead, this.DataWritten, null);
			}
			catch (Exception e)
			{
				this.Error = e;
				this.completeCallback(this);
				this.doneEvent.Set();
			}
		}

		public void DataWritten(IAsyncResult ar)
		{
			try
			{
				to.EndWrite(ar);
				from.BeginRead(buffer, 0, buffer.Length, this.DataRead, null);
			}
			catch (Exception e)
			{
				this.Error = e;
				this.completeCallback(this);
				this.doneEvent.Set();
			}
		}
		
	}
}
