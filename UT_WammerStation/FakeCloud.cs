using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace UT_WammerStation
{
    class FakeCloud: IDisposable
    {
        private string requestedPath;
        private string postData;
        private string reqeustedContentType;

        private string response;
        private System.Net.HttpListener listener;

        public FakeCloud(string response)
        {
            this.response = response;
            this.listener = new System.Net.HttpListener();
            this.listener.Prefixes.Add("http://+:80/");

            // If you get an Access Denied exception in Windows 7 or Windows 2008 or later,
            // you might need to reserve a namespace. Run a console window as Admin, and type something like
            //    netsh http add urlacl http://+:80/ user=domain\user
            // See this page:
            //    http://msdn.microsoft.com/en-us/library/cc307223(VS.85).aspx
            this.listener.Start();
            this.listener.BeginGetContext(this.connected, listener);
        }

        public FakeCloud(object response)
        {
            this.response = fastJSON.JSON.Instance.ToJSON(response, false, false, false, false);

            this.listener = new System.Net.HttpListener();
            this.listener.Prefixes.Add("http://+:80/");

            // If you get an Access Denied exception in Windows 7 or Windows 2008 or later,
            // you might need to reserve a namespace. Run a console window as Admin, and type something like
            //    netsh http add urlacl http://+:80/ user=domain\user
            // See this page:
            //    http://msdn.microsoft.com/en-us/library/cc307223(VS.85).aspx
            this.listener.Start();
            this.listener.BeginGetContext(this.connected, listener);
        }

        private void connected(IAsyncResult result)
        {
            System.Net.HttpListener listener = (System.Net.HttpListener)result.AsyncState;
            HttpListenerContext context = listener.EndGetContext(result);
            requestedPath = context.Request.Url.AbsolutePath;
            reqeustedContentType = context.Request.ContentType;
            
            using (StreamReader reader = new StreamReader(context.Request.InputStream))
            {
                postData = reader.ReadToEnd();
            }

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

        public string PostData
        {
            get { return postData; }
        }

        public string RequestedContentType
        {
            get { return reqeustedContentType; }
        }

        public void Dispose()
        {
            this.listener.Close();
        }
    }

    class FakeErrorCloud : IDisposable
    {
        private string requestedPath;
        private int response;
        private System.Net.HttpListener listener;

        public FakeErrorCloud(int errStatus)
        {
            this.response = errStatus;
            this.listener = new System.Net.HttpListener();
            this.listener.Prefixes.Add("http://+:80/");

            // If you get an Access Denied exception in Windows 7 or Windows 2008 or later,
            // you might need to reserve a namespace. Run a console window as Admin, and type something like
            //    netsh http add urlacl http://+:80/ user=domain\user
            // See this page:
            //    http://msdn.microsoft.com/en-us/library/cc307223(VS.85).aspx
            this.listener.Start();
            this.listener.BeginGetContext(this.connected, listener);
        }

        private void connected(IAsyncResult result)
        {
            System.Net.HttpListener listener = (System.Net.HttpListener)result.AsyncState;
            HttpListenerContext context = listener.EndGetContext(result);
            requestedPath = context.Request.Url.AbsolutePath;
            context.Response.StatusCode = this.response;

            context.Response.OutputStream.Close();
        }

        public string RequestedPath
        {
            get { return requestedPath; }
        }

        public void Dispose()
        {
            this.listener.Close();
        }
    }
}
