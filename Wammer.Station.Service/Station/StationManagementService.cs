using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.IO;
using System.Net;
using System.Collections.Specialized;
using Wammer.Cloud;
using MongoDB.Driver;

namespace Wammer.Station
{
	[ServiceContract]
	public interface IStationManagementService
	{
		[OperationContract]
		[WebInvoke(Method = "POST", UriTemplate = "station/drivers/add", 
			BodyStyle = WebMessageBodyStyle.Bare)]
		Stream AddDriver(Stream requestContent);
	}

	[ServiceBehavior(
		InstanceContextMode = InstanceContextMode.Single,
		ConcurrencyMode = ConcurrencyMode.Multiple)]
	public class StationManagementService : IStationManagementService
	{
		private MongoServer mongodb;
		private string stationId;

		public StationManagementService(MongoServer mongodb, string stationId)
		{
			this.mongodb = mongodb;
			this.stationId = stationId;
		}

		public Stream AddDriver(Stream requestContent)
		{
			using (StreamReader r = new StreamReader(requestContent))
			{
				string requestText = r.ReadToEnd();
				NameValueCollection parameters = HttpUtility.ParseQueryString(requestText);

				string email = parameters["email"];
				string password = parameters["password"];
				string folder = parameters["folder"];

				using (WebClient agent = new WebClient())
				{
					User user = User.LogIn(agent, email, password);
					Cloud.Station station = Cloud.Station.SignUp(agent, stationId, user.Token);
					station.LogOn(agent);


					StationDriver driver = new StationDriver
					{
						email = email,
						folder = folder,
						user_id = user.Id,
						groups = user.Groups
					};

					mongodb.GetDatabase("wammer").GetCollection<StationDriver>("drivers")
						.Insert(driver);
				}


				CloudResponse res = new CloudResponse(200, 0, "success");
				MemoryStream m = new MemoryStream();
				StreamWriter w = new StreamWriter(m);
				w.Write(fastJSON.JSON.Instance.ToJSON(res));
				w.Flush();
				m.Position = 0;
				return m;
			}
		}
	}
}
