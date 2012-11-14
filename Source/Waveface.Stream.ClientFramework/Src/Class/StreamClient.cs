using System;
using Newtonsoft.Json.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using Waveface.Stream.Model;

namespace Waveface.Stream.ClientFramework
{
	public class StreamClient
	{
		#region Public Const
		public const string CLIENT_API_KEY = @"a23f9491-ba70-5075-b625-b8fb5d9ecd90";
		#endregion


		#region Private Const
		private const string APP_NAME = "Stream";
		private const string STATION_ID_KEY = "stationId";
		private const string API_URL = "http://localhost:9981/v2/";
		#endregion

		#region Static Var
		private static StreamClient _instance;
		#endregion


		#region Static Public Property
		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static StreamClient Instance
		{
			get
			{
				return _instance ?? (_instance = new StreamClient());
			}
		}
		#endregion


		#region Public Property
		//TODO: 考慮多個USER登入狀況，目前只是先寫成一個讓Function work
		public String SessionToken { get; private set; }
		#endregion



		#region Constructor
		private StreamClient()
		{

		}
		#endregion


		#region Public Method
		/// <summary>
		/// Logins the specified email.
		/// </summary>
		/// <param name="email">The email.</param>
		/// <param name="password">The password.</param>
		/// <returns></returns>
		public Boolean Login(string email, string password)
		{
			//TODO: 將API切出...
			try
			{
				var response = StationAPI.Login(
					email,
					password,
					(string)StationRegistry.GetValue(STATION_ID_KEY, string.Empty),
					Environment.MachineName);

				var jObject = JObject.Parse(response);
				SessionToken = jObject["session_token"].ToString();

				return true;
			}
			catch
			{
				return false;
			}
		} 
		#endregion
	}
}
