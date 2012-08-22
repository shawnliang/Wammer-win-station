using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.ServiceProcess;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Runtime;
using System.Diagnostics;
using System.Reflection;
using System.Net;

using Wammer.Station.Service;
using Wammer.Utility;
using Wammer.Cloud;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System.Net.NetworkInformation;

namespace Wammer.Station.Management
{
	public class StationController
	{
		private static ServiceController scvCtrl;

		static StationController()
		{
			scvCtrl = new ServiceController(StationService.SERVICE_NAME);
			StationMgmtURL = "http://127.0.0.1:9989/v2/";
			StationFuncURL = "http://127.0.0.1:9981/v2/";
		}

		/// <summary>
		/// Gets station service status
		/// </summary>
		public static System.ServiceProcess.ServiceControllerStatus ServiceStatus
		{
			get { return scvCtrl.Status; }
		}

		/// <summary>
		/// Gets if auto-start of station service is enabled
		/// </summary>
		/// <returns></returns>
		public static bool IsServiceAutoStartEnabled()
		{
			return ServiceHelper.IsServiceAutoStart(scvCtrl);
		}

		/// <summary>
		/// Gets owner's email
		/// </summary>
		/// <returns>Owner's email. If owner is not set yet, null is returned.</returns>
		public static string GetOwner()
		{
			Model.Driver driver = Model.DriverCollection.Instance.FindOne();
			if (driver == null)
				return null;

			return driver.email;
		}

		/// <summary>
		/// Gets default folder to save attachments
		/// </summary>
		/// <remarks>If the owner of station is not set yet, null is returned.</remarks>
		/// <returns></returns>
		public static string GetDefaultFolder()
		{
			Model.Driver driver = Model.DriverCollection.Instance.FindOne();
			if (driver == null)
				return null;

			return driver.folder;
		}

		/// <summary>
		/// Add a user to a station
		/// </summary>
		/// <param name="email">The email.</param>
		/// <param name="password">The password.</param>
		/// <param name="deviceId">The device id.</param>
		/// <param name="deviceName">Name of the device.</param>
		/// <returns>station's session token</returns>
		/// <exception cref="Wammer.Station.Management.AuthenticationException">
		/// Invalid user name or password
		/// </exception>
		/// <exception cref="Wammer.Station.Management.StationAlreadyHasDriverException">
		/// The station already has an driver
		/// </exception>
		/// <exception cref="Wammer.Station.Management.UserAlreadyHasStationException">
		/// The user already has a station. The station's info, such as id/location/sync time, can
		/// be retrieved from the exception
		/// </exception>
		/// <exception cref="Wammer.Station.Management.StationServiceDownException">
		/// Unable to connect to station service, service down?
		/// </exception>
		/// <exception cref="Wammer.Station.Management.ConnectToCloudException">
		/// Unable to connect to waveface cloud, network down?
		/// </exception>
		public static AddUserResponse AddUser(string email, string password, string deviceId, string deviceName)
		{
			try
			{
				AddUserResponse res = CloudServer.request<AddUserResponse>(
					StationMgmtURL + "station/drivers/add",
					new Dictionary<object, object>{
						{ "email", email},
						{ "password", password},
						{ "device_id", deviceId},
						{ "device_name", deviceName}
					},
					false
				);

				return res;
			}
			catch (Cloud.WammerCloudException e)
			{
				throw ExtractApiRetMsg(e);
			}
		}

		public static AddUserResponse AddUser(string userId, string sessionToken)
		{
			try
			{
				AddUserResponse res = CloudServer.request<AddUserResponse>(
					StationMgmtURL + "station/drivers/add",
					new Dictionary<object, object>{
						{ "user_id", userId},
						{ "session_token", sessionToken}
					},
					false
				);

				return res;
			}
			catch (Cloud.WammerCloudException e)
			{
				throw ExtractApiRetMsg(e);
			}
		}

		/// <summary>
		/// login to station
		/// </summary>
		/// <param name="email"></param>
		/// <param name="password"></param>
		/// <param name="deviceId"></param>
		/// <param name="deviceName"></param>
		/// <returns>user's login info</returns>
		public static UserLogInResponse UserLogin(string apikey, string email, string password, string deviceId, string deviceName)
		{
			try
			{
				return CloudServer.request<UserLogInResponse>(
					StationFuncURL + "auth/login",
					new Dictionary<object, object> { 
						{CloudServer.PARAM_API_KEY, apikey},
						{CloudServer.PARAM_EMAIL, email},
						{CloudServer.PARAM_PASSWORD, password},
						{CloudServer.PARAM_DEVICE_ID, deviceId},
						{CloudServer.PARAM_DEVICE_NAME, deviceName}
					},
					false
				);
			}
			catch (WammerCloudException e)
			{
				throw ExtractApiRetMsg(e);
			}
		}

		/// <summary>
		/// login to station
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="sessionToken"></param>
		/// <returns>user's login info</returns>
		public static UserLogInResponse UserLogin(string apikey, string userId, string sessionToken)
		{
			try
			{
				return CloudServer.request<UserLogInResponse>(
					StationFuncURL + "auth/login",
					new Dictionary<object, object> { 
						{CloudServer.PARAM_API_KEY, apikey},
						{CloudServer.PARAM_USER_ID, userId},
						{CloudServer.PARAM_SESSION_TOKEN, sessionToken}
					},
					false
				);
			}
			catch (WammerCloudException e)
			{
				throw ExtractApiRetMsg(e);
			}
		}

		/// <summary>
		/// logout from station
		/// </summary>
		/// <param name="sessionToken"></param>
		public static void UserLogout(string apikey, string sessionToken)
		{
			try
			{
				CloudServer.request<CloudResponse>(
					StationFuncURL + "auth/logout",
					new Dictionary<object, object> { 
						{CloudServer.PARAM_API_KEY, apikey},
						{CloudServer.PARAM_SESSION_TOKEN, sessionToken}
					},
					false
				);
			}
			catch (WammerCloudException e)
			{
				throw ExtractApiRetMsg(e);
			}
		}


		/// <summary>
		/// Sign off a station on behavior of its driver
		/// </summary>
		/// <param name="stationId"></param>
		/// <param name="driverEmail"></param>
		/// <param name="password"></param>
		public static void SignoffStation(string stationId, string driverEmail, string password)
		{
			var user = User.LogIn(driverEmail, password, stationId, Environment.MachineName);
			StationApi.SignOff(stationId, user.Token, user.Id);
		}

		/// <summary>
		/// List all detected cloud storages
		/// </summary>
		public static List<StorageStatus> DetectCloudStorage()
		{
			List<StorageStatus> cloudstorages = new List<StorageStatus>();

			if (DropboxHelper.IsInstalled())
			{
				// currently only support one driver
				Model.Driver driver = Model.DriverCollection.Instance.FindOne();
				Model.CloudStorage cloudstorage = Model.CloudStorageCollection.Instance.FindOne(Query.EQ("Type", "dropbox"));
				if (cloudstorage != null)
				{
					cloudstorages.Add(new StorageStatus
						{
							type = "dropbox",
							connected = true,
							quota = cloudstorage.Quota,
							used = new DropboxFileStorage(driver, cloudstorage).GetUsedSize()
						}
					);
				}
				else
				{
					cloudstorages.Add(new StorageStatus
						{
							type = "dropbox",
							connected = false
						}
					);
				}
			}

			return cloudstorages;
		}

		/// <summary>
		/// Get Dropbox OAuth URL 
		/// </summary>
		/// <exception cref="Wammer.Station.Management.GetDropboxOAuthException">
		/// Unable to get Dropbox OAuth information
		/// </exception>
		public static string GetDropboxOAuthUrl()
		{
			try
			{
				GetDropboxOAuthResponse res = CloudServer.request<GetDropboxOAuthResponse>(
					StationMgmtURL + "cloudstorage/dropbox/oauth",
					new Dictionary<object, object>(),
					true,
					false
				);
				return res.oauth_url;
			}
			catch (Cloud.WammerCloudException e)
			{
				throw ExtractApiRetMsg(e);
			}
		}

		/// <summary>
		/// Connect Waveface to Dropbox
		/// </summary>
		/// <param name="quota"></param>
		/// <exception cref="Wammer.Station.Management.DropboxNoSyncFolderException">
		/// Dropbox sync folder does not exist
		/// </exception>
		/// <exception cref="Wammer.Station.Management.WrongAccountException">
		/// Link to inconsistent Dropbox account
		/// </exception>
		/// <exception cref="Wammer.Station.Management.DropboxNotInstalledException">
		/// Dropbox is not installed
		/// </exception>
		public static void ConnectDropbox(long quota)
		{
			try
			{
				if (!DropboxHelper.IsInstalled())
					throw new DropboxNotInstalledException("Dropbox is not installed");

				string folder = DropboxHelper.GetSyncFolder();
				CloudResponse res = CloudServer.request<CloudResponse>(
					StationMgmtURL + "cloudstorage/dropbox/connect",
					new Dictionary<object, object> { { "quota", quota }, { "folder", folder } },
					true,
					false
				);
			}
			catch (Cloud.WammerCloudException e)
			{
				throw ExtractApiRetMsg(e);
			}
		}

		/// <summary>
		/// Update Dropbox quota for Waveface
		/// </summary>
		/// <param name="quota"></param>
		/// <exception cref="Wammer.Station.Management.DropboxNotConnectedException">
		/// Waveface has not connected to Dropbox
		/// </exception>
		/// <exception cref="Wammer.Station.Management.DropboxNotInstalledException">
		/// Dropbox is not installed
		/// </exception>
		public static void UpdateDropbox(long quota)
		{
			try
			{
				if (!DropboxHelper.IsInstalled())
					throw new DropboxNotInstalledException("Dropbox is not installed");
					
				CloudResponse res = CloudServer.request<CloudResponse>(
					StationMgmtURL + "cloudstorage/dropbox/update",
					new Dictionary<object, object> { { "quota", quota } },
					true,
					false
				);
			}
			catch (Cloud.WammerCloudException e)
			{
				throw ExtractApiRetMsg(e);
			}
		}

		/// <summary>
		/// Disconnect Waveface from Dropbox
		/// </summary>
		public static void DisconnectDropbox()
		{
			try
			{
				CloudResponse res = CloudServer.request<CloudResponse>(
					StationMgmtURL + "cloudstorage/dropbox/disconnect",
					new Dictionary<object, object>(),
					true,
					false
				);
			}
			catch (Cloud.WammerCloudException e)
			{
				throw ExtractApiRetMsg(e);
			}
		}

		public static string StationMgmtURL { get; set; }
		public static string StationFuncURL { get; set; }

		public static void ResumeSync()
		{
			try
			{
				CloudServer.request<CloudResponse>(
					StationMgmtURL + "station/resumeSync",
					new Dictionary<object, object>
					{
						{CloudServer.PARAM_API_KEY, CloudServer.APIKey}
					},
					false
				);

			}
			catch (WammerCloudException e)
			{
				throw ExtractApiRetMsg(e);
			}
		}


		public static void SuspendSync(int timeout)
		{
			try
			{
				CloudServer.request<CloudResponse>(
					StationMgmtURL + "station/suspendSync",
					new Dictionary<object, object>
						{
							{CloudServer.PARAM_API_KEY, CloudServer.APIKey},
						},
					false, true, timeout);
			}
			catch (WammerCloudException e)
			{
				throw ExtractApiRetMsg(e);
			}
		}

		public static void RemoveOwner(string userId, Boolean removeResource = true)
		{
			try
			{
				CloudServer.request<CloudResponse>(
					StationMgmtURL + "station/drivers/remove",
					new Dictionary<object, object>
					{
						{CloudServer.PARAM_API_KEY, CloudServer.APIKey},
						{CloudServer.PARAM_USER_ID, userId},
						{"remove_resource", removeResource}
					},
					false
				);
			}
			catch (WammerCloudException e)
			{
				throw ExtractApiRetMsg(e);
			}
		}


		/// <summary>
		/// Request Waveface Cloud to check station accessibility from the internet.
		/// </summary>
		/// <param name="sessionToken"></param>
		public static void PingMyStation(string sessionToken)
		{
			try
			{
				CloudServer.requestPath<CloudResponse>(
					"users/pingMyStation",
					new Dictionary<object, object> { 
						{CloudServer.PARAM_API_KEY, CloudServer.APIKey},
						{CloudServer.PARAM_SESSION_TOKEN, sessionToken}
					},
					false
				);
			}
			catch (WammerCloudException e)
			{
				throw ExtractApiRetMsg(e);
			}
		}

		[DllImport("wininet")]
		public static extern bool InternetGetConnectedState(
			ref uint lpdwFlags,
			uint dwReserved
			);
		public static void ConnectToInternet()
		{


			//連線的Flag
			uint flags = 0x0;

			bool rtvl;

			//取得本機電腦目前的連線狀態
			rtvl = InternetGetConnectedState(ref flags, 0);

			if(!rtvl)
				throw new WebException();

			//var p = new Ping();
			//if (p.Send("www.google.com", 500).Status != IPStatus.Success)
			//    throw new WebException();

			//using (var agent = new DefaultWebClient())
			//{
			//    agent.DownloadData("http://www.google.com");
			//}
		}

		public static void PingForAvailability()
		{
			try
			{
				CloudServer.request<CloudResponse>(StationFuncURL + "availability/ping/", new Dictionary<object, object>(), true, false);
			}
			catch (WammerCloudException e)
			{
				// service is still alive though it returns an error
				if (e.HttpError == WebExceptionStatus.ProtocolError)
					return;

				throw ExtractApiRetMsg(e);

			}
		}

		public static void PingForServiceAlive()
		{
			try
			{
				CloudServer.request<CloudResponse>(StationMgmtURL + "availability/ping/", new Dictionary<object, object>(), true, false);
			}
			catch (WammerCloudException e)
			{
				// service is still alive though it returns an error
				if (e.HttpError == WebExceptionStatus.ProtocolError)
					return;

				throw ExtractApiRetMsg(e);
			}
		}

		public static GetUserResponse GetUser(string sessionToken, string userId)
		{
			try
			{
				GetUserResponse res = CloudServer.requestPath<GetUserResponse>(
					"users/get",
					new Dictionary<object, object> { 
						{CloudServer.PARAM_API_KEY, CloudServer.APIKey},
						{CloudServer.PARAM_SESSION_TOKEN, sessionToken},
						{"user_id", userId}
					},
					false
				);

				return res;
			}
			catch (WammerCloudException e)
			{
				throw ExtractApiRetMsg(e);
			}
		}

		public static StorageUsageResponse StorageUsage(string sessionToken)
		{
			try
			{
				// TODO: call the api via station
				StorageUsageResponse res = CloudServer.requestPath<StorageUsageResponse>(
					"storages/usage",
					new Dictionary<object, object> { 
						{CloudServer.PARAM_API_KEY, CloudServer.APIKey},
						{CloudServer.PARAM_SESSION_TOKEN, sessionToken}
					},
					false
				);

				return res;
			}
			catch (WammerCloudException e)
			{
				throw ExtractApiRetMsg(e);
			}			
		}

		public static ListCloudStorageResponse ListCloudStorage()
		{
			try
			{
				ListCloudStorageResponse res = CloudServer.request<ListCloudStorageResponse>(
					StationMgmtURL + "cloudstorage/list",
					new Dictionary<object, object>
					{
						{CloudServer.PARAM_API_KEY, CloudServer.APIKey},
					},
					false
				);

				return res;
			}
			catch (WammerCloudException e)
			{
				throw ExtractApiRetMsg(e);
			}
		}

		public static GetStatusResponse GetStationStatus()
		{
			try
			{
				GetStatusResponse res = CloudServer.request<GetStatusResponse>(
					StationMgmtURL + "station/status/get",
					new Dictionary<object, object>
					{
						{CloudServer.PARAM_API_KEY, CloudServer.APIKey},
					},
					false
				);

				return res;
			}
			catch (WammerCloudException e)
			{
				throw ExtractApiRetMsg(e);
			}
		}

		public static void MoveResourceFolder(string newLocation)
		{
			CloudServer.request<CloudResponse>(
				StationMgmtURL + "station/resource_folder/move",
				new Dictionary<object, object>
				{
					{ "folder", newLocation}
				},
				false);
		}

		public static ListDriverResponse ListUser()
		{
			try
			{
				ListDriverResponse res = CloudServer.request<ListDriverResponse>(
					StationMgmtURL + "station/drivers/list",
					new Dictionary<object, object>
					{
						{CloudServer.PARAM_API_KEY, CloudServer.APIKey},
					},
					false
				);

				return res;
			}
			catch (WammerCloudException e)
			{
				throw ExtractApiRetMsg(e);
			}
		}

		public static Exception ExtractApiRetMsg(Cloud.WammerCloudException e)
		{
			if (e.HttpError != WebExceptionStatus.ProtocolError)
			{
#if !DEBUG
				if (ServiceStatus != ServiceControllerStatus.Running)
				{
					return new StationServiceDownException("Station service down");
				}
#endif
				if (e.InnerException != null)
				{
					return new ConnectToCloudException(e.InnerException.Message);
				}
				else
				{
					return new ConnectToCloudException(e.Message);
				}
			}

			WebException webex = (WebException)e.InnerException;
			if (webex != null)
			{
				HttpWebResponse webres = (HttpWebResponse)webex.Response;
				if (webres != null)
				{
					if (webres.StatusCode == HttpStatusCode.BadRequest)
					{
						string resText = e.response;
						Cloud.CloudResponse r = fastJSON.JSON.Instance.ToObject<Cloud.CloudResponse>(resText);

						switch (e.WammerError)
						{
							case (int)AuthApiError.InvalidEmailPassword:
								return new AuthenticationException(r.api_ret_message);
							case (int)StationApiError.AlreadyRegisteredAnotherStation:
								return new UserAlreadyHasStationException(r.api_ret_message);
							case (int)StationLocalApiError.InvalidDriver:
								return new InvalidDriverException(r.api_ret_message);
							case (int)StationLocalApiError.ConnectToCloudError:
								return new ConnectToCloudException(r.api_ret_message);
							case (int)GeneralApiError.NotSupportClient:
								return new VersionNotSupportedException(r.api_ret_message);
							case (int)StationLocalApiError.AuthFailed:
								throw new AuthenticationException(e.Message);
							case (int)StationLocalApiError.DriverExist:
								throw new StationAlreadyHasDriverException(e.Message);
							case (int)StationApiError.UserNotExist:
								throw new AuthenticationException(r.api_ret_message);
							default:
								return new Exception(r.api_ret_message);
						}
					}
					else if (webres.StatusCode == HttpStatusCode.Unauthorized)
					{
						return new AuthenticationException("Authentication failure");
					}
					else if (webres.StatusCode == HttpStatusCode.ServiceUnavailable)
					{
						return new UserAlreadyHasStationException("Driver already registered another station");
					}
				}
				else
				{
					return new Exception(webex.Message);
				}
			}

			return e;
		}
		#region private accessors

		/// <summary>
		/// Starts station services, including MongoDB service
		/// </summary>
		/// <param name="timeout">timeout value</param>
		/// <exception cref="System.TimeoutException"></exception>
		private static void StartServices(TimeSpan timeout)
		{
			for (int i = 0; i < 3; i++)
			{
				scvCtrl.Refresh();

				if (scvCtrl.Status != ServiceControllerStatus.Running &&
					scvCtrl.Status != ServiceControllerStatus.StartPending)
				{
					scvCtrl.Start();

					try
					{
						scvCtrl.WaitForStatus(ServiceControllerStatus.Running, timeout);
						return;
					}
					catch (System.ServiceProcess.TimeoutException)
					{
					}
				}
			}
		}

		/// <summary>
		/// Stops station services, including MongoDB service
		/// </summary>
		private static void StopServices(TimeSpan timeout)
		{
			for (int i = 0; i < 3; i++)
			{
				scvCtrl.Refresh();

				if (scvCtrl.Status != ServiceControllerStatus.Stopped &&
					scvCtrl.Status != ServiceControllerStatus.StopPending)
				{
					scvCtrl.Stop();
					try
					{
						scvCtrl.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
						return;
					}
					catch (System.ServiceProcess.TimeoutException)
					{
					}
				}
			}
		}

		/// <summary>
		/// Sets whether station service auto starts itself after system boots
		/// </summary>
		/// <param name="autoStart"></param>
		private static void SetServiceAutoStart(bool autoStart)
		{
			ServiceHelper.ChangeStartMode(scvCtrl, ServiceStartMode.Automatic);
		}

		

		private static void _MoveDefaultFolder(string newFolder)
		{
			if (!Path.IsPathRooted(newFolder))
				throw new ArgumentException("Not an absolute path");

			StopServices(TimeSpan.FromSeconds(20.0));
			string oldFolder = FileStorage.ResourceFolder;

			foreach (var user in Model.DriverCollection.Instance.FindAll())
			{
				var userFolder = Path.Combine(newFolder, user.folder);
				if (File.Exists(userFolder) ||
					Directory.Exists(userFolder))
					throw new Exception("Folder already exists: " + userFolder);
			}

			bool sameDrive = Path.GetPathRoot(oldFolder).Equals(Path.GetPathRoot(newFolder));

			foreach (var user in Model.DriverCollection.Instance.FindAll())
			{
				if (sameDrive)
				{
					Directory.Move(
						Path.Combine(oldFolder, user.folder),
						Path.Combine(newFolder, user.folder));
				}
				else
				{
					DirectoryCopy(
						Path.Combine(oldFolder, user.folder),
						Path.Combine(newFolder, user.folder),
						true);
				}
			}

			if (!sameDrive)
				foreach (var user in Model.DriverCollection.Instance.FindAll())
					Directory.Delete(Path.Combine(oldFolder, user.folder), true);

			FileStorage.ResourceFolder = newFolder;

			StartServices(TimeSpan.FromSeconds(20.0));
		}

		private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
		{
			DirectoryInfo dir = new DirectoryInfo(sourceDirName);
			DirectoryInfo[] dirs = dir.GetDirectories();

			// If the source directory does not exist, throw an exception.
			if (!dir.Exists)
			{
				throw new DirectoryNotFoundException(
					"Source directory does not exist or could not be found: "
					+ sourceDirName);
			}

			// If the destination directory does not exist, create it.
			if (!Directory.Exists(destDirName))
			{
				Directory.CreateDirectory(destDirName);
			}


			// Get the file contents of the directory to copy.
			FileInfo[] files = dir.GetFiles();

			foreach (FileInfo file in files)
			{
				// Create the path to the new copy of the file.
				string temppath = Path.Combine(destDirName, file.Name);

				// Copy the file.
				file.CopyTo(temppath, false);
			}

			// If copySubDirs is true, copy the subdirectories.
			if (copySubDirs)
			{

				foreach (DirectoryInfo subdir in dirs)
				{
					// Create the subdirectory.
					string temppath = Path.Combine(destDirName, subdir.Name);

					// Copy the subdirectories.
					DirectoryCopy(subdir.FullName, temppath, copySubDirs);
				}
			}
		}

		private static void SetDefaultFolder(string absPath)
		{
			Model.Driver driver = Model.DriverCollection.Instance.FindOne();
			if (driver == null)
				throw new InvalidOperationException("Cannot set default folder before driver is not set yet.");

			driver.folder = absPath;
			Model.DriverCollection.Instance.Save(driver);
		}
		#endregion


		public static int Main(string[] args)
		{
			string output = "";

			try
			{
				
				string moveDestination = "";

				for (int i = 0; i < args.Length; i++)
				{
					switch (args[i])
					{
						case "--moveFolder":
							moveDestination = args[++i];
							break;
						case "--output":
							output = args[++i];
							break;
						default:
							Console.WriteLine("Unknown parameter: " + args[i]);
							break;
					}
				}

				if (string.IsNullOrEmpty(moveDestination))
					throw new ArgumentException("usage: --moveFolder path --output file");

				_MoveDefaultFolder(moveDestination);
				return 0;
			}
			catch (Exception e)
			{
				if (string.IsNullOrEmpty(output))
					Console.WriteLine(e.Message);
				else
				{
					using (StreamWriter w = File.CreateText(output))
					{
						w.WriteLine(e.Message);
					}
				}

				StartServices(TimeSpan.FromSeconds(20.0));

				return 1;
			}
		}
	}

	public class AuthenticationException: Exception
	{
		public AuthenticationException(string msg)
			:base(msg)
		{
		}
	}

	public class StationAlreadyHasDriverException: Exception
	{
		public StationAlreadyHasDriverException(string msg)
			:base(msg)
		{
		}
	}

	public class StationServiceDownException: Exception
	{
		public StationServiceDownException(string msg)
			:base(msg)
		{
		}
	}

	public class ConnectToCloudException: Exception
	{
		public ConnectToCloudException(string msg)
			:base(msg)
		{
		}
	}

	public class VersionNotSupportedException: Exception
	{
		public VersionNotSupportedException(string msg)
			: base(msg)
		{
		}
	}

	public class UserAlreadyHasStationException: Exception
	{
		/// <summary>
		/// Existing station's id
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// Existing station's url
		/// </summary>
		public string Location { get; set; }

		/// <summary>
		/// Existing station's id
		/// </summary>
		public DateTime LastSyncTime { get; set; }

		/// <summary>
		/// Existing station's computer name
		/// </summary>
		public string ComputerName { get; set; }

		public UserAlreadyHasStationException()
			:base()
		{
		}

		public UserAlreadyHasStationException(string msg)
			: base(msg)
		{
		}
	}

	public class InvalidDriverException : Exception
	{
		public InvalidDriverException(string msg)
			: base(msg)
		{
		}
	}

	public class GetDropboxOAuthException : Exception
	{
		public GetDropboxOAuthException(string msg)
			: base(msg)
		{
		}
	}

	public class DropboxNotInstalledException : Exception
	{
		public DropboxNotInstalledException(string msg)
			: base(msg)
		{
		}
	}

	public class DropboxNoSyncFolderException : Exception
	{
		public DropboxNoSyncFolderException(string msg)
			: base(msg)
		{
		}
	}

	public class DropboxWrongAccountException : Exception
	{
		public DropboxWrongAccountException(string msg)
			: base(msg)
		{
		}
	}

	public class DropboxNotConnectedException : Exception
	{
		public DropboxNotConnectedException(string msg)
			: base(msg)
		{
		}
	}

	public class StorageStatus
	{
		public string type { get; set; }
		public bool connected { get; set; }
		public long quota { get; set; }
		public long used { get; set; }
	}
}
