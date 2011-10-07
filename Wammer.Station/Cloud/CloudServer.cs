using System;
using System.Collections.Generic;
using System.Text;

namespace Wammer.Cloud
{
    public class CloudServer
    {
        private static string address = "api.waveface.com";
        private static int port = 8080;

        public static string Address
        {
            get { return address; }
            set { address = value; }
        }

        public static int Port
        {
            get { return port; }
            set { port = value; }
        }
    }
}
