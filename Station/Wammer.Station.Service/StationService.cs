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
using System.ServiceModel;
using System.ServiceModel.Web;

using Microsoft.Win32;
using Wammer.Station;
using Wammer.Cloud;
using Wammer.Model;

namespace Wammer.Station.Service
{
	public partial class StationService : ServiceBase
	{
		public const string SERVICE_NAME = "WavefaceStation";
		public const string MONGO_SERVICE_NAME = "MongoDbForWaveface";

		private static log4net.ILog logger = log4net.LogManager.GetLogger("StationService");
		private HttpServer server;
		private List<WebServiceHost> serviceHosts = new List<WebServiceHost>();
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
			Environment.CurrentDirectory = Path.GetDirectoryName(
									Assembly.GetExecutingAssembly().Location);

			InitStationId();
			InitResourceBasePath();

			fastJSON.JSON.Instance.UseUTCDateTime = true;
			stationTimer = new StationTimer();

			server = new HttpServer(9981); // TODO: remove hard code
			BypassHttpHandler cloudForwarder = new BypassHttpHandler(CloudServer.BaseUrl);
			cloudForwarder.AddExceptPrefix("/" + CloudServer.DEF_BASE_PATH + "/auth/");
			cloudForwarder.AddExceptPrefix("/" + CloudServer.DEF_BASE_PATH + "/users/");
			cloudForwarder.AddExceptPrefix("/" + CloudServer.DEF_BASE_PATH + "/groups/");
			cloudForwarder.AddExceptPrefix("/" + CloudServer.DEF_BASE_PATH + "/stations/");
			server.AddDefaultHandler(cloudForwarder);

			FileStorage storage = new FileStorage(resourceBasePath);

			server.AddHandler("/", new DummyHandler());
			server.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/attachments/view/",
							new AttachmentViewHandler());

			AttachmentUploadHandler attachmentHandler = new AttachmentUploadHandler();
			ImagePostProcessing imgProc = new ImagePostProcessing();
			attachmentHandler.ImageAttachmentSaved += imgProc.HandleImageAttachmentSaved;
			attachmentHandler.ImageAttachmentCompleted += imgProc.HandleImageAttachmentCompleted;
			server.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/attachments/upload/",
							attachmentHandler);

			server.Start();


			// Start WCF REST services
			AddWebServiceHost(new AttachmentService(), 9981, "attachments/");
			
			StationManagementService statMgmtSvc = new StationManagementService("resource", stationId);
			AddWebServiceHost(statMgmtSvc, 9981, "station/");

			AddWebServiceHost(new CloudStorageService(), 9981, "cloudstorage/");
		}

		protected override void OnStop()
		{
			stationTimer.Stop();

			foreach (WebServiceHost svc in serviceHosts)
			{
				svc.Close();
			}

			server.Stop();
			server.Close();
		}

		private void AddWebServiceHost(object service, int port, string basePath)
		{
			string url = string.Format("http://{0}:{1}/{2}/{3}", 
				Dns.GetHostName(), port, CloudServer.DEF_BASE_PATH, basePath);

			WebServiceHost svcHost = new WebServiceHost(service, new Uri(url));
			svcHost.Open();
			serviceHosts.Add(svcHost);
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
