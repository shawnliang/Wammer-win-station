using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Mono.WebServer.FastCgi;
using Mono.FastCgi;
using System.Reflection;
using System.Configuration;

namespace Wammer.Station
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string log_level = ConfigurationManager.AppSettings["log_level"];
                if (log_level!=null)
                    Logger.Level = (LogLevel)Enum.Parse(typeof(LogLevel), log_level);
                else
                    Logger.Level = LogLevel.Standard;

                Logger.WriteToConsole = true;
                
                Logger.Write(LogLevel.Debug,
                    Assembly.GetExecutingAssembly().GetName().Name);

                // Create the socket.
                int port = int.Parse(ConfigurationManager.AppSettings["port"]);
                Socket socket = SocketFactory.CreateTcpSocket(
                                IPAddress.Parse("127.0.0.1"), port);

                Logger.Write(LogLevel.Debug,
                          "Listening on port: 127.0.0.1");
                Logger.Write(LogLevel.Debug,
                          "Listening on address: {0}", port);
                
                string root_dir = Environment.CurrentDirectory;

                WebSource webSource = new WebSource();
                Mono.WebServer.ApplicationServer appserver =
                    new Mono.WebServer.ApplicationServer(webSource, root_dir);
                appserver.Verbose = true;

                appserver.AddApplicationsFromCommandLine("/api:.");


                Logger.Write(LogLevel.Debug, "Root directory: {0}", root_dir);
                Mono.FastCgi.Server server = new Mono.FastCgi.Server(socket,
                    new ApplicationProvider(appserver));

                //server.SetResponder (typeof (Responder));
                server.SetResponder(typeof(Wammer.Station.HelloWorldResponder));

                string max_conn = ConfigurationManager.AppSettings["max_connections"];
                if (max_conn != null)
                    server.MaxConnections = int.Parse(max_conn);
                else
                    server.MaxConnections = 1024;

                string max_req = ConfigurationManager.AppSettings["max_requests"];
                if (max_req != null)
                    server.MaxRequests = int.Parse(max_req);
                else
                    server.MaxRequests = 1024;

                string multiplex = ConfigurationManager.AppSettings["multiplex"];
                if (multiplex != null)
                    server.MultiplexConnections = bool.Parse(multiplex);
                else
                    server.MultiplexConnections = false;

                Logger.Write(LogLevel.Debug, "Max connections: {0}",
                          server.MaxConnections);
                Logger.Write(LogLevel.Debug, "Max requests: {0}",
                          server.MaxRequests);
                Logger.Write(LogLevel.Debug, "Multiplex connections: {0}",
                          server.MultiplexConnections);

                server.Start(true);

                Console.WriteLine(
                    "Hit Return to stop the server.");
                Console.ReadLine();
                server.Stop();
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
