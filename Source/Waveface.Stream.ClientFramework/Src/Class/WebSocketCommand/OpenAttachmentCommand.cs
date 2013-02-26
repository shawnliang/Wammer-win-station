using AutoMapper;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Waveface.Stream.ClientFramework.Properties;
using Waveface.Stream.Core;
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
		public override Dictionary<string, Object> Execute(WebSocketCommandData data, Dictionary<string, Object> systemArgs = null)
		{
			if (!StreamClient.Instance.IsLogined)
				return null;

			var parameters = data.Parameters;

			var attachmentID = parameters.ContainsKey("attachment_id") ? parameters["attachment_id"].ToString() : string.Empty;

			if (attachmentID.Length == 0)
				return null;

			var file = string.Empty;
			var attachment = AttachmentCollection.Instance.FindOne(Query.EQ("_id", attachmentID));

			if (attachment != null)
			{
				var attachmentData = Mapper.Map<Attachment, MediumSizeAttachmentData>(attachment);

				file = attachmentData.Url;
			}

			Debug.Assert(!string.IsNullOrEmpty(file));
			if (string.IsNullOrEmpty(file))
				return null;

			Debug.Assert(File.Exists(file));
			if (!File.Exists(file))
			{
				SynchronizationContextHelper.SendMainSyncContext(() =>
				{
					MessageBox.Show(Resources.FILE_NOT_EXISTS);
				});

				return null;
			}

			if (string.IsNullOrEmpty(FileHelper.GetAssociatedExeFile(file, false)))
			{
				SynchronizationContextHelper.SendMainSyncContext(() =>
				{
					MessageBox.Show(Resources.WITHOUT_ASSOCIATED_EXE_FILE);
				});

				return null;
			}

			Process.Start(file);
			return null;
		}
		#endregion
	}
}


