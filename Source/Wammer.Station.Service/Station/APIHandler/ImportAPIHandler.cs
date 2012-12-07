
using Wammer.Cloud;

namespace Wammer.Station
{
	[APIHandlerInfo(APIHandlerType.ManagementAPI, "/station/Import")]
	public class ImportAPIHandler : HttpHandler
	{
		#region Public Method
		/// <summary>
		/// Handles the request.
		/// </summary>
		public override void HandleRequest()
		{
			const string PATHS_KEY = "paths";
			CheckParameter(PATHS_KEY, CloudServer.PARAM_GROUP_ID);

			var apiKey = Parameters[CloudServer.PARAM_API_KEY];
			var sessionToken = Parameters[CloudServer.PARAM_SESSION_TOKEN];
			var groupID = Parameters[CloudServer.PARAM_GROUP_ID];
			var paths = Parameters[PATHS_KEY];

			TaskQueue.Enqueue(new ImportTask(apiKey, sessionToken, groupID, paths), TaskPriority.VeryLow);

			RespondSuccess();
		}

		/// <summary>
		/// Clones this instance.
		/// </summary>
		/// <returns></returns>
		public override object Clone()
		{
			return this.MemberwiseClone();
		}
		#endregion
	}
}