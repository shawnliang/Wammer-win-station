using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using log4net;
using MongoDB.Driver.Builders;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.Utility;

namespace Wammer.Station
{
	public class AddDriverHandler: HttpHandler
	{
		private static ILog logger = LogManager.GetLogger("AddDriverHandler");
		private readonly string stationId;
		private readonly string resourceBasePath;

		private const int ERR_USER_HAS_ANOTHER_STATION = 16387;
		private const int ERR_BAD_NAME_PASSWORD = 4097;

		public event EventHandler<DriverAddedEvtArgs> DriverAdded;

		public AddDriverHandler(string stationId, string resourceBasePath)
		{
			this.stationId = stationId;
			this.resourceBasePath = resourceBasePath;
		}


		protected override void HandleRequest()
		{
			string email = Parameters["email"];
			string password = Parameters["password"];

			if (email == null || password == null)
				throw new FormatException("Parameter email or password is missing");

			using (WebClient agent = new WebClient())
			{
				try
				{
					Driver existingDriver = Model.DriverCollection.Instance.FindOne(Query.EQ("email", email));
					Boolean isDriverExists = existingDriver != null;                    

					if (isDriverExists)
					{
                        existingDriver.ref_count += 1;
                        DriverCollection.Instance.Save(existingDriver);

                        RespondSuccess(new AddUserResponse()
                        {
                            UserId = existingDriver.user_id,
                            IsPrimaryStation = existingDriver.isPrimaryStation
                        });
						return;
					}

                    string stationToken = SignUpStation(email, password, agent);
                    User user = User.LogIn(agent, email, password);				
								
					Driver driver = new Driver
					{
						email = email,
						folder = Path.Combine(resourceBasePath, "user_" + user.Id),
						user_id = user.Id,
						groups = user.Groups,
						session_token = user.Token,
						isPrimaryStation = IsThisPrimaryStation(user.Stations),
						ref_count = 1
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

					RespondSuccess(new AddUserResponse() 
						{ 
							UserId = user.Id, 
							IsPrimaryStation = driver.isPrimaryStation 
						});
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

		private bool IsThisPrimaryStation(List<UserStation> stations)
		{
			if (stations == null)
				return false;

			foreach (UserStation s in stations)
				if (s.station_id == this.stationId)
				{
					if (string.Compare(s.type, "primary", true) == 0)
						return true;
					else
						return false;
				}

			return false;
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

		public override object Clone()
		{
			return this.MemberwiseClone();
		}

		public override void OnTaskEnqueue(EventArgs e)
		{
		}
	}


	public class AddUserResponse : CloudResponse
	{
		public string UserId { get; set; }
		public bool IsPrimaryStation { get; set; }

		public AddUserResponse()
			: base(200, DateTime.UtcNow, 0, "success")
		{
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
