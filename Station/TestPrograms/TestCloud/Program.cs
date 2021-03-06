﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;


using Wammer.Cloud;
using Wammer.Model;


namespace TestCloud
{
	class Program
	{
		static void Main(string[] args)
		{
			string username = args[0];
			string password = args[1];
			string filepath = args[2];
			try
			{
				string apiKey = "e96546fa-3ed5-540a-9ef2-1f8ce1dc60f2";
				using (WebClient agent = new WebClient())
				{
					User user = User.LogIn(agent, username, password, apiKey);

					string thumbnail = filepath;

					using (FileStream s = File.OpenRead(thumbnail))
					{
						byte[] buffer = new byte[s.Length];
						int nread = s.Read(buffer, 0, (int)s.Length);
						if (nread != s.Length)
							throw new Exception("nread != s.Length");

						if (user.Groups.Count == 0)
							throw new InvalidDataException("User does not have a valid group");

						string thbnId = Guid.NewGuid().ToString();
						ObjectUploadResponse thbnResp = Attachment.UploadImage(
										"http://localhost:9981/v2/attachments/upload",
										buffer, user.Groups[0].group_id, null, 
										thbnId + ".jpeg", "image/jpeg",
										ImageMeta.Origin, apiKey, user.Token);

						Console.WriteLine("thumbnail uploaded: " + thbnId);

						ObjectUploadResponse origResp = Attachment.UploadImage(
										"http://localhost:9981/v2/attachments/upload",
										buffer, user.Groups[0].group_id, null, "big.jpeg",
										"image/jpeg", ImageMeta.Origin,
										apiKey, user.Token);

						Console.WriteLine("orig image uploaded: " + origResp.object_id);
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
