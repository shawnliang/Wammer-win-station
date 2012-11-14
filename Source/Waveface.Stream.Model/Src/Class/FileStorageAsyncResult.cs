using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;

namespace Waveface.Stream.Model
{
	internal class FileStorageAsyncResult : IAsyncResult
	{
		private readonly IAsyncResult fileStreamAsyncResult;
		private readonly FileStream fs;

		public FileStorageAsyncResult(IAsyncResult innerObject, FileStream fs)
		{
			fileStreamAsyncResult = innerObject;
			this.fs = fs;
		}

		public string TempFile { get; set; }
		public string TargetFile { get; set; }

		public IAsyncResult InnerObject
		{
			get { return fileStreamAsyncResult; }
		}

		public FileStream OutputStream
		{
			get { return fs; }
		}

		#region IAsyncResult Members

		public object AsyncState
		{
			get { return fileStreamAsyncResult.AsyncState; }
		}

		public WaitHandle AsyncWaitHandle
		{
			get { return fileStreamAsyncResult.AsyncWaitHandle; }
		}

		public bool CompletedSynchronously
		{
			get { return fileStreamAsyncResult.CompletedSynchronously; }
		}

		public bool IsCompleted
		{
			get { return fileStreamAsyncResult.IsCompleted; }
		}

		#endregion
	}
}
