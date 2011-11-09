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
using Wammer.Utility;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

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
		private MongoCollection<StationDriver> drivers;
		public StationManagementService(MongoServer mongodb, string stationId)
		{
			this.mongodb = mongodb;
			this.stationId = stationId;

			if (!mongodb.GetDatabase("wammer").CollectionExists("drivers"))
				mongodb.GetDatabase("wammer").CreateCollection("drivers");

			this.drivers = mongodb.GetDatabase("wammer").GetCollection<StationDriver>("drivers");
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

				if (email==null || password==null || folder==null || folder.Length==0)
					return WCFRestHelper.GenerateErrStream(WebOperationContext.Current,
											HttpStatusCode.BadRequest, -1, 
											"parameter email/password/folder is missing or empty");

				if (!Path.IsPathRooted(folder))
					return WCFRestHelper.GenerateErrStream(WebOperationContext.Current,
											HttpStatusCode.BadRequest, (int)StationApiError.BadPath,
											"folder is not an absolute path");

				if (drivers.FindOne(Query.EQ("email", email)) != null)
					return WCFRestHelper.GenerateErrStream(WebOperationContext.Current, 
						HttpStatusCode.Conflict, (int)StationApiError.DriverExist,
						"already registered");


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

					drivers.Insert(driver);
				}

				return WCFRestHelper.GenerateSucessStream(new CloudResponse(200, 0, "success"));
			}
		}
	}
}
