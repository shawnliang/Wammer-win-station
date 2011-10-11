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
            string address = string.Format(
                "http://{0}:{1}/api/v2/station/signup/user_token/{2}/station_id/{3}/api_key/{4}",
                CloudServer.Address,
                CloudServer.Port,
                HttpUtility.UrlEncode(userToken),
                HttpUtility.UrlEncode(stationId),
                HttpUtility.UrlEncode(CloudServer.APIKey));

            string response = agent.DownloadString(address);
            StationSignUpResponse res = fastJSON.JSON.Instance.ToObject<StationSignUpResponse>(response);

            if (!res.station.station_id.Equals(stationId))
                throw new SystemException("Wammer clound returned a different station id.");

            return new Station(stationId, res.station_token);
        }

        public void LogOn(WebClient agent)
        {
            string address = string.Format(
                "http://{0}:{1}/api/v2/station/logOn/station_token/{2}/station_id/{3}/api_key/{4}",
                CloudServer.Address,
                CloudServer.Port,
                HttpUtility.UrlEncode(this.token),
                HttpUtility.UrlEncode(this.id),
                HttpUtility.UrlEncode(CloudServer.APIKey));

            string response = agent.DownloadString(address);
            StationLogOnResponse res = fastJSON.JSON.Instance.ToObject<StationLogOnResponse>(response);
            this.token = res.station_token;
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
