using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using log4net;

using Wammer.Model;
using Wammer.Utility;
using Wammer.Cloud;
using MongoDB.Driver.Builders;

namespace Wammer.Station
{
	public class AddDriverHandler: HttpHandler
	{
		private static ILog logger = LogManager.GetLogger("AddDriverHandler");
		private readonly string stationId;
		private readonly string resourceBasePath;
		private readonly HttpServer functionServer;

		private const int ERR_USER_HAS_ANOTHER_STATION = 16387;
		private const int ERR_BAD_NAME_PASSWORD = 4097;

		public EventHandler<DriverAddedEvtArgs> DriverAdded;

		public AddDriverHandler(string stationId, string resourceBasePath)
		{
			this.stationId = stationId;
			this.resourceBasePath = resourceBasePath;
		}

		public AddDriverHandler(string stationId, string resourceBasePath, HttpServer functionServer)
		{
			this.stationId = stationId;
			this.resourceBasePath = resourceBasePath;
			this.functionServer = functionServer;
		}

		protected override void HandleRequest()
		{
			string email = Parameters["email"];
			string password = Parameters["password"];

			if (email == null || password == null)
				throw new FormatException("Parameter email or password is missing");

			if (DriverCollection.Instance.FindOne() != null)
				throw new WammerStationException("Already has a driver", (int)StationApiError.DriverExist);

			using (WebClient agent = new WebClient())
			{
				try
				{
					bool has_old_station;
					string stationToken = SignUpStation(email, password, agent, out has_old_station);
					
					logger.Debug("Station logon successfully, start function server");
					functionServer.BlockAuth(false);
					functionServer.Start();
					WriteOnlineStateToDB();

					User user = User.LogIn(agent, email, password);
					Driver driver = new Driver
					{
						email = email,
						folder = Path.Combine(resourceBasePath, "user_" + user.Id),
						user_id = user.Id,
						groups = user.Groups,
						session_token = user.Token
					};

					//if (!has_old_station)
					//{
					//    Driver oldDriver = OldDriverCollection.Instance.FindOne(Query.EQ("_id", user.Id));
					//    has_old_station = (oldDriver != null);
					//}

					DriverCollection.Instance.Save(driver);

					StationCollection.Instance.Save(
						new StationInfo
						{
							Id = stationId,
							SessionToken = stationToken,
							Location = NetworkHelper.GetBaseURL(),
							LastLogOn = DateTime.Now
						}
					);

					OnDriverAdded(new DriverAddedEvtArgs(driver));

					RespondSuccess(new AddUserResponse(stationToken, has_old_station));
				}
				catch (WammerCloudException ex)
				{
					logger.WarnFormat("Unable to add user {0} to station", email);
					logger.Warn(ex.ToString());

					if (ex.HttpError != WebExceptionStatus.ProtocolError)
						throw new WammerStationException(
							"Unable to connect to waveface cloud. Network error?", 
							(int)StationApiError.ConnectToCloudError);
					if (ex.WammerError == ERR_BAD_NAME_PASSWORD)
						throw new WammerStationException(
							"Bad user name or password", (int)StationApiError.AuthFailed);
					else if (ex.WammerError == ERR_USER_HAS_ANOTHER_STATION)
					{
						ThrowAlreadyHasStation(ex.response);
					}
					else
						throw;
				}
			}

		}

		private string SignUpStation(string email, string password, WebClient agent, out bool has_old_station)
		{
			try
			{
				StationApi api = StationApi.SignUp(agent, stationId, email, password);
				api.LogOn(agent, StatusChecker.GetDetail());
				has_old_station = false;
				return api.Token;
			}
			catch (WammerCloudException e)
			{
				if (e.WammerError != ERR_USER_HAS_ANOTHER_STATION)
					throw;

				AddUserResponse resp = fastJSON.JSON.Instance.ToObject<AddUserResponse>(e.response);
				if (resp.station.station_id != this.stationId)
					throw;

				has_old_station = true;
				return StationApi.LogOn(agent, stationId, email, password, StatusChecker.GetDetail()).session_token;
			}
		}
		
		private static void ThrowAlreadyHasStation(string responseJson)
		{
			AddUserResponse resp = fastJSON.JSON.Instance.ToObject<AddUserResponse>(responseJson);
			throw new WammerStationException(
				new AddUserResponse
				{
					api_ret_code = (int)StationApiError.AlreadyHasStaion,
					api_ret_message = "already has a station",
					status = (int)HttpStatusCode.Conflict,
					timestamp = DateTime.UtcNow,
					station = resp.station,
					has_old_station = true
				});
		}

		private void OnDriverAdded(DriverAddedEvtArgs args)
		{
			EventHandler<DriverAddedEvtArgs> handler = this.DriverAdded;

			if (handler != null)
				handler(this, args);
		}

		private static void WriteOnlineStateToDB()
		{
			Model.Service svc = new Model.Service 
			{
				Id = "StationService",
				State = ServiceState.Online
			};
			ServiceCollection.Instance.Save(svc);
		}

		public override object Clone()
		{
			return this.MemberwiseClone();
		}
	}


	public class AddUserResponse : CloudResponse
	{
		public UserStation station { get; set; }
		public string session_token { get; set; }
		public bool has_old_station { get; set; }

		public AddUserResponse()
			: base()
		{
		}

		public AddUserResponse(string session_token, bool has_old_station)
			: base(200, DateTime.UtcNow, 0, "success")
		{
			this.session_token = session_token;
			this.has_old_station = has_old_station;
		}
	}

	public class DriverAddedEvtArgs : EventArgs
	{
		public Driver Driver { get; private set; }

		public DriverAddedEvtArgs(Driver driver)
			: base()
		{
			this.Driver = driver;
		}
	}
}
