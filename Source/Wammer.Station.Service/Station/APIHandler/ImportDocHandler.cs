using Waveface.Stream.Model;

namespace Wammer.Station.APIHandler
{
	[APIHandlerInfo(APIHandlerType.ManagementAPI, "/station/ImportDoc")]
	class ImportDocHandler : HttpHandler
	{
		public override void HandleRequest()
		{
			CheckParameter(CloudServer.PARAM_API_KEY, CloudServer.PARAM_SESSION_TOKEN, "paths");

			var session_token = Parameters[CloudServer.PARAM_SESSION_TOKEN];

			if (string.IsNullOrEmpty(session_token))
				throw new WammerStationException("session is empty", -1);

			var userSession = LoginedSessionCollection.Instance.FindOneById(session_token);
			if (userSession == null)
				throw new WammerStationException("session does not exist", -1);

			var paths = Parameters["paths"].Split(',');

			TaskQueue.Enqueue(new ImportDocTask(userSession, paths), TaskPriority.VeryLow);

			RespondSuccess();
		}

		public override object Clone()
		{
			return MemberwiseClone();
		}
	}
}
