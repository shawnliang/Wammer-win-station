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

			if (Drivers.collection.FindOne() != null)
				throw new WammerStationException("Already has a driver", (int)StationApiError.DriverExist);

			using (WebClient agent = new WebClient())
			{
				try
				{
					Cloud.StationApi api = Cloud.StationApi.SignUp(agent, stationId, email, password);
					StationLogOnResponse logonRes = api.LogOn(agent, StatusChecker.GetDetail());

					logger.Debug("Station logon successfully");
					if (functionServer != null)
					{
						functionServer.Start();
						WriteOnlineStateToDB();
					}

					User user = User.LogIn(agent, email, password);
					Drivers driver = new Drivers
					{
						email = email,
						folder = Path.Combine(resourceBasePath, user.Groups[0].name),
						user_id = user.Id,
						groups = user.Groups,
						session_token = user.Token
					};

					Drivers.collection.Save(driver);

					StationInfo.collection.Save(
						new StationInfo
						{
							Id = stationId,
							SessionToken = api.Token,
							Location = NetworkHelper.GetBaseURL(),
							LastLogOn = DateTime.Now
						}
					);

					OnDriverAdded(new DriverAddedEvtArgs(driver));

					RespondSuccess(new AddUserResponse {session_token = logonRes.session_token});
				}
				catch (WammerCloudException ex)
				{
					logger.WarnFormat("Unable to add user {0} to station", email);
					logger.Warn(ex.ToString());

					if (ex.HttpError != WebExceptionStatus.ProtocolError)
						throw new WammerStationException(
							"Unable to connect to waveface cloud. Network error?", 
							(int)StationApiError.ConnectToCloudError);
					if (ex.WammerError == 4097)
						throw new WammerStationException(
							"Bad user name or password", (int)StationApiError.AuthFailed);
					else if (ex.WammerError == 16387)
					{
						ThrowAlreadyHasStation(ex.response);
					}
					else
						throw;
				}
			}

		}
		
		private static void ThrowAlreadyHasStation(string responseJson)
		{
			AddUserResponse resp = fastJSON.JSON.Instance.ToObject<AddUserResponse>(responseJson);
			throw new WammerStationException(
				new AddUserResponse
				{
					api_ret_code = (int)StationApiError.AlreadyHasStaion,
					api_ret_msg = "already has a station",
					status = (int)HttpStatusCode.Conflict,
					timestamp = DateTime.UtcNow,
					station = resp.station
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
			Model.Service svc = ServiceCollection.FindOne(Query.EQ("_id", "StationService"));
			if (svc == null)
			{
				svc = new Model.Service { Id = "StationService", State = ServiceState.Online };
			}
			else
				svc.State = ServiceState.Online;

			ServiceCollection.Save(svc);
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

		public AddUserResponse()
			: base()
		{
		}
	}

	public class DriverAddedEvtArgs : EventArgs
	{
		public Drivers Driver { get; private set; }

		public DriverAddedEvtArgs(Drivers driver)
			: base()
		{
			this.Driver = driver;
		}
	}
}
