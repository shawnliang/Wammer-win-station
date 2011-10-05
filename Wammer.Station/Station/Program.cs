using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Mono.WebServer.FastCgi;
using Mono.FastCgi;
using System.Reflection;

namespace Wammer.Station
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Logger.Level = LogLevel.All;
                Logger.WriteToConsole = true;
                Logger.Write(LogLevel.Debug,
                    Assembly.GetExecutingAssembly().GetName().Name);

                // Create the socket.
                Socket socket = SocketFactory.CreateTcpSocket(
                                IPAddress.Parse("127.0.0.1"), 8888);

                Logger.Write(LogLevel.Debug,
                          "Listening on port: 127.0.0.1");
                Logger.Write(LogLevel.Debug,
                          "Listening on address: 8888");

                string root_dir = Environment.CurrentDirectory;
                bool auto_map = false;

                WebSource webSource = new WebSource();
                Mono.WebServer.ApplicationServer appserver =
                    new Mono.WebServer.ApplicationServer(webSource, root_dir);
                appserver.Verbose = true;

                appserver.AddApplicationsFromCommandLine("/api:.");


                Logger.Write(LogLevel.Debug, "Root directory: {0}", root_dir);
                Mono.FastCgi.Server server = new Mono.FastCgi.Server(socket);

                //server.SetResponder (typeof (Responder));
                server.SetResponder(typeof(HelloWorldResponder));

                server.MaxConnections = 1024;
                server.MaxRequests = 1024;
                server.MultiplexConnections = true;

                Logger.Write(LogLevel.Debug, "Max connections: {0}",
                          server.MaxConnections);
                Logger.Write(LogLevel.Debug, "Max requests: {0}",
                          server.MaxRequests);
                Logger.Write(LogLevel.Debug, "Multiplex connections: {0}",
                          server.MultiplexConnections);

                Logger.WriteToConsole = true;
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
