using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.Utility;

namespace Wammer.Station
{
	public class DriverController
	{
		#region Event
		public event EventHandler<DriverAddedEvtArgs> DriverAdded;
		public event EventHandler<BeforeDriverSavedEvtArgs> BeforeDriverSaved;
		#endregion

		#region Private Method
		/// <summary>
		/// Determines whether [is this primary station] [the specified stations].
		/// </summary>
		/// <param name="stationId">The station id.</param>
		/// <param name="stations">The stations.</param>
		/// <returns>
		/// 	<c>true</c> if [is this primary station] [the specified stations]; otherwise, <c>false</c>.
		/// </returns>
		private bool IsThisPrimaryStation(string stationId,IEnumerable<UserStation> stations)
		{
			if (stations == null)
				return false;

			return (from s in stations
					where s.station_id == stationId
					select string.Compare(s.type, "primary", true) == 0).FirstOrDefault();
		}
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
		/// Adds the driver.
		/// </summary>
		/// <param name="resourceBasePath">The resource base path.</param>
		/// <param name="stationId">The station id.</param>
		/// <param name="userID">The user ID.</param>
		/// <param name="sessionToken">The session token.</param>
		/// <returns></returns>
		public AddUserResponse AddDriver(string resourceBasePath, string stationId, string userID, string sessionToken)
		{
			var existingDriver = DriverCollection.Instance.FindOne(Query.EQ("_id", userID));
			if (existingDriver != null)
			{
				existingDriver.ref_count += 1;
				DriverCollection.Instance.Update(Query.EQ("_id", userID), Update.Set("ref_count", existingDriver.ref_count));
				return new AddUserResponse
						{
							UserId = existingDriver.user_id,
							IsPrimaryStation = existingDriver.isPrimaryStation
						};
			}

			var res = StationApi.SignUpBySession(sessionToken, stationId, StatusChecker.GetDetail());
			StationCollection.Instance.Update(
				Query.EQ("_id", stationId),
				Update.Set("SessionToken", res.session_token)
					.Set("Location", NetworkHelper.GetBaseURL())
					.Set("LastLogOn", DateTime.Now),
				UpdateFlags.Upsert
				);

			var user = res.user;

			//Remove residual driver
			DriverCollection.Instance.Remove(Query.EQ("email", user.email));
			
			var driver = new Driver
							{
								user_id = user.user_id,
								email = user.email,
								groups = res.groups,
								folder = Path.Combine(resourceBasePath, "user_" + user.user_id),
								session_token = res.session_token,
								isPrimaryStation = IsThisPrimaryStation(stationId,res.stations),
								ref_count = 1,
								user = user,
								stations = res.stations
							};

			CreateUserFolder(driver);

			var beforeAddArgs = new BeforeDriverSavedEvtArgs(driver);
			OnBeforeDriverSaved(beforeAddArgs);

			DriverCollection.Instance.Save(driver);

			OnDriverAdded(new DriverAddedEvtArgs(driver, beforeAddArgs.UserData));

			return new AddUserResponse
					{
						UserId = driver.user_id,
						IsPrimaryStation = driver.isPrimaryStation,
						Stations = driver.stations
					};
		}

		private static void CreateUserFolder(Driver driver)
		{
			// FileStorage constructor creates user's folder.
			new FileStorage(driver);
		}

		/// <summary>
		/// Adds the driver.
		/// </summary>
		/// <param name="resourceBasePath">The resource base path.</param>
		/// <param name="stationId">The station id.</param>
		/// <param name="email">The email.</param>
		/// <param name="password">The password.</param>
		/// <param name="deviceID">The device ID.</param>
		/// <param name="deviceName">Name of the device.</param>
		/// <returns></returns>
		public AddUserResponse AddDriver(string resourceBasePath, string stationId, string email, string password, string deviceID, string deviceName)
		{
			var existingDriver = DriverCollection.Instance.FindOne(Query.EQ("email", email));

			if (existingDriver != null)
			{
				// check if this email is re-registered, if yes, delete old driver's data
				var user = User.LogIn(email, password, deviceID, deviceName);

				if (user == null)
					throw new WammerStationException("Logined user not found", (int) StationLocalApiError.AuthFailed);

				if (user.Id == existingDriver.user_id)
				{
					existingDriver.ref_count += 1;
					DriverCollection.Instance.Save(existingDriver);

					return new AddUserResponse
					       	{
					       		UserId = existingDriver.user_id,
					       		IsPrimaryStation = existingDriver.isPrimaryStation,
					       		Stations = existingDriver.stations
					       	};
				}

				//Remove the user from db, and stop service this user
				DriverCollection.Instance.Remove(Query.EQ("_id", existingDriver.user_id));

				if (Directory.Exists(existingDriver.folder))
					Directory.Delete(existingDriver.folder, true);

				//All driver removed => Remove station from db
				if (DriverCollection.Instance.FindOne() == null)
					StationCollection.Instance.RemoveAll();
			}

			var res = StationApi.SignUpByEmailPassword(stationId, email, password, deviceID,
			                                           deviceName, StatusChecker.GetDetail());
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
			             		isPrimaryStation = IsThisPrimaryStation(stationId, res.stations),
			             		ref_count = 1,
			             		user = res.user,
			             		stations = res.stations
			             	};

			CreateUserFolder(driver);

			var beforeSaveArgs = new BeforeDriverSavedEvtArgs(driver);
			OnBeforeDriverSaved(beforeSaveArgs);

			DriverCollection.Instance.Save(driver);


			OnDriverAdded(new DriverAddedEvtArgs(driver, beforeSaveArgs.UserData));

			return new AddUserResponse
			       	{
			       		UserId = driver.user_id,
			       		IsPrimaryStation = driver.isPrimaryStation,
			       		Stations = driver.stations
			       	};

		}

		public void RemoveDriver(string stationID, string userID, Boolean removeAllData = false)
		{
			if (string.IsNullOrEmpty(userID))
				throw new ArgumentNullException("userID");

			//Try to find existing driver
			var existingDriver = DriverCollection.Instance.FindOne(Query.EQ("_id", userID));
			var isDriverExists = existingDriver != null;

			//Driver not exists => return
			if (!isDriverExists)
				return;

			//reference count > 1 => reference count decrease one
			if (existingDriver.ref_count > 1)
			{
				--existingDriver.ref_count;
				DriverCollection.Instance.Save(existingDriver);
				return;
			}

			//Notify cloud server that the user signoff
			try
			{
				StationApi.SignOff(stationID, existingDriver.session_token, userID);
			}
			catch (WammerCloudException e)
			{
				this.LogWarnMsg(string.Format("Unable to notify cloud to unlink user {0} from this computer", userID), e);

				// continue removing user even if session expired
				if (e.WammerError != (int)GeneralApiError.SessionNotExist)
					throw;
			}


			//Remove all user data
			if (removeAllData)
			{
				var retryTimes = 0;
				var basePath = (new FileStorage(existingDriver)).basePath;

				while (retryTimes++ < 3)
				{
					if (Directory.Exists(basePath))
					{
						try
						{
							Directory.Delete(basePath, true);
							break;
						}
						catch
						{
						}
					}
				}

				retryTimes = 0;
				while (retryTimes++ < 3)
				{
					var userCacheFolder = Path.Combine("cache", userID);

					try
					{
						if (Directory.Exists(userCacheFolder))
							Directory.Delete(userCacheFolder, true);

						break;
					}
					catch
					{
					}
				}
			}

			//Remove the user from db, and stop service this user
			DriverCollection.Instance.Remove(Query.EQ("_id", userID));

			//Remove login session if existed
			LoginedSessionCollection.Instance.Remove(Query.EQ("_id", existingDriver.session_token));

			foreach (var post in PostCollection.Instance.Find(Query.EQ("creator_id", userID)))
			{
				foreach (var attachmentId in post.attachment_id_array)
				{
					AttachmentCollection.Instance.Remove(Query.EQ("_id", attachmentId));
				}
			}
			PostCollection.Instance.Remove(Query.EQ("creator_id", userID));

			//All driver removed => Remove station from db
			Driver driver = DriverCollection.Instance.FindOne();
			if (driver == null)
				StationCollection.Instance.RemoveAll();
		}

		#endregion
	}
}
