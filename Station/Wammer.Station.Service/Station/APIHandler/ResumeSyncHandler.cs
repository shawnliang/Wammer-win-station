using System;

namespace Wammer.Station
{
	internal class ResumeSyncHandler : HttpHandler
	{
		public event EventHandler SyncResumed;


		public override void HandleRequest()
		{
			Station.Instance.ResumeSync();

			RespondSuccess();

			OnSyncResumed();
		}

		private void OnSyncResumed()
		{
			var handler = SyncResumed;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}
	}
}