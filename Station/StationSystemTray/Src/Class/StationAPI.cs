using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Win32;
using System.Globalization;
using System.Web;
using System.Net;
using System.Collections.Specialized;

namespace StationSystemTray
{
	/// <summary>
	/// 
	/// </summary>
	public class StationAPI
	{
		#region Const
		public static string API_KEY = "a23f9491-ba70-5075-b625-b8fb5d9ecd90";
		#endregion


		#region Private Method
		/// <summary>
		/// Toes the query string.
		/// </summary>
		/// <param name="nvc">The NVC.</param>
		/// <returns></returns>
		private static string ToQueryString(NameValueCollection nvc)
		{
			return string.Join("&", Array.ConvertAll(nvc.AllKeys, key => string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(nvc[key]))));
		}
		#endregion


		#region Public Static Method
		public static void Import(string sessionToken, string groupID, string paths)
		{
			var url = @"http://127.0.0.1:9989/v2/station/Import";

			var parameters = new NameValueCollection() 
				{
					{"apikey", API_KEY},
					{"session_token", sessionToken},
					{"group_id", groupID},
					{"paths", paths}
				};

			var queryString = ToQueryString(parameters);
			var data = Encoding.Default.GetBytes(queryString);
			var request = WebRequest.Create(url) as HttpWebRequest;

			request.Method = "POST";
			request.ContentType = "application/x-www-form-urlencoded";
			request.ContentLength = data.Length;
			
			using(var requestStream = request.GetRequestStream())
			{
				requestStream.Write(data, 0, data.Length);
			}

			WebResponse response = request.GetResponse ();
            // Display the status.
            Console.WriteLine (((HttpWebResponse)response).StatusDescription);
            // Get the stream containing content returned by the server.
            var dataStream = response.GetResponseStream ();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader (dataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd ();
            // Display the content.
            Console.WriteLine (responseFromServer);
		}
		#endregion
	}
}
