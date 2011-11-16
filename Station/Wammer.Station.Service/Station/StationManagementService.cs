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
using Wammer.Model;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace Wammer.Station
{
	[ServiceContract]
	public interface IStationManagementService
	{
		[OperationContract]
		[WebInvoke(Method = "POST", UriTemplate = "drivers/add", 
			BodyStyle = WebMessageBodyStyle.Bare)]
		Stream AddDriver(Stream requestContent);

		[OperationContract]
		[WebGet(UriTemplate = "status/get")]
		Stream GetStatus();
	}

	[ServiceBehavior(
		InstanceContextMode = InstanceContextMode.Single,
		ConcurrencyMode = ConcurrencyMode.Multiple)]
	public class StationManagementService : IStationManagementService
	{
		public Stream AddDriver(Stream requestContent)
		{
			NameValueCollection parameters = WCFRestHelper.ParseFormData(requestContent);
			string email = parameters["email"];
			string password = parameters["password"];
			string folder = parameters["folder"];

			if (email == null || password == null || folder == null || folder.Length == 0)
				return WCFRestHelper.GenerateErrStream(WebOperationContext.Current,
										HttpStatusCode.BadRequest, -1,
										"parameter email/password/folder is missing or empty");

			if (!Path.IsPathRooted(folder))
				return WCFRestHelper.GenerateErrStream(WebOperationContext.Current,
										HttpStatusCode.BadRequest, (int)StationApiError.BadPath,
										"folder is not an absolute path");

			if (Drivers.collection.FindOne() != null)
				return WCFRestHelper.GenerateErrStream(WebOperationContext.Current,
					HttpStatusCode.Conflict, (int)StationApiError.DriverExist,
					"already registered");


			using (WebClient agent = new WebClient())
			{
				try
				{
					User user = User.LogIn(agent, email, password);

					if (user.Stations != null && user.Stations.Count > 0)
						return WCFRestHelper.GenerateErrStream(WebOperationContext.Current,
							HttpStatusCode.Conflict,
							new AddUserResponse
							{
								api_ret_code = (int)StationApiError.AlreadyHasStaion,
								api_ret_msg = "already has a station",
								status = (int)HttpStatusCode.Conflict,
								timestamp = DateTime.UtcNow,
								station = user.Stations[0],
								session_token = user.Token
							}
						);

					string baseurl = NetworkHelper.GetBaseURL();
					Dictionary<object, object> location = new Dictionary<object, object>
															{ {"location", baseurl} };

					StationInfo sinfo = StationInfo.collection.FindOne();
					string stationId = (string)StationRegistry.GetValue("stationId", "");
					Cloud.Station station = Cloud.Station.SignUp(agent, 
																stationId, user.Token, location);
					station.LogOn(agent, location);
					Drivers.collection.Save(
						new Drivers {
							email = email,
							folder = folder,
							user_id = user.Id,
							groups = user.Groups
						}
					);
					StationInfo.collection.Save(
						new StationInfo { 
							Id = stationId, 
							SessionToken = station.Token, 
							Location = NetworkHelper.GetBaseURL(), 
							LastLogOn = DateTime.Now
						}
					);
				}
				catch (WammerCloudException ex)
				{
					if (ex.WammerError == 4097)
						return WCFRestHelper.GenerateErrStream(WebOperationContext.Current,
							HttpStatusCode.Unauthorized, (int)StationApiError.AuthFailed,
							"Bad user name or password");
					else
						return WCFRestHelper.GenerateErrStream(WebOperationContext.Current,
							HttpStatusCode.BadRequest, (int)StationApiError.Error,
							"Error logon user: " + ex.ToString());
				}
				catch (Exception ex)
				{
					return WCFRestHelper.GenerateErrStream(WebOperationContext.Current,
							HttpStatusCode.BadRequest, (int)StationApiError.Error,
							"Error logon station: " + ex.ToString());
				}
			}

			return WCFRestHelper.GenerateSucessStream(WebOperationContext.Current,
														new CloudResponse(200, 0, "success"));
		}

		public Stream GetStatus()
		{
			StationStatus res = StatusChecker.GetStatus();

			return WCFRestHelper.GenerateSucessStream(WebOperationContext.Current, res);
		}
	}

	public class AddUserResponse : CloudResponse
	{
		public UserStation station { get; set; }
		public string session_token { get; set; }

		public AddUserResponse()
			:base()
		{
		}
	}
}
