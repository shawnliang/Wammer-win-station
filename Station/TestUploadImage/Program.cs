using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

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

                if (image_file == null)
                    throw new ArgumentException("--image-file is not specified");




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
            Console.WriteLine("--image-file path        image file to upload.");
            Console.WriteLine("--threads n              upload thread numbers. default = 1");
            Console.WriteLine("--uploads-per-thread n   upload count per thread. default = 100");
            Console.WriteLine();
        }
    }


    class Arguments
    {
        NameValueCollection parameters;

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
