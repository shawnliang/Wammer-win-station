using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Web;

namespace Wammer.Cloud
{
	public class Station
	{

		public Station(string stationId, string stationToken)
		{
			this.Id = stationId;
			this.Token = stationToken;
		}

		public static Station SignUp(WebClient agent, string stationId, string sessionToken)
		{
			Dictionary<object, object> param = new Dictionary<object, object>();
			param.Add(CloudServer.PARAM_SESSION_TOKEN, sessionToken);
			param.Add(CloudServer.PARAM_STATION_ID, stationId);
			param.Add(CloudServer.PARAM_API_KEY, CloudServer.APIKey);

			StationSignUpResponse res =
				CloudServer.requestPath<StationSignUpResponse>(agent, "stations/signup", param);

			return new Station(stationId, res.session_token);
		}

		public static Station SignUp(WebClient agent, string stationId, string sessionToken, 
			Dictionary<object, object> optionals)
		{
			Dictionary<object, object> param = new Dictionary<object, object>(optionals);
			param.Add(CloudServer.PARAM_SESSION_TOKEN, sessionToken);
			param.Add(CloudServer.PARAM_STATION_ID, stationId);
			param.Add(CloudServer.PARAM_API_KEY, CloudServer.APIKey);
			

			StationSignUpResponse res =
				CloudServer.requestPath<StationSignUpResponse>(agent, "stations/signup", param);

			return new Station(stationId, res.session_token);
		}

		public void LogOn(WebClient agent)
		{
			this.LogOn(agent, new Dictionary<object, object>());
		}

		public void LogOn(WebClient agent, Dictionary<object, object> param)
		{
			Dictionary<object, object> parameters = new Dictionary<object, object>(param);
			parameters.Add(CloudServer.PARAM_SESSION_TOKEN, this.Token);
			parameters.Add(CloudServer.PARAM_STATION_ID, this.Id);
			parameters.Add(CloudServer.PARAM_API_KEY, CloudServer.APIKey);

			StationLogOnResponse res =
				CloudServer.requestPath<StationLogOnResponse>(agent, "stations/logOn", parameters);
			this.Token = res.session_token;
		}

		public void Heartbeat(WebClient agent, Dictionary<object, object> param)
		{
			Dictionary<object, object> parameters = new Dictionary<object, object>(param);
			parameters.Add(CloudServer.PARAM_SESSION_TOKEN, this.Token);
			parameters.Add(CloudServer.PARAM_STATION_ID, this.Id);
			parameters.Add(CloudServer.PARAM_API_KEY, CloudServer.APIKey);

			StationHeartbeatResponse res =
				CloudServer.requestPath<StationHeartbeatResponse>(agent, "stations/heartbeat", parameters);
		}

		public string Id { get; private set;}

		public string Token { get; private set;}

	}
}
