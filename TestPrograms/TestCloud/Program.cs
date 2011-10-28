using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;


using Wammer.Cloud;


namespace TestCloud
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				using (WebClient agent = new WebClient())
				{
					User user = User.LogIn(agent, "tj.sheu@waveface.com", "tj");

					string thumbnail = @"C:\Users\shawnliang\bin\0dc93a4e-7625-46d4-89bc-c3dbe2c1fbd6.jpeg";

					using (FileStream s = File.OpenRead(thumbnail))
					{
						byte[] buffer = new byte[s.Length];
						int nread = s.Read(buffer, 0, (int)s.Length);
						if (nread != s.Length)
							throw new Exception("nread != s.Length");



						ObjectUploadResponse origResp = Attachment.UploadImage(
										buffer, null, "big.jpeg" , "image/jpeg", ImageMeta.Small);

						Console.WriteLine("orig image uploaded: " + origResp.object_id);


						string thbnId = Guid.NewGuid().ToString();
						ObjectUploadResponse thbnResp = Attachment.UploadImage(
										buffer, origResp.object_id, thbnId + ".jpeg", "image/jpeg",
										ImageMeta.Origin);

						Console.WriteLine("thumbnail uploaded: " + thbnId);
					}
				}
			}
			catch (WebException e)
			{
				Console.WriteLine(e.Message);

				if (e.Response == null)
					return;

				using (StreamReader r = new StreamReader(e.Response.GetResponseStream()))
				{
					Console.WriteLine(r.ReadToEnd());
				}

			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				
			}

			Console.ReadKey();
		}
	}
}
