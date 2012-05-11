using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.Utility;

namespace Wammer.Station
{
	public class AddDriverHandler : HttpHandler
	{
		private readonly string resourceBasePath;
		private readonly string stationId;

		public AddDriverHandler(string stationId, string resourceBasePath)
		{
			this.stationId = stationId;
			this.resourceBasePath = resourceBasePath;
		}

		public event EventHandler<DriverAddedEvtArgs> DriverAdded;
		public event EventHandler<BeforeDriverSavedEvtArgs> BeforeDriverSaved;

		public override void HandleRequest()
		{
			if (Parameters[CloudServer.PARAM_SESSION_TOKEN] != null && Parameters[CloudServer.PARAM_USER_ID] != null)
			{
				string sessionToken = Parameters[CloudServer.PARAM_SESSION_TOKEN];
				string userId = Parameters[CloudServer.PARAM_USER_ID];

				Driver existingDriver = DriverCollection.Instance.FindOne(Query.EQ("_id", userId));
				if (existingDriver != null)
				{
					existingDriver.ref_count += 1;
					DriverCollection.Instance.Save(existingDriver);
					RespondSuccess(new AddUserResponse
					               	{
					               		UserId = existingDriver.user_id,
					               		IsPrimaryStation = existingDriver.isPrimaryStation
					               	});
				}
				else
				{
					StationSignUpResponse res = StationApi.SignUpBySession(new WebClient(), sessionToken, stationId);
					StationCollection.Instance.Update(
						Query.EQ("_id", stationId),
						Update.Set("SessionToken", res.session_token)
							.Set("Location", NetworkHelper.GetBaseURL())
							.Set("LastLogOn", DateTime.Now),
						UpdateFlags.Upsert
						);

					var driver = new Driver
					             	{
					             		user_id = res.user.user_id,
					             		email = res.user.email,
					             		groups = res.groups,
					             		folder = Path.Combine(resourceBasePath, "user_" + res.user.user_id),
					             		session_token = res.session_token,
					             		isPrimaryStation = IsThisPrimaryStation(res.stations),
					             		ref_count = 1,
					             		stations = res.stations
					             	};

					Directory.CreateDirectory(driver.folder);

					OnBeforeDriverSaved(new BeforeDriverSavedEvtArgs(driver));

					DriverCollection.Instance.Save(driver);

					OnDriverAdded(new DriverAddedEvtArgs(driver));

					RespondSuccess(new AddUserResponse
					               	{
					               		UserId = driver.user_id,
					               		IsPrimaryStation = driver.isPrimaryStation,
					               		Stations = driver.stations
					               	});
				}
			}
			else
			{
				CheckParameter(CloudServer.PARAM_EMAIL,
				               CloudServer.PARAM_PASSWORD,
				               CloudServer.PARAM_DEVICE_ID,
				               CloudServer.PARAM_DEVICE_NAME);

				string email = Parameters[CloudServer.PARAM_EMAIL];
				string password = Parameters[CloudServer.PARAM_PASSWORD];
				string deviceId = Parameters[CloudServer.PARAM_DEVICE_ID];
				string deviceName = Parameters[CloudServer.PARAM_DEVICE_NAME];

				using (WebClient agent = new DefaultWebClient())
				{
					Driver existingDriver = DriverCollection.Instance.FindOne(Query.EQ("email", email));

					if (existingDriver != null)
					{
						// check if this email is re-registered, if yes, delete old driver's data
						User user = User.LogIn(agent, email, password, deviceId, deviceName);

						if (user == null)
							throw new WammerStationException("Logined user not found", (int) StationLocalApiError.AuthFailed);

						if (user.Id == existingDriver.user_id)
						{
							existingDriver.ref_count += 1;
							DriverCollection.Instance.Save(existingDriver);

							RespondSuccess(new AddUserResponse
							               	{
							               		UserId = existingDriver.user_id,
							               		IsPrimaryStation = existingDriver.isPrimaryStation,
							               		Stations = existingDriver.stations
							               	});
							return;
						}

						//Remove the user from db, and stop service this user
						DriverCollection.Instance.Remove(Query.EQ("_id", existingDriver.user_id));

						if (Directory.Exists(existingDriver.folder))
							Directory.Delete(existingDriver.folder, true);

						//All driver removed => Remove station from db
						if (DriverCollection.Instance.FindOne() == null)
							StationCollection.Instance.RemoveAll();
					}

					StationSignUpResponse res = StationApi.SignUpByEmailPassword(agent, stationId, email, password, deviceId,
					                                                             deviceName);
					StationCollection.Instance.Update(
						Query.EQ("_id", stationId),
						Update.Set("SessionToken", res.session_token)
							.Set("Location", NetworkHelper.GetBaseURL())
							.Set("LastLogOn", DateTime.Now),
						UpdateFlags.Upsert
						);

					var driver = new Driver
					             	{
					             		user_id = res.user.user_id,
					             		email = email,
					             		folder = Path.Combine(resourceBasePath, "user_" + res.user.user_id),
					             		groups = res.groups,
					             		session_token = res.session_token,
					             		isPrimaryStation = IsThisPrimaryStation(res.stations),
					             		ref_count = 1,
					             		stations = res.stations
					             	};

					Directory.CreateDirectory(driver.folder);

					OnBeforeDriverSaved(new BeforeDriverSavedEvtArgs(driver));

					DriverCollection.Instance.Save(driver);


					OnDriverAdded(new DriverAddedEvtArgs(driver));

					RespondSuccess(new AddUserResponse
					               	{
					               		UserId = driver.user_id,
					               		IsPrimaryStation = driver.isPrimaryStation,
					               		Stations = driver.stations
					               	});
				}
			}
		}

		private bool IsThisPrimaryStation(IEnumerable<UserStation> stations)
		{
			if (stations == null)
				return false;

			foreach (UserStation s in stations)
				if (s.station_id == stationId)
				{
					if (string.Compare(s.type, "primary", true) == 0)
						return true;
					return false;
				}

			return false;
		}

		private void OnDriverAdded(DriverAddedEvtArgs args)
		{
			EventHandler<DriverAddedEvtArgs> handler = DriverAdded;

			if (handler != null)
				handler(this, args);
		}

		private void OnBeforeDriverSaved(BeforeDriverSavedEvtArgs args)
		{
			EventHandler<BeforeDriverSavedEvtArgs> handler = BeforeDriverSaved;

			if (handler != null)
				handler(this, args);
		}

		public override object Clone()
		{
			return MemberwiseClone();
		}
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
		public DriverAddedEvtArgs(Driver driver)
		{
			Driver = driver;
		}

		public Driver Driver { get; private set; }
	}

	public class BeforeDriverSavedEvtArgs : EventArgs
	{
		public BeforeDriverSavedEvtArgs(Driver driver)
		{
			Driver = driver;
		}

		public Driver Driver { get; private set; }
	}
}