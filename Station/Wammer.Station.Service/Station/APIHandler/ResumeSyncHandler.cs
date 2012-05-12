using System;
using Wammer.Cloud;
using Wammer.PostUpload;

namespace Wammer.Station
{
	internal class ResumeSyncHandler : HttpHandler
	{
		private readonly AbstrackTaskRunner[] bodySyncRunners;
		private readonly PostUploadTaskRunner postUploadRunner;
		private readonly StationTimer stationTimer;
		private readonly AbstrackTaskRunner[] upstreamRunners;

		public ResumeSyncHandler(PostUploadTaskRunner postUploadRunner, StationTimer stationTimer,
		                         AbstrackTaskRunner[] bodySyncRunners, AbstrackTaskRunner[] upstreamRunners)
		{
			this.postUploadRunner = postUploadRunner;
			this.stationTimer = stationTimer;
			this.bodySyncRunners = bodySyncRunners;
			this.upstreamRunners = upstreamRunners;
		}

		public override void HandleRequest()
		{
			postUploadRunner.Start();
			stationTimer.Start();
			Array.ForEach(bodySyncRunners, taskRunner => taskRunner.Start());
			Array.ForEach(upstreamRunners, taskRunner => taskRunner.Start());

			this.LogDebugMsg("Start function server successfully");

			RespondSuccess();
		}
	}

	public class StationOnlineResponse : CloudResponse
	{
		public string session_token { get; set; }
	}
}