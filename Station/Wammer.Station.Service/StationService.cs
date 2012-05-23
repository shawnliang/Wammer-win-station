using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using Wammer.Cloud;
using Wammer.PerfMonitor;
using Wammer.PostUpload;
using Wammer.Station.APIHandler;
using Wammer.Station.AttachmentUpload;
using Wammer.Station.Timeline;
using Wammer.Utility;
using fastJSON;
using log4net;
using log4net.Config;
using Wammer.Model;

namespace Wammer.Station.Service
{
	public partial class StationService : ServiceBase
	{
		#region Const
		public const string SERVICE_NAME = "WavefaceStation";
		public const string MONGO_SERVICE_NAME = "MongoDbForWaveface"; 
		#endregion

		#region Var
		private PostUploadTaskRunner _postUploadRunner;
		private BackOff _backoff;
		#endregion


		#region Private Property

		private PostUploadTaskRunner m_PostUploadRunner
		{
			get { return _postUploadRunner ?? (_postUploadRunner = new PostUploadTaskRunner(PostUploadTaskQueue.Instance)); }
		}


		private BackOff m_BackOff
		{
			get { return _backoff ?? (_backoff = new BackOff(500, 1000, 2000, 3000, 5000)); }
		}

		#endregion

		private static readonly ILog logger = LogManager.GetLogger("StationService");
		private TaskRunner<IResourceDownloadTask>[] bodySyncRunners;
		private HttpServer functionServer;
		private HttpServer managementServer;
		private string resourceBasePath;
		private string stationId;
		private StationTimer stationTimer;
		private TaskRunner<ITask>[] upstreamTaskRunner;
		private AttachmentViewHandler viewHandler;

		public StationService()
		{
			XmlConfigurator.Configure();
			InitializeComponent();
			ServiceName = SERVICE_NAME;
		}

		~StationService()
		{
			if (managementServer != null)
				managementServer.Dispose();

			if (functionServer != null)
				functionServer.Dispose();
		}

		public void Run()
		{
			OnStart(null);

			Console.WriteLine("Press any key to exit");
			Console.ReadKey();

			OnStop();
		}

		protected override void OnStart(string[] args)
		{
			try
			{
				logger.Info("============== Starting Stream Station =================");

				m_BackOff.ResetLevel();
				while (!Database.TestConnection(1))
				{
					Thread.Sleep(m_BackOff.NextValue());
					System.Windows.Forms.Application.DoEvents();
					m_BackOff.IncreaseLevel();
				}

				TaskQueue.Init();

				ConfigThreadPool();

				ResetPerformanceCounter();

				AppDomain.CurrentDomain.UnhandledException +=
					CurrentDomain_UnhandledException;

				Environment.CurrentDirectory = Path.GetDirectoryName(
					Assembly.GetExecutingAssembly().Location);

				
				logger.Debug("Initialize Stream Service");
				InitStationId();
				InitResourceBasePath();

				JSON.Instance.UseUTCDateTime = true;

				functionServer = new HttpServer(9981); // TODO: remove hard code


				stationTimer = new StationTimer(BodySyncQueue.Instance, stationId);

				functionServer.TaskEnqueue += HttpRequestMonitor.Instance.OnTaskEnqueue;


				var attachmentHandler = new AttachmentUploadHandler();
				var attachmentMonitor = new AttachmentUploadMonitor();

				attachmentHandler.AttachmentProcessed +=
					new AttachmentProcessedHandler(
						new AttachmentUtility()).OnProcessed;
				attachmentHandler.ProcessSucceeded += attachmentMonitor.OnProcessSucceeded;


				var cloudForwarder = new BypassHttpHandler(CloudServer.BaseUrl);
				InitCloudForwarder(cloudForwarder);

				var downstreamMonitor = new AttachmentDownloadMonitor();
				BodySyncQueue.Instance.Enqueued += downstreamMonitor.OnDownstreamTaskEnqueued;

				InitFunctionServerHandlers(attachmentHandler, cloudForwarder, downstreamMonitor);


				logger.Debug("Start function server");
				functionServer.Start();
				stationTimer.Start();

				const int bodySyncThreadNum = 1;
				bodySyncRunners = new TaskRunner<IResourceDownloadTask>[bodySyncThreadNum];
				for (int i = 0; i < bodySyncThreadNum; i++)
				{
					var bodySyncRunner = new TaskRunner<IResourceDownloadTask>(BodySyncQueue.Instance);
					bodySyncRunners[i] = bodySyncRunner;

					bodySyncRunner.TaskExecuted += downstreamMonitor.OnDownstreamTaskDone;
					bodySyncRunner.Start();
				}
				BodySyncQueue.Instance.TaskDropped += downstreamMonitor.OnDownstreamTaskDone;

				m_PostUploadRunner.Start();

				const int upstreamThreads = 1;
				upstreamTaskRunner = new TaskRunner<ITask>[upstreamThreads];
				for (int i = 0; i < upstreamThreads; i++)
				{
					upstreamTaskRunner[i] = new TaskRunner<ITask>(AttachmentUploadQueue.Instance);
					upstreamTaskRunner[i].Start();
				}


				logger.Debug("Add handlers to management server");
				managementServer = new HttpServer(9989);
				managementServer.TaskEnqueue += HttpRequestMonitor.Instance.OnTaskEnqueue;

				var addDriverHandler = new AddDriverHandler(stationId, resourceBasePath);
				InitManagementServerHandler(addDriverHandler);

				addDriverHandler.DriverAdded += addDriverHandler_DriverAdded;
				addDriverHandler.BeforeDriverSaved += addDriverHandler_BeforeDriverSaved;
				logger.Debug("Start management server");
				managementServer.Start();


				logger.Info("Stream station is started");
			}
			catch (Exception ex)
			{
				logger.Error("Unknown exception", ex);
				throw;
			}
		}

		private static void InitCloudForwarder(BypassHttpHandler cloudForwarder)
		{
			cloudForwarder.AddExceptPrefix(GetDefaultBathPath("/auth/"));
			cloudForwarder.AddExceptPrefix(GetDefaultBathPath("/users/"));
			cloudForwarder.AddExceptPrefix(GetDefaultBathPath("/groups/"));
			cloudForwarder.AddExceptPrefix(GetDefaultBathPath("/stations/"));
		}

		private void InitManagementServerHandler(AddDriverHandler addDriverHandler)
		{
			var resumeHandler = new ResumeSyncHandler(m_PostUploadRunner, stationTimer, bodySyncRunners, upstreamTaskRunner);
			resumeHandler.SyncResumed += viewHandler.OnSyncResumed;
			managementServer.AddHandler(GetDefaultBathPath("/station/resumeSync/"), resumeHandler);

			var suspendHandler = new SuspendSyncHandler(m_PostUploadRunner, stationTimer, bodySyncRunners, upstreamTaskRunner);
			suspendHandler.SyncSuspended += viewHandler.OnSyncSuspended;
			managementServer.AddHandler(GetDefaultBathPath("/station/suspendSync/"), suspendHandler);

			managementServer.AddHandler(GetDefaultBathPath("/station/drivers/add/"), addDriverHandler);
			managementServer.AddHandler(GetDefaultBathPath("/station/drivers/list/"), new ListDriverHandler());

			var removeOwnerHandler = new RemoveOwnerHandler(stationId);
			removeOwnerHandler.DriverRemoved += new EventHandler<DriverRemovedEventArgs>(removeOwnerHandler_DriverRemoved);

			managementServer.AddHandler(GetDefaultBathPath("/station/drivers/remove/"), removeOwnerHandler);
			managementServer.AddHandler(GetDefaultBathPath("/station/status/get/"), new StatusGetHandler());
			managementServer.AddHandler(GetDefaultBathPath("/cloudstorage/list"), new ListCloudStorageHandler());
			managementServer.AddHandler(GetDefaultBathPath("/cloudstorage/dropbox/oauth/"), new DropBoxOAuthHandler());
			managementServer.AddHandler(GetDefaultBathPath("/cloudstorage/dropbox/connect/"), new DropBoxConnectHandler());
			managementServer.AddHandler(GetDefaultBathPath("/cloudstorage/dropbox/update/"), new DropBoxUpdateHandler());
			managementServer.AddHandler(GetDefaultBathPath("/cloudstorage/dropbox/disconnect/"), new DropboxDisconnectHandler());
			managementServer.AddHandler(GetDefaultBathPath("/availability/ping/"), new PingHandler());
		}

		void removeOwnerHandler_DriverRemoved(object sender, DriverRemovedEventArgs e)
		{
			try
			{
				BodySyncQueue.Instance.RemoveAllByUserId(e.UserId);
			}
			catch (Exception ex)
			{
				logger.Warn("Unable to remove body sync tasks of removed user " + e.UserId, ex);
			}
		}

		private void InitFunctionServerHandlers(AttachmentUploadHandler attachmentHandler, BypassHttpHandler cloudForwarder,
		                                        AttachmentDownloadMonitor downstreamMonitor)
		{
			logger.Debug("Add cloud forwarders to function server");
			functionServer.AddDefaultHandler(cloudForwarder);

			logger.Debug("Add handlers to function server");

			functionServer.AddHandler("/", new DummyHandler());

			functionServer.AddHandler(GetDefaultBathPath("/attachments/upload/"),
			                          attachmentHandler);

			functionServer.AddHandler(GetDefaultBathPath("/station/resourceDir/get/"),
			                          new ResouceDirGetHandler(resourceBasePath));

			functionServer.AddHandler(GetDefaultBathPath("/station/resourceDir/set/"),
			                          new ResouceDirSetHandler());

			functionServer.AddHandler(GetDefaultBathPath("/attachments/get/"),
			                          new AttachmentGetHandler());

			functionServer.AddHandler(GetDefaultBathPath("/availability/ping/"),
			                          new PingHandler());

			functionServer.AddHandler(GetDefaultBathPath("/reachability/ping/"),
			                          new PingHandler());

			functionServer.AddHandler(GetDefaultBathPath("/posts/getLatest/"),
			                          new HybridCloudHttpRouter(new PostGetLatestHandler()));

			functionServer.AddHandler(GetDefaultBathPath("/posts/get/"),
			                          new HybridCloudHttpRouter(new PostGetHandler()));

			functionServer.AddHandler(GetDefaultBathPath("/posts/getSingle/"),
			                          new HybridCloudHttpRouter(new PostGetSingleHandler()));

			functionServer.AddHandler(GetDefaultBathPath("/posts/fetchByFilter/"),
									  new HybridCloudHttpRouter(new PostFetchByFilterHandler()));

			functionServer.AddHandler(GetDefaultBathPath("/posts/new/"),
			                          new HybridCloudHttpRouter(new NewPostHandler(PostUploadTaskController.Instance)));

			//functionServer.AddHandler(GetDefaultBathPath("/posts/new/"),
			//new NewPostHandler(PostUploadTaskController.Instance));

			functionServer.AddHandler(GetDefaultBathPath("/posts/update/"),
			                          new HybridCloudHttpRouter((new UpdatePostHandler(PostUploadTaskController.Instance))));

			functionServer.AddHandler(GetDefaultBathPath("/posts/hide/"),
			                          new HybridCloudHttpRouter((new HidePostHandler(PostUploadTaskController.Instance))));

			functionServer.AddHandler(GetDefaultBathPath("/posts/newComment/"),
			                          new HybridCloudHttpRouter((new NewPostCommentHandler(PostUploadTaskController.Instance))));

			functionServer.AddHandler(GetDefaultBathPath("/footprints/setLastScan/"),
			                          new HybridCloudHttpRouter(new FootprintSetLastScanHandler()));

			functionServer.AddHandler(GetDefaultBathPath("/footprints/getLastScan/"),
			                          new HybridCloudHttpRouter(new FootprintGetLastScanHandler()));

			functionServer.AddHandler(GetDefaultBathPath("/usertracks/get/"),
			                          new HybridCloudHttpRouter(new UserTrackHandler()));

			var loginHandler = new UserLoginHandler(stationId, resourceBasePath);
			functionServer.AddHandler(GetDefaultBathPath("/auth/login/"),
			                          loginHandler);

			loginHandler.UserLogined += loginHandler_UserLogined;

			functionServer.AddHandler(GetDefaultBathPath("/auth/logout/"),
									  new UserLogoutHandler());

			viewHandler = new AttachmentViewHandler(stationId);
			functionServer.AddHandler(GetDefaultBathPath("/attachments/view/"),
			                          viewHandler);

			viewHandler.FileDownloadStarted += downstreamMonitor.OnDownstreamTaskEnqueued;
			viewHandler.FileDownloadInProgress += downstreamMonitor.OnDownstreamTaskInProgress;
			viewHandler.FileDownloadFinished += downstreamMonitor.OnDownstreamTaskDone;
		}

		private void loginHandler_UserLogined(object sender, UserLoginEventArgs e)
		{
			TaskQueue.Enqueue(new UpdateDriverDBTask(e, stationId), TaskPriority.High);
		}

		private static string GetDefaultBathPath(string relativedPath)
		{
			return "/" + CloudServer.DEF_BASE_PATH + relativedPath;
		}

		private void addDriverHandler_DriverAdded(object sender, DriverAddedEvtArgs e)
		{
		}

		private void addDriverHandler_BeforeDriverSaved(object sender, BeforeDriverSavedEvtArgs e)
		{
			var syncer = new TimelineSyncer(
				new PostProvider(),
				new TimelineSyncerDBWithDriverCached(e.Driver),
				new UserTracksApi()
				);

			var downloader = new ResourceDownloader(BodySyncQueue.Instance, stationId);
			syncer.PostsRetrieved += downloader.PostRetrieved;
			syncer.PullTimeline(e.Driver);
		}

		private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			if (e.IsTerminating)
			{
				logger.Fatal("Unhandled exception. Program terminates");
				logger.Fatal(e.ExceptionObject);
			}
			else
			{
				logger.Fatal("Unhandled exception: " + e.ExceptionObject);
			}
		}

		protected override void OnStop()
		{
			Array.ForEach(bodySyncRunners, runner => runner.Stop());
			Array.ForEach(upstreamTaskRunner, runner => runner.Stop());
			m_PostUploadRunner.Stop();

			functionServer.Stop();
			functionServer.Close();

			stationTimer.Stop();
			stationTimer.Close();

			managementServer.Stop();
			managementServer.Close();
		}

		private void InitResourceBasePath()
		{
			resourceBasePath = (string) StationRegistry.GetValue("resourceBasePath", null);
			if (resourceBasePath == null)
			{
				resourceBasePath = "resource";
				if (!Directory.Exists(resourceBasePath))
					Directory.CreateDirectory(resourceBasePath);
			}
		}

		private void InitStationId()
		{
			stationId = (string) StationRegistry.GetValue("stationId", null);

			if (stationId == null)
			{
				stationId = GenerateUniqueDeviceId();

				StationRegistry.SetValue("stationId", stationId);
			}
		}

		private string GenerateUniqueDeviceId()
		{
			// uniqueness is at least guaranteed by volume serial number
			string volumeSN = string.Empty;
			try
			{
				string drive = Path.GetPathRoot(Environment.CurrentDirectory).TrimEnd('\\');
				ManagementObject disk = new ManagementObject(string.Format("win32_logicaldisk.deviceid=\"{0}\"", drive));
				disk.Get();
				volumeSN = disk["VolumeSerialNumber"].ToString();
				logger.DebugFormat("volume serial number = {0}", volumeSN);
			}
			catch (Exception e)
			{
				logger.Debug("Unable to retrieve volume serial number", e);
				return Guid.NewGuid().ToString();
			}

			string cpuID = "DEFAULT";
			try
			{
				ManagementClass mc = new ManagementClass("win32_processor");
				ManagementObjectCollection moc = mc.GetInstances();
				foreach (var mo in moc)
				{
					// use first CPU's ID
					cpuID = mo.Properties["processorID"].Value.ToString();
					break;
				}
				logger.DebugFormat("processor ID = {0}", cpuID);
			}
			catch (Exception e)
			{
				logger.Debug("Unable to retrieve processor ID", e);
			}

			byte[] md5 = MD5.Create().ComputeHash(Encoding.Default.GetBytes(cpuID + "-" + volumeSN));
			return new Guid(md5).ToString();
		}

		private void ConfigThreadPool()
		{
			int minWorker, minIO;

			ThreadPool.GetMinThreads(out minWorker, out minIO);

			minWorker = int.Parse(StationRegistry.GetValue("MinWorkerThreads", minWorker.ToString()).ToString());
			minIO = int.Parse(StationRegistry.GetValue("MinIOThreads", minIO.ToString()).ToString());

			if (minWorker > 0 && minIO > 0)
			{
				ThreadPool.SetMinThreads(minWorker, minIO);
				logger.InfoFormat("Min worker threads {0}, min IO completion threads {1}",
				                  minWorker.ToString(), minIO.ToString());
			}

			int maxConcurrentTaskCount = int.Parse(StationRegistry.GetValue("MaxConcurrentTaskCount", "6").ToString());
			if (maxConcurrentTaskCount > 0)
				TaskQueue.MaxConcurrentTaskCount = maxConcurrentTaskCount;
		}

		#region Private Method

		private void ResetPerformanceCounter()
		{
			PerfCounter.GetCounter(PerfCounter.UP_REMAINED_COUNT);
			PerfCounter.GetCounter(PerfCounter.DW_REMAINED_COUNT);
			PerfCounter.GetCounter(PerfCounter.UPSTREAM_RATE);
			PerfCounter.GetCounter(PerfCounter.DWSTREAM_RATE);
		}

		#endregion
	}


	internal class DummyHandler : IHttpHandler
	{
		#region IHttpHandler Members

		public event EventHandler<HttpHandlerEventArgs> ProcessSucceeded;

		public void HandleRequest(HttpListenerRequest request, HttpListenerResponse response)
		{
			//Debug.Fail("should not reach this code");
		}

		public object Clone()
		{
			return MemberwiseClone();
		}

		public void SetBeginTimestamp(long beginTime)
		{
		}


		public void HandleRequest()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}