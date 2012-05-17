using System;
using System.IO;
using System.Net;
using MongoDB.Driver.Builders;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.Utility;

namespace Wammer.Station
{
	public class RemoveOwnerHandler : HttpHandler
	{
		#region Var
		private DriverController _driverAgent;
		#endregion

		#region Property
		/// <summary>
		/// Gets or sets the m_ station ID.
		/// </summary>
		/// <value>The m_ station ID.</value>
		private String m_StationID { get; set; }

		/// <summary>
		/// Gets the m_ driver agent.
		/// </summary>
		/// <value>The m_ driver agent.</value>
		private DriverController m_DriverAgent
		{
			get
			{
				return _driverAgent ?? new DriverController();
			}
		}
		#endregion


		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="RemoveOwnerHandler"/> class.
		/// </summary>
		/// <param name="stationId">The station id.</param>
		public RemoveOwnerHandler(string stationId)
		{
			this.m_StationID = stationId;
		} 
		#endregion


		public override void HandleRequest()
		{
			CheckParameter(CloudServer.PARAM_USER_ID, CloudServer.PARAM_REMOVE_ALL_DATA);

			var userID = Parameters[CloudServer.PARAM_USER_ID];
			var removeAllData = bool.Parse(Parameters[CloudServer.PARAM_REMOVE_ALL_DATA]);

			m_DriverAgent.RemoveDriver(m_StationID, userID, removeAllData);

			RespondSuccess();
		}
	}
}