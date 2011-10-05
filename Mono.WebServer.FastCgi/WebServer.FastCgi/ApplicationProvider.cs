using System;
using System.Collections.Generic;
using System.Text;
using Mono.WebServer;

namespace Mono.FastCgi
{
    public class ApplicationProvider : IApplicationProvider
    {
        private ApplicationServer appserver;

        public ApplicationProvider(ApplicationServer appserver)
        {
            this.appserver = appserver;
        }

        public VPathToHost GetApplicationForPath(string vhost,
                                                                 int port,
                                                                 string path,
                                                                 string realPath)
        {
            return appserver.GetApplicationForPath(vhost, port, path, false);
        }
    }
}
