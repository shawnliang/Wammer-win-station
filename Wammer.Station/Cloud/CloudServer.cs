using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

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

        public static T request<T>(WebClient agent, string url)
        {
            string response = "";
            T resObj;

            try
            {
                response = agent.DownloadString(url);
                resObj = fastJSON.JSON.Instance.ToObject<T>(response);
            }
            catch (WebException e)
            {
                throw new WammerCloudException("Wammer cloud error", e.Status, 0, e);
            }
            catch (Exception e)
            {
                throw new WammerCloudException("Wammer cloud error. response = " + response, e);
            }

            if (resObj is CloudResponse)
            {
                CloudResponse cres = resObj as CloudResponse;
                if (cres.response.status != 200)
                    throw new WammerCloudException("Wammer cloud returns error", WebExceptionStatus.Success, cres.response.status);
            }

            
            return resObj;
        }
    }
}
