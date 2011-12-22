using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.IO;
using System.Threading;
using Wammer.Utility;
using Wammer.Cloud;

namespace TestUploadImage
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Arguments arguments = new Arguments(args);

                string image_file = arguments["--image-file"];
                int threads = (arguments["--threads"] != null) ? int.Parse(arguments["--threads"]) : 1;
                int uploads_per_thread = (arguments["--uploads-per-thread"] != null) ? int.Parse(arguments["--uploads-per-thread"]) : 100;

                string email = arguments["--email"];
                string password = arguments["--password"];
                string station = arguments["--station"];


                if (image_file == null)
                    throw new ArgumentException("--image-file is not specified");
                if (email == null)
                    throw new ArgumentException("--email is not specified");
                if (password == null)
                    throw new ArgumentException("--password is not specified");

                if (station == null)
                    station = "127.0.0.1";

                User user = User.LogIn(new WebClient(), email, password);

                byte[] image_data = LoadImage(image_file);

                ManualResetEvent start_event = new ManualResetEvent(false);


                // Start threads to upload images
                List<UploadThread> workers = new List<UploadThread>();
                for (int i = 0; i < threads; i++)
                {
                    UploadThread thread = new UploadThread(uploads_per_thread, image_data, start_event, user.Token, user.Groups[0].group_id, station);
                    workers.Add(thread);
                    thread.Start();
                }

                start_event.Set();


                // Waiting upload complete
                for (int i = 0; i < threads; i++)
                {
                    workers[i].Join();
                }



                
                // Compute statistics
                int total_success = 0;
                int total_error = 0;
                TimeSpan total_duration_of_success = TimeSpan.FromTicks(0);

                for (int i = 0; i < threads; i++)
                {
                    int success_count = workers[i].success_count;
                    int error_count = workers[i].error_count;
                    TimeSpan success_duration = workers[i].total_duration;

                    string text = string.Format("Worker {0}: success {1}, success duration {2}, failure {3}",
                        i, success_count, success_duration, error_count);

                    Console.WriteLine(text);

                    total_success += success_count;
                    total_error += error_count;
                    total_duration_of_success += success_duration;
                }

                Console.WriteLine("===================================");
                Console.WriteLine("total success: " + total_success);
                Console.WriteLine("total error: " + total_error);
                Console.WriteLine("total duration of success: " + total_duration_of_success.TotalMilliseconds + " ms");

                Console.WriteLine("Average duration of a success upload: " + total_duration_of_success.TotalMilliseconds / total_success);
                Console.WriteLine("Throughput : " + total_success * 1000.0 / total_duration_of_success.TotalMilliseconds + "uploads/sec");
            }
            catch (ArgumentException e)
            {
                PrintUsage();
                Console.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                PrintUsage();
                Console.WriteLine(e.ToString());
            }
        }


        static void PrintUsage()
        {
            Console.WriteLine("TestUploadImage usages:");
            Console.WriteLine();
            Console.WriteLine("--image-file path        image file to upload");
            Console.WriteLine("--email email            waveface user email");
            Console.WriteLine("--password pwd           waveface user password");

            Console.WriteLine("--threads n              upload thread numbers. default = 1");
            Console.WriteLine("--uploads-per-thread n   upload count per thread. default = 100");
            Console.WriteLine("--station ip_or_host     station's ip or hostname. default = 127.0.0.1");

            Console.WriteLine();
        }

        static byte[] LoadImage(string filename)
        {
            MemoryStream m = new MemoryStream();
            StreamHelper.Copy(File.OpenRead(filename), m);
            return m.ToArray();
        }
    }


    class Arguments
    {
        NameValueCollection parameters = new NameValueCollection();

        public Arguments(string[] args)
        {
            int i = 0;
            while (i < args.Length)
            {
                if (args[i].StartsWith("--"))
                {
                    if (i + 1 < args.Length && !args[i + 1].StartsWith("--"))
                    {
                        parameters[args[i]] = args[i + 1];
                        ++i;
                    }
                    else
                    {
                        parameters[args[i]] = "";
                    }
                }
                else
                {
                    throw new ArgumentException("invalid argument: " + args[i]);
                }

                ++i;
            }
        }

        public string this[string key]
        {
            get { return parameters[key]; }
        }

        public string[] AllKeys
        {
            get { return parameters.AllKeys; }
        }
    }
}
