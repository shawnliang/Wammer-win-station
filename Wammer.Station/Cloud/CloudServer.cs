using System;
using System.Collections.Generic;
using System.Text;

namespace Wammer.Cloud
{
    public class CloudServer
    {
        private static string address = "api.waveface.com";
        private static int port = 8080;
        private static string apiKey = "0ffd0a63-65ef-512b-94c7-ab3b33117363";

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

        public static string APIKey
        {
            get { return apiKey; }
            set { apiKey = value; }
        }
    }
}
