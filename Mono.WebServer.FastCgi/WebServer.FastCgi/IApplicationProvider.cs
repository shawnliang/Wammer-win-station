using System;
using System.Collections.Generic;
using System.Text;

namespace Mono.FastCgi
{
    public interface IApplicationProvider
    {
        Mono.WebServer.VPathToHost GetApplicationForPath(string vhost,
            int port, string path, string realPath);
    }
}
