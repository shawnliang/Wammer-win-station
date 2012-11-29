using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Waveface.Stream.Model
{
    public class ConnectToCloudException : Exception
    {
        public ConnectToCloudException(string msg)
            : base(msg)
        {
        }
    }
}
