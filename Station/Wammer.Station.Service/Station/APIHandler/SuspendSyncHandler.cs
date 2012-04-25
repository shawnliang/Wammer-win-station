using System;
using log4net;
using System.Linq;
using Wammer.PostUpload;

namespace Wammer.Station
{
	class SuspendSyncHandler : HttpHandler
	{
		private readonly StationTimer stationTimer;
		private readonly TaskRunner[] bodySyncRunners;
		private readonly PostUploadTaskRunner postUploadRunner;
		private readonly TaskRunner[] upstreamRunners;

		public SuspendSyncHandler(PostUploadTaskRunner postUploadRunner, StationTimer stationTimer, TaskRunner[] bodySyncRunners, TaskRunner[] upstreamRunners)
		{
			this.stationTimer = stationTimer;
			this.bodySyncRunners = bodySyncRunners;
			this.postUploadRunner = postUploadRunner;
			this.upstreamRunners = upstreamRunners;
		}

		protected override void HandleRequest()
		{
			postUploadRunner.Stop();
			stationTimer.Stop();
			Array.ForEach(bodySyncRunners, (taskRunner) => taskRunner.Stop());
			Array.ForEach(upstreamRunners, (taskRunner) => taskRunner.Stop());

			this.LogDebugMsg("Stop function server successfully");
			RespondSuccess();
		}
	}
}
