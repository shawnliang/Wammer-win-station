using Microsoft.Win32;
using MongoDB.Driver.Builders;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using Wammer.Cloud;
using Wammer.Model;
using Wammer.PostUpload;
using Wammer.Station.AttachmentUpload;
using Wammer.Station.Timeline;
using Waveface.Stream.Model;
using Wammer.Station.Import;

namespace Wammer.Station
{
	public class Station
	{
		#region Const
		private const string STATION_ID_REGISTORY_KEY = "stationId";
		private const int BODY_SYNC_THREAD_COUNT = 10;
		private const int UPSTREAM_THREAD_COUNT = 1;
		private const int IMPORT_TASK_THREAD_COUNT = 1;
		#endregion


		#region Static Var
		private static Station _instance;
		#endregion


		#region Var
		private PostUploadTaskRunner _postUploadRunner;
		private StationTimer _stationTimer;
		private string _stationID;
		private TaskRunner<IResourceDownloadTask>[] _bodySyncRunners;
		private TaskRunner<INamedTask>[] _upstreamTaskRunner;
		private TaskRunner<ImportTask>[] _importTaskRunner;

		private DriverController _driverAgent;
		private Object _bodySyncRunnersLockObj;
		private Exit threadsExit = new Exit();

		private bool userWantsSyncing = true;
		private bool isSyncing = false;
		private object synclock = new object();

		private Notify.WebSocketNotifyChannels wsChannelServer = new Notify.WebSocketNotifyChannels(9983);
		private Notify.PostUpsertNotifier postUpsertNotifier;
		private Notify.ConnectionUpdator connectionUpdator = new Wammer.Station.Notify.ConnectionUpdator();
		#endregion


		#region Public Static Property
		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static Station Instance
		{
			get { return _instance ?? (_instance = new Station()); }
		}
		#endregion


		#region Private Property
		/// <summary>
		/// Gets the m_ post upload runner.
		/// </summary>
		/// <value>The m_ post upload runner.</value>
		private PostUploadTaskRunner m_PostUploadRunner
		{
			get { return _postUploadRunner ?? (_postUploadRunner = new PostUploadTaskRunner(PostUploadTaskQueue.Instance)); }
		}

		/// <summary>
		/// Gets the m_ station timer.
		/// </summary>
		/// <value>The m_ station timer.</value>
		private StationTimer m_StationTimer
		{
			get
			{
				return _stationTimer ?? (_stationTimer = new StationTimer(BodySyncQueue.Instance));
			}
		}

		/// <summary>
		/// Gets the m_ body sync runners lock obj.
		/// </summary>
		/// <value>The m_ body sync runners lock obj.</value>
		private Object m_BodySyncRunnersLockObj
		{
			get
			{
				return _bodySyncRunnersLockObj ?? (_bodySyncRunnersLockObj = new Object());
			}
		}

		/// <summary>
		/// Gets the m_ body sync runners.
		/// </summary>
		/// <value>The m_ body sync runners.</value>
		private TaskRunner<IResourceDownloadTask>[] m_BodySyncRunners
		{
			get
			{
				lock (m_BodySyncRunnersLockObj)
				{
					if (_bodySyncRunners == null)
					{
						_bodySyncRunners = Enumerable.Range(0, BODY_SYNC_THREAD_COUNT).Select(
							item => new TaskRunner<IResourceDownloadTask>(BodySyncQueue.Instance, threadsExit)).ToArray();
					}
					return _bodySyncRunners;
				}
			}
		}

		/// <summary>
		/// Gets the m_ upstream task runner.
		/// </summary>
		/// <value>The m_ upstream task runner.</value>
		private TaskRunner<INamedTask>[] m_UpstreamTaskRunner
		{
			get
			{
				return _upstreamTaskRunner ?? (_upstreamTaskRunner = Enumerable.Range(0, UPSTREAM_THREAD_COUNT).Select(
					item => new TaskRunner<INamedTask>(AttachmentUploadQueueHelper.Instance, threadsExit)).ToArray());
			}
		}


		private TaskRunner<ImportTask>[] m_ImportTaskRunner
		{
			get
			{
				return _importTaskRunner ?? (_importTaskRunner = Enumerable.Range(0, IMPORT_TASK_THREAD_COUNT).Select(
					item => new TaskRunner<ImportTask>(ImportTaskQueue.Instance, threadsExit)).ToArray());
			}
		}
		/// <summary>
		/// Gets the m_ driver agent.
		/// </summary>
		/// <value>The m_ driver agent.</value>
		private DriverController m_DriverAgent
		{
			get { return _driverAgent ?? (_driverAgent = new DriverController()); }
		}
		#endregion


		#region Public Property
		/// <summary>
		/// Gets the station ID.
		/// </summary>
		/// <value>The station ID.</value>
		public string StationID
		{
			get
			{
				if (_stationID == null)
				{
					InitStationId();
					_stationID = (string)StationRegistry.GetValue(STATION_ID_REGISTORY_KEY, null);
				}
				return _stationID;
			}
		}

		public Wammer.Station.Notify.PostUpsertNotifier PostUpsertNotifier
		{
			get { return postUpsertNotifier; }
		}
		#endregion


		#region Event
		public EventHandler<UserLoginEventArgs> UserLogined;
		#endregion


		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="Station"/> class.
		/// </summary>
		private Station()
		{
			NetworkChange.NetworkAvailabilityChanged += NetworkChange_NetworkAvailabilityChanged;
			SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;




			postUpsertNotifier = new Notify.PostUpsertNotifier(wsChannelServer, new Notify.PostUpsertNotifierDB());

			wsChannelServer.ChannelAdded += (s, e) =>
			{
				try
				{
					if (DriverCollection.Instance.FindOne(Query.EQ("_id", e.UserId)) == null)
					{
						e.Channel.Close((int)Notify.ErrorCode.AccessDenied, "User not exist");
						return;
					}

					// After device notification channel is established, force device to check changelogs/usertracks because 
					// station ignores its subscription message which specifies where the sync should starts ( a timestamp or seq_num ).
					e.Channel.Notify();
				}
				catch (Exception ex)
				{
					this.LogWarnMsg("Unable to notify device to check changelogs after connection is established. User: " + e.Channel.UserId, ex);
				}
			};

			wsChannelServer.ChannelAdded += connectionUpdator.OnClientConnected;
			wsChannelServer.ChannelRemoved += connectionUpdator.OnClientDisconnected;

			wsChannelServer.Start();
			m_PostUploadRunner.PostUpserted += postUpsertNotifier.OnPostUpserted;
		}
		#endregion

		#region Private Method

		/// <summary>
		/// Inits the station id.
		/// </summary>
		private void InitStationId()
		{
			var stationId = (string)StationRegistry.GetValue(STATION_ID_REGISTORY_KEY, null);

			if (stationId == null)
			{
				stationId = GenerateUniqueDeviceId();

				StationRegistry.SetValue("stationId", stationId);
			}
		}

		/// <summary>
		/// Generates the unique device id.
		/// </summary>
		/// <returns></returns>
		private string GenerateUniqueDeviceId()
		{
			// uniqueness is at least guaranteed by volume serial number
			var volumeSN = string.Empty;
			try
			{
				var pathRoot = Path.GetPathRoot(Environment.CurrentDirectory);

				Debug.Assert(pathRoot != null);
				var drive = pathRoot.TrimEnd('\\');
				var disk = new ManagementObject(string.Format("win32_logicaldisk.deviceid=\"{0}\"", drive));
				disk.Get();
				volumeSN = disk["VolumeSerialNumber"].ToString();
				this.LogDebugMsg(String.Format("volume serial number = {0}", volumeSN));
			}
			catch (Exception e)
			{
				this.LogDebugMsg("Unable to retrieve volume serial number", e);
				return Guid.NewGuid().ToString();
			}

			var cpuID = "DEFAULT";
			try
			{
				var mc = new ManagementClass("win32_processor");
				var moc = mc.GetInstances();
				foreach (var mo in moc)
				{
					// use first CPU's ID
					cpuID = mo.Properties["processorID"].Value.ToString();
					break;
				}
				this.LogDebugMsg(string.Format("processor ID = {0}", cpuID));
			}
			catch (Exception e)
			{
				this.LogDebugMsg("Unable to retrieve processor ID", e);
			}

			var md5 = MD5.Create().ComputeHash(Encoding.Default.GetBytes(cpuID + "-" + volumeSN));
			return new Guid(md5).ToString();
		}

		private void CheckAndUpdateDriver(LoginedSession loginInfo, string sessionToken, string userID)
		{
			if (loginInfo == null)
				return;

			var user = loginInfo.user;

			Debug.Assert(user != null, "user != null");

			var driver = DriverCollection.Instance.FindOne(Query.EQ("_id", user.user_id));

			if (driver != null)
				return;

			driver = DriverCollection.Instance.FindOne(Query.EQ("email", user.email));

			if (driver == null)
				throw new WammerStationException("Driver not existed", (int)StationLocalApiError.NotFound);

			m_DriverAgent.RemoveDriver(StationID, user.user_id);

			m_DriverAgent.AddDriver(driver.folder, StationID, userID, sessionToken);
		}

		private void CheckAndUpdateDriver(LoginedSession loginInfo, string email, string password, string deviceID, string deviceName)
		{
			if (loginInfo == null)
				return;

			var user = loginInfo.user;

			Debug.Assert(user != null, "user != null");

			var driver = DriverCollection.Instance.FindOne(Query.EQ("_id", user.user_id));

			if (driver != null)
				return;

			driver = DriverCollection.Instance.FindOne(Query.EQ("email", user.email));

			if (driver == null)
				throw new WammerStationException("Driver not existed", (int)StationLocalApiError.NotFound);

			m_DriverAgent.RemoveDriver(StationID, user.user_id);

			m_DriverAgent.AddDriver(driver.folder, StationID, email, password, deviceID, deviceName);
		}

		#endregion


		#region Protected Method

		/// <summary>
		/// Raises the <see cref="E:UserLogined"/> event.
		/// </summary>
		/// <param name="arg">The <see cref="Wammer.Station.UserLoginEventArgs"/> instance containing the event data.</param>
		protected void OnUserLogined(UserLoginEventArgs arg)
		{
			var handler = UserLogined;
			if (handler != null)
			{
				handler(this, arg);
			}
		}
		#endregion


		#region Public Method
		/// <summary>
		/// Starts this instance.
		/// </summary>
		public void Start()
		{
			ResumeSyncByUser();
		}

		/// <summary>
		/// Stops this instance.
		/// </summary>
		public void Stop()
		{
			SuspendSyncByUser();
			wsChannelServer.Stop();
		}

		/// <summary>
		/// Suspends the sync.
		/// </summary>
		public void SuspendSyncByUser()
		{
			userWantsSyncing = false;

			if (!isSyncing)
				suspendSync();
		}

		private void suspendSync()
		{
			m_StationTimer.Stop();

			threadsExit.GoExit = true;

			// stop ws notify channel
			wsChannelServer.Stop();

			// Signal to stop runners
			m_PostUploadRunner.StopAsync();
			Array.ForEach(m_ImportTaskRunner, taskRunner => taskRunner.StopAsync());
			Array.ForEach(m_BodySyncRunners, taskRunner => taskRunner.StopAsync());
			Array.ForEach(m_UpstreamTaskRunner, taskRunner => taskRunner.StopAsync());

			// Wait runners to stop
			m_PostUploadRunner.JoinOrKill();
			Array.ForEach(m_ImportTaskRunner, taskRunner => taskRunner.JoinOrKill());
			Array.ForEach(m_BodySyncRunners, taskRunner => taskRunner.JoinOrKill());
			Array.ForEach(m_UpstreamTaskRunner, taskRunner => taskRunner.JoinOrKill());

			this.LogDebugMsg("Stop synchronization successfully");
		}

		/// <summary>
		/// Resumes the sync.
		/// </summary>
		public void ResumeSyncByUser()
		{
			userWantsSyncing = true;

			if (!isSyncing)
				resumeSync();
		}

		private void resumeSync()
		{
			threadsExit.GoExit = false;
			m_PostUploadRunner.Start();
			m_StationTimer.Start();
			Array.ForEach(m_BodySyncRunners, taskRunner => taskRunner.Start());
			Array.ForEach(m_UpstreamTaskRunner, taskRunner => taskRunner.Start());
			Array.ForEach(m_ImportTaskRunner, taskRunner => taskRunner.Start());
			wsChannelServer.Start();

			this.LogDebugMsg("Start synchronization successfully");
		}

		public LoginedSession Login(string apikey, string sessionToken, string userID)
		{
			if (apikey == null) throw new ArgumentNullException("apikey");
			if (sessionToken == null) throw new ArgumentNullException("sessionToken");
			if (userID == null) throw new ArgumentNullException("userID");

			var loginInfo = User.GetLoginInfo(userID, apikey, sessionToken);
			CheckAndUpdateDriver(loginInfo, sessionToken, userID);

			LoginedSessionCollection.Instance.Save(loginInfo);

			OnUserLogined(new UserLoginEventArgs(loginInfo.user.email, loginInfo.session_token, apikey, userID));
			return loginInfo;
		}

		public LoginedSession Login(string apikey, string email, string password, string deviceID, string deviceName)
		{
			if (apikey == null) throw new ArgumentNullException("apikey");
			if (email == null) throw new ArgumentNullException("email");
			if (password == null) throw new ArgumentNullException("password");
			if (deviceID == null) throw new ArgumentNullException("deviceID");
			if (deviceName == null) throw new ArgumentNullException("deviceName");

			var user = User.LogIn(email, password, apikey, deviceID, deviceName, 2500);

			Debug.Assert(user != null, "user != null");
			var loginInfo = user.LoginedInfo;

			CheckAndUpdateDriver(loginInfo, email, password, deviceID, deviceName);

			LoginedSessionCollection.Instance.Remove(Query.EQ("user.email", email));
			LoginedSessionCollection.Instance.Save(loginInfo);

			OnUserLogined(new UserLoginEventArgs(email, loginInfo.session_token, apikey, user.Id));
			return loginInfo;
		}


		public void Logout(string apiKey, string sessionToken)
		{
			try
			{
				User.LogOut(sessionToken, apiKey);
			}
			catch (Exception e)
			{
				this.LogDebugMsg("Unable to logout from Stream cloud", e);
			}

			var loginedSession = LoginedSessionCollection.Instance.FindOne(Query.EQ("_id", sessionToken));

			if (loginedSession != null)
				LoginedSessionCollection.Instance.Remove(Query.EQ("user.email", loginedSession.user.email));
		}
		#endregion


		#region Event Process
		/// <summary>
		/// Handles the PowerModeChanged event of the SystemEvents control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="Microsoft.Win32.PowerModeChangedEventArgs"/> instance containing the event data.</param>
		void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
		{
			this.LogDebugMsg("Power mode => " + e.Mode.ToString());
		}

		/// <summary>
		/// Handles the NetworkAvailabilityChanged event of the NetworkChange control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Net.NetworkInformation.NetworkAvailabilityEventArgs"/> instance containing the event data.</param>
		void NetworkChange_NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
		{
			lock (synclock)
			{
				this.LogDebugMsg("Network available => " + e.IsAvailable.ToString());

				if (!e.IsAvailable)
					suspendSync();
				else if (userWantsSyncing)
					resumeSync();
			}
		}

		void m_PostUploadRunner_PostUpserted(object sender, PostUpsertEventArgs e)
		{
			try
			{
				wsChannelServer.NotifyToUserChannels(e.UserId, e.SessionToken);
			}
			catch (Exception ex)
			{
				this.LogWarnMsg("Unable to notify post changed event to devices. user: " + e.UserId, ex);
			}
		}
		#endregion
	}
}
