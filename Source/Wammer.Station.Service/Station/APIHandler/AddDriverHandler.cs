using System;
using System.Collections.Generic;
using Wammer.Cloud;
using Wammer.Model;
using Waveface.Stream.Model;

namespace Wammer.Station
{
	/// <summary>
	/// 
	/// </summary>
	[APIHandlerInfo(APIHandlerType.ManagementAPI, "/station/drivers/add/")]
	public class AddDriverHandler : HttpHandler
	{
		#region Var
		private DriverController _driverAgent;
		#endregion

		#region Property
		/// <summary>
		/// Gets the m_ driver agent.
		/// </summary>
		/// <value>The m_ driver agent.</value>
		public DriverController m_DriverAgent
		{
			get
			{
				if (_driverAgent == null)
				{
					_driverAgent = new DriverController();
					_driverAgent.BeforeDriverSaved += _driverAgent_BeforeDriverSaved;
					_driverAgent.DriverAdded += _driverAgent_DriverAdded;
				}
				return _driverAgent;
			}
		}
		#endregion


		#region Event
		/// <summary>
		/// Occurs when [driver added].
		/// </summary>
		public event EventHandler<DriverAddedEvtArgs> DriverAdded;

		/// <summary>
		/// Occurs when [before driver saved].
		/// </summary>
		public event EventHandler<BeforeDriverSavedEvtArgs> BeforeDriverSaved;
		#endregion


		#region Protected Method
		/// <summary>
		/// Called when [driver added].
		/// </summary>
		/// <param name="args">The args.</param>
		protected void OnDriverAdded(DriverAddedEvtArgs args)
		{
			var handler = DriverAdded;

			if (handler != null)
				handler(this, args);
		}

		/// <summary>
		/// Called when [before driver saved].
		/// </summary>
		/// <param name="args">The args.</param>
		protected void OnBeforeDriverSaved(BeforeDriverSavedEvtArgs args)
		{
			var handler = BeforeDriverSaved;

			if (handler != null)
				handler(this, args);
		}
		#endregion



		#region Public Method
		/// <summary>
		/// Handles the request.
		/// </summary>
		public override void HandleRequest()
		{
			if (Parameters[CloudServer.PARAM_SESSION_TOKEN] != null && Parameters[CloudServer.PARAM_USER_ID] != null)
			{
				var sessionToken = Parameters[CloudServer.PARAM_SESSION_TOKEN];
				var userID = Parameters[CloudServer.PARAM_USER_ID];
				var userFilder = Parameters[CloudServer.PARAM_USER_FOLDER];

				RespondSuccess(m_DriverAgent.AddDriver(userFilder, Station.Instance.StationID, userID, sessionToken));
			}
			else
			{
				CheckParameter(CloudServer.PARAM_EMAIL,
							   CloudServer.PARAM_PASSWORD,
							   CloudServer.PARAM_DEVICE_ID,
							   CloudServer.PARAM_DEVICE_NAME);

				var email = Parameters[CloudServer.PARAM_EMAIL];
				var password = Parameters[CloudServer.PARAM_PASSWORD];
				var deviceId = Parameters[CloudServer.PARAM_DEVICE_ID];
				var deviceName = Parameters[CloudServer.PARAM_DEVICE_NAME];
				var userFilder = Parameters[CloudServer.PARAM_USER_FOLDER];

				RespondSuccess(m_DriverAgent.AddDriver(userFilder, Station.Instance.StationID, email, password, deviceId, deviceName));
			}
		}
		#endregion



		#region Event Process
		/// <summary>
		/// _drivers the agent_ driver added.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The e.</param>
		void _driverAgent_DriverAdded(object sender, DriverAddedEvtArgs e)
		{
			OnDriverAdded(e);
		}

		/// <summary>
		/// _drivers the agent_ before driver saved.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The e.</param>
		void _driverAgent_BeforeDriverSaved(object sender, BeforeDriverSavedEvtArgs e)
		{
			OnBeforeDriverSaved(e);
		}
		#endregion
	}


	public class AddUserResponse : CloudResponse
	{
		public AddUserResponse()
			: base(200, DateTime.UtcNow, 0, "success")
		{
		}

		public string UserId { get; set; }
		public bool IsPrimaryStation { get; set; }
		public List<UserStation> Stations { get; set; }
	}

	public class DriverAddedEvtArgs : EventArgs
	{
		public DriverAddedEvtArgs(Driver driver, object userData)
		{
			Driver = driver;
			UserData = userData;
		}

		public Driver Driver { get; private set; }
		public object UserData { get; private set; }
	}

	public class BeforeDriverSavedEvtArgs : EventArgs
	{
		public object UserData { get; set; }

		public BeforeDriverSavedEvtArgs(Driver driver)
		{
			Driver = driver;
		}

		public Driver Driver { get; private set; }
	}
}
