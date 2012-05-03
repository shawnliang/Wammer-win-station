using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using log4net;
using MongoDB.Driver.Builders;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.Utility;
using System.Linq;

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
		public event EventHandler<BeforeDriverSavedEvtArgs> BeforeDriverSaved;

		public AddDriverHandler(string stationId, string resourceBasePath)
		{
			this.stationId = stationId;
			this.resourceBasePath = resourceBasePath;
		}

		public override void HandleRequest()
		{
			CheckParameter(
				CloudServer.PARAM_EMAIL,
				CloudServer.PARAM_PASSWORD);

			string email = Parameters[CloudServer.PARAM_EMAIL];
			string password = Parameters[CloudServer.PARAM_PASSWORD];

			using (WebClient client = new DefaultWebClient())
			{
				try
				{
					Driver existingDriver = DriverCollection.Instance.FindOne(Query.EQ("email", email));
					Boolean isDriverExists = existingDriver != null;

					Driver driver = null;
					User user = null;
					
					if (isDriverExists)
					{
						user = User.LogIn(client, email, password);

						if (user == null)
							throw new WammerStationException("Logined user not found", (int)StationApiError.AuthFailed);

						if(user.Id == existingDriver.user_id)
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

						//Remove the user from db, and stop service this user
						DriverCollection.Instance.Remove(Query.EQ("_id", existingDriver.user_id));

						if (Directory.Exists(existingDriver.folder))
							Directory.Delete(existingDriver.folder, true);

						//All driver removed => Remove station from db
						driver = DriverCollection.Instance.FindOne();
						if (driver == null)
							StationCollection.Instance.RemoveAll();
					}

					string stationToken = SignUpStation(email, password, client);		

					if(!isDriverExists)
						user = User.LogIn(client, email, password);

					if (user == null)
						throw new WammerStationException("Logined user not found", (int)StationApiError.AuthFailed);

					driver = new Driver
					{
						email = email,
						folder = Path.Combine(resourceBasePath, "user_" + user.Id),
						user_id = user.Id,
						groups = user.Groups,
						session_token = stationToken,
						isPrimaryStation = IsThisPrimaryStation(user.Stations),
						ref_count = 1
					};

					if (!Directory.Exists(driver.folder))
						Directory.CreateDirectory(driver.folder);

					OnBeforeDriverSaved(new BeforeDriverSavedEvtArgs(driver));

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

		private void OnBeforeDriverSaved(BeforeDriverSavedEvtArgs args)
		{
			EventHandler<BeforeDriverSavedEvtArgs> handler = this.BeforeDriverSaved;

			if (handler != null)
				handler(this, args);
		}

		public override object Clone()
		{
			return this.MemberwiseClone();
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

	public class BeforeDriverSavedEvtArgs : EventArgs
	{
		public Driver Driver { get; private set; }

		public BeforeDriverSavedEvtArgs(Driver driver)
			: base()
		{
			this.Driver = driver;
		}
	}
}
