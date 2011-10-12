using System;
using System.Collections.Generic;
using System.Text;

namespace Wammer.Cloud
{
    public class StationSignUpResponse : CloudResponse
    {
        private StationResponse _station;
        private string _token;

        public StationSignUpResponse(StatusResponse status, string stationToken)
            : base(status)
        {
            this._token = stationToken;
        }

        public StationSignUpResponse()
            :base()
        {
        }

        public string station_token
        {
            get { return _token; }
            set { _token = value; }
        }

        public StationResponse station
        {
            get { return _station; }
            set { _station = value; }
        }
    }

    public class StationLogOnResponse : CloudResponse
    {
        private string _token;

        public StationLogOnResponse()
            :base()
        {
        }

        public StationLogOnResponse(StatusResponse status, string token)
            :base(status)
        {
            _token = token;
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
