using AutoMapper;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Waveface.Stream.Model;

namespace Waveface.Stream.ClientFramework
{
	[Obfuscation]
	public class OpenAttachmentCommand : WebSocketCommandBase
	{
		#region Public Property
		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>The name.</value>
		public override string Name
		{
			get { return "openAttachment"; }
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
			if (!StreamClient.Instance.IsLogined)
				return null;

			var parameters = data.Parameters;

			var loginedUser =  StreamClient.Instance.LoginedUser;
			var sessionToken = loginedUser.SessionToken;

			var userID = loginedUser.UserID;
			var groupID = loginedUser.GroupID;

			var attachmentID = parameters.ContainsKey("attachment_id") ? parameters["attachment_id"].ToString() : string.Empty;
			var file = parameters.ContainsKey("file") ? parameters["file"].ToString() : string.Empty;

			if (attachmentID.Length != 0)
			{
				var attachment = AttachmentCollection.Instance.FindOne(Query.EQ("_id", attachmentID));

				if (attachment != null)
				{
					var attachmentData = Mapper.Map<Attachment, MediumSizeAttachmentData>(attachment);

					file = (new Uri(attachmentData.Url)).LocalPath;
				}
			}

			if (string.IsNullOrEmpty(file))
				return null;

			if (string.IsNullOrEmpty(FileHelper.GetAssociatedExeFile(file, false)))
				return null;

			Process.Start(file);
			return null;
		}
		#endregion
	}
}


