using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.IO;

namespace UT_WammerStation
{
    [TestClass]
    public class TestUserSingOn
    {
        string AbsPath;

        private void connected(IAsyncResult result)
        {
            System.Net.HttpListener listener = (System.Net.HttpListener)result.AsyncState;
            HttpListenerContext context = listener.EndGetContext(result);
            AbsPath = context.Request.Url.AbsolutePath;
            context.Response.StatusCode = 200;

            using (StreamWriter w = new StreamWriter(context.Response.OutputStream))
            {
                w.WriteLine("{ \"status\": 200, \"uid\": \"id1\", \"userToken\": \"token1\" }");
            }
        }

        [TestMethod]
        public void TestSignOn()
        {
            System.Net.HttpListener listener = new System.Net.HttpListener();
            listener.Prefixes.Add("http://+:80/");

            // If you get an Access Denied exception in Windows 7 or Windows 2008 or later, 
            // you might need to reserve a namespace. Run a console window as Admin, and type something like
            //    netsh http add urlacl http://+:80/ user=domain\user
            // See this page: 
            //    http://msdn.microsoft.com/en-us/library/cc307223(VS.85).aspx
            listener.Start();
            listener.BeginGetContext(this.connected, listener);

            Wammer.Cloud.User user = Wammer.Cloud.User.SignIn("user1", "passwd1", "apiKey1");
            Assert.AreEqual("user1", user.Name);
            Assert.AreEqual("passwd1", user.Password);
            Assert.AreEqual("/api/v2/auth/login/email/user1/password/passwd1/apiKey/apiKey1", AbsPath);
            Assert.AreEqual("token1", user.Token);
            Assert.AreEqual("id1", user.Id);
        }
    }
}
