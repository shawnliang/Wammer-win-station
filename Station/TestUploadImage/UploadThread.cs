using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.IO;
using System.Threading;
using Wammer.Model;

namespace TestUploadImage
{
	class UploadThread
	{
		byte[] uploadData;
		int upload_count;
		ManualResetEvent start_event;
		Thread thread;
		string station;

		public int error_count { get; private set; }
		public int success_count { get; private set; }
		public TimeSpan total_duration { get; private set; }

		public UploadThread(int upload_count, byte[] uploadData, ManualResetEvent start_event, string station)
		{
			this.uploadData = uploadData;
			this.upload_count = upload_count;
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

			for (int i = 0; i < upload_count; i++)
			{
				DateTime start = DateTime.Now;

				try
				{
					HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://" + station + ":9981/v2/attachments/upload/");
					request.Method = "POST";
					request.ContentType = "multipart/form-data; boundary=--abcdefgABCDEFG";

					using (BinaryWriter w = new BinaryWriter(request.GetRequestStream()))
					{
						w.Write(this.uploadData);
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
				catch (Exception e)
				{
					error_count++;
					Console.WriteLine("Image upload failed: " + e.Message + " at thread " + Thread.CurrentThread.ManagedThreadId);
				}
			}
		}
	}
}
