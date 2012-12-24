using System.Collections.Generic;
using Wammer.Utility;
using Waveface.Stream.Model;

namespace Wammer.Cloud
{
	public class StationApi
	{
		public StationApi(string stationId, string stationToken)
		{
			Id = stationId;
			Token = stationToken;
		}

		public string Id { get; private set; }
		public string Token { get; private set; }

		public static StationSignUpResponse SignUpBySession(string sessionToken, string stationId, StationDetail detail)
		{
			var param = new Dictionary<object, object>
			            	{
			            		{CloudServer.PARAM_STATION_ID, stationId},
			            		{CloudServer.PARAM_API_KEY, CloudServer.APIKey},
			            		{CloudServer.PARAM_SESSION_TOKEN, sessionToken},
			            		{CloudServer.PARAM_DETAIL, detail.ToFastJSON()}
			            	};

			var res = CloudServer.requestPath<StationSignUpResponse>("stations/signup", param, false);
			return res;
		}

		public static StationSignUpResponse SignUpByEmailPassword(string stationId, string email,
																  string passwd, string deviceId, string deviceName, StationDetail detail)
		{
			var param = new Dictionary<object, object>
			            	{
			            		{CloudServer.PARAM_EMAIL, email},
			            		{CloudServer.PARAM_PASSWORD, passwd},
			            		{CloudServer.PARAM_DEVICE_ID, deviceId},
			            		{CloudServer.PARAM_DEVICE_NAME, deviceName},
			            		{CloudServer.PARAM_STATION_ID, stationId},
			            		{CloudServer.PARAM_API_KEY, CloudServer.APIKey},
								{CloudServer.PARAM_DETAIL, detail.ToFastJSON()}
			            	};

			return CloudServer.requestPath<StationSignUpResponse>("stations/signup", param, false);
		}

		public void LogOn()
		{
			LogOn(new Dictionary<object, object>());
		}

		public StationLogOnResponse LogOn(StationDetail detail)
		{
			return LogOn(detail, CloudServer.APIKey);
		}

		public StationLogOnResponse LogOn(StationDetail detail, string apiKey)
		{
			var parameters = new Dictionary<object, object>
			                 	{
			                 		{CloudServer.PARAM_SESSION_TOKEN, Token},
			                 		{CloudServer.PARAM_STATION_ID, Id},
			                 		{CloudServer.PARAM_API_KEY, apiKey},
			                 		{CloudServer.PARAM_DETAIL, detail.ToFastJSON()}
			                 	};

			var res =
				CloudServer.requestPath<StationLogOnResponse>("stations/logOn", parameters, false);
			Token = res.session_token;
			return res;
		}

		public StationLogOnResponse LogOn(Dictionary<object, object> param)
		{
			var parameters = new Dictionary<object, object>(param)
			                 	{
			                 		{CloudServer.PARAM_SESSION_TOKEN, Token},
			                 		{CloudServer.PARAM_STATION_ID, Id},
			                 		{CloudServer.PARAM_API_KEY, CloudServer.APIKey}
			                 	};

			var res =
				CloudServer.requestPath<StationLogOnResponse>("stations/logOn", parameters, false);
			Token = res.session_token;

			return res;
		}

		public StationHeartbeatResponse Heartbeat(StationDetail detail)
		{
			var parameters = new Dictionary<object, object>
			                 	{
			                 		{CloudServer.PARAM_SESSION_TOKEN, Token},
			                 		{CloudServer.PARAM_STATION_ID, Id},
			                 		{CloudServer.PARAM_API_KEY, CloudServer.APIKey},
			                 		{CloudServer.PARAM_DETAIL, detail.ToFastJSON()}
			                 	};

			var res = CloudServer.requestPath<StationHeartbeatResponse>("stations/heartbeat", parameters);
			return res;
		}

		public static void SignOff(string stationId, string sessionToken, string userID)
		{
			var parameters = new Dictionary<object, object>
			                 	{
			                 		{CloudServer.PARAM_STATION_ID, stationId},
			                 		{CloudServer.PARAM_SESSION_TOKEN, sessionToken},
			                 		{CloudServer.PARAM_API_KEY, CloudServer.APIKey},
			                 		{CloudServer.PARAM_USER_ID, userID}
			                 	};

			CloudServer.requestPath<CloudResponse>("stations/signoff", parameters, false);
		}

		public static void SignOff(string stationId, string sessionToken)
		{
			var parameters = new Dictionary<object, object>
			                 	{
			                 		{CloudServer.PARAM_STATION_ID, stationId},
			                 		{CloudServer.PARAM_SESSION_TOKEN, sessionToken},
			                 		{CloudServer.PARAM_API_KEY, CloudServer.APIKey}
			                 	};

			CloudServer.requestPath<CloudResponse>("stations/signoff", parameters, false);
		}

		public void Offline()
		{
			var parameters = new Dictionary<object, object>
			                 	{
			                 		{CloudServer.PARAM_STATION_ID, Id},
			                 		{CloudServer.PARAM_SESSION_TOKEN, Token},
			                 		{CloudServer.PARAM_API_KEY, CloudServer.APIKey}
			                 	};

			CloudServer.requestPath<StationHeartbeatResponse>("stations/offline", parameters);
		}
	}


	public class StationDetail
	{
		public string location { get; set; }
		public string ws_location { get; set; }
		public List<DiskUsage> diskusage { get; set; }
		public UPnPInfo upnp { get; set; }
		public string computer_name { get; set; }
		public string version { get; set; }
	}

	public class DiskUsage
	{
		public string group_id { get; set; }
		public long used { get; set; }
		public long avail { get; set; }
	}

	public class UPnPInfo
	{
		public bool status { get; set; }
		public string public_addr { get; set; }
		public int public_port { get; set; }
	}
}