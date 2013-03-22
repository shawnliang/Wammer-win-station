using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.Utility;
using Waveface.Stream.Core;
using Waveface.Stream.Model;

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
		private bool IsThisPrimaryStation(string stationId, IEnumerable<UserStation> stations)
		{
			if (stations == null)
				return false;

			return (from s in stations
					where s.station_id == stationId
					select string.Compare(s.type, "primary", true) == 0).FirstOrDefault();
		}
		#endregion

		#region Public Property
		public IBillingPlanChecker PlanChecker { get; set; }
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
		/// <param name="preferredFolderPath">The resource base path.</param>
		/// <param name="stationId">The station id.</param>
		/// <param name="userID">The user ID.</param>
		/// <param name="sessionToken">The session token.</param>
		/// <returns></returns>
		public AddUserResponse AddDriver(string preferredFolderPath, string stationId, string userID, string sessionToken)
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

			var actualFolderPath = chooseFolderPath(user.user_id, preferredFolderPath);

			var driver = new Driver
							{
								user_id = user.user_id,
								email = user.email,
								groups = res.groups,
								folder = actualFolderPath,
								session_token = res.session_token,
								isPrimaryStation = IsThisPrimaryStation(stationId, res.stations),
								ref_count = 1,
								user = user,
								stations = res.stations,
								sync_range = new SyncRange(),
								isPaidUser = PlanChecker.IsPaidUser(user.user_id, res.session_token)
							};

			var beforeAddArgs = new BeforeDriverSavedEvtArgs(driver);
			OnBeforeDriverSaved(beforeAddArgs);

			DriverCollection.Instance.Save(driver);
			StorageRegistry.Save(user.user_id, actualFolderPath);
			OnDriverAdded(new DriverAddedEvtArgs(driver, beforeAddArgs.UserData));

			return new AddUserResponse
					{
						UserId = driver.user_id,
						IsPrimaryStation = driver.isPrimaryStation,
						Stations = driver.stations
					};
		}

		private string chooseFolderPath(string user_id, string preferredFolderPath)
		{
			var oldFolder = StorageRegistry.QueryStorageLocation(user_id);
			if (!string.IsNullOrEmpty(oldFolder) && Directory.Exists(oldFolder))
				return oldFolder;

			if (!Directory.Exists(preferredFolderPath))
				Directory.CreateDirectory(preferredFolderPath);

			return preferredFolderPath;
		}


		/// <summary>
		/// Adds the driver.
		/// </summary>
		/// <param name="preferredFolderPath">The resource base path.</param>
		/// <param name="stationId">The station id.</param>
		/// <param name="email">The email.</param>
		/// <param name="password">The password.</param>
		/// <param name="deviceID">The device ID.</param>
		/// <param name="deviceName">Name of the device.</param>
		/// <returns></returns>
		public AddUserResponse AddDriver(string preferredFolderPath, string stationId, string email, string password, string deviceID, string deviceName)
		{
			var existingDriver = DriverCollection.Instance.FindOne(Query.EQ("email", email));

			if (existingDriver != null)
			{
				// check if this email is re-registered, if yes, delete old driver's data
				var user = User.LogIn(email, password, deviceID, deviceName);

				if (user == null)
					throw new WammerStationException("Logined user not found", (int)StationLocalApiError.AuthFailed);

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

			var actualFolderPath = chooseFolderPath(res.user.user_id, preferredFolderPath);

			var driver = new Driver
							{
								user_id = res.user.user_id,
								email = email,
								folder = actualFolderPath,
								groups = res.groups,
								session_token = res.session_token,
								isPrimaryStation = IsThisPrimaryStation(stationId, res.stations),
								ref_count = 1,
								user = res.user,
								stations = res.stations,
								sync_range = new SyncRange(),
								isPaidUser = PlanChecker.IsPaidUser(res.user.user_id, res.session_token)
							};


			var beforeSaveArgs = new BeforeDriverSavedEvtArgs(driver);
			OnBeforeDriverSaved(beforeSaveArgs);

			DriverCollection.Instance.Save(driver);
			StorageRegistry.Save(res.user.user_id, actualFolderPath);

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
					var userCacheFolder = FileStorage.GetCachePath(userID);

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
			StorageRegistry.Remove(userID);

			//Remove login session if existed
			if (!string.IsNullOrEmpty(existingDriver.session_token))
				LoginedSessionCollection.Instance.Remove(Query.EQ("_id", existingDriver.session_token));

			AttachmentCollection.Instance.Remove(Query.EQ("group_id", existingDriver.groups[0].group_id));
			PostDBDataCollection.Instance.Remove(Query.EQ("creator_id", userID));
			MonitorItemCollection.Instance.Remove(Query.EQ("user_id", userID));
			CollectionCollection.Instance.Remove(Query.EQ("creator_id", userID));
			TaskStatusCollection.Instance.Remove(Query.EQ("UserId", userID));

			//All driver removed => Remove station from db
			Driver driver = DriverCollection.Instance.FindOne();
			if (driver == null)
				StationCollection.Instance.RemoveAll();
		}

		#endregion

		public DriverController()
		{
			PlanChecker = new BillingPlanChecker();
		}
	}


	public interface IBillingPlanChecker
	{
		bool IsPaidUser(string user_id, string session_token);
	}

	class BillingPlanChecker : IBillingPlanChecker
	{
		public bool IsPaidUser(string user_id, string session_token)
		{
			var userInfo = User.GetInfo(user_id, CloudServer.APIKey, session_token);
			return userInfo.billing.type.Equals("paid", StringComparison.InvariantCultureIgnoreCase);
		}
	}
}
