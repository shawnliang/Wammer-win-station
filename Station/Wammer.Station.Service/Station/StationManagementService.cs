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

		[OperationContract]
		[WebGet(UriTemplate = "resourceDir/get")]
		Stream GetResourceDir();

		[OperationContract]
		[WebInvoke(Method = "POST", UriTemplate = "resourceDir/set",
			BodyStyle = WebMessageBodyStyle.Bare)]
		Stream SetResourceDir(Stream requestContent);
	}

	[ServiceBehavior(
		InstanceContextMode = InstanceContextMode.Single,
		ConcurrencyMode = ConcurrencyMode.Multiple)]
	public class StationManagementService : IStationManagementService
	{
		private readonly string resourceBasePath;
		private readonly string stationId;
		private log4net.ILog logger = log4net.LogManager.GetLogger(typeof(StationManagementService));

		public StationManagementService(string resourceBasePath, string stationId)
		{
			this.resourceBasePath = resourceBasePath;
			this.stationId = stationId;
		}

		public Stream AddDriver(Stream requestContent)
		{
			NameValueCollection parameters = WCFRestHelper.ParseFormData(requestContent);
			string email = parameters["email"];
			string password = parameters["password"];

			if (email == null || password == null)
				return WCFRestHelper.GenerateErrStream(WebOperationContext.Current,
										HttpStatusCode.BadRequest, -1,
										"parameter email or password is missing or empty");

			if (Drivers.collection.FindOne() != null)
				return WCFRestHelper.GenerateErrStream(WebOperationContext.Current,
					HttpStatusCode.Conflict, (int)StationApiError.DriverExist,
					"already registered");

			using (WebClient agent = new WebClient())
			{
				try
				{
					logger.DebugFormat("login with given driver information, email={0}", email);
					User user = User.LogIn(agent, email, password);

					if (user.Stations != null && user.Stations.Count > 0)
						return AlreadyHasStation(user);

					string baseurl = NetworkHelper.GetBaseURL();
					Dictionary<object, object> location = new Dictionary<object, object>
												{ {"location", baseurl} };

					logger.DebugFormat("cloud signup, stationId={0}, token={1}, location={2}", stationId, user.Token, baseurl);
					Cloud.Station station = Cloud.Station.SignUp(agent, 
																stationId, user.Token, location);
					logger.DebugFormat("cloud logon, session token={0}", station.Token);
					station.LogOn(agent, location);
					Drivers.collection.Save(
						new Drivers {
							email = email,
							folder = Path.Combine(resourceBasePath, user.Groups[0].name),
							user_id = user.Id,
							groups = user.Groups
						}
					);

					StationInfo sinfo = StationInfo.collection.FindOne();
					if (sinfo == null)
					{
						logger.Debug("first add driver, save station information");
						StationInfo.collection.Save(
							new StationInfo
							{
								Id = stationId,
								SessionToken = station.Token,
								Location = baseurl,
								LastLogOn = DateTime.Now
							}
						);
					}
					else
					{
						logger.Debug("update station information");
						sinfo.Location = baseurl;
						sinfo.LastLogOn = DateTime.Now;
						StationInfo.collection.Save(sinfo);
					}
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

			return WCFRestHelper.GenerateSucessStream(WebOperationContext.Current);
		}

		public Stream GetStatus()
		{
			logger.Debug("GetStatus is called");
			StationStatus res = StatusChecker.GetStatus();

			return WCFRestHelper.GenerateSucessStream(WebOperationContext.Current, res);
		}

		public Stream GetResourceDir()
		{
			return WCFRestHelper.GenerateSucessStream(WebOperationContext.Current,
				new
				{
					status = 200,
					api_ret_code = 0,
					api_ret_msg = "success",
					timestamp = DateTime.UtcNow,
					path = Path.IsPathRooted(resourceBasePath)?
							resourceBasePath : Path.GetFullPath(resourceBasePath)
				});
		}
		
		public Stream SetResourceDir(Stream requestContent)
		{
			try
			{
				NameValueCollection parameters = WCFRestHelper.ParseFormData(requestContent);
				string path = parameters["path"];

				if (path == null || !Path.IsPathRooted(path))
				{
					return WCFRestHelper.GenerateErrStream(WebOperationContext.Current,
						HttpStatusCode.BadRequest, new CloudResponse((int)HttpStatusCode.BadRequest,
							(int)StationApiError.BadPath, "path is null or not a full path"));
				}

				StationRegistry.SetValue("resourceBasePath", path);

				return WCFRestHelper.GenerateSucessStream(WebOperationContext.Current);
			}
			catch (Exception e)
			{
				return WCFRestHelper.GenerateErrStream(WebOperationContext.Current,
					HttpStatusCode.InternalServerError, -1, e.Message);
			}
		}

		private static MemoryStream AlreadyHasStation(User user)
		{
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
