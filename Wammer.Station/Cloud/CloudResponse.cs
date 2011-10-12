using System;
using System.Collections.Generic;
using System.Text;

namespace Wammer.Cloud
{
    public abstract class CloudResponse
    {
        private StatusResponse _status;

        protected CloudResponse()
        {
        }

        protected CloudResponse(StatusResponse status)
        {
            this._status = status;
        }

        public StatusResponse response
        {
            get { return _status; }
            set { _status = value; }
        }
    }
}
