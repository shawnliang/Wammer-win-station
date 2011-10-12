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


            StationSignUpResponse res = CloudServer.request<StationSignUpResponse>(agent, address);

            if (!res.station.station_id.Equals(stationId))
                throw new WammerCloudException("Wammer clound returned a different station id.");

            return new Station(stationId, res.station_token);
        }

        public void LogOn(WebClient agent)
        {
            this.LogOn(agent, new Dictionary<object, object>());
        }

        public void LogOn(WebClient agent, Dictionary<object, object>param)
        {
            string address = string.Format(
                "http://{0}:{1}/api/v2/station/logOn/station_token/{2}/station_id/{3}/api_key/{4}",
                CloudServer.Address,
                CloudServer.Port,
                HttpUtility.UrlEncode(this.token),
                HttpUtility.UrlEncode(this.id),
                HttpUtility.UrlEncode(CloudServer.APIKey));

            StringBuilder strBuf = new StringBuilder(address);
            foreach (KeyValuePair<object, object> pair in param)
            {
                strBuf.Append("/");
                strBuf.Append(HttpUtility.UrlEncode(pair.Key.ToString()));
                strBuf.Append("/");
                strBuf.Append(HttpUtility.UrlEncode(pair.Value.ToString()));
            }

            StationLogOnResponse res = CloudServer.request<StationLogOnResponse>(agent, strBuf.ToString());
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
