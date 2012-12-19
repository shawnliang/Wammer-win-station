using AutoMapper;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Reflection;
using Waveface.Stream.Model;

namespace Waveface.Stream.Core
{
	[Obfuscation]
	public class GetUserInfoCommand : WebSocketCommandBase
	{
		#region Public Property
		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>The name.</value>
		public override string Name
		{
			get { return "getUserInfo"; }
		}
		#endregion


		#region Public Method
		/// <summary>
		/// Executes the specified parameters.
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public override Dictionary<string, Object> Execute(WebSocketCommandData data, Dictionary<string, Object> systemArgs = null)
		{
			var parameters = data.Parameters;

			var sessionToken = parameters.ContainsKey("session_token") ? parameters["session_token"].ToString() : string.Empty;

			if (sessionToken.Length == 0)
				return null;

			var loginedUser = LoginedSessionCollection.Instance.FindOneById(sessionToken);

			if (loginedUser == null)
				return null;

			var userData = Mapper.Map<LoginedSession, UserData>(loginedUser);

			return new Dictionary<string, Object>() 
            {
                {"nickname", userData.NickName},
                {"email", userData.Email},
                {"session_token", userData.SessionToken},
                {"devices", userData.Devices},
            };
		}
		#endregion
	}
}
