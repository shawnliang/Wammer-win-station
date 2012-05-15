using System;
using System.IO;
using System.Threading;

namespace Wammer.Utility
{
	public static class StreamHelper
	{
		public static void Copy(Stream from, Stream to)
		{
			var buffer = new byte[32768];
			int nRead;

			while ((nRead = from.Read(buffer, 0, buffer.Length)) > 0)
			{
				to.Write(buffer, 0, nRead);
			}
		}

		public static IAsyncResult BeginCopy(Stream from, Stream to, AsyncCallback callback, object state)
		{
			var buffer = new byte[32768];

			var asyncState = new StreamCopyState(from, to, buffer, callback, state);
			from.BeginRead(buffer, 0, buffer.Length, asyncState.DataRead, null);

			return asyncState;
		}

		public static void EndCopy(IAsyncResult ar)
		{
			var result = (StreamCopyState) ar;

			if (result.Error != null)
				throw result.Error;
		}
	}

	internal class StreamCopyState : IAsyncResult
	{
		private readonly byte[] buffer;
		private readonly AsyncCallback completeCallback;
		private readonly AutoResetEvent doneEvent;
		private readonly Stream from;
		private readonly Stream to;

		public StreamCopyState(Stream from, Stream to, byte[] buffer, AsyncCallback completeCB, object state)
		{
			this.from = from;
			this.to = to;
			this.buffer = buffer;
			completeCallback = completeCB;

			doneEvent = new AutoResetEvent(false);
			AsyncState = state;
		}

		public Exception Error { get; private set; }

		#region IAsyncResult Members

		public object AsyncState { get; private set; }
		public bool CompletedSynchronously { get; private set; }
		public bool IsCompleted { get; private set; }

		public WaitHandle AsyncWaitHandle
		{
			get { return doneEvent; }
		}

		#endregion

		public void DataRead(IAsyncResult ar)
		{
			try
			{
				int nRead = from.EndRead(ar);

				if (nRead == 0)
				{
					IsCompleted = true;
					completeCallback(this);
					doneEvent.Set();
					return;
				}

				to.BeginWrite(buffer, 0, nRead, DataWritten, null);
			}
			catch (Exception e)
			{
				Error = e;
				completeCallback(this);
				doneEvent.Set();
			}
		}

		public void DataWritten(IAsyncResult ar)
		{
			try
			{
				to.EndWrite(ar);
				from.BeginRead(buffer, 0, buffer.Length, DataRead, null);
			}
			catch (Exception e)
			{
				Error = e;
				completeCallback(this);
				doneEvent.Set();
			}
		}
	}
}