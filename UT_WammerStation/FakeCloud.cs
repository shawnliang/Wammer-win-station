using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace UT_WammerStation
{
    class FakeCloud
    {
        private string requestedPath;
        private string response;

        public FakeCloud(string response)
        {
            this.response = response;

            System.Net.HttpListener listener = new System.Net.HttpListener();
            listener.Prefixes.Add("http://+:80/");

            // If you get an Access Denied exception in Windows 7 or Windows 2008 or later,
            // you might need to reserve a namespace. Run a console window as Admin, and type something like
            //    netsh http add urlacl http://+:80/ user=domain\user
            // See this page:
            //    http://msdn.microsoft.com/en-us/library/cc307223(VS.85).aspx
            listener.Start();
            listener.BeginGetContext(this.connected, listener);
        }

        public FakeCloud(object response)
        {
            this.response = fastJSON.JSON.Instance.ToJSON(response, false, false, false, false);

            System.Net.HttpListener listener = new System.Net.HttpListener();
            listener.Prefixes.Add("http://+:80/");

            // If you get an Access Denied exception in Windows 7 or Windows 2008 or later,
            // you might need to reserve a namespace. Run a console window as Admin, and type something like
            //    netsh http add urlacl http://+:80/ user=domain\user
            // See this page:
            //    http://msdn.microsoft.com/en-us/library/cc307223(VS.85).aspx
            listener.Start();
            listener.BeginGetContext(this.connected, listener);
        }

        private void connected(IAsyncResult result)
        {
            System.Net.HttpListener listener = (System.Net.HttpListener)result.AsyncState;
            HttpListenerContext context = listener.EndGetContext(result);
            requestedPath = context.Request.Url.AbsolutePath;
            context.Response.StatusCode = 200;

            using (StreamWriter w = new StreamWriter(context.Response.OutputStream))
            {
                w.Write(this.response);
            }
        }

        public string RequestedPath
        {
            get { return requestedPath; }
        }
    }
}
