using System;
using Wammer.PostUpload;

namespace Wammer.Station
{
	internal class SuspendSyncHandler : HttpHandler
	{
		private readonly AbstrackTaskRunner[] bodySyncRunners;
		private readonly PostUploadTaskRunner postUploadRunner;
		private readonly StationTimer stationTimer;
		private readonly AbstrackTaskRunner[] upstreamRunners;

		public event EventHandler SyncSuspended;

		public SuspendSyncHandler(PostUploadTaskRunner postUploadRunner, StationTimer stationTimer,
		                          AbstrackTaskRunner[] bodySyncRunners, AbstrackTaskRunner[] upstreamRunners)
		{
			this.stationTimer = stationTimer;
			this.bodySyncRunners = bodySyncRunners;
			this.postUploadRunner = postUploadRunner;
			this.upstreamRunners = upstreamRunners;
		}

		public override void HandleRequest()
		{
			postUploadRunner.Stop();
			stationTimer.Stop();
			Array.ForEach(bodySyncRunners, taskRunner => taskRunner.Stop());
			Array.ForEach(upstreamRunners, taskRunner => taskRunner.Stop());

			this.LogDebugMsg("Stop function server successfully");
			RespondSuccess();

			OnSyncSuspended();
		}

		private void OnSyncSuspended()
		{
			EventHandler handler = SyncSuspended;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}
	}
}