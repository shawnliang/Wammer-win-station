using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.IO;
using System.Threading;
using Wammer.Utility;
using Wammer.Cloud;
using Wammer.MultiPart;

namespace TestAttachmentView
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				Arguments arguments = new Arguments(args);

				int threads = (arguments["--threads"] != null) ? int.Parse(arguments["--threads"]) : 1;
				int repeat = (arguments["--repeat"] != null) ? int.Parse(arguments["--repeat"]) : 1;
				string email = arguments["--email"];
				string password = arguments["--password"];
				string station = arguments["--station"];
				string object_id = arguments["--object_id"];
				string image_meta = arguments["--image_meta"];


				if (email == null)
					throw new ArgumentException("--email is not specified");
				if (password == null)
					throw new ArgumentException("--password is not specified");

				if (station == null)
					station = "develop.waveface.com";

				User user = User.LogIn(new WebClient(), email, password);

				ManualResetEvent start_event = new ManualResetEvent(false);

				PingWFCloud();

				// Start threads to upload images
				List<AttachmentViewThread> workers = new List<AttachmentViewThread>();
				for (int i = 0; i < threads; i++)
				{
					AttachmentViewThread thread = new AttachmentViewThread(user, object_id, image_meta, repeat, start_event, station);
					workers.Add(thread);
					thread.Start();
				}

				start_event.Set();


				// Waiting upload complete
				for (int i = 0; i < threads; i++)
				{
					workers[i].Join();
				}

				PingWFCloud();

				
				// Compute statistics
				int total_success = 0;
				int total_error = 0;
				TimeSpan total_duration_of_success = TimeSpan.FromTicks(0);
				TimeSpan max_duration = TimeSpan.FromSeconds(0);

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

					if (success_duration > max_duration)
						max_duration = success_duration;
				}

				Console.WriteLine("===================================");
				Console.WriteLine("total success: " + total_success);
				Console.WriteLine("total error: " + total_error);
				Console.WriteLine("total duration of success: " + total_duration_of_success.TotalMilliseconds + " ms");

				Console.WriteLine("Average response time: " + total_duration_of_success.TotalSeconds / total_success + " sec");
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
			Console.WriteLine("TestGetLatestPost usages:");
			Console.WriteLine();
			Console.WriteLine("--email email            waveface user email");
			Console.WriteLine("--password pwd           waveface user password");

			Console.WriteLine("--threads n              attachmentView thread numbers. default = 1");
			Console.WriteLine("--repeat n               number of request per thread. default = 1");
			Console.WriteLine("--station ip_or_host     station's ip or hostname. default = develop.waveface.com");
			Console.WriteLine("--object_id              attachment object id");
			Console.WriteLine("--image_meta             small/medium/large");

			Console.WriteLine();
		}

		static void PingWFCloud()
		{
			DateTime start = DateTime.Now;
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(
						"https://develop.waveface.com/v2/doc/api"
					);
			HttpWebResponse response = (HttpWebResponse)request.GetResponse();
			using (StreamReader r = new StreamReader(response.GetResponseStream()))
			{
				string resText = r.ReadToEnd();
			}
			TimeSpan resptime = DateTime.Now - start;
			Console.WriteLine("Ping response time: " + resptime.TotalMilliseconds + " ms");
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
