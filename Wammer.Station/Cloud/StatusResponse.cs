using System;
using System.Collections.Generic;
using System.Text;

namespace Wammer.Cloud
{
    public class StatusResponse
    {
        private int _status;
        private DateTime _timestamp;

        public StatusResponse()
        {
        }

        public StatusResponse(int status, DateTime timestamp)
        {
            _status = status;
            _timestamp = timestamp;
        }

        public int status
        {
            get { return _status; }
            set { _status = value; }
        }

        public DateTime timestamp
        {
            get { return _timestamp;}
            set { _timestamp = value; }
        }
    }
}
