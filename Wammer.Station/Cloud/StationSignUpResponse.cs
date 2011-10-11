using System;
using System.Collections.Generic;
using System.Text;

namespace Wammer.Cloud
{
    public class StationSignUpResponse
    {
        private StatusResponse _status;
        private StationResponse _station;
        private string _token;

        public StationSignUpResponse(StatusResponse status, string stationToken)
        {
            this._status = status;
            this._token = stationToken;
        }

        public StationSignUpResponse()
        {
        }

        public string station_token
        {
            get { return _token; }
            set { _token = value; }
        }

        public StatusResponse response
        {
            get { return _status; }
            set { _status = value; }
        }

        public StationResponse station
        {
            get { return _station; }
            set { _station = value; }
        }
    }

    public class StationLogOnResponse
    {
        private StatusResponse _status;
        private string _token;

        public StationLogOnResponse()
        {
        }

        public StationLogOnResponse(StatusResponse status, string token)
        {
            _status = status;
            _token = token;
        }

        public StatusResponse response
        {
            get { return _status; }
            set { _status = value; }
        }

        public string station_token
        {
            get { return _token; }
            set { _token = value; }
        }
    }

    public class StationResponse
    {
        private string _stationId;
        private string _creatorId;
        //private DateTime _timestamp;
        //private string[] _groups;
        //private string _name;

        public StationResponse()
        {
        }

        public string station_id
        {
            get { return _stationId; }
            set { _stationId = value; }
        }

        public string creator_id
        {
            get { return _creatorId; }
            set { _creatorId = value; }
        }
    }
}
