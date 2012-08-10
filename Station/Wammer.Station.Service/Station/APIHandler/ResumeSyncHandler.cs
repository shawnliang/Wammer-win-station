using System;

namespace Wammer.Station
{
	[APIHandlerInfo(APIHandlerType.ManagementAPI, "/station/resumeSync/")]
	internal class ResumeSyncHandler : HttpHandler
	{
		public event EventHandler SyncResumed;


		public override void HandleRequest()
		{
			Station.Instance.ResumeSyncByUser();

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