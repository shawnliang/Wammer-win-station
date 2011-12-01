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
					string baseurl = NetworkHelper.GetBaseURL();
					Cloud.Station station = Cloud.Station.SignUp(
														agent, stationId, email, password, baseurl);

					User user = User.LogIn(agent, email, password);
					Drivers.collection.Save(
						new Drivers {
							email = email,
							folder = Path.Combine(resourceBasePath, user.Groups[0].name),
							user_id = user.Id,
							groups = user.Groups,
							session_token = user.Token
						}
					);

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
				catch (WammerCloudException ex)
				{
					logger.WarnFormat("Unable to add user {0} to station", email);
					logger.Warn(ex.ToString());

					if (ex.HttpError != WebExceptionStatus.ProtocolError)
					{
						return WCFRestHelper.GenerateErrStream(WebOperationContext.Current,
							HttpStatusCode.ServiceUnavailable, (int)StationApiError.ConnectToCloudError,
							"Unable to connect to waveface cloud. Network error?");
					}
					if (ex.WammerError == 4097)
						return WCFRestHelper.GenerateErrStream(WebOperationContext.Current,
							HttpStatusCode.Unauthorized, (int)StationApiError.AuthFailed,
							"Bad user name or password");
					else if (ex.WammerError == 16387)
					{
						return AlreadyHasStation(ex.response);
					}
					else
						return WCFRestHelper.GenerateErrStream(WebOperationContext.Current,
							HttpStatusCode.BadRequest, (int)StationApiError.Error,
							"Error add user: " + ex.ToString());
				}
				catch (Exception ex)
				{
					logger.WarnFormat("Unable to add user {0} to station", email);
					logger.Warn(ex.ToString());

					return WCFRestHelper.GenerateErrStream(WebOperationContext.Current,
							HttpStatusCode.BadRequest, (int)StationApiError.Error,
							"Error add station: " + ex.ToString());
				}
			}

			logger.InfoFormat("user {0} is added to this station successfully", email);
			return WCFRestHelper.GenerateSucessStream(WebOperationContext.Current);
		}

		public Stream GetStatus()
		{
			logger.Debug("GetStatus is called");
			try
			{
				StationStatus res = StatusChecker.GetStatus();

				return WCFRestHelper.GenerateSucessStream(WebOperationContext.Current, new GetStatusResponse
				{
					api_ret_code = 0,
					api_ret_msg = "success",
					status = 200,
					timestamp = DateTime.UtcNow,
					station_status = res
				});
			}
			catch (Exception e)
			{
				logger.Error("Error in GetStatus", e);
				return WCFRestHelper.GenerateErrStream(WebOperationContext.Current, HttpStatusCode.BadRequest, -1, e.Message);
			}
		}

		public Stream GetResourceDir()
		{
			try
			{
				return WCFRestHelper.GenerateSucessStream(WebOperationContext.Current,
					new GetResourceDirResponse
					{
						status = 200,
						api_ret_code = 0,
						api_ret_msg = "success",
						timestamp = DateTime.UtcNow,
						path = Path.IsPathRooted(resourceBasePath) ?
								resourceBasePath : Path.GetFullPath(resourceBasePath)
					});
			}
			catch (Exception e)
			{
				logger.Error("Error in GetResourceDir", e);
				return WCFRestHelper.GenerateErrStream(WebOperationContext.Current, HttpStatusCode.BadRequest, -1, e.Message);
			}
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
				logger.Error("Error in SetResourceDir", e);
				return WCFRestHelper.GenerateErrStream(WebOperationContext.Current,
					HttpStatusCode.InternalServerError, -1, e.Message);
			}
		}

		private static MemoryStream AlreadyHasStation(string responseJson)
		{
			AddUserResponse resp = fastJSON.JSON.Instance.ToObject<AddUserResponse>(responseJson);
			return WCFRestHelper.GenerateErrStream(WebOperationContext.Current,
										HttpStatusCode.Conflict,
										new AddUserResponse
										{
											api_ret_code = (int)StationApiError.AlreadyHasStaion,
											api_ret_msg = "already has a station",
											status = (int)HttpStatusCode.Conflict,
											timestamp = DateTime.UtcNow,
											station = resp.station
										}
									);
		}
	}

	public class GetResourceDirResponse : CloudResponse
	{
		public string path { get; set; }

		public GetResourceDirResponse()
			:base()
		{
		}
	}

	public class GetStatusResponse : CloudResponse
	{
		public StationStatus station_status { get; set; }
		
		public GetStatusResponse()
			:base()
		{
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
