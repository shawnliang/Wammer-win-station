using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace Wammer.Cloud
{
    public class WammerCloudException: Exception
    {
        private WebExceptionStatus httpError = WebExceptionStatus.Success;
        private int wammerError = 0;

        public WammerCloudException()
            : base()
        {
        }

        public WammerCloudException(string msg)
            : base(msg)
        {
        }

        public WammerCloudException(string msg, WebExceptionStatus httpError, int wammerError)
            : base(msg)
        {
            this.httpError = httpError;
            this.wammerError = wammerError;
        }

        public WammerCloudException(string msg, Exception innerException)
            : base(msg, innerException)
        {
        }

        public WammerCloudException(string msg, WebExceptionStatus httpError, int wammerError, Exception innerException)
            : base(msg, innerException)
        {
            this.httpError = httpError;
            this.wammerError = wammerError;
        }

        public WebExceptionStatus HttpError
        {
            get { return httpError; }
        }

        public int WammerError
        {
            get { return wammerError; }
        }
    }
}
