using AutoMapper;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Reflection;
using Waveface.Stream.Model;

namespace Waveface.Stream.ClientFramework
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
		public override Dictionary<string, Object> Execute(WebSocketCommandData data)
		{
			var parameters = data.Parameters;

			var sessionToken = StreamClient.Instance.LoginedUser.SessionToken;
			var loginedSession = LoginedSessionCollection.Instance.FindOne(Query.EQ("_id", sessionToken));

			if (loginedSession == null)
				return null;

			var userData = Mapper.Map<LoginedSession, UserData>(loginedSession);

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
