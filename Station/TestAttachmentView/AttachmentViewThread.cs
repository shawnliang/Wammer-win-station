using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.IO;
using System.Threading;
using System.Web;
using Wammer.Model;
using Wammer.Cloud;

namespace TestAttachmentView
{
	class AttachmentViewThread
	{
		User user;
		string object_id;
		string image_meta;
		int repeat;
		ManualResetEvent start_event;
		Thread thread;
		string station;

		public int error_count { get; private set; }
		public int success_count { get; private set; }
		public TimeSpan total_duration { get; private set; }

		public AttachmentViewThread(User user, string object_id, string image_meta, int repeat, ManualResetEvent start_event, string station)
		{
			this.user = user;
			this.object_id = object_id;
			this.image_meta = image_meta;
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
							"https://" + station + "/v2/attachments/view" + "?" +
							"apikey" + "=" + "0ffd0a63-65ef-512b-94c7-ab3b33117363" + "&" +
							"session_token" + "=" + HttpUtility.UrlEncode(user.Token) + "&" +
							"object_id" + "=" + object_id + "&" +
							"image_meta" + "=" + image_meta
						);
					}
					else
					{
						request = (HttpWebRequest)WebRequest.Create(
							"http://" + station + ":9981/v2/attachments/view" + "?" +
							"apikey" + "=" + "0ffd0a63-65ef-512b-94c7-ab3b33117363" + "&" +
							"session_token" + "=" + HttpUtility.UrlEncode(user.Token) + "&" +
							"object_id" + "=" + object_id + "&" +
							"image_meta" + "=" + image_meta
						);
					}

					HttpWebResponse response = (HttpWebResponse)request.GetResponse();
					using (StreamReader r = new StreamReader(response.GetResponseStream()))
					{
						string resText = r.ReadToEnd();
					}

					DateTime stop = DateTime.Now;
					total_duration += stop - start;
					success_count++;
				}
			}
			catch (Exception e)
			{
				error_count++;
				Console.WriteLine("AttachmentView failed: " + e.Message + " at thread " + Thread.CurrentThread.ManagedThreadId);
			}
		}
	}
}
