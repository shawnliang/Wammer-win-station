using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Waveface.Stream.Model
{
	public class AttachmentCollection : DBCollection<Attachment>
	{
		#region Var
		private static AttachmentCollection _instance;
		#endregion


		#region Property
		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>
		/// The instance.
		/// </value>
		public static AttachmentCollection Instance
		{
			get
			{
				return _instance ?? (_instance = new AttachmentCollection());
			}
		}
		#endregion


		#region Constructor
		/// <summary>
		/// Prevents a default instance of the <see cref="AttachmentCollection" /> class from being created.
		/// </summary>
		private AttachmentCollection()
			: base("attachments")
		{
			EnsureIndex(new IndexKeysBuilder().Ascending("group_id"));
			EnsureIndex(new IndexKeysBuilder().Ascending("md5"));
		}
		#endregion

		public void UpdateWebThumbSavedFile(string object_id, long webthumb_id, string saved_file_name)
		{
			Instance.Update(
				Query.And(
					Query.EQ("_id", object_id),
					Query.EQ("web_meta.thumbs._id", webthumb_id)),
				MongoDB.Driver.Builders.Update.Set("web_meta.thumbs.$.saved_file_name", saved_file_name)
			);
		}
	}
}
