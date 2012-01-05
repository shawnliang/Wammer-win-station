using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.IO;
using System.Threading;
using System.Web;
using Wammer.Model;
using Wammer.Cloud;

namespace TestGetLatestPost
{
	class GetLatestThread
	{
		User user;
		int limit;
		int repeat;
		ManualResetEvent start_event;
		Thread thread;
		string station;

		public int error_count { get; private set; }
		public int success_count { get; private set; }
		public TimeSpan total_duration { get; private set; }

		public GetLatestThread(User user, int repeat, int limit, ManualResetEvent start_event, string station)
		{
			this.user = user;
			this.limit = limit;
			this.repeat = repeat;
			this.start_event = start_event;
			this.station = station;

			total_duration = TimeSpan.FromTicks(0);
		}

		public void Start()
		{
			thread = new Thread(this.Do);
			thread.Start();
		}

		public void Join()
		{
			thread.Join();
		}

		private void Do()
		{
			if (!start_event.WaitOne())
				throw new InvalidOperationException("start_event is not waitable");

			try
			{
				for (int i = 0; i < repeat; i++)
				{
					DateTime start = DateTime.Now;

					HttpWebRequest request;
					if (station == "develop.waveface.com")
					{
						request = (HttpWebRequest)WebRequest.Create(
							"https://" + station + "/v2/posts/getLatest" + "?" +
							"apikey" + "=" + "a23f9491-ba70-5075-b625-b8fb5d9ecd90" + "&" +
							"session_token" + "=" + HttpUtility.UrlEncode(user.Token) + "&" +
							"group_id" + "=" + user.Groups[0].group_id + "&" +
							"limit" + "=" + limit
						);
					}
					else
					{
						request = (HttpWebRequest)WebRequest.Create(
							"http://" + station + ":9981/v2/posts/getLatest" + "?" +
							"apikey" + "=" + "a23f9491-ba70-5075-b625-b8fb5d9ecd90" + "&" +
							"session_token" + "=" + HttpUtility.UrlEncode(user.Token) + "&" +
							"group_id" + "=" + user.Groups[0].group_id + "&" +
							"limit" + "=" + limit
						);
					}

					HttpWebResponse response = (HttpWebResponse)request.GetResponse();

					using (StreamReader r = new StreamReader(response.GetResponseStream()))
					{
						string resText = r.ReadToEnd();
						//Console.WriteLine(response.Headers.ToString());
					}

					DateTime stop = DateTime.Now;
					total_duration += stop - start;
					success_count++;
				}
			}
			catch (Exception e)
			{
				error_count++;
				Console.WriteLine("GetLatestPost failed: " + e.Message + " at thread " + Thread.CurrentThread.ManagedThreadId);
			}
		}
	}
}
