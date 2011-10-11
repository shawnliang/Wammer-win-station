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

        private Station(string stationId, string stationToken)
        {
            this.id = stationId;
            this.token = stationToken;
        }

        public static Station SignUp(string stationId, string userToken, string apiKey)
        {
            WebClient http = new WebClient();
            string address = string.Format(
                "http://{0}:{1}/api/v2/station/sign_up/user_token/{2}/station_id/{3}/api_key/{4}",
                CloudServer.Address,
                CloudServer.Port,
                HttpUtility.UrlEncode(userToken),
                HttpUtility.UrlEncode(stationId),
                HttpUtility.UrlEncode(apiKey));

            string response = http.DownloadString(address);
            StationSignUpResponse res = fastJSON.JSON.Instance.ToObject<StationSignUpResponse>(response);
            return new Station(stationId, res.stationToken);
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


    public class StationSignUpResponse
    {
        private int _status;
        private string _token;

        public StationSignUpResponse(int status, string stationToken)
        {
            this._status = status;
            this._token = stationToken;
        }

        public StationSignUpResponse()
        {
        }

        public string stationToken
        {
            get { return _token; }
            set { _token = value; }
        }

        public int status
        {
            get { return _status; }
            set { _status = value; }
        }
    }
}
