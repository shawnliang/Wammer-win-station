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

namespace Wammer.Station.Service
{
	public partial class StationService : ServiceBase
	{
		private static log4net.ILog logger = log4net.LogManager.GetLogger("StationService");
		private HttpServer server;
		private List<WebServiceHost> serviceHosts = new List<WebServiceHost>();
		private string stationId;

		public StationService()
		{
			log4net.Config.XmlConfigurator.Configure();
			InitializeComponent();
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

			fastJSON.JSON.Instance.UseUTCDateTime = true;
			this.stationId = InitStationId();

			server = new HttpServer(9981); // TODO: remove hard code
			BypassHttpHandler cloudForwarder = new BypassHttpHandler(
															CloudServer.HostName, CloudServer.Port);
			cloudForwarder.AddExceptPrefix("/" + CloudServer.DEF_BASE_PATH + "/auth/");
			cloudForwarder.AddExceptPrefix("/" + CloudServer.DEF_BASE_PATH + "/users/");
			cloudForwarder.AddExceptPrefix("/" + CloudServer.DEF_BASE_PATH + "/groups/");
			cloudForwarder.AddExceptPrefix("/" + CloudServer.DEF_BASE_PATH + "/stations/");
			server.AddDefaultHandler(cloudForwarder);

			FileStorage storage = new FileStorage("resource");

			MongoDB.Driver.MongoServer mongodb = MongoDB.Driver.MongoServer.Create(
									string.Format("mongodb://localhost:{0}/?safe=true",
									StationRegistry.GetValue("dbPort", 10319))); // TODO: Remove Hard code

			server.AddHandler("/", new DummyHandler());
			server.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/attachments/view/",
							new ViewObjectHandler(storage, mongodb));

			ObjectUploadHandler attachmentHandler = new ObjectUploadHandler(storage, mongodb);
			ImagePostProcessing imgProc = new ImagePostProcessing(storage);
			attachmentHandler.ImageAttachmentSaved += imgProc.HandleImageAttachmentSaved;
			attachmentHandler.ImageAttachmentCompleted += imgProc.HandleImageAttachmentCompleted;
			server.AddHandler("/" + CloudServer.DEF_BASE_PATH + "/attachments/upload/",
							attachmentHandler);

			server.Start();

			AddWebServiceHost(new AttachmentService(mongodb), 9981, "attachments/");
			AddWebServiceHost(new StationManagementService(mongodb, stationId), 9981, "station/");
		}

		protected override void OnStop()
		{
			foreach (WebServiceHost svc in serviceHosts)
			{
				svc.Close();
			}

			server.Stop();
			server.Close();
		}

		private string InitStationId()
		{
			return (string)StationRegistry.GetValue("stationId", Guid.NewGuid().ToString());
		}

		private void AddWebServiceHost(object service, int port, string basePath)
		{
			string url = string.Format("http://{0}:{1}/{2}/{3}", 
				Dns.GetHostName(), port, CloudServer.DEF_BASE_PATH, basePath);

			WebServiceHost svcHost = new WebServiceHost(service, new Uri(url));
			svcHost.Open();
			serviceHosts.Add(svcHost);
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
			return new DummyHandler();
		}
	}
}
