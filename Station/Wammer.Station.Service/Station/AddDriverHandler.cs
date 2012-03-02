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

        //Larry 2012/03/01 Mark, function server control by admin panel
        //private readonly HttpServer functionServer;
        //private readonly StationTimer stationTimer;

		private const int ERR_USER_HAS_ANOTHER_STATION = 16387;
		private const int ERR_BAD_NAME_PASSWORD = 4097;

		public event EventHandler<DriverAddedEvtArgs> DriverAdded;

		public AddDriverHandler(string stationId, string resourceBasePath)
		{
			this.stationId = stationId;
			this.resourceBasePath = resourceBasePath;
		}

		public AddDriverHandler(string stationId, string resourceBasePath, HttpServer functionServer, StationTimer stationTimer)
		{
			this.stationId = stationId;
			this.resourceBasePath = resourceBasePath;

            //Larry 2012/03/01 Mark, function server control by admin panel
            //this.functionServer = functionServer;
            //this.stationTimer = stationTimer;
		}

		protected override void HandleRequest()
		{
			string email = Parameters["email"];
			string password = Parameters["password"];

			if (email == null || password == null)
				throw new FormatException("Parameter email or password is missing");

            Driver driver = Model.DriverCollection.Instance.FindOne(Query.EQ("email", email));
            if(driver != null)
                throw new WammerStationException("Already has the same driver", (int)StationApiError.DriverExist);

            //Larry 2012/03/01 Mark, accept service multiple user
            //if (DriverCollection.Instance.FindOne() != null)
            //    throw new WammerStationException("Already has a driver", (int)StationApiError.DriverExist);

			using (WebClient agent = new WebClient())
			{
				try
				{
					string stationToken = SignUpStation(email, password, agent);
					
                    //Larry 2012/03/01 Mark
                    //logger.Debug("Station logon successfully, start function server");
                    //functionServer.BlockAuth(false);
                    //functionServer.Start();
                    //stationTimer.Start();
                    //WriteOnlineStateToDB();

					User user = User.LogIn(agent, email, password);
					driver = new Driver
					{
						email = email,
						folder = Path.Combine(resourceBasePath, "user_" + user.Id),
						user_id = user.Id,
						groups = user.Groups,
						session_token = user.Token
					};

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

					RespondSuccess(new AddUserResponse(stationToken));
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
					else
						throw;
				}
			}

		}

		private string SignUpStation(string email, string password, WebClient agent)
		{
			StationApi api = StationApi.SignUp(agent, stationId, email, password);
			api.LogOn(agent, StatusChecker.GetDetail());
			return api.Token;
		}

		private void OnDriverAdded(DriverAddedEvtArgs args)
		{
			EventHandler<DriverAddedEvtArgs> handler = this.DriverAdded;

			if (handler != null)
				handler(this, args);
		}

        //Larry 2012/03/02 Mark, multiple user use the same service
        //private static void WriteOnlineStateToDB()
        //{
        //    Model.Service svc = new Model.Service 
        //    {
        //        Id = "StationService",
        //        State = ServiceState.Online
        //    };
        //    ServiceCollection.Instance.Save(svc);
        //}

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

		public AddUserResponse(string session_token)
			: base(200, DateTime.UtcNow, 0, "success")
		{
			this.session_token = session_token;
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
