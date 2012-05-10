using System.Collections.Generic;
using System.Net;

using Wammer.Utility;

namespace Wammer.Cloud
{
	public class StationApi
	{

		public StationApi(string stationId, string stationToken)
		{
			this.Id = stationId;
			this.Token = stationToken;
		}

		public static StationSignUpResponse SignUpBySession(WebClient agent, string sessionToken, string stationId)
		{
			Dictionary<object, object> param = new Dictionary<object, object>
			{
				{CloudServer.PARAM_STATION_ID, stationId},
				{CloudServer.PARAM_API_KEY, CloudServer.APIKey},
				{CloudServer.PARAM_SESSION_TOKEN, sessionToken}
			};

			StationSignUpResponse res = CloudServer.requestPath<StationSignUpResponse>(agent, "stations/signup", param, false);
			return res;
		}

		public static StationSignUpResponse SignUpByEmailPassword(WebClient agent, string stationId, string email, string passwd, string deviceId, string deviceName)
		{
			Dictionary<object, object> param = new Dictionary<object, object>
			{
				{CloudServer.PARAM_EMAIL, email},
				{CloudServer.PARAM_PASSWORD, passwd},
				{CloudServer.PARAM_DEVICE_ID, deviceId},
				{CloudServer.PARAM_DEVICE_NAME, deviceName},
				{CloudServer.PARAM_STATION_ID, stationId},
				{CloudServer.PARAM_API_KEY, CloudServer.APIKey}
			};

			StationSignUpResponse res = CloudServer.requestPath<StationSignUpResponse>(agent, "stations/signup", param, false);
			return res;
		}

		public void LogOn(WebClient agent)
		{
			this.LogOn(agent, new Dictionary<object, object>());
		}
		
		public StationLogOnResponse LogOn(WebClient agent, StationDetail detail)
		{
			return LogOn(agent, detail, CloudServer.APIKey);
		}

		public StationLogOnResponse LogOn(WebClient agent, StationDetail detail, string apiKey)
		{
			Dictionary<object, object> parameters = new Dictionary<object, object>
			{
				{ CloudServer.PARAM_SESSION_TOKEN, this.Token },
				{ CloudServer.PARAM_STATION_ID, this.Id },
				{ CloudServer.PARAM_API_KEY, apiKey },
				{ CloudServer.PARAM_DETAIL, detail.ToFastJSON() }
			};

			StationLogOnResponse res =
				CloudServer.requestPath<StationLogOnResponse>(agent, "stations/logOn", parameters, false);
			this.Token = res.session_token;
			return res;
		}

		public StationLogOnResponse LogOn(WebClient agent, Dictionary<object, object> param)
		{
			Dictionary<object, object> parameters = new Dictionary<object, object>(param);
			parameters.Add(CloudServer.PARAM_SESSION_TOKEN, this.Token);
			parameters.Add(CloudServer.PARAM_STATION_ID, this.Id);
			parameters.Add(CloudServer.PARAM_API_KEY, CloudServer.APIKey);

			StationLogOnResponse res =
				CloudServer.requestPath<StationLogOnResponse>(agent, "stations/logOn", parameters, false);
			this.Token = res.session_token;

			return res;
		}
		
		public StationHeartbeatResponse Heartbeat(WebClient agent, StationDetail detail)
		{
			Dictionary<object, object> parameters = new Dictionary<object, object>
			{
				{ CloudServer.PARAM_SESSION_TOKEN, this.Token },
				{ CloudServer.PARAM_STATION_ID, this.Id },
				{ CloudServer.PARAM_API_KEY, CloudServer.APIKey },
				{ CloudServer.PARAM_DETAIL, detail.ToFastJSON() }
			};

			StationHeartbeatResponse res = CloudServer.requestPath<StationHeartbeatResponse>(agent, "stations/heartbeat", parameters);
			return res;
		}

		public static void SignOff(WebClient agent, string stationId, string sessionToken, string userID)
		{
			Dictionary<object, object> parameters = new Dictionary<object, object>
			{
				{ CloudServer.PARAM_STATION_ID, stationId },
				{ CloudServer.PARAM_SESSION_TOKEN, sessionToken },
				{ CloudServer.PARAM_API_KEY, CloudServer.APIKey },
                { CloudServer.PARAM_USER_ID, userID }
			};

			CloudServer.requestPath<CloudResponse>(agent, "stations/signoff", parameters, false);
		}

        public static void SignOff(WebClient agent, string stationId, string sessionToken)
        {
            Dictionary<object, object> parameters = new Dictionary<object, object>
			{
				{ CloudServer.PARAM_STATION_ID, stationId },
				{ CloudServer.PARAM_SESSION_TOKEN, sessionToken },
				{ CloudServer.PARAM_API_KEY, CloudServer.APIKey }
			};

            CloudServer.requestPath<CloudResponse>(agent, "stations/signoff", parameters, false);
        }

		public void Offline(WebClient agent)
		{
			Dictionary<object, object> parameters = new Dictionary<object, object>
			{
				{ CloudServer.PARAM_STATION_ID, this.Id },
				{ CloudServer.PARAM_SESSION_TOKEN, this.Token },
				{ CloudServer.PARAM_API_KEY, CloudServer.APIKey }
			};

			CloudServer.requestPath<StationHeartbeatResponse>(agent, "stations/offline", parameters);		
		}

		public string Id { get; private set;}
		public string Token { get; private set;}

	}


	public class StationDetail
	{
		public string location { get; set; }
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
