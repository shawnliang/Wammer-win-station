using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Web;

namespace Wammer.Cloud
{
	public class Station
	{
		private string id;
		private string token;

		public Station(string stationId, string stationToken)
		{
			this.id = stationId;
			this.token = stationToken;
		}

		public static Station SignUp(WebClient agent, string stationId, string userToken)
		{
			Dictionary<object, object> param = new Dictionary<object, object>();
			param.Add("session_token", userToken);
			param.Add("station_id", stationId);
			param.Add("api_key", CloudServer.APIKey);

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
			parameters.Add("session_token", this.token);
			parameters.Add("station_id", this.id);
			parameters.Add("api_key", CloudServer.APIKey);

			StationLogOnResponse res =
				CloudServer.requestPath<StationLogOnResponse>(agent, "stations/logOn", parameters);
			this.token = res.session_token;
		}

		public string Id
		{
			get { return this.id; }
		}

		public string Token
		{
			get { return this.token; }
		}

	}
}
