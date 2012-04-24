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

		public SuspendSyncHandler(PostUploadTaskRunner postUploadRunner, StationTimer stationTimer, TaskRunner[] bodySyncRunners)
		{
			this.stationTimer = stationTimer;
			this.bodySyncRunners = bodySyncRunners;
			this.postUploadRunner = postUploadRunner;
		}

		protected override void HandleRequest()
		{
			postUploadRunner.Stop();
			stationTimer.Stop();
			Array.ForEach(bodySyncRunners, (taskRunner) => taskRunner.Stop());

			this.LogDebugMsg("Stop function server successfully");
			RespondSuccess();
		}
	}
}
