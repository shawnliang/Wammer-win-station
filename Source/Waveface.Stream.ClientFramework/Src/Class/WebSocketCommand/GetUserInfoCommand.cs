using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebSocketSharp.Server;
using System.Windows.Forms;
using System.Diagnostics;
using Waveface.Stream.Model;
using AutoMapper;

namespace Waveface.Stream.ClientFramework
{
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


        #region Constructor
        public GetUserInfoCommand()
        {
            Mapper.CreateMap<LoginedSession, UserData>()
                .ForMember(dest => dest.email, opt => opt.MapFrom(src => src.user.email))
                .ForMember(dest => dest.nickname, opt => opt.MapFrom(src => src.user.nickname))
                .ForMember(dest => dest.devices, opt => opt.MapFrom(src => src.user.devices));

            Mapper.CreateMap<Device, DeviceData>();
        }
        #endregion


        #region Public Method
        /// <summary>
		/// Executes the specified parameters.
		/// </summary>
		/// <param name="parameters">The parameters.</param>
        public override Dictionary<string, Object> Execute(Dictionary<string, Object> parameters = null)
		{
            var loginedSession = LoginedSessionCollection.Instance.FindOne();

            if (loginedSession == null)
                return null;

            var userData = Mapper.Map<LoginedSession, UserData>(loginedSession);

            return new Dictionary<string, Object>() 
            {
                {"nickname", userData.nickname},
                {"email", userData.email},
                {"session_token", userData.session_token},
                {"devices", userData.devices},
            };
        }
		#endregion
	}
}
