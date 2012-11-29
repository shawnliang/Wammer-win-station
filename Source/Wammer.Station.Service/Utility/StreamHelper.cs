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


		// same as BeginCopy. But the data copied will be returned in EndDup().
		public static IAsyncResult BeginDup(Stream from, Stream to, AsyncCallback callback, object state)
		{
			var buffer = new byte[32768];

			var asyncState = new StreamDupState(from, to, buffer, callback, state);
			from.BeginRead(buffer, 0, buffer.Length, asyncState.DataRead, null);

			return asyncState;
		}

		public static MemoryStream EndDup(IAsyncResult ar)
		{
			var result = (StreamDupState)ar;

			if (result.Error != null)
				throw result.Error;

			return result.MirroredData;
		}
	}

	internal class StreamCopyState : IAsyncResult
	{
		protected readonly byte[] buffer;
		protected readonly AsyncCallback completeCallback;
		protected readonly AutoResetEvent doneEvent;
		protected readonly Stream from;
		protected readonly Stream to;

		public StreamCopyState(Stream from, Stream to, byte[] buffer, AsyncCallback completeCB, object state)
		{
			this.from = from;
			this.to = to;
			this.buffer = buffer;
			completeCallback = completeCB;

			doneEvent = new AutoResetEvent(false);
			AsyncState = state;
		}

		public Exception Error { get; protected set; }

		#region IAsyncResult Members

		public object AsyncState { get; protected set; }
		public bool CompletedSynchronously { get; protected set; }
		public bool IsCompleted { get; protected set; }

		public WaitHandle AsyncWaitHandle
		{
			get { return doneEvent; }
		}

		#endregion

		public virtual void DataRead(IAsyncResult ar)
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

	internal class StreamDupState: StreamCopyState
	{
		private MemoryStream mirroredData = new MemoryStream();

		public StreamDupState(Stream from, Stream to, byte[] buffer, AsyncCallback completeCB, object state)
			:base(from, to, buffer, completeCB, state)
		{}

		public override void DataRead(IAsyncResult ar)
		{
			try
			{
				int nRead = from.EndRead(ar);

				if (nRead == 0)
				{
					mirroredData.Position = 0;
					IsCompleted = true;
					completeCallback(this);
					doneEvent.Set();
					return;
				}

				mirroredData.Write(buffer, 0, nRead);
				to.BeginWrite(buffer, 0, nRead, DataWritten, null);
			}
			catch (Exception e)
			{
				Error = e;
				completeCallback(this);
				doneEvent.Set();
			}
		}

		public MemoryStream MirroredData
		{
			get { return mirroredData; }
		}
	}
}