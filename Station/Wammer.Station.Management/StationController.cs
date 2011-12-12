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

namespace Wammer.Station.Management
{
	public class StationController
	{
		private static ServiceController scvCtrl;

		static StationController()
		{
			scvCtrl = new ServiceController(StationService.SERVICE_NAME);
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
			Model.Drivers driver = Model.Drivers.collection.FindOne();
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
			Model.Drivers driver = Model.Drivers.collection.FindOne();
			if (driver == null)
				return null;

			return driver.folder;
		}

		/// <summary>
		/// Move default folder to another location
		/// </summary>
		/// <remarks>
		/// Moving the default folder does these internally :
		/// - stop station service
		/// - write new folder location to database
		/// - move folder to the destination
		/// - start station service again
		/// 
		/// This method must be called after station owner is set. Otherwise, InvalidOperation is 
		/// thrown.
		/// </remarks>
		/// <param name="absPath">absolute path to user's folder. The folder must be empty</param>
		/// <exception cref="System.ArgumentException">
		/// absPath is not an absolute path</exception>
		/// <exception cref="System.IOException">
		/// absPath is not empty, readonly or used by other process</exception>
		/// <exception cref="System.InvalidOperationException">
		/// The station's owner is not set yet.</exception>
		public static void MoveDefaultFolder(string absPath)
		{
			if (!Path.IsPathRooted(absPath))
				throw new ArgumentException("Not an absolute path");

			string srcDir = GetDefaultFolder();
			if (srcDir == null)
				throw new InvalidOperationException("Station owner is not set yet.");

			ProcessStartInfo info = new ProcessStartInfo(
				Assembly.GetExecutingAssembly().Location,
				"--moveFolder " + absPath);

			info.Verb = "runas"; // to elevate privileges
			info.WindowStyle = ProcessWindowStyle.Hidden;
			info.CreateNoWindow = true;

			Process proc = new Process
			{
				EnableRaisingEvents = true,
				StartInfo = info,
			};

			proc.Start();
			proc.WaitForExit();

			if (proc.ExitCode != 0)
			{
				throw new ExternalException(proc.StandardOutput.ReadToEnd(), proc.ExitCode);
			}
		}


		/// <summary>
		/// Add a user to a station
		/// </summary>
		/// <param name="email"></param>
		/// <param name="password"></param>
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
		public static string AddUser(string email, string password)
		{
			try
			{
				AddUserResponse res = CloudServer.request<AddUserResponse>(
					new WebClient(),
					"http://localhost:9981/v2/station/drivers/add", 
					new Dictionary<object, object>{
					{ "email", email},
					{ "password", password}
				});

				return res.session_token;
			}
			catch (Cloud.WammerCloudException e)
			{
				if (e.HttpError == WebExceptionStatus.ConnectFailure)
					throw new StationServiceDownException("Station service down?");

				switch (e.WammerError)
				{
					case (int)StationApiError.ConnectToCloudError:
						throw new ConnectToCloudException(e.Message);
					case (int)StationApiError.AuthFailed:
						throw new AuthenticationException(e.Message);
					case (int)StationApiError.DriverExist:
						throw new StationAlreadyHasDriverException(e.Message);
					case (int)StationApiError.AlreadyHasStaion:
						StationSignUpResponse resp = fastJSON.JSON.Instance.
												ToObject<Cloud.StationSignUpResponse>(e.response);
						throw new UserAlreadyHasStationException
						{
							Id = resp.station.station_id,
							Location = resp.station.location,
							LastSyncTime = resp.station.LastSeen,
						};
						
					default:
						throw;
				}
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
			using (WebClient agent = new WebClient())
			{
				User user = User.LogIn(agent, driverEmail, password);
				Cloud.StationApi.SignOff(agent, stationId, user.Token);
			}
		}

		/// <summary>
		/// List all detected cloud storages
		/// </summary>
		public static List<StorageStatus> ListCloudStorage()
		{
			List<StorageStatus> cloudstorages = new List<StorageStatus>();

			if (DropboxHelper.IsInstalled())
			{
				// currently only support one driver
				Model.Drivers driver = Model.Drivers.collection.FindOne();
				Model.CloudStorage cloudstorage = Model.CloudStorage.collection.FindOne(Query.EQ("Type", "dropbox"));
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
					new WebClient(),
					"http://localhost:9981/v2/cloudstorage/dropbox/oauth",
					new Dictionary<object, object>(),
					true
				);
				return res.oauth_url;
			}
			catch (Cloud.WammerCloudException e)
			{
				switch (e.WammerError)
				{
					case (int)DropboxApiError.GetOAuthFailed:
						throw new GetDropboxOAuthException("Unable to get Dropbox OAuth information");

					default:
						throw;
				}
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
			if (DropboxHelper.IsInstalled())
			{
				try
				{
					string folder = DropboxHelper.GetSyncFolder();
					CloudResponse res = CloudServer.request<CloudResponse>(
						new WebClient(),
						"http://localhost:9981/v2/cloudstorage/dropbox/connect",
						new Dictionary<object, object> { { "quota", quota }, { "folder", folder } },
						true
					);
				}
				catch (Cloud.WammerCloudException e)
				{
					switch (e.WammerError)
					{
						case (int)DropboxApiError.NoSyncFolder:
							throw new DropboxNoSyncFolderException("Dropbox sync folder does not exist");

						case (int)DropboxApiError.LinkWrongAccount:
							throw new DropboxWrongAccountException("Link to inconsistent Dropbox account");

						default:
							throw;
					}
				}
			}
			else
			{
				throw new DropboxNotInstalledException("Dropbox is not installed");
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
			if (DropboxHelper.IsInstalled())
			{
				try
				{
					CloudResponse res = CloudServer.request<CloudResponse>(
						new WebClient(),
						"http://localhost:9981/v2/cloudstorage/dropbox/update",
						new Dictionary<object, object> { { "quota", quota } },
						true
					);
				}
				catch (Cloud.WammerCloudException e)
				{
					switch (e.WammerError)
					{
						case (int)DropboxApiError.DropboxNotConnected:
							throw new DropboxNotConnectedException("Dropbox is not connected");

						default:
							throw;
					}
				}
			}
			else
			{
				throw new DropboxNotInstalledException("Dropbox is not installed");
			}
		}

		/// <summary>
		/// Disconnect Waveface from Dropbox
		/// </summary>
		public static void DisconnectDropbox()
		{
			CloudResponse res = CloudServer.request<CloudResponse>(
				new WebClient(),
				"http://localhost:9981/v2/cloudstorage/dropbox/disconnect",
				new Dictionary<object, object>(),
				true
			);
		}
		#region private accessors

		/// <summary>
		/// Starts station services, including MongoDB service
		/// </summary>
		/// <param name="timeout">timeout value</param>
		/// <exception cref="System.TimeoutException"></exception>
		private static void StartServices(TimeSpan timeout)
		{
			if (scvCtrl.Status != ServiceControllerStatus.Running &&
				scvCtrl.Status != ServiceControllerStatus.StartPending)
				scvCtrl.Start();

			scvCtrl.WaitForStatus(ServiceControllerStatus.Running, timeout);
		}

		/// <summary>
		/// Stops station services, including MongoDB service
		/// </summary>
		private static void StopServices(TimeSpan timeout)
		{
			if (scvCtrl.Status != ServiceControllerStatus.Stopped &&
				scvCtrl.Status != ServiceControllerStatus.StopPending)
				scvCtrl.Stop();

			scvCtrl.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
		}

		/// <summary>
		/// Sets whether station service auto starts itself after system boots
		/// </summary>
		/// <param name="autoStart"></param>
		private static void SetServiceAutoStart(bool autoStart)
		{
			ServiceHelper.ChangeStartMode(scvCtrl, ServiceStartMode.Automatic);
		}

		

		private static void _MoveDefaultFolder(string absPath)
		{
			if (!Path.IsPathRooted(absPath))
				throw new ArgumentException("Not an absolute path");

			if (Directory.Exists(absPath))
				Directory.Delete(absPath);

			string srcDir = GetDefaultFolder();
			StopServices(TimeSpan.FromSeconds(10.0));
			Directory.Move(srcDir, absPath);
			SetDefaultFolder(absPath);
			StartServices(TimeSpan.FromSeconds(10.0));
		}

		private static void SetDefaultFolder(string absPath)
		{
			Model.Drivers driver = Model.Drivers.collection.FindOne();
			if (driver == null)
				throw new InvalidOperationException("Cannot set default folder before driver is not set yet.");

			driver.folder = absPath;
			Model.Drivers.collection.Save(driver);
		}
		#endregion


		public static int Main(string[] args)
		{
			try
			{
				for (int i = 0; i < args.Length; i++)
				{
					switch (args[i])
					{
						case "--moveFolder":
							string destFolder = args[++i];
							_MoveDefaultFolder(destFolder);
							return 0;
						default:
							Console.WriteLine("Unknown parameter: " + args[i]);
							break;
					}
				}

				Console.WriteLine("no parameter...");
				return 1;
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
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

		public UserAlreadyHasStationException()
			:base()
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
