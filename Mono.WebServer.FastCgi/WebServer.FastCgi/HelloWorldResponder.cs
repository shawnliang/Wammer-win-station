using System;
using System.Collections.Generic;
using System.Text;
using Mono.FastCgi;

namespace Mono.WebServer.FastCgi
{
    class HelloWorldResponder : MarshalByRefObject, Mono.FastCgi.IResponder
    {
        private ResponderRequest request = null;
        private static string helloWorld =
            "Status: 200 OK\r\n" +
            "Content-Type: text/html; charset=utf-8\r\n" +
            "Connection: close\r\n\r\n" +
            "<html>\r\n" +
            "	<head>\r\n" +
            "		<title>Hellow World</title>\r\n" +
            "	</head>\r\n" +
            "	<body>\r\n" +
            "		<h1>Hello World</h1>\r\n" +
            "	</body>\r\n" +
            "</html>\r\n";
        public HelloWorldResponder(ResponderRequest request)
        {
            this.request = request;
        }

        public ResponderRequest Request
        {
            get { return request; }
        }

        public int Process()
        {
            request.SendOutputText(helloWorld);
            return 0;
        }
    }
}
