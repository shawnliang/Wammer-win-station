using MongoDB.Driver.Builders;
using System;
using Waveface.Stream.Model;

namespace Wammer.Station.APIHandler
{
	[APIHandlerInfo(APIHandlerType.ManagementAPI, "/station/moveFolder/")]
	class MoveFolderHandler : HttpHandler
	{
		public override void HandleRequest()
		{
			CheckParameter(CloudServer.PARAM_API_KEY, CloudServer.PARAM_SESSION_TOKEN, CloudServer.PARAM_USER_ID, CloudServer.PARAM_USER_FOLDER);

			var user_id = Parameters[CloudServer.PARAM_USER_ID];
			var folder = Parameters[CloudServer.PARAM_USER_FOLDER];

			var user = DriverCollection.Instance.FindOneById(user_id);
			if (user == null)
				throw new WammerStationException("user does not exist anymore: " + user_id, (int)StationApiError.UserNotExist);

			if (user.folder.Equals(folder, StringComparison.InvariantCultureIgnoreCase))
			{
				RespondSuccess();
				return;
			}

			var move = new FolderMover(new FolderUtility());

			Station.Instance.Stop();

			try
			{
				move.MoveFolder(user.folder, folder);

				DriverCollection.Instance.Update(Query.EQ("_id", user_id), Update.Set("folder", folder));

				RespondSuccess();
			}
			catch (Exception e)
			{
				throw new WammerStationException(e.Message, -1, e);
			}
			finally
			{
				Station.Instance.Start();
			}
		}
	}
}
