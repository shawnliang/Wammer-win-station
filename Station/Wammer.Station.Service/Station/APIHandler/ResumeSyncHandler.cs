using System;
using System.Net;
using log4net;
using MongoDB.Driver.Builders;
using Wammer.Cloud;
using Wammer.Model;
using System.Linq;
using Wammer.Utility;
using Wammer.PostUpload;

namespace Wammer.Station
{
	class ResumeSyncHandler : HttpHandler
	{
		private readonly PostUploadTaskRunner postUploadRunner;
		private readonly StationTimer stationTimer;
		private readonly AbstrackTaskRunner[] bodySyncRunners;
		private readonly AbstrackTaskRunner[] upstreamRunners;

		public ResumeSyncHandler(PostUploadTaskRunner postUploadRunner, StationTimer stationTimer, AbstrackTaskRunner[] bodySyncRunners, AbstrackTaskRunner[] upstreamRunners)
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
			Array.ForEach(bodySyncRunners, (taskRunner) => taskRunner.Start());
			Array.ForEach(upstreamRunners, (taskRunner) => taskRunner.Start());

			this.LogDebugMsg("Start function server successfully");

			RespondSuccess();
		}
	}

	public class StationOnlineResponse : CloudResponse
	{
		public string session_token { get; set; }
	}
}
