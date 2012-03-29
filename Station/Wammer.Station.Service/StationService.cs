using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.ServiceProcess;
using Wammer.Cloud;
using Wammer.PerfMonitor;

namespace Wammer.Station.Service
{
	public partial class StationService : ServiceBase
	{
		public const string SERVICE_NAME = "WavefaceStation";
		public const string MONGO_SERVICE_NAME = "MongoDbForWaveface";

		private static log4net.ILog logger = log4net.LogManager.GetLogger("StationService");
		private HttpServer managementServer;
		private HttpServer functionServer;
		private StationTimer stationTimer;
		private string stationId;
		private string resourceBasePath;

		public StationService()
		{			
			log4net.Config.XmlConfigurator.Configure();
			InitializeComponent();
			this.ServiceName = SERVICE_NAME;
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
				logger.Info("============== Starting Waveface Station =================");
				ConfigThreadPool();

				ResetPerformanceCounter();

				AppDomain.CurrentDomain.UnhandledException +=
					new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

				Environment.CurrentDirectory = Path.GetDirectoryName(
										Assembly.GetExecutingAssembly().Location);

				logger.Debug("Initialize Waveface Service");
				InitStationId();
				InitResourceBasePath();

				fastJSON.JSON.Instance.UseUTCDateTime = true;

				HttpRequestMonitor httpRequestMonitor = new HttpRequestMonitor();
				functionServer = new HttpServer(9981, httpRequestMonitor); // TODO: remove hard code
				stationTimer = new StationTimer(functionServer);

				functionServer.TaskEnqueue += new EventHandler<TaskQueueEventArgs>(functionServer_TaskEnqueue);

				logger.Debug("Add cloud forwarders to function server");
				BypassHttpHandler cloudForwarder = new BypassHttpHandler(CloudServer.BaseUrl);
				cloudForwarder.AddExceptPrefix("/" + CloudServer.DEF_BASE_PATH + "/auth/");
				cloudForwarder.AddExceptPrefix("/" + CloudServer.DEF_BASE_PATH + "/users/");
				cloudForwarder.AddExceptPrefix("/" + CloudServer.DEF_BASE_PATH + "/groups/");
				cloudForwarder.AddExceptPrefix("/" + CloudServer.DEF_BASE_PATH + "/stations/");
				functionServer.AddDefaultHandler(cloudForwarder);

				logger.Debug("Add handlers to function server");
				functionServer.AddHandler("/", new DummyHandler());
				functionServer.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/attachments/view/",
								new AttachmentViewHandler(stationId));

				AttachmentUploadHandler attachmentHandler = new AttachmentUploadHandler();
				AttachmentUploadMonitor attachmentMonitor = new AttachmentUploadMonitor();
				ImagePostProcessing imgProc = new ImagePostProcessing(new UpstreamThumbnailTaskFactory());
				
				
				attachmentHandler.ImageAttachmentSaved += imgProc.HandleImageAttachmentSaved;
				attachmentHandler.ImageAttachmentCompleted += imgProc.HandleImageAttachmentCompleted;
				attachmentHandler.ThumbnailUpstreamed += attachmentMonitor.OnThumbnailUpstreamed;
				UpstreamThumbnailTask.ThumbnailUpstreamed += attachmentMonitor.OnThumbnailUpstreamed;


				CloudStorageSync cloudSync = new CloudStorageSync();
				attachmentHandler.BodyAttachmentSaved += cloudSync.HandleAttachmentSaved;
				attachmentHandler.ProcessSucceeded += attachmentMonitor.OnProcessSucceeded;
				


				functionServer.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/attachments/upload/",
								attachmentHandler);

				functionServer.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/station/resourceDir/get/",
								new ResouceDirGetHandler(resourceBasePath));

				functionServer.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/station/resourceDir/set/",
								new ResouceDirSetHandler());

				functionServer.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/attachments/get/",
								new AttachmentGetHandler());


				functionServer.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/availability/ping/",
								new PingHandler());
				functionServer.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/reachability/ping/",
								new PingHandler());

				logger.Debug("Start function server");
				functionServer.Start();
				stationTimer.Start();

				logger.Debug("Add handlers to management server");
				managementServer = new HttpServer(9989, httpRequestMonitor);
				AddDriverHandler addDriverHandler = new AddDriverHandler(stationId, resourceBasePath);
				managementServer.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/station/online/", new StationOnlineHandler(functionServer, stationTimer));
				managementServer.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/station/offline/", new StationOfflineHandler(functionServer, stationTimer));
				managementServer.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/station/drivers/add/", addDriverHandler);
				managementServer.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/station/drivers/list/", new ListDriverHandler());
				managementServer.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/station/drivers/remove/", new RemoveOwnerHandler(stationId));
				managementServer.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/station/status/get/", new StatusGetHandler());
				managementServer.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/cloudstorage/list", new ListCloudStorageHandler());
				managementServer.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/cloudstorage/dropbox/oauth/", new DropBoxOAuthHandler());
				managementServer.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/cloudstorage/dropbox/connect/", new DropBoxConnectHandler());
				managementServer.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/cloudstorage/dropbox/update/", new DropBoxUpdateHandler());
				managementServer.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/cloudstorage/dropbox/disconnect/", new DropboxDisconnectHandler());
				managementServer.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/availability/ping/", new PingHandler());

				addDriverHandler.DriverAdded += new EventHandler<DriverAddedEvtArgs>(addDriverHandler_DriverAdded);
				logger.Debug("Start management server");
				managementServer.Start();

				logger.Info("Waveface station is started");
			}
			catch (Exception ex)
			{
				logger.Error("Unknown exception", ex);
				throw;
			}
		}

		void functionServer_TaskEnqueue(object sender, TaskQueueEventArgs e)
		{
			if (e.Handler is AttachmentUploadHandler)
			{
				PerfCounter.GetCounter(PerfCounter.UP_REMAINED_COUNT, false).Increment();
			}			
		}

		#region Private Method
		private void ResetPerformanceCounter()
		{
			PerfCounter.GetCounter(PerfCounter.UP_REMAINED_COUNT, true);
			PerfCounter.GetCounter(PerfCounter.DW_REMAINED_COUNT, true);
			PerfCounter.GetCounter(PerfCounter.UPSTREAM_RATE, true);
			PerfCounter.GetCounter(PerfCounter.DWSTREAM_RATE, true);
		} 
		#endregion

		void addDriverHandler_DriverAdded(object sender, DriverAddedEvtArgs e)
		{
#if !DEBUG
			// client login page refers this value as default account
			StationRegistry.SetValue("driver", e.Driver.email);
#endif


			PublicPortMapping.Instance.DriverAdded(sender, e);
		}

		void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
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
			functionServer.Stop();
			functionServer.Close();

			stationTimer.Stop();
			stationTimer.Close();

			managementServer.Stop();
			managementServer.Close();

			PublicPortMapping.Instance.Close();
		}

		private void InitResourceBasePath()
		{
			resourceBasePath = (string)StationRegistry.GetValue("resourceBasePath", null);
			if (resourceBasePath == null)
			{
				resourceBasePath = "resource";
				if (!Directory.Exists(resourceBasePath))
					Directory.CreateDirectory(resourceBasePath);
			}
		}

		private void InitStationId()
		{
			stationId = (string)StationRegistry.GetValue("stationId", null);

			if (stationId == null)
			{
				stationId = Guid.NewGuid().ToString();
				StationRegistry.SetValue("stationId", stationId);
			}
		}

		private void ConfigThreadPool()
		{
			int minWorker;
			int minIO;

			System.Threading.ThreadPool.GetMinThreads(out minWorker, out minIO);

			minWorker = (int)StationRegistry.GetValue("MinWorkerThreads", minWorker);
			minIO = (int)StationRegistry.GetValue("MinIOThreads", minIO);

			if (minWorker > 0 && minIO > 0)
			{
				System.Threading.ThreadPool.SetMinThreads(minWorker, minIO);
				logger.InfoFormat("Min worker threads {0}, min IO completion threads {1}",
					minWorker, minIO);
			}

			int maxConcurrentTaskCount = (int)StationRegistry.GetValue("MaxConcurrentTaskCount", 6);
			if (maxConcurrentTaskCount > 0)
				TaskQueue.MaxCurrentTaskCount = maxConcurrentTaskCount;
		}

	}


	class DummyHandler : IHttpHandler
	{
		public event EventHandler<HttpHandlerEventArgs> ProcessSucceeded;

		public void HandleRequest(HttpListenerRequest request, HttpListenerResponse response)
		{
			//Debug.Fail("should not reach this code");
		}

		public object Clone()
		{
			return this.MemberwiseClone();
		}

		public void SetBeginTimestamp(long beginTime)
		{
		}


		public void OnTaskEnqueue(EventArgs e)
		{
		}
	}
}
