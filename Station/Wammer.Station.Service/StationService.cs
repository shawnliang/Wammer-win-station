using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.IO;
using System.Reflection;

using MongoDB.Driver.Builders;
using Microsoft.Win32;
using Wammer.Station;
using Wammer.Cloud;
using Wammer.Model;
using TCMPortMapper;

namespace Wammer.Station.Service
{
	public partial class StationService : ServiceBase
	{
		public const string SERVICE_NAME = "WavefaceStation";
		public const string MONGO_SERVICE_NAME = "MongoDbForWaveface";

		private static log4net.ILog logger = log4net.LogManager.GetLogger("StationService");
		private HttpServer server;
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
			AppDomain.CurrentDomain.UnhandledException +=
				new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

			Environment.CurrentDirectory = Path.GetDirectoryName(
									Assembly.GetExecutingAssembly().Location);

			InitStationId();
			InitResourceBasePath();

			fastJSON.JSON.Instance.UseUTCDateTime = true;
			stationTimer = new StationTimer();

			server = new HttpServer(9981); // TODO: remove hard code
			server.OfflineKey = InitOfflineKey();
			BypassHttpHandler cloudForwarder = new BypassHttpHandler(CloudServer.BaseUrl);
			cloudForwarder.AddExceptPrefix("/" + CloudServer.DEF_BASE_PATH + "/auth/");
			cloudForwarder.AddExceptPrefix("/" + CloudServer.DEF_BASE_PATH + "/users/");
			cloudForwarder.AddExceptPrefix("/" + CloudServer.DEF_BASE_PATH + "/groups/");
			cloudForwarder.AddExceptPrefix("/" + CloudServer.DEF_BASE_PATH + "/stations/");
			server.AddDefaultHandler(cloudForwarder);

			server.AddHandler("/", new DummyHandler());
			server.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/attachments/view/",
							new AttachmentViewHandler());

			AttachmentUploadHandler attachmentHandler = new AttachmentUploadHandler();
			ImagePostProcessing imgProc = new ImagePostProcessing();
			attachmentHandler.ImageAttachmentSaved += imgProc.HandleImageAttachmentSaved;
			attachmentHandler.ImageAttachmentCompleted += imgProc.HandleImageAttachmentCompleted;
			
			CloudStorageSync cloudSync = new CloudStorageSync();
			attachmentHandler.AttachmentSaved += cloudSync.HandleAttachmentSaved;

			server.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/attachments/upload/",
							attachmentHandler);

			AddDriverHandler addDriverHandler = new AddDriverHandler(stationId, resourceBasePath);
			server.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/station/drivers/add/",
							addDriverHandler);
			addDriverHandler.DriverAdded += PublicPortMapping.Instance.DriverAdded;

			server.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/station/status/get/",
							new StatusGetHandler());

			server.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/station/resourceDir/get/",
							new ResouceDirGetHandler(resourceBasePath));

			server.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/station/resourceDir/set/",
							new ResouceDirSetHandler());

			server.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/attachments/get/",
							new AttachmentGetHandler());

			server.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/cloudstorage/dropbox/oauth/",
							new DropBoxOAuthHandler());

			server.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/cloudstorage/dropbox/connect/",
							new DropBoxConnectHandler());

			server.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/cloudstorage/dropbox/update/",
							new DropBoxUpdateHandler());

			server.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/cloudstorage/dropbox/disconnect/",
							new DropboxDisconnectHandler());

			server.Start();
		}

		void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			if (e.IsTerminating)
			{
				logger.Fatal("Unhandled exception. Program terminates");
				logger.Fatal(e.ToString());
			}
			else
			{
				logger.Fatal("Unhandled exception" + e.ToString());
			}
		}

		

		protected override void OnStop()
		{
			stationTimer.Stop();

			server.Stop();
			server.Close();

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

		private string InitOfflineKey()
		{
			Model.Service svc = Model.ServiceCollection.FindOne(Query.EQ("_id", "StationService"));
			if (svc == null)
			{
				svc = new Model.Service();
				svc.Id = "StationService";
			}

			svc.OfflineKey = Guid.NewGuid().ToString();
			Model.ServiceCollection.Save(svc);

			return svc.OfflineKey;
		}
	}


	class DummyHandler : IHttpHandler
	{
		public void HandleRequest(HttpListenerRequest request, HttpListenerResponse response)
		{
			Debug.Fail("should not reach this code");
		}

		public object Clone()
		{
			return this.MemberwiseClone();
		}
	}
}
