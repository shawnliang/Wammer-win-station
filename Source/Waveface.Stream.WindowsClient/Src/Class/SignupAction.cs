using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Waveface.Stream.Core;
using Waveface.Stream.Model;
using Newtonsoft.Json;
using Waveface.Stream.ClientFramework;
using System.IO;

namespace Waveface.Stream.WindowsClient
{
	class StreamSignup : ISignupAction
	{
		public void SignUpWithEmail(string email, string password, string name)
		{
			var device_id = StationRegistry.GetValue("stationId", string.Empty).ToString();
			var device_name = Environment.MachineName;

			StationAPI.UserSignup(email, password, name, device_id, device_name);

			var userFolder = prepareUserFolderPath();

			StationAPI.AddUser(email, password, device_id, device_name, userFolder);
			StreamClient.Instance.Login(email, password);
		}

		public void SignUpWithFacebook()
		{
			var fbLogin = new StreamLogin();
			fbLogin.LoginWithFacebook();
		}

		private static string prepareUserFolderPath()
		{
			var userFolder = Path.Combine(Environment.GetEnvironmentVariable("USERPROFILE"), "AOStream");
			if (!Directory.Exists(userFolder))
				Directory.CreateDirectory(userFolder);
			return userFolder;
		}

	}
}
