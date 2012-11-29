using System;
using Wammer.Cloud;

namespace Wammer.Station
{
	[APIHandlerInfo(APIHandlerType.ManagementAPI, "/station/drivers/remove/")]
	public class RemoveOwnerHandler : HttpHandler
	{
		#region Var
		private DriverController _driverAgent;
		#endregion

		#region Property
		/// <summary>
		/// Gets the m_ driver agent.
		/// </summary>
		/// <value>The m_ driver agent.</value>
		private DriverController m_DriverAgent
		{
			get
			{
				return _driverAgent ?? (_driverAgent = new DriverController());
			}
		}
		#endregion


		#region Events
		public event EventHandler<DriverRemovedEventArgs> DriverRemoved;
		#endregion


		public override void HandleRequest()
		{
			CheckParameter(CloudServer.PARAM_USER_ID, CloudServer.PARAM_REMOVE_ALL_DATA);

			var userID = Parameters[CloudServer.PARAM_USER_ID];
			var removeAllData = bool.Parse(Parameters[CloudServer.PARAM_REMOVE_ALL_DATA]);

			m_DriverAgent.RemoveDriver(Station.Instance.StationID, userID, removeAllData);

			OnDriverRemoved(userID);
			RespondSuccess();
		}

		private void OnDriverRemoved(string userId)
		{
			EventHandler<DriverRemovedEventArgs> handler = DriverRemoved;
			if (handler != null)
				handler(this, new DriverRemovedEventArgs(userId));
		}
	}

	public class DriverRemovedEventArgs: EventArgs
	{
		public string UserId { get; private set; }

		public DriverRemovedEventArgs(string user_id)
		{
			UserId = user_id;
		}
	}
}